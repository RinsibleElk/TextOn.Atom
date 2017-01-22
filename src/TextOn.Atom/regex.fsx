#I @"bin/Debug"
#r @"TextOn.Atom"

open System.Text.RegularExpressions
open System
open System.IO

let dir = @"D:\NodeJs\TextOn.Atom\examples"
let file = Path.Combine(dir, "example.texton")
let lines = Preprocessor.preprocess Preprocessor.realFileResolver file (Some dir) (file |> File.ReadAllLines |> List.ofArray)

let line = "@func @hello()"
let remaining = line.Substring(5)
let off = remaining.IndexOf("@")
let name = remaining.Substring(1 + off)

open System.Text.RegularExpressions
let regex = Regex("^([a-zA-Z][a-zA-Z0-9_]*)(\(\))?")
let m = regex.Match("helloWorld")
m.Success
m.Groups.Count
m.Groups.[2]

let somethingThatTakes1Sec() =
    [ 0 .. 9999999 ]
    |> Seq.iter ignore
let makeAsync (n:int) =
    async {
        [ 1 .. 5 ]
        |> List.iter
            (fun m ->
                printfn "Running %d%s" n ([ 0 .. (m - 1) ] |> Seq.map (fun _ -> ".") |> fun s -> String.Join("", s))
                somethingThatTakes1Sec())
        printfn "Returning %d" n
        return n }
let results =
    [ 0 .. 9 ]
    |> List.map makeAsync
    |> Async.Parallel
    |> Async.RunSynchronously

