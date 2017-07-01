[<RequireQualifiedAccess>]
module TextOn.Core.Tokenizer

//    {
//        Category : Category
//        File : string
//        StartLine : int
//        EndLine : int
//        Tokens : AttributedTokenizedLine list
//    }

type private TokenizerState =
    | Done
    | Outside of int
    | Function of int * int * int * AttributedTokenizedLine list

let rec private tokenizeInner fileName state lines output =
    let newState, newLines, newOutput =
        match state, lines with
        | Done, _
        | Outside _, [] -> Done, [], output
    if newState = Done then
        newOutput
    else
        tokenizeInner fileName newState newLines newOutput

/// Tokenize a file.
let internal tokenize (fileName:string) (lines:string list) : CategorizedAttributedTokenSet list =
    failwith ""
