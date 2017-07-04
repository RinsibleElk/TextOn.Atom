namespace TextOn.Core.Tokenizing

open System

/// All the tokens recognised by the TextOn language.
[<NoComparison>]
type internal Token =
    | Att
    | Var
    | Func
    | Free
    | Break
    | Sequential
    | Choice
    | VariableName of string
    | OpenBrace
    | CloseBrace
    | OpenCurly
    | CloseCurly
    | AttributeName of string
    | OpenBracket
    | CloseBracket
    | QuotedString of string
    | Or
    | And
    | Equals
    | NotEquals
    | InvalidUnrecognised of string
    | InvalidReservedToken of string
    | ChoiceSeparator
    | RawText of string
    | FunctionName of string
    | Private
    | Include
    | Import

/// A token with its start and end columns. These start at column 1.
[<NoComparison>]
type internal AttributedToken =
    {
        TokenStartLocation : int
        TokenEndLocation : int
        Token : Token
    }

/// A non-empty, comment-stripped, tokenized line, with its line number (starting at line 1).
[<NoComparison>]
type internal AttributedTokenizedLine =
    {
        LineNumber : int
        Tokens : AttributedToken list
    }

/// The output for the LineCategorizer.
[<NoComparison>]
type internal Category =
    | CategorizedFuncDefinition
    | CategorizedVarDefinition
    | CategorizedAttDefinition
    | CategorizedImport
    | CategorizationError of string

/// A block of tokens representing a function, variable, attribute, or import.
[<NoComparison>]
type internal CategorizedAttributedTokenSet =
    {
        Category : Category
        File : string
        StartLine : int
        EndLine : int
        Tokens : AttributedTokenizedLine list
    }
