namespace TextOn.Atom

open System

type ParseResult =
    | ParserErrors of ParseError[]
    | ParsedFunction of ParsedFunctionDefinition
    | ParsedVariable of ParsedVariableDefinition
    | ParsedAttribute of ParsedAttributeDefinition

type ParsedElement = {
    File : string
    Result : ParseResult }

[<RequireQualifiedAccess>]
module Parser =
    /// Do the context-specific parse.
    let parse (tokenSet:CategorizedAttributedTokenSet) : ParsedElement =
        match tokenSet.Category with
        | CategorizedFuncDefinition ->
            let parsedFunc = FunctionDefinitionParser.parseFunctionDefinition tokenSet
            let result =
                match parsedFunc.Tree with
                | ParseErrors errors -> ParserErrors errors
                | _ -> ParsedFunction parsedFunc
            {   File = tokenSet.File
                Result = result }
        | CategorizedVarDefinition ->
            let parsedVar = VariableDefinitionParser.parseVariableDefinition tokenSet
            let result =
                match parsedVar.Result with
                | ParsedVariableErrors errors -> ParserErrors errors
                | _ -> ParsedVariable parsedVar
            {   File = tokenSet.File
                Result = result }
        | CategorizedAttDefinition ->
            let parsedAtt = AttributeDefinitionParser.parseAttributeDefinition tokenSet
            let result =
                match parsedAtt.Result with
                | ParsedAttributeErrors errors -> ParserErrors errors
                | _ -> ParsedAttribute parsedAtt
            {   File = tokenSet.File
                Result = result }
        | CategorizedPreprocessorError
        | CategorizationError ->
            let parseErrors =
                tokenSet.Tokens
                |> List.toArray
                |> Array.collect
                    (fun l ->
                        l.Tokens
                        |> List.choose
                            (fun t ->
                                match t.Token with
                                | InvalidPreprocessorError(s) ->
                                    Some
                                        {   File = tokenSet.File
                                            LineNumber = l.LineNumber
                                            StartLocation = t.TokenStartLocation
                                            EndLocation = t.TokenEndLocation
                                            ErrorText = s }
                                | InvalidUnrecognised(s) ->
                                    Some
                                        {   File = tokenSet.File
                                            LineNumber = l.LineNumber
                                            StartLocation = t.TokenStartLocation
                                            EndLocation = t.TokenEndLocation
                                            ErrorText = "Unrecognised raw text outside of function: " + s }
                                | _ -> None)
                        |> List.toArray)
            {   File = tokenSet.File
                Result =
                    ParserErrors parseErrors }
