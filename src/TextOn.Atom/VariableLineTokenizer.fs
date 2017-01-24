namespace TextOn.Atom

open System
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
/// Take a line that has been determined to be within a variable definition and tokenize it.
module VariableLineTokenizer =
    let private matches =
        [
            (Regex("^(@var)\\s+"), (fun n (m:Match) -> { StartIndex = n + 1 ; EndIndex = n + 4 ; Token = Var }))
            (Regex("^(\\s*)\\$(\w+)(\\s*|$)", RegexOptions.CultureInvariant), (fun n (m:Match) -> { StartIndex = n + m.Groups.[1].Length + 1 ; EndIndex = n + m.Groups.[1].Length + 1 + m.Groups.[2].Length ; Token = VariableName(m.Groups.[2].Value) }))
            (Regex("^(\\s*)%(\w+)(\\s*|$)", RegexOptions.CultureInvariant), (fun n (m:Match) -> { StartIndex = n + m.Groups.[1].Length + 1 ; EndIndex = n + m.Groups.[1].Length + 1 + m.Groups.[2].Length ; Token = AttributeName(m.Groups.[2].Value) }))
            (Regex("^(\\s*)\\{(\\s*|$)"), (fun n (m:Match) -> { StartIndex = n + m.Groups.[1].Length + 1 ; EndIndex = n + m.Groups.[1].Length + 1 ; Token = OpenCurly }))
            (Regex("^(\\s*)\\}(\\s*|$)"), (fun n (m:Match) -> { StartIndex = n + m.Groups.[1].Length + 1 ; EndIndex = n + m.Groups.[1].Length + 1 ; Token = CloseCurly }))
            (Regex("^(\\s*)\\[(\\s*|$)"), (fun n (m:Match) -> { StartIndex = n + m.Groups.[1].Length + 1 ; EndIndex = n + m.Groups.[1].Length + 1 ; Token = OpenBrace }))
            (Regex("^(\\s*)\\](\\s*|$)"), (fun n (m:Match) -> { StartIndex = n + m.Groups.[1].Length + 1 ; EndIndex = n + m.Groups.[1].Length + 1 ; Token = CloseBrace }))
            (Regex("^(\\s*)\\((\\s*|$)"), (fun n (m:Match) -> { StartIndex = n + m.Groups.[1].Length + 1 ; EndIndex = n + m.Groups.[1].Length + 1 ; Token = OpenBracket }))
            (Regex("^(\\s*)\\)(\\s*|$)"), (fun n (m:Match) -> { StartIndex = n + m.Groups.[1].Length + 1 ; EndIndex = n + m.Groups.[1].Length + 1 ; Token = CloseBracket }))
            (Regex("^(\\s*)&&(\\s*|$)"), (fun n (m:Match) -> { StartIndex = n + m.Groups.[1].Length + 1 ; EndIndex = n + m.Groups.[1].Length + 2 ; Token = And }))
            (Regex("^(\\s*)\\|\\|(\\s*|$)"), (fun n (m:Match) -> { StartIndex = n + m.Groups.[1].Length + 1 ; EndIndex = n + m.Groups.[1].Length + 2 ; Token = Or }))
            (Regex("^(\\s*)=(\\s*|$)"), (fun n (m:Match) -> { StartIndex = n + m.Groups.[1].Length + 1 ; EndIndex = n + m.Groups.[1].Length + 1 ; Token = Equals }))
            (Regex("^(\\s*)\\*(\\s*|$)"), (fun n (m:Match) -> { StartIndex = n + m.Groups.[1].Length + 1 ; EndIndex = n + m.Groups.[1].Length + 1 ; Token = Star }))
            (Regex("^(\\s*)<>(\\s*|$)"), (fun n (m:Match) -> { StartIndex = n + m.Groups.[1].Length + 1 ; EndIndex = n + m.Groups.[1].Length + 2 ; Token = NotEquals }))
            (Regex("^(\\s*)\"(([^\\\"\\\\]+|\\\\\\\"|\\\\\\\\)*)\"(\\s*|$)"), (fun n (m:Match) -> { StartIndex = n + m.Groups.[1].Length + 1 ; EndIndex = n + m.Groups.[1].Length + 2 + m.Groups.[2].Length ; Token = QuotedString(m.Groups.[2].Value.Replace("\\\\", "\\").Replace("\\\"", "\"")) }))
        ]
    let rec private tokenizeLineInner n line =
        matches
        |> Seq.choose
            (fun (re, c) ->
                let m = re.Match(line)
                if m.Success |> not then None
                else Some (m.Length, (c n m)))
        |> Seq.tryFind (fun _ -> true)
        |> defaultArg <| (line.Length, { StartIndex = n + 1 ; EndIndex = n + line.Length ; Token = InvalidUnrecognised })
        |> fun (l, t) ->
            seq {
                yield t
                if l < line.Length then yield! (tokenizeLineInner (n + l) (line.Substring(l))) }
    /// Tokenize a line within a variable definition.
    let tokenizeLine line = tokenizeLineInner 0 line
