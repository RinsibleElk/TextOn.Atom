// --------------------------------------------------------------------------------------
// FAKE build script
// --------------------------------------------------------------------------------------

#I "packages/FAKE/tools"
#r "packages/FAKE/tools/FakeLib.dll"
open System
open System.Diagnostics
open System.IO
open Fake
open Fake.Git
open Fake.ProcessHelper
open Fake.ReleaseNotesHelper
open Fake.ZipHelper

#if MONO
#else
#load   "src/TextOn.Atom.Js/atom-bindings.fsx"
        "src/TextOn.Atom.Js/ProcessHelpers.fs"
        "src/TextOn.Atom.Js/Control.fs"
        "src/TextOn.Atom.Js/DTO.fs"
        "src/TextOn.Atom.Js/Logging.fs"
        "src/TextOn.Atom.Js/LanguageService.fs"
        "src/TextOn.Atom.Js/Errors.fs"
        "src/TextOn.Atom.Js/TextOnIDE.fs"
        "src/TextOn.Atom.Js/main.fs"
#endif


// Git configuration (used for publishing documentation in gh-pages branch)
// The profile where the project is posted
let gitOwner = "RinsibleElk"
let gitHome = "https://github.com/" + gitOwner


// The name of the project on GitHub
let gitName = "TextOn.Atom"

// The url for the raw files hosted
let gitRaw = environVarOrDefault "gitRaw" "https://raw.github.com/RinsibleElk"

let tempReleaseDir = "temp/release"

// Read additional information from the release notes document
let releaseNotesData =
    File.ReadAllLines "RELEASE_NOTES.md"
    |> parseAllReleaseNotes

let release = List.head releaseNotesData

let msg =  release.Notes |> List.fold (fun r s -> r + s + "\n") ""
let releaseMsg = (sprintf "Release %s\n" release.NugetVersion) + msg


let run cmd args dir =
    if execProcess( fun info ->
        info.FileName <- cmd
        if not( String.IsNullOrWhiteSpace dir) then
            info.WorkingDirectory <- dir
        info.Arguments <- args
    ) System.TimeSpan.MaxValue = false then
        traceError <| sprintf "Error while running '%s' with args: %s" cmd args

let atomPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) </> "atom" </> "bin" 

let apmTool, atomTool =
    #if MONO
        "apm", atom 
    #else
        atomPath </> "apm.cmd" , atomPath </> "atom.cmd"
    #endif


// --------------------------------------------------------------------------------------
// Build the Generator project and run it
// --------------------------------------------------------------------------------------

Target "Clean" (fun _ ->
    CopyFiles "release" ["README.md"; "LICENSE"; "RELEASE_NOTES.md"]
    CleanDir tempReleaseDir
)


Target "BuildGenerator" (fun () ->
    [ __SOURCE_DIRECTORY__ @@ "src" @@ "TextOn.Atom.Js.fsproj" ]
    |> MSBuildDebug "" "Rebuild"
    |> Log "AppBuild-Output: "
)

Target "RunGenerator" (fun () ->
    (TimeSpan.FromMinutes 5.0)
    |> ProcessHelper.ExecProcess (fun p ->
        p.FileName <- __SOURCE_DIRECTORY__ @@ "src" @@ "TextOn.Atom.Js" @@ "bin" @@ "Debug" @@ "TextOn.Atom.Js.exe" )
    |> ignore
)

#if MONO
#else
Target "RunScript" (fun () ->
    TextOn.Atom.Js.Generator.translateModules "../../release/lib/texton.js"
)
#endif

// Installs npm dependencies defined in "release\package.json" to "release\node_modules"
Target "InstallDependencies" (fun _ ->
    run apmTool "install" "release"
)

// --------------------------------------------------------------------------------------
// Run generator by default. Invoke 'build <Target>' to override
// --------------------------------------------------------------------------------------

Target "Default" DoNothing

#if MONO
"Clean"
    ==> "BuildGenerator"
    ==> "RunGenerator"
    ==> "InstallDependencies"
#else
"Clean"
    ==> "RunScript"
    ==> "InstallDependencies"
#endif

"InstallDependencies"
    ==> "Default"

RunTargetOrDefault "Default"
