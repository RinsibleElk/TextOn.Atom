namespace TextOn.Atom

open System
open System.IO
open System.Collections.Concurrent

type Commands (serialize : Serializer) =
    let fileLinesMap = ConcurrentDictionary<string, string list>()
    let fileTemplateMap = ConcurrentDictionary<string, CompiledTemplate>()
    //TODO: Make thread-safe.
    let mutable generator = None
    let add fileName directory lines =
        let key = Path.Combine(directory, fileName).ToLower()
        let f = System.Func<string, string list, string list>(fun _ _ -> lines)
        fileLinesMap.AddOrUpdate(key, lines, f) |> ignore
    let fileResolver f d =
        let o =
            let (ok, r) = fileLinesMap.TryGetValue(Path.Combine(d, f).ToLower())
            if ok then Some r
            else None
            |> Option.map (fun x -> (f, d, x))
        if o.IsNone then Preprocessor.realFileResolver f d
        else o
    let doCompile fileName directory lines =
        async {
            let lines = Preprocessor.preprocess fileResolver fileName directory lines
            let lines' = CommentStripper.stripComments lines
            let groups = LineCategorizer.categorize lines'
            let tokens = groups |> List.map Tokenizer.tokenize
            let source = tokens |> List.map Parser.parse
            let output = Compiler.compile source
            match output with
            | CompilationSuccess template -> fileTemplateMap.[fileName] <- template
            | _ -> ()
            return Success output }
    let parse' fileName directory lines =
        async {
            add fileName directory lines
            let! result = doCompile fileName directory lines
            return
                match result with
                | Failure e -> [CommandResponse.error serialize e]
                | Success (compilationResult) ->
                    match compilationResult with
                    | CompilationResult.CompilationFailure(errors) ->
                        [ CommandResponse.errors serialize (errors, fileName) ]
                    | _ ->
                        let errors = [||]
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
        |]
        |> Array.map (fun (x,d) -> { text = x ; ``type`` = "keyword" ; description = d } : DTO.DTO.Suggestion)

    let doGenerateStart fileName directory lines line = async {
        add fileName directory lines
        let! compileResult = doCompile fileName directory lines
        return
            match compileResult with
            | Failure e -> Failure e
            | Success compilationResult ->
                match compilationResult with
                | CompilationResult.CompilationFailure errors -> Success (GeneratorStartResult.CompilationFailure errors)
                | CompilationResult.CompilationSuccess template ->
                    template.Functions
                    |> Array.tryFind (fun f -> f.File = fileName && f.StartLine <= line && f.EndLine >= line)
                    |> Option.map
                        (fun f ->
                            let g = GeneratorServer(f.File, f.Name)
                            g.UpdateTemplate(template)
                            generator <- Some g
                            Success (GeneratorStartResult.GeneratorStarted generator.Value.Data))
                    |> defaultArg <| Failure "Nothing to generate" }

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
        generator <- None
        return [] }

    member __.GeneratorValueSet ty name value = async {
        return
            if generator.IsSome then
                generator.Value.SetValue ty name value
                [ CommandResponse.generatorSetup serialize generator.Value.Data ]
            else [ CommandResponse.error serialize "Nothing to generate" ] }

    member __.Generate config = async {
        return
            if generator.IsSome then
                generator.Value.Generate config
                [ CommandResponse.generatorSetup serialize generator.Value.Data ]
            else [ CommandResponse.error serialize "Nothing to generate" ] }

    member __.UpdateGenerator () = async {
        if generator.IsSome then
            let generator = generator.Value
            let fi = generator.File
            let lines = fileResolver fi.Name fi.Directory.FullName
            if lines.IsSome then
                let (file, directory, lines) = lines.Value
                let! compileResult = doCompile file directory lines
                match compileResult with
                | Success r ->
                    match r with
                    | CompilationResult.CompilationSuccess template ->
                        generator.UpdateTemplate template
                        return
                            [ CommandResponse.generatorSetup serialize generator.Data ]
                    | _ -> return []
                | _ -> return []
            else return []
        else return [] }

    member __.GetCompletions fileName ty (line:string) (col:int) = async {
        let template = fileTemplateMap.TryFind(fileName)
        match ty with
        | "Function" ->
            // We add the keywords to this list.
            let functions = template |> Option.map (fun t -> t.Functions |> Array.map (fun f -> { text = f.Name ; ``type`` = "function" ; description = "Call the @" + f.Name + " function" } : DTO.DTO.Suggestion)) |> defaultArg <| [||]
            return [ CommandResponse.suggestions serialize (Array.append functions keywords) ]
        | "Variable" ->
            return [ CommandResponse.suggestions serialize (template |> Option.map (fun t -> t.Variables |> Array.map (fun x -> { text = x.Name ; ``type`` = "variable" ; description = sprintf "$%s: %s" x.Name x.Text } : DTO.DTO.Suggestion)) |> defaultArg <| [||]) ]
        | "Attribute" ->
            return [ CommandResponse.suggestions serialize (template |> Option.map (fun t -> t.Attributes |> Array.map (fun x -> { text = x.Name ; ``type`` = "attribute" ; description = sprintf "%%%s: %s" x.Name x.Text } : DTO.DTO.Suggestion)) |> defaultArg <| [||]) ]
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
                if i < 0 || (line.[i] <> '$' && line.[i] <> '%' && line.[i] <> '#') then
                    return [ CommandResponse.suggestions serialize [||] ]
                else
                    let values =
                        if line.[i] = '#' then
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
                            |> Option.bind (fun t -> t.Variables |> Array.tryFind (fun v -> v.Name = actualName))
                            |> Option.map
                                (fun v ->
                                    let description = sprintf "Value for variable $%s - %s" v.Name
                                    v.Values
                                    |> Array.map (fun x -> x.Value, description x.Value, "value"))
                            |> defaultArg <| [||]
                        else
                            let actualName = String.Join("", name |> List.toArray)
                            template
                            |> Option.bind (fun t -> t.Attributes |> Array.tryFind (fun v -> v.Name = actualName))
                            |> Option.map
                                (fun v ->
                                    let description = sprintf "Value for attribute %%%s - %s" v.Name
                                    v.Values
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
                    template.Value.Functions |> Array.tryFind (fun f -> f.Name = functionName) |> Option.map (fun f -> f.File, f.StartLine)
                | (Some ('$', l)) ->
                    let variableName = String.Join("", l@forwardCharacters)
                    template.Value.Variables |> Array.tryFind (fun f -> f.Name = variableName) |> Option.map (fun f -> f.File, f.StartLine)
                | (Some ('%', l)) ->
                    let attributeName = String.Join("", l@forwardCharacters)
                    template.Value.Attributes |> Array.tryFind (fun f -> f.Name = attributeName) |> Option.map (fun f -> f.File, f.StartLine)
                | _ -> None
            if fileAndLine.IsSome then
                let (file, line) = fileAndLine.Value
                return [ CommandResponse.navigate serialize { FileName = file ; LineNumber = line ; Location = 1 } ]
            else
                return [ CommandResponse.error serialize "Cannot find symbol" ] }

    member __.Navigate (file:SourceFilePath) ty name = async {
        let fi = Path.GetFullPath file |> FileInfo
        let lines = fileResolver fi.Name fi.Directory.FullName
        if lines |> Option.isSome then
            let (file, directory, lines) = lines.Value
            let! compileResult = doCompile file directory lines
            return
                match compileResult with
                | Failure e -> [CommandResponse.error serialize e]
                | Success (compilationResult) ->
                    match compilationResult with
                    | CompilationResult.CompilationFailure(errors) ->
                        [ CommandResponse.error serialize "File had compilation errors" ]
                    | CompilationResult.CompilationSuccess template ->
                        let errors = [||]
                        let f =
                            match ty with
                            | "Function" -> template.Functions |> Array.tryFind (fun fn -> fn.Name = name) |> Option.map (fun fn -> fn.File, fn.StartLine)
                            | "Variable" -> template.Variables |> Array.tryFind (fun fn -> fn.Name = name) |> Option.map (fun fn -> fn.File, fn.StartLine)
                            | "Attribute" -> template.Attributes |> Array.tryFind (fun fn -> fn.Name = name) |> Option.map (fun fn -> fn.File, fn.StartLine)
                            | _ -> failwith "Internal error"
                        if f |> Option.isNone then
                            [ CommandResponse.error serialize "Function not found" ]
                        else
                            let (f, l) = f.Value
                            [ CommandResponse.navigate serialize { FileName = f ; LineNumber = l ; Location = 1 } ]
        else
            return [CommandResponse.error serialize "File not found"] }
