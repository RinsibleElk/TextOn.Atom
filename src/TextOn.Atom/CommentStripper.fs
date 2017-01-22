namespace TextOn.Atom

open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
/// Maps preprocessed source into preprocessed source having removed comments.
/// OPS: It's not obvious this is sensible prior to tokenizing. Note this basically assumes that comments are whole lines.
module CommentStripper =
    let private commentLineRegex = Regex(@"^\s*(//.*)?$")

    /// Strip out all comment and blank lines from the preprocessed source.
    let stripComments (lines:PreprocessedSourceLine seq) : PreprocessedSourceLine seq =
        lines
        |> Seq.filter
            (fun line ->
                let content = line.Contents
                match content with
                | Line line -> (not (commentLineRegex.IsMatch(line)))
                | _ -> true)
