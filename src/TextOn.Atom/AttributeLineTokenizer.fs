namespace TextOn.Atom

open System
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
/// Take a line that has been determined to be within a attribute definition and tokenize it.
module internal AttributeLineTokenizer =
    let private matches =
        [
            // Match "@att " at the start of a string.
            (Regex("^(@att)\\s+"), (fun n (m:Match) -> { TokenStartLocation = n + 1 ; TokenEndLocation = n + 4 ; Token = Att }))
            // Match "  %SomeAtt " at the start of a string.
            (Regex("^(\\s*)%(\w+)(\\s*|$)", RegexOptions.CultureInvariant), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 1 + m.Groups.[2].Length ; Token = AttributeName(m.Groups.[2].Value) }))
            // Match "  { " at the start of a string.
            (Regex("^(\\s*)\\{(\\s*|$)"), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 1 ; Token = OpenCurly }))
            // Match "  } " at the start of a string.
            (Regex("^(\\s*)\\}(\\s*|$)"), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 1 ; Token = CloseCurly }))
            // Match "  [ " at the start of a string.
            (Regex("^(\\s*)\\[(\\s*|$)"), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 1 ; Token = OpenBrace }))
            // Match "  ] " at the start of a string.
            (Regex("^(\\s*)\\](\\s*|$)"), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 1 ; Token = CloseBrace }))
            // Match "  ( " at the start of a string.
            (Regex("^(\\s*)\\((\\s*|$)"), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 1 ; Token = OpenBracket }))
            // Match "  ) " at the start of a string.
            (Regex("^(\\s*)\\)(\\s*|$)"), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 1 ; Token = CloseBracket }))
            // Match "  && " at the start of a string.
            (Regex("^(\\s*)&&(\\s*|$)"), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 2 ; Token = And }))
            // Match "  || " at the start of a string.
            (Regex("^(\\s*)\\|\\|(\\s*|$)"), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 2 ; Token = Or }))
            // Match "  = " at the start of a string.
            (Regex("^(\\s*)=(\\s*|$)"), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 1 ; Token = Equals }))
            // Match "  <> " at the start of a string.
            (Regex("^(\\s*)<>(\\s*|$)"), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 2 ; Token = NotEquals }))
            // Match "  "Some quoted string with escaped characters like \" and \\." " at the start of a string.
            (Regex("^(\\s*)\"(([^\\\"\\\\]+|\\\\\\\"|\\\\\\\\)*)\"(\\s*|$)"), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 2 + m.Groups.[2].Length ; Token = QuotedString(m.Groups.[2].Value.Replace("\\\\", "\\").Replace("\\\"", "\"")) }))
        ]
    let rec private tokenizeLineInner n line =
        matches
        |> Seq.choose
            (fun (re, c) ->
                let m = re.Match(line)
                if m.Success |> not then None
                else Some (m.Length, (c n m)))
        |> Seq.tryFind (fun _ -> true)
        |> defaultArg <| (line.Length, { TokenStartLocation = n + 1 ; TokenEndLocation = n + line.Length ; Token = InvalidUnrecognised line })
        |> fun (l, t) ->
            seq {
                yield t
                if l < line.Length then yield! (tokenizeLineInner (n + l) (line.Substring(l))) }
    /// Tokenize a line within a variable definition.
    let tokenizeLine line = tokenizeLineInner 0 line
