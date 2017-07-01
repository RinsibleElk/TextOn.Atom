[<RequireQualifiedAccess>]
module internal TextOn.Core.VariableOrAttributeLineTokenizer

open System
open System.Collections.Generic

/// Tokenize a line within a variable or attribute definition.
let tokenizeLine (line:string) =
    let lastIndex = line.Length - 1
    let mutable i = 0
    let tokens = List<AttributedToken>()
    while i <= lastIndex do
        let c = line.[i]
        if Char.IsWhiteSpace c then
            i <- i + 1
        else
            match c with
            | '[' ->
                tokens.AddRange(ConditionTokenizer.tokenizeCondition i lastIndex line)
                i <- lastIndex + 1
            | '/' ->
                if i = lastIndex || line.[i + 1] <> '/' then
                    tokens.Add({ TokenStartLocation = i + 1 ; TokenEndLocation = lastIndex + 1 ; Token = InvalidUnrecognised(line.Substring(i)) })
                // Comments don't need tokenizing.
                i <- lastIndex + 1
            | '=' ->
                tokens.Add({ TokenStartLocation = (i + 1) ; TokenEndLocation = (i + 1) ; Token = Equals })
                i <- i + 1
            | '"' ->
                i <- IdentifierTokenizer.tokenizeQuotedString tokens i lastIndex line
            | '@' ->
                i <- IdentifierTokenizer.tokenizeFunctionName tokens i lastIndex line
            | '$' ->
                let len = IdentifierTokenizer.findLengthOfWord (i + 1) lastIndex line
                if len = 0 then
                    tokens.Add({ TokenStartLocation = (i + 1) ; TokenEndLocation = (i + 1) ; Token = InvalidUnrecognised("$") })
                    i <- i + 1
                else
                    tokens.Add({ TokenStartLocation = (i + 1) ; TokenEndLocation = i + 1 + len ; Token = VariableName(line.Substring(i + 1, len)) })
                    i <- i + len + 1
            | '%' ->
                let len = IdentifierTokenizer.findLengthOfWord (i + 1) lastIndex line
                if len = 0 then
                    tokens.Add({ TokenStartLocation = (i + 1) ; TokenEndLocation = (i + 1) ; Token = InvalidUnrecognised("%") })
                    i <- i + 1
                else
                    tokens.Add({ TokenStartLocation = (i + 1) ; TokenEndLocation = i + 1 + len ; Token = AttributeName(line.Substring(i + 1, len)) })
                    i <- i + len + 1
            | '{' ->
                tokens.Add({ TokenStartLocation = (i + 1) ; TokenEndLocation = (i + 1) ; Token = OpenCurly })
                i <- i + 1
            | '}' ->
                tokens.Add({ TokenStartLocation = (i + 1) ; TokenEndLocation = (i + 1) ; Token = CloseCurly })
                i <- i + 1
            | _ ->
                tokens.Add({ TokenStartLocation = (i + 1) ; TokenEndLocation = (lastIndex + 1) ; Token = InvalidUnrecognised(line.Substring(i)) })
                i <- lastIndex + 1
    tokens |> Seq.toList
