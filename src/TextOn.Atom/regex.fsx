#I __SOURCE_DIRECTORY__
#r "bin/Debug/TextOn.Atom.exe"
open TextOn.Atom
open System
open System.IO
open System.Text.RegularExpressions

let stopwatch = new System.Diagnostics.Stopwatch()
stopwatch.Start()
// Jonas: here's the pipeline so far. I want to test this with more complex examples like the original ones, once we've converted.
let file =
    [
        @"D:\NodeJs\TextOn.Atom\examples\example.texton"
        @"/Users/Oliver/Projects/TextOn.Atom/TextOn.Atom/examples/example.texton"
    ]
    |> List.find File.Exists
    |> FileInfo
let preprocessed = Preprocessor.preprocess Preprocessor.realFileResolver file.Name (Some file.Directory.FullName) (file.FullName |> File.ReadAllLines |> List.ofArray)
let stripped = CommentStripper.stripComments preprocessed
let categorized = stripped |> LineCategorizer.categorize
let tokenized = categorized |> Seq.map (fun a -> async { return Tokenizer.tokenize a }) |> Async.Parallel |> Async.RunSynchronously
let parsed = tokenized |> Array.map Parser.parse
let compiled = parsed |> Compiler.compile
stopwatch.Stop()
stopwatch.ElapsedMilliseconds

