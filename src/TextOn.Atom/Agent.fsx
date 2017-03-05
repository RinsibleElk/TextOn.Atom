#r "bin/Debug/TextOn.Atom.exe"
open TextOn.Atom
open System
open System.IO

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
let tokenizer       = categorizer |> Agent.map (time (List.map (Tokenizer.tokenize)))
let parser          = tokenizer |> Agent.map (time (List.map Parser.parse))
let compiler        = parser |> Agent.map (time Compiler.compile)
