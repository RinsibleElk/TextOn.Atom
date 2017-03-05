namespace TextOn.Atom

open System

[<RequireQualifiedAccess>]
/// Tokenize pre-categorized data.
module Tokenizer =
    /// Do the context-specific tokenization.
    let tokenize (group:CategorizedLines) : CategorizedAttributedTokenSet =
        let tokens =
            match group.Category with
            | CategorizedFuncDefinition ->
                group.Lines
                |> List.map
                    (fun line ->
                        async {
                            return
                                {
                                    LineNumber = line.CurrentFileLineNumber
                                    Tokens = match line.Contents with | PreprocessorLine line -> FunctionLineTokenizer.tokenizeLine line |> Seq.toList | _ -> failwithf "Unexpected line contents for categorized func definition %A" line.Contents
                                } })
                |> Async.Parallel
                |> Async.RunSynchronously
                |> List.ofArray
            | CategorizedVarDefinition ->
                group.Lines
                |> List.map
                    (fun line ->
                        async {
                            return
                                {
                                    LineNumber = line.CurrentFileLineNumber
                                    Tokens = match line.Contents with | PreprocessorLine line -> VariableOrAttributeLineTokenizer.tokenizeLine line | _ -> failwithf "Unexpected line contents for categorized variable definition %A" line.Contents
                                } })
                |> Async.Parallel
                |> Async.RunSynchronously
                |> List.ofArray
            | CategorizedAttDefinition ->
                group.Lines
                |> List.map
                    (fun line ->
                        async {
                            return
                                {
                                    LineNumber = line.CurrentFileLineNumber
                                    Tokens = match line.Contents with | PreprocessorLine line -> VariableOrAttributeLineTokenizer.tokenizeLine line | _ -> failwithf "Unexpected line contents for categorized attribute definition %A" line.Contents
                                } })
                |> Async.Parallel
                |> Async.RunSynchronously
                |> List.ofArray
            | _ ->
                group.Lines
                |> List.map
                    (fun line ->
                        {   LineNumber = line.CurrentFileLineNumber
                            Tokens = [(match line.Contents with | PreprocessorError error -> { TokenStartLocation = error.StartLocation ; TokenEndLocation = error.EndLocation ; Token = InvalidPreprocessorError error.ErrorText } | PreprocessorLine text -> { TokenStartLocation = 1 ; TokenEndLocation = text.Length ; Token = InvalidUnrecognised text })] })
        {
            Category = group.Category
            StartLine = group.StartLine
            EndLine = group.EndLine
            Index = group.Index
            File = group.File
            Tokens = tokens
        }
