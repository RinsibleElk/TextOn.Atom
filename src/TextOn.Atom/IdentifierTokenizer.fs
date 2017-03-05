namespace TextOn.Atom

open System

[<RequireQualifiedAccess>]
module internal IdentifierTokenizer =
    /// Find the length of a "word".
    let findLengthOfWord startIndex lastIndex (line:string) =
        let mutable i = startIndex
        while i <= lastIndex && (Char.IsLetterOrDigit(line.[i]) || line.[i] = '_') do
            i <- i + 1
        i - startIndex
