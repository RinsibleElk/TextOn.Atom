#I __SOURCE_DIRECTORY__
#r "bin/Debug/TextOn.Atom.exe"
open TextOn.Atom
open System
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

let normalString = "Hello"
let specialString = "Héllo"
let withNumbersString = "Hello_$9"
let re = Regex("^\w+$", RegexOptions.CultureInvariant)
re.Match(withNumbersString)

let trailingWhitespaceRegex = Regex(@"^(.*?)\s+$")
let stripTrailingWhitespace l =
    let m = trailingWhitespaceRegex.Match(l)
    if m.Success then m.Groups.[1].Value
    else l


[
    "@func @main"
    "{"
    "  @seq {"
    "    You are a bloke. [%Gender = \"Male\"]"
    "    You live in {$City|a city in $Country}."
    "    We are in $City which is in $Country."
    "    @break"
    "    @guyStuff [%Gender = \"Male\"]"
    "  }"
    "}"
]
|> Seq.map (fun a -> (a, DefinitionLineTokenizer.tokenizeLine a))
|> Seq.iter
    (fun (x,y) ->
        printfn "(\"%s\", [%s])" x (String.Join("; ", (y |> Seq.map (fun z -> sprintf "{StartIndex = %d;EndIndex = %d;Token = %A}" z.StartIndex z.EndIndex z.Token)))))



