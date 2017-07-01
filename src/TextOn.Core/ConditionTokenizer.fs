[<RequireQualifiedAccess>]
module internal TextOn.Core.ConditionTokenizer

open System
open System.Text
open System.Collections.Generic

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
                tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = i + 1 ; Token = Equals })
                i <- i + 1
            | '<' ->
                if i = lastIndex then
                    tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = lastIndex + 1 ; Token = InvalidUnrecognised(line.Substring(i)) })
                    i <- lastIndex + 1
                else if line.[i + 1] <> '>' then
                    tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = lastIndex + 1 ; Token = InvalidUnrecognised(line.Substring(i)) })
                    i <- lastIndex + 1
                else
                    tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = i + 1 + 1 ; Token = NotEquals })
                    i <- i + 2
            | '&' ->
                if i = lastIndex then
                    tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = lastIndex + 1 ; Token = InvalidUnrecognised(line.Substring(i)) })
                    i <- lastIndex + 1
                else if line.[i + 1] <> '&' then
                    tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = lastIndex + 1 ; Token = InvalidUnrecognised(line.Substring(i)) })
                    i <- lastIndex + 1
                else
                    tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = i + 1 + 1 ; Token = And })
                    i <- i + 2
            | '|' ->
                if i = lastIndex then
                    tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = lastIndex + 1 ; Token = InvalidUnrecognised(line.Substring(i)) })
                    i <- lastIndex + 1
                else if line.[i + 1] <> '|' then
                    tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = lastIndex + 1 ; Token = InvalidUnrecognised(line.Substring(i)) })
                    i <- lastIndex + 1
                else
                    tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = i + 1 + 1 ; Token = Or })
                    i <- i + 2
            | '[' ->
                tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = i + 1 ; Token = OpenBrace })
                i <- i + 1
            | ']' ->
                tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = i + 1 ; Token = CloseBrace })
                i <- i + 1
            | '(' ->
                tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = i + 1 ; Token = OpenBracket })
                i <- i + 1
            | ')' ->
                tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = i + 1 ; Token = CloseBracket })
                i <- i + 1
            | '%' ->
                let len = IdentifierTokenizer.findLengthOfWord (i + 1) lastIndex line
                if len = 0 then
                    tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = i + 1 ; Token = InvalidUnrecognised("%") })
                    i <- i + 1
                else
                    tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = i + 1 + len ; Token = AttributeName(line.Substring(i + 1, len)) })
                    i <- i + len + 1
            | '$' ->
                let len = IdentifierTokenizer.findLengthOfWord (i + 1) lastIndex line
                if len = 0 then
                    tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = i + 1 ; Token = InvalidUnrecognised("$") })
                    i <- i + 1
                else
                    tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = i + 1 + len ; Token = VariableName(line.Substring(i + 1, len)) })
                    i <- i + len + 1
            | '"' ->
                i <- IdentifierTokenizer.tokenizeQuotedString tokens i lastIndex line
            | '/' ->
                if i = lastIndex || line.[i + 1] <> '/' then
                    tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = lastIndex + 1 ; Token = InvalidUnrecognised(line.Substring(i)) })
                // Comments don't need tokenizing.
                i <- lastIndex + 1
            | _ ->
                tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = lastIndex + 1 ; Token = InvalidUnrecognised(line.Substring(i)) })
                i <- lastIndex + 1
    tokens |> Seq.toList

