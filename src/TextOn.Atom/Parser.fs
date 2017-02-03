namespace TextOn.Atom

open System
open System.Text.RegularExpressions

type ParseResult =
    | ParserErrors of ParseError[]
    | ParsedFunction of ParsedFunctionDefinition

type ParsedElement = {
    File : string
    Result : ParseResult }

[<RequireQualifiedAccess>]
module Parser =
    /// Do the context-specific parse.
    let parse (tokenSet:CategorizedAttributedTokenSet) : ParsedElement =
        match tokenSet.Category with
        | Category.CategorizedFuncDefinition ->
            let parsedFunc = FunctionDefinitionParser.parseFunction tokenSet
            let result =
                match parsedFunc.Tree with
                | ParseErrors errors -> ParserErrors errors
                | _ -> ParsedFunction parsedFunc
            {   File = tokenSet.File
                Result = result }
        | _ -> failwith ""
