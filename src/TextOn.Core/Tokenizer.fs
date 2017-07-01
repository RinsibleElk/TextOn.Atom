[<RequireQualifiedAccess>]
module TextOn.Core.Tokenizer

type private TokenizerState =
    | Done
    | Outside of int
    | Function of int * int * int * int * AttributedTokenizedLine list

let private makeAttributedLine line tokens =
    {
        LineNumber = line
        Tokens = tokens
    }

let private makeTokenSet category fileName startLine endLine tokens =
    {
        Category = category
        File = fileName
        StartLine = startLine
        EndLine = endLine
        Tokens = tokens |> List.rev
    }

let private makeError error = makeTokenSet (CategorizationError error)
let private makeFunction = makeTokenSet CategorizedFuncDefinition

let private transitionToFunction line (tokens:_ list) =
    (Function (line, line, line + 1, (if tokens.[tokens.Length - 1].Token = OpenCurly then 1 else 0), tokens |> makeAttributedLine line |> List.singleton))
let private transitionToOutside line =
    Outside line

let rec private tokenizeInner fileName state lines output =
    let newState, newLines, newOutput =
        match state, lines with
        | Done, _ -> failwith "Invalid state reached - Done"
        | Outside _, [] -> Done, [], output
        | Outside line, h::t ->
            let tokens = OutsideLineTokenizer.tokenizeLine h
            if tokens.Length = 0 then
                Outside (line + 1), t, output
            else
                match tokens.[0].Token with
                | Func ->
                    transitionToFunction line tokens, t, output
                | _ ->
                    // TODO
                    Done, [], output
        | Function (startLine, endLine, currentLine, numBrackets, tokenizedLines), [] ->
            // Output the incomplete function as an error and transition to Done state
            Done, [], (makeError "Incomplete function" fileName startLine endLine tokenizedLines)::output
        | Function (startLine, endLine, currentLine, numBrackets, tokenizedLines), h::t ->
            // Tokenize the line as a function line.
            let tokens = FunctionLineTokenizer.tokenizeLine h
            if tokens.Length = 0 then
                Function(startLine, endLine, currentLine + 1, numBrackets, tokenizedLines), t, output
            else
                // TODO First we detect if the user has moved onto something new but screwed up.

                // Has the bracket count changed?
                if tokens.[0].Token = CloseCurly then
                    if numBrackets = 1 then
                        transitionToOutside (currentLine + 1), t, (makeFunction fileName startLine currentLine ((makeAttributedLine currentLine tokens)::tokenizedLines))::output
                    else
                        Function(startLine, currentLine, currentLine + 1, numBrackets - 1, (makeAttributedLine currentLine tokens)::tokenizedLines), t, output
                else if tokens.[tokens.Length - 1].Token = OpenCurly then
                    Function(startLine, currentLine, currentLine + 1, numBrackets + 1, (makeAttributedLine currentLine tokens)::tokenizedLines), t, output
                else
                    Function(startLine, currentLine, currentLine + 1, numBrackets, (makeAttributedLine currentLine tokens)::tokenizedLines), t, output
    if newState = Done then
        newOutput |> List.rev
    else
        tokenizeInner fileName newState newLines newOutput

/// Tokenize a file.
let internal tokenize (fileName:string) (lines:string list) : CategorizedAttributedTokenSet list =
    tokenizeInner fileName (Outside 1) lines []
