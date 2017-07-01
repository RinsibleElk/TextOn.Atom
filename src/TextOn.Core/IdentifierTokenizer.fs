[<RequireQualifiedAccess>]
module internal TextOn.Core.IdentifierTokenizer

open System
open System.Text
open System.Collections.Generic

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

/// Tokenize something after an '#' symbol.
let legacyTokenizeInclude (tokens:List<AttributedToken>) startIndex lastIndex (line:string) =
    let mutable i = startIndex
    let len = findLengthOfWord (i + 1) lastIndex line
    if len = 0 then
        tokens.Add({ TokenStartLocation = (i + 1) ; TokenEndLocation = (i + 1) ; Token = InvalidUnrecognised("#") })
        i <- i + 1
    else
        let name = line.Substring(i + 1, len)
        match name with
        | "include" -> tokens.Add({ TokenStartLocation = (i + 1) ; TokenEndLocation = i + 1 + len ; Token = Include })
        | _ -> tokens.Add({ TokenStartLocation = (i + 1) ; TokenEndLocation = i + 1 + len ; Token = InvalidUnrecognised("#" + name) })
        i <- i + len + 1
    i

/// Tokenize something after an '@' symbol.
let tokenizeFunctionName (tokens:List<AttributedToken>) startIndex lastIndex (line:string) =
    let mutable i = startIndex
    let len = findLengthOfWord (i + 1) lastIndex line
    if len = 0 then
        tokens.Add({ TokenStartLocation = (i + 1) ; TokenEndLocation = (i + 1) ; Token = InvalidUnrecognised("@") })
        i <- i + 1
    else
        let name = line.Substring(i + 1, len)
        match name with
        | "var" -> tokens.Add({ TokenStartLocation = (i + 1) ; TokenEndLocation = i + 1 + len ; Token = Var })
        | "att" -> tokens.Add({ TokenStartLocation = (i + 1) ; TokenEndLocation = i + 1 + len ; Token = Att })
        | "func" -> tokens.Add({ TokenStartLocation = (i + 1) ; TokenEndLocation = i + 1 + len ; Token = Func })
        | "free" -> tokens.Add({ TokenStartLocation = (i + 1) ; TokenEndLocation = i + 1 + len ; Token = Free })
        | "break" -> tokens.Add({ TokenStartLocation = (i + 1) ; TokenEndLocation = i + 1 + len ; Token = Break })
        | "choice" -> tokens.Add({ TokenStartLocation = (i + 1) ; TokenEndLocation = i + 1 + len ; Token = Choice })
        | "seq" -> tokens.Add({ TokenStartLocation = (i + 1) ; TokenEndLocation = i + 1 + len ; Token = Sequential })
        | "private" -> tokens.Add({ TokenStartLocation = (i + 1) ; TokenEndLocation = i + 1 + len ; Token = Private })
        | "import" -> tokens.Add({ TokenStartLocation = (i + 1) ; TokenEndLocation = i + 1 + len ; Token = Import })
        | _ -> tokens.Add({ TokenStartLocation = (i + 1) ; TokenEndLocation = i + 1 + len ; Token = FunctionName(name) })
        i <- i + len + 1
    i

