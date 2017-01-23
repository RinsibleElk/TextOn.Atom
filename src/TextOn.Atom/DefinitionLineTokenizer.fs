namespace TextOn.Atom

open System
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
/// Take a line that has been determined to be within a function definition and tokenize it.
module DefinitionLineTokenizer =
    // The tokens to find are '{', '|', '}', '[', ']', '=', '"', '@', '%', '/'. Escape character is '\'.
    let tokenize () = ()
