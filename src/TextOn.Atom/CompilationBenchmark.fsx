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
        @"D:\NodeJs\TextOn.Atom\src\TextOn.Atom\Benchmarking.texton"
        @"/Users/Oliver/Projects/TextOn.Atom/src/TextOn.Atom/Benchmarking.texton"
        @"/Users/jonaskiessling/Documents/src/TextOn.Atom/Benchmarking.texton"
    ]
    |> List.find File.Exists
    |> FileInfo
let fileName = Path.Combine(file.Directory.FullName, file.Name)
let directory = file.Directory.FullName
let lines =
    fileName
    |> File.ReadAllLines
    |> List.ofArray
let makeTokenized() =
    Preprocessor.preprocess (fun _ _ -> failwith "") file.Name directory lines
    |> CommentStripper.stripComments
    |> LineCategorizer.categorize
    |> List.map Tokenizer.tokenize
let timeTokenized() =
    let sw = Stopwatch()
    sw.Start()
    makeTokenized() |> ignore
    sw.Stop()
    sw.Elapsed.TotalMilliseconds
let makeCategorized() =
    Preprocessor.preprocess (fun _ _ -> failwith "") fileName directory lines
    |> CommentStripper.stripComments
    |> LineCategorizer.categorize
let timeCategorized() =
    let sw = Stopwatch()
    sw.Start()
    makeCategorized() |> ignore
    sw.Stop()
    sw.Elapsed.TotalMilliseconds
let makeStripped() =
    Preprocessor.preprocess (fun _ _ -> failwith "") fileName directory lines
    |> CommentStripper.stripComments
let timeStripped() =
    let sw = Stopwatch()
    sw.Start()
    makeStripped() |> ignore
    sw.Stop()
    sw.Elapsed.TotalMilliseconds
let meanAndStdev l =
    l
    |> List.fold (fun (s,ss,n) x -> (s + x, ss + x * x, n + 1.0)) (0.0,0.0,0.0)
    |> fun (s,ss,n) -> ((s/n),(ss/n)) |> fun (e,e2) -> (e,(sqrt (e2 - e * e)))
let resultsTokenized = [ 0 .. 99 ] |> List.map (fun _ -> timeTokenized()) |> meanAndStdev
let resultsStripped = [ 0 .. 99 ] |> List.map (fun _ -> timeStripped()) |> meanAndStdev
let resultsCategorized = [ 0 .. 99 ] |> List.map (fun _ -> timeCategorized()) |> meanAndStdev

// Master (Oliver's PC):
// val resultsTokenized : float * float = (6.2361054, 1.924761209)
// val resultsStripped : float * float = (0.4971996, 0.07256326729)

// IrregularExpressions (Oliver's PC):
// val resultsTokenized : float * float = (6.843914, 4.429226291)
// val resultsStripped : float * float = (0.36916, 0.07727673389)
// val resultsCategorized : float * float = (0.652632, 0.1008072883)

// IrregularExpressions (Oliver's Mac):
// val resultsTokenized : float * float = (53.326875, 16.15775433) - yikes
// val resultsStripped : float * float = (0.812813, 0.4466267669)
// val resultsCategorized : float * float = (2.345453, 0.622461468)

