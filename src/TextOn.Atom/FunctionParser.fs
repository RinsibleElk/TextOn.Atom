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

type ParsedNode =
    | ParsedSentence of ParsedSentenceNode * ParsedCondition
    | ParsedFunctionInvocation of ParsedFunctionName * ParsedCondition
    | ParsedSeq of ParsedNode[] * ParsedCondition
    | ParsedChoice of ParsedNode[] * ParsedCondition
    | ParsedParagraphBreak of ParsedCondition

[<RequireQualifiedAccess>]
module FunctionParser =
    /// Parse the CategorizedAttributedTokenSet for a function definition into a tree.
    let parseFunction (tokenSet:CategorizedAttributedTokenSet) =

        ()
