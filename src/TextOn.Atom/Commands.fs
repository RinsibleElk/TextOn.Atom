﻿namespace TextOn.Atom

open System
open System.IO

type Commands (serialize : Serializer) =
    let fileLinesMap = System.Collections.Concurrent.ConcurrentDictionary<string, string list>()
    //TODO: Make thread-safe.
    let mutable generator = None
    let add fileName directory lines =
        let key = Path.Combine(directory, fileName).ToLower()
        let f = System.Func<string, string list, string list>(fun _ _ -> lines)
        fileLinesMap.AddOrUpdate(key, lines, f) |> ignore
    let fileResolver f d =
        let o =
            let (ok, r) = fileLinesMap.TryGetValue(Path.Combine(d, f).ToLower())
            if ok then Some r
            else None
            |> Option.map (fun x -> (f, d, x))
        if o.IsNone then Preprocessor.realFileResolver f d
        else o
    let doCompile fileName directory lines =
        async {
            let lines = Preprocessor.preprocess fileResolver fileName directory lines
            let lines' = CommentStripper.stripComments lines
            let groups = LineCategorizer.categorize lines'
            let tokens = groups |> List.map Tokenizer.tokenize
            let source = tokens |> List.map Parser.parse
            let output = Compiler.compile source
            return Success output }
    let parse' fileName directory lines =
        async {
            add fileName directory lines
            let! result = doCompile fileName directory lines
            return
                match result with
                | Failure e -> [CommandResponse.error serialize e]
                | Success (compilationResult) ->
                    match compilationResult with
                    | CompilationResult.CompilationFailure(errors) ->
                        [ CommandResponse.errors serialize (errors, fileName) ]
                    | _ ->
                        let errors = [||]
                        [ CommandResponse.errors serialize (errors, fileName) ] }

    let doGenerateStart fileName directory lines line = async {
        add fileName directory lines
        let! compileResult = doCompile fileName directory lines
        return
            match compileResult with
            | Failure e -> Failure e
            | Success compilationResult ->
                match compilationResult with
                | CompilationResult.CompilationFailure errors -> Success (GeneratorStartResult.CompilationFailure errors)
                | CompilationResult.CompilationSuccess template ->
                    template.Functions
                    |> Array.tryFind (fun f -> f.File = fileName && f.StartLine <= line && f.EndLine >= line)
                    |> Option.map
                        (fun f ->
                            let g = GeneratorServer(f.File, f.Name)
                            g.UpdateTemplate(template)
                            generator <- Some g
                            Success (GeneratorStartResult.GeneratorStarted generator.Value.Data))
                    |> defaultArg <| Failure "Nothing to generate" }

    member __.Parse file lines =
        async {
            let lines = lines |> List.ofArray
            let fi = Path.GetFullPath file |> FileInfo
            return! parse' file fi.Directory.FullName lines }

    member __.GenerateStart (file:SourceFilePath) lines line = async {
        let fi = Path.GetFullPath file |> FileInfo
        let! result = doGenerateStart file fi.Directory.FullName lines line
        return
            match result with
            | Failure e -> [CommandResponse.error serialize e]
            | Success (generateStartResult) ->
                match generateStartResult with
                | GeneratorStartResult.CompilationFailure(errors) ->
                    [ CommandResponse.error serialize "Nothing to generate"]
                | GeneratorStartResult.GeneratorStarted(generatorSetup) ->
                    [ CommandResponse.generatorSetup serialize generatorSetup ] }

    member __.GenerateStop () = async {
        generator <- None
        return [] }

    member __.GeneratorValueSet ty name value = async {
        return
            if generator.IsSome then
                generator.Value.SetValue ty name value
                [ CommandResponse.generatorSetup serialize generator.Value.Data ]
            else [ CommandResponse.error serialize "Nothing to generate" ] }

    member __.Generate config = async {
        return
            if generator.IsSome then
                generator.Value.Generate config
                [ CommandResponse.generatorSetup serialize generator.Value.Data ]
            else [ CommandResponse.error serialize "Nothing to generate" ] }

    member __.UpdateGenerator () = async {
        if generator.IsSome then
            let generator = generator.Value
            let fi = generator.File
            let lines = fileResolver fi.Name fi.Directory.FullName
            if lines.IsSome then
                let (file, directory, lines) = lines.Value
                let! compileResult = doCompile file directory lines
                match compileResult with
                | Success r ->
                    match r with
                    | CompilationResult.CompilationSuccess template ->
                        generator.UpdateTemplate template
                        return
                            [ CommandResponse.generatorSetup serialize generator.Data ]
                    | _ -> return []
                | _ -> return []
            else return []
        else return [] }

    member __.Navigate (file:SourceFilePath) ty name = async {
        let fi = Path.GetFullPath file |> FileInfo
        let lines = fileResolver fi.Name fi.Directory.FullName
        if lines |> Option.isSome then
            let (file, directory, lines) = lines.Value
            let! compileResult = doCompile file directory lines
            return
                match compileResult with
                | Failure e -> [CommandResponse.error serialize e]
                | Success (compilationResult) ->
                    match compilationResult with
                    | CompilationResult.CompilationFailure(errors) ->
                        [ CommandResponse.error serialize "File had compilation errors" ]
                    | CompilationResult.CompilationSuccess template ->
                        let errors = [||]
                        let f =
                            match ty with
                            | "Function" -> template.Functions |> Array.tryFind (fun fn -> fn.Name = name) |> Option.map (fun fn -> fn.File, fn.StartLine)
                            | "Variable" -> template.Variables |> Array.tryFind (fun fn -> fn.Name = name) |> Option.map (fun fn -> fn.File, fn.StartLine)
                            | "Attribute" -> template.Attributes |> Array.tryFind (fun fn -> fn.Name = name) |> Option.map (fun fn -> fn.File, fn.StartLine)
                            | _ -> failwith "Internal error"
                        if f |> Option.isNone then
                            [ CommandResponse.error serialize "Function not found" ]
                        else
                            let (f, l) = f.Value
                            [ CommandResponse.navigate serialize { FileName = f ; LineNumber = l ; Location = 1 } ]
        else
            return [CommandResponse.error serialize "File not found"] }
