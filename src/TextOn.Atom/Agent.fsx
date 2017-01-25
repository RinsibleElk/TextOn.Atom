#r "bin/Debug/TextOn.Atom.exe"
open TextOn.Atom
open System
open System.IO
open System.Text.RegularExpressions

type Message<'a,'b> =
    | Quit
    | RegisterListener of ('b -> unit)
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
                            | RegisterListener f ->
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
        node.Post(RegisterListener(NewData >> agent.Post))
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
                        | RegisterListener f ->
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
        agent.Post(RegisterListener(f))
    let newData data (agent:Agent<_,_>) =
        agent.Post(NewData(data))
let source          = Agent.source()
let preprocessor    = source |> Agent.map (fun (a,b,c:string seq) -> Preprocessor.preprocess Preprocessor.realFileResolver a b c)
let stripper        = preprocessor |> Agent.map CommentStripper.stripComments
let categorizer     = stripper |> Agent.map LineCategorizer.categorize
let tokenizer       = categorizer |> Agent.map (fun s -> s |> Seq.map (fun x -> async { return Tokenizer.tokenize x }) |> Async.Parallel |> Async.RunSynchronously)
let printer         = tokenizer |> Agent.iter (printfn "%A")
let post a b c      = source.Post(NewData(a,b,c))
let f = FileInfo(@"D:\NodeJs\TextOn.Atom\examples\example.texton")
let directory = f.Directory.FullName |> Some
let file = f.Name
let lines = f.FullName |> File.ReadAllLines |> Seq.ofArray
source |> Agent.newData (file,directory,lines)
