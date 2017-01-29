namespace TextOn.Atom

open System
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
/// Tokenize pre-categorized data.
module Tokenizer =
    /// Do the context-specific tokenization.
    let tokenize group =
        match group.Category with
        | CategorizedFuncDefinition -> Some (group.Lines |> Seq.map (fun line -> match line.Contents with | PreprocessorLine line -> DefinitionLineTokenizer.tokenizeLine line | _ -> failwith ""))
        | CategorizedVarDefinition -> Some (group.Lines |> Seq.map (fun line -> match line.Contents with | PreprocessorLine line -> VariableLineTokenizer.tokenizeLine line | _ -> failwith ""))
        | CategorizedAttDefinition -> Some (group.Lines |> Seq.map (fun line -> match line.Contents with | PreprocessorLine line -> AttributeLineTokenizer.tokenizeLine line | _ -> failwith ""))
        | CategorizedPreprocessorError -> None
        | CategorizedPreprocessorWarning -> None
        | CategorizationError -> None
