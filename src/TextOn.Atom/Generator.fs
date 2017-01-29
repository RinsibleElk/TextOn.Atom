﻿namespace TextOn.Atom

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
        | SimpleText(s) -> s
    let rec private generateInner attributeValues variableValues (random:Random) (node:CompiledDefinitionNode) =
        match node with
        | Sentence(inputFile, inputLineNumber, s) -> Seq.singleton (SentenceText (inputFile, inputLineNumber, (generateSentence variableValues random s)))
        | ParagraphBreak(inputfile, inputLineNumber) -> Seq.singleton (ParaBreak(inputfile, inputLineNumber))
        | Choice(s) ->
            let options =
                s
                |> Array.choose
                    (fun (node, condition) ->
                        if ConditionEvaluator.resolve attributeValues condition then Some node
                        else None)
            if options.Length = 0 then failwith "This cannot happen"
            let index = random.Next(options.Length)
            generateInner attributeValues variableValues random options.[index]
        | Seq(s) ->
            s
            |> Array.filter (fun (node, condition) -> ConditionEvaluator.resolve attributeValues condition)
            |> Array.map
                (fun (node, _) ->
                    let random = Random(random.Next())
                    async {
                        return (generateInner attributeValues variableValues random node) })
            |> Async.Parallel
            |> Async.RunSynchronously
            |> Seq.concat
    let generate (input:GeneratorInput) (compiledTemplate:CompiledTemplate) : GeneratorOutput =
        let randomSeed =
            match input.RandomSeed with
            | SpecificValue seed -> seed
            | NoSeed -> Random().Next()
        let random = Random(randomSeed)
        let attributeValues = Map.empty
        let variableValues = Map.empty
        let output = generateInner attributeValues variableValues random compiledTemplate.Definition |> Seq.toArray
        let sentenceBreakText = [ 1 .. input.Config.NumSpacesBetweenSentences ] |> Seq.map (fun _ -> " ") |> Seq.fold (+) ""
        let lineBreakText = match input.Config.LineEnding with | CRLF -> "\r\n" | _ -> "\n"
        let paraBreakText = [ 0 .. input.Config.NumBlankLinesBetweenParagraphs ] |> Seq.map (fun _ -> lineBreakText) |> Seq.fold (+) ""
        let rec loop previousIsSentence remaining =
            if remaining |> Seq.isEmpty then Seq.empty
            else
                let (newPreviousIsSentence, output) =
                    match (remaining |> Seq.head) with
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
                    yield! loop newPreviousIsSentence (remaining |> Seq.tail) }
        {   LastSeed    = randomSeed
            Text        = loop false output |> Seq.toArray }
