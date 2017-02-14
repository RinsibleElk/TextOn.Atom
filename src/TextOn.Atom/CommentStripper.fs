namespace TextOn.Atom

open System
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
/// Maps preprocessed source into preprocessed source having removed comments.
/// OPS: It's not obvious this is sensible prior to tokenizing. Note this basically assumes that comments are whole lines.
module CommentStripper =
    let private wholeLineCommentRegex = Regex("^\\s*//")

    /// Strip out all comment and blank lines from the preprocessed source.
    let stripComments (lines:PreprocessedSourceLine list) : PreprocessedSourceLine list =
        lines
        |> List.filter
            (fun line ->
                let content = line.Contents
                match content with
                | PreprocessorLine l ->
                    if String.IsNullOrWhiteSpace l then false
                    else (not (wholeLineCommentRegex.IsMatch l))
                | _ -> true)
