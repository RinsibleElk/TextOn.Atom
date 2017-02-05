﻿#r "bin/Debug/TextOn.Atom.exe"
open TextOn.Atom
open System
open System.IO
open System.Text.RegularExpressions

type Message<'a,'b> =
    | Quit
    | Connect of ('b -> unit)
    | NewData of 'a
    | Fetch of 'b option AsyncReplyChannel
type Agent<'a,'b> = Message<'a,'b> MailboxProcessor
[<RequireQualifiedAccess>]
module Agent =
    let map (f:'a -> 'b) (node:Agent<_,'a>) =
        let agent =
            Agent.Start
                (fun inbox ->
                    let mutable b = None
                    let mutable listeners = []
                    let rec loop() =
                        async {
                            let! msg = inbox.Receive()
                            match msg with
                            | Quit -> return ()
                            | Connect f ->
                                listeners <- f::listeners
                                return! loop()
                            | NewData a ->
                                b <- Some (f a)
                                listeners
                                |> List.iter
                                    (fun f ->
                                        f b.Value)
                                return! loop()
                            | Fetch(reply) ->
                                reply.Reply(b)
                                return! loop() }
                    loop())
        node.Post(Connect(NewData >> agent.Post))
        agent
    let map2 (f:'a -> 'b -> 'c) (node1:Agent<_,'a>) (node2:Agent<_,'b>) =
        let nodeA = node1 |> map Choice1Of2
        let nodeB = node2 |> map Choice2Of2
        let agent =
            Agent.Start
                (fun inbox ->
                    let mutable a = None
                    let mutable b = None
                    let mutable c = None
                    let mutable listeners = []
                    let rec loop() =
                        async {
                            let! msg = inbox.Receive()
                            match msg with
                            | Quit -> return ()
                            | Connect f ->
                                listeners <- f::listeners
                                return! loop()
                            | NewData data ->
                                match data with
                                | Choice1Of2 x ->
                                    a <- Some x
                                    if b.IsSome then
                                        c <- Some (f x b.Value)
                                        listeners |> List.iter (fun f -> f c.Value)
                                | Choice2Of2 x ->
                                    b <- Some x
                                    if a.IsSome then
                                        c <- Some (f a.Value x)
                                        listeners |> List.iter (fun f -> f c.Value)
                                return! loop()
                            | Fetch(reply) ->
                                reply.Reply(c)
                                return! loop() }
                    loop())
        nodeA.Post(Connect(NewData >> agent.Post))
        nodeB.Post(Connect(NewData >> agent.Post))
        agent
    let source() =
        Agent.Start
            (fun inbox ->
                let mutable b = None
                let mutable listeners = []
                let rec loop() =
                    async {
                        let! msg = inbox.Receive()
                        match msg with
                        | Quit -> return ()
                        | Connect f ->
                            listeners <- f::listeners
                            return! loop()
                        | NewData a ->
                            b <- Some a
                            listeners
                            |> List.iter
                                (fun f ->
                                    f b.Value)
                            return! loop()
                        | Fetch(reply) ->
                            reply.Reply(b)
                            return! loop() }
                loop())
    let fetch (agent:Agent<_,_>) =
        agent.PostAndReply(Fetch)
    let iter f (agent:Agent<_,_>) =
        agent.Post(Connect(f))
    let post data (agent:Agent<_,_>) =
        agent.Post(NewData(data))

// Set up pipeline.
let time f (li,a) =
    let sw = System.Diagnostics.Stopwatch()
    sw.Start()
    let r = f a
    sw.Stop()
    (sw.Elapsed :: li, r)
let source          = Agent.source()
let preprocessor    = source |> Agent.map (time (fun (a,b,c) -> Preprocessor.preprocess Preprocessor.realFileResolver a b c))
let stripper        = preprocessor |> Agent.map (time CommentStripper.stripComments)
let categorizer     = stripper |> Agent.map (time LineCategorizer.categorize)
let tokenizer       =
    categorizer
    |> Agent.map
        (time
            (fun s ->
                s
                |> List.map (Tokenizer.tokenize)
                |> List.toArray))
let parser          = tokenizer |> Agent.map (time (Array.map Parser.parse))
//let compiler        = parser |> Agent.map (time Compiler.compile)

// Example data.
let filename =
    [
        @"D:\NodeJs\TextOn.Atom\examples\original\sixt.texton"
        @"/Users/Oliver/Projects/TextOn.Atom/TextOn.Atom/examples/original/sixt.texton"
    ]
    |> List.tryFind File.Exists
    |> Option.get
let f               = FileInfo(filename)
let directory       = f.Directory.FullName |> Some
let file            = f.Name
let lines           = f.FullName |> File.ReadAllLines |> List.ofArray
let mutable count   = 0
let stopwatch       = System.Diagnostics.Stopwatch()
parser.Post(
    Connect
        (fun (li,_) ->
            stopwatch.Stop()
            count <- count + 1
            printfn "Done %A for %d" (stopwatch.Elapsed) count
            li
            |> List.rev
            |> List.iter (printfn "%A")))
stopwatch.Start()
source |> Agent.post ([], (file,directory,lines))

let s =
    tokenizer
    |> Agent.fetch
    |> Option.get
    |> snd

let tokens =
    @"@func @stycke20160823_6
{
  @choice {
    Oavsett hur du {planerar|lägger} upp din {vistelse|resa} har vi garanterat
    en hyrbil i $MÄRKE för dig.
    Alla resor ser olika ut. Oavsett om du är här {för en längre eller kortare tid|för en längre eller kortare period|för en längre eller kortare tidsperiod|för en lång eller kort tid|för en längre eller kortare tid|för en lång eller kort tidsperiod} så har vi en {modell|bilmodell|hyrbilsslösning} för dig.
    Vi har en bilmodell för alla {slags|typer av} resor. Oavsett om du ska stanna {för en längre eller kortare tid|för en längre eller kortare period|för en längre eller kortare tidsperiod|för en lång eller kort tid|för en längre eller kortare tid|för en lång eller kort tidsperiod} {tar vi fram|hittar vi|finner vi} en lösning för dig.
    Vi på Sixt kan ta fram en lösning oavsett om du behöver en hyrbil i $MÄRKE för
    {för en längre eller kortare tid|för en längre eller kortare period|för en längre eller kortare tidsperiod|för en lång eller kort tid|för en längre eller kortare tid|för en lång eller kort tidsperiod}.
  }
}"
    |> fun s -> s.Split([|'\n'|], StringSplitOptions.RemoveEmptyEntries)
    |> List.ofArray
    |> Preprocessor.preprocess (fun _ _ -> None) "example.texton" None
    |> CommentStripper.stripComments
    |> LineCategorizer.categorize
    |> List.head
    |> Tokenizer.tokenize
    |> Parser.parse
