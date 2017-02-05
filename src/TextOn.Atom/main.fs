namespace TextOn.Atom

open System
open System.IO

type Template =
    {
        [<ArgDescription("The template file to compile for")>]
        Template : string
    }
type Mode =
    | Template of Template

module internal Main =
    [<EntryPoint>]
    let main argv =
        let mode : Mode option = ArgParser.parse argv
        if (mode |> Option.isNone) then 0
        else
            match mode.Value with
            | Template (template) ->
                let file = template.Template
                if file |> File.Exists |> not then
                    failwithf "File not found: %s" file
                else
                    let f = FileInfo file
                    let compilationResult =
                        Preprocessor.preprocess Preprocessor.realFileResolver f.FullName (Some f.Directory.FullName) (f.FullName |> File.ReadAllLines |> List.ofArray)
                        |> CommentStripper.stripComments
                        |> LineCategorizer.categorize
                        |> List.map (Tokenizer.tokenize >> Parser.parse)
                        |> List.toArray
                        |> Compiler.compile
                    match compilationResult with
                    | CompilationFailure errors ->
                        errors
                        |> Array.iter
                            (function
                                | GeneralError error ->
                                    eprintfn "%s" error
                                | ParserError error ->
                                    eprintfn "%s at %s line %d (character %d)" error.ErrorText error.File error.LineNumber error.StartLocation)
                        1
                    | CompilationSuccess template ->
                        printfn ""
                        let attributeValues =
                            template.Attributes
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
                        let variableValues =
                            template.Variables
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
                            RandomSeed = NoSeed
                            Config =
                                {   NumSpacesBetweenSentences = 2
                                    NumBlankLinesBetweenParagraphs = 1
                                    LineEnding = CRLF }
                            Attributes  =
                                template.Attributes
                                |> List.ofArray
                                |> List.map (fun att -> { Name = att.Name ; Value = attributeValues.[att.Index] })
                            Variables =
                                template.Variables
                                |> List.ofArray
                                |> List.map (fun att -> { Name = att.Name ; Value = variableValues.[att.Index] }) }
                        Generator.generate generatorInput template
                        |> function | GeneratorSuccess output -> output.Text | _ -> failwith ""
                        |> Seq.map (fun t -> t.Value)
                        |> Seq.fold (+) ""
                        |> printfn "%s"
                        printfn ""
                        0 // return an integer exit code
