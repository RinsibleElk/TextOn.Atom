namespace TextOn.Atom

open System
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
/// Maps preprocessed source into preprocessed source having removed comments.
/// OPS: It's not obvious this is sensible prior to tokenizing. Note this basically assumes that comments are whole lines.
module CommentStripper =
    let private singleLineCommentRegex = Regex("(^|^(.+?[^\\\\]))(\\s*//.*)?$")
    let private matchEvaluator = MatchEvaluator(fun m -> m.Groups.[1].Value)
    let replace s = singleLineCommentRegex.Replace(s, matchEvaluator)

    /// Strip out all comment and blank lines from the preprocessed source.
    let stripComments (lines:PreprocessedSourceLine list) : PreprocessedSourceLine list =
        lines
        |> List.choose
            (fun line ->
                let content = line.Contents
                match content with
                | PreprocessorLine l ->
                    let l = l |> replace
                    if String.IsNullOrWhiteSpace(l) then None
                    else Some { line with Contents = PreprocessorLine l }
                | _ -> Some line)
