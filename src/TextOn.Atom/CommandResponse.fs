namespace TextOn.Atom

open System
open TextOn.Atom.DTO.DTO

module TextOnErrorInfo =
    let OfCompilationError(error) =
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
    let generatorSetup (serialize : Serializer) (generatorData:GeneratorData) =
        serialize { Kind = "generatorSetup" ; Data = generatorData }
    let navigate (serialize : Serializer) (data:NavigateData) =
        serialize { Kind = "navigate" ; Data = data }
