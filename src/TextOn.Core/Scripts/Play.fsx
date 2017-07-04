#r @"../bin/Debug/TextOn.Core.dll"

open System
open System.IO
open TextOn.Core.Compiling
open TextOn.Core.Linking
open TextOn.Core.Parsing
open TextOn.Core.Pipeline

let template =
    @"D:\NodeJs\TextOn.Atom\examples\example.texton"
    |> fun file ->
        let lines = file |> File.ReadAllLines |> List.ofArray
        Builder.build file lines

