namespace TextOn.Atom

open System
open System.IO
open System.Collections.Concurrent
open TextOn.Core.Utils
open TextOn.Core.Linking
open TextOn.Core.Pipeline

type Commands (serialize : Serializer) =
    let fileLinesMap = ConcurrentDictionary<string, string list>()
    let fileTemplateMap = ConcurrentDictionary<string, Template>()
    let generatorSingleton = ConcurrentDictionary<int, GeneratorServer>()
    let browserSingleton = ConcurrentDictionary<int, BrowserServer>()
    let mutable isBrowsing = false
    let add n lines =
        let f = System.Func<string, string list, string list>(fun _ _ -> lines)
        fileLinesMap.AddOrUpdate(n, lines, f) |> ignore
    let fileResolver n f =
        let o =
            let (ok, l) = fileLinesMap.TryGetValue(n)
            if ok then Some l
            else None
        if o.IsNone then
            let o = f |> fun f -> if f |> File.Exists then (Some (f |> File.ReadAllLines |> List.ofArray)) else None
            if o.IsNone then None
            else
                add n o.Value
                o
        else o
    let doCompile fileName lines =
        async {
            let template = Builder.build fileResolver fileName lines
            fileTemplateMap.[fileName] <- template
            return Success template }
    let parse' fileName directory lines =
        async {
            let n = Utils.getNormalizedPath fileName
            add n lines
            let! result = doCompile fileName lines
            return
                match result with
                | Failure e -> [CommandResponse.error serialize e]
                | Success (template) ->
                    let errors = (template.Errors @ template.Warnings) |> List.toArray
                    [ CommandResponse.errors serialize (errors, fileName) ] }

    let keywords =
        [|
            ("var",     "Define a new variable")
            ("att",     "Define a new attribute")
            ("func",    "Define a new function")
            ("free",    "Allow a variable to take any user value")
            ("break",   "Insert a paragraph break")
            ("choice",  "Define a new random choice")
            ("seq",     "Define a new sequence of sentences")
            ("private", "Do not export this function outside of the current TextOn file")
            ("import",  "Import another texton module")
        |]
        |> Array.map (fun (x,d) -> { text = x ; ``type`` = "keyword" ; description = d } : DTO.DTO.Suggestion)

    let doGenerateStart fileName directory lines line = async {
        let n = Utils.getNormalizedPath fileName
        add n lines
        let! result = doCompile fileName lines
        return
            match result with
            | Failure e -> Failure e
            | Success template ->
                if template.Errors.Length > 0 then
                    Success (GeneratorStartResult.CompilationFailure (template.Errors |> List.toArray))
                else
                    template.Functions
                    |> List.tryFind (fun f -> f.File = fileName && (not f.IsPrivate) && f.StartLine <= line && f.EndLine >= line)
                    |> Option.map
                        (fun f ->
                            let generator =
                                let generator = GeneratorServer(f.File, f.Name)
                                generatorSingleton.AddOrUpdate(0, generator, fun _ _ -> generator)
                            generator.UpdateTemplate(template)
                            Success (GeneratorStartResult.GeneratorStarted generator.Data))
                    |> defaultArg <| Failure "Nothing to generate" }

    let doBrowserStart fileName directory lines = async {
        let n = Utils.getNormalizedPath fileName
        add n lines
        let! result = doCompile fileName lines
        return
            match result with
            | Failure e -> Failure e
            | Success template ->
                if template.Errors.Length > 0 then Success (BrowserStartResult.BrowserCompilationFailure (template.Errors |> List.toArray))
                else
                    let browser =
                        let browser = new BrowserServer(fileName)
                        browserSingleton.AddOrUpdate(0, browser, (fun _ _ -> browser))
                    browser.UpdateTemplate(template)
                    Success (BrowserStartResult.BrowserStarted browser.Data) }

    let doBrowserExpand fileName directory rootFunction indexPath = async {
        if (not (isBrowsing)) then return Failure "Not browsing"
        else
            let ok, browser = browserSingleton.TryGetValue(0)
            if not ok then return Failure "No browser"
            else if browser.File.FullName <> fileName then return Failure (sprintf "Browser is for file %s, asked for file %s" browser.File.FullName fileName)
            else
                let items = browser.ExpandAt rootFunction indexPath
                return
                    items
                    |> Option.map Success
                    |> defaultArg <| Failure "Couldn't find IndexPath" }

    let doBrowserCollapse fileName directory rootFunction indexPath = async {
        if (not (isBrowsing)) then return Failure "Not browsing"
        else
            let ok, browser = browserSingleton.TryGetValue(0)
            if not ok then return Failure "No browser"
            else if browser.File.FullName <> fileName then return Failure (sprintf "Browser is for file %s, asked for file %s" browser.File.FullName fileName)
            else
                let res = browser.CollapseAt rootFunction indexPath
                return
                    res
                    |> Option.map Success
                    |> defaultArg <| Failure "Couldn't find IndexPath" }

    member __.Parse file lines =
        async {
            let lines = lines |> List.ofArray
            let fi = Path.GetFullPath file |> FileInfo
            return! parse' file fi.Directory.FullName lines }

    member __.GenerateStart (file:SourceFilePath) lines line = async {
        let fi = Path.GetFullPath file |> FileInfo
        let! result = doGenerateStart file fi.Directory.FullName lines line
        return
            match result with
            | Failure e -> [CommandResponse.error serialize e]
            | Success (generateStartResult) ->
                match generateStartResult with
                | GeneratorStartResult.CompilationFailure(errors) ->
                    [ CommandResponse.error serialize "Nothing to generate"]
                | GeneratorStartResult.GeneratorStarted(generatorSetup) ->
                    [ CommandResponse.generatorSetup serialize generatorSetup ] }

    member __.GenerateStop () = async {
        generatorSingleton.Clear()
        return [] }

    member __.BrowserStart (file:SourceFilePath) lines = async {
        let fi = Path.GetFullPath file |> FileInfo
        let! result = doBrowserStart file fi.Directory.FullName lines
        return
            match result with
            | Failure e -> [CommandResponse.error serialize e]
            | Success (generateStartResult) ->
                match generateStartResult with
                | BrowserStartResult.BrowserCompilationFailure(errors) ->
                    [ CommandResponse.error serialize "Nothing to browse"]
                | BrowserStartResult.BrowserStarted(browserUpdate) ->
                    isBrowsing <- true
                    [ CommandResponse.browserUpdate serialize browserUpdate ] }

    member __.BrowserExpand file rootFunction indexPath = async {
        let fi = Path.GetFullPath file |> FileInfo
        let! result = doBrowserExpand file fi.Directory.FullName rootFunction indexPath
        return
            match result with
            | Failure e -> [CommandResponse.error serialize e]
            | Success browserItems ->
                [ CommandResponse.browserItems serialize browserItems ] }

    member __.BrowserCollapse file rootFunction indexPath = async {
        let fi = Path.GetFullPath file |> FileInfo
        let! result = doBrowserCollapse file fi.Directory.FullName rootFunction indexPath
        return
            match result with
            | Failure e -> [CommandResponse.error serialize e]
            | Success _ -> [CommandResponse.thanks serialize] }

    member __.BrowserStop () = async {
        isBrowsing <- false
        browserSingleton.Clear()
        return [] }

    member __.GeneratorValueSet ty name value = async {
        let ok, generator = generatorSingleton.TryGetValue 0
        return
            if ok then
                generator.SetValue ty name value
                [ CommandResponse.generatorSetup serialize generator.Data ]
            else [ CommandResponse.error serialize "Nothing to generate" ] }

    member __.BrowserValueSet fileName ty name value = async {
        let ok, browser = browserSingleton.TryGetValue(0)
        return
            if not ok then [ CommandResponse.error serialize "Nothing to browse" ]
            else if browser.File.FullName <> fileName then [ CommandResponse.error serialize (sprintf "Browser is for file %s, asked for file %s" browser.File.FullName fileName) ]
            else
                browser.SetValue ty name value
                [ CommandResponse.browserUpdate serialize browser.Data ] }

    member __.Generate config = async {
        let ok, generator = generatorSingleton.TryGetValue 0
        return
            if ok then
                generator.Generate config
                [ CommandResponse.generatorSetup serialize generator.Data ]
            else [ CommandResponse.error serialize "Nothing to generate" ] }

    member __.UpdateGenerator () = async {
        let ok, generator = generatorSingleton.TryGetValue 0
        if ok then
            let fi = generator.File
            let lines = fileResolver (Utils.getNormalizedPath fi.FullName) fi.FullName
            if lines.IsSome then
                let lines = lines.Value
                let! compileResult = doCompile fi.FullName lines
                match compileResult with
                | Success template ->
                    if template.Errors.Length = 0 then
                        generator.UpdateTemplate template
                        return [ CommandResponse.generatorSetup serialize generator.Data ]
                    else return [ CommandResponse.error serialize "Nothing to generate" ]
                | _ -> return [ CommandResponse.error serialize "Nothing to generate" ]
            else return [ CommandResponse.error serialize "Nothing to generate" ]
        else return [ CommandResponse.error serialize "Nothing to generate" ] }

    member __.UpdateBrowser() = async {
        let ok, browser = browserSingleton.TryGetValue 0
        if ok then
            let fi = browser.File
            let lines = fileResolver (Utils.getNormalizedPath fi.FullName) fi.FullName
            if lines.IsSome then
                let lines = lines.Value
                let! compileResult = doCompile fi.FullName lines
                match compileResult with
                | Success template ->
                    if template.Errors.Length = 0 then
                        browser.UpdateTemplate template
                        return [ CommandResponse.browserUpdate serialize browser.Data ]
                    else return [ CommandResponse.error serialize "Nothing to browse" ]
                | _ -> return [ CommandResponse.error serialize "Nothing to browse" ]
            else return [ CommandResponse.error serialize "Nothing to browse" ]
        else return [ CommandResponse.error serialize "Nothing to browse" ] }

    member __.BrowserCycle fileName line = async {
        let ok, browser = browserSingleton.TryGetValue 0
        if ok then
            if browser.CycleThroughTo fileName line then
                return [ CommandResponse.browserUpdate serialize browser.Data ]
            else
                return [ CommandResponse.error serialize "Text not in a function" ]
        else return [ CommandResponse.error serialize "Nothing to browse" ] }

    member __.GetCompletions fileName ty (line:string) (col:int) = async {
        let template = fileTemplateMap.TryFind(fileName)
        match ty with
        | "Function" ->
            // We add the keywords to this list.
            let functions = template |> Option.map (fun t -> t.Functions |> List.toArray |> Array.filter (fun fn -> (not fn.IsPrivate) || fn.File = fileName) |> Array.map (fun f -> { text = f.Name ; ``type`` = "function" ; description = "Call the @" + f.Name + " function" } : DTO.DTO.Suggestion)) |> defaultArg <| [||]
            return [ CommandResponse.suggestions serialize (Array.append functions keywords) ]
        | "Variable" ->
            return [ CommandResponse.suggestions serialize (template |> Option.map (fun t -> t.Variables |> List.toArray |> Array.map (fun x -> { text = x.Name ; ``type`` = "variable" ; description = sprintf "$%s: %s" x.Name x.Text } : DTO.DTO.Suggestion)) |> defaultArg <| [||]) ]
        | "Attribute" ->
            return [ CommandResponse.suggestions serialize (template |> Option.map (fun t -> t.Attributes |> List.toArray |> Array.map (fun x -> { text = x.Name ; ``type`` = "attribute" ; description = sprintf "%%%s: %s" x.Name x.Text } : DTO.DTO.Suggestion)) |> defaultArg <| [||]) ]
        | "QuotedString" ->
            // Bit of work to do. We need to backtrack to try and find a '%' or a '$' character, then try and tokenize just the named value after that point.
            let mutable name = []
            let mutable prefix = []
            let mutable i = col
            while i >= 0 && line.[i] <> '"' do
                prefix <- line.[i]::prefix
                i <- i - 1
            while i >= 0 && (not (Char.IsLetterOrDigit line.[i])) && (line.[i] <> '_') do
                i <- i - 1
            if i < 0 then
                return [ CommandResponse.suggestions serialize [||] ]
            else
                while i >= 0 && ((Char.IsLetterOrDigit line.[i]) || (line.[i] = '_')) do
                    name <- line.[i]::name
                    i <- i - 1
                if i < 0 || (line.[i] <> '$' && line.[i] <> '%' && line.[i] <> '@') then
                    return [ CommandResponse.suggestions serialize [||] ]
                else
                    let values =
                        if line.[i] = '@' then
                            // We add the directory contents.
                            let fi = FileInfo fileName
                            if fi.Exists |> not then [||]
                            else
                                // We want to look at the given prefix, and find the right set of suggested directory contents to send back.
                                let existingQuery = String.Join("", prefix)
                                let existingDirectory =
                                    let i = existingQuery.LastIndexOf('/')
                                    if i >= 0 then
                                        DirectoryInfo(Path.Combine(fi.Directory.FullName, existingQuery.Substring(0, i + 1)))
                                    else
                                        fi.Directory
                                let files = existingDirectory.GetFiles("*.texton") |> Array.filter (fun f -> f.FullName.ToUpper() <> fi.FullName.ToUpper())
                                let directories = existingDirectory.GetDirectories()
                                Array.append
                                    (files |> Array.map (fun f -> f.Name, "Include the contents of " + f.Name, "include"))
                                    (directories |> Array.map (fun f -> f.Name + "/", "Subdirectory " + f.Name, "directory"))
                        else if line.[i] = '$' then
                            let actualName = String.Join("", name |> List.toArray)
                            template
                            |> Option.bind (fun t -> t.Variables |> List.tryFind (fun v -> v.Name = actualName))
                            |> Option.map
                                (fun v ->
                                    let description = sprintf "Value for variable $%s - %s" v.Name
                                    v.Values
                                    |> List.toArray
                                    |> Array.map (fun x -> x.Value, description x.Value, "value"))
                            |> defaultArg <| [||]
                        else
                            let actualName = String.Join("", name |> List.toArray)
                            template
                            |> Option.bind (fun t -> t.Attributes |> List.tryFind (fun v -> v.Name = actualName))
                            |> Option.map
                                (fun v ->
                                    let description = sprintf "Value for attribute %%%s - %s" v.Name
                                    v.Values
                                    |> List.toArray
                                    |> Array.map (fun x -> x.Value, description x.Value, "value"))
                            |> defaultArg <| [||]
                    return [ CommandResponse.suggestions serialize (values |> Array.map (fun (value, desc, ty) -> { text = value ; description = desc ; ``type`` = ty })) ]
        | _ ->
            return [ CommandResponse.error serialize "Unexpected type" ] }

    member __.NavigateToSymbol (file:SourceFilePath) (line:string) column = async {
        let template = fileTemplateMap.TryFind(file)
        if template.IsNone then
            return [ CommandResponse.error serialize "Cannot find symbol" ]
        else
            let forwardCharacters =
                let mutable i = column + 1
                let mutable l = []
                while i < line.Length && ((Char.IsLetterOrDigit line.[i]) || line.[i] = '_') do
                    l <- line.[i]::l
                    i <- i + 1
                l |> List.rev
            let backwardCharacters =
                let mutable i = column
                let mutable l = []
                while i >= 0 && ((Char.IsLetterOrDigit line.[i]) || line.[i] = '_') do
                    l <- line.[i] :: l
                    i <- i - 1
                if i < 0 then None else Some (line.[i], l)
            let fileAndLine =
                match backwardCharacters with
                | None -> None
                | (Some ('@', l)) ->
                    let functionName = String.Join("", l@forwardCharacters)
                    template.Value.Functions |> List.tryFind (fun f -> f.Name = functionName) |> Option.map (fun f -> f.File, f.StartLine)
                | (Some ('$', l)) ->
                    let variableName = String.Join("", l@forwardCharacters)
                    template.Value.Variables |> List.tryFind (fun f -> f.Name = variableName) |> Option.map (fun f -> f.File, f.StartLine)
                | (Some ('%', l)) ->
                    let attributeName = String.Join("", l@forwardCharacters)
                    template.Value.Attributes |> List.tryFind (fun f -> f.Name = attributeName) |> Option.map (fun f -> f.File, f.StartLine)
                | _ -> None
            if fileAndLine.IsSome then
                let (file, line) = fileAndLine.Value
                return [ CommandResponse.navigate serialize { FileName = file ; LineNumber = line ; Location = 1 } ]
            else
                return [ CommandResponse.error serialize "Cannot find symbol" ] }

    member __.Navigate (file:SourceFilePath) ty name = async {
        let fi = Path.GetFullPath file |> FileInfo
        let lines = fileResolver (Utils.getNormalizedPath fi.FullName) fi.FullName
        if lines |> Option.isSome then
            let lines = lines.Value
            let! compileResult = doCompile fi.FullName lines
            return
                match compileResult with
                | Failure e -> [CommandResponse.error serialize e]
                | Success (template) ->
                    if template.Errors.Length > 0 then
                        [ CommandResponse.error serialize "File had compilation errors" ]
                    else
                        let f =
                            match ty with
                            | "Function" -> template.Functions |> List.tryFind (fun fn -> fn.Name = name) |> Option.map (fun fn -> fn.File, fn.StartLine)
                            | "Variable" -> template.Variables |> List.tryFind (fun fn -> fn.Name = name) |> Option.map (fun fn -> fn.File, fn.StartLine)
                            | "Attribute" -> template.Attributes |> List.tryFind (fun fn -> fn.Name = name) |> Option.map (fun fn -> fn.File, fn.StartLine)
                            | _ -> failwith "Internal error"
                        if f |> Option.isNone then
                            [ CommandResponse.error serialize "Function not found" ]
                        else
                            let (f, l) = f.Value
                            [ CommandResponse.navigate serialize { FileName = f ; LineNumber = l ; Location = 1 } ]
        else
            return [CommandResponse.error serialize "File not found"] }
