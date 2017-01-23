#I __SOURCE_DIRECTORY__
#r "bin/Debug/TextOn.Atom.exe"
open TextOn.Atom
open System.IO

let categorized =
    @"D:\NodeJs\TextOn.Atom\examples\example.texton"
    |> File.ReadLines
    |> Preprocessor.preprocess Preprocessor.realFileResolver @"D:\NodeJs\TextOn.Atom\examples\example.texton" (Some @"D:\NodeJs\TextOn.Atom\examples")
    |> CommentStripper.stripComments
    |> LineCategorizer.categorize
categorized
|> Seq.map (fun x -> (x.Category, x.Index, (x.Lines |> Seq.head |> fun a -> a.Contents)))
|> Seq.iter (printfn "%A")


