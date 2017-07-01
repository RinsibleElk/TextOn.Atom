[<RequireQualifiedAccess>]
module TextOn.Core.Tokenizer

/// Tokenize lines from a function.
/// Line number starts at 1.
let rec private tokenizeFunctionLines (lineNumber:int) (fileName:string) (lines:string list) =
    ()

/// Tokenize a file.
let internal tokenize (fileName:string) (lines:string list) : CategorizedAttributedTokenSet list =
    failwith ""
