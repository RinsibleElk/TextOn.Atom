module TextOn.Atom.Test.TestParser

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote
open FSharp.Quotations
open System.Collections.Generic
open System.IO
open TextOn.Atom

let exampleFileName = "example.texton"
let exampleDirectory = @"D:\Example"

[<Test>]
let ``Test choice parsing``() =
    let result =
        @"@func @stycke20160823_6
{
    @choice {
    Oavsett hur du {planerar|lägger} upp din {vistelse|resa} har vi garanterat
    en hyrbil i $MÄRKE för dig.
    Alla resor ser olika ut. Oavsett om du är här {för en längre eller kortare tid|för en längre eller kortare period|för en längre eller kortare tidsperiod|för en lång eller kort tid|för en längre eller kortare tid|för en lång eller kort tidsperiod} så har vi en {modell|bilmodell|hyrbilsslösning} för dig.
    Vi har en bilmodell för alla {slags|typer av} resor. Oavsett om du ska stanna {för en längre eller kortare tid|för en längre eller kortare period|för en längre eller kortare tidsperiod|för en lång eller kort tid|för en längre eller kortare tid|för en lång eller kort tidsperiod} {tar vi fram|hittar vi|finner vi} en lösning för dig.
    Vi på Sixt kan ta fram en lösning oavsett om du behöver en hyrbil i $MÄRKE för
    {för en längre eller kortare tid|för en längre eller kortare period|för en längre eller kortare tidsperiod|för en lång eller kort tid|för en längre eller kortare tid|för en lång eller kort tidsperiod}.
    }
}"
        |> fun s -> s.Split([|'\r';'\n'|], StringSplitOptions.RemoveEmptyEntries)
        |> List.ofArray
        |> Preprocessor.preprocess (fun _ _ -> None) exampleFileName exampleDirectory
        |> CommentStripper.stripComments
        |> LineCategorizer.categorize
        |> List.head
        |> Tokenizer.tokenize
        |> Parser.parse
    let expected =
        {   File = Path.Combine(exampleDirectory, exampleFileName)
            Result =
                ParsedFunction
                    {   StartLine = 1
                        EndLine = 11
                        Index = 0
                        HasErrors = false
                        Name = "stycke20160823_6"
                        Dependencies = [|ParsedVariableName "MÄRKE"|]
                        Tree =
                            ParsedSeq
                                [|
                                    (ParsedChoice
                                        [|
                                            (ParsedSentence
                                                (   4,
                                                    ParsedSimpleSeq
                                                        [|
                                                            ParsedStringValue "Oavsett hur du "
                                                            ParsedSimpleChoice [|ParsedStringValue "planerar";ParsedStringValue "lägger"|]
                                                            ParsedStringValue " upp din "
                                                            ParsedSimpleChoice [|ParsedStringValue "vistelse";ParsedStringValue "resa"|]
                                                            ParsedStringValue " har vi garanterat"
                                                        |]
                                                ),
                                                ParsedUnconditional)
                                            (ParsedSentence
                                                (   5,
                                                    ParsedSimpleSeq [|ParsedStringValue "en hyrbil i ";ParsedSimpleVariable (17, 22, "MÄRKE");ParsedStringValue " för dig."|]),
                                                    ParsedUnconditional)
                                            (ParsedSentence
                                                (   6,
                                                    ParsedSimpleSeq
                                                        [|
                                                            ParsedStringValue "Alla resor ser olika ut. Oavsett om du är här "
                                                            ParsedSimpleChoice
                                                                [|
                                                                    ParsedStringValue "för en längre eller kortare tid"
                                                                    ParsedStringValue "för en längre eller kortare period"
                                                                    ParsedStringValue "för en längre eller kortare tidsperiod"
                                                                    ParsedStringValue "för en lång eller kort tid"
                                                                    ParsedStringValue "för en längre eller kortare tid"
                                                                    ParsedStringValue "för en lång eller kort tidsperiod"
                                                                |]
                                                            ParsedStringValue " så har vi en "
                                                            ParsedSimpleChoice [|ParsedStringValue "modell";ParsedStringValue "bilmodell";ParsedStringValue "hyrbilsslösning"|]
                                                            ParsedStringValue " för dig."
                                                        |]),
                                                    ParsedUnconditional)
                                            (ParsedSentence
                                                (   7,
                                                    ParsedSimpleSeq
                                                        [|
                                                            ParsedStringValue "Vi har en bilmodell för alla "
                                                            ParsedSimpleChoice [|ParsedStringValue "slags";ParsedStringValue "typer av"|]
                                                            ParsedStringValue " resor. Oavsett om du ska stanna "
                                                            ParsedSimpleChoice
                                                                [|
                                                                    ParsedStringValue "för en längre eller kortare tid";
                                                                    ParsedStringValue "för en längre eller kortare period";
                                                                    ParsedStringValue "för en längre eller kortare tidsperiod";
                                                                    ParsedStringValue "för en lång eller kort tid";
                                                                    ParsedStringValue "för en längre eller kortare tid";
                                                                    ParsedStringValue "för en lång eller kort tidsperiod"
                                                                |]
                                                            ParsedStringValue " "
                                                            ParsedSimpleChoice [|ParsedStringValue "tar vi fram";ParsedStringValue "hittar vi";ParsedStringValue "finner vi"|]
                                                            ParsedStringValue " en lösning för dig."
                                                        |]),
                                                    ParsedUnconditional)
                                            (ParsedSentence
                                                (   8,
                                                    ParsedSimpleSeq
                                                        [|
                                                            ParsedStringValue "Vi på Sixt kan ta fram en lösning oavsett om du behöver en hyrbil i ";
                                                            ParsedSimpleVariable (73, 78, "MÄRKE");
                                                            ParsedStringValue " för"
                                                        |]),
                                                    ParsedUnconditional)
                                            (ParsedSentence
                                                (   9,
                                                    ParsedSimpleSeq
                                                        [|
                                                            ParsedSimpleChoice
                                                                [|
                                                                    ParsedStringValue "för en längre eller kortare tid";
                                                                    ParsedStringValue "för en längre eller kortare period";
                                                                    ParsedStringValue "för en längre eller kortare tidsperiod";
                                                                    ParsedStringValue "för en lång eller kort tid";
                                                                    ParsedStringValue "för en längre eller kortare tid";
                                                                    ParsedStringValue "för en lång eller kort tidsperiod"
                                                                |]
                                                            ParsedStringValue "."
                                                        |]),
                                                    ParsedUnconditional)
                                        |], ParsedUnconditional)
                                |] } }
    test <@ result = expected @>
