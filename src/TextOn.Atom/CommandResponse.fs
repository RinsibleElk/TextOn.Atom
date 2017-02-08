namespace TextOn.Atom

open System

type TextOnErrorSeverity =
    | Warning
    | Error
type TextOnErrorInfo =
    {
        FileName: string
        StartLine:int
        EndLine:int
        StartColumn:int
        EndColumn:int
        Severity:TextOnErrorSeverity
        Message:string
        Subcategory:string
    }
    with
        static member OfCompilationError(error) =
            match error with
            | ParserError(e) ->
                {
                    FileName = e.File
                    StartLine = e.LineNumber
                    EndLine = e.LineNumber
                    StartColumn = e.StartLocation
                    EndColumn = e.EndLocation
                    Severity = TextOnErrorSeverity.Error
                    Message = e.ErrorText
                    Subcategory = "Parser"
                }
            | GeneralError(e) ->
                {
                    FileName = e.File
                    StartLine = 1
                    EndLine = 1
                    StartColumn = 1
                    EndColumn = 1
                    Severity = TextOnErrorSeverity.Error
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

[<RequireQualifiedAccess>]
module CommandResponse =
    let errors (serialize : Serializer) (errors:CompilationError[], file: string) =
        serialize { Kind = "errors"
                    Data = { File = file
                             Errors = Array.map TextOnErrorInfo.OfCompilationError errors } }
    let info (serialize : Serializer) (s: string) = serialize { Kind = "info"; Data = s }
    let error (serialize : Serializer) (s: string) = serialize { Kind = "error"; Data = s }
