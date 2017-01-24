#I __SOURCE_DIRECTORY__
#r "bin/Debug/TextOn.Atom.exe"
open TextOn.Atom
open System
open System.IO
open System.Text.RegularExpressions

// Jonas: here's the pipeline so far. I want to test this with more complex examples like the original ones, once we've converted.
let file = FileInfo(@"D:\NodeJs\TextOn.Atom\examples\example.texton")
let preprocessed = Preprocessor.preprocess Preprocessor.realFileResolver file.Name (Some file.Directory.FullName) (file.FullName |> File.ReadLines)
let stripped = CommentStripper.stripComments preprocessed
let categorized = stripped |> LineCategorizer.categorize
// Obviously, here I would retain the info from categories - I need this for stuff like GotoDefinition, Tooltips and so on.
categorized
|> Seq.map
    (fun x ->
        match x.Category with
        | FuncDefinition -> async { return Some (x.Lines |> Seq.map (fun line -> match line.Contents with | Line line -> DefinitionLineTokenizer.tokenizeLine line | _ -> failwith "")) }
        | VarDefinition -> async { return Some (x.Lines |> Seq.map (fun line -> match line.Contents with | Line line -> VariableLineTokenizer.tokenizeLine line | _ -> failwith "")) }
        | AttDefinition -> async { return Some (x.Lines |> Seq.map (fun line -> match line.Contents with | Line line -> AttributeLineTokenizer.tokenizeLine line | _ -> failwith "")) }
        | PreprocessorError -> async { return None }
        | PreprocessorWarning -> async { return None }
        | CategorizationError -> async { return None })
|> Async.Parallel
|> Async.RunSynchronously
|> Seq.collect (Option.get)
|> Seq.concat
|> Seq.iter ignore




