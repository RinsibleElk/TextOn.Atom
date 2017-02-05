namespace TextOn.Atom

open System

/// Store a mapping.
type StringNameValuePair = {
    /// The name. Case sensitive.
    Name : string
    /// The value. Case sensitive.
    Value : string }

/// What line ending character(s) to use.
type LineEnding =
    /// Windows-style (\r\n).
    | CRLF
    /// UNIX-style (\n).
    | LF

/// Configuration for things like line endings.
type GeneratorConfig = {
    /// Number of spaces between sentences.
    NumSpacesBetweenSentences : int
    /// Number of blank lines between paragraphs.
    NumBlankLinesBetweenParagraphs : int
    /// What line ending character(s) to use.
    LineEnding : LineEnding }

/// Used to configure the generator for a run with a given random seed.
type RandomSeed =
    /// Create a new random seed for this run.
    | NoSeed
    /// Use a specific value for the seed.
    | SpecificValue of int

/// JSON input to generation.
type GeneratorInput = {
    /// The random seed to use.
    RandomSeed : RandomSeed
    /// Generator configuration.
    Config : GeneratorConfig
    /// Attribute values.
    Attributes : StringNameValuePair list
    /// Variable values.
    Variables : StringNameValuePair list }

type AttributedOutputString = {
    InputFile : string
    InputLineNumber : int
    Value : string }

type AttributedOutputText =
    | SentenceText of (string * int * string)
    | ParaBreak of (string * int)

/// JSON output from generator.
type GeneratorOutput = {
    LastSeed : int
    Text : AttributedOutputString[] }

type GeneratorResult =
    | GeneratorError of string
    | GeneratorSuccess of GeneratorOutput

[<RequireQualifiedAccess>]
module Generator =
    let rec private generateSentence (variableValues:Map<int,string>) (random:Random) (node:SimpleCompiledDefinitionNode) =
        match node with
        | VariableValue(index) -> variableValues.[index]
        | SimpleChoice(nodes) ->
            generateSentence variableValues random nodes.[(random.Next(nodes.Length))]
        | SimpleSeq(nodes) ->
            nodes
            |> Array.map (generateSentence variableValues random)
            |> Array.fold (+) ""
        | SimpleText(s) ->
            printfn "Text: %s" s
            s
    let rec private generateInner attributeValues variableValues (random:Random) (node:CompiledDefinitionNode) =
        match node with
        | Sentence(inputFile, inputLineNumber, s) ->
            printfn "Sentence: %s %d" inputFile inputLineNumber
            Seq.singleton (SentenceText (inputFile, inputLineNumber, (generateSentence variableValues random s)))
        | ParagraphBreak(inputfile, inputLineNumber) -> Seq.singleton (ParaBreak(inputfile, inputLineNumber))
        | Choice(s) ->
            printfn "Choice"
            let options =
                s
                |> Array.choose
                    (fun (node, condition) ->
                        if ConditionEvaluator.resolve attributeValues condition then Some node
                        else None)
            if options.Length = 0 then failwith "Internal error"
            let index = random.Next(options.Length)
            generateInner attributeValues variableValues random options.[index]
        | Seq(s) ->
            printfn "Seq"
            s
            |> Array.filter (fun (node, condition) -> ConditionEvaluator.resolve attributeValues condition)
            |> Seq.collect
                (fun (node, _) ->
                    let random = Random(random.Next())
                    generateInner attributeValues variableValues random node)
    let generate (input:GeneratorInput) (compiledTemplate:CompiledTemplate) : GeneratorResult =
        let randomSeed =
            match input.RandomSeed with
            | SpecificValue seed -> seed
            | NoSeed -> Random().Next()
        let random = Random(randomSeed)
        let attributeValues, attributeError =
            // Need to do a check that the user has provided values for all attribute values.
            let requiredAttributes = compiledTemplate.Attributes |> Array.map (fun x -> x.Name) |> Set.ofArray
            let givenAttributes = input.Attributes |> List.map (fun x -> x.Name) |> Set.ofList
            let extraAttributes = Set.difference givenAttributes requiredAttributes
            let missingAttributes = Set.difference requiredAttributes givenAttributes
            if extraAttributes |> Set.isEmpty |> not then
                (Map.empty, Some (sprintf "Unrecognised attribute: %s" (extraAttributes |> Seq.head)))
            else if missingAttributes |> Set.isEmpty |> not then
                (Map.empty, Some (sprintf "Undefined attribute: %s" (missingAttributes |> Seq.head)))
            else
                let attributeIndices =
                    compiledTemplate.Attributes
                    |> Array.map (fun x -> x.Name, x.Index)
                    |> Map.ofArray
                let attributeValues =
                    input.Attributes
                    |> List.map
                        (fun x -> (attributeIndices |> Map.find x.Name), x.Value)
                    |> Map.ofSeq
                (attributeValues, None)
        let variableValues, variableError =
            // Need to do a check that the user has provided values for all variable values.
            let requiredVariables = compiledTemplate.Variables |> Array.map (fun x -> x.Name) |> Set.ofArray
            let givenVariables = input.Variables |> List.map (fun x -> x.Name) |> Set.ofList
            let extraVariables = Set.difference givenVariables requiredVariables
            let missingVariables = Set.difference requiredVariables givenVariables
            if extraVariables |> Set.isEmpty |> not then
                (Map.empty, Some (sprintf "Unrecognised variable: %s" (extraVariables |> Seq.head)))
            else if missingVariables |> Set.isEmpty |> not then
                (Map.empty, Some (sprintf "Undefined variable: %s" (missingVariables |> Seq.head)))
            else
                let variableIndices =
                    compiledTemplate.Variables
                    |> Array.map (fun x -> x.Name, x.Index)
                    |> Map.ofArray
                let variableValues =
                    input.Variables
                    |> List.map
                        (fun x -> (variableIndices |> Map.find x.Name), x.Value)
                    |> Map.ofSeq
                (variableValues, None)
        match (attributeError, variableError) with
        | (Some e, _)
        | (_, Some e) -> GeneratorError e
        | _ ->
            printfn "At the generate bit"
            let output = generateInner attributeValues variableValues random compiledTemplate.Definition |> Seq.toList
            let sentenceBreakText = [ 1 .. input.Config.NumSpacesBetweenSentences ] |> Seq.map (fun _ -> " ") |> Seq.fold (+) ""
            let lineBreakText = match input.Config.LineEnding with | CRLF -> "\r\n" | _ -> "\n"
            let paraBreakText = [ 0 .. input.Config.NumBlankLinesBetweenParagraphs ] |> Seq.map (fun _ -> lineBreakText) |> Seq.fold (+) ""
            let rec loop previousIsSentence remaining =
                match remaining with
                | [] -> Seq.empty
                | h::t ->
                    let (newPreviousIsSentence, output) =
                        match h with
                        | SentenceText(inputFile, inputLineNumber, text) ->
                            let l =
                                if previousIsSentence then
                                    seq [
                                        {   InputFile = null
                                            InputLineNumber = Int32.MinValue
                                            Value = sentenceBreakText }
                                        {
                                            InputFile = inputFile
                                            InputLineNumber = inputLineNumber
                                            Value = text }
                                    ]
                                else
                                    Seq.singleton
                                        {
                                            InputFile = inputFile
                                            InputLineNumber = inputLineNumber
                                            Value = text }
                            (true, l)
                        | ParaBreak(inputFile, inputLineNumber) ->
                            (false,
                                Seq.singleton
                                    {   InputFile = inputFile
                                        InputLineNumber = inputLineNumber
                                        Value = paraBreakText })
                    seq {
                        yield! output
                        yield! loop newPreviousIsSentence t }
            GeneratorSuccess
                {   LastSeed    = randomSeed
                    Text        = loop false output |> Seq.toArray }
