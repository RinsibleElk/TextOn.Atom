namespace TextOn.Atom

open System
open System.Text
open System.Collections.Generic

[<RequireQualifiedAccess>]
module internal ConditionTokenizer =
    /// Tokenize a condition.
    let tokenizeCondition n lastIndex (line:string) =
        let mutable i = n
        let tokens = List<AttributedToken>()
        while i <= lastIndex do
            let c = line.[i]
            if Char.IsWhiteSpace(c) then
                i <- i + 1
            else
                match c with
                | '=' ->
                    tokens.Add({ TokenStartLocation = i ; TokenEndLocation = i ; Token = Equals })
                    i <- i + 1
                | '<' ->
                    if i = lastIndex then
                        tokens.Add({ TokenStartLocation = i ; TokenEndLocation = lastIndex ; Token = InvalidUnrecognised(line.Substring(i)) })
                        i <- lastIndex + 1
                    else if line.[i + 1] <> '>' then
                        tokens.Add({ TokenStartLocation = i ; TokenEndLocation = lastIndex ; Token = InvalidUnrecognised(line.Substring(i)) })
                        i <- lastIndex + 1
                    else
                        tokens.Add({ TokenStartLocation = i ; TokenEndLocation = i + 1 ; Token = NotEquals })
                        i <- i + 2
                | '[' ->
                    tokens.Add({ TokenStartLocation = i ; TokenEndLocation = i ; Token = OpenBrace })
                    i <- i + 1
                | ']' ->
                    tokens.Add({ TokenStartLocation = i ; TokenEndLocation = i ; Token = CloseBrace })
                    i <- i + 1
                | '(' ->
                    tokens.Add({ TokenStartLocation = i ; TokenEndLocation = i ; Token = OpenBracket })
                    i <- i + 1
                | ')' ->
                    tokens.Add({ TokenStartLocation = i ; TokenEndLocation = i ; Token = CloseBracket })
                    i <- i + 1
                | '%' ->
                    let len = IdentifierTokenizer.findLengthOfWord (i + 1) lastIndex line
                    if len = 0 then
                        tokens.Add({ TokenStartLocation = i ; TokenEndLocation = i ; Token = InvalidUnrecognised("%") })
                        i <- i + 1
                    else
                        tokens.Add({ TokenStartLocation = i ; TokenEndLocation = i + len ; Token = AttributeName(line.Substring(i + 1, len)) })
                        i <- i + len + 1
                | '$' ->
                    let len = IdentifierTokenizer.findLengthOfWord (i + 1) lastIndex line
                    if len = 0 then
                        tokens.Add({ TokenStartLocation = i ; TokenEndLocation = i ; Token = InvalidUnrecognised("%") })
                        i <- i + 1
                    else
                        tokens.Add({ TokenStartLocation = i ; TokenEndLocation = i + len ; Token = VariableName(line.Substring(i + 1, len)) })
                        i <- i + len + 1
                | '"' ->
                    let sb = StringBuilder()
                    let startIndex = i
                    i <- i + 1
                    while i <= lastIndex && line.[i] <> '"' do
                        if line.[i] = '\\' then
                            if i = lastIndex then
                                // Will add unrecognised at end.
                                i <- i + 1
                            else
                                sb.Append(line.[i + 1]) |> ignore
                                i <- i + 2
                        else
                            sb.Append(line.[i]) |> ignore
                            i <- i + 1
                    if i > lastIndex then
                        tokens.Add({ TokenStartLocation = startIndex ; TokenEndLocation = lastIndex ; Token = InvalidUnrecognised(line.Substring(startIndex, lastIndex - startIndex)) })
                    else
                        tokens.Add({ TokenStartLocation = startIndex ; TokenEndLocation = i ; Token = QuotedString(sb.ToString()) })
                        i <- i + 1
                | '/' ->
                    if i = lastIndex || line.[i + 1] <> '/' then
                        tokens.Add({ TokenStartLocation = i ; TokenEndLocation = lastIndex ; Token = InvalidUnrecognised(line.Substring(i)) })
                    // Comments don't need tokenizing.
                    i <- lastIndex + 1
                | _ ->
                    tokens.Add({ TokenStartLocation = i ; TokenEndLocation = lastIndex ; Token = InvalidUnrecognised(line.Substring(i)) })
                    i <- lastIndex + 1
        tokens |> Seq.toList
