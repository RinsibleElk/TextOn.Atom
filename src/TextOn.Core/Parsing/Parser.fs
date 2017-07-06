namespace TextOn.Core.Parsing

open TextOn.Core.Tokenizing

open System

type internal ParseResult =
    | ParsedImport of ParsedImportDefinition
    | ParsedFunction of ParsedFunctionDefinition
    | ParsedVariable of ParsedVariableDefinition
    | ParsedAttribute of ParsedAttributeDefinition

type internal ParsedElement = {
    File : string
    Result : ParseResult }

[<RequireQualifiedAccess>]
module internal Parser =
    /// Do the context-specific parse.
    let parseTokenSet (tokenSet:CategorizedAttributedTokenSet) : ParseError[] * ParsedElement option =
        match tokenSet.Category with
        | CategorizedImport ->
            let (errors, result) = ImportParser.parseImport tokenSet
            (errors,
                result
                |> Option.map
                    (fun result ->
                        {   File = tokenSet.File
                            Result = ParsedImport result }))
        | CategorizedFuncDefinition ->
            let (errors, result) = FunctionDefinitionParser.parseFunctionDefinition tokenSet
            (errors,
                result
                |> Option.map
                    (fun result ->
                        {   File = tokenSet.File
                            Result = ParsedFunction result }))
        | CategorizedVarDefinition ->
            let (errors, result) = VariableDefinitionParser.parseVariableDefinition tokenSet
            (errors,
                result
                |> Option.map
                    (fun result ->
                        {   File = tokenSet.File
                            Result = ParsedVariable result }))
        | CategorizedAttDefinition ->
            let (errors, result) = AttributeDefinitionParser.parseAttributeDefinition tokenSet
            (errors,
                result
                |> Option.map
                    (fun result ->
                        {   File = tokenSet.File
                            Result = ParsedAttribute result }))
        | CategorizationError globalError ->
            let parseErrors =
                tokenSet.Tokens
                |> List.collect
                    (fun l ->
                        l.Tokens
                        |> List.choose
                            (fun t ->
                                match t.Token with
                                | InvalidUnrecognised(s) ->
                                    Some
                                        {   File = tokenSet.File
                                            Severity = Error
                                            StartLine = l.LineNumber
                                            EndLine = l.LineNumber
                                            StartLocation = t.TokenStartLocation
                                            EndLocation = t.TokenEndLocation
                                            ErrorText = "Unrecognised raw text outside of function: " + s }
                                | _ -> None))
                |> fun l ->
                    let t = tokenSet.Tokens.[tokenSet.Tokens.Length - 1]
                    let lt = t.Tokens.[t.Tokens.Length - 1].TokenEndLocation
                    (   {
                            File = tokenSet.File
                            Severity = Error
                            StartLine = tokenSet.StartLine
                            EndLine = tokenSet.EndLine
                            StartLocation = tokenSet.Tokens.[0].Tokens.[0].TokenStartLocation
                            EndLocation = lt
                            ErrorText = globalError
                        } :: l)
                |> List.toArray
            (parseErrors, None)
