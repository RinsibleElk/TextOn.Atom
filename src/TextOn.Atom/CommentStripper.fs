namespace TextOn.Atom

open System

[<RequireQualifiedAccess>]
/// Maps preprocessed source into preprocessed source having removed comments.
/// OPS: It's not obvious this is sensible prior to tokenizing. Note this basically assumes that comments are whole lines.
module CommentStripper =
    let private eatWhitespaceAtBeginning (l:string) =
        seq [ 0 .. l.Length - 1 ]
        |> Seq.tryFind (fun i -> (not (Char.IsWhiteSpace (l.[i]))))

    let private isNotWhitespaceOrComment (l:string) =
        if l |> String.IsNullOrEmpty then false
        else
            let offsetToNonWhitespace = eatWhitespaceAtBeginning l
            if offsetToNonWhitespace.IsNone then false
            else if offsetToNonWhitespace.Value >= l.Length - 1 then true
            else
                l.[offsetToNonWhitespace.Value] <> '/' || l.[offsetToNonWhitespace.Value + 1] <> '/'

    /// Strip out all comment and blank lines from the preprocessed source.
    let stripComments (lines:PreprocessedSourceLine list) : PreprocessedSourceLine list =
        lines
        |> List.filter
            (fun line ->
                let content = line.Contents
                match content with
                | PreprocessorLine l -> isNotWhitespaceOrComment l
                | _ -> true)
