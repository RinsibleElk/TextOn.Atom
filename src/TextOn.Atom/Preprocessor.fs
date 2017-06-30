namespace TextOn.Atom

open System
open System.IO

/// Service to resolve a file. Can be used in testing to not actually bother having real files lying around.
type PreprocessorFileResolver = string -> string -> (string * string * string list) option

// Loop through lines
// If line contains a #include
// - If include is invalid (no file specified), output a preprocessor error, and continue.
// - If include is valid but file doesn't exist, output a preprocessor error, and continue.
// - If include is valid and file exists, but file has already been included, ignore it and continue.
// - Preprocess the inner and splice it in.
type PreprocessorError = {
    StartLocation : int
    EndLocation : int
    ErrorText : string }

/// A preprocessed line, decorated with error/warning details.
type PreprocessedLine =
    | PreprocessorError of PreprocessorError
    | PreprocessorLine of string

/// Representation of the source file, post the preprocessing phase.
type PreprocessedSourceLine = {
    TopLevelFileLineNumber : int
    CurrentFileLineNumber : int
    CurrentFile : string
    Contents : PreprocessedLine }

[<RequireQualifiedAccess>]
module Preprocessor =
    /// Regular expression to extract the file name from the #include directive.
    let private includeLine = @"#include"
    let private doubleQuoteChar = '"'

    /// Stateful container for files that have already been included, earlier in the document (recursively).
    type private IncludedFilesContainer() =
        let mutable includedFiles = Set.empty
        member __.AlreadyIncluded (s:string) = includedFiles |> Set.contains s
        member __.Add s = includedFiles <- includedFiles |> Set.add s

    let private eatWhitespaceAtBeginning n (l:string) =
        let mutable i = n
        let e = l.Length - 1
        while i <= e && (Char.IsWhiteSpace(l.[i])) do
            i <- i + 1
        if i > e then None else Some i

    let private eatWhitespaceAtEnd (l:string) =
        let mutable i = l.Length - 1
        while i >= 0 && (Char.IsWhiteSpace(l.[i])) do
            i <- i - 1
        if i < 0 then None else Some i

    /// Perform the preprocessing.
    let rec private preprocessInner inTopLevelFile topLevelFileLineNumber currentFileLineNumber currentFile (fileResolver:PreprocessorFileResolver) (currentDirectory:string) (includedFilesContainer:IncludedFilesContainer) (lines:string list) =
        match lines with
        | [] -> Seq.empty
        | line::remaining ->
            seq {
                if (not (line.StartsWith("#"))) then
                    yield {
                        TopLevelFileLineNumber = topLevelFileLineNumber
                        CurrentFileLineNumber = currentFileLineNumber
                        CurrentFile = Path.GetFullPath(Path.Combine(currentDirectory, currentFile))
                        Contents = PreprocessorLine line }
                else if (not (line.StartsWith(includeLine))) || line.Length <= 8 || (not (Char.IsWhiteSpace (line.[8]))) then
                    yield {
                        TopLevelFileLineNumber = topLevelFileLineNumber
                        CurrentFileLineNumber = currentFileLineNumber
                        CurrentFile = Path.GetFullPath(Path.Combine(currentDirectory, currentFile))
                        Contents = PreprocessorError {
                            StartLocation = 1
                            EndLocation = line.Length
                            ErrorText = (line |> sprintf "Not a valid #include directive: %s") } }
                else
                    let offsetToFile = eatWhitespaceAtBeginning 8 line
                    let endOfFile = eatWhitespaceAtEnd line
                    match (offsetToFile, endOfFile) with
                    | (None, _)
                    | (_, None) ->
                        yield {
                            TopLevelFileLineNumber = topLevelFileLineNumber
                            CurrentFileLineNumber = currentFileLineNumber
                            CurrentFile = Path.GetFullPath(Path.Combine(currentDirectory, currentFile))
                            Contents = PreprocessorError {
                                StartLocation = 1
                                EndLocation = line.Length
                                ErrorText = (line |> sprintf "Not a valid #include directive: %s") } }
                    | (Some s, Some e) ->
                        if s >= e || line.[s] <> doubleQuoteChar || line.[e] <> doubleQuoteChar then
                            yield {
                                TopLevelFileLineNumber = topLevelFileLineNumber
                                CurrentFileLineNumber = currentFileLineNumber
                                CurrentFile = Path.GetFullPath(Path.Combine(currentDirectory, currentFile))
                                Contents = PreprocessorError {
                                    StartLocation = 1
                                    EndLocation = line.Length
                                    ErrorText = (line |> sprintf "Not a valid #include directive: %s") } }
                        else
                            let includeFileUnresolved = line.Substring(s + 1, e - 1 - s)
                            let resolvedFile = fileResolver includeFileUnresolved currentDirectory
                            if (resolvedFile |> Option.isNone) then
                                yield {
                                    TopLevelFileLineNumber = topLevelFileLineNumber
                                    CurrentFileLineNumber = currentFileLineNumber
                                    CurrentFile = Path.GetFullPath(Path.Combine(currentDirectory, currentFile))
                                    Contents = PreprocessorError {
                                        StartLocation = 1 + line.IndexOf(doubleQuoteChar)
                                        EndLocation = line.Length
                                        ErrorText = (includeFileUnresolved |> sprintf "Unable to resolve file: %s") } }
                            else
                                let (includeFileResolved, includeDirectory, includeLines) = resolvedFile |> Option.get
                                if (not (includedFilesContainer.AlreadyIncluded includeFileResolved)) then
                                    includedFilesContainer.Add includeFileResolved
                                    yield! preprocessInner false topLevelFileLineNumber 1 includeFileResolved fileResolver includeDirectory includedFilesContainer includeLines
                yield! (preprocessInner inTopLevelFile (if inTopLevelFile then (topLevelFileLineNumber + 1) else topLevelFileLineNumber) (currentFileLineNumber + 1) currentFile fileResolver currentDirectory includedFilesContainer remaining) }

    /// Get a "real" file resolver.
    let realFileResolver : PreprocessorFileResolver =
        (fun fileUnresolved directory ->
            let file = FileInfo(Path.Combine(directory, fileUnresolved))
            if file.Exists then Some (file.Name, file.Directory.FullName, file.FullName |> File.ReadAllLines |> List.ofArray)
            else None)

    /// Perform the preprocess.
    let preprocess fileResolver fileName currentDirectory (lines:string list) =
        preprocessInner true 1 1 fileName fileResolver currentDirectory (IncludedFilesContainer()) lines
        |> Seq.toList
