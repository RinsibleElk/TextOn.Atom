#I __SOURCE_DIRECTORY__
#r "bin/Debug/TextOn.Atom.exe"
open TextOn.Atom
open System.IO
open System.Text.RegularExpressions

let quotedStringRegex =
    Regex("^(\\s*)\"(([^\\\"\\\\]+|\\\\\\\"|\\\\\\\\)*)\"\\s*")

let lines =
    [
        "@var $City"
        "  \"Which city are you writing about?\""
        "  {"
        "    \"London\" [$Country = \"U.K.\"]"
        "    \"Berlin\" [$Country = \"Germany\" && %Gender = \"Male\"]"
        "    \"Paris\" [$Country <> \"Germany\" && $Country <> \"U.K.\"]"
        "    *"
        "  }"
    ]
let tokens =
    lines
    |> Seq.collect VariableLineTokenizer.tokenizeLine
tokens |> Seq.iter (fun x -> printfn "%A" x.Token)
