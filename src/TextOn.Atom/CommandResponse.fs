namespace TextOn.Atom

open System
open TextOn.Atom.DTO.DTO
open TextOn.Core.Parsing

module TextOnErrorInfo =
    let OfCompilationError (error:ParseError) =
        {
            range =
                [|
                    [|(float (error.LineNumber - 1));(float (error.StartLocation - 1))|]
                    [|(float (error.LineNumber - 1));(float (error.EndLocation))|]
                |]
            ``type`` = match error.Severity with | Error -> "Error" | Warning -> "Warning"
            text = error.ErrorText
            filePath = error.File
        }

[<RequireQualifiedAccess>]
module CommandResponse =
    let errors (serialize : Serializer) (errors:ParseError[], file: string) =
        serialize { Kind = "errors"
                    Data = errors |> Array.map TextOnErrorInfo.OfCompilationError }
    let suggestions (serialize : Serializer) (s: Suggestion[]) = serialize { Kind = "suggestion"; Data = s }
    let error (serialize : Serializer) (s: string) = serialize { Kind = "error"; Data = [|s|] }
    let generatorSetup (serialize : Serializer) (generatorData:GeneratorData) =
        serialize { Kind = "generatorSetup" ; Data = [|generatorData|] }
    let browserUpdate (serialize : Serializer) (update:BrowserUpdate) =
        serialize { Kind = "browserUpdate" ; Data = [|update|] }
    let browserItems (serialize : Serializer) (items:BrowserItems) =
        serialize { Kind = "browseritems" ; Data = [|items|] }
    let navigate (serialize : Serializer) (data:NavigateData) =
        serialize { Kind = "navigate" ; Data = [|data|] }
    let thanks (serialize : Serializer) =
        serialize { Kind = "thanks" ; Data = [||] }
