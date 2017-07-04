module TextOn.Core.Test.TestLinker

open System
open FsUnit
open FsCheck
open NUnit.Framework
open Swensen.Unquote
open FSharp.Quotations
open System.Collections.Generic
open System.IO
open TextOn.Core.Linking
open TextOn.Core.Parsing
open TextOn.Core.Compiling
open TextOn.Core.Conditions

let citiesFile = @"D:\NodeJs\TextOn.Atom\examples\cities.texton"
let genderFile = @"D:\NodeJs\TextOn.Atom\examples\gender.texton"
let exampleFile = @"D:\NodeJs\TextOn.Atom\examples\example.texton"

let expected =
    {
        Errors = []
        Warnings =
            [
                {
                    File = citiesFile
                    Severity = Warning
                    LineNumber = 1
                    StartLocation = 1
                    EndLocation = 8
                    ErrorText = "Deprecated - please use @import"
                }
                {
                    File = exampleFile
                    Severity = Warning
                    LineNumber = 1
                    StartLocation = 1
                    EndLocation = 8
                    ErrorText = "Deprecated - please use @import"
                }
            ]
        Attributes =
            [
                {
                    Name = "Gender"
                    Text = "What is the gender of your target audience?"
                    Index = 0
                    File = genderFile
                    StartLine = 1
                    EndLine = 5
                    AttributeDependencies = []
                    Values = [{Value = "Male";Condition = True};{Value = "Female";Condition = True}]
                }
                {
                    Name = "Pronoun"
                    Text = "What is the pronoun of your target audience?"
                    Index = 1
                    File = genderFile
                    StartLine = 6
                    EndLine = 10
                    AttributeDependencies = [0]
                    Values = [{Value = "He";Condition = AreEqual (0,"Male")};{Value = "She";Condition = AreEqual (0,"Female")}]
                }
            ]
        Variables =
            [
                {
                    Name = "Country"
                    Index = 0
                    File = citiesFile
                    StartLine = 3
                    EndLine = 31
                    PermitsFreeValue = false
                    Text = "Which country are you writing about?"
                    AttributeDependencies = []
                    VariableDependencies = []
                    Values =
                        [
                            {Value = "U.K.";Condition = VarTrue}
                            {Value = "Germany";Condition = VarTrue}
                            {Value = "France";Condition = VarTrue}
                            {Value = "Sweden";Condition = VarTrue}
                            {Value = "Belgium";Condition = VarTrue}
                            {Value = "Netherlands";Condition = VarTrue}
                            {Value = "Ethiopia";Condition = VarTrue}
                            {Value = "Australia";Condition = VarTrue}
                            {Value = "Cuba";Condition = VarTrue}
                            {Value = "Egypt";Condition = VarTrue}
                            {Value = "Albania";Condition = VarTrue}
                            {Value = "Macedonia";Condition = VarTrue}
                            {Value = "Burkina Faso";Condition = VarTrue}
                            {Value = "Japan";Condition = VarTrue}
                            {Value = "Switzerland";Condition = VarTrue}
                            {Value = "Thailand";Condition = VarTrue}
                            {Value = "Vietnam";Condition = VarTrue}
                            {Value = "Cambodia";Condition = VarTrue}
                            {Value = "China";Condition = VarTrue}
                            {Value = "India";Condition = VarTrue}
                            {Value = "Brazil";Condition = VarTrue}
                            {Value = "Peru";Condition = VarTrue}
                            {Value = "Argentina";Condition = VarTrue}
                            {Value = "Canada";Condition = VarTrue}
                            {Value = "U.S.A.";Condition = VarTrue}
                            {Value = "Saudi Arabia";Condition = VarTrue}
                        ]
                }
                {
                    Name = "City"
                    Index = 1
                    File = citiesFile
                    StartLine = 33
                    EndLine = 46
                    PermitsFreeValue = true
                    Text = "Which city are you writing about?"
                    AttributeDependencies = []
                    VariableDependencies = [0]
                    Values =
                        [
                            {Value = "London";Condition = VarAreEqual (Variable 0,"U.K.");};
                            {Value = "Berlin";Condition = VarAreEqual (Variable 0,"Germany");};
                            {Value = "Paris";Condition = VarAreEqual (Variable 0,"France");};
                            {Value = "Birmingham";Condition = VarEither(VarAreEqual (Variable 0,"U.K."),VarAreEqual (Variable 0,"U.S.A."));};
                            {Value = "New York";Condition = VarAreEqual (Variable 0,"U.S.A.");};
                            {Value = "York";Condition = VarAreEqual (Variable 0,"U.K.");};
                            {Value = "Cardiff";Condition = VarAreEqual (Variable 0,"U.K.");};
                            {Value = "Tokyo";Condition = VarAreEqual (Variable 0,"Japan");};
                            {Value = "Lyon";Condition = VarAreEqual (Variable 0,"France");};
                            {Value = "Kyoto";Condition = VarAreEqual (Variable 0,"Japan");};
                            {Value = "Hamburg";Condition = VarAreEqual (Variable 0,"Germany");}
                        ]
                }
                {
                    Name = "ConstrainedVariable"
                    Index = 2
                    File = citiesFile
                    StartLine = 48
                    EndLine = 53
                    PermitsFreeValue = false
                    Text = "This is a constrained variable."
                    AttributeDependencies = []
                    VariableDependencies = []
                    Values =
                        [
                            {Value = "Foo";Condition = VarTrue;}
                            {Value = "Bar";Condition = VarTrue;}
                            {Value = "Baz";Condition = VarTrue;}
                        ]
                }
            ]
        Functions =
            [
                {
                    Name = "dependsOnCity"
                    Index = 0
                    File = citiesFile
                    IsPrivate = true
                    StartLine = 55
                    EndLine = 57
                    FunctionDependencies = []
                    AttributeDependencies = []
                    VariableDependencies = [1]
                    Tree =
                       Seq
                         (citiesFile,55,
                          [(Sentence
                              (citiesFile, 56,
                               SimpleSeq
                                 [SimpleText "This function, that depends on ";
                                  VariableValue 1;
                                  SimpleText ", is defined in the cities file."]), True)]);};
                {
                    Name = "cityExport"
                    Index = 1
                    File = citiesFile
                    IsPrivate = false
                    StartLine = 59
                    EndLine = 61
                    FunctionDependencies = [0]
                    AttributeDependencies = []
                    VariableDependencies = []
                    Tree = Seq (citiesFile,59, [(Function (citiesFile,60,0), True)])
                }
                {
                    Name = "guyStuff"
                    Index = 2
                    File = exampleFile
                    IsPrivate = true
                    StartLine = 6
                    EndLine = 12
                    FunctionDependencies = []
                    AttributeDependencies = []
                    VariableDependencies = []
                    Tree =
                        Seq
                            (exampleFile,6,
                            [(Choice
                                (exampleFile,8,
                                [(Sentence
                                    (exampleFile, 9,
                                    SimpleText "Blah."), True);
                                (Sentence
                                    (exampleFile, 10,
                                    SimpleText "Whatever."), True)]), True)])
                }
                {
                    Name = "dependsOnCity"
                    Index = 3
                    File = exampleFile
                    IsPrivate = true
                    StartLine = 14
                    EndLine = 16
                    FunctionDependencies = []
                    AttributeDependencies = []
                    VariableDependencies = [1]
                    Tree =
                        Seq
                            (exampleFile,14,
                            [(Sentence
                                (exampleFile, 15,
                                SimpleSeq
                                    [SimpleText "This function, that depends on ";
                                    VariableValue 1;
                                    SimpleText ", is defined in the example file."]), True)])
                }
                {
                    Name = "main"
                    Index = 4;
                    File = exampleFile
                    IsPrivate = false;
                    StartLine = 19;
                    EndLine = 31;
                    FunctionDependencies = [1; 3; 2];
                    AttributeDependencies = [0];
                    VariableDependencies = [1; 0];
                    Tree =
                        Seq
                            (exampleFile,19,
                            [(Seq
                                (exampleFile,21,
                                [(Sentence
                                    (exampleFile, 22,
                                    SimpleText "You are a bloke."), AreEqual (0,"Male"));
                                (Sentence
                                    (exampleFile, 23,
                                    SimpleSeq
                                        [SimpleText "You live in ";
                                        SimpleChoice
                                            [VariableValue 1;
                                            SimpleSeq
                                            [SimpleText "a ";
                                                SimpleChoice
                                                [SimpleText "city";
                                                SimpleText "metropolitan area";
                                                SimpleText "town"]; SimpleText " in ";
                                                VariableValue 0]]; SimpleText "."]), True);
                                (Sentence
                                    (exampleFile, 24,
                                    SimpleSeq
                                        [VariableValue 1; SimpleText " is in "; VariableValue 0;
                                        SimpleText "."]), True);
                                (ParagraphBreak
                                    (exampleFile,25), True);
                                (Function
                                    (exampleFile,26,3),
                                    True);
                                (Function
                                    (exampleFile,27,3),
                                    True);
                                (Function
                                    (exampleFile,28,1),
                                    True);
                                (Function
                                    (exampleFile,29,2),
                                    AreEqual (0,"Male"))]), True)])
                }
            ]
    }

let exampleLines =
    [
        "#include \"../examples/cities.texton\""
        ""
        "// This is a comment."
        ""
        "// Here's a function I can call from within the main."
        "@func @private @guyStuff"
        "{"
        "  @choice {"
        "    Blah."
        "    Whatever."
        "  }"
        "}"
        ""
        "@func @private @dependsOnCity {"
        "  This function, that depends on $City, is defined in the example file."
        "}"
        ""
        "// Every full TextOn script must have a main function."
        "@func @main"
        "{"
        "  @seq {"
        "    You are a bloke. [%Gender = \"Male\"]"
        "    You live in {$City|a {city|metropolitan area|town} in $Country}."
        "    $City is in $Country."
        "    @break"
        "    @dependsOnCity"
        "    @dependsOnCity"
        "    @cityExport"
        "    @guyStuff [%Gender = \"Male\"]"
        "  }"
        "}"
    ]

let genderLines =
    [
        "@att %Gender = \"What is the gender of your target audience?\""
        "  {"
        "    \"Male\""
        "    \"Female\""
        "  }"
        "@att %Pronoun = \"What is the pronoun of your target audience?\""
        "  {"
        "    \"He\" [%Gender = \"Male\"]"
        "    \"She\" [%Gender = \"Female\"]"
        "  }"
    ]

let citiesLines =
    [
        "#include \"gender.texton\""
        ""
        "@var $Country = \"Which country are you writing about?\""
        "  {"
        "    \"U.K.\""
        "    \"Germany\""
        "    \"France\""
        "    \"Sweden\""
        "    \"Belgium\""
        "    \"Netherlands\""
        "    \"Ethiopia\""
        "    \"Australia\""
        "    \"Cuba\""
        "    \"Egypt\""
        "    \"Albania\""
        "    \"Macedonia\""
        "    \"Burkina Faso\""
        "    \"Japan\""
        "    \"Switzerland\""
        "    \"Thailand\""
        "    \"Vietnam\""
        "    \"Cambodia\""
        "    \"China\""
        "    \"India\""
        "    \"Brazil\""
        "    \"Peru\""
        "    \"Argentina\""
        "    \"Canada\""
        "    \"U.S.A.\""
        "    \"Saudi Arabia\""
        "  }"
        ""
        "@var @free $City = \"Which city are you writing about?\""
        "  {"
        "    \"London\" [$Country = \"U.K.\"]"
        "    \"Berlin\" [$Country = \"Germany\"]"
        "    \"Paris\" [$Country = \"France\"]"
        "    \"Birmingham\" [$Country = \"U.K.\" || $Country = \"U.S.A.\"]"
        "    \"New York\" [$Country = \"U.S.A.\"]"
        "    \"York\" [$Country = \"U.K.\"]"
        "    \"Cardiff\" [$Country = \"U.K.\"]"
        "    \"Tokyo\" [$Country = \"Japan\"]"
        "    \"Lyon\" [$Country = \"France\"]"
        "    \"Kyoto\" [$Country = \"Japan\"]"
        "    \"Hamburg\" [$Country = \"Germany\"]"
        "  }"
        ""
        "@var $ConstrainedVariable = \"This is a constrained variable.\""
        "  {"
        "    \"Foo\""
        "    \"Bar\""
        "    \"Baz\""
        "  }"
        ""
        "@func @private @dependsOnCity {"
        "  This function, that depends on $City, is defined in the cities file."
        "}"
        ""
        "@func @cityExport {"
        "  @dependsOnCity"
        "}"
    ]

[<Test>]
let ``Full compilation and linking of valid files with deprecated include directive``() =
    let result =
        [
            (citiesFile, citiesLines)
            (exampleFile, exampleLines)
            (genderFile, genderLines)
        ]
        |> List.map (fun (file, lines) -> Compiler.compile file lines)
        |> Linker.link exampleFile
    test <@ result = expected @>

[<Test>]
let ``Missing references``() =
    let result =
        [
            (exampleFile, exampleLines |> List.skip 1)
        ]
        |> List.map (fun (file, lines) -> Compiler.compile file lines)
        |> Linker.link exampleFile
    let expectedMissingReferences =
        {
            Errors =
                [
                    {
                        File = "D:\NodeJs\TextOn.Atom\examples\example.texton";
                        Severity = Error;
                        LineNumber = 14;
                        StartLocation = 34;
                        EndLocation = 38;
                        ErrorText = "Unknown variable City"
                    }
                    {
                        File = "D:\NodeJs\TextOn.Atom\examples\example.texton";
                        Severity = Error;
                        LineNumber = 21;
                        StartLocation = 23;
                        EndLocation = 29;
                        ErrorText = "Unknown attribute Gender"
                    }
                    {
                        File = "D:\NodeJs\TextOn.Atom\examples\example.texton";
                        Severity = Error;
                        LineNumber = 22;
                        StartLocation = 18;
                        EndLocation = 22;
                        ErrorText = "Unknown variable City"
                    }
                    {
                        File = "D:\NodeJs\TextOn.Atom\examples\example.texton";
                        Severity = Error;
                        LineNumber = 22;
                        StartLocation = 59;
                        EndLocation = 66;
                        ErrorText = "Unknown variable Country"
                    }
                    {
                        File = "D:\NodeJs\TextOn.Atom\examples\example.texton";
                        Severity = Error;
                        LineNumber = 23;
                        StartLocation = 5;
                        EndLocation = 9;
                        ErrorText = "Unknown variable City"
                    }
                    {
                        File = "D:\NodeJs\TextOn.Atom\examples\example.texton";
                        Severity = Error;
                        LineNumber = 23;
                        StartLocation = 17;
                        EndLocation = 24;
                        ErrorText = "Unknown variable Country"
                    }
                    {
                        File = "D:\NodeJs\TextOn.Atom\examples\example.texton";
                        Severity = Error;
                        LineNumber = 27;
                        StartLocation = 5;
                        EndLocation = 15;
                        ErrorText = "Unknown function cityExport"
                    }
                    {
                        File = "D:\NodeJs\TextOn.Atom\examples\example.texton";
                        Severity = Error;
                        LineNumber = 28;
                        StartLocation = 16;
                        EndLocation = 22;
                        ErrorText = "Unknown attribute Gender"
                    }
                ]
            Warnings = []
            Attributes = []
            Variables = []
            Functions = []
        }
    test <@ result = expectedMissingReferences @>

[<Test>]
let ``Circular reference in files``() =
    let makeFileName = sprintf @"D:\NodeJs\TextOn.Atom\examples\example%d.texton"
    let makeExampleFile index refToIndices =
        let lines =
            refToIndices
            |> List.map (fun refToIndex -> sprintf "@import \"example%d.texton\"" refToIndex)
        lines |> Compiler.compile (makeFileName index)
    let result =
        [
            (makeExampleFile 1 [2;3])
            (makeExampleFile 2 [4;5])
            (makeExampleFile 3 [4;6])
            (makeExampleFile 4 [6])
            (makeExampleFile 5 [6])
            (makeExampleFile 6 [2])
        ]
        |> Linker.link (makeFileName 1)
    let expected =
        {
            Errors =
                [
                    {
                        File = "D:\NodeJs\TextOn.Atom\examples\example2.texton"
                        Severity = Error
                        LineNumber = 1
                        StartLocation = 1
                        EndLocation = 25
                        ErrorText = "Circular reference - import example4.texton or one of its imports references file D:\NodeJs\TextOn.Atom\examples\example2.texton"
                    }
                    {
                        File = "D:\NodeJs\TextOn.Atom\examples\example2.texton"
                        Severity = Error
                        LineNumber = 2
                        StartLocation = 1
                        EndLocation = 25
                        ErrorText = "Circular reference - import example5.texton or one of its imports references file D:\NodeJs\TextOn.Atom\examples\example2.texton"
                    }
                    {
                        File = "D:\NodeJs\TextOn.Atom\examples\example4.texton"
                        Severity = Error
                        LineNumber = 1
                        StartLocation = 1
                        EndLocation = 25
                        ErrorText = "Circular reference - import example6.texton or one of its imports references file D:\NodeJs\TextOn.Atom\examples\example4.texton"
                    }
                    {
                        File = "D:\NodeJs\TextOn.Atom\examples\example5.texton"
                        Severity = Error
                        LineNumber = 1
                        StartLocation = 1
                        EndLocation = 25
                        ErrorText = "Circular reference - import example6.texton or one of its imports references file D:\NodeJs\TextOn.Atom\examples\example5.texton"
                    }
                    {
                        File = "D:\NodeJs\TextOn.Atom\examples\example6.texton"
                        Severity = Error
                        LineNumber = 1
                        StartLocation = 1
                        EndLocation = 25
                        ErrorText = "Circular reference - import example2.texton or one of its imports references file D:\NodeJs\TextOn.Atom\examples\example6.texton"
                    }
                ]
            Warnings = []
            Attributes = []
            Variables = []
            Functions = []
        }
    test <@ result = expected @>

[<Test>]
let ``Self reference in file``() =
    let makeFileName = sprintf @"D:\NodeJs\TextOn.Atom\examples\example%d.texton"
    let makeExampleFile index refToIndices =
        let lines =
            refToIndices
            |> List.map (fun refToIndex -> sprintf "@import \"example%d.texton\"" refToIndex)
        lines |> Compiler.compile (makeFileName index)
    let result =
        [
            (makeExampleFile 1 [1])
        ]
        |> Linker.link (makeFileName 1)
    let expected =
        {
            Errors =
              [
                    {
                        File = "D:\NodeJs\TextOn.Atom\examples\example1.texton"
                        Severity = Error
                        LineNumber = 1
                        StartLocation = 1
                        EndLocation = 25
                        ErrorText = "Circular reference - import example1.texton or one of its imports references file D:\NodeJs\TextOn.Atom\examples\example1.texton"
                    }
                ]
            Warnings = []
            Attributes = []
            Variables = []
            Functions = []
        }
    test <@ result = expected @>

[<Test>]
let ``Infinite recursion``() =
    let lines =
        [
            "@func @outer {"
            "    @inner"
            "}"
            "@func @inner {"
            "    @outer"
            "}"
        ]
    let template =
        lines
        |> Compiler.compile exampleFile
        |> List.singleton
        |> Linker.link exampleFile
    test <@ template.Errors.Length > 0 @>

