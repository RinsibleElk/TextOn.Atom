namespace TextOn.Atom

open System
open System.Text.RegularExpressions

type ParsedVariableName = string
type ParsedFunctionName = string

type ParsedSentenceNode =
    | ParsedStringValue of string
    | ParsedVariable of ParsedVariableName
    | ParsedSimpleChoice of ParsedSentenceNode[]
    | ParsedSimpleSeq of ParsedSentenceNode[]
    | ParsedSentenceErrors of ParseError[]

type ParsedNode =
    | ParsedSentence of ParsedSentenceNode * ParsedCondition
    | ParsedFunctionInvocation of ParsedFunctionName * ParsedCondition
    | ParsedSeq of ParsedNode[] * ParsedCondition
    | ParsedChoice of ParsedNode[] * ParsedCondition
    | ParsedParagraphBreak of ParsedCondition
    | ParseErrors of ParseError[]

type ParsedFunctionDefinition = {
    File : string
    StartLine : int
    EndLine : int
    Index : int
    HasErrors : bool
    Name : ParsedFunctionName
    Tree : ParsedNode }

[<RequireQualifiedAccess>]
module internal FunctionDefinitionParser =
    let private makeParseError line s e t =
        {   LineNumber = line
            StartLocation = s
            EndLocation = e
            ErrorText = t }
    let rec private listPartition indices li =
        match indices with
        | [] -> [li]
        | h::t -> [(li |> List.take h)]@(listPartition (t |> List.map (fun x -> x - h - 1)) (li |> List.skip (h + 1)))
    let rec private parseSequentialInner line output tokens =
        match tokens with
        | [] ->
            // Success!
            let l = output |> List.length
            if l = 0 then failwith "Cannot happen"
            else if l = 1 then output |> List.head
            else ParsedSimpleSeq (output |> List.rev |> Array.ofList)
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
                    ParsedSentenceErrors [|(makeParseError line h.TokenStartLocation ((t |> List.tryLast |> defaultArg <| h).TokenEndLocation) "Unmatched open curly")|]
                else
                    let parsedChoice = parseChoice line (t |> List.take endIndex.Value)
                    match parsedChoice with
                    | ParsedSentenceErrors errors -> parsedChoice
                    | _ -> parseSequentialInner line (parsedChoice::output) (t |> List.skip (endIndex.Value + 1))
            | RawText text -> parseSequentialInner line ((ParsedStringValue text)::output) t
            | VariableName name -> parseSequentialInner line ((ParsedVariable name)::output) t
            | _ -> ParsedSentenceErrors [|(makeParseError line h.TokenStartLocation h.TokenEndLocation "Invalid token")|]
    and private parseChoice line (tokens:AttributedToken list) =
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
        |> List.map (parseSequentialInner line [])
        |> Array.ofList
        |> fun a ->
            // If there are any errors, then concatenate the errors and return that.
            a
            |> Array.choose (function | ParsedSentenceErrors y -> Some y | _ -> None)
            |> Array.concat
            |> fun errors ->
                if errors.Length > 0 then
                    ParsedSentenceErrors errors
                else
                    ParsedSimpleChoice a

    /// Name of the function and closing bracket and stuff already dealt with.
    /// Each line is one of the following:
    /// - A function invocation (permits condition).
    /// - A break (permits condition).
    /// - The start of a seq.
    /// - The end of a seq (permits condition).
    /// - The start of a choice.
    /// - The end of a choice (permits condition).
    /// - Some text (permits condition).
    let rec private parseFunctionInner (tokens:AttributedTokenizedLine list) =
        
        failwith ""

    /// Parse the CategorizedAttributedTokenSet for a function definition into a tree.
    let parseFunction (tokenSet:CategorizedAttributedTokenSet) : ParsedFunctionDefinition =
        let (name, hasErrors, tree) =
            if tokenSet.Tokens.Length = 0 then
                failwith "Internal error" // Cannot happen due to categorization
            else
                // First line is either "@func @funcName" or "@func @funcName {".
                let tokens = tokenSet.Tokens
                let fl = tokens.[0]
                let firstLine = fl.Tokens
                let l = firstLine.Length
                if l < 2 then ("<unknown>", true, (ParseErrors [|(makeParseError tokens.[0].LineNumber firstLine.[0].TokenStartLocation firstLine.[0].TokenEndLocation "No name given for function")|]))
                else if l > 3 then
                    let name = match firstLine.[1].Token with | FunctionName name -> name | _ -> "<unknown>"
                    (name, true, (ParseErrors [|(makeParseError tokens.[0].LineNumber firstLine.[0].TokenStartLocation firstLine.[0].TokenEndLocation "Function first line too long")|]))
                else
                    let name = match firstLine.[1].Token with | FunctionName name -> Some name | _ -> None
                    if name.IsNone then
                        ("<unknown>", true, (ParseErrors [|(makeParseError tokens.[0].LineNumber firstLine.[0].TokenStartLocation firstLine.[0].TokenEndLocation "Unnamed function")|]))
                    else
                        let name = name.Value
                        if tokenSet.Tokens.[tokenSet.Tokens.Length - 1].Tokens.Length > 1 || tokenSet.Tokens.[tokenSet.Tokens.Length - 1].Tokens.[0].Token <> CloseCurly then
                            let line = tokenSet.Tokens.[tokenSet.Tokens.Length - 1]
                            let attToken = line.Tokens.[0]
                            (name, true, (ParseErrors [|(makeParseError line.LineNumber attToken.TokenStartLocation attToken.TokenEndLocation "Invalid token")|]))
                        else if l = 2 then
                            if tokenSet.Tokens.Length < 3 then
                                (name, true, (ParseErrors [|(makeParseError tokens.[0].LineNumber firstLine.[0].TokenStartLocation firstLine.[firstLine.Length - 1].TokenEndLocation "Insufficient brackets")|]))
                            else if tokenSet.Tokens.[1].Tokens.Length > 1 || tokenSet.Tokens.[1].Tokens.[0].Token <> OpenCurly then
                                (name, true, (ParseErrors [|(makeParseError tokens.[1].LineNumber tokens.[1].Tokens.[0].TokenStartLocation tokens.[1].Tokens.[0].TokenEndLocation "Invalid token")|]))
                            else
                                let tree = tokenSet.Tokens |> List.skip 2 |> List.take (tokenSet.Tokens.Length - 3) |> parseFunctionInner
                                let hasErrors = match tree with | ParseErrors _ -> true | _ -> false
                                (name, hasErrors, tree)
                        else if firstLine.[2].Token <> OpenCurly then
                            (name, true, (ParseErrors [|(makeParseError fl.LineNumber firstLine.[2].TokenStartLocation firstLine.[2].TokenEndLocation "Invalid token")|]))
                        else
                            let tree = tokenSet.Tokens |> List.skip 1 |> List.take (tokenSet.Tokens.Length - 2) |> parseFunctionInner
                            let hasErrors = match tree with | ParseErrors _ -> true | _ -> false
                            (name, hasErrors, tree)
        {   File = tokenSet.File
            StartLine = tokenSet.StartLine
            EndLine = tokenSet.EndLine
            Index = tokenSet.Index
            HasErrors = hasErrors
            Name = name
            Tree = tree }

