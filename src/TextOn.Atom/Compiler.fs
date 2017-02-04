namespace TextOn.Atom

open System
open System.Text.RegularExpressions

type CompilationError =
    | ParserError of ParseError
    | CompilerError of string

type CompilationResult =
    | CompilationFailure of CompilationError[]
    | CompilationSuccess of CompiledTemplate

[<RequireQualifiedAccess>]
module Compiler =
    let rec private compileInner variableDefinitions attributeDefinitions functionDefinitions errors (elements:ParsedElement list) =
        match elements with
        | [] ->
            match errors with
            | [] ->
                let mainFunction = functionDefinitions |> Map.tryFind "main"
                if mainFunction |> Option.isSome then
                    CompilationSuccess
                        {
                            Attributes = attributeDefinitions |> Map.toArray |> Array.map snd
                            Variables = variableDefinitions |> Map.toArray |> Array.map snd
                            Definition = mainFunction.Value
                        }
                else
                    CompilationFailure([|CompilerError "No main function specified"|])
            | _ ->
                CompilationFailure (errors |> List.rev |> List.toArray)
        | h::t ->
            failwith ""

    /// Compile the results of parsing into a tree, with inlined functions, variables and attributes.
    let compile (elements:ParsedElement[]) : CompilationResult =
        compileInner Map.empty Map.empty Map.empty [] (elements |> List.ofArray)



