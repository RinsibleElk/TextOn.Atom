namespace TextOn.Atom

type ParsedVariableSuggestedValue = {
     }

type ParsedVariableDefinition = {
    File : string
    StartLine : int
    EndLine : int
    Index : int
    HasErrors : bool
    Name : ParsedVariableName
    Text : string
    SupportsFreeValue : bool
    SuggestedValues : ParsedVariableSuggestedValue list }

module internal VariableDefinitionParser =
    let private a = 1

