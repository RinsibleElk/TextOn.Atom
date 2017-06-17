namespace TextOn.Atom

open System

[<RequireQualifiedAccess>]
module Browser =
    let rec makeSimpleText variableNames variableValues (node:SimpleCompiledDefinitionNode) =
        match node with
        | VariableValue(i) ->
            variableValues
            |> Map.tryFind i
            |> defaultArg <| (variableNames |> Map.find i |> fun x -> "$" + x)
        | SimpleChoice(nodes) ->
            "{" + (nodes |> Array.fold (fun x a -> (if x = "" then "" else (x + "|")) + (makeSimpleText variableNames variableValues a)) "") + "}"
        | SimpleSeq(nodes) ->
            (nodes |> Array.fold (fun x a -> x + (makeSimpleText variableNames variableValues a)) "")
        | SimpleText(s) -> s
    let rec private makeTextInner isRoot functionNames attributeValues variableNames variableValues (node:CompiledDefinitionNode) =
        match node with
        | Sentence(_, _, simpleNode) -> makeSimpleText variableNames variableValues simpleNode
        | ParagraphBreak(_) -> "<paragraph break>"
        | Choice(_, _, nodes) ->
            nodes
            |> Array.filter (fun (_, condition) -> ConditionEvaluator.resolvePartial attributeValues condition)
            |> Array.tryFind (fun _ -> true)
            |> Option.map (fst >> makeTextInner false functionNames attributeValues variableNames variableValues)
            |> defaultArg <| "<no output>"
            |> fun text ->
                if isRoot then
                    "[Choice] " + (if text.Length > 50 then text.Substring(0, 50) + "..." else text)
                else text
        | Seq(_, _, nodes) ->
            nodes
            |> Array.filter (fun (_, condition) -> ConditionEvaluator.resolvePartial attributeValues condition)
            |> Array.tryFind (fun _ -> true)
            |> Option.map (fst >> makeTextInner false functionNames attributeValues variableNames variableValues)
            |> defaultArg <| "<no output>"
            |> fun text ->
                if isRoot then
                    "[Seq] " + (if text.Length > 50 then text.Substring(0, 50) + "..." else text)
                else text
        | Function(_, _, f) ->
            functionNames
            |> Map.find f
            |> fun x -> "@" + x
    let makeText functionNames attributeValues variableNames variableValues (node:CompiledDefinitionNode) =
        makeTextInner true functionNames attributeValues variableNames variableValues node
