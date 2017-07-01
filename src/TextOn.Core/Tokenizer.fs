[<RequireQualifiedAccess>]
module internal TextOn.Core.Tokenizer

type private TokenizerState =
    | Done
    | Outside of int
    | Error of int * int * int * AttributedTokenizedLine list
    | Function of int * int * int * int * AttributedTokenizedLine list
    | VariableOrAttribute of Category * int * int * int * int * AttributedTokenizedLine list

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
let private makeImport fileName line tokens = makeTokenSet CategorizedImport fileName line line (tokens |> makeAttributedLine line |> List.singleton)

let private transitionToFunction line (tokens:_ list) =
    (Function (line, line, line + 1, (if tokens.[tokens.Length - 1].Token = OpenCurly then 1 else 0), tokens |> makeAttributedLine line |> List.singleton))
let private transitionToVariableOrAttribute category line (tokens:_ list) =
    (VariableOrAttribute (category, line, line, line + 1, (if tokens.[tokens.Length - 1].Token = OpenCurly then 1 else 0), tokens |> makeAttributedLine line |> List.singleton))
let private transitionToOutside line =
    Outside line

let private errorBlockText = "Unrecognised starting token"
let private incompleteFunctionText = "Incomplete function definition"
let private incompleteVarText = "Incomplete variable/attribute definition"

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
                | Func -> transitionToFunction line tokens, t, output
                | Var -> transitionToVariableOrAttribute CategorizedVarDefinition line tokens, t, output
                | Att -> transitionToVariableOrAttribute CategorizedAttDefinition line tokens, t, output
                | Import
                | Include -> Outside (line + 1), t, ((makeImport fileName line tokens)::output)
                | _ -> Error(line, line, line + 1, (tokens |> makeAttributedLine line |> List.singleton)), t, output
        | Error(startLine, endLine, currentLine, tokenizedLines), [] ->
            Done, [], ((makeError errorBlockText fileName startLine endLine tokenizedLines)::output)
        | Error(startLine, endLine, currentLine, tokenizedLines), h::t ->
            let tokens = OutsideLineTokenizer.tokenizeLine h
            if tokens.Length = 0 then
                Error(startLine, endLine, currentLine + 1, tokenizedLines), t, output
            else
                match tokens.[0].Token with
                | Func -> transitionToFunction currentLine tokens, t, ((makeError errorBlockText fileName startLine endLine tokenizedLines)::output)
                | Var -> transitionToVariableOrAttribute CategorizedVarDefinition currentLine tokens, t, ((makeError errorBlockText fileName startLine endLine tokenizedLines)::output)
                | Att -> transitionToVariableOrAttribute CategorizedAttDefinition currentLine tokens, t, ((makeError errorBlockText fileName startLine endLine tokenizedLines)::output)
                | Import
                | Include -> Outside (currentLine + 1), t, ((makeImport fileName currentLine tokens)::((makeError errorBlockText fileName startLine endLine tokenizedLines)::output))
                | _ -> Error(startLine, endLine, currentLine + 1, (tokens |> makeAttributedLine currentLine)::tokenizedLines), t, output
        | Function (startLine, endLine, currentLine, numBrackets, tokenizedLines), [] ->
            // Output the incomplete function as an error and transition to Done state
            Done, [], (makeError incompleteFunctionText fileName startLine endLine tokenizedLines)::output
        | VariableOrAttribute (category, startLine, endLine, currentLine, numBrackets, tokenizedLines), [] ->
            // Output the incomplete variable or attribute as an error and transition to Done state
            Done, [], (makeError incompleteVarText fileName startLine endLine tokenizedLines)::output
        | Function (startLine, endLine, currentLine, numBrackets, tokenizedLines), h::t ->
            // Tokenize the line as a function line.
            let tokens = FunctionLineTokenizer.tokenizeLine h
            if tokens.Length = 0 then
                Function(startLine, endLine, currentLine + 1, numBrackets, tokenizedLines), t, output
            else
                // First we detect if the user has moved onto something new but screwed up.
                match tokens.[0].Token with
                | Func -> transitionToFunction currentLine tokens, t, ((makeError incompleteFunctionText fileName startLine endLine tokenizedLines)::output)
                | Var -> transitionToVariableOrAttribute CategorizedVarDefinition currentLine tokens, t, ((makeError incompleteFunctionText fileName startLine endLine tokenizedLines)::output)
                | Att -> transitionToVariableOrAttribute CategorizedAttDefinition currentLine tokens, t, ((makeError incompleteFunctionText fileName startLine endLine tokenizedLines)::output)
                | Import
                | Include -> Outside (currentLine + 1), t, ((makeImport fileName currentLine tokens)::((makeError incompleteFunctionText fileName startLine endLine tokenizedLines)::output))
                // Has the bracket count changed?
                | CloseCurly ->
                    if numBrackets = 1 then
                        transitionToOutside (currentLine + 1), t, (makeFunction fileName startLine currentLine ((makeAttributedLine currentLine tokens)::tokenizedLines))::output
                    else
                        Function(startLine, currentLine, currentLine + 1, numBrackets - 1, (makeAttributedLine currentLine tokens)::tokenizedLines), t, output
                | _ ->
                    if tokens.[tokens.Length - 1].Token = OpenCurly then
                        Function(startLine, currentLine, currentLine + 1, numBrackets + 1, (makeAttributedLine currentLine tokens)::tokenizedLines), t, output
                    else
                        Function(startLine, currentLine, currentLine + 1, numBrackets, (makeAttributedLine currentLine tokens)::tokenizedLines), t, output
        | VariableOrAttribute (category, startLine, endLine, currentLine, numBrackets, tokenizedLines), h::t ->
            // Tokenize the line as a variable line.
            let tokens = VariableOrAttributeLineTokenizer.tokenizeLine h
            if tokens.Length = 0 then
                VariableOrAttribute(category, startLine, endLine, currentLine + 1, numBrackets, tokenizedLines), t, output
            else
                match tokens.[0].Token with
                | Func -> transitionToFunction currentLine tokens, t, ((makeError incompleteVarText fileName startLine endLine tokenizedLines)::output)
                | Var -> transitionToVariableOrAttribute CategorizedVarDefinition currentLine tokens, t, ((makeError incompleteVarText fileName startLine endLine tokenizedLines)::output)
                | Att -> transitionToVariableOrAttribute CategorizedAttDefinition currentLine tokens, t, ((makeError incompleteVarText fileName startLine endLine tokenizedLines)::output)
                | Import
                | Include -> Outside (currentLine + 1), t, ((makeImport fileName currentLine tokens)::((makeError incompleteVarText fileName startLine endLine tokenizedLines)::output))
                // Has the bracket count changed?
                | CloseCurly ->
                    if numBrackets = 1 then
                        transitionToOutside (currentLine + 1), t, (makeTokenSet category fileName startLine currentLine ((makeAttributedLine currentLine tokens)::tokenizedLines))::output
                    else
                        VariableOrAttribute(category, startLine, currentLine, currentLine + 1, numBrackets - 1, (makeAttributedLine currentLine tokens)::tokenizedLines), t, output
                | _ ->
                    if tokens.[tokens.Length - 1].Token = OpenCurly then
                        VariableOrAttribute(category, startLine, currentLine, currentLine + 1, numBrackets + 1, (makeAttributedLine currentLine tokens)::tokenizedLines), t, output
                    else
                        VariableOrAttribute(category, startLine, currentLine, currentLine + 1, numBrackets, (makeAttributedLine currentLine tokens)::tokenizedLines), t, output
    match newState with
    | Done ->
        newOutput |> List.rev
    | _ ->
        tokenizeInner fileName newState newLines newOutput

/// Tokenize a file.
let tokenize (fileName:string) (lines:string list) : CategorizedAttributedTokenSet list =
    tokenizeInner fileName (Outside 1) lines []
