namespace TextOn.Core

open System

/// All the tokens recognised by the TextOn language.
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
    with
        override this.ToString() =
            match this with
            | Att                       -> "Att"
            | Var                       -> "Var"
            | Func                      -> "Func"
            | Free                      -> "Free"
            | Break                     -> "Break"
            | Sequential                -> "Sequential"
            | Choice                    -> "Choice"
            | VariableName s            -> sprintf "VariableName %s" s
            | OpenBrace                 -> "OpenBrace"
            | CloseBrace                -> "CloseBrace"
            | OpenCurly                 -> "OpenCurly"
            | CloseCurly                -> "CloseCurly"
            | AttributeName s           -> sprintf "AttributeName %s" s
            | OpenBracket               -> "OpenBracket"
            | CloseBracket              -> "CloseBracket"
            | QuotedString s            -> sprintf "QuotedString %s" s
            | Or                        -> "Or"
            | And                       -> "And"
            | Equals                    -> "Equals"
            | NotEquals                 -> "NotEquals"
            | InvalidUnrecognised s     -> sprintf "InvalidUnrecognised %s" s
            | InvalidReservedToken s    -> sprintf "InvalidReservedToken %s" s
            | ChoiceSeparator           -> "ChoiceSeparator"
            | RawText s                 -> sprintf "RawText %s" s
            | FunctionName s            -> sprintf "FunctionName %s" s
            | Private                   -> "Private"
            | Include                   -> "Include"
            | Import                    -> "Import"

/// A token with its start and end columns. These start at column 1.
type internal AttributedToken =
    {
        TokenStartLocation : int
        TokenEndLocation : int
        Token : Token
    }
    with
        override this.ToString() =
            sprintf "{%s [%d-%d]}" (this.Token.ToString()) this.TokenStartLocation this.TokenEndLocation

/// A non-empty, comment-stripped, tokenized line, with its line number (starting at line 1).
type internal AttributedTokenizedLine =
    {
        LineNumber : int
        Tokens : AttributedToken list
    }
    with
        override this.ToString() =
            sprintf "[%s]@%d" (this.Tokens |> List.fold (fun a b -> (if a = "" then "" else (a + ";")) +  b.ToString()) "") this.LineNumber

/// The output for the LineCategorizer.
type internal Category =
    | CategorizedFuncDefinition
    | CategorizedVarDefinition
    | CategorizedAttDefinition
    | CategorizedImport
    | CategorizationError of string
    with
        override this.ToString() =
            match this with
            | CategorizedFuncDefinition -> "CategorizedFuncDefinition"
            | CategorizedVarDefinition -> "CategorizedVarDefinition"
            | CategorizedAttDefinition -> "CategorizedAttDefinition"
            | CategorizedImport -> "CategorizedImport"
            | CategorizationError s -> sprintf "CategorizationError %s" s

/// A block of tokens representing a function, variable, attribute, or import.
type internal CategorizedAttributedTokenSet =
    {
        Category : Category
        File : string
        StartLine : int
        EndLine : int
        Tokens : AttributedTokenizedLine list
    }
    with
        override this.ToString() =
            sprintf "{\n%s (%s - %d-%d) :%s\n}" (this.Category.ToString()) this.File this.StartLine this.EndLine (this.Tokens |> List.fold (fun a b -> a + "\n" + b.ToString()) "")
