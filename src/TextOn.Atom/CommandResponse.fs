namespace TextOn.Atom

open System
open TextOn.Atom.DTO.DTO

module TextOnErrorInfo =
    let OfCompilationError(error) =
        match error with
        | ParserError(e) ->
            {
                range =
                    [|
                        [|(float (e.LineNumber - 1));(float (e.StartLocation - 1))|]
                        [|(float (e.LineNumber - 1));(float (e.EndLocation))|]
                    |]
                ``type`` = "Error"
                text = e.ErrorText
                filePath = e.File
            }
        | GeneralError(e) ->
            {
                range =
                    [|
                        [|0.0;0.0|]
                        [|0.0;1.0|]
                    |]
                filePath = e.File
                text = e.ErrorText
                ``type`` = "Error"
            }

[<RequireQualifiedAccess>]
module CommandResponse =
    let errors (serialize : Serializer) (errors:CompilationError[], file: string) =
        serialize { Kind = "errors"
                    Data = errors
                           |> Array.filter (function | ParserError(e) -> e.File = file | _ -> true)
                           |> Array.map TextOnErrorInfo.OfCompilationError }
    let suggestions (serialize : Serializer) (s: Suggestion[]) = serialize { Kind = "suggestion"; Data = s }
    let error (serialize : Serializer) (s: string) = serialize { Kind = "error"; Data = [|s|] }
    let generatorSetup (serialize : Serializer) (generatorData:GeneratorData) =
        serialize { Kind = "generatorSetup" ; Data = [|generatorData|] }
    let navigate (serialize : Serializer) (data:NavigateData) =
        serialize { Kind = "navigate" ; Data = [|data|] }
