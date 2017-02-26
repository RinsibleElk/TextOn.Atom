﻿namespace TextOn.Atom

open TextOn.Atom.DTO.DTO

type GeneratorStartResult =
    | CompilationFailure of CompilationError[]
    | GeneratorStarted of GeneratorData

[<Sealed>]
type GeneratorServer(file, name) =
    let mutable currentTemplate = None
    let mutable lastResult = None
    let mutable attributeValues = Map.empty
    let mutable variableValues = Map.empty
    member __.File = file |> System.IO.FileInfo
    member __.UpdateTemplate (template:CompiledTemplate) = currentTemplate <- Some template
    member __.Data =
        let (haveSeenEmpty, attributes, variables) =
            Array.append
                (currentTemplate.Value.Attributes |> Array.map Choice1Of2)
                (currentTemplate.Value.Variables |> Array.map Choice2Of2)
            |> Array.scan
                (fun (haveSeenFilled, haveSeenEmpty, _) value ->
                    match value with
                    | Choice1Of2 att ->
                        let data =
                            if haveSeenEmpty then
                                attributeValues |> Map.remove att.Index |> ignore
                                {
                                    GeneratorAttribute.Name = att.Name
                                    Value = ""
                                    Suggestions = [||]
                                    IsEditable = false
                                }
                            else
                                let suggestions = att.Values |> Array.filter (fun a -> ConditionEvaluator.resolve attributeValues a.Condition) |> Array.map (fun a -> a.Value)
                                let value = attributeValues |> Map.tryFind att.Index
                                let value =
                                    if value.IsSome && suggestions |> Array.tryFind (fun x -> x = value.Value) |> Option.isNone then
                                        attributeValues <- attributeValues |> Map.remove att.Index
                                        ""
                                    else
                                        value |> defaultArg <| ""
                                {
                                    Name = att.Name
                                    Value = value
                                    Suggestions = suggestions
                                    IsEditable = true
                                }
                        (haveSeenFilled || data.Value <> "", haveSeenEmpty || data.Value = "", (Some (Choice1Of2 data)))
                    | Choice2Of2 var ->
                        let data =
                            if haveSeenEmpty then
                                variableValues |> Map.remove var.Index |> ignore
                                {
                                    GeneratorVariable.Name = var.Name
                                    Text = var.Text
                                    Value = ""
                                    Suggestions = [||]
                                    IsEditable = false
                                    IsFree = var.PermitsFreeValue
                                }
                            else
                                let suggestions = var.Values |> Array.filter (fun a -> VariableConditionEvaluator.resolve attributeValues variableValues a.Condition) |> Array.map (fun a -> a.Value)
                                let value = variableValues |> Map.tryFind var.Index
                                let value =
                                    if (not (var.PermitsFreeValue)) && value.IsSome && suggestions |> Array.tryFind (fun x -> x = value.Value) |> Option.isNone then
                                        variableValues <- variableValues |> Map.remove var.Index
                                        ""
                                    else
                                        value |> defaultArg <| ""
                                {
                                    Name = var.Name
                                    Text = var.Text
                                    Value = value
                                    Suggestions = suggestions
                                    IsEditable = true
                                    IsFree = var.PermitsFreeValue
                                }
                        (haveSeenFilled || data.Value <> "", haveSeenEmpty || data.Value = "", (Some (Choice2Of2 data))))
                    (false, false, None)
            |> Array.skip 1
            |> Array.map (fun (_,e,o) -> (e,o.Value))
            |> fun a ->
                let attributes = a |> Array.map snd |> Array.choose (function | Choice1Of2 a -> Some a | _ -> None)
                let variables = a |> Array.map snd |> Array.choose (function | Choice2Of2 a -> Some a | _ -> None)
                let haveSeenEmpty = a |> Array.map fst |> List.ofArray |> List.tryLast |> defaultArg <| false
                (haveSeenEmpty, attributes, variables)
        {
            FileName = file
            FunctionName = name
            Attributes = attributes
            Variables = variables
            CanGenerate = (not haveSeenEmpty)
            Output = if haveSeenEmpty || lastResult.IsNone then [||] else lastResult.Value
        }
    member __.SetValue ty name value =
        if ty = "Variable" then
            let index = currentTemplate.Value.Variables |> Array.tryFind (fun v -> v.Name = name) |> Option.map (fun v -> v.Index)
            if index.IsSome then variableValues <- variableValues |> Map.add index.Value value
        else
            let index = currentTemplate.Value.Attributes |> Array.tryFind (fun v -> v.Name = name) |> Option.map (fun v -> v.Index)
            if index.IsSome then attributeValues <- attributeValues |> Map.add index.Value value
    member __.Generate (config:GeneratorConfiguration) =
        let generatorInput =
            {   RandomSeed = NoSeed
                Config = {  NumSpacesBetweenSentences = config.NumSpacesBetweenSentences
                            NumBlankLinesBetweenParagraphs = config.NumBlankLinesBetweenParagraphs
                            LineEnding = (if config.WindowsLineEndings then CRLF else LF) }
                Attributes = currentTemplate.Value.Attributes |> List.ofArray |> List.map (fun a -> { Name = a.Name ; Value = attributeValues |> Map.find a.Index })
                Variables = currentTemplate.Value.Variables |> List.ofArray |> List.map (fun a -> { Name = a.Name ; Value = variableValues |> Map.find a.Index })
                Function = Some name }
        match (Generator.generate generatorInput currentTemplate.Value) with
        | GeneratorError _ -> lastResult <- None
        | GeneratorSuccess r -> lastResult <- Some (r.Text |> Array.map (fun a -> {File = a.InputFile ; LineNumber = a.InputLineNumber ; Value = a.Value ; IsPb = a.IsPb }))
