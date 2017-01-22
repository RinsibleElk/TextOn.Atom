namespace TextOn.Atom

open System.Text.RegularExpressions
open System.IO

/// Service to resolve a file. Can be used in testing to not actually bother having real files lying around.
type PreprocessorFileResolver = string -> string option -> (string * string option * string list) option

// Loop through lines
// If line contains a #include
// - If include is invalid (no file specified), output a preprocessor error, and continue.
// - If include is valid but file doesn't exist, output a preprocessor error, and continue.
// - If include is valid and file exists, but file has already been included, output a preprocessor warning, and continue.
// - Preprocess the inner and splice it in.
type PreprocessorError = string
type PreprocessorWarning = string
type PreprocessorSuccess = string

/// 
type PreprocessedLine =
    | Error of PreprocessorError
    | Warning of PreprocessorWarning
    | Line of PreprocessorSuccess

/// Representation of the source file, post the preprocessing phase.
type PreprocessedSourceLine = {
    TopLevelFileLineNumber : int
    CurrentFileLineNumber : int
    CurrentFile : string
    Contents : PreprocessedLine }

[<RequireQualifiedAccess>]
module Preprocessor =
    /// Regular expression to extract the file name from the #include directive.
    let private includeRegex = Regex("^#include\\s+\"(.+)\"\s*$")

    /// Stateful container for files that have already been included, earlier in the document (recursively).
    type private IncludedFilesContainer() =
        let mutable includedFiles = Set.empty
        member __.AlreadyIncluded (s:string) = includedFiles |> Set.contains s
        member __.Add s = includedFiles <- includedFiles |> Set.add s

    /// Perform the preprocessing.
    let rec private preprocessInner inTopLevelFile topLevelFileLineNumber currentFileLineNumber currentFile (fileResolver:PreprocessorFileResolver) (currentDirectory:string option) (includedFilesContainer:IncludedFilesContainer) (lines:string list) =
        match lines with
        | [] -> Seq.empty
        | line::remaining ->
            seq {
                if (not (line.StartsWith("#"))) then
                    yield {
                        TopLevelFileLineNumber = topLevelFileLineNumber
                        CurrentFileLineNumber = currentFileLineNumber
                        CurrentFile = currentFile
                        Contents = Line line }
                else
                    let includeMatch = includeRegex.Match(line)
                    if (not (includeMatch.Success)) then
                        yield {
                            TopLevelFileLineNumber = topLevelFileLineNumber
                            CurrentFileLineNumber = currentFileLineNumber
                            CurrentFile = currentFile
                            Contents = Error (line |> sprintf "Not a valid #include directive: %s") }
                    else
                        let includeFileUnresolved = includeMatch.Groups.[1].Value
                        let resolvedFile = fileResolver includeFileUnresolved currentDirectory
                        if (resolvedFile |> Option.isNone) then
                            yield {
                                TopLevelFileLineNumber = topLevelFileLineNumber
                                CurrentFileLineNumber = currentFileLineNumber
                                CurrentFile = currentFile
                                Contents = Error (includeFileUnresolved |> sprintf "Unable to resolve file: %s") }
                        else
                            let (includeFileResolved, includeDirectory, includeLines) = resolvedFile |> Option.get
                            if includedFilesContainer.AlreadyIncluded includeFileResolved then
                                yield {
                                    TopLevelFileLineNumber = topLevelFileLineNumber
                                    CurrentFileLineNumber = currentFileLineNumber
                                    CurrentFile = currentFile
                                    Contents = Warning (includeFileResolved |> sprintf "Already included: %s") }
                            else
                                includedFilesContainer.Add includeFileResolved
                                yield! preprocessInner false topLevelFileLineNumber 1 includeFileResolved fileResolver includeDirectory includedFilesContainer includeLines
                yield! (preprocessInner inTopLevelFile (if inTopLevelFile then (topLevelFileLineNumber + 1) else topLevelFileLineNumber) (currentFileLineNumber + 1) currentFile fileResolver currentDirectory includedFilesContainer remaining) }

    /// Get a "real" file resolver.
    let realFileResolver : PreprocessorFileResolver =
        (fun fileUnresolved directory ->
            let file =
                if directory |> Option.isSome then
                    FileInfo(Path.Combine(directory.Value, fileUnresolved))
                else
                    FileInfo(fileUnresolved)
            if file.Exists then Some (file.FullName, file.Directory.FullName |> Some, file.FullName |> File.ReadAllLines |> List.ofArray)
            else None)

    /// Perform the preprocess.
    let preprocess fileResolver fileName currentDirectory lines =
        preprocessInner true 1 1 fileName fileResolver currentDirectory (IncludedFilesContainer()) lines
        |> List.ofSeq
