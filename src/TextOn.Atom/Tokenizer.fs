namespace TextOn.Atom

open System
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
/// Tokenize pre-categorized data.
module Tokenizer =
    /// Do the context-specific tokenization.
    let tokenize (group:CategorizedLines) : CategorizedAttributedTokenSet =
        let tokens =
            match group.Category with
            | CategorizedFuncDefinition ->
                group.Lines
                |> Seq.map
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
                |> Seq.map
                    (fun line ->
                        async {
                            return
                                {
                                    LineNumber = line.CurrentFileLineNumber
                                    Tokens = match line.Contents with | PreprocessorLine line -> VariableLineTokenizer.tokenizeLine line |> Seq.toList | _ -> failwithf "Unexpected line contents for categorized variable definition %A" line.Contents
                                } })
                |> Async.Parallel
                |> Async.RunSynchronously
                |> List.ofArray
            | CategorizedAttDefinition ->
                group.Lines
                |> Seq.map
                    (fun line ->
                        async {
                            return
                                {
                                    LineNumber = line.CurrentFileLineNumber
                                    Tokens = match line.Contents with | PreprocessorLine line -> AttributeLineTokenizer.tokenizeLine line |> Seq.toList | _ -> failwithf "Unexpected line contents for categorized attribute definition %A" line.Contents
                                } })
                |> Async.Parallel
                |> Async.RunSynchronously
                |> List.ofArray
            | _ -> []
        {
            Category = group.Category
            StartLine = group.StartLine
            EndLine = group.EndLine
            Index = group.Index
            File = group.File
            Tokens = tokens
        }
