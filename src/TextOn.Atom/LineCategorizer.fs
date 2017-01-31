namespace TextOn.Atom

open System
open System.Text.RegularExpressions

/// The output for the LineCategorizer.
type Category =
    | CategorizedFuncDefinition
    | CategorizedVarDefinition
    | CategorizedAttDefinition
    | CategorizedPreprocessorError
    | CategorizedPreprocessorWarning
    | CategorizationError

/// Meta data for a section of code.
type CategorizedLines = {
    Category : Category
    Index : int
    File : string
    StartLine : int
    EndLine : int
    Lines : PreprocessedSourceLine list }

[<RequireQualifiedAccess>]
/// Maps groups of preprocessed lines into categories.
module LineCategorizer =
    let private funcDefinitionRegex = Regex("^@func\s+")
    let private varDefinitionRegex = Regex("^@var\s+")
    let private attDefinitionRegex = Regex("^@att\s+")
    type private State =
        | Root of int
        | Context of Category * int * string * int * int * PreprocessedSourceLine list
    let rec private categorizeInner state lines : CategorizedLines seq =
        match lines with
        | [] ->
            match state with
            | Root(_) -> Seq.empty
            | Context(category, nextIndex, file, startLine, endLine, preprocessedSourceLines) ->
                Seq.singleton
                    {   Category = category
                        Index = nextIndex
                        File = file
                        StartLine = startLine
                        EndLine = endLine
                        Lines = preprocessedSourceLines }
        | line::remaining ->
            let (nextState, output) =
                // If the line is a preprocessor error or warning then the current state is immediately terminated.
                match line.Contents with
                | PreprocessorWarning(_)
                | PreprocessorError(_) ->
                    match state with
                    | Root(nextIndex) ->
                        (Root(nextIndex + 1),
                            Some
                                (Seq.singleton
                                    {   Category = match line.Contents with | PreprocessorWarning(_) -> CategorizedPreprocessorWarning | _ -> CategorizedPreprocessorError
                                        Index = nextIndex
                                        File = line.CurrentFile
                                        StartLine = line.CurrentFileLineNumber
                                        EndLine = line.CurrentFileLineNumber
                                        Lines = List.singleton line }))
                    | Context(category, nextIndex, file, startLine, endLine, preprocessedSourceLines) ->
                        (Root(nextIndex + 2),
                            Some
                                (seq [
                                    {   Category = category
                                        Index = nextIndex
                                        File = file
                                        StartLine = startLine
                                        EndLine = endLine
                                        Lines = preprocessedSourceLines }
                                    {   Category = match line.Contents with | PreprocessorWarning(_) -> CategorizedPreprocessorWarning | _ -> CategorizedPreprocessorError
                                        Index = nextIndex + 1
                                        File = line.CurrentFile
                                        StartLine = line.CurrentFileLineNumber
                                        EndLine = line.CurrentFileLineNumber
                                        Lines = List.singleton line } ]))
                | PreprocessorLine(text) ->
                    // Whatever state we're in, we're looking for one of the root definitions above.
                    let funcMatch = funcDefinitionRegex.Match(text)
                    if (funcMatch.Success) then
                        match state with
                        | Root(nextIndex) ->
                            (Context(CategorizedFuncDefinition, nextIndex, line.CurrentFile, line.CurrentFileLineNumber, line.CurrentFileLineNumber, List.singleton line), None)
                        | Context(category, nextIndex, file, startLine, endLine, preprocessedSourceLines) ->
                            (Context(CategorizedFuncDefinition, nextIndex + 1, line.CurrentFile, line.CurrentFileLineNumber, line.CurrentFileLineNumber, List.singleton line),
                                Some
                                    (Seq.singleton
                                        {   Category = category
                                            Index = nextIndex
                                            File = file
                                            StartLine = startLine
                                            EndLine = endLine
                                            Lines = preprocessedSourceLines }))
                    else
                        let varMatch = varDefinitionRegex.Match(text)
                        if (varMatch.Success) then
                            match state with
                            | Root(nextIndex) ->
                                (Context(CategorizedVarDefinition, nextIndex, line.CurrentFile, line.CurrentFileLineNumber, line.CurrentFileLineNumber, List.singleton line), None)
                            | Context(category, nextIndex, file, startLine, endLine, preprocessedSourceLines) ->
                                (Context(CategorizedVarDefinition, nextIndex + 1, line.CurrentFile, line.CurrentFileLineNumber, line.CurrentFileLineNumber, List.singleton line),
                                    Some
                                        (Seq.singleton
                                            {   Category = category
                                                Index = nextIndex
                                                File = file
                                                StartLine = startLine
                                                EndLine = endLine
                                                Lines = preprocessedSourceLines }))
                        else
                            let attMatch = attDefinitionRegex.Match(text)
                            if (attMatch.Success) then
                                match state with
                                | Root(nextIndex) ->
                                    (Context(CategorizedAttDefinition, nextIndex, line.CurrentFile, line.CurrentFileLineNumber, line.CurrentFileLineNumber, List.singleton line), None)
                                | Context(category, nextIndex, file, startLine, endLine, preprocessedSourceLines) ->
                                    (Context(CategorizedAttDefinition, nextIndex + 1, line.CurrentFile, line.CurrentFileLineNumber, line.CurrentFileLineNumber, List.singleton line),
                                        Some
                                            (Seq.singleton
                                                {   Category = category
                                                    Index = nextIndex
                                                    File = file
                                                    StartLine = startLine
                                                    EndLine = endLine
                                                    Lines = preprocessedSourceLines }))
                            else
                                // Root : start CategorizationError region
                                // Other : if file has changed, then output the previous and start a new CategorizationError region, else continue the current region
                                match state with
                                | Root(nextIndex) ->
                                    (Context(CategorizationError, nextIndex, line.CurrentFile, line.CurrentFileLineNumber, line.CurrentFileLineNumber, List.singleton line), None)
                                | Context(category, nextIndex, file, startLine, endLine, preprocessedSourceLines) ->
                                    if file = line.CurrentFile then
                                        (Context(category, nextIndex, file, startLine, line.CurrentFileLineNumber, List.append preprocessedSourceLines [line]), None)
                                    else
                                        (Context(CategorizationError, nextIndex + 1, line.CurrentFile, line.CurrentFileLineNumber, line.CurrentFileLineNumber, List.singleton line),
                                            Some
                                                (Seq.singleton
                                                    {   Category = category
                                                        Index = nextIndex
                                                        File = file
                                                        StartLine = startLine
                                                        EndLine = endLine
                                                        Lines = preprocessedSourceLines }))
            seq {
                if output |> Option.isSome then yield! output.Value
                yield! (categorizeInner nextState remaining) }

    /// Categorize output from the comment stripper into sections for (potentially parallelized) context-specific tokenization and compilation.
    let categorize lines = categorizeInner (Root(0)) lines |> Seq.toList
