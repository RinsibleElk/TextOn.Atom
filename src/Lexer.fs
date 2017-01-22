namespace TextOn.Atom

type Token =
    | Error

type TokenizedLine =
    | PreprocessorError of PreprocessorError
    | PreprocessorWarning of PreprocessorWarning
    | Tokens of Token list

type TokenizedSourceLine = {
    Contents : TokenizedLine }

[<RequireQualifiedAccess>]
module Lexer =
    let private lexLine (preprocessedSourceLine:PreprocessedSourceLine) : TokenizedSourceLine =
        { Contents = PreprocessorError "" }
