namespace TextOn.Core.Compiling

open TextOn.Core.Tokenizing
open TextOn.Core.Parsing

type CompiledModule =
    {
        File                : string
        ImportedFiles       : ParsedImportDefinition list
        RequiredFunctions   : string list
        RequiredVariables   : string list
        RequiredAttributes  : string list
        Variables           : ParsedVariableDefinition list
        Attributes          : ParsedAttributeDefinition list
        PrivateFunctions    : ParsedFunctionDefinition list
        PublicFunctions     : ParsedFunctionDefinition list
        Errors              : ParseError list
        Warnings            : ParseError list
    }

[<RequireQualifiedAccess>]
module Compiler =
    let private makeParseError file line s e t =
        {   File = file
            Severity = Error
            LineNumber = line
            StartLocation = s
            EndLocation = e
            ErrorText = t }
    let private makeParseWarning file line s e t =
        {   File = file
            Severity = Error
            LineNumber = line
            StartLocation = s
            EndLocation = e
            ErrorText = t }
    /// Compile from lines into a module.
    let compile file lines =
        let parsedElements =
            lines
            |> Tokenizer.tokenize file
            |> List.map Parser.parseTokenSet
        let parseErrors = parsedElements |> List.collect (fst >> List.ofArray)
        let (imports, functions, attributes, variables) =
            parsedElements
            |> List.choose snd
            |> List.fold
                (fun (imports, functions, attributes, variables) element ->
                    match element.Result with
                    | ParsedImport importDef ->
                        importDef::imports, functions, attributes, variables
                    | ParsedFunction funcDef ->
                        imports, funcDef::functions, attributes, variables
                    | ParsedVariable varDef ->
                        imports, functions, attributes, varDef::variables
                    | ParsedAttribute attDef ->
                        imports, functions, attDef::attributes, variables)
                ([], [], [], [])
            |> fun (a,b,c,d) -> (a |> List.rev, b |> List.rev, c |> List.rev, d |> List.rev)
        let importDupeWarnings =
            imports
            |> List.groupBy (fun import -> import.ImportedFileName.ToLower())
            |> List.filter (fun (_, l) -> l.Length > 1)
            |> List.collect (fun (_, l) -> l |> List.map (fun i -> makeParseWarning file i.Line i.StartLocation i.EndLocation (sprintf "Duplicate import of file \"%s\"" i.ImportedFileName)))
        let functionDupeErrors =
            functions
            |> List.groupBy (fun funcDef -> funcDef.Name)
            |> List.filter (fun (_, l) -> l.Length > 1)
            |> List.collect (fun (_, l) -> l |> List.map (fun i -> makeParseError file i.StartLine 1 1 (sprintf "Duplicate definition of function @%s" i.Name)))
        let attributeDupeErrors =
            attributes
            |> List.groupBy (fun attDef -> attDef.Name)
            |> List.filter (fun (_, l) -> l.Length > 1)
            |> List.collect (fun (_, l) -> l |> List.map (fun i -> makeParseError file i.StartLine 1 1 (sprintf "Duplicate definition of attribute %%%s" i.Name)))
        let variableDupeErrors =
            variables
            |> List.groupBy (fun varDef -> varDef.Name)
            |> List.filter (fun (_, l) -> l.Length > 1)
            |> List.collect (fun (_, l) -> l |> List.map (fun i -> makeParseError file i.StartLine 1 1 (sprintf "Duplicate definition of variable $%s" i.Name)))
        let errors = (parseErrors |> List.filter (fun error -> error.Severity = Error)) @ functionDupeErrors @ attributeDupeErrors @ variableDupeErrors
        let warnings = (parseErrors |> List.filter (fun error -> error.Severity = Warning)) @ importDupeWarnings
        let knownFunctions = functions |> List.map (fun f -> f.Name) |> Set.ofList
        let knownAttributes = attributes |> List.map (fun f -> f.Name) |> Set.ofList
        let knownVariables = variables |> List.map (fun f -> f.Name) |> Set.ofList
        let requiredFunctions = functions |> List.collect (fun f -> f.Dependencies |> Array.choose (function | ParsedFunctionRef f -> Some f | _ -> None) |> List.ofArray) |> Set.ofList |> Set.toList |> List.filter (fun f -> knownFunctions |> Set.contains f |> not)
        let requiredVariables =
            (   (functions |> List.collect (fun f -> f.Dependencies |> Array.choose (function | ParsedVariableRef f -> Some f | _ -> None) |> List.ofArray))
                @
                (variables |> List.collect (fun f -> f.Dependencies |> Array.choose (function | ParsedVariableName f -> Some f | _ -> None) |> List.ofArray)))
            |> Set.ofList
            |> Set.toList
            |> List.filter (fun f -> knownVariables |> Set.contains f |> not)
        let requiredAttributes =
            (   (functions |> List.collect (fun f -> f.Dependencies |> Array.choose (function | ParsedAttributeRef f -> Some f | _ -> None) |> List.ofArray))
                @
                (variables |> List.collect (fun f -> f.Dependencies |> Array.choose (function | ParsedAttributeName f -> Some f | _ -> None) |> List.ofArray))
                @
                (attributes |> List.collect (fun f -> f.Dependencies |> Array.choose (function | ParsedAttributeName f -> Some f | _ -> None) |> List.ofArray)))
            |> Set.ofList
            |> Set.toList
            |> List.filter (fun f -> knownAttributes |> Set.contains f |> not)
        {
            File                = file
            ImportedFiles       = imports
            RequiredFunctions   = requiredFunctions
            RequiredVariables   = requiredVariables
            RequiredAttributes  = requiredAttributes
            Variables           = variables
            Attributes          = attributes
            PrivateFunctions    = functions |> List.filter (fun f -> f.IsPrivate)
            PublicFunctions     = functions |> List.filter (fun f -> not f.IsPrivate)
            Errors              = errors
            Warnings            = warnings
        }
