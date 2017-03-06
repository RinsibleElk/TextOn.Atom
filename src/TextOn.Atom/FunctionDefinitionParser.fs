namespace TextOn.Atom

open System

type ParsedSentenceNode =
    | ParsedStringValue of string
    | ParsedSimpleVariable of (int * int * string)
    | ParsedSimpleChoice of ParsedSentenceNode[]
    | ParsedSimpleSeq of ParsedSentenceNode[]
    | ParsedSentenceErrors of ParseError[]

type ParsedNode =
    | ParsedSentence of (int * ParsedSentenceNode)
    | ParsedFunctionInvocation of (int * int * int * string)
    | ParsedSeq of (ParsedNode * ParsedCondition)[]
    | ParsedChoice of (ParsedNode * ParsedCondition)[]
    | ParsedParagraphBreak of int
    | ParseErrors of ParseError[]

type ParsedNodeWithDependencies =
    {
        Dependencies : ParsedAttributeOrVariable[]
        Node : ParsedNode
    }

type ParsedSentenceNodeWithDependencies =
    {
        Dependencies : ParsedAttributeOrVariable[]
        SentenceNode : ParsedSentenceNode
    }

type ParsedFunctionDefinition = {
    StartLine : int
    EndLine : int
    Index : int
    HasErrors : bool
    Name : string
    Dependencies : ParsedAttributeOrVariable[]
    Tree : ParsedNode }

[<RequireQualifiedAccess>]
module internal FunctionDefinitionParser =
    let private makeParseError file line s e t =
        {   File = file
            LineNumber = line
            StartLocation = s
            EndLocation = e
            ErrorText = t }
    let rec private listPartition indices li =
        match indices with
        | [] -> [li]
        | h::t -> [(li |> List.take h)]@(listPartition (t |> List.map (fun x -> x - h - 1)) (li |> List.skip (h + 1)))
    let private makeParsedSentenceErrors errors =
        {
            Dependencies = [||]
            SentenceNode = ParsedSentenceErrors errors
        }
    let private makeParsedStringValue text =
        {
            Dependencies = [||]
            SentenceNode = ParsedStringValue text
        }
    let private makeParsedSimpleVariable tokenStartLocation tokenEndLocation name =
        {
            Dependencies = [|ParsedAttributeOrVariable.ParsedVariableName name|]
            SentenceNode = ParsedSimpleVariable (tokenStartLocation, tokenEndLocation, name)
        }
    let private makeParsedSimpleChoice (choices:ParsedSentenceNodeWithDependencies[]) =
        {
            Dependencies = choices |> Seq.collect (fun c -> c.Dependencies) |> Set.ofSeq |> Set.toArray
            SentenceNode = choices |> Array.map (fun c -> c.SentenceNode) |> ParsedSimpleChoice
        }
    let rec private parseSimpleSequentialInner file line output tokens =
        match tokens with
        | [] ->
            // Success!
            let l = output |> List.length
            if l = 0 then { Dependencies = [||] ; SentenceNode = ParsedStringValue "" }
            else if l = 1 then output |> List.head
            else { Dependencies = (output |> Seq.collect (fun x -> x.Dependencies) |> Set.ofSeq |> Set.toArray) ; SentenceNode = ParsedSimpleSeq (output |> List.map (fun x -> x.SentenceNode) |> List.rev |> Array.ofList) }
        | h::t ->
            match h.Token with
            | OpenCurly ->
                // If it's the start of a choice, then find the end of the choice.
                let endIndex =
                    t
                    |> List.scan
                        (fun (bracketCount,token) attToken ->
                            match attToken.Token with
                            | CloseCurly -> (bracketCount - 1, Some CloseCurly)
                            | OpenCurly -> (bracketCount + 1, Some OpenCurly)
                            | _ -> (bracketCount, Some attToken.Token))
                        (1, None)
                    |> List.skip 1
                    |> List.tryFindIndex (fun (a,o) -> a = 0 && o.Value = CloseCurly)
                if endIndex |> Option.isNone then
                    makeParsedSentenceErrors [|(makeParseError file line h.TokenStartLocation ((t |> List.tryLast |> defaultArg <| h).TokenEndLocation) "Unmatched open curly")|]
                else
                    let parsedChoice = parseSimpleChoiceInner file line (t |> List.take endIndex.Value)
                    match parsedChoice.SentenceNode with
                    | ParsedSentenceErrors errors -> parsedChoice
                    | _ -> parseSimpleSequentialInner file line (parsedChoice::output) (t |> List.skip (endIndex.Value + 1))
            | RawText text -> parseSimpleSequentialInner file line ((makeParsedStringValue text)::output) t
            | VariableName name -> parseSimpleSequentialInner file line ((makeParsedSimpleVariable h.TokenStartLocation h.TokenEndLocation name)::output) t
            | _ -> makeParsedSentenceErrors [|(makeParseError file line h.TokenStartLocation h.TokenEndLocation "Invalid token")|]
    and private parseSimpleChoiceInner file line (tokens:AttributedToken list) =
        // Find '|' tokens at curly count 0.
        let li =
            tokens
            |> List.scan
                (fun (bracketCount,index,_) attToken ->
                    if attToken.Token = OpenCurly then (bracketCount + 1, index + 1, Some attToken)
                    else if attToken.Token = CloseCurly then (bracketCount - 1, index + 1, Some attToken)
                    else (bracketCount, index + 1, Some attToken))
                (0, -1, None)
            |> List.skip 1
        let indices =
            li
            |> List.filter (fun (a, _, ao) -> a = 0 && ao.Value.Token = ChoiceSeparator)
            |> List.map (fun (_, i, _) -> i)
        listPartition indices tokens
        |> List.map (parseSimpleSequentialInner file line [])
        |> Array.ofList
        |> fun a ->
            // If there are any errors, then concatenate the errors and return that.
            a
            |> Array.map (fun x -> x.SentenceNode)
            |> Array.choose (function | ParsedSentenceErrors y -> Some y | _ -> None)
            |> Array.concat
            |> fun errors ->
                if errors.Length > 0 then
                    makeParsedSentenceErrors errors
                else
                    makeParsedSimpleChoice a

    let private makeParsedFunctionInvocation lineNumber startLocation endLocation functionName =
        {
            Dependencies = [||]
            Node = ParsedFunctionInvocation(lineNumber, startLocation, endLocation, functionName)
        }

    let private unconditional =
        {
            HasErrors = false
            Dependencies = [||]
            Condition = ParsedUnconditional
        }

    let private makeParseErrors errors =
        {
            Dependencies = [||]
            Node = ParseErrors(errors)
        }

    let private makeParsedMultiple makeNode (nodes:(ParsedNodeWithDependencies * ConditionParseResults)[]) : ParsedNodeWithDependencies =
        {
            Dependencies = nodes |> Seq.collect (fun (n,c) -> Set.union (n.Dependencies |> Set.ofArray) (c.Dependencies |> Set.ofArray)) |> Set.ofSeq |> Set.toArray
            Node = nodes |> Array.map (fun (n,c) -> n.Node, c.Condition) |> makeNode
        }

    let private makeParsedSeq = makeParsedMultiple ParsedSeq
    let private makeParsedChoice = makeParsedMultiple ParsedChoice

    let private makeParagraphBreak lineNumber =
        {
            Dependencies = [||]
            Node = ParsedParagraphBreak lineNumber
        }

    let private makeParsedSentence lineNumber sentenceNode =
        {
            Node = ParsedSentence(lineNumber, sentenceNode.SentenceNode)
            Dependencies = sentenceNode.Dependencies
        }

    /// Name of the function and closing bracket and stuff already dealt with.
    /// Each line is one of the following:
    /// - A function invocation (permits condition).
    /// - A break (permits condition).
    /// - The start of a seq.
    /// - The end of a seq (permits condition).
    /// - The start of a choice.
    /// - The end of a choice (permits condition).
    /// - Some text (permits condition).
    let rec private parseSequentialOrChoiceInner file (makeNode:(ParsedNodeWithDependencies * ConditionParseResults)[] -> ParsedNodeWithDependencies) output (tokens:AttributedTokenizedLine list) =
        match tokens with
        | [] -> makeNode (output |> List.rev |> Array.ofList)
        | line::t ->
            match line.Tokens.[0].Token with
            | FunctionName functionName ->
                if line.Tokens.Length = 1 then
                    parseSequentialOrChoiceInner file makeNode (((makeParsedFunctionInvocation line.LineNumber line.Tokens.[0].TokenStartLocation line.Tokens.[0].TokenEndLocation functionName), unconditional)::output) t
                else if line.Tokens.[1].Token <> OpenBrace then
                    makeParseErrors [|(makeParseError file line.LineNumber line.Tokens.[1].TokenStartLocation line.Tokens.[1].TokenEndLocation "Invalid token")|]
                else
                    let condition = ConditionParser.parseCondition file line.LineNumber false (line.Tokens |> List.skip 1)
                    if condition.HasErrors then
                        match condition.Condition with
                        | ParsedCondition.ParsedConditionError errors -> makeParseErrors errors
                        | _ -> failwith "Internal error"
                    else
                        parseSequentialOrChoiceInner file makeNode (((makeParsedFunctionInvocation line.LineNumber line.Tokens.[0].TokenStartLocation line.Tokens.[0].TokenEndLocation functionName), condition)::output) t
            | Break ->
                if line.Tokens.Length = 1 then
                    parseSequentialOrChoiceInner file makeNode (((makeParagraphBreak line.LineNumber), unconditional)::output) t
                else if line.Tokens.[1].Token <> OpenBrace then
                    makeParseErrors([|(makeParseError file line.LineNumber line.Tokens.[1].TokenStartLocation line.Tokens.[1].TokenEndLocation "Invalid token")|])
                else
                    let condition = ConditionParser.parseCondition file line.LineNumber false (line.Tokens |> List.skip 1)
                    if condition.HasErrors then
                        match condition.Condition with
                        | ParsedCondition.ParsedConditionError errors -> makeParseErrors errors
                        | _ -> failwith "Internal error"
                    else
                        parseSequentialOrChoiceInner file makeNode ((makeParagraphBreak(line.LineNumber), condition)::output) t
            | Sequential ->
                let (toSkip, result, condition) = findEndOfSeqOrChoice file makeParsedSeq line t
                match result.Node with
                | ParseErrors _ -> result
                | _ ->
                    let contLines = t |> List.skip toSkip
                    parseSequentialOrChoiceInner file makeNode ((result, condition)::output) contLines
            | Choice ->
                let (toSkip, result, condition) = findEndOfSeqOrChoice file makeParsedChoice line t
                match result.Node with
                | ParseErrors _ -> result
                | _ ->
                    let contLines = t |> List.skip toSkip
                    parseSequentialOrChoiceInner file makeNode ((result, condition)::output) contLines
            | _ ->
                // Try to find a condition.
                let conditionIndex = line.Tokens |> List.tryFindIndex (fun attToken -> attToken.Token = OpenBrace)
                if conditionIndex |> Option.isNone then
                    let sentenceNode = parseSimpleSequentialInner file line.LineNumber [] line.Tokens
                    match sentenceNode.SentenceNode with
                    | ParsedSentenceErrors errors -> makeParseErrors errors
                    | _ -> parseSequentialOrChoiceInner file makeNode (((makeParsedSentence line.LineNumber sentenceNode), unconditional)::output) t
                else
                    let conditionIndex = conditionIndex.Value
                    let condition = line.Tokens |> List.skip conditionIndex |> ConditionParser.parseCondition file line.LineNumber false
                    match condition.Condition with
                    | ParsedConditionError errors -> makeParseErrors errors
                    | _ ->
                        let sentenceNode = parseSimpleSequentialInner file line.LineNumber [] (line.Tokens |> List.take conditionIndex)
                        match sentenceNode.SentenceNode with
                        | ParsedSentenceErrors errors -> makeParseErrors errors
                        | _ -> parseSequentialOrChoiceInner file makeNode (((makeParsedSentence line.LineNumber sentenceNode), condition)::output) t
    and private findCloseCurly (s:AttributedTokenizedLine list) =
        s
        |> List.scan
            (fun (bracketCount, isCloseCurly) x ->
                // To determine if it's an open or close curly, you basically need to say that either the entire line is an open or close curly
                // plus optionally a condition, or that the entire line is @seq { or @choice {.
                if x.Tokens.Length = 1 then
                    if x.Tokens.[0].Token = OpenCurly then (bracketCount + 1, false)
                    else if x.Tokens.[0].Token = CloseCurly then (bracketCount - 1, true)
                    else (bracketCount, false)
                else if x.Tokens.[0].Token = CloseCurly then (bracketCount - 1, true)
                else if x.Tokens.[0].Token <> Choice && x.Tokens.[0].Token <> Sequential then (bracketCount, false)
                else if x.Tokens.[1].Token = OpenCurly then (bracketCount + 1, false)
                else (bracketCount, false))
            (1, false)
        |> List.skip 1
        |> List.tryFindIndex (fun (bracketCount, isCloseCurly) -> bracketCount = 0 && isCloseCurly)
    and private findEndOfSeqOrChoice file (makeNode:(ParsedNodeWithDependencies * ConditionParseResults)[] -> ParsedNodeWithDependencies) line remainingLines : int * ParsedNodeWithDependencies * ConditionParseResults =
        // Either the open brace is on this line or is the only thing on the next line.
        let lineTokens = line.Tokens
        if lineTokens.Length > 2 then
            (0, makeParseErrors([|(makeParseError file line.LineNumber lineTokens.[0].TokenStartLocation lineTokens.[2].TokenEndLocation "Invalid token")|]), unconditional)
        else if lineTokens.Length = 1 then
            match remainingLines with
            | [] -> (0, makeParseErrors([|(makeParseError file line.LineNumber lineTokens.[0].TokenStartLocation lineTokens.[0].TokenEndLocation "Missing {")|]), unconditional)
            | line2::t2 ->
                if line2.Tokens.Length <> 1 || line2.Tokens.[0].Token <> OpenCurly then
                    (0, makeParseErrors([|(makeParseError file line2.LineNumber line2.Tokens.[0].TokenStartLocation line2.Tokens.[0].TokenEndLocation "Invalid token")|]), unconditional)
                else
                    let closeCurlyIndex = findCloseCurly t2
                    if closeCurlyIndex |> Option.isNone then
                        (0, makeParseErrors([|(makeParseError file line2.LineNumber line2.Tokens.[0].TokenStartLocation line2.Tokens.[0].TokenEndLocation "Unmatched {")|]), unconditional)
                    else
                        let closeCurlyIndex = closeCurlyIndex.Value
                        let closeCurlyLine = t2 |> List.skip closeCurlyIndex |> List.head
                        if closeCurlyLine.Tokens.Length = 1 then
                            (closeCurlyIndex + 2, parseSequentialOrChoiceInner file makeNode [] (t2 |> List.take closeCurlyIndex), unconditional)
                        else if closeCurlyLine.Tokens.[1].Token <> OpenBrace then
                            let attToken = closeCurlyLine.Tokens.[1]
                            (closeCurlyIndex + 2, makeParseErrors([|(makeParseError file line.LineNumber attToken.TokenStartLocation attToken.TokenEndLocation "Invalid token")|]), unconditional)
                        else
                            let condition = ConditionParser.parseCondition file line.LineNumber false (closeCurlyLine.Tokens |> List.skip 1)
                            match condition.Condition with
                            | ParsedConditionError errors -> (closeCurlyIndex + 1, makeParseErrors errors, unconditional)
                            | _ -> (closeCurlyIndex + 2, parseSequentialOrChoiceInner file makeNode [] (t2 |> List.take closeCurlyIndex), condition)
        else if lineTokens.[1].Token <> OpenCurly then
            (0, makeParseErrors([|(makeParseError file line.LineNumber lineTokens.[1].TokenStartLocation lineTokens.[1].TokenEndLocation "Invalid token")|]), unconditional)
        else
            let closeCurlyIndex = findCloseCurly remainingLines
            if closeCurlyIndex |> Option.isNone then
                (0, makeParseErrors([|(makeParseError file line.LineNumber line.Tokens.[0].TokenStartLocation line.Tokens.[1].TokenEndLocation "Unmatched {")|]), unconditional)
            else
                let closeCurlyIndex = closeCurlyIndex.Value
                let closeCurlyLine = remainingLines |> List.skip closeCurlyIndex |> List.head
                if closeCurlyLine.Tokens.Length = 1 then
                    (closeCurlyIndex + 1, parseSequentialOrChoiceInner file makeNode [] (remainingLines |> List.take closeCurlyIndex), unconditional)
                else if closeCurlyLine.Tokens.[1].Token <> OpenBrace then
                    let attToken = closeCurlyLine.Tokens.[1]
                    (closeCurlyIndex + 1, makeParseErrors([|(makeParseError file line.LineNumber attToken.TokenStartLocation attToken.TokenEndLocation "Invalid token")|]), unconditional)
                else
                    let condition = ConditionParser.parseCondition file line.LineNumber false (closeCurlyLine.Tokens |> List.skip 1)
                    match condition.Condition with
                    | ParsedConditionError errors -> (closeCurlyIndex + 1, makeParseErrors errors, unconditional)
                    | _ -> (closeCurlyIndex + 1, parseSequentialOrChoiceInner file makeNode [] (remainingLines |> List.take closeCurlyIndex), condition)

    /// Parse the CategorizedAttributedTokenSet for a function definition into a tree.
    let parseFunctionDefinition (tokenSet:CategorizedAttributedTokenSet) : ParsedFunctionDefinition =
        let (name, hasErrors, tree) =
            if tokenSet.Tokens.Length = 0 then
                failwith "Internal error" // Cannot happen due to categorization
            else
                // First line is either "@func @funcName" or "@func @funcName {".
                let tokens = tokenSet.Tokens
                let fl = tokens.[0]
                let firstLine = fl.Tokens
                let l = firstLine.Length
                if l < 2 then ("<unknown>", true, (makeParseErrors [|(makeParseError tokenSet.File tokens.[0].LineNumber firstLine.[0].TokenStartLocation firstLine.[0].TokenEndLocation "No name given for function")|]))
                else if l > 3 then
                    let name = match firstLine.[1].Token with | FunctionName name -> name | _ -> "<unknown>"
                    (name, true, (makeParseErrors [|(makeParseError tokenSet.File tokens.[0].LineNumber firstLine.[0].TokenStartLocation firstLine.[0].TokenEndLocation "Function first line too long")|]))
                else
                    let name = match firstLine.[1].Token with | FunctionName name -> Choice1Of2 name | InvalidReservedToken value -> Choice2Of2 ("Reserved token: " + value) | _ -> Choice2Of2 "Unnamed function"
                    match name with
                    | Choice2Of2 errorText ->
                        ("<unknown>", true, (makeParseErrors [|(makeParseError tokenSet.File tokens.[0].LineNumber firstLine.[0].TokenStartLocation firstLine.[0].TokenEndLocation errorText)|]))
                    | Choice1Of2 name ->
                        if tokenSet.Tokens.[tokenSet.Tokens.Length - 1].Tokens.Length > 1 || tokenSet.Tokens.[tokenSet.Tokens.Length - 1].Tokens.[0].Token <> CloseCurly then
                            let line = tokenSet.Tokens.[tokenSet.Tokens.Length - 1]
                            let attToken = line.Tokens.[0]
                            (name, true, (makeParseErrors [|(makeParseError tokenSet.File line.LineNumber attToken.TokenStartLocation attToken.TokenEndLocation "Invalid token")|]))
                        else if l = 2 then
                            if tokenSet.Tokens.Length < 3 then
                                (name, true, (makeParseErrors [|(makeParseError tokenSet.File tokens.[0].LineNumber firstLine.[0].TokenStartLocation firstLine.[firstLine.Length - 1].TokenEndLocation "Insufficient brackets")|]))
                            else if tokenSet.Tokens.[1].Tokens.Length > 1 || tokenSet.Tokens.[1].Tokens.[0].Token <> OpenCurly then
                                (name, true, (makeParseErrors [|(makeParseError tokenSet.File tokens.[1].LineNumber tokens.[1].Tokens.[0].TokenStartLocation tokens.[1].Tokens.[0].TokenEndLocation "Invalid token")|]))
                            else
                                let tree = tokenSet.Tokens |> List.skip 2 |> List.take (tokenSet.Tokens.Length - 3) |> parseSequentialOrChoiceInner tokenSet.File makeParsedSeq []
                                let hasErrors = match tree.Node with | ParseErrors _ -> true | _ -> false
                                (name, hasErrors, tree)
                        else if firstLine.[2].Token <> OpenCurly then
                            (name, true, (makeParseErrors [|(makeParseError tokenSet.File fl.LineNumber firstLine.[2].TokenStartLocation firstLine.[2].TokenEndLocation "Invalid token")|]))
                        else
                            let tree = tokenSet.Tokens |> List.skip 1 |> List.take (tokenSet.Tokens.Length - 2) |> parseSequentialOrChoiceInner tokenSet.File makeParsedSeq []
                            let hasErrors = match tree.Node with | ParseErrors _ -> true | _ -> false
                            (name, hasErrors, tree)
        {   StartLine = tokenSet.StartLine
            EndLine = tokenSet.EndLine
            Index = tokenSet.Index
            HasErrors = hasErrors
            Name = name
            Dependencies = tree.Dependencies
            Tree = tree.Node }

