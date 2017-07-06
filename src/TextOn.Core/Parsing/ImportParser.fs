namespace TextOn.Core.Parsing

open TextOn.Core.Tokenizing

type ParsedImportDefinition =
    {
        Line : int
        StartLocation : int
        EndLocation : int
        ImportedFileName : string
    }

[<RequireQualifiedAccess>]
module internal ImportParser =
    let private makeParseError file line s e t =
        {   File = file
            Severity = Error
            StartLine = line
            EndLine = line
            StartLocation = s
            EndLocation = e
            ErrorText = t }

    let private makeParseWarning file line s e t =
        {   File = file
            Severity = Warning
            StartLine = line
            EndLine = line
            StartLocation = s
            EndLocation = e
            ErrorText = t }

    let private makeImport line startLocation endLocation file = { Line = line ; StartLocation = startLocation ; EndLocation = endLocation ; ImportedFileName = file }

    let parseImport (tokenSet:CategorizedAttributedTokenSet) : ParseError[] * ParsedImportDefinition option =
        if tokenSet.Tokens.Length <> 1 then
            failwith "Shouldn't have more than one line in an import"
        else
            let tokensLine = tokenSet.Tokens.[0]
            let lineNumber = tokensLine.LineNumber
            let tokens = tokensLine.Tokens
            if tokens.Length = 1 then
                ([|(makeParseError tokenSet.File lineNumber tokens.[0].TokenStartLocation tokens.[0].TokenEndLocation "Missing file name - expected @import \"filename.texton\"")|], None)
            else if tokens.Length > 2 then
                ([|(makeParseError tokenSet.File lineNumber tokens.[2].TokenStartLocation tokens.[tokens.Length - 1].TokenEndLocation "Unrecognised token - expected @import \"filename.texton\"")|], None)
            else
                match (tokens.[0].Token, tokens.[1].Token) with
                | (Import, QuotedString s) ->
                    [||], Some (makeImport lineNumber tokens.[0].TokenStartLocation tokens.[1].TokenEndLocation s)
                | (Include, QuotedString s) ->
                    [|(makeParseWarning tokenSet.File lineNumber tokens.[0].TokenStartLocation tokens.[0].TokenEndLocation "Deprecated - please use @import")|], Some (makeImport lineNumber tokens.[0].TokenStartLocation tokens.[1].TokenEndLocation s)
                | (Import, _)
                | (Include, _) ->
                    [|(makeParseError tokenSet.File lineNumber tokens.[1].TokenStartLocation tokens.[1].TokenEndLocation "Unrecognised token - expected @import \"filename.texton\"")|], None
                | _ -> failwith "Should have an Include or an Import by this point"

