namespace TextOn.Atom

open System
open System.Text.RegularExpressions

type FullFileError =
    {
        File : string
        ErrorText : string
    }

type CompilationError =
    | ParserError of ParseError
    | GeneralError of FullFileError

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
            match ((compileCondition file attributeDefinitions c1), (compileCondition file attributeDefinitions c2)) with
            | Errors e1, Errors e2 -> Errors (Array.append e1 e2)
            | Errors e, _
            | _, Errors e -> Errors e
            | Result c1, Result c2 -> Result (Either(c1, c2))
        | ParsedAnd(c1, c2) ->
            match ((compileCondition file attributeDefinitions c1), (compileCondition file attributeDefinitions c2)) with
            | Errors e1, Errors e2 -> Errors (Array.append e1 e2)
            | Errors e, _
            | _, Errors e -> Errors e
            | Result c1, Result c2 -> Result (Both(c1, c2))
        | ParsedAreEqual(line, startLocation, endLocation, ParsedAttributeName attribute, value) ->
            let att : CompiledAttributeDefinition option = attributeDefinitions |> Map.tryFind attribute
            match att with
            | None -> Errors [|(makeParseError file line startLocation endLocation (sprintf "Undefined attribute %s" attribute))|]
            | Some att -> Result (AreEqual(att.Index, value)) // OPS Missing validation.
        | ParsedAreNotEqual(line, startLocation, endLocation, ParsedAttributeName attribute, value) ->
            let att : CompiledAttributeDefinition option = attributeDefinitions |> Map.tryFind attribute
            match att with
            | None -> Errors [|(makeParseError file line startLocation endLocation (sprintf "Undefined attribute %s" attribute))|]
            | Some att -> Result (AreNotEqual(att.Index, value)) // OPS Missing validation.
        | _ -> failwith "Internal error"
    let rec private compileVariableCondition file variableDefinitions attributeDefinitions condition =
        match condition with
        | ParsedUnconditional -> Result VarTrue
        | ParsedOr(c1, c2) ->
            match (compileVariableCondition file variableDefinitions attributeDefinitions c1) with
            | Errors e -> Errors e
            | Result c1 ->
                match (compileVariableCondition file variableDefinitions attributeDefinitions c2) with
                | Errors e -> Errors e
                | Result c2 -> Result (VarEither(c1, c2))
        | ParsedAnd(c1, c2) ->
            match (compileVariableCondition file variableDefinitions attributeDefinitions c1) with
            | Errors e -> Errors e
            | Result c1 ->
                match (compileVariableCondition file variableDefinitions attributeDefinitions c2) with
                | Errors e -> Errors e
                | Result c2 -> Result (VarBoth(c1, c2))
        | ParsedAreEqual(line, startLocation, endLocation, ParsedAttributeName attribute, value) ->
            let att : CompiledAttributeDefinition option = attributeDefinitions |> Map.tryFind attribute
            match att with
            | None -> Errors [|(makeParseError file line startLocation endLocation (sprintf "Undefined attribute %s" attribute))|]
            | Some att -> Result (VarAreEqual(AttributeOrVariableIdentity.Attribute att.Index, value)) // OPS Missing validation.
        | ParsedAreNotEqual(line, startLocation, endLocation, ParsedAttributeName attribute, value) ->
            let att : CompiledAttributeDefinition option = attributeDefinitions |> Map.tryFind attribute
            match att with
            | None -> Errors [|(makeParseError file line startLocation endLocation (sprintf "Undefined attribute %s" attribute))|]
            | Some att -> Result (VarAreNotEqual(AttributeOrVariableIdentity.Attribute att.Index, value)) // OPS Missing validation.
        | ParsedAreEqual(line, startLocation, endLocation, ParsedVariableName variable, value) ->
            let att : CompiledVariableDefinition option = variableDefinitions |> Map.tryFind variable
            match att with
            | None -> Errors [|(makeParseError file line startLocation endLocation (sprintf "Undefined variable %s" variable))|]
            | Some att -> Result (VarAreEqual(AttributeOrVariableIdentity.Variable att.Index, value)) // OPS Missing validation.
        | ParsedAreNotEqual(line, startLocation, endLocation, ParsedVariableName variable, value) ->
            let att : CompiledVariableDefinition option = variableDefinitions |> Map.tryFind variable
            match att with
            | None -> Errors [|(makeParseError file line startLocation endLocation (sprintf "Undefined variable %s" variable))|]
            | Some att -> Result (VarAreNotEqual(AttributeOrVariableIdentity.Variable att.Index, value)) // OPS Missing validation.
        | _ -> failwith "Internal error"
    let rec private compileSentence file line (variableDefinitions:Map<ParsedVariableName, CompiledVariableDefinition>) sentence =
        match sentence with
        | ParsedStringValue(text) -> Result(SimpleText(text))
        | ParsedSimpleVariable(startLocation, endLocation, variableName) ->
            let variable = variableDefinitions |> Map.tryFind variableName
            match variable with
            | None -> Errors [|(makeParseError file line startLocation endLocation (sprintf "Undefined variable %s" variableName))|]
            | Some v -> Result (VariableValue(v.Index))
        | ParsedSimpleChoice(choices) ->
            let choices =
                choices
                |> Array.map (compileSentence file line variableDefinitions)
            let errors =
                choices
                |> Array.choose (function | Errors e -> (Some e) | _ -> None)
                |> Array.concat
            if errors.Length <> 0 then
                Errors errors
            else
                Result (SimpleChoice (choices |> Array.map (function | Result r -> r | _ -> failwith "Internal error")))
        | ParsedSimpleSeq(choices) ->
            let choices =
                choices
                |> Array.map (compileSentence file line variableDefinitions)
            let errors =
                choices
                |> Array.choose (function | Errors e -> (Some e) | _ -> None)
                |> Array.concat
            if errors.Length <> 0 then
                Errors errors
            else
                Result (SimpleSeq (choices |> Array.map (function | Result r -> r | _ -> failwith "Internal error")))
        | _ -> failwith "Internal error"
    let rec private compileFunc file variableDefinitions attributeDefinitions functionDefinitions parsedNode =
        match parsedNode with
        | ParsedSentence(lineNumber, sentence) ->
            match (compileSentence file lineNumber variableDefinitions sentence) with
            | Errors e -> Errors e
            | Result compiledSentence -> Result (Sentence(file, lineNumber, compiledSentence))
        | ParsedFunctionInvocation(lineNumber, startLocation, endLocation, functionName) ->
            let func : CompiledFunctionDefinition option = functionDefinitions |> Map.tryFind functionName
            match func with
            | None -> Errors [|(makeParseError file lineNumber startLocation endLocation (sprintf "Undefined function: %s" functionName))|]
            | Some f -> Result (Function f.Index)
        | ParsedSeq(nodes) ->
            let nodes =
                nodes
                |> Array.map (fun (node, condition) -> ((compileFunc file variableDefinitions attributeDefinitions functionDefinitions node), (compileCondition file attributeDefinitions condition)))
            let errors =
                nodes
                |> Array.choose
                    (function
                        | (Errors e1, Errors e2) -> Some (Array.append e1 e2)
                        | (Errors e, _)
                        | (_, Errors e) -> Some e
                        | _ -> None)
                |> Array.concat
            if errors.Length <> 0 then
                Errors errors
            else
                Result (Seq (nodes |> Array.map (function | (Result n, Result c) -> (n,c) | _ -> failwith "Internal error")))
        | ParsedChoice(nodes) ->
            let nodes =
                nodes
                |> Array.map (fun (node, condition) -> ((compileFunc file variableDefinitions attributeDefinitions functionDefinitions node), (compileCondition file attributeDefinitions condition)))
            let errors =
                nodes
                |> Array.choose
                    (function
                        | (Errors e1, Errors e2) -> Some (Array.append e1 e2)
                        | (Errors e, _)
                        | (_, Errors e) -> Some e
                        | _ -> None)
                |> Array.concat
            if errors.Length <> 0 then
                Errors errors
            else
                Result (Choice (nodes |> Array.map (function | (Result n, Result c) -> (n,c) | _ -> failwith "Internal error")))
        | ParsedParagraphBreak(lineNumber) ->
            Result (ParagraphBreak(file, lineNumber))
        | _ -> failwith "Internal error"
    let private compileAttribute (file:string) index startLine endLine name (attributeDefinitions:Map<ParsedAttributeName, CompiledAttributeDefinition>) (parsedAttributeValues:ParsedAttributeValue[]) : CompiledAttributeDefinition ResultOrErrors =
        let existing = attributeDefinitions |> Map.tryFind name
        if existing |> Option.isSome then
            Errors ([|(makeParseError file startLine 1 4 (sprintf "Duplicate definition of attribute %s" name))|])
        else
            let values =
                parsedAttributeValues
                |> Array.map
                    (fun value ->
                        match (compileCondition file attributeDefinitions value.Condition) with
                        | Errors e -> Errors e
                        | Result condition -> Result { CompiledAttributeValue.Value = value.Value ; Condition = condition })
            let errors =
                values
                |> Array.choose (function | Errors e -> Some e | _ -> None)
                |> Array.concat
            if errors.Length <> 0 then
                Errors errors
            else
                let values =
                    values
                    |> Array.map (function | Result r -> r | _ -> failwith "Internal error")
                Result
                    {
                        Name = name
                        Index = index
                        File = file
                        StartLine = startLine
                        EndLine = endLine
                        Values = values
                    }
    let private compileVariable (file:string) index startLine endLine name text supportsFreeValue (variableDefinitions:Map<ParsedVariableName, CompiledVariableDefinition>) (attributeDefinitions:Map<ParsedAttributeName, CompiledAttributeDefinition>) (parsedSuggestedValues:ParsedVariableSuggestedValue[]) : CompiledVariableDefinition ResultOrErrors =
        let existing = variableDefinitions |> Map.tryFind name
        if existing |> Option.isSome then
            Errors ([|(makeParseError file startLine 1 4 (sprintf "Duplicate definition of variable %s" name))|])
        else
            let suggestedValues =
                parsedSuggestedValues
                |> Array.map
                    (fun suggestedValue ->
                        match (compileVariableCondition file variableDefinitions attributeDefinitions suggestedValue.Condition) with
                        | Errors e -> Errors e
                        | Result condition -> Result { Value = suggestedValue.Value ; Condition = condition })
            let errors =
                suggestedValues
                |> Array.choose (function | Errors e -> Some e | _ -> None)
                |> Array.concat
            if errors.Length <> 0 then
                Errors errors
            else
                let values =
                    suggestedValues
                    |> Array.map (function | Result r -> r | _ -> failwith "Internal error")
                Result
                    {
                        Name = name
                        Index = index
                        File = file
                        StartLine = startLine
                        EndLine = endLine
                        PermitsFreeValue = supportsFreeValue
                        Text = text
                        Values = values
                    }
    let rec private compileInner variableDefinitions attributeDefinitions functionDefinitions errors (elements:ParsedElement list) =
        match elements with
        | [] ->
            match errors with
            | [] ->
                CompilationSuccess
                    {
                        Attributes = attributeDefinitions |> Map.toArray |> Array.map snd |> Array.sortBy (fun v -> v.Index)
                        Variables = variableDefinitions |> Map.toArray |> Array.map snd |> Array.sortBy (fun v -> v.Index)
                        Functions = functionDefinitions |> Map.toArray |> Array.map snd |> Array.sortBy (fun v -> v.Index)
                    }
            | _ ->
                CompilationFailure (errors |> List.toArray)
        | h::t ->
            let (v,a,f,e) =
                match h.Result with
                | ParserErrors x -> (variableDefinitions, attributeDefinitions, functionDefinitions, errors@(x |> List.ofArray |> List.map ParserError))
                | ParsedFunction f ->
                    // Traverse through replacing variable & attribute references and inlining function references.
                    if f.HasErrors then
                        match f.Tree with
                        | ParseErrors x -> (variableDefinitions, attributeDefinitions, functionDefinitions, errors@(x |> List.ofArray |> List.map ParserError))
                        | _ -> (variableDefinitions, attributeDefinitions, functionDefinitions, errors)
                    else
                        let existing = functionDefinitions |> Map.tryFind f.Name
                        if existing.IsSome then
                            let e =
                                [|(makeParseError h.File f.StartLine 1 5 (sprintf "Duplicate definition of function %s" f.Name))|]
                            (variableDefinitions, attributeDefinitions, functionDefinitions, errors@(e |> List.ofArray))
                        else
                            match (compileFunc h.File variableDefinitions attributeDefinitions functionDefinitions f.Tree) with
                            | Errors e -> (variableDefinitions, attributeDefinitions, functionDefinitions, errors@(e |> List.ofArray))
                            | Result r ->
                                let fn = {
                                    Name = f.Name
                                    Index = f.Index
                                    File = h.File
                                    StartLine = f.StartLine
                                    EndLine = f.EndLine
                                    Tree = r }
                                (variableDefinitions, attributeDefinitions, (functionDefinitions |> Map.add f.Name fn), errors)
                | ParsedAttribute a ->
                    // Traverse through replacing variable & attribute references and inlining function references.
                    if a.HasErrors then
                        match a.Result with
                        | ParsedAttributeErrors x -> (variableDefinitions, attributeDefinitions, functionDefinitions, errors@(x |> List.ofArray |> List.map ParserError))
                        | _ -> (variableDefinitions, attributeDefinitions, functionDefinitions, errors)
                    else
                        match (compileAttribute h.File a.Index a.StartLine a.EndLine a.Name attributeDefinitions (match a.Result with | ParsedAttributeSuccess a -> a | _ -> failwith "Internal error")) with
                        | Errors e -> (variableDefinitions, attributeDefinitions, functionDefinitions, errors@(e |> List.ofArray))
                        | Result r -> (variableDefinitions, (attributeDefinitions |> Map.add a.Name r), functionDefinitions, errors)
                | ParsedVariable v ->
                    // Traverse through replacing variable & attribute references and inlining function references.
                    if v.HasErrors then
                        match v.Result with
                        | ParsedVariableErrors x -> (variableDefinitions, attributeDefinitions, functionDefinitions, errors@(x |> List.ofArray |> List.map ParserError))
                        | _ -> (variableDefinitions, attributeDefinitions, functionDefinitions, errors)
                    else
                        match (compileVariable h.File v.Index v.StartLine v.EndLine v.Name v.Text v.SupportsFreeValue variableDefinitions attributeDefinitions (match v.Result with | ParsedVariableSuccess v -> v | _ -> failwith "Internal error")) with
                        | Errors e -> (variableDefinitions, attributeDefinitions, functionDefinitions, errors@(e |> List.ofArray))
                        | Result r -> ((variableDefinitions |> Map.add v.Name r), attributeDefinitions, functionDefinitions, errors)
            compileInner v a f e t

    /// Compile the results of parsing into a tree, with inlined functions, variables and attributes.
    let compile (elements:ParsedElement list) : CompilationResult =
        compileInner Map.empty Map.empty Map.empty [] elements



