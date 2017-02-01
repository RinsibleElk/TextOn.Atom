namespace TextOn.Atom

open System
open System.Text.RegularExpressions

type ParsedVariableName = string
type ParsedFunctionName = string

type ParsedSentenceNode =
    | ParsedStringValue of string
    | ParsedVariable of ParsedVariableName
    | ParsedSimpleChoice of ParsedSentenceNode[]
    | ParsedSimpleSeq of ParsedSentenceNode[]

type ParsedNode =
    | ParsedSentence of int * ParsedSentenceNode * ParsedCondition
    | ParsedFunctionInvocation of int * ParsedFunctionName * ParsedCondition
    | ParsedSeq of int * ParsedNode[] * ParsedCondition
    | ParsedChoice of int * ParsedNode[] * ParsedCondition
    | ParsedParagraphBreak of int * ParsedCondition
    | ParseErrors of ParseError[]

type ParsedFunctionDefinition = {
    File : string
    StartLine : int
    EndLine : int
    Index : int
    HasErrors : bool
    Name : ParsedFunctionName
    Tree : ParsedNode }

[<RequireQualifiedAccess>]
module internal FunctionDefinitionParser =

    let rec private parseFunctionLine pushNode (tokens:AttributedToken list) =
        failwith ""

    let rec private parseFunctionInner (tokens:AttributedTokenizedLine[]) =
        ()

    /// Parse the CategorizedAttributedTokenSet for a function definition into a tree.
    let parseFunction (tokenSet:CategorizedAttributedTokenSet) : ParsedFunctionDefinition =
        // First line(s) should contain @func @funcName {
        // Last line should just be }
        let (name, hasErrors, tree) =
            if tokenSet.Tokens.Length = 0 then
                failwith ""
            else
                failwith ""
        {   File = tokenSet.File
            StartLine = tokenSet.StartLine
            EndLine = tokenSet.EndLine
            Index = tokenSet.Index
            HasErrors = hasErrors
            Name = name
            Tree = tree }

