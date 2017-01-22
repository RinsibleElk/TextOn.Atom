namespace TextOn.Atom

open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
module CommentStripper =
    let private commentLineRegex = Regex(@"^\s*(//.*)?$")

    /// Strip out all comment and blank lines from the preprocessed source.
    let stripComments (lines:PreprocessedSourceLine list) : PreprocessedSourceLine list =
        lines
        |> List.filter
            (fun line ->
                let content = line.Contents
                match content with
                | Line line -> (not (commentLineRegex.IsMatch(line)))
                | _ -> true)
