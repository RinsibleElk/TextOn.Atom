namespace TextOn.Atom

type NodeHash = int

type SimpleCompiledDefinitionNode =
    | VariableValue of int
    | SimpleChoice of SimpleCompiledDefinitionNode[]
    | SimpleSeq of SimpleCompiledDefinitionNode[]
    | SimpleText of string

type CompiledDefinitionNode =
    | Sentence of (string * int * SimpleCompiledDefinitionNode)
    | ParagraphBreak of string * int
    | Choice of string * int * (CompiledDefinitionNode * Condition)[]
    | Seq of string * int * (CompiledDefinitionNode * Condition)[]
    | Function of string * int * int
