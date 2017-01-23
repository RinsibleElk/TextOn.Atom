namespace TextOn.Atom

open System

type Token =
    | Att
    | Var
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
    | Unrecognised

type AttributedToken =
    {
        StartIndex : int
        EndIndex : int
        Token : Token
    }
