namespace TextOn.Atom

type ParsedVariableSuggestedValue = {
    Value : string
    Condition : ParsedCondition }

type ParsedVariableResult =
    | ParsedVariableErrors of ParseError[]
    | ParsedVariableSuccess of ParsedVariableSuggestedValue[]

type ParsedVariableDefinition = {
    File : string
    StartLine : int
    EndLine : int
    Index : int
    HasErrors : bool
    Name : ParsedVariableName
    Text : string
    SupportsFreeValue : bool
    Result : ParsedVariableResult }

module internal VariableDefinitionParser =
    let private makeVariableDefinition (tokenSet:CategorizedAttributedTokenSet) name text freeValue result =
        {   File = tokenSet.File
            StartLine = tokenSet.StartLine
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

    /// Parse the CategorizedAttributedTokenSet for a variable definition into a tree.
    let parseVariableDefinition (tokenSet:CategorizedAttributedTokenSet) : ParsedVariableDefinition =
        let lines = tokenSet.Tokens
        match lines with
        | [] -> failwith "Internal error"
        | firstLine::remainingLines ->
            let (name, text, freevalue, error) = parseFirstLine firstLine
            if error |> Option.isSome then
                makeVariableDefinition tokenSet name (text |> defaultArg <| "") freevalue (ParsedVariableErrors [|(makeParseError firstLine.LineNumber (firstLine.Tokens.[0].TokenStartLocation) (firstLine.Tokens.[firstLine.Tokens.Length - 1].TokenEndLocation) error.Value)|])
            else
                if text |> Option.isSome then
                    // Remaining lines 
                    failwith ""
                else
                    failwith ""
