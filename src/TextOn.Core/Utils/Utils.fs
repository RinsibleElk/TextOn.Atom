[<RequireQualifiedAccess>]
module TextOn.Core.Utils.Utils

open System.IO

let getNormalizedPath file =
    Path.GetFullPath(file).ToLower()

let getImportPath rootFile importedFile =
    Path.GetFullPath(Path.Combine(FileInfo(rootFile).Directory.FullName, importedFile))

let getNormalizedImportPath rootFile importedFile =
    getImportPath rootFile importedFile |> getNormalizedPath

