namespace TextOn.Atom

type ParsedVariableSuggestedValue = {
    Value : string
    Condition : ParsedCondition }

type ParsedVariableResult =
    | ParsedVariableErrors of ParseError[]
    | ParsedVariableSuccess of ParsedVariableSuggestedValue[]

type ParsedVariableDefinition = {
    StartLine : int
    EndLine : int
    Index : int
    HasErrors : bool
    Name : ParsedVariableName
    Text : string
    SupportsFreeValue : bool
    Result : ParsedVariableResult }

module internal VariableDefinitionParser =
    type private ParsedSuggestionResult =
        | ParsedSuggestionSuccess of ParsedVariableSuggestedValue
        | ParsedSuggestionError of ParseError[]
    let private makeVariableDefinition (tokenSet:CategorizedAttributedTokenSet) name text freeValue result =
        {   StartLine = tokenSet.StartLine
            EndLine = tokenSet.EndLine
            Index = tokenSet.Index
            HasErrors = match result with | ParsedVariableErrors _ -> true | _ -> false
            Name = name
            Text = text
            SupportsFreeValue = freeValue
            Result = result }
    let private makeParseError line startLocation endLocation errorText =
        {   LineNumber = line
            StartLocation = startLocation
            EndLocation = endLocation
            ErrorText = errorText }
    let private parseFirstLine (firstLine:AttributedTokenizedLine) =
        // Basically @var (@free)? $VarName = ("Some text")?
        let tokens = firstLine.Tokens |> List.map (fun attToken -> attToken.Token)
        match tokens with
        | [Var;VariableName name;Equals] -> (name, None, false, None)
        | [Var;Free;VariableName name;Equals] -> (name, None, true, None)
        | [Var;VariableName name;Equals;QuotedString text] -> (name, Some text, false, None)
        | [Var;Free;VariableName name;Equals;QuotedString text] -> (name, Some text, true, None)
        | _ -> ("<unknown>", None, false, Some "Invalid variable declaration")

    let private parseSuggestion (suggestionLine:AttributedTokenizedLine) =
        let tokens = suggestionLine.Tokens
        match tokens with
        | [] -> failwith "Internal error"
        | [textToken] ->
            match textToken.Token with
            | QuotedString suggestedValue -> ParsedSuggestionSuccess { Value = suggestedValue ; Condition = ParsedUnconditional }
            | _ -> ParsedSuggestionError [|(makeParseError suggestionLine.LineNumber textToken.TokenStartLocation textToken.TokenEndLocation "Invalid token - expected a quoted string")|]
        | textToken::conditionTokens ->
            match textToken.Token with
            | QuotedString suggestedValue ->
                match conditionTokens with
                | [] -> failwith "Internal error"
                | h::t ->
                    match h.Token with
                    | OpenBrace ->
                        let parsedCondition = ConditionParser.parseCondition suggestionLine.LineNumber true conditionTokens
                        match parsedCondition.Condition with
                        | ParsedCondition.ParsedConditionError errors -> ParsedSuggestionError errors
                        | _ -> ParsedSuggestionSuccess { Value = suggestedValue ; Condition = parsedCondition.Condition }
                    | _ ->
                        ParsedSuggestionError [|(makeParseError suggestionLine.LineNumber textToken.TokenStartLocation textToken.TokenEndLocation "Invalid token - expected '['")|]
            | _ -> ParsedSuggestionError [|(makeParseError suggestionLine.LineNumber textToken.TokenStartLocation textToken.TokenEndLocation "Invalid token - expected a quoted string")|]

    let private makeVariableDefinitionWithSuggestions makeVariableDefinition suggestionLines =
        let suggestedValues = suggestionLines |> List.toArray |> Array.map parseSuggestion
        let errors = suggestedValues |> Array.collect (function | ParsedSuggestionError errors -> errors | _ -> [||])
        if errors.Length > 0 then
            makeVariableDefinition (ParsedVariableErrors errors)
        else
            makeVariableDefinition (ParsedVariableSuccess (suggestedValues |> Array.map (function | ParsedSuggestionSuccess x -> x | _ -> failwith "Internal error")))

    /// Parse the CategorizedAttributedTokenSet for a variable definition into a tree.
    let parseVariableDefinition (tokenSet:CategorizedAttributedTokenSet) : ParsedVariableDefinition =
        let lines = tokenSet.Tokens
        match lines with
        | [] -> failwith "Internal error"
        | firstLine::remainingLines ->
            let (name, text, freeValue, error) = parseFirstLine firstLine
            if error |> Option.isSome then
                makeVariableDefinition tokenSet name (text |> defaultArg <| "") freeValue (ParsedVariableErrors [|(makeParseError firstLine.LineNumber (firstLine.Tokens.[0].TokenStartLocation) (firstLine.Tokens.[firstLine.Tokens.Length - 1].TokenEndLocation) error.Value)|])
            else
                if text |> Option.isSome then
                    let text = text.Value
                    // If it's a free, that might be it. We don't validate that the suggestions make sense in this phase though.
                    match remainingLines with
                    | [] -> makeVariableDefinition tokenSet name text freeValue (ParsedVariableSuccess [||])
                    | h::t ->
                        if h.Tokens.Length > 1 || h.Tokens.[0].Token <> OpenCurly then
                            makeVariableDefinition tokenSet name text freeValue (ParsedVariableErrors [|(makeParseError h.LineNumber h.Tokens.[0].TokenStartLocation h.Tokens.[h.Tokens.Length - 1].TokenEndLocation "Expected '{'")|])
                        else
                            let l = t |> List.last
                            if l.Tokens.Length > 1 || l.Tokens.[0].Token <> CloseCurly then
                                makeVariableDefinition tokenSet name text freeValue (ParsedVariableErrors [|(makeParseError l.LineNumber l.Tokens.[0].TokenStartLocation l.Tokens.[l.Tokens.Length - 1].TokenEndLocation "Expected '}'")|])
                            else
                                makeVariableDefinitionWithSuggestions (makeVariableDefinition tokenSet name text freeValue) (t |> List.take (t.Length - 1))
                else
                    match remainingLines with
                    | [] -> makeVariableDefinition tokenSet name "" freeValue (ParsedVariableErrors [|(makeParseError firstLine.LineNumber (firstLine.Tokens.[0].TokenStartLocation) (firstLine.Tokens.[firstLine.Tokens.Length - 1].TokenEndLocation) "Expected quoted string")|])
                    | h::t ->
                        match (h.Tokens |> List.map (fun t -> t.Token)) with
                        | [QuotedString text] ->
                            match t with
                            | [] ->
                                // If it's a free, that might be it. We don't validate that the suggestions make sense in this phase though.
                                makeVariableDefinition tokenSet name text freeValue (ParsedVariableSuccess [||])
                            | h::t ->
                                if h.Tokens.Length > 1 || h.Tokens.[0].Token <> OpenCurly then
                                    makeVariableDefinition tokenSet name text freeValue (ParsedVariableErrors [|(makeParseError h.LineNumber h.Tokens.[0].TokenStartLocation h.Tokens.[h.Tokens.Length - 1].TokenEndLocation "Expected '{'")|])
                                else
                                    let l = t |> List.last
                                    if l.Tokens.Length > 1 || l.Tokens.[0].Token <> CloseCurly then
                                        makeVariableDefinition tokenSet name text freeValue (ParsedVariableErrors [|(makeParseError l.LineNumber l.Tokens.[0].TokenStartLocation l.Tokens.[l.Tokens.Length - 1].TokenEndLocation "Expected '}'")|])
                                    else
                                        makeVariableDefinitionWithSuggestions (makeVariableDefinition tokenSet name text freeValue) (t |> List.take (t.Length - 1))
                        | _ ->
                            makeVariableDefinition tokenSet name "" freeValue (ParsedVariableErrors [|(makeParseError h.LineNumber (h.Tokens.[0].TokenStartLocation) (h.Tokens.[h.Tokens.Length - 1].TokenEndLocation) "Expected quoted string")|])
