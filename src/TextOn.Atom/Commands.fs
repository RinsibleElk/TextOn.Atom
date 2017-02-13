namespace TextOn.Atom

open System
open System.IO

[<AutoOpen>]
module Contract =
    type ParseRequest = { FileName : string; IsAsync : bool; Lines : string[] }
    type LintRequest = { FileName : string }

type Commands (serialize : Serializer) =
    let fileLinesMap = System.Collections.Concurrent.ConcurrentDictionary<string, string list>()
    let add fileName directory lines =
        let key = Path.Combine(directory, fileName).ToLower()
        let f = System.Func<string, string list, string list>(fun _ _ -> lines)
        fileLinesMap.AddOrUpdate(key, lines, f) |> ignore
    let fileResolver f d =
        let o =
            d
            |> Option.bind
                (fun d ->
                    let (ok, r) = fileLinesMap.TryGetValue(Path.Combine(d, f).ToLower())
                    if ok then Some r
                    else None)
            |> Option.map (fun x -> (f, d, x))
        if o.IsNone then Preprocessor.realFileResolver f d
        else o
    let doCompile fileName directory lines =
        async {
            let lines = Preprocessor.preprocess fileResolver fileName (Some directory) lines
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
    member __.Parse file lines =
        async {
            let fi = Path.GetFullPath file |> FileInfo
            return! parse' file fi.Directory.FullName lines }

    member __.Lint (file: SourceFilePath) = async {
        let file = Path.GetFullPath file
        let res = [ CommandResponse.lint serialize [] ]
        return res }
