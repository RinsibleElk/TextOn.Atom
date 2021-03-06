﻿namespace TextOn.Atom

open System
open System.IO

type InteractiveConfig =
    {
        [<ArgDescription("The template file to compile.")>]
        Template : FileInfo
        [<ArgDescription("Which line ending characters to use.")>]
        LineEnding : LineEnding option
        [<ArgDescription("How many spaces between sentences.")>]
        [<ArgRange(1,2)>]
        SentenceSpaces : int option
        [<ArgDescription("How many lines between paragraphs.")>]
        [<ArgRange(0,2)>]
        ParagraphLines : int option
        [<ArgDescription("If specified, print out the random seed that was used.")>]
        PrintRandomSeed : bool option
        [<ArgDescription("Specify the random seed for testing.")>]
        UseRandomSeed : int option
        [<ArgDescription("If specified, repeat with different inputs.")>]
        Continuous : bool option
        [<ArgDescription("The function to generate for. Default: \"main\".")>]
        Function : string option
    }

[<RequireQualifiedAccess>]
module internal RunInteractive =
    let run interactive =
        let file = interactive.Template
        let compilationResult =
            Preprocessor.preprocess Preprocessor.realFileResolver file.Name file.Directory.FullName (file.FullName |> File.ReadAllLines |> List.ofArray)
            |> CommentStripper.stripComments
            |> LineCategorizer.categorize
            |> List.map (Tokenizer.tokenize >> Parser.parse)
            |> Compiler.compile
        match compilationResult with
        | CompilationFailure errors ->
            errors
            |> Array.iter
                (function
                    | GeneralError error ->
                        eprintfn "%s in %s" error.ErrorText error.File
                    | ParserError error ->
                        eprintfn "%s at %s line %d (character %d)" error.ErrorText error.File error.LineNumber error.StartLocation)
            1
        | CompilationSuccess template ->
            let mutable repeat = true
            while repeat do
                repeat <- (interactive.Continuous |> defaultArg <| false)
                printfn ""
                let functionName = interactive.Function |> defaultArg <| "main"
                let functionDef = template.Functions |> Array.tryFind (fun f -> f.Name = functionName)
                if functionDef.IsNone then GeneratorError (sprintf "Function \"%s\" is not defined - nothing to generate" functionName)
                else
                    let attributeRequired = functionDef.Value.AttributeDependencies |> Set.ofArray |> fun s i -> s |> Set.contains i
                    let requiredAttributes =
                        template.Attributes
                        |> Array.filter (fun a -> a.Index |> attributeRequired)
                    let attributeValues =
                        requiredAttributes
                        |> Array.fold
                            (fun m att ->
                                let values =
                                    att.Values
                                    |> Array.filter (fun a -> ConditionEvaluator.resolve m a.Condition)
                                    |> Array.map (fun a -> a.Value)
                                if values |> Array.isEmpty then failwith "Invalid values for attributes"
                                else if values.Length = 1 then m |> Map.add att.Index values.[0]
                                else
                                    let possibilities = String.Join(", ", (values |> Array.truncate 5))
                                    printfn "[%s] Please enter a value. (Possible values: %s)" att.Name possibilities
                                    let mutable value = ""
                                    while (values |> Array.contains value |> not) do
                                        value <- Console.ReadLine()
                                    printfn ""
                                    m |> Map.add att.Index value)
                            Map.empty
                    let variableRequired = functionDef.Value.VariableDependencies |> Set.ofArray |> fun s i -> s |> Set.contains i
                    let requiredVariables =
                        template.Variables
                        |> Array.filter (fun a -> a.Index |> variableRequired)
                    let variableValues =
                        requiredVariables
                        |> Array.fold
                            (fun m att ->
                                let values =
                                    att.Values
                                    |> Array.filter (fun a -> VariableConditionEvaluator.resolve attributeValues m a.Condition)
                                    |> Array.map (fun a -> a.Value)
                                if (not att.PermitsFreeValue && values |> Array.isEmpty) then failwith "Invalid values for variables"
                                else if (not att.PermitsFreeValue && values.Length = 1) then m |> Map.add att.Index values.[0]
                                else
                                    if values.Length = 0 then
                                        printfn "[%s] %s" att.Name att.Text
                                    else
                                        let possibilities = String.Join(", ", (values |> Array.truncate 5))
                                        printfn "[%s] %s (Suggested values: %s)" att.Name att.Text possibilities
                                    let mutable value = ""
                                    let mutable validValue = false
                                    while (not validValue) do
                                        value <- Console.ReadLine()
                                        if att.PermitsFreeValue then validValue <- true
                                        else validValue <- (values |> Array.contains value)
                                    printfn ""
                                    m |> Map.add att.Index value)
                            Map.empty
                    let generatorInput = {
                        RandomSeed = (interactive.UseRandomSeed |> Option.map SpecificValue |> defaultArg <| NoSeed)
                        Config =
                            {   NumSpacesBetweenSentences = (interactive.SentenceSpaces |> defaultArg <| 2)
                                NumBlankLinesBetweenParagraphs = (interactive.ParagraphLines |> defaultArg <| 1)
                                LineEnding = (interactive.LineEnding |> defaultArg <| CRLF) }
                        Attributes  =
                            requiredAttributes
                            |> List.ofArray
                            |> List.map (fun att -> { Name = att.Name ; Value = attributeValues.[att.Index] })
                        Variables =
                            requiredVariables
                            |> List.ofArray
                            |> List.map (fun att -> { Name = att.Name ; Value = variableValues.[att.Index] })
                        Function = functionName }
                    Generator.generate generatorInput template
                |> function
                    | GeneratorSuccess output ->
                        if (interactive.PrintRandomSeed |> defaultArg <| false) then
                            output.LastSeed |> printfn "Random seed used was %d\n\n"
                        output.Text
                        |> Seq.map (fun t -> t.Value)
                        |> Seq.fold (+) ""
                    | GeneratorError error ->
                        sprintf "Generator Error: %s" error
                |> printfn "%s"
                printfn ""
            0 // return an integer exit code
