namespace TextOn.Atom

open System
open System.Text.RegularExpressions

type ParsedAttributeName = string
type ParsedVariableName = string
type ParsedFunctionName = string

type ParsedSentenceNode =
    | ParsedStringValue of string
    | ParsedVariable of ParsedVariableName
    | ParsedSimpleChoice of ParsedSentenceNode[]
    | ParsedSimpleSeq of ParsedSentenceNode[]

type ParsedCondition =
    | ParsedUnconditional
    | ParsedOr of ParsedCondition * ParsedCondition
    | ParsedAnd of ParsedCondition * ParsedCondition
    | ParsedAreEqual of ParsedAttributeName * string
    | ParsedAreNotEqual of ParsedAttributeName * string

type ParseError = {
    LineNumber : int
    StartLocation : int
    EndLocation : int
    ErrorText : string }

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
    let rec private parseFunctionInner (tokens:AttributedTokenizedLine[]) =
        ()

    /// Parse the CategorizedAttributedTokenSet for a function definition into a tree.
    let parseFunction (tokenSet:CategorizedAttributedTokenSet) : ParsedFunctionDefinition =

        failwith ""
