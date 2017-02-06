#I __SOURCE_DIRECTORY__
#r "bin/Debug/TextOn.Atom.exe"
open TextOn.Atom
open System
open System.IO
open System.Text.RegularExpressions

// Jonas: here's the pipeline so far. I want to test this with more complex examples like the original ones, once we've converted.
let file =
    [
        @"D:\NodeJs\TextOn.Atom\examples\example.texton"
        @"/Users/Oliver/Projects/TextOn.Atom/TextOn.Atom/examples/example.texton"
    ]
    |> List.find File.Exists
    |> FileInfo
let compiled =
    Preprocessor.preprocess Preprocessor.realFileResolver file.Name (Some file.Directory.FullName) (file.FullName |> File.ReadAllLines |> List.ofArray)
    |> CommentStripper.stripComments
    |> LineCategorizer.categorize
    |> List.map (Tokenizer.tokenize >> Parser.parse)
    |> Compiler.compile
