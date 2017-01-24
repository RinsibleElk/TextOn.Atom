namespace TextOn.Atom

open System

type Token =
    | Att
    | Var
    | Func
    | Break
    | Seq
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
    | Star
    | InvalidUnrecognised
    | ChoiceSeparator
    | RawText of string
    | InvalidReservedToken of string
    | FunctionName of string

type AttributedToken =
    {
        StartIndex : int
        EndIndex : int
        Token : Token
    }
