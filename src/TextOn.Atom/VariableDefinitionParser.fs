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
    /// Parse the CategorizedAttributedTokenSet for a variable definition into a tree.
    let parseVariableDefinition (tokenSet:CategorizedAttributedTokenSet) : ParsedVariableDefinition =
        let lines = tokenSet.Tokens
        match lines with
        | [] -> failwith "Internal error"
        | firstLine::remainingLines ->
            match firstLine.Tokens with
            | [varToken;varNameToken] ->
                match varNameToken.Token with
                | VariableName varName ->
                    failwith ""
                | _ -> failwith ""
            | _ -> failwith ""
