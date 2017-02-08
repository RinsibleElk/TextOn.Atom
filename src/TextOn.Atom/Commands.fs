namespace TextOn.Atom

open System
open System.IO

[<AutoOpen>]
module Contract =
    type ParseRequest = { FileName : string; IsAsync : bool; Lines : string[] }
    type LintRequest = { FileName : string }

type Commands (serialize : Serializer) =
    let preprocess (file:FileInfo) lines = async { return Preprocessor.preprocess Preprocessor.realFileResolver file.FullName (Some file.Directory.FullName) lines }
    let stripComments lines = async { return CommentStripper.stripComments lines }
    let categorize lines = async { return LineCategorizer.categorize lines }
    let tokenize groups = async { return Tokenizer.tokenize groups }
    let parse tokens = async { return Parser.parse tokens }
    let compile source = async { return Compiler.compile source }
    let doCompile file lines =
        async {
            let! lines = preprocess file lines
            let! lines' = stripComments lines
            let! groups = categorize lines'
            let! tokens = groups |> List.map tokenize |> Async.Parallel
            let! source = tokens |> Array.map parse |> Async.Parallel
            let! output = compile (source |> List.ofArray)
            return Success output }
    let parse' file lines =
        async {
            let! result = doCompile file lines
            return
                match result with
                | Failure e -> [CommandResponse.error serialize e]
                | Success (compilationResult) ->
                    match compilationResult with
                    | CompilationResult.CompilationFailure(errors) ->
                        [ CommandResponse.errors serialize (errors, file.FullName) ]
                    | _ ->
                        let errors = [||]
                        [ CommandResponse.errors serialize (errors, file.FullName) ] }
    let someTextWriter = new System.IO.StreamWriter(new FileStream(@"D:\Documents\TextOn.Output\out.txt", FileMode.OpenOrCreate, FileAccess.Write))
    member __.Parse file lines =
        fprintf someTextWriter @"Asked to parse %s" file
        async {
            let file = Path.GetFullPath file
            return! parse' (FileInfo file) lines }

    member __.Lint (file: SourceFilePath) = async {
        fprintf someTextWriter @"Asked to lint %s" file
        let file = Path.GetFullPath file
        let file = file |> FileInfo
        let! result = doCompile file (file.FullName |> File.ReadAllLines |> List.ofArray)
        return
            match result with
            | Failure e -> [CommandResponse.error serialize e]
            | Success (compilationResult) ->
                match compilationResult with
                | CompilationResult.CompilationFailure(errors) ->
                    [ CommandResponse.errors serialize (errors, file.FullName) ]
                | _ ->
                    let errors = [||]
                    [ CommandResponse.errors serialize (errors, file.FullName) ] }
