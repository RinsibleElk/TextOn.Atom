namespace TextOn.Atom

/// The definition of a function, compiled, with meta-data.
type CompiledFunctionDefinition =
    {
        Name : string
        Index : int
        File : string
        IsPrivate : bool
        StartLine : int
        EndLine : int
        FunctionDependencies : int[]
        AttributeDependencies : int[]
        VariableDependencies : int[]
        Tree : CompiledDefinitionNode
    }
