namespace TextOn.Atom

open System
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
/// Maps preprocessed lines into categories.
module LineCategorizer =
    type private BracketCount = int
    let private funcDefinitionRegex = Regex("^@func\s+")
    let private varDefinitionRegex = Regex("^@var\s+")
    let private attDefinitionRegex = Regex("^@att\s+")
    let private openBraceRegex = Regex("\\s*\{\\s*$")
    let private closeBraceRegex = Regex("\\s*\}\\s*$")

