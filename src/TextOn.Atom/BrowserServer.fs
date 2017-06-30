namespace TextOn.Atom

open TextOn.Atom.DTO.DTO
open System
open System.IO

type BrowserStartResult =
    | BrowserCompilationFailure of CompilationError[]
    | BrowserStarted of BrowserUpdate

[<Sealed>]
type BrowserServer(file) =
    let mutable currentValue : BrowserUpdate option = None
    let mutable currentTemplate = None
    let mutable attributeValues : Map<string, string> = Map.empty
    let mutable variableValues : Map<string, string> = Map.empty
    let mutable currentSelectedFileName = None
    let mutable currentSelectedLine = None
    let mutable currentSelectedPaths : (string * int[])[] option = None
    let mutable currentSelectedPathIndex = -1

    let resetSelection() =
        currentSelectedFileName <- None
        currentSelectedLine <- None
        currentSelectedPaths <- None
        currentSelectedPathIndex <- -1

    let makeSelectedPath() =
        if currentSelectedPaths.IsNone then [||]
        else currentSelectedPaths.Value.[currentSelectedPathIndex] |> snd

    let rec collapseAt (browserNodes:BrowserNode[]) indexPath =
        match indexPath with
        | [] ->
            failwith "Cannot reach here"
        | [h] ->
            if browserNodes.Length > h then
                Some
                    (Array.append
                        (browserNodes |> Array.take h)
                        (Array.append
                            (Array.singleton { browserNodes.[h] with children = [||] ; isCollapsed = true })
                            (browserNodes |> Array.skip (min (h + 1) browserNodes.Length))))
            else
                None
        | h::t ->
            if browserNodes.Length > h then
                let innerChildren = collapseAt browserNodes.[h].children t
                innerChildren
                |> Option.map
                    (fun innerChildren ->
                        Array.append
                            (browserNodes |> Array.take h)
                            (Array.append
                                (Array.singleton { browserNodes.[h] with children = innerChildren })
                                (browserNodes |> Array.skip (min (h + 1) browserNodes.Length))))
            else
                None
    let simpleNodeMap rootFunction functionNames attributeValues variableNames variableValues currentIndexPathRev i n =
        let actualIndexPath = (i::currentIndexPathRev) |> List.rev |> List.toArray
        let text = Browser.makeText functionNames attributeValues variableNames variableValues n
        match n with
        | Sentence(file, line, simpleNode) ->
            {
                text = text
                nodeType = "text"
                rootFunction = rootFunction
                indexPath = actualIndexPath
                isCollapsible = false
                isCollapsed = true
                file = file
                line = line
                children = [||]
            }
        | ParagraphBreak(file, line) ->
            {
                text = text
                nodeType = "paragraphbreak"
                rootFunction = rootFunction
                indexPath = actualIndexPath
                isCollapsible = false
                isCollapsed = true
                file = file
                line = line
                children = [||]
            }
        | Choice(file, line, stuff) ->
            {
                text = text
                nodeType = "choice"
                rootFunction = rootFunction
                indexPath = actualIndexPath
                isCollapsible = true
                isCollapsed = true
                file = file
                line = line
                children = [||]
            }
        | Seq(file, line, stuff) ->
            {
                text = text
                nodeType = "seq"
                rootFunction = rootFunction
                indexPath = actualIndexPath
                isCollapsible = true
                isCollapsed = true
                file = file
                line = line
                children = [||]
            }
        | Function(file, line, f) ->
            {
                text = text
                nodeType = "function"
                rootFunction = rootFunction
                indexPath = actualIndexPath
                isCollapsible = true
                isCollapsed = true
                file = file
                line = line
                children = [||]
            }

    let getDetails =
        function
        | Sentence (a,b,_) -> a, b, "text"
        | Seq (a,b,_) -> a, b, "seq"
        | Function (a,b,_) -> a, b, "function"
        | Choice (a,b,_) -> a, b, "choice"
        | ParagraphBreak (a,b) -> a, b, "paragraphBreak"

    // Returns an array of the same length as the compiled nodes - if not None, then the browser node is guaranteed to be of the same type as the
    // compiled node. 
    let syncUpNodes functionNames attributeValues variableNames variableValues (compiledDefinitionNodes:CompiledDefinitionNode[]) (browserNodes:BrowserNode[]) : BrowserNode option [] =
        if compiledDefinitionNodes.Length < browserNodes.Length - 1 || compiledDefinitionNodes.Length > browserNodes.Length + 1 then
            compiledDefinitionNodes |> Array.map (fun _ -> None)
        else if (compiledDefinitionNodes.Length = browserNodes.Length) then
            // TODO
            Array.zip
                compiledDefinitionNodes
                browserNodes
            |> Array.map
                (fun (cn,bn) ->
                    let (_,_,nt) = cn |> getDetails
                    if nt = bn.nodeType then Some bn
                    else None)
        else
            // TODO
            compiledDefinitionNodes |> Array.map (fun _ -> None)

    let rec updateWithExpansion rootFunction functionNames attributeValues variableNames variableValues currentIndexPathRev i (cn,bn) =
        if bn |> Option.isNone || (not bn.Value.isCollapsible) || bn.Value.isCollapsed then
            simpleNodeMap rootFunction functionNames attributeValues variableNames variableValues currentIndexPathRev i cn
        else
            let bn = bn.Value
            let (collapsible, collapsed, children) =
                match cn with
                | Sentence _
                | ParagraphBreak _ -> (false, true, [||])
                | Choice (_, _, stuff)
                | Seq (_, _, stuff) ->
                    let filtered = stuff |> Array.filter (fun (_, c) -> ConditionEvaluator.resolvePartial attributeValues c) |> Array.map fst
                    let children = updateNodes rootFunction functionNames attributeValues variableNames variableValues filtered bn.children (i::currentIndexPathRev)
                    (true, children |> Array.isEmpty, children)
                | Function(_, _, f) ->
                    currentTemplate.Value.Functions
                    |> Array.tryFind (fun fn -> fn.Index = f)
                    |> Option.map
                        (fun fn ->
                            let children = updateNodes rootFunction functionNames attributeValues variableNames variableValues [|fn.Tree|] bn.children (i::currentIndexPathRev)
                            (true, false, children))
                    |> defaultArg <| (true, true, [||])
            let text = Browser.makeText functionNames attributeValues variableNames variableValues cn
            let file, line, nodeType = getDetails cn
            {
                text = text
                nodeType = nodeType
                rootFunction = rootFunction
                indexPath = (i::currentIndexPathRev) |> List.rev |> List.toArray
                isCollapsible = collapsible
                isCollapsed = collapsed
                file = file
                line = line
                children = children
            }

    // The template has updated - try to map the old expanded layout onto the new template as best we can.
    and updateNodes rootFunction functionNames attributeValues variableNames variableValues (compiledDefinitionNodes:CompiledDefinitionNode[]) (browserNodes:BrowserNode[]) currentIndexPathRev : BrowserNode[] =
        // Let's say there are 4 cases:
        // 1. compiledDefinitionNodes has same length as browserNodes. We just match them up.
        // 2. compiledDefinitionNodes is 1 longer. We try and guess where to splice.
        // 3. compiledDefinitionNodes is 1 shorter. We try and guess which one of the browserNodes to kill.
        // 4. They're out by more. We ditch all expansion and bail.
        Array.zip
            compiledDefinitionNodes
            (syncUpNodes functionNames attributeValues variableNames variableValues compiledDefinitionNodes browserNodes)
        |> Array.mapi (updateWithExpansion rootFunction functionNames attributeValues variableNames variableValues currentIndexPathRev)

    let rec expandAt rootFunction functionNames attributeValues variableNames variableValues (compiledDefinitionNodes:CompiledDefinitionNode[]) (browserNodes:BrowserNode[]) currentIndexPathRev (searchIndexPath:int list) : BrowserNode[] * BrowserNode[] =
        match searchIndexPath with
        | [] ->
            let newNodes =
                compiledDefinitionNodes
                |> Array.mapi (simpleNodeMap rootFunction functionNames attributeValues variableNames variableValues currentIndexPathRev)
            newNodes, newNodes
        | h::t ->
            if compiledDefinitionNodes.Length = browserNodes.Length && browserNodes.Length > h then
                let firstHalf = browserNodes |> Array.take h
                let secondHalf = browserNodes |> Array.skip (min browserNodes.Length (h + 1))
                let innerChildren, retVal =
                    let cn =
                        match compiledDefinitionNodes.[h] with
                        | Sentence _
                        | ParagraphBreak _ -> [||]
                        | Seq(_, _, nodes)
                        | Choice(_, _, nodes) ->
                            nodes
                            |> Array.filter (fun (_, condition) -> ConditionEvaluator.resolvePartial attributeValues condition)
                            |> Array.map fst
                        | Function(_, _, f) ->
                            currentTemplate.Value.Functions |> Array.tryFind (fun fn -> fn.Index = f) |> Option.map (fun fn -> [|fn.Tree|]) |> defaultArg <| [||]
                    let bn = browserNodes.[h].children
                    expandAt rootFunction functionNames attributeValues variableNames variableValues cn bn (h::currentIndexPathRev) t
                let innerNode =
                    let actualIndexPath = (h::currentIndexPathRev) |> List.rev |> List.toArray
                    let n = compiledDefinitionNodes.[h]
                    let text = Browser.makeText functionNames attributeValues variableNames variableValues n
                    match n with
                    | Sentence(file, line, simpleNode) ->
                        {
                            text = text
                            nodeType = "text"
                            rootFunction = rootFunction
                            indexPath = actualIndexPath
                            isCollapsible = false
                            isCollapsed = true
                            file = file
                            line = line
                            children = [||]
                        }
                    | ParagraphBreak(file, line) ->
                        {
                            text = text
                            nodeType = "paragraphbreak"
                            rootFunction = rootFunction
                            indexPath = actualIndexPath
                            isCollapsible = false
                            isCollapsed = true
                            file = file
                            line = line
                            children = [||]
                        }
                    | Choice(file, line, stuff) ->
                        {
                            text = text
                            nodeType = "choice"
                            rootFunction = rootFunction
                            indexPath = actualIndexPath
                            isCollapsible = true
                            isCollapsed = false
                            file = file
                            line = line
                            children = innerChildren
                        }
                    | Seq(file, line, stuff) ->
                        {
                            text = text
                            nodeType = "seq"
                            rootFunction = rootFunction
                            indexPath = actualIndexPath
                            isCollapsible = true
                            isCollapsed = false
                            file = file
                            line = line
                            children = innerChildren
                        }
                    | Function(file, line, f) ->
                        {
                            text = text
                            nodeType = "function"
                            rootFunction = rootFunction
                            indexPath = actualIndexPath
                            isCollapsible = true
                            isCollapsed = false
                            file = file
                            line = line
                            children = innerChildren
                        }
                (Array.append firstHalf (Array.append [|innerNode|] secondHalf)), retVal
            else
                [||], [||]

    let rec expandPath rootFunction functionNames attributeValues variableNames variableValues (compiledDefinitionNodes:CompiledDefinitionNode[]) (browserNodes:BrowserNode[]) currentIndexPathRev (searchIndexPath:int list) : BrowserNode[] =
        match searchIndexPath with
        | [] -> browserNodes
        | h::t ->
            let newBrowserNodes, _ = expandAt rootFunction functionNames attributeValues variableNames variableValues compiledDefinitionNodes browserNodes [] (currentIndexPathRev |> List.rev)
            let actualIndexPathRev = h::currentIndexPathRev
            expandPath rootFunction functionNames attributeValues variableNames variableValues compiledDefinitionNodes newBrowserNodes actualIndexPathRev t

    let nodeLine n =
        match n with
        | Choice (_, l, _)
        | Seq (_, l, _)
        | ParagraphBreak(_, l)
        | Sentence(_, l, _)
        | Function (_, l, _) -> l

    let rec findIndexPathsForFunction functionIndex (functionDefinitions:Map<int, CompiledFunctionDefinition>) attributeValues (nodes:(CompiledDefinitionNode * int * (int list)) list) output =
        match nodes with
        | [] -> output
        | (node, f, path)::t ->
            let newNodesAndIndexPathAdditionsOrOutputAddition =
                match node with
                | Function(_, _, i) ->
                    if i = functionIndex then Choice2Of2 (f, (path |> List.rev))
                    else
                        let fn = functionDefinitions.[i]
                        Choice1Of2 ([(fn.Tree, f, 0::path)])
                | ParagraphBreak _
                | Sentence _ -> Choice1Of2 []
                | Seq (_, _, nodes)
                | Choice (_, _, nodes) ->
                    nodes
                    |> Array.filter (fun (_, condition) -> ConditionEvaluator.resolvePartial attributeValues condition)
                    |> Array.mapi (fun i (n, _) -> (n, f, i::path))
                    |> List.ofArray
                    |> Choice1Of2
            let newNodes = match newNodesAndIndexPathAdditionsOrOutputAddition with | Choice1Of2 x -> t@x | _ -> t
            let newOutput = match newNodesAndIndexPathAdditionsOrOutputAddition with | Choice2Of2 x -> x::output | _ -> output
            findIndexPathsForFunction functionIndex functionDefinitions attributeValues newNodes newOutput

    let rec findNodeAtLineWithinFunction attributeValues line node currentPathRev =
        let returnBad, indexAndNode =
            match node with
            | Seq(_, l, nodes)
            | Choice(_, l, nodes) ->
                if l = line then (false, None)
                else
                    nodes
                    |> Array.filter (fun (_, condition) -> ConditionEvaluator.resolvePartial attributeValues condition)
                    |> Array.mapi (fun i (n, _) -> (i, n))
                    |> Array.takeWhile (snd >> nodeLine >> fun l -> l <= line)
                    |> Array.tryLast
                    |> fun o ->
                        if o.IsNone then (true, None)
                        else (false, o)
            | ParagraphBreak(_, l)
            | Function(_, l, _)
            | Sentence(_, l, _) ->
                if l = line then (false, None)
                else (true, None)
        if returnBad then None
        else if indexAndNode.IsNone then (Some (currentPathRev |> List.rev))
        else
            let (i, n) = indexAndNode.Value
            findNodeAtLineWithinFunction attributeValues line n (i::currentPathRev)

    member __.File = file |> System.IO.FileInfo
    member __.UpdateTemplate (template:CompiledTemplate) =
        currentTemplate <- Some template
        resetSelection()
    member private __.ExpandAndSelect rootFunction (selectedPath:int[]) =
        let currentTemplate = currentTemplate.Value
        let functionNames = Browser.functionNames currentTemplate
        let functionIndex = selectedPath.[0]
        let variableNames = currentTemplate.Variables |> Array.map (fun v -> v.Index, v.Name) |> Map.ofArray
        let publicFunctions = currentTemplate.Functions |> Array.filter (fun fn -> not fn.IsPrivate)
        let compiledNodes = [|publicFunctions.[functionIndex].Tree|]
        let attributeNameToIndex, attributeIndexToName = currentTemplate.Attributes |> Array.map (fun a -> a.Name, a.Index) |> fun a -> (a |> Map.ofArray |> fun m x -> m |> Map.tryFind x), (a |> Array.map (fun (x,y) -> (y,x)) |> Map.ofArray |> fun m x -> m |> Map.tryFind x)
        let variableNameToIndex, variableIndexToName = currentTemplate.Variables |> Array.map (fun a -> a.Name, a.Index) |> fun a -> (a |> Map.ofArray |> fun m x -> m |> Map.tryFind x), (a |> Array.map (fun (x,y) -> (y,x)) |> Map.ofArray |> fun m x -> m |> Map.tryFind x)
        let attributeValuesByIndex = attributeValues |> Map.toSeq |> Seq.choose (fun (n,v) -> n |> attributeNameToIndex |> Option.map (fun i -> (i,v))) |> Map.ofSeq
        let variableValuesByIndex = variableValues |> Map.toSeq |> Seq.choose (fun (n,v) -> n |> variableNameToIndex |> Option.map (fun i -> (i,v))) |> Map.ofSeq
        let rootItems = expandPath rootFunction functionNames attributeValuesByIndex variableNames variableValuesByIndex compiledNodes currentValue.Value.nodes.[selectedPath.[0]].children [] (selectedPath |> List.ofArray |> List.skip 1)
        currentValue <-
            Some
                {
                    attributes = currentValue.Value.attributes
                    variables = currentValue.Value.variables
                    nodes =
                        Array.append
                            (currentValue.Value.nodes |> Array.take functionIndex)
                            (Array.append
                                [|
                                    { currentValue.Value.nodes.[functionIndex] with children = rootItems ; isCollapsed = (rootItems.Length = 0) }
                                |]
                                (currentValue.Value.nodes |> Array.skip (min (functionIndex + 1) (currentValue.Value.nodes.Length))))
                    file = file
                    selectedPath = selectedPath |> Array.skip 1
                }
    member this.CycleThroughTo fileName line =
        if currentSelectedFileName.IsNone || currentSelectedFileName.Value <> fileName || currentSelectedLine.IsNone || currentSelectedLine.Value <> line then
            resetSelection()
            // Find which function it's in.
            let fn = currentTemplate.Value.Functions |> Array.tryFind (fun x -> x.File = fileName && x.StartLine <= line && x.EndLine >= line)
            if fn.IsSome then
                let fn = fn.Value
                let currentTemplate = currentTemplate.Value
                let attributeNameToIndex, attributeIndexToName = currentTemplate.Attributes |> Array.map (fun a -> a.Name, a.Index) |> fun a -> (a |> Map.ofArray |> fun m x -> m |> Map.tryFind x), (a |> Array.map (fun (x,y) -> (y,x)) |> Map.ofArray |> fun m x -> m |> Map.tryFind x)
                let attributeValuesByIndex = attributeValues |> Map.toSeq |> Seq.choose (fun (n,v) -> n |> attributeNameToIndex |> Option.map (fun i -> (i,v))) |> Map.ofSeq

                // Find the path within the function.
                let indexPathWithinFn = findNodeAtLineWithinFunction attributeValuesByIndex line fn.Tree [0]
                if indexPathWithinFn.IsSome then
                    let indexPathWithinFn = indexPathWithinFn.Value
                    let fnDefs = currentTemplate.Functions |> Array.map (fun fn -> fn.Index, fn) |> Map.ofArray
                    let fnIndex = fn.Index
                    let nodes =
                        currentTemplate.Functions
                        |> Array.filter (fun fn -> fn.FunctionDependencies |> Array.contains fnIndex)
                        |> Array.filter (fun fn -> not fn.IsPrivate)
                        |> Array.map (fun fn -> (fn.Tree, fn.Index, [0]))
                        |> List.ofArray
                    let starterRef = if fn.IsPrivate then [] else [(fn.Index, [])]

                    // Find all references to that function (recursively) and the paths.
                    let refsToFunction = findIndexPathsForFunction fnIndex fnDefs attributeValuesByIndex nodes starterRef
                    let publicFunctions =
                        currentTemplate.Functions
                        |> Array.filter (fun fn -> fn.IsPrivate |> not)
                        |> Array.mapi (fun i fn -> (fn.Index, (i, fn.Name)))
                        |> Map.ofArray
                    let paths =
                        refsToFunction
                        |> List.rev
                        |> List.toArray
                        |> Array.map
                            (fun (f, fPath) ->
                                let (i, n) = publicFunctions.[f]
                                (n, ((i::(fPath@indexPathWithinFn)) |> List.toArray)))
                    if paths |> Array.isEmpty then false
                    else
                        currentSelectedFileName <- Some fileName
                        currentSelectedLine <- Some line
                        currentSelectedPaths <- Some paths
                        currentSelectedPathIndex <- 0
                        let (rootFunction, indexPath) = currentSelectedPaths.Value.[currentSelectedPathIndex]
                        this.ExpandAndSelect rootFunction indexPath
                        true
                else
                    false
            else
                false
        else
            currentSelectedPathIndex <- currentSelectedPathIndex + 1
            if currentSelectedPathIndex >= (currentSelectedPaths.Value |> Array.length) then
                currentSelectedPathIndex <- 0
            let (rootFunction, indexPath) = currentSelectedPaths.Value.[currentSelectedPathIndex]
            this.ExpandAndSelect rootFunction indexPath
            true

    member __.Data =
        let currentTemplate = currentTemplate.Value
        let attributeNameToIndex, attributeIndexToName = currentTemplate.Attributes |> Array.map (fun a -> a.Name, a.Index) |> fun a -> (a |> Map.ofArray |> fun m x -> m |> Map.tryFind x), (a |> Array.map (fun (x,y) -> (y,x)) |> Map.ofArray |> fun m x -> m |> Map.tryFind x)
        attributeValues <- attributeValues |> Map.filter (fun k _ -> k |> attributeNameToIndex |> Option.isSome)
        let variableNameToIndex, variableIndexToName = currentTemplate.Variables |> Array.map (fun a -> a.Name, a.Index) |> fun a -> (a |> Map.ofArray |> fun m x -> m |> Map.tryFind x), (a |> Array.map (fun (x,y) -> (y,x)) |> Map.ofArray |> fun m x -> m |> Map.tryFind x)
        variableValues <- variableValues |> Map.filter (fun k _ -> k |> variableNameToIndex |> Option.isSome)
        let functionNames = Browser.functionNames currentTemplate
        let (attributes, variables) =
            let functionDefs = currentTemplate.Functions
            let requiredAttributes = functionDefs |> Array.collect (fun f -> f.AttributeDependencies) |> Set.ofArray |> Set.toArray |> Array.map (fun i -> currentTemplate.Attributes |> Array.find (fun a -> a.Index = i))
            let requiredVariables = functionDefs |> Array.collect (fun f -> f.VariableDependencies) |> Set.ofArray |> Set.toArray |> Array.map (fun i -> currentTemplate.Variables |> Array.find (fun a -> a.Index = i))
            Array.append
                (requiredAttributes |> Array.map Choice1Of2)
                (requiredVariables |> Array.map Choice2Of2)
            |> Array.scan
                (fun _ value ->
                    match value with
                    | Choice1Of2 att ->
                        let data =
                            let attributeValuesByIndex = attributeValues |> Map.toSeq |> Seq.choose (fun (n,v) -> n |> attributeNameToIndex |> Option.map (fun i -> (i,v))) |> Map.ofSeq
                            let suggestions = att.Values |> Array.filter (fun a -> ConditionEvaluator.resolvePartial attributeValuesByIndex a.Condition) |> Array.map (fun a -> a.Value)
                            let value = attributeValuesByIndex |> Map.tryFind att.Index
                            let value =
                                if value.IsSome && (suggestions |> Array.tryFind ((=) value.Value) |> Option.isNone) then
                                    attributeValues <- attributeValues |> Map.remove att.Name
                                    None
                                else
                                    value
                            let newValue, newSuggestions =
                                if value.IsNone && suggestions.Length > 0 then
                                    "", (""::(suggestions |> List.ofArray) |> List.toArray)
                                else if value.IsSome then
                                    // Put the current value at the front.
                                    value.Value, (value.Value::(suggestions |> List.ofArray |> List.filter ((<>) value.Value))) |> List.toArray
                                else
                                    "", [|""|]
                            {
                                name = att.Name
                                value = newValue
                                items = newSuggestions
                                text = att.Text
                                permitsFreeValue = false
                            }
                        (Some (Choice1Of2 data))
                    | Choice2Of2 var ->
                        let data =
                            let attributeValuesByIndex = attributeValues |> Map.toSeq |> Seq.choose (fun (n,v) -> n |> attributeNameToIndex |> Option.map (fun i -> (i,v))) |> Map.ofSeq
                            let variableValuesByIndex = variableValues |> Map.toSeq |> Seq.choose (fun (n,v) -> n |> variableNameToIndex |> Option.map (fun i -> (i,v))) |> Map.ofSeq
                            let value = variableValuesByIndex |> Map.tryFind var.Index
                            let suggestions =
                                var.Values
                                |> Array.filter (fun a -> VariableConditionEvaluator.resolvePartial attributeValuesByIndex variableValuesByIndex a.Condition)
                                |> Array.map (fun a -> a.Value)
                            let value =
                                if value.IsSome && (not var.PermitsFreeValue) && (suggestions |> Array.tryFind ((=) value.Value) |> Option.isNone) then
                                    attributeValues <- attributeValues |> Map.remove var.Name
                                    None
                                else
                                    value
                            // We remove the choice if it is invalid, or if it was system generated.
                            let value =
                                if value.IsSome && (not var.PermitsFreeValue) && (suggestions |> Array.tryFind ((=) value.Value) |> Option.isNone) then
                                    variableValues <- variableValues |> Map.remove var.Name
                                    None
                                else
                                    value
                            let newValue, newSuggestions =
                                if value.IsNone && suggestions.Length > 0 then
                                    "", (""::(suggestions |> List.ofArray) |> List.toArray)
                                else if value.IsSome then
                                    // Put the current value at the front.
                                    value.Value, (value.Value::(suggestions |> List.ofArray |> List.filter ((<>) value.Value))) |> List.toArray
                                else
                                    "", [|""|]
                            {
                                name = var.Name
                                value = newValue
                                items = newSuggestions
                                text = var.Text
                                permitsFreeValue = var.PermitsFreeValue
                            }
                        (Some (Choice2Of2 data)))
                    None
            |> Array.skip 1
            |> Array.map Option.get
            |> fun a ->
                let attributes = a |> Array.choose (function | Choice1Of2 a -> Some a | _ -> None)
                let variables = a |> Array.choose (function | Choice2Of2 a -> Some a | _ -> None)
                (attributes, variables)
        let variableNames = currentTemplate.Variables |> Array.map (fun v -> v.Index, v.Name) |> Map.ofArray
        let attributeValuesByIndex = attributeValues |> Map.toSeq |> Seq.choose (fun (n,v) -> n |> attributeNameToIndex |> Option.map (fun i -> (i,v))) |> Map.ofSeq
        let variableValuesByIndex = variableValues |> Map.toSeq |> Seq.choose (fun (n,v) -> n |> variableNameToIndex |> Option.map (fun i -> (i,v))) |> Map.ofSeq
        let retVal =
            {
                attributes = attributes
                variables = variables
                nodes =
                    currentTemplate.Functions
                    |> Array.filter (fun fn -> (not fn.IsPrivate))
                    |> Array.mapi
                        (fun i fn ->
                            let browserNode =
                                currentValue
                                |> Option.bind (fun browser -> browser.nodes |> Array.tryFind (fun bn -> bn.text = fn.Name))
                            if browserNode.IsNone || browserNode.Value.isCollapsed then
                                {
                                    text = fn.Name
                                    nodeType = "function"
                                    rootFunction = fn.Name
                                    indexPath = [||]
                                    isCollapsible = true
                                    isCollapsed = true
                                    file = fn.File
                                    line = fn.StartLine
                                    children = [||]
                                }
                            else
                                let browserNode = browserNode.Value
                                let children = updateNodes fn.Name functionNames attributeValuesByIndex variableNames variableValuesByIndex [|fn.Tree|] browserNode.children []
                                {
                                    text = fn.Name
                                    nodeType = "function"
                                    rootFunction = fn.Name
                                    indexPath = [||]
                                    isCollapsible = true
                                    isCollapsed = children |> Array.isEmpty
                                    file = fn.File
                                    line = fn.StartLine
                                    children = children
                                })
                file = file
                selectedPath = makeSelectedPath()
            }
        currentValue <- Some retVal
        retVal

    // This should set the new state on the parent to expanded, and return the children, having resolved some text to write.
    // It should update the current value.
    member __.ExpandAt rootFunction (indexPath:int[]) =
        if currentValue.IsNone then None
        else
            let currentTemplate = currentTemplate.Value
            let functionIndex = currentValue.Value.nodes |> Array.mapi (fun a b -> (a,b)) |> Array.tryFind (fun (_, fn) -> fn.text = rootFunction) |> Option.map fst
            if functionIndex.IsNone then None
            else
                let functionNames = Browser.functionNames currentTemplate
                let variableNames = currentTemplate.Variables |> Array.map (fun v -> v.Index, v.Name) |> Map.ofArray
                let functionIndex = functionIndex.Value
                let publicFunctions = currentTemplate.Functions |> Array.filter (fun fn -> not fn.IsPrivate)
                let compiledNodes = [|publicFunctions.[functionIndex].Tree|]
                let attributeNameToIndex, attributeIndexToName = currentTemplate.Attributes |> Array.map (fun a -> a.Name, a.Index) |> fun a -> (a |> Map.ofArray |> fun m x -> m |> Map.tryFind x), (a |> Array.map (fun (x,y) -> (y,x)) |> Map.ofArray |> fun m x -> m |> Map.tryFind x)
                let variableNameToIndex, variableIndexToName = currentTemplate.Variables |> Array.map (fun a -> a.Name, a.Index) |> fun a -> (a |> Map.ofArray |> fun m x -> m |> Map.tryFind x), (a |> Array.map (fun (x,y) -> (y,x)) |> Map.ofArray |> fun m x -> m |> Map.tryFind x)
                let attributeValuesByIndex = attributeValues |> Map.toSeq |> Seq.choose (fun (n,v) -> n |> attributeNameToIndex |> Option.map (fun i -> (i,v))) |> Map.ofSeq
                let variableValuesByIndex = variableValues |> Map.toSeq |> Seq.choose (fun (n,v) -> n |> variableNameToIndex |> Option.map (fun i -> (i,v))) |> Map.ofSeq
                let (rootItems, newItems) =
                    expandAt rootFunction functionNames attributeValuesByIndex variableNames variableValuesByIndex compiledNodes currentValue.Value.nodes.[functionIndex].children [] (indexPath |> List.ofArray)
                currentValue <-
                    Some
                        {
                            attributes = currentValue.Value.attributes
                            variables = currentValue.Value.variables
                            nodes =
                                Array.append
                                    (currentValue.Value.nodes |> Array.take functionIndex)
                                    (Array.append
                                        [|
                                            { currentValue.Value.nodes.[functionIndex] with children = rootItems ; isCollapsed = (rootItems.Length = 0) }
                                        |]
                                        (currentValue.Value.nodes |> Array.skip (min (functionIndex + 1) (currentValue.Value.nodes.Length))))
                            file = file
                            selectedPath = [||]
                        }
                Some
                    {
                        newItems = newItems
                    }

    member __.CollapseAt rootFunction (indexPath:int[]) =
        if currentValue.IsNone then None
        else
            let functionIndex = currentValue.Value.nodes |> Array.mapi (fun i n -> (i,n)) |> Array.tryFind (fun (_, fn) -> fn.text = rootFunction) |> Option.map fst
            if functionIndex.IsNone then None
            else
                let children = collapseAt currentValue.Value.nodes (functionIndex.Value :: (indexPath |> List.ofArray))
                if children |> Option.isSome then
                    currentValue <-
                        Some
                            {
                                attributes = currentValue.Value.attributes
                                variables = currentValue.Value.variables
                                nodes = children.Value
                                file = file
                                selectedPath = [||]
                            }
                    Some true
                else
                    None

    member __.SetValue ty name value =
        resetSelection()
        if ty = "Variable" then
            if value |> String.IsNullOrEmpty then
                variableValues <- variableValues |> Map.remove name
            else
                variableValues <- variableValues |> Map.add name value
        else
            if value |> String.IsNullOrEmpty then
                attributeValues <- attributeValues |> Map.remove name
            else
                attributeValues <- attributeValues |> Map.add name value
