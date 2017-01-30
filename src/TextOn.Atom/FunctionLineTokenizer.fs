namespace TextOn.Atom

open System
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
/// Take a line that has been determined to be within a function definition and tokenize it.
module internal FunctionLineTokenizer =
    // OPS This is surely horrendous perf wise?
    let private conditionMatches =
        [
            (Regex("^(\\s*)%(\w+)(\\s*|$)", RegexOptions.CultureInvariant), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 1 + m.Groups.[2].Length ; Token = AttributeName(m.Groups.[2].Value) }))
            (Regex("^(\\s*)\\[(\\s*|$)"), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 1 ; Token = OpenBrace }))
            (Regex("^(\\s*)\\](\\s*|$)"), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 1 ; Token = CloseBrace }))
            (Regex("^(\\s*)\\((\\s*|$)"), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 1 ; Token = OpenBracket }))
            (Regex("^(\\s*)\\)(\\s*|$)"), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 1 ; Token = CloseBracket }))
            (Regex("^(\\s*)&&(\\s*|$)"), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 2 ; Token = And }))
            (Regex("^(\\s*)\\|\\|(\\s*|$)"), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 2 ; Token = Or }))
            (Regex("^(\\s*)=(\\s*|$)"), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 1 ; Token = Equals }))
            (Regex("^(\\s*)\\*(\\s*|$)"), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 1 ; Token = Star }))
            (Regex("^(\\s*)<>(\\s*|$)"), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 2 ; Token = NotEquals }))
            (Regex("^(\\s*)\"(([^\\\"\\\\]+|\\\\\\\"|\\\\\\\\)*)\"(\\s*|$)"), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 2 + m.Groups.[2].Length ; Token = QuotedString(m.Groups.[2].Value.Replace("\\\\", "\\").Replace("\\\"", "\"")) }))
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
        |> defaultArg <| (line.Length, { TokenStartLocation = n + 1 ; TokenEndLocation = n + line.Length ; Token = InvalidUnrecognised line })
        |> fun (l, t) ->
            seq {
                yield t
                if l < line.Length then yield! (tokenizeConditionInner (n + l) (line.Substring(l))) }
    let private funcAtStartRegex = Regex(@"^\$(\w+)", RegexOptions.CultureInvariant)
    let rec private tokenizeMainInner n (line:string) : AttributedToken seq =
        if line.Length = 0 then Seq.empty
        else
            // Find the first character that is one of the special characters I know about.
            let firstIndex = line.IndexOfAny([|'\\';'$';'@';'{';'|';'}';'#'|])
            if firstIndex < 0 then
                Seq.singleton {TokenStartLocation = n + 1;TokenEndLocation = n + line.Length;Token = RawText(line)}
            else if firstIndex = 0 && line.Length = 1 then
                let token =
                    match line.[0] with
                    | '{' -> OpenCurly
                    | '|' -> ChoiceSeparator
                    | '}' -> CloseCurly
                    | _ -> InvalidUnrecognised (line.[0].ToString())
                Seq.singleton {TokenStartLocation = n + 1;TokenEndLocation = n + 1;Token = token}
            else
                let output, toRemove =
                    if firstIndex = 0 then
                        match line.[0] with
                        | '{' -> {TokenStartLocation = n + 1;TokenEndLocation = n + 1;Token = OpenCurly}, 1
                        | '}' -> {TokenStartLocation = n + 1;TokenEndLocation = n + 1;Token = CloseCurly}, 1
                        | '|' -> {TokenStartLocation = n + 1;TokenEndLocation = n + 1;Token = ChoiceSeparator}, 1
                        | '$' ->
                            let funcAtStartMatch = funcAtStartRegex.Match(line)
                            if funcAtStartMatch.Success then {TokenStartLocation = n + 1;TokenEndLocation = n + funcAtStartMatch.Length;Token = VariableName(funcAtStartMatch.Groups.[1].Value)}, funcAtStartMatch.Length
                            else {TokenStartLocation = n + 1;TokenEndLocation = n + 1;Token = InvalidUnrecognised (line.[0].ToString()) }, 1
                        | '\\' ->
                            {TokenStartLocation = n + 1;TokenEndLocation = n + 2;Token = RawText(line.[1].ToString())}, 2
                        | _ ->
                            {TokenStartLocation = n + 1;TokenEndLocation = n + 1;Token = InvalidUnrecognised (line.[0].ToString())}, 1
                    else
                        {TokenStartLocation = n + 1;TokenEndLocation = n + firstIndex;Token = RawText(line.Substring(0, firstIndex))}, firstIndex
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
                | "seq" -> Sequential
                | "choice" -> Choice
                | _ -> FunctionName name
            let attributedToken = {TokenStartLocation = n + 1;TokenEndLocation = n + 1 + name.Length;Token = token}
            if strippedFuncInvocationMatch.Groups.[3].Success then
                let index = n + 1 + name.Length + strippedFuncInvocationMatch.Groups.[2].Length + 1
                seq [attributedToken;{TokenStartLocation = index;TokenEndLocation = index;Token = OpenCurly}]
            else
                Seq.singleton attributedToken
        else
            tokenizeMainInner n strippedLine
    let private funcDefinitionRegex = Regex("^@func\s+")
    let private openCurlyRegex = Regex("^(\\s*)\\{\\s*$")
    let private closeCurlyRegex = Regex("^(\\s*)\\}\\s*$")
    let private unescapedOpenBraceRegex = Regex("^(\\s*)(([^\[\\\\]+|\\\\\[|\\\\\\\\)*)(\[.*)?$")
    let private funcNameRegex = Regex("^@(\w+)(\s*)(\{)?\s*$")
    /// Tokenize a single line of source that has been categorized to lie within a function definition.
    let tokenizeLine (line:string) =
        let funcDefinitionMatch = funcDefinitionRegex.Match(line)
        if funcDefinitionMatch.Success then
            let funcDefinitionToken = {TokenStartLocation = 1;TokenEndLocation = 5;Token = Func}
            let funcNameMatch = funcNameRegex.Match(line.Substring(funcDefinitionMatch.Length))
            if funcNameMatch.Success then
                let funcName = funcNameMatch.Groups.[1].Value
                let funcNameToken = {TokenStartLocation = funcDefinitionMatch.Length + 1;TokenEndLocation = funcDefinitionMatch.Length + funcNameMatch.Length;Token = (match funcName with | "break" | "var" | "att" | "func" | "seq" | "choice" -> InvalidReservedToken(funcName) | _ -> FunctionName(funcName))}
                let openCurlyToken =
                    if funcNameMatch.Groups.[3].Success then
                        let index = funcDefinitionMatch.Length + funcNameMatch.Groups.[1].Length + funcNameMatch.Groups.[2].Length + 1
                        Seq.singleton {TokenStartLocation = index;TokenEndLocation = index;Token = OpenCurly}
                    else
                        Seq.empty
                seq { yield funcDefinitionToken; yield funcNameToken; yield! openCurlyToken }
            else
                let unrecognisedToken = {TokenStartLocation = funcDefinitionMatch.Length + 1;TokenEndLocation = line.Length;Token = InvalidUnrecognised (line.Substring(funcDefinitionMatch.Length + 1)) }
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
                        Seq.singleton {TokenStartLocation = index;TokenEndLocation = index;Token = OpenCurly}
                    else
                        let closeCurlyMatch = closeCurlyRegex.Match(line)
                        if closeCurlyMatch.Success then
                            let index = closeCurlyMatch.Groups.[1].Length + 1
                            Seq.singleton {TokenStartLocation = index;TokenEndLocation = index;Token = CloseCurly}
                        else
                            tokenizeMain unescapedOpenBraceMatch.Groups.[1].Length unescapedOpenBraceMatch.Groups.[2].Value
            else
                // I think this can probably only fail if the line is only a backslash?
                Seq.singleton {TokenStartLocation = 1;TokenEndLocation = line.Length; Token = InvalidUnrecognised line }

