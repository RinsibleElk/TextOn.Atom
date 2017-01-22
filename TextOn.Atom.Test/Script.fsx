
#I @"bin/Debug"
#r @"TextOn.Atom"
#r @"TextOn.Atom.Test"

open TextOn.Atom
open System.Text.RegularExpressions
open System.IO

let dir = @"D:\NodeJs\TextOn.Atom\examples"
let file = Path.Combine(dir, "example.texton")
let lines = Preprocessor.preprocess Preprocessor.realFileResolver file (Some dir) (file |> File.ReadAllLines |> List.ofArray)
