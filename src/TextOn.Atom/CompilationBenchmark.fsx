#I __SOURCE_DIRECTORY__
#r "bin/Debug/TextOn.Atom.exe"
#r "bin/Debug/TextOn.Atom.DTO.dll"
open TextOn.Atom
open System
open System.Diagnostics
open System.IO
open System.Text.RegularExpressions

let file =
    [
        @"D:\NodeJs\TextOn.Atom\examples\original\sixt.texton"
        @"/Users/Oliver/Projects/TextOn.Atom/TextOn.Atom/examples/original/sixt.texton"
        @"/Users/jonaskiessling/Documents/TextOn.Atom/examples/original/sixt.texton"
    ]
    |> List.find File.Exists
    |> FileInfo
let makeTokenized() =
    Preprocessor.preprocess Preprocessor.realFileResolver file.Name file.Directory.FullName (file.FullName |> File.ReadAllLines |> List.ofArray)
    |> CommentStripper.stripComments
    |> LineCategorizer.categorize
    |> List.map Tokenizer.tokenize
let timeTokenized() =
    let sw = Stopwatch()
    sw.Start()
    makeTokenized() |> ignore
    sw.Stop()
    sw.Elapsed.TotalMilliseconds
let meanAndStdev l =
    l
    |> List.fold (fun (s,ss,n) x -> (s + x, ss + x * x, n + 1.0)) (0.0,0.0,0.0)
    |> fun (s,ss,n) -> ((s/n),(ss/n)) |> fun (e,e2) -> (e,(sqrt (e2 - e * e)))
let results = [ 0 .. 999 ] |> List.map (fun _ -> timeTokenized()) |> meanAndStdev

// Master (Oliver's PC):
// Cold: val results : float * float = (7.4268538, 2.246032421)
// Hot: val results : float * float = (7.2207699, 0.9147641481)

