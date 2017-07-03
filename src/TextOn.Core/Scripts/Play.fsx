#r @"../bin/Debug/TextOn.Core.dll"

open System
open System.IO
open TextOn.Core.Compiling
open TextOn.Core.Linking
open TextOn.Core.Parsing

let modules =
    @"D:\NodeJs\TextOn.Atom\examples"
    |> DirectoryInfo
    |> fun di -> di.GetFiles()
    |> List.ofArray
    |> List.map
        (fun fi ->
            let file = fi.FullName
            Compiler.compile file (file |> File.ReadAllLines |> List.ofArray))
let template = Linker.link @"D:\NodeJs\TextOn.Atom\examples\example.texton" modules
@"D:\NodeJs\TextOn.Atom\examples\cities.texton"
|> File.ReadAllLines
|> Array.map (fun x -> x.Replace("\"", "\\\""))
|> Array.iter (printfn "\"%s\"")
