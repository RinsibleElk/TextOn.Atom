namespace TextOn.Atom

open System
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
/// Tokenize pre-categorized data.
module Tokenizer =
    /// Do the context-specific tokenization.
    let tokenize group =
        match group.Category with
        | FuncDefinition -> Some (group.Lines |> Seq.map (fun line -> match line.Contents with | Line line -> DefinitionLineTokenizer.tokenizeLine line | _ -> failwith ""))
        | VarDefinition -> Some (group.Lines |> Seq.map (fun line -> match line.Contents with | Line line -> VariableLineTokenizer.tokenizeLine line | _ -> failwith ""))
        | AttDefinition -> Some (group.Lines |> Seq.map (fun line -> match line.Contents with | Line line -> AttributeLineTokenizer.tokenizeLine line | _ -> failwith ""))
        | PreprocessorError -> None
        | PreprocessorWarning -> None
        | CategorizationError -> None
