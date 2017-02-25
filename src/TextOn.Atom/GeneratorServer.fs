namespace TextOn.Atom

open TextOn.Atom.DTO

type GeneratorStartResult =
    | CompilationFailure of CompilationError[]
    | GeneratorStarted of DTO.GeneratorData

[<Sealed>]
type GeneratorServer() =
    member __.Start(file, line, col) =
        ()

