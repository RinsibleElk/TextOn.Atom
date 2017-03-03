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
    Text : string
    Result : ParsedAttributeResult }

module internal AttributeDefinitionParser =
    let private makeAttributeDefinition (tokenSet:CategorizedAttributedTokenSet) name text result =
        {   StartLine = tokenSet.StartLine
            EndLine = tokenSet.EndLine
            Index = tokenSet.Index
            HasErrors = match result with | ParsedAttributeErrors _ -> true | _ -> false
            Name = name
            Text = text
            Result = result }

    let private makeParseError file line startLocation endLocation errorText =
        {   File = file
            LineNumber = line
            StartLocation = startLocation
            EndLocation = endLocation
            ErrorText = errorText }

    let private parseFirstLine' (firstLine:AttributedTokenizedLine) =
        // Basically @var (@free)? $VarName = ("Some text")?
        let tokens = firstLine.Tokens |> List.map (fun attToken -> attToken.Token)
        match tokens with
        | [Var;VariableName name;Equals] -> (name, None, false, None)
        | [Var;Free;VariableName name;Equals] -> (name, None, true, None)
        | [Var;VariableName name;Equals;QuotedString text] -> (name, Some text, false, None)
        | [Var;Free;VariableName name;Equals;QuotedString text] -> (name, Some text, true, None)
        | _ -> ("<unknown>", None, false, Some "Invalid variable declaration")

    let private parseFirstLine (firstLine:AttributedTokenizedLine) =
        // Basically @att %AttName = ("Some description")?
        let tokens = firstLine.Tokens |> List.map (fun attToken -> attToken.Token)
        match tokens with
        | [Att;AttributeName name;Equals] -> (name, None, None)
        | [Att;AttributeName name;Equals;QuotedString text] -> (name, Some text, None)
        | _ -> ("<unknown>", None, Some "Invalid attribute declaration")

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
            let (name, text, error) = parseFirstLine firstLine
            if error |> Option.isSome then
                makeAttributeDefinition tokenSet name (text |> defaultArg <| "") (ParsedAttributeErrors [|(makeParseError tokenSet.File firstLine.LineNumber (firstLine.Tokens.[0].TokenStartLocation) (firstLine.Tokens.[firstLine.Tokens.Length - 1].TokenEndLocation) error.Value)|])
            else if text |> Option.isSome then
                let text = text.Value
                match remainingLines with
                | [] -> makeAttributeDefinition tokenSet name text (ParsedAttributeErrors [|(makeParseError tokenSet.File firstLine.LineNumber firstLine.Tokens.[0].TokenStartLocation firstLine.Tokens.[firstLine.Tokens.Length - 1].TokenEndLocation "No attribute values given")|])
                | h::t ->
                    if h.Tokens.Length > 1 || h.Tokens.[0].Token <> OpenCurly then
                        makeAttributeDefinition tokenSet name text (ParsedAttributeErrors [|(makeParseError tokenSet.File h.LineNumber h.Tokens.[0].TokenStartLocation h.Tokens.[h.Tokens.Length - 1].TokenEndLocation "Expected '{'")|])
                    else
                        let l = t |> List.last
                        if l.Tokens.Length > 1 || l.Tokens.[0].Token <> CloseCurly then
                            makeAttributeDefinition tokenSet name text (ParsedAttributeErrors [|(makeParseError tokenSet.File l.LineNumber l.Tokens.[0].TokenStartLocation l.Tokens.[l.Tokens.Length - 1].TokenEndLocation "Expected '}'")|])
                        else
                            makeAttributeDefinitionWithValues tokenSet.File (makeAttributeDefinition tokenSet name text) (t |> List.take (t.Length - 1))
            else
                match remainingLines with
                | [] -> makeAttributeDefinition tokenSet name "" (ParsedAttributeErrors [|(makeParseError tokenSet.File firstLine.LineNumber (firstLine.Tokens.[0].TokenStartLocation) (firstLine.Tokens.[firstLine.Tokens.Length - 1].TokenEndLocation) "Expected quoted string")|])
                | h::t ->
                    match (h.Tokens |> List.map (fun t -> t.Token)) with
                    | [QuotedString text] ->
                        match t with
                        | [] -> makeAttributeDefinition tokenSet name text (ParsedAttributeErrors [|(makeParseError tokenSet.File firstLine.LineNumber (firstLine.Tokens.[0].TokenStartLocation) (firstLine.Tokens.[firstLine.Tokens.Length - 1].TokenEndLocation) "No values defined for attribute")|])
                        | h::t ->
                            if h.Tokens.Length > 1 || h.Tokens.[0].Token <> OpenCurly then
                                makeAttributeDefinition tokenSet name text (ParsedAttributeErrors [|(makeParseError tokenSet.File h.LineNumber h.Tokens.[0].TokenStartLocation h.Tokens.[h.Tokens.Length - 1].TokenEndLocation "Expected '{'")|])
                            else
                                let l = t |> List.last
                                if l.Tokens.Length > 1 || l.Tokens.[0].Token <> CloseCurly then
                                    makeAttributeDefinition tokenSet name text (ParsedAttributeErrors [|(makeParseError tokenSet.File l.LineNumber l.Tokens.[0].TokenStartLocation l.Tokens.[l.Tokens.Length - 1].TokenEndLocation "Expected '}'")|])
                                else
                                    makeAttributeDefinitionWithValues tokenSet.File (makeAttributeDefinition tokenSet name text) (t |> List.take (t.Length - 1))
                    | _ ->
                        makeAttributeDefinition tokenSet name "" (ParsedAttributeErrors [|(makeParseError tokenSet.File h.LineNumber (h.Tokens.[0].TokenStartLocation) (h.Tokens.[h.Tokens.Length - 1].TokenEndLocation) "Expected quoted string")|])
