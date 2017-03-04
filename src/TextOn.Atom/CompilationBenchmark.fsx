#I __SOURCE_DIRECTORY__
#r "bin/Debug/TextOn.Atom.exe"
#r "bin/Debug/TextOn.Atom.DTO.dll"
open TextOn.Atom
open System
open System.Diagnostics
open System.IO
open System.Text.RegularExpressions

let fileName = Path.Combine(__SOURCE_DIRECTORY__, @"Benchmarking.texton")
let directory = @""
let lines =
    fileName
    |> File.ReadAllLines
    |> List.ofArray

let makeTokenized() =
    Preprocessor.preprocess (fun _ _ -> failwith "") fileName directory lines
    |> CommentStripper.stripComments
    |> LineCategorizer.categorize
    |> List.map Tokenizer.tokenize
let timeTokenized() =
    let sw = Stopwatch()
    sw.Start()
    makeTokenized() |> ignore
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
let resultsTokenized = [ 0 .. 999 ] |> List.map (fun _ -> timeTokenized()) |> meanAndStdev
let resultsStripped = [ 0 .. 999 ] |> List.map (fun _ -> timeStripped()) |> meanAndStdev

// Master (Oliver's PC):
// val resultsTokenized : float * float = (6.2361054, 1.924761209)
// val resultsStripped : float * float = (0.4971996, 0.07256326729)


