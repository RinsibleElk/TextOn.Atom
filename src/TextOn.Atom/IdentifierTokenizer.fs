namespace TextOn.Atom

open System
open System.Text
open System.Collections.Generic

[<RequireQualifiedAccess>]
module internal IdentifierTokenizer =
    /// Find the length of a "word".
    let findLengthOfWord startIndex lastIndex (line:string) =
        let mutable i = startIndex
        while i <= lastIndex && (Char.IsLetterOrDigit(line.[i]) || line.[i] = '_') do
            i <- i + 1
        i - startIndex

    /// Tokenize a quoted string.
    let tokenizeQuotedString (tokens:List<AttributedToken>) startIndex lastIndex (line:string) =
        let sb = StringBuilder()
        // Assume we've been given something starting with '"'
        let mutable i = startIndex + 1
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
            tokens.Add({ TokenStartLocation = (startIndex + 1) ; TokenEndLocation = lastIndex + 1 ; Token = InvalidUnrecognised(line.Substring(startIndex, lastIndex - startIndex + 1)) })
        else
            tokens.Add({ TokenStartLocation = (startIndex + 1) ; TokenEndLocation = i + 1 ; Token = QuotedString(sb.ToString()) })
            i <- i + 1
        i
