namespace TextOn.Atom

open System
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
/// Take a line that has been determined to be within a function definition and tokenize it.
module DefinitionLineTokenizer =
    // OPS This is surely horrendous perf wise?
    let private conditionMatches =
        [
            (Regex("^(\\s*)%(\w+)(\\s*|$)", RegexOptions.CultureInvariant), (fun n (m:Match) -> { StartIndex = n + m.Groups.[1].Length + 1 ; EndIndex = n + m.Groups.[1].Length + 1 + m.Groups.[2].Length ; Token = AttributeName(m.Groups.[2].Value) }))
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
    let private trailingWhitespaceRegex = Regex(@"^(.*?)\s+$")
    let private stripTrailingWhitespace l =
        let m = trailingWhitespaceRegex.Match(l)
        if m.Success then m.Groups.[1].Value
        else l
    let rec private tokenizeConditionInner n line =
        conditionMatches
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
                if l < line.Length then yield! (tokenizeConditionInner (n + l) (line.Substring(l))) }
    let private funcAtStartRegex = Regex(@"^\$(\w+)", RegexOptions.CultureInvariant)
    let rec private tokenizeMainInner n (line:string) : AttributedToken seq =
        if line.Length = 0 then Seq.empty
        else
            // Find the first character that is one of the special characters I know about.
            let firstIndex = line.IndexOfAny([|'\\';'$';'@';'{';'|';'}'|])
            if firstIndex < 0 then
                Seq.singleton {StartIndex = n + 1;EndIndex = n + line.Length;Token = RawText(line)}
            else if firstIndex = 0 && line.Length = 1 then
                let token =
                    match line.[0] with
                    | '{' -> OpenCurly
                    | '|' -> ChoiceSeparator
                    | '}' -> CloseCurly
                    | _ -> InvalidUnrecognised
                Seq.singleton {StartIndex = n + 1;EndIndex = n + 1;Token = token}
            else
                let output, toRemove =
                    if firstIndex = 0 then
                        match line.[0] with
                        | '{' -> {StartIndex = n + 1;EndIndex = n + 1;Token = OpenCurly}, 1
                        | '}' -> {StartIndex = n + 1;EndIndex = n + 1;Token = CloseCurly}, 1
                        | '|' -> {StartIndex = n + 1;EndIndex = n + 1;Token = ChoiceSeparator}, 1
                        | '$' ->
                            let funcAtStartMatch = funcAtStartRegex.Match(line)
                            if funcAtStartMatch.Success then {StartIndex = n + 1;EndIndex = n + funcAtStartMatch.Length;Token = VariableName(funcAtStartMatch.Groups.[1].Value)}, funcAtStartMatch.Length
                            else {StartIndex = n + 1;EndIndex = n + 1;Token = InvalidUnrecognised}, 1
                        | '\\' ->
                            {StartIndex = n + 1;EndIndex = n + 2;Token = RawText(line.[1].ToString())}, 2
                        | _ ->
                            {StartIndex = n + 1;EndIndex = n + 2;Token = InvalidUnrecognised}, 1
                    else
                        {StartIndex = n + 1;EndIndex = n + firstIndex;Token = RawText(line.Substring(0, firstIndex))}, firstIndex
                seq {
                    yield output
                    yield! (tokenizeMainInner (n + toRemove) (line.Substring(toRemove))) }
    let private strippedFuncInvocationRegex = Regex(@"@(\w+)(\s*)(\{)?$", RegexOptions.CultureInvariant)
    let private tokenizeMain n line =
        // Strip trailing whitespace and pass to main. Leading whitespace should already be gone.
        let strippedLine = stripTrailingWhitespace line
        let strippedFuncInvocationMatch = strippedFuncInvocationRegex.Match(strippedLine)
        if strippedFuncInvocationMatch.Success then
            let name = strippedFuncInvocationMatch.Groups.[1].Value
            let token =
                match name with
                | "var"
                | "func"
                | "att" -> InvalidReservedToken name
                | "break" -> Break
                | "seq" -> Seq
                | "choice" -> Choice
                | _ -> FunctionName name
            let attributedToken = {StartIndex = n + 1;EndIndex = n + name.Length;Token = token}
            if strippedFuncInvocationMatch.Groups.[3].Success then
                let index = n + name.Length + strippedFuncInvocationMatch.Groups.[2].Length + 1
                seq [attributedToken;{StartIndex = index;EndIndex = index;Token = OpenCurly}]
            else
                Seq.singleton attributedToken
        else
            tokenizeMainInner n strippedLine
    let private funcDefinitionRegex = Regex("^@func\s+")
    let private openCurlyRegex = Regex("^(\\s*)\\{\\s*$")
    let private closeCurlyRegex = Regex("^(\\s*)\\}\\s*$")
    let private unescapedOpenBraceRegex = Regex("^(\\s*)(([^\[\\\\]+|\\\\\[|\\\\\\\\)*)(\[.*)?$")
    let private funcNameRegex = Regex("^@(\w+)(\s*)(\{)?\s*$")
    let tokenizeLine (line:string) =
        let funcDefinitionMatch = funcDefinitionRegex.Match(line)
        if funcDefinitionMatch.Success then
            let funcDefinitionToken = {StartIndex = 1;EndIndex = 5;Token = Func}
            let funcNameMatch = funcNameRegex.Match(line.Substring(funcDefinitionMatch.Length))
            if funcNameMatch.Success then
                let funcName = funcNameMatch.Groups.[1].Value
                let funcNameToken = {StartIndex = funcDefinitionMatch.Length + 1;EndIndex = funcDefinitionMatch.Length + funcNameMatch.Length;Token = (match funcName with | "break" | "var" | "att" | "func" | "seq" | "choice" -> InvalidReservedToken(funcName) | _ -> FunctionName(funcName))}
                let openCurlyToken =
                    if funcNameMatch.Groups.[3].Success then
                        let index = funcDefinitionMatch.Length + funcNameMatch.Groups.[1].Length + funcNameMatch.Groups.[2].Length + 1
                        Seq.singleton {StartIndex = index;EndIndex = index;Token = OpenCurly}
                    else
                        Seq.empty
                seq { yield funcDefinitionToken; yield funcNameToken; yield! openCurlyToken }
            else
                let unrecognisedToken = {StartIndex = 6;EndIndex = line.Length;Token = InvalidUnrecognised}
                seq {
                    yield funcDefinitionToken
                    yield unrecognisedToken }
        else
            // Find the first unescaped '['.
            let unescapedOpenBraceMatch = unescapedOpenBraceRegex.Match(line)
            if unescapedOpenBraceMatch.Success then
                if unescapedOpenBraceMatch.Groups.[4].Success then
                    let main = tokenizeMain unescapedOpenBraceMatch.Groups.[1].Length unescapedOpenBraceMatch.Groups.[2].Value
                    let condition = tokenizeConditionInner (unescapedOpenBraceMatch.Groups.[1].Length + unescapedOpenBraceMatch.Groups.[2].Length) unescapedOpenBraceMatch.Groups.[4].Value
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
                            tokenizeMain unescapedOpenBraceMatch.Groups.[1].Length unescapedOpenBraceMatch.Groups.[2].Value
            else
                failwith "I honestly don't know how this can fail."


