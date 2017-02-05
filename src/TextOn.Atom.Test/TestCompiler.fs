﻿module TextOn.Atom.Test.TestCompiler

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote
open FSharp.Quotations
open System.Collections.Generic

open TextOn.Atom

let funcText =
    "@var @free $SomeVar = \"Hello world\"

@func @main
{
  @choice {
    @seq {
      Lorem ipsum dolor sit amet, consectetur adipiscing elit.
      Cras ante lorem, {faucibus|maximus} vel mauris eget, luctus suscipit elit.
      Morbi lorem nibh, ultricies ac ligula gravida, pharetra posuere nibh.
      Curabitur eu mauris aliquam, mattis arcu vitae, semper lectus.
      Phasellus at elit ac sem dapibus dapibus.
      Morbi convallis varius $SomeVar.
      Curabitur scelerisque semper justo sit amet vehicula.
      Ut ut velit at ante viverra euismod.
      Nullam et pharetra libero, sit amet consequat nunc.
      Fusce ac sagittis libero.
      Duis vel mi a {liquet odio|ante viverra} blandit tincidunt.
      Integer a mi $SomeVar.
      Integer vitae ipsum non purus tincidunt semper.
    }
    @seq {
      Ut ornare pellentesque quam, consectetur congue augue ultricies nec.
      In gravida lacinia $SomeVar.
      Vivamus scelerisque blandit pulvinar.
      Praesent ullamcorper et ipsum et scelerisque.
      Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.
      Aliquam ac augue pharetra, placerat sem non, lobortis massa.
      Mauris eu mauris luctus, ullamcorper leo eget, scelerisque orci.
      Vivamus ipsum lacus, facilisis semper risus eu, placerat dapibus nulla.
      Sed finibus libero ipsum, sed vestibulum leo cursus sit amet.
      Aenean mollis condimentum nulla.
    }
  }
}"

let exampleFileName = "example.texton"

let expected =
    {
        Attributes = [||]
        Variables = [|{Name = "SomeVar";
                Index = 0;
                File = "example.texton";
                StartLine = 1;
                EndLine = 1;
                PermitsFreeValue = true;
                Text = "Hello world";
                Values = [||];}|];
         Definition =
          Seq
            [|(Choice
                 [|(Seq
                      [|(Sentence
                           ("example.texton", 7,
                            SimpleText
                              "Lorem ipsum dolor sit amet, consectetur adipiscing elit."),
                         True);
                        (Sentence
                           ("example.texton", 8,
                            SimpleSeq
                              [|SimpleText "Cras ante lorem, ";
                                SimpleChoice
                                  [|SimpleText "faucibus"; SimpleText "maximus"|];
                                SimpleText " vel mauris eget, luctus suscipit elit."|]),
                         True);
                        (Sentence
                           ("example.texton", 9,
                            SimpleText
                              "Morbi lorem nibh, ultricies ac ligula gravida, pharetra posuere nibh."),
                         True);
                        (Sentence
                           ("example.texton", 10,
                            SimpleText
                              "Curabitur eu mauris aliquam, mattis arcu vitae, semper lectus."),
                         True);
                        (Sentence
                           ("example.texton", 11,
                            SimpleText "Phasellus at elit ac sem dapibus dapibus."),
                         True);
                        (Sentence
                           ("example.texton", 12,
                            SimpleSeq
                              [|SimpleText "Morbi convallis varius "; VariableValue 0;
                                SimpleText "."|]), True);
                        (Sentence
                           ("example.texton", 13,
                            SimpleText
                              "Curabitur scelerisque semper justo sit amet vehicula."),
                         True);
                        (Sentence
                           ("example.texton", 14,
                            SimpleText "Ut ut velit at ante viverra euismod."), True);
                        (Sentence
                           ("example.texton", 15,
                            SimpleText
                              "Nullam et pharetra libero, sit amet consequat nunc."),
                         True);
                        (Sentence
                           ("example.texton", 16, SimpleText "Fusce ac sagittis libero."),
                         True);
                        (Sentence
                           ("example.texton", 17,
                            SimpleSeq
                              [|SimpleText "Duis vel mi a ";
                                SimpleChoice
                                  [|SimpleText "liquet odio"; SimpleText "ante viverra"|];
                                SimpleText " blandit tincidunt."|]), True);
                        (Sentence
                           ("example.texton", 18,
                            SimpleSeq
                              [|SimpleText "Integer a mi "; VariableValue 0;
                                SimpleText "."|]), True);
                        (Sentence
                           ("example.texton", 19,
                            SimpleText "Integer vitae ipsum non purus tincidunt semper."),
                         True)|], True);
                   (Seq
                      [|(Sentence
                           ("example.texton", 22,
                            SimpleText
                              "Ut ornare pellentesque quam, consectetur congue augue ultricies nec."),
                         True);
                        (Sentence
                           ("example.texton", 23,
                            SimpleSeq
                              [|SimpleText "In gravida lacinia "; VariableValue 0;
                                SimpleText "."|]), True);
                        (Sentence
                           ("example.texton", 24,
                            SimpleText "Vivamus scelerisque blandit pulvinar."), True);
                        (Sentence
                           ("example.texton", 25,
                            SimpleText "Praesent ullamcorper et ipsum et scelerisque."),
                         True);
                        (Sentence
                           ("example.texton", 26,
                            SimpleText
                              "Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus."),
                         True);
                        (Sentence
                           ("example.texton", 27,
                            SimpleText
                              "Aliquam ac augue pharetra, placerat sem non, lobortis massa."),
                         True);
                        (Sentence
                           ("example.texton", 28,
                            SimpleText
                              "Mauris eu mauris luctus, ullamcorper leo eget, scelerisque orci."),
                         True);
                        (Sentence
                           ("example.texton", 29,
                            SimpleText
                              "Vivamus ipsum lacus, facilisis semper risus eu, placerat dapibus nulla."),
                         True);
                        (Sentence
                           ("example.texton", 30,
                            SimpleText
                              "Sed finibus libero ipsum, sed vestibulum leo cursus sit amet."),
                         True);
                        (Sentence
                           ("example.texton", 31,
                            SimpleText "Aenean mollis condimentum nulla."), True)|],
                    True)|], True)|];}

[<Test>]
let ``End to end test``() =
    let funcLines = funcText.Split([|'\n'|], StringSplitOptions.RemoveEmptyEntries) |> List.ofArray
    let compiled =
        funcLines
        |> Preprocessor.preprocess (fun _ _ -> None) exampleFileName None
        |> CommentStripper.stripComments
        |> LineCategorizer.categorize
        |> List.map (Tokenizer.tokenize >> Parser.parse)
        |> List.toArray
        |> Compiler.compile
    test <@ compiled = CompilationSuccess expected @>

let compileLines (lines:string) =
    lines.Split([|'\n'|], StringSplitOptions.RemoveEmptyEntries)
    |> List.ofArray
    |> Preprocessor.preprocess (fun _ _ -> None) exampleFileName None
    |> CommentStripper.stripComments
    |> LineCategorizer.categorize
    |> List.map (Tokenizer.tokenize >> Parser.parse)
    |> List.toArray
    |> Compiler.compile

[<Test>]
let ``Declare the same variable twice``() =
    let lines =
        "@var @free $VarName = \"Variable one\"
@var @free $VarName = \"Variable one\"
@func @main {
  Hello world.
}"
    let result = lines |> compileLines
    let expected =
        CompilationFailure
            [|
                ParserError
                    {
                        File = exampleFileName
                        LineNumber = 2
                        StartLocation = 1
                        EndLocation = 4
                        ErrorText = "Duplicate definition of variable VarName"
                    }
            |]
    test <@ result = expected @>

[<Test>]
let ``Declare the same attribute twice``() =
    let lines =
        "@att %AttName
  {
    \"Hello\"
  }
@att %AttName
  {
    \"Hello\"
  }
@func @main {
  Hello world.
}"
    let result = lines |> compileLines
    let expected =
        CompilationFailure
            [|
                ParserError
                    {
                        File = exampleFileName
                        LineNumber = 5
                        StartLocation = 1
                        EndLocation = 4
                        ErrorText = "Duplicate definition of attribute AttName"
                    }
            |]
    test <@ result = expected @>

[<Test>]
let ``Declare the same function twice``() =
    let lines =
        "@func @FuncName
  {
    Hello
  }
@func @FuncName
  {
    Hello
  }
@func @main {
  Hello world.
}"
    let result = lines |> compileLines
    let expected =
        CompilationFailure
            [|
                ParserError
                    {
                        File = "example.texton"
                        LineNumber = 5
                        StartLocation = 1
                        EndLocation = 5
                        ErrorText = "Duplicate definition of function FuncName"
                    }
            |]
    test <@ result = expected @>