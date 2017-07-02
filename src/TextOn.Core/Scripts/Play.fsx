#r @"../bin/Debug/TextOn.Core.dll"

open System
open TextOn.Core.Compiling
open TextOn.Core.Parsing

let text =
    "#include \"../examples/cities.texton\"

// This is a comment.

// Here's a function I can call from within the main.
@func @private @guyStuff
{
  @choice {
    Blah.
    Whatever.
  }
}

@func @private @dependsOnCity {
  This function, that depends on $City, is defined in the example file.
}

// Every full TextOn script must have a main function.
@func @main
{
  @seq {
    You are a bloke. [%Gender = \"Male\"]
    You live in {$City|a {city|metropolitan area|town} in $Country}.
    $City is in $Country.
    @break
    @dependsOnCity
    @dependsOnCity
    @cityExport
    @guyStuff [%Gender = \"Male\"]
  }
}"

let lines = text.Split([|'\n';'\r'|], StringSplitOptions.RemoveEmptyEntries) |> List.ofArray

let fileName = @"D:\NodeJs\TextOn\example.texton"

let compiledModule = Compiler.compile fileName lines
