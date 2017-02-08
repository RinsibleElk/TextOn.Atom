namespace TextOn.Atom

open System

type TextOnErrorInfo = {
        /// 1-indexed first line of the error block
        StartLine : int
        /// 1-indexed first column of the error block
        StartColumn : int
        /// 1-indexed last line of the error block
        EndLine : int
        /// 1-indexed last column of the error block
        EndColumn : int
        /// Description of the error
        Message : string
        /// The severity - "Error" or "Warning".
        Severity : string
        /// Type of the Error
        Subcategory : string
    }
    with
        static member OfCompilationError(error) =
            match error with
            | ParserError(e) ->
                {
                    StartLine = e.LineNumber
                    EndLine = e.LineNumber
                    StartColumn = e.StartLocation
                    EndColumn = e.EndLocation
                    Severity = "Error"
                    Message = e.ErrorText
                    Subcategory = "Parser"
                }
            | GeneralError(e) ->
                {
                    StartLine = 1
                    EndLine = 1
                    StartColumn = 1
                    EndColumn = 1
                    Severity = "Error"
                    Message = e.ErrorText
                    Subcategory = "General"
                }

type ErrorResponse =
    {
        File: string
        Errors: TextOnErrorInfo []
    }

type ResponseMsg<'T> =
    {
        Kind: string
        Data: 'T
    }

type LintWarning =
    {
        /// Warning to display to the user.
        Info: string
        /// 1-indexed first line of the lint block
        StartLine : int
        /// 1-indexed first column of the lint block
        StartColumn : int
        /// 1-indexed last line of the lint block
        EndLine : int
        /// 1-indexed last column of the lint block
        EndColumn : int
        /// Entire input file, needed to display where in the file the error occurred.
        Input: string
    }

[<RequireQualifiedAccess>]
module CommandResponse =
    let errors (serialize : Serializer) (errors:CompilationError[], file: string) =
        serialize { Kind = "errors"
                    Data = errors
                           |> Array.filter (function | ParserError(e) -> e.File = file | _ -> true)
                           |> Array.map TextOnErrorInfo.OfCompilationError }
    let info (serialize : Serializer) (s: string) = serialize { Kind = "info"; Data = s }
    let error (serialize : Serializer) (s: string) = serialize { Kind = "error"; Data = s }
    let lint (serialize : Serializer) (warnings : LintWarning list) =
        let data = warnings |> List.toArray
        serialize { Kind = "lint"; Data = data }
