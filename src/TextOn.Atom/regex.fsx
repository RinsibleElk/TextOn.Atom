#I __SOURCE_DIRECTORY__
#r "bin/Debug/TextOn.Atom.exe"
open TextOn.Atom
open System
open System.IO
open System.Text.RegularExpressions

let stopwatch = new System.Diagnostics.Stopwatch()
stopwatch.Start()
// Jonas: here's the pipeline so far. I want to test this with more complex examples like the original ones, once we've converted.
let file = FileInfo(@"D:\NodeJs\TextOn.Atom\examples\example.texton")
let preprocessed = Preprocessor.preprocess Preprocessor.realFileResolver file.Name (Some file.Directory.FullName) (file.FullName |> File.ReadAllLines |> Seq.ofArray)
let stripped = CommentStripper.stripComments preprocessed
let categorized = stripped |> LineCategorizer.categorize
let tokenized = categorized |> Seq.map (fun a -> async { return Tokenizer.tokenize a }) |> Async.Parallel |> Async.RunSynchronously
stopwatch.Stop()
stopwatch.ElapsedMilliseconds

let fixLine (line:string) =
    line.Replace("[MÄRKE]", "$MÄRKE")
        .Replace("[P2]", "$P2")
        .Replace("[P3]", "$P3")
        .Split([|' '|], StringSplitOptions.RemoveEmptyEntries)
    |> Array.map
        (fun x ->
            let splitBySlash = x.Split([|'/'|], StringSplitOptions.RemoveEmptyEntries)
            if splitBySlash.Length = 1 then
                x
            else
                "{" + (String.Join("|", (splitBySlash |> Array.map (fun y -> y.Replace("_", " "))))) + "}")
    |> fun a -> "    " + String.Join(" ", a)
let rec fixLines funcPrefix functionIndex (lines:string seq) =
    if lines |> Seq.isEmpty then Seq.empty
    else
        let line = lines |> Seq.head
        let remaining = lines |> Seq.tail
        if line.StartsWith("STYCKE SLUT") then Seq.empty
        else
            if line.StartsWith("STYCKE") then
                if functionIndex |> Option.isNone then
                    seq {
                        yield (sprintf "@func @stycke%s%d" funcPrefix 0)
                        yield "  {"
                        yield! (fixLines funcPrefix (Some 1) remaining) }
                else
                    seq {
                        yield (sprintf "@func @stycke%s%d" funcPrefix functionIndex.Value)
                        yield "  {"
                        yield! (fixLines funcPrefix (Some (functionIndex.Value + 1)) remaining) }
            else if line |> String.IsNullOrWhiteSpace then
                seq {
                    yield "  }"
                    yield ""
                    yield! (fixLines funcPrefix functionIndex remaining) }
            else
                seq {
                    yield (fixLine line)
                    yield! fixLines funcPrefix functionIndex remaining }
[
    @"2016-08-23"
    @"2016-08-29"
    @"2016-08-31"
    @"2016-09-14"
]
|> Seq.iter
    (fun file ->
        let funcPrefix = file.Replace("-", "")
        let file = Path.Combine(@"D:\NodeJs\TextOn.Atom\examples\source", file + ".txt")
        let lines = file |> File.ReadAllLines
        lines
        |> fixLines (funcPrefix + "_") None
        |> fun outputLines -> File.WriteAllLines(Path.Combine(@"D:\NodeJs\TextOn.Atom\examples\original", funcPrefix + ".texton"), outputLines))


