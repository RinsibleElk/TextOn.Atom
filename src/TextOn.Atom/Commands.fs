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
    let doCompile fileName directory lines =
        async {
            let lines = Preprocessor.preprocess Preprocessor.realFileResolver fileName (Some directory) lines
            let lines' = CommentStripper.stripComments lines
            let groups = LineCategorizer.categorize lines'
            let tokens = groups |> List.map Tokenizer.tokenize
            let source = tokens |> List.map Parser.parse
            let output = Compiler.compile source
            return Success output }
    let parse' fileName directory lines =
        async {
            let! result = doCompile fileName directory lines
            return
                match result with
                | Failure e -> [CommandResponse.error serialize e]
                | Success (compilationResult) ->
                    match compilationResult with
                    | CompilationResult.CompilationFailure(errors) ->
                        [ CommandResponse.errors serialize (errors, fileName) ]
                    | _ ->
                        let errors = [|(GeneralError({File=fileName;ErrorText="Hello world"}))|]
                        [ CommandResponse.errors serialize (errors, fileName) ] }
    member __.Parse file lines =
        async {
            let fi = Path.GetFullPath file |> FileInfo
            return! parse' file fi.Directory.FullName lines }

    member __.Lint (file: SourceFilePath) = async {
        let errors = [|(GeneralError({File=file;ErrorText="Hello world"}))|]
        return [ CommandResponse.errors serialize (errors, file) ] }
