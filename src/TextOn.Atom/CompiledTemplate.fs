﻿namespace TextOn.Atom

type CompiledTemplate =
    {
        Attributes : CompiledAttributeDefinition[]
        Variables : CompiledVariableDefinition[]
        Functions : CompiledFunctionDefinition[]
    }
