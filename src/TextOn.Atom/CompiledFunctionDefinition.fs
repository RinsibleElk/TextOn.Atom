namespace TextOn.Atom

/// The definition of a function, compiled, with meta-data.
type CompiledFunctionDefinition =
    {
        Name : string
        Index : int
        File : string
        StartLine : int
        EndLine : int
        AttributeDependencies : int[]
        VariableDependencies : int[]
        Tree : CompiledDefinitionNode
    }
