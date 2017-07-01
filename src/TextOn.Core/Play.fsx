type Token =
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

sprintf "%A" (VariableName "Hello")
