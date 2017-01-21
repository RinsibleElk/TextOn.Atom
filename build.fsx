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
#load   "src/main.fs"
#endif

let grammarDir = "definition/grammars"
let grammarRelease = "release/grammars"

Target "CopyGrammar" (fun _ ->
    ensureDirectory grammarRelease
    CleanDir grammarRelease
    CopyFiles grammarRelease [
        grammarDir </> "texton.json"
    ]
)

Target "Default" DoNothing

RunTargetOrDefault "Default"
