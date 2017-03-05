namespace TextOn.Atom

open System
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
/// Take a line that has been determined to be within a function definition and tokenize it.
module internal FunctionLineTokenizer =
    let private eatWhitespaceAtBeginning n e (l:string) =
        let mutable i = n
        while i <= e && (Char.IsWhiteSpace(l.[i])) do
            i <- i + 1
        if i >= e then None else Some i

    let private expectWord makeToken n e (l:string) =
        let mutable i = n
        while i <= e && (Char.IsLetterOrDigit(l.[i])) do
            i <- i + 1
        (i, makeToken (i - 1) (l.Substring(n, i - n)))
 
    type private State =
        | Root
    
    let private makeAtToken startLocation endLocation s =
        {
            TokenStartLocation = startLocation
            TokenEndLocation = endLocation
            Token =
                (   match s with
                    | "" -> Token.InvalidUnrecognised("@")
                    | "seq" -> Token.Sequential
                    | "choice" -> Token.Choice
                    | "break" -> Token.Break
                    | "var" -> Token.Var
                    | "att" -> Token.Att
                    | "func" -> Token.Func
                    | _ -> Token.FunctionName(s))
        }
           
    // Special characters depend on context.
    // In condition: '(', ')', '&', '|', '=', '<', '%', '['; ']' changes context
    // Start: '@', '{', '}'; anything else changes context
    // In text: '\', '{', '|', '}', '$'; '[' changes context
    let tokenize (line:string) =
        let lastChar = line.Length - 1
        let io = eatWhitespaceAtBeginning 0 lastChar line
        if io.IsNone then failwith "Internal error" // Blank line filtered out already
        let mutable i = io.Value
        let mutable state = Root
        let tokens = System.Collections.Generic.List<AttributedToken>()
        let l = line.Length
        while i < l do
            match state with
            | Root ->
                match line.[i] with
                | '{' ->
                    tokens.Add({ TokenStartLocation = i ; TokenEndLocation = i ; Token = OpenCurly })
                    i <- i + 1
                | '}' ->
                    tokens.Add({ TokenStartLocation = i ; TokenEndLocation = i ; Token = CloseCurly })
                    i <- i + 1
                | '@' ->
                    expectWord (makeAtToken i) (i + 1) lastChar line
                    |> fun (newi, token) ->
                        i <- if newi = i then i + 1 else newi
                        tokens.Add(token)
        ()

    let private conditionMatches =
        [
            // Match "  %SomeAttribute " at the start of a string.
            (Regex("^(\\s*)%(\w+)(\\s*|$)", RegexOptions.CultureInvariant), (fun n (m:Match) -> { TokenStartLocation = n + m.Groups.[1].Length + 1 ; TokenEndLocation = n + m.Groups.[1].Length + 1 + m.Groups.[2].Length ; Token = AttributeName(m.Groups.[2].Value) }))
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
    let private varAtStartRegex = Regex(@"^\$(\w+)", RegexOptions.CultureInvariant)
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
                            let varAtStartMatch = varAtStartRegex.Match(line)
                            if varAtStartMatch.Success then {TokenStartLocation = n + 1;TokenEndLocation = n + varAtStartMatch.Length;Token = VariableName(varAtStartMatch.Groups.[1].Value)}, varAtStartMatch.Length
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
                | "free"
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
    let private funcDefinition = @"@func"
    let private openCurlyRegex = Regex("^(\\s*)\\{\\s*$")
    let private closeCurlyRegex = Regex("^(\\s*)\\}\\s*$")
    let private unescapedOpenBraceRegex = Regex("^(\\s*)(([^\[\\\\]+|\\\\\[|\\\\\\\\)*)(\[.*)?$")
    let private funcNameRegex = Regex("^@(\w+)(\s*)(\{)?\s*$")
    let private leadingWhitespaceRegex = Regex("^(\\s*)")
    /// Tokenize a single line of source that has been categorized to lie within a function definition.
    let tokenizeLine (line:string) =
        let lastChar = line.Length - 1
        if line.StartsWith("@func") then
            let funcDefinitionToken = {TokenStartLocation = 1;TokenEndLocation = 5;Token = Func}
            let nonWhitespaceOffset = eatWhitespaceAtBeginning 5 lastChar line
            if nonWhitespaceOffset.IsNone then
                Seq.singleton funcDefinitionToken
            else
                let nonWhitespaceOffset = nonWhitespaceOffset.Value
                let funcNameMatch = funcNameRegex.Match(line.Substring(nonWhitespaceOffset))
                if funcNameMatch.Success then
                    let funcName = funcNameMatch.Groups.[1].Value
                    let funcNameToken = {TokenStartLocation = nonWhitespaceOffset + 1;TokenEndLocation = nonWhitespaceOffset + funcNameMatch.Length;Token = (match funcName with | "break" | "var" | "att" | "func" | "free" | "seq" | "choice" -> InvalidReservedToken(funcName) | _ -> FunctionName(funcName))}
                    let openCurlyToken =
                        if funcNameMatch.Groups.[3].Success then
                            let index = nonWhitespaceOffset + funcNameMatch.Groups.[1].Length + funcNameMatch.Groups.[2].Length + 1
                            Seq.singleton {TokenStartLocation = index;TokenEndLocation = index;Token = OpenCurly}
                        else
                            Seq.empty
                    seq { yield funcDefinitionToken; yield funcNameToken; yield! openCurlyToken }
                else if nonWhitespaceOffset >= line.Length then
                    Seq.singleton funcDefinitionToken
                else
                    let unrecognisedToken = {TokenStartLocation = nonWhitespaceOffset + 1;TokenEndLocation = line.Length;Token = InvalidUnrecognised (line.Substring(nonWhitespaceOffset + 1)) }
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
                let m = leadingWhitespaceRegex.Match(line)
                let l = m.Groups.[1].Length
                tokenizeMain l (line.Substring(l))
