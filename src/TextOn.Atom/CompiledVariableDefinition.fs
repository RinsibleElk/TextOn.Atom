namespace TextOn.Atom

/// The definition of a variable value, compiled, with meta-data.
type CompiledVariableValue =
    {
        Value : string
        Condition : VariableCondition
    }

/// The definition of a variable, compiled, with meta-data.
type CompiledVariableDefinition =
    {
        Name : string
        Index : int
        File : string
        StartLine : int
        EndLine : int
        PermitsFreeValue : bool
        Text : string
        Values : CompiledVariableValue[]
    }
