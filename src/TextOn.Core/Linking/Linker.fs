namespace TextOn.Core.Linking

open System.IO
open TextOn.Core.Compiling
open TextOn.Core.Parsing
open TextOn.Core.Conditions
open TextOn.Core.Utils

[<RequireQualifiedAccess>]
module Linker =
    type private Reference =
        | Public of string
        | Private of string * string
    let private makeParseError file line s e t =
        {   File = file
            Severity = Error
            StartLine = line
            EndLine = line
            StartLocation = s
            EndLocation = e
            ErrorText = t }

    let rec private findCircularReferenceErrors knownFiles fileReferencePath (currentFile:CompiledModule) =
        let currentNormalizedPath = currentFile.File |> Utils.getNormalizedPath
        let circularReference = fileReferencePath |> List.tryFind (fun (f, _) -> f = currentNormalizedPath)
        if circularReference.IsSome then
            let (file, import) = circularReference.Value |> snd
            [makeParseError file import.Line import.StartLocation import.EndLocation (sprintf "Circular reference - import %s or one of its imports references file %s" import.ImportedFileName file)]
        else
            currentFile.ImportedFiles
            |> List.collect
                (fun i ->
                    let normalizedImport = Utils.getNormalizedImportPath currentFile.File i.ImportedFileName
                    let f = knownFiles |> Map.tryFind normalizedImport
                    if f.IsNone then []
                    else
                        findCircularReferenceErrors knownFiles ((currentNormalizedPath, (currentFile.File, i))::fileReferencePath) f.Value)

    let rec private findInfiniteRecursionErrors knownFiles (functionReferencePath:(_ * (_ * ParsedFunctionDefinition)) list) (currentFile:CompiledModule) (currentFunction:ParsedFunctionDefinition) =
        let currentNormalizedPath = currentFile.File |> Utils.getNormalizedPath
        let circularReference = functionReferencePath |> List.tryFind (fun (normFile, (file, fn)) -> normFile = currentNormalizedPath && fn.Name = currentFunction.Name)
        if circularReference.IsSome then
            let (file, fn) = circularReference.Value |> snd
            [makeParseError file fn.StartLine 1 1 (sprintf "Infinite recursion within function %s" fn.Name)]
        else
            currentFunction.Dependencies
            |> List.ofArray
            |> List.collect
                (function
                    | ParsedFunctionRef f ->
                        let privfn = currentFile.PrivateFunctions |> List.tryFind (fun fn -> fn.Name = f)
                        let functions =
                            if privfn.IsSome then [(currentNormalizedPath, (currentFile, privfn.Value))]
                            else
                                let pubfn = currentFile.PublicFunctions |> List.tryFind (fun fn -> fn.Name = f)
                                if pubfn.IsSome then [(currentNormalizedPath, (currentFile, pubfn.Value))]
                                else
                                    currentFile.ImportedFiles
                                    |> List.choose
                                        (fun import ->
                                            let normFile = Utils.getNormalizedImportPath currentFile.File import.ImportedFileName
                                            let file = knownFiles |> Map.tryFind normFile
                                            file
                                            |> Option.bind
                                                (fun file ->
                                                    file.PublicFunctions
                                                    |> List.tryFind (fun fn -> fn.Name = f)
                                                    |> Option.map (fun fn -> (normFile, (file, fn)))))
                        functions
                        |> List.collect
                            (fun (normFile, (file, fn)) ->
                                findInfiniteRecursionErrors knownFiles ((currentNormalizedPath, (currentFile.File, currentFunction))::functionReferencePath) file fn)
                    | _ -> [])

    let rec private findVariableDependencyCircularReferencErrors  knownFiles (variableReferencePath:(_ * (_ * ParsedVariableDefinition)) list) (currentFile:CompiledModule) (currentVariable:ParsedVariableDefinition) =
        let currentNormalizedPath = currentFile.File |> Utils.getNormalizedPath
        let circularReference = variableReferencePath |> List.tryFind (fun (normFile, (file, fn)) -> normFile = currentNormalizedPath && fn.Name = currentVariable.Name)
        if circularReference.IsSome then
            let (file, fn) = circularReference.Value |> snd
            [makeParseError file fn.StartLine 1 1 (sprintf "Circular reference within variable %s" fn.Name)]
        else
            currentVariable.Dependencies
            |> List.ofArray
            |> List.collect
                (function
                    | ParsedVariableName f ->
                        let v = currentFile.Variables |> List.tryFind (fun fn -> fn.Name = f)
                        let variables =
                            if v.IsSome then [(currentNormalizedPath, (currentFile, v.Value))]
                            else
                                currentFile.ImportedFiles
                                |> List.choose
                                    (fun import ->
                                        let normFile = Utils.getNormalizedImportPath currentFile.File import.ImportedFileName
                                        let file = knownFiles |> Map.tryFind normFile
                                        file
                                        |> Option.bind
                                            (fun file ->
                                                file.Variables
                                                |> List.tryFind (fun fn -> fn.Name = f)
                                                |> Option.map (fun fn -> (normFile, (file, fn)))))
                        variables
                        |> List.collect
                            (fun (normFile, (file, fn)) ->
                                findVariableDependencyCircularReferencErrors knownFiles ((currentNormalizedPath, (currentFile.File, currentVariable))::variableReferencePath) file fn)
                    | _ -> [])

    let rec private findAttributeDependencyCircularReferencErrors  knownFiles (attributeReferencePath:(_ * (_ * ParsedAttributeDefinition)) list) (currentFile:CompiledModule) (currentAttribute:ParsedAttributeDefinition) =
        let currentNormalizedPath = currentFile.File |> Utils.getNormalizedPath
        let circularReference = attributeReferencePath |> List.tryFind (fun (normFile, (file, fn)) -> normFile = currentNormalizedPath && fn.Name = currentAttribute.Name)
        if circularReference.IsSome then
            let (file, fn) = circularReference.Value |> snd
            [makeParseError file fn.StartLine 1 1 (sprintf "Circular reference within attribute %s" fn.Name)]
        else
            currentAttribute.Dependencies
            |> List.ofArray
            |> List.collect
                (function
                    | ParsedAttributeName f ->
                        let v = currentFile.Attributes |> List.tryFind (fun fn -> fn.Name = f)
                        let attributes =
                            if v.IsSome then [(currentNormalizedPath, (currentFile, v.Value))]
                            else
                                currentFile.ImportedFiles
                                |> List.choose
                                    (fun import ->
                                        let normFile = Utils.getNormalizedImportPath currentFile.File import.ImportedFileName
                                        let file = knownFiles |> Map.tryFind normFile
                                        file
                                        |> Option.bind
                                            (fun file ->
                                                file.Attributes
                                                |> List.tryFind (fun fn -> fn.Name = f)
                                                |> Option.map (fun fn -> (normFile, (file, fn)))))
                        attributes
                        |> List.collect
                            (fun (normFile, (file, fn)) ->
                                findAttributeDependencyCircularReferencErrors knownFiles ((currentNormalizedPath, (currentFile.File, currentAttribute))::attributeReferencePath) file fn)
                    | _ -> [])

    let rec private findImportedModulesInner knownFiles knownImports rootNormalizedPath (currentFile:CompiledModule) =
        currentFile.ImportedFiles
        |> List.fold
            (fun knownImports import ->
                let importNormalizedPath = Utils.getNormalizedImportPath currentFile.File import.ImportedFileName
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
        let rootNormalizedPath = rootFile.File |> Utils.getNormalizedPath
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

    let rec private getImports allModules (m:CompiledModule) =
        let directImports =
            m.ImportedFiles
            |> List.map
                (fun i ->
                    let f = (Utils.getNormalizedImportPath m.File i.ImportedFileName)
                    f, allModules |> Map.find f)
        let recursiveImports = directImports |> List.collect (snd >> getImports allModules)
        recursiveImports @ directImports

    let private cache<'k, 'a when 'k : comparison> (f:'k -> 'a) =
        let mutable d = Map.empty
        fun k ->
            let o = d |> Map.tryFind k
            if o.IsNone then
                let r = f k
                d <- d |> Map.add k r
                r
            else
                o.Value

    let private cache2<'k1, 'k2, 'a when 'k1 : comparison and 'k2 : comparison> (f:'k1 -> 'k2 -> 'a) =
        let mutable d = Map.empty
        fun k1 k2 ->
            let k = (k1, k2)
            let o = d |> Map.tryFind k
            if o.IsNone then
                let r = f k1 k2
                d <- d |> Map.add k r
                r
            else
                o.Value

    let rec private buildCondition getAttributeIndex file condition =
        match condition with
        | ParsedConditionError e -> failwithf "Parse errors not expected when building a condition %A" e
        | ParsedAnd (c1, c2) -> Both(buildCondition getAttributeIndex file c1, buildCondition getAttributeIndex file c2)
        | ParsedOr (c1, c2) -> Either(buildCondition getAttributeIndex file c1, buildCondition getAttributeIndex file c2)
        | ParsedAreEqual (_, _, _, a, v) ->
            match a with
            | ParsedAttributeName a -> AreEqual(getAttributeIndex a, v)
            | ParsedVariableName v -> failwithf "Invalid variable reference to %s v in condition in file %s" v file
        | ParsedAreNotEqual (_, _, _, a, v) ->
            match a with
            | ParsedAttributeName a -> AreNotEqual(getAttributeIndex a, v)
            | ParsedVariableName v -> failwithf "Invalid variable reference to %s v in condition in file %s" v file
        | ParsedUnconditional -> True

    let rec private buildVariableCondition getVariableIndex getAttributeIndex file (condition:ParsedCondition) : VariableCondition =
        match condition with
        | ParsedConditionError e -> failwithf "Parse errors not expected when building a condition %A" e
        | ParsedAnd (c1, c2) -> VarBoth(buildVariableCondition getVariableIndex getAttributeIndex file c1, buildVariableCondition getVariableIndex getAttributeIndex file c2)
        | ParsedOr (c1, c2) -> VarEither(buildVariableCondition getVariableIndex getAttributeIndex file c1, buildVariableCondition getVariableIndex getAttributeIndex file c2)
        | ParsedAreEqual (_, _, _, a, v) ->
            match a with
            | ParsedAttributeName a -> VarAreEqual(Attribute (getAttributeIndex a), v)
            | ParsedVariableName a -> VarAreEqual(Variable (getVariableIndex a), v)
        | ParsedAreNotEqual (_, _, _, a, v) ->
            match a with
            | ParsedAttributeName a -> VarAreNotEqual(Attribute (getAttributeIndex a), v)
            | ParsedVariableName a -> VarAreNotEqual(Variable (getVariableIndex a), v)
        | ParsedUnconditional -> VarTrue

    let rec private buildSentenceNode getVariableIndex file sentenceNode =
        match sentenceNode with
        | ParsedSentenceErrors e -> failwithf "Parse errors not expected when building a sentence %A in file %s" e file
        | ParsedSimpleChoice c ->
            c
            |> List.ofArray
            |> List.map (buildSentenceNode getVariableIndex file)
            |> SimpleChoice
        | ParsedSimpleSeq c ->
            c
            |> List.ofArray
            |> List.map (buildSentenceNode getVariableIndex file)
            |> SimpleSeq
        | ParsedSimpleVariable (_, _, v) -> VariableValue (getVariableIndex v)
        | ParsedStringValue s -> SimpleText s

    let rec private buildFunctionNode getFunctionIndex getVariableIndex getAttributeIndex file (node:ParsedNode) =
        match node with
        | ParsedChoice (lineNumber, choices) ->
            choices
            |> List.ofArray
            |> List.map (fun (n, c) -> (buildFunctionNode getFunctionIndex getVariableIndex getAttributeIndex file n), (buildCondition getAttributeIndex file c))
            |> fun c -> Choice(file, lineNumber, c)
        | ParsedSeq (lineNumber, choices) ->
            choices
            |> List.ofArray
            |> List.map (fun (n, c) -> (buildFunctionNode getFunctionIndex getVariableIndex getAttributeIndex file n), (buildCondition getAttributeIndex file c))
            |> fun c -> Seq(file, lineNumber, c)
        | ParsedFunctionInvocation(lineNumber, _, _, name) -> Function(file, lineNumber, getFunctionIndex name)
        | ParsedParagraphBreak lineNumber -> ParagraphBreak(file, lineNumber)
        | ParsedSentence(lineNumber, sentenceNode) ->
            Sentence(file, lineNumber, buildSentenceNode getVariableIndex file sentenceNode)
        | ParseErrors e -> failwithf "Parse errors not expected when building a node %A" e

    let private buildFunction functionAttributeDependencies functionVariableDependencies functionFunctionDependencies variableAttributeDependencies variableVariableDependencies attributeDependencies getFunctionIndex getVariableIndex getAttributeIndex file (fn:ParsedFunctionDefinition) =
        let thisIndex = getFunctionIndex fn.Name
        let thisFunctionDependencies =
            fn.Dependencies
            |> List.ofArray
            |> List.collect
                (function
                    | ParsedFunctionRef f ->
                        let i = (getFunctionIndex f)
                        i::(functionFunctionDependencies |> Map.find i)
                    | _ -> [])
            |> List.distinct
            |> List.sort
        let thisVariableDependencies =
            fn.Dependencies
            |> List.ofArray
            |> List.collect
                (function
                    | ParsedFunctionRef f ->
                        let i = (getFunctionIndex f)
                        (functionVariableDependencies |> Map.find i)
                    | ParsedVariableRef f ->
                        let i = (getVariableIndex f)
                        i::(variableVariableDependencies |> Map.find i)
                    | _ -> [])
            |> List.distinct
            |> List.sort
        let thisAttributeDependencies =
            fn.Dependencies
            |> List.ofArray
            |> List.collect
                (function
                    | ParsedFunctionRef f ->
                        let i = (getFunctionIndex f)
                        (functionAttributeDependencies |> Map.find i)
                    | ParsedVariableRef f ->
                        let i = (getVariableIndex f)
                        (variableAttributeDependencies |> Map.find i)
                    | ParsedAttributeRef f ->
                        let i = (getAttributeIndex f)
                        i::(attributeDependencies |> Map.find i))
            |> List.distinct
            |> List.sort
        let a =
            {
                Name = fn.Name
                Index = thisIndex
                File = file
                IsPrivate = fn.IsPrivate
                StartLine = fn.StartLine
                EndLine = fn.EndLine
                FunctionDependencies = thisFunctionDependencies
                AttributeDependencies = thisAttributeDependencies
                VariableDependencies = thisVariableDependencies
                Tree = buildFunctionNode getFunctionIndex getVariableIndex getAttributeIndex file fn.Tree
            }
        (functionAttributeDependencies |> Map.add thisIndex thisAttributeDependencies, functionVariableDependencies |> Map.add thisIndex thisVariableDependencies, functionFunctionDependencies |> Map.add thisIndex thisFunctionDependencies, Some a)

    let private buildVariableValue getVariableIndex getAttributeIndex file (s:ParsedVariableSuggestedValue) : VariableValue =
        {
            Value = s.Value
            Condition = buildVariableCondition getVariableIndex getAttributeIndex file s.Condition.Condition
        }

    let private buildVariable variableAttributeDependencies variableVariableDependencies attributeDependencies getVariableIndex getAttributeIndex file (a:ParsedVariableDefinition) : Map<int, int list> * Map<int, int list> * VariableDefinition option =
        let thisIndex = getVariableIndex a.Name
        let thisAttributeDependencies =
            a.Dependencies
            |> List.ofArray
            |> List.collect
                (function
                    | ParsedAttributeName a ->
                        let i = (getAttributeIndex a)
                        i::(attributeDependencies |> Map.find i)
                    | ParsedVariableName v ->
                        let i = (getVariableIndex v)
                        (variableAttributeDependencies |> Map.find i))
            |> List.distinct
            |> List.sort
        let thisVariableDependencies =
            a.Dependencies
            |> List.ofArray
            |> List.collect
                (function
                    | ParsedAttributeName a -> []
                    | ParsedVariableName v ->
                        let i = (getVariableIndex v)
                        i::(variableVariableDependencies |> Map.find i))
            |> List.distinct
            |> List.sort
        let v =
            {
                Name = a.Name
                Text = a.Text
                Index = thisIndex
                File = file
                StartLine = a.StartLine
                EndLine = a.EndLine
                PermitsFreeValue = a.SupportsFreeValue
                AttributeDependencies = thisAttributeDependencies
                VariableDependencies = thisVariableDependencies
                Values = a.Result |> List.ofArray |> List.map (buildVariableValue getVariableIndex getAttributeIndex file)
            }
        (variableAttributeDependencies |> Map.add thisIndex thisAttributeDependencies, variableVariableDependencies |> Map.add thisIndex thisVariableDependencies, Some v)

    let private buildAttributeValue getAttributeIndex file (value:ParsedAttributeValue) : AttributeValue =
        {
            Value = value.Value
            Condition = buildCondition getAttributeIndex file value.Condition
        }

    let private buildAttribute attributeDependencies getAttributeIndex file (a:ParsedAttributeDefinition) =
        let thisAttributeDependencies =
            a.Dependencies
            |> List.ofArray
            |> List.collect
                (function
                    | ParsedAttributeName a ->
                        let index = getAttributeIndex a
                        index::(attributeDependencies |> Map.find index)
                    | ParsedVariableName v -> failwithf "Unexpected reference to a variable %s in attribute %s" v a.Name)
            |> List.distinct
            |> List.sort
        let thisIndex = getAttributeIndex a.Name
        let a : AttributeDefinition=
            {
                Name = a.Name
                Text = a.Text
                Index = thisIndex
                File = file
                StartLine = a.StartLine
                EndLine = a.EndLine
                AttributeDependencies = thisAttributeDependencies
                Values = a.Result |> List.ofArray |> List.map (buildAttributeValue getAttributeIndex file)
            }
        (attributeDependencies |> Map.add thisIndex thisAttributeDependencies), Some a

    /// Link together a set of compiled modules to make a template ready for passing errors back to the user or generating texts.
    let link file (modules:CompiledModule list) : Template =
        // Missing any references?
        let knownFiles              = modules |> List.map (fun m -> Utils.getNormalizedPath m.File, m) |> Map.ofList
        let allReferences           =
            modules
            |> List.collect
                (fun m ->
                    m.ImportedFiles
                    |> List.map
                        (fun import ->
                            let importedFile = Utils.getNormalizedImportPath m.File import.ImportedFileName
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
            |> Set.ofList
            |> Set.toList

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

        // Find infinite recursion errors.
        let infiniteRecursionErrors =
            modules
            |> List.collect
                (fun m ->
                    (m.PrivateFunctions@m.PublicFunctions)
                    |> List.collect (fun fn -> findInfiniteRecursionErrors knownFiles [] m fn))
            |> Set.ofList
            |> Set.toList

        // Find variable dependency circular reference errors.
        let variableDependencyCircularReferencErrors =
            modules
            |> List.collect
                (fun m ->
                    m.Variables
                    |> List.collect (fun v -> findVariableDependencyCircularReferencErrors knownFiles [] m v))

        // Find variable dependency circular reference errors.
        let attributeDependencyCircularReferencErrors =
            modules
            |> List.collect
                (fun m ->
                    m.Attributes
                    |> List.collect (fun v -> findAttributeDependencyCircularReferencErrors knownFiles [] m v))

        // All the errors and warnings.
        let allErrors = (modules |> List.collect (fun m -> m.Errors)) @ unknownReferenceErrors @ publicFunctionClashErrors @ attributeClashErrors @ variableClashErrors @ circularReferenceErrors @ missingEntityErrors @ infiniteRecursionErrors @ variableDependencyCircularReferencErrors @ attributeDependencyCircularReferencErrors
        let allWarnings = modules |> List.collect (fun m -> m.Warnings)

        // This should be all the errors we can possibly encounter. Report them and bail if there are any.
        if allErrors.Length > 0 then
            {
                Errors = allErrors
                Warnings = allWarnings
                Attributes = []
                Variables = []
                Functions = []
            }
        else
            // Pick a sensible ordering for everything.
            let mainModule = modules |> List.find (fun m -> m.File = file)
            let imports =
                getImports knownFiles mainModule
                @
                [(Utils.getNormalizedPath mainModule.File, mainModule)]
            let allModules =
                imports
                |> List.scan
                    (fun (_, d) (s, m) ->
                        if d |> Set.contains s then (None, d)
                        else (Some m, d |> Set.add s))
                    (None, Set.empty)
                |> List.choose fst

            let allFunctions, allVariables, allAttributes =
                allModules
                |> List.fold
                    (fun (f, v, a) m -> (f@((m.PrivateFunctions @ m.PublicFunctions) |> List.map (fun f -> (m.File, f))), v@(m.Variables |> List.map (fun f -> (m.File, f))), a@(m.Attributes |> List.map (fun f -> (m.File, f)))))
                    ([],[],[])

            // Assign an index to everything.
            let functions =
                let mutable fs = allFunctions
                let mutable functions : (int * string * ParsedFunctionDefinition) list = []
                let mutable index = 0
                while fs |> List.isEmpty |> not do
                    // Find the first function that only depends on the ones we currently have.
                    let i =
                        fs
                        |> List.findIndex
                            (fun (file, f) ->
                                f.Dependencies
                                |> Array.choose (function | ParsedFunctionRef refFunc -> Some refFunc | _ -> None)
                                |> Array.tryFind
                                    (fun rf ->
                                        let inFs = fs |> List.exists (fun (file2, f2) -> f2.Name = rf && (file2 = file || (not f2.IsPrivate)))
                                        let inFunctions = functions |> List.exists (fun (_, file2, f2) -> f2.Name = rf && (file2 = file || (not f2.IsPrivate)))
                                        inFs && (not inFunctions))
                                |> Option.isNone)
                    let (file, f) = fs |> List.item i
                    fs <- (fs |> List.take i) @ (fs |> List.skip (min (i + 1) fs.Length))
                    functions <- (index, file, f)::functions
                    index <- index + 1
                functions |> List.rev
            let variables =
                let mutable fs = allVariables
                let mutable variables : (int * string * ParsedVariableDefinition) list = []
                let mutable index = 0
                while fs |> List.isEmpty |> not do
                    // Find the first variable that only depends on the ones we currently have.
                    let i =
                        fs
                        |> List.findIndex
                            (fun (file, f) ->
                                f.Dependencies
                                |> Array.choose (function | ParsedVariableName refFunc -> Some refFunc | _ -> None)
                                |> Array.tryFind
                                    (fun rf ->
                                        let inFs = fs |> List.exists (fun (file2, f2) -> f2.Name = rf)
                                        inFs)
                                |> Option.isNone)
                    let (file, f) = fs |> List.item i
                    fs <- (fs |> List.take i) @ (fs |> List.skip (min (i + 1) fs.Length))
                    variables <- (index, file, f)::variables
                    index <- index + 1
                variables |> List.rev
            let attributes =
                let mutable fs = allAttributes
                let mutable attributes : (int * string * ParsedAttributeDefinition) list = []
                let mutable index = 0
                while fs |> List.isEmpty |> not do
                    // Find the first attribute that only depends on the ones we currently have.
                    let i =
                        fs
                        |> List.findIndex
                            (fun (file, f) ->
                                f.Dependencies
                                |> Array.choose (function | ParsedAttributeName refFunc -> Some refFunc | _ -> None)
                                |> Array.tryFind
                                    (fun rf ->
                                        let inFs = fs |> List.exists (fun (file2, f2) -> f2.Name = rf)
                                        inFs)
                                |> Option.isNone)
                    let (file, f) = fs |> List.item i
                    fs <- (fs |> List.take i) @ (fs |> List.skip (min (i + 1) fs.Length))
                    attributes <- (index, file, f)::attributes
                    index <- index + 1
                attributes |> List.rev
            let getFunctionIndex =
                functions
                |> List.map
                    (fun (index, file, fn) ->
                        if fn.IsPrivate then (Private (file, fn.Name), index)
                        else (Public fn.Name), index)
                |> Map.ofList
                |> fun m ->
                    cache2
                        (fun file name ->
                            let priv = m |> Map.tryFind (Private(file, name))
                            if priv.IsSome then priv.Value
                            else
                                let pub = m |> Map.tryFind (Public(name))
                                if pub.IsSome then pub.Value
                                else failwithf "Could not find function %s referenced in file %s" name file)
            let getVariableIndex =
                variables
                |> List.map (fun (index, _, v) -> v.Name, index)
                |> Map.ofList
                |> fun m name ->
                    let o = m |> Map.tryFind name
                    if o.IsSome then o.Value
                    else failwithf "Could not find variable %s" name
            let getAttributeIndex =
                attributes
                |> List.map (fun (index, _, a) -> a.Name, index)
                |> Map.ofList
                |> fun m name ->
                    let o = m |> Map.tryFind name
                    if o.IsSome then o.Value
                    else failwithf "Could not find attribute %s" name
            let attributeDependencies, attributes =
                attributes
                |> List.scan (fun (attributeDependencies, _) (_, file, a) -> buildAttribute attributeDependencies getAttributeIndex file a) (Map.empty, None)
                |> fun l ->
                    let attributeDependencies = l |> List.last |> fst
                    let attributes = l |> List.choose snd
                    (attributeDependencies, attributes)
            let variableAttributeDependencies, variableVariableDependencies, variables =
                variables
                |> List.scan (fun (variableAttributeDependencies, variableVariableDependencies, _) (_, file, a) -> buildVariable variableAttributeDependencies variableVariableDependencies attributeDependencies getVariableIndex getAttributeIndex file a) (Map.empty, Map.empty, None)
                |> fun l ->
                    let variableAttributeDependencies, variableVariableDependencies = l |> List.last |> fun (a,b,_) -> (a,b)
                    let variables = l |> List.choose (fun (_,_,a) -> a)
                    variableAttributeDependencies, variableVariableDependencies, variables
            let functionAttributeDependencies, functionVariableDependencies, functionFunctionDependencies, functions =
                functions
                |> List.scan (fun (functionAttributeDependencies, functionVariableDependencies, functionFunctionDependencies, _) (_, file, a) -> buildFunction functionAttributeDependencies functionVariableDependencies functionFunctionDependencies variableAttributeDependencies variableVariableDependencies attributeDependencies (getFunctionIndex file) getVariableIndex getAttributeIndex file a) (Map.empty, Map.empty, Map.empty, None)
                |> fun l ->
                    let functionAttributeDependencies, functionVariableDependencies, functionFunctionDependencies = l |> List.last |> fun (a,b,c,_) -> (a,b,c)
                    let functions = l |> List.choose (fun (_,_,_,a) -> a)
                    functionAttributeDependencies, functionVariableDependencies, functionFunctionDependencies, functions
            {
                Errors = allErrors
                Warnings = allWarnings
                Attributes = attributes
                Variables = variables
                Functions = functions
            }
