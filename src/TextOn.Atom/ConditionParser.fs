namespace TextOn.Atom

open System
open System.Text.RegularExpressions

type ParsedAttributeOrVariable =
    | ParsedAttribute of string
    | ParsedVariable of string

type ParsedCondition =
    | ParsedUnconditional
    | ParsedOr of ParsedCondition * ParsedCondition
    | ParsedAnd of ParsedCondition * ParsedCondition
    | ParsedAreEqual of ParsedAttributeOrVariable * string
    | ParsedAreNotEqual of ParsedAttributeOrVariable * string
    | ParsedConditionError of string

type ConditionParseResults = {
    HasErrors : bool
    Condition : ParsedCondition }

[<RequireQualifiedAccess>]
module ConditionParser =
    let rec private parseConditionInner variablesAreAllowed conditionTokens =
        if conditionTokens |> List.isEmpty then
            { HasErrors = true; Condition = ParsedConditionError "Invalid empty condition" }
        else
            // Find the first And at root (0 brackets) level.
            let li =
                conditionTokens
                |> List.scan
                    (fun (bracketCount,index,_) attToken ->
                        if attToken.Token = OpenBracket then (bracketCount + 1, index + 1, Some attToken)
                        else if attToken.Token = CloseBracket then (bracketCount - 1, index + 1, Some attToken)
                        else (bracketCount, index + 1, Some attToken))
                    (0, -1, None)
                |> List.skip 1
            let rootAnd =
                li
                |> List.tryFind (fun (a, _, ao) -> a = 0 && ao.Value.Token = And)
            if rootAnd |> Option.isSome then
                let (_, index, _) = rootAnd.Value
                let left = parseConditionInner variablesAreAllowed (conditionTokens |> List.take index)
                let right = parseConditionInner variablesAreAllowed (conditionTokens |> List.skip (index + 1))
                { HasErrors = left.HasErrors || right.HasErrors; Condition = ParsedAnd(left.Condition, right.Condition) }
            else
                let rootOr =
                    li
                    |> List.tryFind (fun (a, _, ao) -> a = 0 && ao.Value.Token = Or)
                if rootOr |> Option.isSome then
                    let (_, index, _) = rootOr.Value
                    let left = parseConditionInner variablesAreAllowed (conditionTokens |> List.take index)
                    let right = parseConditionInner variablesAreAllowed (conditionTokens |> List.skip (index + 1))
                    { HasErrors = left.HasErrors || right.HasErrors; Condition = ParsedOr(left.Condition, right.Condition) }
                else
                    if conditionTokens.Length = 3 then
                        match (conditionTokens.[0].Token, conditionTokens.[1].Token, conditionTokens.[2].Token) with
                        | (AttributeName name, Equals, QuotedString value) ->
                            { HasErrors = false; Condition = ParsedAreEqual(ParsedAttribute name, value) }
                        | (AttributeName name, NotEquals, QuotedString value) ->
                            { HasErrors = false; Condition = ParsedAreNotEqual(ParsedAttribute name, value) }
                        | (VariableName name, Equals, QuotedString value) ->
                            if variablesAreAllowed then
                                { HasErrors = false; Condition = ParsedAreEqual(ParsedVariable name, value) }
                            else
                                { HasErrors = true; Condition = ParsedConditionError "Invalid reference to variable in attribute-based condition" }
                        | (VariableName name, NotEquals, QuotedString value) ->
                            if variablesAreAllowed then
                                { HasErrors = false; Condition = ParsedAreNotEqual(ParsedVariable name, value) }
                            else
                                { HasErrors = true; Condition = ParsedConditionError "Invalid reference to variable in attribute-based condition" }
                        | _ ->
                            { HasErrors = true; Condition = ParsedConditionError "Invalid condition" }
                    else if (conditionTokens.[0].Token <> OpenBracket) || (conditionTokens.[conditionTokens.Length - 1].Token <> CloseBracket) then
                        { HasErrors = true; Condition = ParsedConditionError "Invalid condition" }
                    else
                        parseConditionInner variablesAreAllowed (conditionTokens |> List.skip 1 |> List.take (conditionTokens.Length - 2))

    /// Parse a condition.
    let parseCondition variablesAreAllowed (conditionTokens:AttributedToken list) =
        // It is known that the first is an OpenBrace. If the last is not a CloseBrace, it's an error.
        if conditionTokens.[conditionTokens.Length - 1].Token <> CloseBrace then { HasErrors = true; Condition = ParsedConditionError "Invalid condition" }
        else parseConditionInner variablesAreAllowed (conditionTokens |> List.skip 1 |> List.take (conditionTokens.Length - 2))


