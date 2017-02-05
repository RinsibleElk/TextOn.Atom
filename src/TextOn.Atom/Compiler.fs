namespace TextOn.Atom

open System
open System.Text.RegularExpressions

type CompilationError =
    | ParserError of ParseError
    | GeneralError of string

type CompilationResult =
    | CompilationFailure of CompilationError[]
    | CompilationSuccess of CompiledTemplate

type 'a ResultOrErrors =
    | Result of 'a
    | Errors of CompilationError[]

[<RequireQualifiedAccess>]
module Compiler =
    let private makeParseError file line s e t =
        ParserError
            {   File = file
                LineNumber = line
                StartLocation = s
                EndLocation = e
                ErrorText = t }
    let rec private compileCondition file attributeDefinitions condition =
        match condition with
        | ParsedUnconditional -> Result True
        | ParsedOr(c1, c2) ->
            match (compileCondition file attributeDefinitions c1) with
            | Errors e -> Errors e
            | Result c1 ->
                match (compileCondition file attributeDefinitions c2) with
                | Errors e -> Errors e
                | Result c2 -> Result (Either(c1, c2))
        | ParsedAnd(c1, c2) ->
            match (compileCondition file attributeDefinitions c1) with
            | Errors e -> Errors e
            | Result c1 ->
                match (compileCondition file attributeDefinitions c2) with
                | Errors e -> Errors e
                | Result c2 -> Result (Both(c1, c2))
        | ParsedAreEqual(startLocation, endLocation, ParsedAttributeName attribute, value) ->
            let att = attributeDefinitions |> Map.tryFind attribute
            failwith ""
        | ParsedAreNotEqual(startLocation, endLocation, ParsedAttributeName attribute, value) ->
            let att = attributeDefinitions |> Map.tryFind attribute
            failwith ""
        | _ -> failwith "Internal error"
    let rec private compileFunc file variableDefinitions attributeDefinitions functionDefinitions parsedNode =
        match parsedNode with
        | ParsedSentence(lineNumber, sentence) ->
            failwith ""
        | ParsedFunctionInvocation(lineNumber, startLocation, endLocation, functionName) ->
            let func = functionDefinitions |> Map.tryFind functionName
            match func with
            | None -> Errors [|(makeParseError file lineNumber startLocation endLocation (sprintf "Undefined function: %s" functionName))|]
            | Some f -> Result f
        | ParsedSeq(nodes) ->
            failwith ""
        | ParsedChoice(nodes) ->
            failwith ""
        | ParsedParagraphBreak(lineNumber) ->
            failwith ""
        | _ -> failwith "Internal error"
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



