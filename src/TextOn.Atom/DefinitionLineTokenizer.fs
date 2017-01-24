namespace TextOn.Atom

open System
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
/// Take a line that has been determined to be within a function definition and tokenize it.
module DefinitionLineTokenizer =
    let private conditionMatches =
        [
            (Regex("^(\\s*)%([A-Za-z][A-Za-z0-9_]*)(\\s*|$)"), (fun n (m:Match) -> { StartIndex = n + m.Groups.[1].Length + 1 ; EndIndex = n + m.Groups.[1].Length + 1 + m.Groups.[2].Length ; Token = AttributeName(m.Groups.[2].Value) }))
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
    let rec private tokenizeConditionInner n line =
        conditionMatches
        |> Seq.choose
            (fun (re, c) ->
                let m = re.Match(line)
                if m.Success |> not then None
                else Some (m.Length, (c n m)))
        |> Seq.tryFind (fun _ -> true)
        |> defaultArg <| (line.Length, { StartIndex = n + 1 ; EndIndex = n + line.Length ; Token = Unrecognised })
        |> fun (l, t) ->
            seq {
                yield t
                if l < line.Length then yield! (tokenizeConditionInner (n + l) (line.Substring(l))) }
    let rec private tokenizeMainInner n line : AttributedToken seq =
        failwith ""
    let private funcDefinitionRegex = Regex("^@func\s+")
    let private funcNameRegex = Regex("^(\\s*)@([A-Za-z][A-Za-z0-9_]*)(\\s*|$)")
    let private openCurlyRegex = Regex("^(\\s*)\\{(\\s*|$)")
    let private closeCurlyRegex = Regex("^(\\s*)\\}(\\s*|$)")
    let private unescapedOpenBraceRegex = Regex("^(\\s*)(([^\[\\\\]+|\\\\\[|\\\\\\\\)*)(\[.*)?$")
    // OPS don't forget @seq, @break, @choice, @funcName, etc....
    let tokenizeLine (line:string) =
        let funcDefinitionMatch = funcDefinitionRegex.Match(line)
        if funcDefinitionMatch.Success then
            failwith ""
        else
            // Find the first unescaped '['.
            let unescapedOpenBraceMatch = unescapedOpenBraceRegex.Match(line)
            if unescapedOpenBraceMatch.Success then
                if unescapedOpenBraceMatch.Groups.[2].Success then
                    let main = tokenizeMainInner unescapedOpenBraceMatch.Groups.[0].Length unescapedOpenBraceMatch.Groups.[0].Value
                    let condition = tokenizeConditionInner (unescapedOpenBraceMatch.Groups.[0].Length + unescapedOpenBraceMatch.Groups.[1].Length) unescapedOpenBraceMatch.Groups.[2].Value
                    Seq.append main condition
                else
                    let openCurlyMatch = openCurlyRegex.Match(line)
                    if openCurlyMatch.Success then
                        let index = openCurlyMatch.Groups.[1].Length + 1
                        Seq.singleton {StartIndex = index;EndIndex = index;Token = OpenCurly}
                    else
                        let closeCurlyMatch = closeCurlyRegex.Match(line)
                        if closeCurlyMatch.Success then
                            let index = closeCurlyMatch.Groups.[1].Length + 1
                            Seq.singleton {StartIndex = index;EndIndex = index;Token = CloseCurly}
                        else
                            tokenizeMainInner unescapedOpenBraceMatch.Groups.[0].Length unescapedOpenBraceMatch.Groups.[0].Value
            else
                failwith "I honestly don't know how this can fail."


