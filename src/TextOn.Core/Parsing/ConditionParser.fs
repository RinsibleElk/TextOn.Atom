namespace TextOn.Core.Parsing

open System
open TextOn.Core.Tokenizing

type ParsedAttributeOrVariable =
    | ParsedAttributeName of string
    | ParsedVariableName of string

type ParsedAttributeOrVariableOrFunction =
    | ParsedAttributeRef of string
    | ParsedVariableRef of string
    | ParsedFunctionRef of string

type ParseErrorSeverity =
    | Warning
    | Error

type ParseError = {
    File : string
    Severity : ParseErrorSeverity
    StartLine : int
    EndLine : int
    StartLocation : int
    EndLocation : int
    ErrorText : string }

type ParsedCondition =
    | ParsedUnconditional
    | ParsedOr of ParsedCondition * ParsedCondition
    | ParsedAnd of ParsedCondition * ParsedCondition
    | ParsedAreEqual of int * int * int * ParsedAttributeOrVariable * string
    | ParsedAreNotEqual of int * int * int * ParsedAttributeOrVariable * string
    | ParsedConditionError of ParseError[]

type ConditionParseResults = {
    HasErrors : bool
    Dependencies : ParsedAttributeOrVariable[]
    Condition : ParsedCondition }

[<RequireQualifiedAccess>]
module ConditionParser =
    let private makeParseError file line s e t =
        {   File = file
            Severity = Error
            StartLine = line
            EndLine = line
            StartLocation = s
            EndLocation = e
            ErrorText = t }

    let rec private parseConditionInner file line position variablesAreAllowed conditionTokens =
        if conditionTokens |> List.isEmpty then
            { HasErrors = true; Dependencies = [||]; Condition = ParsedConditionError [|(makeParseError file line position position "Invalid empty condition")|] }
        else
            // Find the first Or at root (0 brackets) level.
            let li =
                conditionTokens
                |> List.scan
                    (fun (bracketCount,index,_) attToken ->
                        if attToken.Token = OpenBracket then (bracketCount + 1, index + 1, Some attToken)
                        else if attToken.Token = CloseBracket then (bracketCount - 1, index + 1, Some attToken)
                        else (bracketCount, index + 1, Some attToken))
                    (0, -1, None)
                |> List.skip 1
            let rootOr =
                li
                |> List.tryFind (fun (a, _, ao) -> a = 0 && ao.Value.Token = Or)
            if rootOr |> Option.isSome then
                let (_, index, _) = rootOr.Value
                let left = parseConditionInner file line position variablesAreAllowed (conditionTokens |> List.take index)
                let right = parseConditionInner file line (position + index + 1) variablesAreAllowed (conditionTokens |> List.skip (index + 1))
                let hasErrors = left.HasErrors || right.HasErrors
                let condition =
                    if hasErrors then
                        ParsedConditionError(
                            Array.append
                                (match left.Condition with | ParsedConditionError(errors) -> errors | _ -> [||])
                                (match right.Condition with | ParsedConditionError(errors) -> errors | _ -> [||]))
                    else
                        ParsedOr(left.Condition, right.Condition)
                {   HasErrors = hasErrors
                    Dependencies = (Set.union (left.Dependencies |> Set.ofArray) (right.Dependencies |> Set.ofArray) |> Set.toArray)
                    Condition = condition }
            else
                let rootAnd =
                    li
                    |> List.tryFind (fun (a, _, ao) -> a = 0 && ao.Value.Token = And)
                if rootAnd |> Option.isSome then
                    let (_, index, _) = rootAnd.Value
                    let left = parseConditionInner file line position variablesAreAllowed (conditionTokens |> List.take index)
                    let right = parseConditionInner file line (position + index + 1) variablesAreAllowed (conditionTokens |> List.skip (index + 1))
                    let hasErrors = left.HasErrors || right.HasErrors
                    let condition =
                        if hasErrors then
                            ParsedConditionError(
                                Array.append
                                    (match left.Condition with | ParsedConditionError(errors) -> errors | _ -> [||])
                                    (match right.Condition with | ParsedConditionError(errors) -> errors | _ -> [||]))
                        else
                            ParsedAnd(left.Condition, right.Condition)
                    {   HasErrors = hasErrors
                        Dependencies = (Set.union (left.Dependencies |> Set.ofArray) (right.Dependencies |> Set.ofArray) |> Set.toArray)
                        Condition = condition }
                else
                    if conditionTokens.Length = 3 then
                        match (conditionTokens.[0].Token, conditionTokens.[1].Token, conditionTokens.[2].Token) with
                        | (AttributeName name, Equals, QuotedString value) ->
                            { HasErrors = false; Dependencies = [|ParsedAttributeName name|] ; Condition = ParsedAreEqual(line, conditionTokens.[0].TokenStartLocation, conditionTokens.[0].TokenEndLocation, ParsedAttributeName name, value) }
                        | (AttributeName name, NotEquals, QuotedString value) ->
                            { HasErrors = false; Dependencies = [|ParsedAttributeName name|] ; Condition = ParsedAreNotEqual(line, conditionTokens.[0].TokenStartLocation, conditionTokens.[0].TokenEndLocation, ParsedAttributeName name, value) }
                        | (VariableName name, Equals, QuotedString value) ->
                            if variablesAreAllowed then
                                { HasErrors = false; Dependencies = [|ParsedVariableName name|] ; Condition = ParsedAreEqual(line, conditionTokens.[0].TokenStartLocation, conditionTokens.[0].TokenEndLocation, ParsedVariableName name, value) }
                            else
                                { HasErrors = true; Dependencies = [||]; Condition = ParsedConditionError ([|(makeParseError file line conditionTokens.[0].TokenStartLocation conditionTokens.[0].TokenEndLocation "Invalid reference to variable in attribute-based condition")|]) }
                        | (VariableName name, NotEquals, QuotedString value) ->
                            if variablesAreAllowed then
                                { HasErrors = false; Dependencies = [|ParsedVariableName name|] ; Condition = ParsedAreNotEqual(line, conditionTokens.[0].TokenStartLocation, conditionTokens.[0].TokenEndLocation, ParsedVariableName name, value) }
                            else
                                { HasErrors = true; Dependencies = [||] ; Condition = ParsedConditionError ([|(makeParseError file line conditionTokens.[0].TokenStartLocation conditionTokens.[0].TokenEndLocation "Invalid reference to variable in attribute-based condition")|]) }
                        | _ ->
                            { HasErrors = true; Dependencies = [||] ; Condition = ParsedConditionError ([|(makeParseError file line conditionTokens.[0].TokenStartLocation conditionTokens.[2].TokenEndLocation "Invalid condition")|]) }
                    else if (conditionTokens.[0].Token <> OpenBracket) || (conditionTokens.[conditionTokens.Length - 1].Token <> CloseBracket) then
                        { HasErrors = true; Dependencies = [||] ; Condition = ParsedConditionError ([|(makeParseError file line conditionTokens.[0].TokenStartLocation conditionTokens.[conditionTokens.Length - 1].TokenEndLocation "Invalid condition")|]) }
                    else
                        parseConditionInner file line conditionTokens.[0].TokenStartLocation variablesAreAllowed (conditionTokens |> List.skip 1 |> List.take (conditionTokens.Length - 2))

    /// Parse a condition.
    let internal parseCondition file line variablesAreAllowed (conditionTokens:AttributedToken list) =
        // It is known that the first is an OpenBrace. If the last is not a CloseBrace, it's an error.
        if conditionTokens.[conditionTokens.Length - 1].Token <> CloseBrace then
            { HasErrors = true; Dependencies = [||] ; Condition = ParsedConditionError ([|(makeParseError file line conditionTokens.[0].TokenStartLocation conditionTokens.[conditionTokens.Length - 1].TokenEndLocation "Invalid condition")|]) }
        else parseConditionInner file line conditionTokens.[0].TokenStartLocation variablesAreAllowed (conditionTokens |> List.skip 1 |> List.take (conditionTokens.Length - 2))
