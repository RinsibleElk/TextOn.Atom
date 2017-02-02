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
    let rec private parseSequentialInner line tokens =
        failwith ""
    and private parseSentenceInner line (tokens:AttributedToken list) =
        match tokens with
        | [] -> ParsedStringValue ""
        | [s] ->
            match s.Token with
            | RawText text -> ParsedStringValue text
            | VariableName name -> ParsedVariable name
            | _ -> ParsedSentenceErrors [|(makeParseError line s.TokenStartLocation s.TokenEndLocation "Invalid token")|]
        | h::t ->
            // At this point it's either a sequential or a choice.
            if h.Token = OpenCurly then
                // Find the matching close curly.

                failwith ""
            else
                failwith ""
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
        |> List.map (parseSentenceInner line)
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

    let rec private parseFunctionLine (tokens:AttributedToken list) =
        failwith ""

    let rec private parseFunctionInner (tokens:AttributedTokenizedLine[]) =
        ()

    /// Parse the CategorizedAttributedTokenSet for a function definition into a tree.
    let parseFunction (tokenSet:CategorizedAttributedTokenSet) : ParsedFunctionDefinition =
        // First line(s) should contain @func @funcName {
        // Last line should just be }
        let (name, hasErrors, tree) =
            if tokenSet.Tokens.Length = 0 then
                failwith ""
            else
                failwith ""
        {   File = tokenSet.File
            StartLine = tokenSet.StartLine
            EndLine = tokenSet.EndLine
            Index = tokenSet.Index
            HasErrors = hasErrors
            Name = name
            Tree = tree }

