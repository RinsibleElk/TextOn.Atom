﻿namespace TextOn.Atom

type ParsedAttributeName = string

type ParsedAttributeValue = {
    Value : string
    Condition : ParsedCondition }

type ParsedAttributeResult =
    | ParsedAttributeErrors of ParseError[]
    | ParsedAttributeSuccess of ParsedAttributeValue[]

type ParsedAttributeDefinition = {
    StartLine : int
    EndLine : int
    Index : int
    HasErrors : bool
    Name : ParsedAttributeName
    Result : ParsedAttributeResult }

module internal AttributeDefinitionParser =
    let private makeAttributeDefinition (tokenSet:CategorizedAttributedTokenSet) name result =
        {   StartLine = tokenSet.StartLine
            EndLine = tokenSet.EndLine
            Index = tokenSet.Index
            HasErrors = match result with | ParsedAttributeErrors _ -> true | _ -> false
            Name = name
            Result = result }

    let private makeParseError file line startLocation endLocation errorText =
        {   File = file
            LineNumber = line
            StartLocation = startLocation
            EndLocation = endLocation
            ErrorText = errorText }

    let private parseFirstLine (firstLine:AttributedTokenizedLine) =
        // Basically @att %AttName
        let tokens = firstLine.Tokens |> List.map (fun attToken -> attToken.Token)
        match tokens with
        | [Att;AttributeName name] -> (name, None)
        | _ -> ("<unknown>", Some "Invalid attribute declaration")

    type private ParsedSuggestionResult =
        | ParsedAttributeValueSuccess of ParsedAttributeValue
        | ParsedAttributeValueError of ParseError[]

    let private parseAttributeValue file (valueLine:AttributedTokenizedLine) =
        let tokens = valueLine.Tokens
        match tokens with
        | [] -> failwith "Internal error"
        | [textToken] ->
            match textToken.Token with
            | QuotedString value -> ParsedAttributeValueSuccess { Value = value ; Condition = ParsedUnconditional }
            | _ -> ParsedAttributeValueError [|(makeParseError file valueLine.LineNumber textToken.TokenStartLocation textToken.TokenEndLocation "Invalid token - expected a quoted string")|]
        | textToken::conditionTokens ->
            match textToken.Token with
            | QuotedString value ->
                match conditionTokens with
                | [] -> failwith "Internal error"
                | h::t ->
                    match h.Token with
                    | OpenBrace ->
                        let parsedCondition = ConditionParser.parseCondition file valueLine.LineNumber false conditionTokens
                        match parsedCondition.Condition with
                        | ParsedCondition.ParsedConditionError errors -> ParsedAttributeValueError errors
                        | _ -> ParsedAttributeValueSuccess { Value = value ; Condition = parsedCondition.Condition }
                    | _ ->
                        ParsedAttributeValueError [|(makeParseError file valueLine.LineNumber textToken.TokenStartLocation textToken.TokenEndLocation "Invalid token - expected '['")|]
            | _ -> ParsedAttributeValueError [|(makeParseError file valueLine.LineNumber textToken.TokenStartLocation textToken.TokenEndLocation "Invalid token - expected a quoted string")|]

    let private makeAttributeDefinitionWithValues file makeAttributeDefinition valueLines =
        let values = valueLines |> List.toArray |> Array.map (parseAttributeValue file)
        let errors = values |> Array.collect (function | ParsedAttributeValueError errors -> errors | _ -> [||])
        if errors.Length > 0 then
            makeAttributeDefinition (ParsedAttributeErrors errors)
        else
            makeAttributeDefinition (ParsedAttributeSuccess (values |> Array.map (function | ParsedAttributeValueSuccess x -> x | _ -> failwith "Internal error")))

    /// Parse a categorized attribute definition token set into a definition.
    let parseAttributeDefinition (tokenSet:CategorizedAttributedTokenSet) : ParsedAttributeDefinition =
        let lines = tokenSet.Tokens
        match lines with
        | [] -> failwith "Internal error"
        | firstLine::remainingLines ->
            let (name, error) = parseFirstLine firstLine
            if error |> Option.isSome then
                makeAttributeDefinition tokenSet name (ParsedAttributeErrors [|(makeParseError tokenSet.File firstLine.LineNumber (firstLine.Tokens.[0].TokenStartLocation) (firstLine.Tokens.[firstLine.Tokens.Length - 1].TokenEndLocation) error.Value)|])
            else
                match remainingLines with
                | [] -> makeAttributeDefinition tokenSet name (ParsedAttributeErrors [|(makeParseError tokenSet.File firstLine.LineNumber firstLine.Tokens.[0].TokenStartLocation firstLine.Tokens.[firstLine.Tokens.Length - 1].TokenEndLocation "No attribute values given")|])
                | h::t ->
                    if h.Tokens.Length > 1 || h.Tokens.[0].Token <> OpenCurly then
                        makeAttributeDefinition tokenSet name (ParsedAttributeErrors [|(makeParseError tokenSet.File h.LineNumber h.Tokens.[0].TokenStartLocation h.Tokens.[h.Tokens.Length - 1].TokenEndLocation "Expected '{'")|])
                    else
                        let l = t |> List.last
                        if l.Tokens.Length > 1 || l.Tokens.[0].Token <> CloseCurly then
                            makeAttributeDefinition tokenSet name (ParsedAttributeErrors [|(makeParseError tokenSet.File l.LineNumber l.Tokens.[0].TokenStartLocation l.Tokens.[l.Tokens.Length - 1].TokenEndLocation "Expected '}'")|])
                        else
                            makeAttributeDefinitionWithValues tokenSet.File (makeAttributeDefinition tokenSet name) (t |> List.take (t.Length - 1))
