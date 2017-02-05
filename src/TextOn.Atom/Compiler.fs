namespace TextOn.Atom

open System
open System.Text.RegularExpressions

type CompilationError =
    | ParserError of ParseError
    | GeneralError of string

type CompilationResult =
    | CompilationFailure of CompilationError[]
    | CompilationSuccess of CompiledTemplate

[<RequireQualifiedAccess>]
module Compiler =
    let rec private compileFunc file variableDefinitions attributeDefinitions functionDefinitions parsedNode =
        ()
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
                    CompilationFailure([|GeneralError "No main function specified"|])
            | _ ->
                CompilationFailure (errors |> List.toArray)
        | h::t ->
            let (v,a,f,e) =
                match h.Result with
                | ParserErrors x -> (variableDefinitions, attributeDefinitions, functionDefinitions, errors@(x |> List.ofArray |> List.map ParserError))
                | ParsedFunction f ->
                    // Traverse through replacing variable & attribute references and inlining function references.
                    if f.HasErrors || (errors |> List.isEmpty |> not) then
                        match f.Tree with
                        | ParseErrors x -> (variableDefinitions, attributeDefinitions, functionDefinitions, errors@(x |> List.ofArray |> List.map ParserError))
                        | _ -> (variableDefinitions, attributeDefinitions, functionDefinitions, errors)
                    else
                        let x = compileFunc h.File variableDefinitions attributeDefinitions functionDefinitions f.Tree
                        failwith ""
                | _ -> failwith ""
            compileInner v a f e t

    /// Compile the results of parsing into a tree, with inlined functions, variables and attributes.
    let compile (elements:ParsedElement[]) : CompilationResult =
        compileInner Map.empty Map.empty Map.empty [] (elements |> List.ofArray)



