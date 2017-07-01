[<RequireQualifiedAccess>]
/// Take a line that has been determined to be within a function definition and tokenize it.
module internal TextOn.Core.FunctionLineTokenizer

open System
open System.Text
open System.Collections.Generic

/// Special characters depend on context.
/// In condition: '(', ')', '&', '|', '=', '<', '>', '%', '['; ']' changes context
/// Start: '@', '{', '}'; anything else changes context
/// In text: '\', '{', '|', '}', '$'; '[' changes context
let tokenizeLine (line:string) =
    let mutable i = 0
    let lastIndex = line.Length - 1
    let tokens = List<AttributedToken>()
    let mutable sb = StringBuilder()
    let mutable rawTextStart = -1
    let mutable trailingWhitespaceCharacters = 0
    while i <= lastIndex do
        let c = line.[i]
        if rawTextStart >= 0 then
            if Char.IsWhiteSpace c then
                sb.Append(c) |> ignore
                trailingWhitespaceCharacters <- trailingWhitespaceCharacters + 1
                i <- i + 1
            else
                match c with
                | '[' ->
                    let text = sb.ToString()
                    if i + 1 <> rawTextStart then
                        tokens.Add({ TokenStartLocation = rawTextStart ; TokenEndLocation = i - trailingWhitespaceCharacters ; Token = RawText(text.Substring(0, text.Length - trailingWhitespaceCharacters)) })
                    tokens.AddRange(ConditionTokenizer.tokenizeCondition i lastIndex line)
                    rawTextStart <- -1
                    i <- lastIndex + 1
                | '{' ->
                    if i + 1 <> rawTextStart then
                        tokens.Add({ TokenStartLocation = rawTextStart ; TokenEndLocation = i ; Token = RawText(sb.ToString()) })
                        sb <- StringBuilder()
                        trailingWhitespaceCharacters <- 0
                    tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = i + 1 ; Token = OpenCurly })
                    i <- i + 1
                    rawTextStart <- i + 1
                | '|' ->
                    if i + 1 <> rawTextStart then
                        tokens.Add({ TokenStartLocation = rawTextStart ; TokenEndLocation = i ; Token = RawText(sb.ToString()) })
                        sb <- StringBuilder()
                        trailingWhitespaceCharacters <- 0
                    tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = i + 1 ; Token = ChoiceSeparator })
                    i <- i + 1
                    rawTextStart <- i + 1
                | '}' ->
                    if i + 1 <> rawTextStart then
                        tokens.Add({ TokenStartLocation = rawTextStart ; TokenEndLocation = i ; Token = RawText(sb.ToString()) })
                        sb <- StringBuilder()
                        trailingWhitespaceCharacters <- 0
                    tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = i + 1 ; Token = CloseCurly })
                    i <- i + 1
                    rawTextStart <- i + 1
                | '$' ->
                    trailingWhitespaceCharacters <- 0
                    if i + 1 <> rawTextStart then
                        tokens.Add({ TokenStartLocation = rawTextStart ; TokenEndLocation = i ; Token = RawText(sb.ToString()) })
                        sb <- StringBuilder()
                    let len = IdentifierTokenizer.findLengthOfWord (i + 1) lastIndex line
                    tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = i + 1 + len ; Token = VariableName(line.Substring(i+1, len)) })
                    i <- i + len + 1
                    rawTextStart <- i + 1
                | '/' ->
                    if i = lastIndex || line.[i + 1] <> '/' then
                        trailingWhitespaceCharacters <- 0
                        sb.Append('/') |> ignore
                        i <- i + 1
                    else
                        let text = sb.ToString()
                        if i + 1 <> rawTextStart then
                            tokens.Add({ TokenStartLocation = rawTextStart ; TokenEndLocation = i - trailingWhitespaceCharacters ; Token = RawText(text.Substring(0, text.Length - trailingWhitespaceCharacters)) })
                        rawTextStart <- -1
                        // Comments don't need tokenizing.
                        i <- lastIndex + 1
                | '\\' ->
                    if i = lastIndex then
                        if i + 1 <> rawTextStart then
                            tokens.Add({ TokenStartLocation = rawTextStart ; TokenEndLocation = i ; Token = RawText(sb.ToString()) })
                        tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = lastIndex + 1 ; Token = InvalidUnrecognised(line.Substring(i)) })
                        rawTextStart <- -1
                        i <- lastIndex + 1
                    else
                        sb.Append(line.[i + 1]) |> ignore
                        trailingWhitespaceCharacters <- 0
                        i <- i + 1
                | _ ->
                    sb.Append(c) |> ignore
                    trailingWhitespaceCharacters <- 0
                    i <- i + 1
        else
            if Char.IsWhiteSpace c then
                i <- i + 1
            else
                match c with
                | '@' ->
                    i <- IdentifierTokenizer.tokenizeFunctionName tokens i lastIndex line
                | '[' ->
                    tokens.AddRange(ConditionTokenizer.tokenizeCondition i lastIndex line)
                    i <- lastIndex + 1
                | '{' ->
                    tokens.Add({TokenStartLocation=i+1;TokenEndLocation=i+1;Token=OpenCurly })
                    i <- i + 1
                | '}' ->
                    tokens.Add({TokenStartLocation=i+1;TokenEndLocation=i+1;Token=CloseCurly })
                    i <- i + 1
                | '\\' ->
                    if i = lastIndex then
                        tokens.Add({TokenStartLocation=i+1;TokenEndLocation=i+1;Token=InvalidUnrecognised "\\" })
                        i <- lastIndex + 1
                    else
                        rawTextStart <- i + 1
                        sb.Append(line.[i + 1]) |> ignore
                        i <- i + 2
                | '/' ->
                    if i = lastIndex || line.[i + 1] <> '/' then
                        tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = lastIndex + 1 ; Token = InvalidUnrecognised(line.Substring(i)) })
                    i <- lastIndex + 1
                | '$' ->
                    let len = IdentifierTokenizer.findLengthOfWord (i + 1) lastIndex line
                    tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = i + 1 + len ; Token = VariableName(line.Substring(i+1, len)) })
                    i <- i + len + 1
                    rawTextStart <- i + 1
                | _ ->
                    rawTextStart <- i + 1
                    sb.Append(c) |> ignore
                    i <- i + 1
    let text = sb.ToString()
    if rawTextStart <> -1 && i + 1 <> rawTextStart then
        tokens.Add({ TokenStartLocation = rawTextStart ; TokenEndLocation = i - trailingWhitespaceCharacters ; Token = RawText(text.Substring(0, text.Length - trailingWhitespaceCharacters)) })
    tokens |> Seq.toList

