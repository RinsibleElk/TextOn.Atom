namespace TextOn.Core.Linking

open TextOn.Core.Compiling
open TextOn.Core.Parsing
open System.IO

[<RequireQualifiedAccess>]
module Linker =
    let private makeParseError file line s e t =
        {   File = file
            Severity = Error
            LineNumber = line
            StartLocation = s
            EndLocation = e
            ErrorText = t }

    let rec private linkInner modules template =
        match modules with
        | [] -> template
        | m::r ->
            template

    let private getNormalizedPath file =
        Path.GetFullPath(file).ToLower()

    let private getNormalizedImportPath rootFile importedFile =
        Path.Combine(FileInfo(rootFile).Directory.FullName, importedFile) |> getNormalizedPath

    let rec private findCircularReferenceErrors knownFiles fileReferencePath (currentFile:CompiledModule) =
        let currentNormalizedPath = currentFile.File |> getNormalizedPath
        let circularReference = fileReferencePath |> List.tryFind (fun (f, _) -> f = currentNormalizedPath)
        if circularReference.IsSome then
            let (file, import) = circularReference.Value |> snd
            [makeParseError file import.Line 1 1 (sprintf "Circular reference - import %s or one of its imports references file %s" import.ImportedFileName file)]
        else
            currentFile.ImportedFiles
            |> List.collect
                (fun i ->
                    let normalizedImport = getNormalizedImportPath currentFile.File i.ImportedFileName
                    let f = knownFiles |> Map.tryFind normalizedImport
                    if f.IsNone then []
                    else
                        findCircularReferenceErrors knownFiles ((currentNormalizedPath, (currentFile.File, i))::fileReferencePath) f.Value)

    let rec private findImportedModulesInner knownFiles knownImports rootNormalizedPath (currentFile:CompiledModule) =
        currentFile.ImportedFiles
        |> List.fold
            (fun knownImports import ->
                let importNormalizedPath = getNormalizedImportPath currentFile.File import.ImportedFileName
                if importNormalizedPath = rootNormalizedPath || knownImports |> Set.contains importNormalizedPath then
                    knownImports
                else
                    let knownImports = knownImports |> Set.add importNormalizedPath
                    let importedModule = knownFiles |> Map.tryFind importNormalizedPath
                    if importedModule.IsNone then knownImports
                    else
                        findImportedModulesInner knownFiles knownImports rootNormalizedPath importedModule.Value)
            knownImports

    let private findImportedModules knownFiles (rootFile:CompiledModule) =
        let rootNormalizedPath = rootFile.File |> getNormalizedPath
        findImportedModulesInner knownFiles Set.empty rootNormalizedPath rootFile
        |> Set.toList
        |> List.choose (fun f -> knownFiles |> Map.tryFind f)

    let rec private missingReferencesInCondition file missingAttributes missingVariables condition =
        match condition with
        | ParsedAnd(c1, c2) -> List.append (missingReferencesInCondition file missingAttributes missingVariables c1) (missingReferencesInCondition file missingAttributes missingVariables c2)
        | ParsedOr(c1, c2) -> List.append (missingReferencesInCondition file missingAttributes missingVariables c1) (missingReferencesInCondition file missingAttributes missingVariables c2)
        | ParsedAreNotEqual(l,s,e,v,_)
        | ParsedAreEqual(l,s,e,v,_) ->
            match v with
            | ParsedVariableName v ->
                if missingVariables |> Set.contains v then [makeParseError file l s e (sprintf "Unknown variable %s" v)] else []
            | ParsedAttributeName v ->
                if missingAttributes |> Set.contains v then [makeParseError file l s e (sprintf "Unknown attribute %s" v)] else []
        | _ -> []

    let rec private missingReferencesInSentence file line missingVariables node =
        match node with
        | ParsedSentenceNode.ParsedSimpleChoice l
        | ParsedSentenceNode.ParsedSimpleSeq l -> l |> List.ofArray |> List.collect (missingReferencesInSentence file line missingVariables)
        | ParsedSentenceNode.ParsedSimpleVariable (s,e,v) -> if missingVariables |> Set.contains v then [makeParseError file line s e (sprintf "Unknown variable %s" v)] else []
        | _ -> []

    let rec private missingReferencesInFunction file missingAttributes missingVariables missingFunctions node =
        match node with
        | ParsedFunctionInvocation (l,s,e,f) -> if missingFunctions |> Set.contains f then [makeParseError file l s e (sprintf "Unknown function %s" f)] else []
        | ParsedChoice (_, l)
        | ParsedSeq (_, l) ->
            l
            |> List.ofArray
            |> List.collect
                (fun (n, c) ->
                    let refsInFunction = missingReferencesInFunction file missingAttributes missingVariables missingFunctions n
                    let refsInCondition = missingReferencesInCondition file missingAttributes missingVariables c
                    List.append refsInFunction refsInCondition)
        | ParsedSentence(line, n) ->
            missingReferencesInSentence file line missingVariables n
        | _ -> []

    let private missingReferencesInVariable file missingAttributes missingVariables (variable:ParsedVariableDefinition) =
        variable.Result
        |> List.ofArray
        |> List.collect
            (fun value ->
                missingReferencesInCondition file missingAttributes missingVariables value.Condition.Condition )

    let private missingReferencesInAttribute file missingAttributes (attribute:ParsedAttributeDefinition) =
        attribute.Result
        |> List.ofArray
        |> List.collect
            (fun value ->
                missingReferencesInCondition file missingAttributes Set.empty value.Condition)

    /// Link together a set of compiled modules to make a template ready for passing errors back to the user or generating texts.
    let link (modules:CompiledModule list) : Template =
        // Missing any references?
        let knownFiles              = modules |> List.map (fun m -> Path.GetFullPath(m.File).ToLower(), m) |> Map.ofList
        let allReferences           =
            modules
            |> List.collect
                (fun m ->
                    m.ImportedFiles
                    |> List.map
                        (fun import ->
                            let importedFile = getNormalizedImportPath m.File import.ImportedFileName
                            (m, import, importedFile)))
        let unknownReferenceErrors  =
            allReferences
            |> List.filter (fun (m, i, f) -> knownFiles |> Map.containsKey f |> not)
            |> List.map (fun (m, i, f) -> makeParseError m.File i.Line i.StartLocation i.EndLocation (sprintf "Unable to find imported file \"%s\"" i.ImportedFileName))

        // Find any clashing public entities across different files.
        let publicFunctionClashErrors =
            modules
            |> List.collect (fun m -> m.PublicFunctions |> List.map (fun f -> (m.File, f)) |> List.groupBy (fun (_,f) -> f.Name) |> List.map (snd >> List.head))
            |> List.groupBy (fun (_,f) -> f.Name)
            |> List.filter (fun (_,l) -> l.Length > 1)
            |> List.collect (fun (_,l) -> l |> List.map (fun (file, f) -> makeParseError file f.StartLine 1 1 (sprintf "Duplicate function name %s" f.Name)))
        let attributeClashErrors =
            modules
            |> List.collect (fun m -> m.Attributes |> List.map (fun f -> (m.File, f)) |> List.groupBy (fun (_,f) -> f.Name) |> List.map (snd >> List.head))
            |> List.groupBy (fun (_,f) -> f.Name)
            |> List.filter (fun (_,l) -> l.Length > 1)
            |> List.collect (fun (_,l) -> l |> List.map (fun (file, f) -> makeParseError file f.StartLine 1 1 (sprintf "Duplicate attribute name %s" f.Name)))
        let variableClashErrors =
            modules
            |> List.collect (fun m -> m.Variables |> List.map (fun f -> (m.File, f)) |> List.groupBy (fun (_,f) -> f.Name) |> List.map (snd >> List.head))
            |> List.groupBy (fun (_,f) -> f.Name)
            |> List.filter (fun (_,l) -> l.Length > 1)
            |> List.collect (fun (_,l) -> l |> List.map (fun (file, f) -> makeParseError file f.StartLine 1 1 (sprintf "Duplicate variable name %s" f.Name)))

        // Any circular references?
        let circularReferenceErrors =
            modules
            |> List.collect (findCircularReferenceErrors knownFiles [])

        // Find any entities that are used but cannot be found in an import.
        let missingEntityErrors =
            modules
            |> List.collect
                (fun m ->
                    // Recursively find all of the imported modules.
                    let allImportedModules              = findImportedModules knownFiles m
                    let allPublicFunctions              = allImportedModules |> List.collect (fun m -> m.PublicFunctions |> List.map (fun f -> f.Name)) |> Set.ofList
                    let allVariables                    = allImportedModules |> List.collect (fun m -> m.Variables |> List.map (fun f -> f.Name)) |> Set.ofList
                    let allAttributes                   = allImportedModules |> List.collect (fun m -> m.Attributes |> List.map (fun f -> f.Name)) |> Set.ofList
                    let missingPublicFunctions          = m.RequiredFunctions |> List.filter (fun f -> allPublicFunctions |> Set.contains f |> not) |> Set.ofList
                    let missingVariables                = m.RequiredVariables |> List.filter (fun f -> allVariables |> Set.contains f |> not) |> Set.ofList
                    let missingAttributes               = m.RequiredAttributes |> List.filter (fun f -> allAttributes |> Set.contains f |> not) |> Set.ofList
                    if (missingPublicFunctions |> Set.isEmpty && missingVariables |> Set.isEmpty && missingAttributes |> Set.isEmpty) then []
                    else
                        let attributesToInspect = m.Attributes |> List.filter (fun a -> a.Dependencies |> Array.tryFind (function | ParsedAttributeName att -> missingAttributes |> Set.contains att | _ -> false) |> Option.isSome)
                        let variablesToInspect = m.Variables |> List.filter (fun a -> a.Dependencies |> Array.tryFind (function | ParsedAttributeName att -> missingAttributes |> Set.contains att | ParsedVariableName v -> missingVariables |> Set.contains v) |> Option.isSome)
                        let functionsToInspect =
                            List.append m.PrivateFunctions m.PublicFunctions
                            |> List.filter
                                (fun a ->
                                    a.Dependencies
                                    |> Array.tryFind
                                        (function   | ParsedAttributeRef att -> missingAttributes |> Set.contains att
                                                    | ParsedVariableRef v -> missingVariables |> Set.contains v
                                                    | ParsedFunctionRef f -> missingPublicFunctions |> Set.contains f)
                                    |> Option.isSome)
                        let functionMissingRefs =
                            functionsToInspect
                            |> List.collect
                                (fun f ->
                                    missingReferencesInFunction m.File missingAttributes missingVariables missingPublicFunctions f.Tree)
                        let attributeMissingRefs =
                            attributesToInspect
                            |> List.collect
                                (fun f ->
                                    missingReferencesInAttribute m.File missingAttributes f)
                        let variableeMissingRefs =
                            variablesToInspect
                            |> List.collect
                                (fun f ->
                                    missingReferencesInVariable m.File missingAttributes missingVariables f)
                        functionMissingRefs @ attributeMissingRefs @ variableeMissingRefs)

        // Report any errors in imported files to the parent.

        // This should be all the errors we can possibly encounter. Report them and bail if there are any.
        let allErrors = (modules |> List.collect (fun m -> m.Errors)) @ unknownReferenceErrors @ publicFunctionClashErrors @ attributeClashErrors @ variableClashErrors @ circularReferenceErrors @ missingEntityErrors
        let allWarnings = modules |> List.collect (fun m -> m.Warnings)
        if allErrors.Length > 0 then
            {
                Errors = allErrors
                Warnings = allWarnings
                Attributes = Map.empty
                Variables = Map.empty
                Functions = Map.empty
            }
        else
            failwith ""
