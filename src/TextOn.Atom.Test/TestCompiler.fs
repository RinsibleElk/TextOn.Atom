module TextOn.Atom.Test.TestCompiler

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote
open FSharp.Quotations
open System.Collections.Generic
open System.IO
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
let exampleDirectory = @"D:\Example"
let fullExampleFile = Path.Combine(exampleDirectory, exampleFileName)

let expected =
    {Attributes = [||];
     Variables = [|{Name = "SomeVar"
                    Index = 0
                    File = Path.Combine(exampleDirectory, exampleFileName)
                    StartLine = 1
                    EndLine = 1
                    PermitsFreeValue = true
                    Text = "Hello world"
                    Values = [||] }|]
     Functions =
      [|{Name = "main";
         Index = 1;
         File = Path.Combine(exampleDirectory, exampleFileName);
         StartLine = 2
         EndLine = 33
         AttributeDependencies = [||]
         VariableDependencies = [|0|]
         Tree =
          Seq
            [|(Choice
                 [|(Seq
                      [|(Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 6,
                            SimpleText
                              "Lorem ipsum dolor sit amet, consectetur adipiscing elit."),
                         True);
                        (Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 7,
                            SimpleSeq
                              [|SimpleText "Cras ante lorem, ";
                                SimpleChoice
                                  [|SimpleText "faucibus";
                                    SimpleText "maximus"|];
                                SimpleText
                                  " vel mauris eget, luctus suscipit elit."|]),
                         True);
                        (Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 8,
                            SimpleText
                              "Morbi lorem nibh, ultricies ac ligula gravida, pharetra posuere nibh."),
                         True);
                        (Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 9,
                            SimpleText
                              "Curabitur eu mauris aliquam, mattis arcu vitae, semper lectus."),
                         True);
                        (Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 10,
                            SimpleText
                              "Phasellus at elit ac sem dapibus dapibus."),
                         True);
                        (Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 11,
                            SimpleSeq
                              [|SimpleText "Morbi convallis varius ";
                                VariableValue 0; SimpleText "."|]), True);
                        (Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 12,
                            SimpleText
                              "Curabitur scelerisque semper justo sit amet vehicula."),
                         True);
                        (Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 13,
                            SimpleText "Ut ut velit at ante viverra euismod."),
                         True);
                        (Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 14,
                            SimpleText
                              "Nullam et pharetra libero, sit amet consequat nunc."),
                         True);
                        (Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 15,
                            SimpleText "Fusce ac sagittis libero."), True);
                        (Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 16,
                            SimpleSeq
                              [|SimpleText "Duis vel mi a ";
                                SimpleChoice
                                  [|SimpleText "liquet odio";
                                    SimpleText "ante viverra"|];
                                SimpleText " blandit tincidunt."|]), True);
                        (Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 17,
                            SimpleSeq
                              [|SimpleText "Integer a mi "; VariableValue 0;
                                SimpleText "."|]), True);
                        (Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 18,
                            SimpleText
                              "Integer vitae ipsum non purus tincidunt semper."),
                         True)|], True);
                   (Seq
                      [|(Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 21,
                            SimpleText
                              "Ut ornare pellentesque quam, consectetur congue augue ultricies nec."),
                         True);
                        (Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 22,
                            SimpleSeq
                              [|SimpleText "In gravida lacinia ";
                                VariableValue 0; SimpleText "."|]), True);
                        (Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 23,
                            SimpleText "Vivamus scelerisque blandit pulvinar."),
                         True);
                        (Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 24,
                            SimpleText
                              "Praesent ullamcorper et ipsum et scelerisque."),
                         True);
                        (Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 25,
                            SimpleText
                              "Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus."),
                         True);
                        (Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 26,
                            SimpleText
                              "Aliquam ac augue pharetra, placerat sem non, lobortis massa."),
                         True);
                        (Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 27,
                            SimpleText
                              "Mauris eu mauris luctus, ullamcorper leo eget, scelerisque orci."),
                         True);
                        (Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 28,
                            SimpleText
                              "Vivamus ipsum lacus, facilisis semper risus eu, placerat dapibus nulla."),
                         True);
                        (Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 29,
                            SimpleText
                              "Sed finibus libero ipsum, sed vestibulum leo cursus sit amet."),
                         True);
                        (Sentence
                           (Path.Combine(exampleDirectory, exampleFileName), 30,
                            SimpleText "Aenean mollis condimentum nulla."),
                         True)|], True)|], True)|];}|];}

let rec compareTemplates t e =
    match (t,e) with
    | (Sentence(ts,ti,tn), Sentence(es,ei,en)) ->
        test <@ ts = es @>
        test <@ ti = ti @>
        test <@ tn = en @>
    | (ParagraphBreak(ts,ti), ParagraphBreak(es,ei)) ->
        test <@ ts = es @>
        test <@ ti = ti @>
    | (Choice(tn), Choice(en)) ->
        if tn.Length <> en.Length then failwithf "Different lengths in choices %A <> %A" tn en
        else
            Array.zip tn en
            |> Array.iter
                (fun ((tn,tc),(en,ec)) ->
                    test <@ tc = ec @>
                    compareTemplates tn en)
    | (Seq(tn), Seq(en)) ->
        if tn.Length <> en.Length then failwithf "Different lengths in seqs %A <> %A" tn en
        else
            Array.zip tn en
            |> Array.iter
                (fun ((tn,tc),(en,ec)) ->
                    test <@ tc = ec @>
                    compareTemplates tn en)
    | (Function(ti), Function(ei)) ->
        test <@ ti = ei @>
    | _ -> failwithf "Got different elements %A <> %A" t e

[<Test>]
let ``End to end test``() =
    let funcLines = funcText.Split([|'\r';'\n'|], StringSplitOptions.RemoveEmptyEntries) |> List.ofArray
    let compiled =
        funcLines
        |> Preprocessor.preprocess (fun _ _ -> None) exampleFileName exampleDirectory
        |> CommentStripper.stripComments
        |> LineCategorizer.categorize
        |> List.map (Tokenizer.tokenize >> Parser.parse)
        |> Compiler.compile
    match compiled with
    | CompilationFailure errors -> failwithf "Got errors during compilation %A" errors
    | CompilationSuccess template ->
        test <@ template.Attributes = expected.Attributes @>
        test <@ template.Variables = expected.Variables @>
        if template.Functions.Length <> expected.Functions.Length then
            failwithf "Different lengths %d <> %d" template.Functions.Length expected.Functions.Length
        else
            Array.zip
                template.Functions
                expected.Functions
            |> Array.iter
                (fun (t,e) ->
                    test <@ t.Name = e.Name @>
                    test <@ t.EndLine = e.EndLine @>
                    test <@ t.StartLine = e.StartLine @>
                    test <@ t.Index = e.Index @>
                    test <@ t.File = e.File @>
                    compareTemplates t.Tree e.Tree)

let compileLines (lines:string) =
    lines.Split([|'\r';'\n'|], StringSplitOptions.RemoveEmptyEntries)
    |> List.ofArray
    |> Preprocessor.preprocess (fun _ _ -> None) exampleFileName exampleDirectory
    |> CommentStripper.stripComments
    |> LineCategorizer.categorize
    |> List.map (Tokenizer.tokenize >> Parser.parse)
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
                        File = Path.Combine(exampleDirectory, exampleFileName)
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
        "@att %AttName = \"This is an attribute\"
  {
    \"Hello\"
  }
@att %AttName = \"This is an attribute\"
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
                        File = Path.Combine(exampleDirectory, exampleFileName)
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
                        File = Path.Combine(exampleDirectory, exampleFileName)
                        LineNumber = 5
                        StartLocation = 1
                        EndLocation = 5
                        ErrorText = "Duplicate definition of function FuncName"
                    }
            |]
    test <@ result = expected @>

[<Test>]
let ``Test func with no name``() =
    let lines = "@func "
    let result = lines |> compileLines
    let expected =
        CompilationFailure
            [|
                ParserError
                    {
                        File = Path.Combine(exampleDirectory, exampleFileName)
                        LineNumber = 1
                        StartLocation = 1
                        EndLocation = 5
                        ErrorText = "No name given for function"
                    }
            |]
    test <@ result = expected @>

[<Test>]
let ``Test not eager on condition errors``() =
    let lines =
        "@func @blah {
  Hello. [%Unknown1 = \"Unknown\" && %Unknown2 = \"Unknown\"]
  Goodbye. [%Unknown1 = \"Unknown\" || %Unknown2 = \"Unknown\"]
}"
    let result = lines |> compileLines
    let expected =
        CompilationFailure
            [|
                ParserError
                    {
                        File = Path.Combine(exampleDirectory, exampleFileName);
                        LineNumber = 2;
                        StartLocation = 11;
                        EndLocation = 19;
                        ErrorText = "Undefined attribute Unknown1"
                    }
                ParserError
                    {
                        File = Path.Combine(exampleDirectory, exampleFileName)
                        LineNumber = 2;
                        StartLocation = 36;
                        EndLocation = 44;
                        ErrorText = "Undefined attribute Unknown2"
                    }
                ParserError
                    {
                        File = Path.Combine(exampleDirectory, exampleFileName)
                        LineNumber = 3;
                        StartLocation = 13;
                        EndLocation = 21;
                        ErrorText = "Undefined attribute Unknown1"
                    }
                ParserError
                    {
                        File = Path.Combine(exampleDirectory, exampleFileName)
                        LineNumber = 3;
                        StartLocation = 38;
                        EndLocation = 46;
                        ErrorText = "Undefined attribute Unknown2"
                    }
            |]
    test <@ result = expected @>

[<Test>]
let ``Test invoking a function from within a function``() =
    let lines =
        "@func @func1 {
  Hello world.
}
@func @func2 {
  @func1
}"
    let result = lines |> compileLines
    let expected =
        CompilationSuccess
            {   Attributes = [||];
                Variables = [||];
                Functions =
                    [|
                        {   Name = "func1"
                            Index = 0
                            File = fullExampleFile
                            StartLine = 1
                            EndLine = 3
                            AttributeDependencies = [||]
                            VariableDependencies = [||]
                            Tree =
                                Seq
                                    [|
                                        (Sentence (fullExampleFile, 2, SimpleText "Hello world."), True)
                                    |]
                        }
                        {   Name = "func2";
                            Index = 1;
                            File = fullExampleFile
                            StartLine = 4;
                            EndLine = 6;
                            AttributeDependencies = [||]
                            VariableDependencies = [||]
                            Tree = Seq [|(Function 0, True)|];}|];}
    test <@ result = expected @>

[<Test>]
let ``Test filtering out an entire seq block``() =
    let lines =
        "@att %Gender = \"Something\"
  {
    \"Male\"
    \"Female\"
  }
@func @main
{
  @seq {
    Blah.
  } [%Gender = \"Male\"]
}"
    let result = lines |> compileLines
    let expected =
        CompilationSuccess
            {   Attributes =
                    [|
                        {
                            Name = "Gender"
                            Text = "Something"
                            Index = 0
                            File = fullExampleFile
                            StartLine = 1
                            EndLine = 5
                            Values =
                                [|
                                    { Value = "Male"; Condition = True }
                                    { Value = "Female"; Condition = True }
                                |]
                        }
                    |]
                Variables = [||]
                Functions =
                    [|
                        {   Name = "main"
                            Index = 1
                            File = fullExampleFile
                            StartLine = 6
                            EndLine = 11
                            AttributeDependencies = [|0|]
                            VariableDependencies = [||]
                            Tree =
                                Seq
                                    [|
                                        (Seq
                                            [|
                                                (Sentence (fullExampleFile, 9, SimpleText "Blah."), True)
                                            |], AreEqual(0, "Male"))
                                    |]
                        }
                    |] }
    test <@ result = expected @>

[<Test>]
let ``Test filtering out an entire choice block``() =
    let lines =
        "@att %Gender = \"Something\"
  {
    \"Male\"
    \"Female\"
  }
@func @main
{
  @choice {
    Blah.
  } [%Gender = \"Male\"]
}"
    let result = lines |> compileLines
    let expected =
        CompilationSuccess
            {   Attributes =
                    [|
                        {
                            Name = "Gender"
                            Text = "Something"
                            Index = 0
                            File = fullExampleFile
                            StartLine = 1
                            EndLine = 5
                            Values =
                                [|
                                    { Value = "Male"; Condition = True }
                                    { Value = "Female"; Condition = True }
                                |]
                        }
                    |]
                Variables = [||]
                Functions =
                    [|
                        {   Name = "main"
                            Index = 1
                            File = fullExampleFile
                            StartLine = 6
                            EndLine = 11
                            AttributeDependencies = [|0|]
                            VariableDependencies = [||]
                            Tree =
                                Seq
                                    [|
                                        (Choice
                                            [|
                                                (Sentence (fullExampleFile, 9, SimpleText "Blah."), True)
                                            |], AreEqual(0, "Male"))
                                    |]
                        }
                    |] }
    test <@ result = expected @>

[<Test>]
let ``Test variable dependency chain``() =
    let text = "@var $Country = \"Which country are you writing about?\"
  {
    \"U.K.\"
    \"Germany\"
  }

@var @free $City = \"Which city are you writing about?\"
  {
    \"London\" [$Country = \"U.K.\"]
    \"Berlin\" [$Country = \"Germany\"]
  }

@func @dependsOnCity
{
  I just need city here : $City.
}
"
    let result = text |> compileLines
    let expected =
        CompilationSuccess
            {   Attributes = [||]
                Variables =
                    [|
                        {   Name = "Country"
                            Index = 0;
                            File = fullExampleFile
                            StartLine = 1;
                            EndLine = 5;
                            PermitsFreeValue = false;
                            Text = "Which country are you writing about?";
                            Values =
                                [|
                                    {Value = "U.K."; Condition = VarTrue }
                                    {Value = "Germany"; Condition = VarTrue }
                                |] }
                        {   Name = "City";
                            Index = 1;
                            File = fullExampleFile
                            StartLine = 6;
                            EndLine = 10;
                            PermitsFreeValue = true;
                            Text = "Which city are you writing about?";
                            Values =
                                [|
                                    {Value = "London"; Condition = VarAreEqual (Variable 0,"U.K.") }
                                    {Value = "Berlin"; Condition = VarAreEqual (Variable 0,"Germany") }
                                |]
                        }
                    |]
                Functions =
                    [|
                        {   Name = "dependsOnCity"
                            Index = 2;
                            File = fullExampleFile
                            StartLine = 11;
                            EndLine = 14;
                            AttributeDependencies = [||];
                            VariableDependencies = [|0;1|];
                            Tree =
                                Seq
                                    [|
                                        (Sentence
                                            (fullExampleFile, 13,
                                                SimpleSeq
                                                    [|  SimpleText "I just need city here : "; VariableValue 1;
                                                        SimpleText "."
                                                    |]), True)
                                    |] } |] }
    test <@ result = expected @>
