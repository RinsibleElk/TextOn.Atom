namespace TextOn.Atom

open System
open System.Text.RegularExpressions

type ParseResult =
    | ParserErrors of ParseError[]
    | ParsedFunction of ParsedFunctionDefinition
    | ParsedVariable of ParsedVariableDefinition

type ParsedElement = {
    File : string
    Result : ParseResult }

[<RequireQualifiedAccess>]
module Parser =
    /// Do the context-specific parse.
    let parse (tokenSet:CategorizedAttributedTokenSet) : ParsedElement =
        match tokenSet.Category with
        | Category.CategorizedFuncDefinition ->
            let parsedFunc = FunctionDefinitionParser.parseFunctionDefinition tokenSet
            let result =
                match parsedFunc.Tree with
                | ParseErrors errors -> ParserErrors errors
                | _ -> ParsedFunction parsedFunc
            {   File = tokenSet.File
                Result = result }
        | Category.CategorizedVarDefinition ->
            let parsedVar = VariableDefinitionParser.parseVariableDefinition tokenSet
            let result =
                if parsedVar.HasErrors then
                    ()
                else
                    ()
            {   File = tokenSet.File
                Result = ParsedVariable(parsedVar) }
        | _ -> failwith ""
