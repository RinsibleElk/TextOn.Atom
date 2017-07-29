namespace TextOn.Atom.Test

open System
open System.IO

type TestTempDir() =
    let tempPath    = Path.GetTempPath()
    let tempDir     = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
    let di          = DirectoryInfo(Path.Combine(tempPath, tempDir))
    do di.Create()
    member __.CreateFile file lines =
        File.WriteAllLines(Path.Combine(di.FullName, file), lines |> List.toArray)
    member this.ModifyFile file lines =
        this.CreateFile file lines
    member __.GetFileName file =
        Path.Combine(di.FullName, file)
    member __.GetLines file =
        Path.Combine(di.FullName, file)
        |> File.ReadAllLines
        |> List.ofArray
    interface IDisposable
        with
            member __.Dispose() =
                di.Delete(true)

