[<RequireQualifiedAccess>]
module TextOn.Core.Pipeline.Builder

open System.IO
open TextOn.Core.Tokenizing
open TextOn.Core.Parsing
open TextOn.Core.Compiling
open TextOn.Core.Linking
open TextOn.Core.Utils

/// Naive non-caching full build.
let build fileResolver file lines =
    let compiled = Compiler.compile file lines
    let mutable allCompiled = [(Utils.getNormalizedPath file, compiled)] |> Map.ofList
    let mutable requiredImports =
        compiled.ImportedFiles
        |> List.map (fun i -> Utils.getNormalizedImportPath file i.ImportedFileName, Utils.getImportPath file i.ImportedFileName)
        |> Map.ofList
        |> Map.filter (fun k _ -> allCompiled |> Map.containsKey k |> not)
    while requiredImports |> Map.isEmpty |> not do
        let newCompiled =
            requiredImports
            |> Map.toArray
            |> Array.choose (fun (k,f) -> (fileResolver k f) |> Option.map (fun lines -> (k,f,lines)))
            |> Array.map
                (fun (k,f,lines) ->
                    async {
                        let compiled = Compiler.compile f lines
                        return (k, compiled) })
            |> Async.Parallel
            |> Async.RunSynchronously
            |> List.ofArray
        requiredImports <- Map.empty
        newCompiled |> List.iter (fun (k,c) -> allCompiled <- allCompiled |> Map.add k c)
        newCompiled
        |> List.collect (fun (_,c) -> c.ImportedFiles |> List.map (fun i -> Utils.getNormalizedImportPath c.File i.ImportedFileName, Utils.getImportPath c.File i.ImportedFileName))
        |> List.filter (fun (k,_) -> allCompiled |> Map.containsKey k |> not)
        |> Map.ofList
        |> Map.iter (fun k f -> requiredImports <- requiredImports |> Map.add k f)
    allCompiled
    |> Map.toList
    |> List.map snd
    |> Linker.link file
