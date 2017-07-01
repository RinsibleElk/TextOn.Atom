namespace TextOn.ArgParser

open System
open System.IO
open System.Reflection
open FSharp.Reflection
open FSharp.Quotations
open FSharp.Quotations.Patterns

type internal ColoredText =
    | Plain of string
    | Colored of ConsoleColor * string
    | Cons of ColoredText list
    with
        override this.ToString() =
            match this with
            | Plain(s) -> s
            | Colored(_, s) -> s
            | Cons(li) -> String.Join("", li |> List.map (fun x -> x.ToString()))
        member this.OutputToConsole() =
            match this with
            | Plain(s) -> printf "%s" s
            | Colored(c, s) -> cprintf c "%s" s
            | Cons(li) -> li |> List.iter (fun c -> c.OutputToConsole())

[<AutoOpen>]
module internal ColoredTextUtils =
    let (++) (c1:ColoredText) (c2:ColoredText) =
        match (c1, c2) with
        | (Cons(l1),(Cons(l2))) -> Cons(l1@l2)
        | (_,Cons(l2)) -> Cons(c1::l2)
        | (Cons(l1),_) -> Cons(l1@[c2])
        | _ -> Cons([c1;c2])
    let (+<+) (s:string) (c:ColoredText) =
        (Plain(s)) ++ c
    let (+>+) (c:ColoredText) (s:string) =
        c ++ (Plain(s))
    let (>*>) (s:string) (c:ConsoleColor) =
        Colored(c, s)

/// Add a description to an argument.
[<Sealed>]
type ArgDescriptionAttribute(description:string) =
    inherit Attribute()
    member __.Description = description

/// Add an acceptable range to an argument.
[<Sealed>]
type ArgRangeAttribute(minValue:obj, maxValue:obj) =
    inherit Attribute()
    member __.MinValue = minValue
    member __.MaxValue = maxValue
    member __.PrintRange() = sprintf "[%A - %A]" minValue maxValue

module internal ReflectionUtils =
    let rec invokeStatic e t p =
        match e with
        | Patterns.Lambda(_, e) -> invokeStatic e t p
        | Patterns.Call(_, mi, _) -> mi.GetGenericMethodDefinition().MakeGenericMethod(t).Invoke(null, p)
        | _ -> failwith "Dunno"
    let private boxOption<'t> (t:obj option) =
        box (t |> Option.map (unbox<'t>))
    let private boxList<'t> (t:obj list) =
        box (t |> List.map (unbox<'t>))
    let boxListGen ty (t:obj list) =
        invokeStatic <@ boxList @> [|ty|] [|t|]
    let boxOptionGen ty (t:obj option) =
        invokeStatic <@ boxOption @> [|ty|] [|t|]

[<RequireQualifiedAccess>]
module ArgParser =
    type private Primitive =
        | PrimitiveString
        | PrimitiveInt
        | PrimitiveDouble
        | PrimitiveBool
        | PrimitiveFileInfo
        | PrimitiveUnion of Type
        with
            member this.Type =
                match this with
                | PrimitiveString -> typeof<string>
                | PrimitiveInt -> typeof<int>
                | PrimitiveBool -> typeof<bool>
                | PrimitiveDouble -> typeof<float>
                | PrimitiveFileInfo -> typeof<FileInfo>
                | PrimitiveUnion ty -> ty
            override this.ToString() =
                match this with
                | PrimitiveString -> "String"
                | PrimitiveInt -> "Integer"
                | PrimitiveBool -> "Bool"
                | PrimitiveDouble -> "Float"
                | PrimitiveFileInfo -> "FileName"
                | PrimitiveUnion(t) ->
                    t
                    |> FSharpType.GetUnionCases
                    |> Array.map (fun case -> case.Name)
                    |> fun a -> String.Join("/", a)
            member this.ParseAndValidate(n, r:ArgRangeAttribute option, s) =
                match this with
                | PrimitiveString ->
                    let errorMessage =
                        r
                        |> Option.map
                            (fun r ->
                                let minValue = unbox<string> r.MinValue
                                let maxValue = unbox<string> r.MaxValue
                                if minValue > s || maxValue < s then
                                    Some (sprintf "%s - value %s is outside range [%s - %s]" n s minValue maxValue)
                                else
                                    None)
                        |> defaultArg <| None
                    if errorMessage.IsSome then Choice2Of2 errorMessage.Value
                    else Choice1Of2 (box s)
                | PrimitiveFileInfo ->
                    let fi = FileInfo s
                    let errorMessage =
                        if r.IsSome then
                            Some (sprintf "%s - FileInfo does not support ArgRange" n)
                        else
                            if fi.Exists |> not then
                                Some (sprintf "%s - file '%s' does not exist" n s)
                            else
                                None
                    if errorMessage.IsSome then Choice2Of2 errorMessage.Value
                    else Choice1Of2 (box fi)
                | PrimitiveInt ->
                    let (ok, v) = Int32.TryParse(s)
                    if (not ok) then
                        Choice2Of2 (sprintf "%s - '%s' is not a valid integer" n s)
                    else
                        let s = v
                        r
                        |> Option.map
                            (fun r ->
                                let minValue = unbox<int>(r.MinValue)
                                let maxValue = unbox<int>(r.MaxValue)
                                if minValue > s || maxValue < s then
                                    Some (sprintf "%s - value %d is outside range [%d - %d]" n s minValue maxValue)
                                else
                                    None)
                        |> defaultArg <| None
                        |> function | None -> Choice1Of2 (box v) | Some e -> Choice2Of2 e
                | PrimitiveBool ->
                    let (ok, v) = Boolean.TryParse(s)
                    if (not ok) then
                        Choice2Of2 (sprintf "%s - '%s' is not a valid bool" n s)
                    else
                        let s = v
                        r
                        |> Option.map
                            (fun r ->
                                let minValue = unbox<bool>(r.MinValue)
                                let maxValue = unbox<bool>(r.MaxValue)
                                if minValue > s || maxValue < s then
                                    Some (sprintf "%s - value %b is outside range [%b - %b]" n s minValue maxValue)
                                else
                                    None)
                        |> defaultArg <| None
                        |> function | None -> Choice1Of2 (box v) | Some e -> Choice2Of2 e
                | PrimitiveDouble ->
                    let (ok, v) = Double.TryParse(s)
                    if (not ok) then
                        Choice2Of2 (sprintf "%s - '%s' is not a valid double" n s)
                    else
                        let s = v
                        r
                        |> Option.map
                            (fun r ->
                                let minValue = unbox<double>(r.MinValue)
                                let maxValue = unbox<double>(r.MaxValue)
                                if minValue > s || maxValue < s then
                                    Some (sprintf "%s - value %f is outside range [%f - %f]" n s minValue maxValue)
                                else
                                    None)
                        |> defaultArg <| None
                        |> function | None -> Choice1Of2 (box v) | Some e -> Choice2Of2 e
                | PrimitiveUnion t ->
                    t
                    |> FSharpType.GetUnionCases
                    |> Array.tryFind (fun case -> case.Name = s)
                    |> Option.map
                        (fun case ->
                            Choice1Of2 (FSharpValue.MakeUnion(case, [||])))
                    |> defaultArg <| Choice2Of2 (sprintf "%s - '%s' is not a valid '%s'" n s t.Name)

    type private ArgParserTypeInfo =
        | ArgRequired of string * string * ArgRangeAttribute option * Primitive
        | ArgOptional of string * string * ArgRangeAttribute option * Primitive
        | ArgList of string * string * ArgRangeAttribute option * Primitive
        | ArgOptionalBool of string * string
        | ArgRecord of Type * (string * ArgParserTypeInfo)[]
        | ArgUnion of Type * (string * ArgParserTypeInfo)[]
        | ArgInvalid
        with
            member internal this.Print(i) : ColoredText =
                match this with
                | ArgRequired(arg, desc, r, ty) ->
                    let rangeString = r |> Option.map (fun r -> " " + r.PrintRange()) |> defaultArg <| ""
                    (String.Join("", [1 .. i] |> List.toArray |> Array.map (fun _ -> " "))) +<+ (arg >*> ConsoleColor.Yellow) +>+ " <argument> (" ++ (ty.ToString() >*> ConsoleColor.Green) ++ (rangeString >*> ConsoleColor.Cyan) +>+ (") : " + desc + "\n")
                | ArgOptional(arg, desc, r, ty) ->
                    let rangeString = r |> Option.map (fun r -> " " + r.PrintRange()) |> defaultArg <| ""
                    (String.Join("", [1 .. i] |> List.toArray |> Array.map (fun _ -> " "))) + "[optional]" +<+ (arg >*> ConsoleColor.Yellow) +>+ " <argument> (" ++ (ty.ToString() >*> ConsoleColor.Green) ++ (rangeString >*> ConsoleColor.Cyan) +>+ (") : " + desc + "\n")
                | ArgList(arg, desc, r, ty) ->
                    let rangeString = r |> Option.map (fun r -> " " + r.PrintRange()) |> defaultArg <| ""
                    (String.Join("", [1 .. i] |> List.toArray |> Array.map (fun _ -> " "))) + "[*]" +<+ (arg >*> ConsoleColor.Yellow) +>+ " <argument> (" ++ (ty.ToString() >*> ConsoleColor.Green) ++ (rangeString >*> ConsoleColor.Cyan) +>+ (") : " + desc + "\n")
                | ArgOptionalBool(arg, desc) ->
                    (String.Join("", [1 .. i] |> List.toArray |> Array.map (fun _ -> " "))) + "[optional]" +<+ (arg >*> ConsoleColor.Yellow) +>+ " <argument> (" ++ (PrimitiveBool.ToString() >*> ConsoleColor.Green) +>+ (") : " + desc + "\n")
                | ArgRecord(ty, info) ->
                    let a =
                        info
                        |> List.ofArray
                        |> List.map (fun (_, info) -> info.Print(i + 2) +>+ "\n")
                    seq {
                        yield (Plain (sprintf "%s{\n" (String.Join("", [1 .. i] |> List.toArray |> Array.map (fun _ -> " ")))))
                        yield! a
                        yield (Plain (sprintf "%s}\n" (String.Join("", [1 .. i] |> List.toArray |> Array.map (fun _ -> " "))))) }
                    |> List.ofSeq
                    |> Cons
                | ArgUnion(ty, info) ->
                    info
                    |> List.ofArray
                    |> List.map
                        (fun (name, info) ->
                            let nameLine = sprintf "%s| %s:\n" (String.Join("", [1 .. i] |> List.toArray |> Array.map (fun _ -> " "))) name
                            nameLine +<+ (info.Print(i + 2) +>+ "\n"))
                    |> Cons
                | ArgInvalid ->
                    (String.Join("", [1 .. i] |> List.toArray |> Array.map (fun _ -> " "))) +<+ ("Invalid\n" >*> ConsoleColor.Red)
            override this.ToString() = this.Print(0).ToString()
            member this.OutputToConsole() =
                this.Print(0).OutputToConsole()

    type private ArgParserDataInfo =
        | RequiredData of Primitive * obj
        | OptionalData of Primitive * obj
        | ListData of Primitive * obj
        | RecordData of Type * ArgParserDataInfo[]
        | UnionData of (UnionCaseInfo * ArgParserDataInfo) option
        | InvalidData of string
        with
            member this.Errors =
                match this with
                | InvalidData e -> Some e
                | RecordData(_, d) -> Some (String.Join("\n", d |> Array.map (fun a -> a.Errors) |> Array.filter Option.isSome |> Array.map Option.get))
                | _ -> None
    let private makeName (s:string) =
        s.ToCharArray()
        |> Array.mapi (fun i c -> if c >= 'A' && c <= 'Z' then (if i = 0 then "" else "-") + (Char.ToLower(c).ToString()) else c.ToString())
        |> fun a -> "--" + String.Join("", a)
    let rec private isFilledIn data =
        match data with
        | RecordData (_, r) ->
            r
            |> Array.tryFind (isFilledIn >> not)
            |> Option.isNone
        | UnionData (o) -> o.IsSome
        | InvalidData(_) -> false
        | _ -> true
    let private getOptionalArg (ty:Type) (field:PropertyInfo) description range =
        let genTy = ty.GetGenericArguments().[0]
        if genTy = typeof<FileInfo> then
            (field.Name, ArgOptional((makeName field.Name), description, range, PrimitiveFileInfo))
        else if genTy = typeof<string> then
            (field.Name, ArgOptional((makeName field.Name), description, range, PrimitiveString))
        else if genTy = typeof<int> then
            (field.Name, ArgOptional((makeName field.Name), description, range, PrimitiveInt))
        else if genTy = typeof<double> then
            (field.Name, ArgOptional((makeName field.Name), description, range, PrimitiveDouble))
        else if genTy = typeof<bool> then
            (field.Name, ArgOptionalBool((makeName field.Name), description))
        else if genTy |> FSharpType.IsUnion then
            let cases = FSharpType.GetUnionCases genTy
            let isSimple = cases |> Array.map (fun caseInfo -> caseInfo.GetFields() |> Array.isEmpty) |> Array.tryFind not |> Option.isNone
            if isSimple then
                (field.Name, ArgOptional((makeName field.Name), description, range, PrimitiveUnion(genTy)))
            else
                (field.Name, ArgInvalid)
        else
            (field.Name, ArgInvalid)
    let private getListArg (ty:Type) (field:PropertyInfo) description range =
        let genTy = ty.GetGenericArguments().[0]
        if genTy = typeof<string> then
            (field.Name, ArgList((makeName field.Name), description, range, PrimitiveString))
        else if genTy = typeof<FileInfo> then
            (field.Name, ArgList((makeName field.Name), description, range, PrimitiveFileInfo))
        else if genTy = typeof<int> then
            (field.Name, ArgList((makeName field.Name), description, range, PrimitiveInt))
        else if genTy = typeof<double> then
            (field.Name, ArgList((makeName field.Name), description, range, PrimitiveDouble))
        else if genTy = typeof<bool> then
            (field.Name, ArgList((makeName field.Name), description, range, PrimitiveBool))
        else if genTy |> FSharpType.IsUnion then
            let cases = FSharpType.GetUnionCases genTy
            let isSimple = cases |> Array.map (fun caseInfo -> caseInfo.GetFields() |> Array.isEmpty) |> Array.tryFind not |> Option.isNone
            if isSimple then
                (field.Name, ArgList((makeName field.Name), description, range, PrimitiveUnion(genTy)))
            else
                (field.Name, ArgInvalid)
        else
            (field.Name, ArgInvalid)
    let rec private getUnionArgs ty =
        ty
        |> FSharpType.GetUnionCases
        |> Array.map
            (fun caseInfo ->
                let fields = caseInfo.GetFields()
                if fields.Length <> 1 then
                    (caseInfo.Name, ArgInvalid)
                else
                    let field = fields.[0]
                    let ty = field.PropertyType
                    let description =
                        caseInfo.GetCustomAttributes(typeof<ArgDescriptionAttribute>)
                        |> Array.tryFind (fun _ -> true)
                        |> Option.map (fun a -> (a :?> ArgDescriptionAttribute).Description)
                        |> defaultArg <| caseInfo.Name
                    (description, (getArgs ty)))
        |> fun x ->
            let isInvalid = x |> Array.map snd |> Array.tryFind (function | ArgInvalid -> true | _ -> false) |> Option.isSome
            if isInvalid then ArgInvalid
            else ArgUnion(ty, x)
    and private getArgs (ty:Type) =
        if (FSharpType.IsUnion ty) then
            getUnionArgs ty
        else if (FSharpType.IsRecord ty) then
            FSharpType.GetRecordFields(ty)
            |> Array.map
                (fun field ->
                    let ty = field.PropertyType
                    let description =
                        field.GetCustomAttributes(typeof<ArgDescriptionAttribute>, false)
                        |> Seq.tryFind (fun _ -> true)
                        |> Option.map (fun a -> (a :?> ArgDescriptionAttribute).Description)
                        |> defaultArg <| field.Name
                    let range =
                        field.GetCustomAttributes(typeof<ArgRangeAttribute>, false)
                        |> Seq.tryFind (fun _ -> true)
                        |> Option.map (fun a -> a :?> ArgRangeAttribute)
                    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<_ option> then
                        getOptionalArg ty field description range
                    else if ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<_ list> then
                        getListArg ty field description range
                    else if ty |> FSharpType.IsRecord then
                        let a = getArgs ty
                        if a = ArgInvalid then (field.Name, ArgInvalid)
                        else (field.Name, a)
                    else if ty |> FSharpType.IsUnion then
                        let cases = FSharpType.GetUnionCases ty
                        let isSimple = cases |> Array.map (fun caseInfo -> caseInfo.GetFields() |> Array.isEmpty) |> Array.tryFind not |> Option.isNone
                        if isSimple then
                            (field.Name, ArgRequired((makeName field.Name), description, range, PrimitiveUnion(ty)))
                        else
                            (field.Name, getArgs ty)
                    else if ty = typeof<string> then
                        (field.Name, ArgRequired((makeName field.Name), description, range, PrimitiveString))
                    else if ty = typeof<FileInfo> then
                        (field.Name, ArgRequired((makeName field.Name), description, range, PrimitiveFileInfo))
                    else if ty = typeof<int> then
                        (field.Name, ArgRequired((makeName field.Name), description, range, PrimitiveInt))
                    else if ty = typeof<double> then
                        (field.Name, ArgRequired((makeName field.Name), description, range, PrimitiveDouble))
                    else if ty = typeof<bool> then
                        (field.Name, ArgRequired((makeName field.Name), description, range, PrimitiveBool))
                    else
                        (field.Name, ArgInvalid))
            |> fun x ->
                let isInvalid = x |> Array.map snd |> Array.tryFind (function | ArgInvalid -> true | _ -> false) |> Option.isSome
                if isInvalid then ArgInvalid
                else ArgRecord(ty, x)
        else
            ArgInvalid

    let rec private doParse typeInfo (args:string[]) : (string[] * ArgParserDataInfo) =
        match typeInfo with
        | ArgRequired(matchString, _, r, ty) ->
            let i = args |> Array.tryFindIndex (fun x -> x = matchString)
            if i |> Option.isNone then
                (args, InvalidData (sprintf "%s - Not supplied" matchString))
            else if i.Value = args.Length - 1 then
                ((args |> Array.take (args.Length - 1)), InvalidData (sprintf "%s - Missing argument" matchString))
            else
                let v = args.[i.Value + 1]
                let data =
                    match (ty.ParseAndValidate(matchString, r, v)) with
                    | Choice1Of2 o -> RequiredData(ty, o)
                    | Choice2Of2 e -> InvalidData e
                ((Array.append (args |> Array.take i.Value) (args |> Array.skip (i.Value + 2))), data)
        | ArgOptional(matchString, _, r, ty) ->
            let i = args |> Array.tryFindIndex (fun x -> x = matchString)
            if i |> Option.isNone then
                (args, OptionalData(ty, box None))
            else if i.Value = args.Length - 1 then
                ((args |> Array.take (args.Length - 1)), InvalidData (sprintf "%s - Missing argument" matchString))
            else
                let v = args.[i.Value + 1]
                let data =
                    match (ty.ParseAndValidate(matchString, r, v)) with
                    | Choice1Of2 o -> OptionalData(ty, (o |> Some |> ReflectionUtils.boxOptionGen ty.Type))
                    | Choice2Of2 e -> InvalidData e
                ((Array.append (args |> Array.take i.Value) (args |> Array.skip (i.Value + 2))), data)
        | ArgList(matchString, _, r, ty) ->
            let mutable li = []
            let mutable anymore = true
            let mutable error = null
            let mutable newArgs = args
            while anymore do
                let i = newArgs |> Array.tryFindIndex (fun x -> x = matchString)
                if i |> Option.isNone then
                    anymore <- false
                else if i.Value = newArgs.Length - 1 then
                    error <- sprintf "%s - Missing argument" matchString
                    anymore <- false
                else
                    let v = newArgs.[i.Value + 1]
                    match (ty.ParseAndValidate(matchString, r, v)) with
                    | Choice1Of2 o ->
                        li <- o::li
                    | Choice2Of2 e ->
                        error <- e
                        anymore <- false
                    newArgs <- Array.append (newArgs |> Array.take i.Value) (newArgs |> Array.skip (i.Value + 2))
            if error |> isNull |> not then
                (newArgs, InvalidData(error))
            else
                (newArgs, ListData(ty, (li |> List.rev |> ReflectionUtils.boxListGen ty.Type)))
        | ArgOptionalBool(matchString, _) ->
            let i = args |> Array.tryFindIndex (fun x -> x = matchString)
            if i |> Option.isNone then
                (args, OptionalData(PrimitiveBool, box None))
            else
                ((Array.append (args |> Array.take i.Value) (args |> Array.skip (i.Value + 1))), OptionalData(PrimitiveBool, (box (Some true))))
        | ArgRecord(ty, fields) ->
            let (args, data) =
                fields
                |> Array.fold
                    (fun (args, output) (_, inner) ->
                        let (newArgs, newOutput) = doParse inner args
                        (newArgs, newOutput::output))
                    (args, [])
            (args, RecordData(ty, (data |> List.rev |> List.toArray)))
        | ArgUnion(ty, fields) ->
            let data =
                fields
                |> Array.zip (ty |> FSharpType.GetUnionCases)
                |> Array.map
                    (fun (unionCaseInfo, (_, inner)) ->
                        let (newArgs, output) = doParse inner args
                        (unionCaseInfo, newArgs, output))
            let validData = data |> Array.tryFind (fun (_, _, a) -> isFilledIn a)
            if validData.IsSome then
                let (unionCaseInfo, args, data) = validData.Value
                (args, UnionData(Some (unionCaseInfo, data)))
            else

                (args, InvalidData(String.Join("\n", data |> Array.map (fun (_, _, a) -> a.Errors) |> Array.filter Option.isSome |> Array.map Option.get)))
        | ArgInvalid -> failwith "Internal error"

    let rec private buildType data =
        match data with
        | ListData(_, s)
        | RequiredData(_, s)
        | OptionalData(_, s) -> s
        | RecordData(ty, data) ->
            FSharpValue.MakeRecord(ty, data |> Array.map buildType)
        | UnionData(o) ->
            let (case, data) = o.Value
            FSharpValue.MakeUnion(case, [|(buildType data)|])
        | InvalidData(s) -> failwith "Internal error"

    /// Parse command line arguments into a record.
    let internal parseOrError<'r> args =
        let info = getArgs (typeof<'r>)
        match info with
        | ArgInvalid ->
            Choice2Of2 ("Not a valid ArgParser type" >*> ConsoleColor.Red)
        | _ ->
            let help = args |> Array.tryFind (fun x -> x = "--help")
            if help.IsSome then
                Choice2Of2 ("Usage:\n" +<+ (info.Print(0)))
            else
                let (args, data) = doParse info args
                if (not (isFilledIn data)) then
                    Choice2Of2 (((sprintf "%s\n" (data.Errors |> Option.get)) >*> ConsoleColor.Red) ++ (info.Print(0)))
                else if args |> Array.isEmpty |> not then
                    Choice2Of2 (((sprintf "Extra args: %A\n" args) >*> ConsoleColor.Blue) ++ (info.Print(0)))
                else
                    Choice1Of2 (unbox<'r>(buildType data))

    /// Parse the arguments type, outputting errors to standard error.
    let tryParse<'r> args =
        match (parseOrError<'r> args) with
        | Choice1Of2 o -> Some o
        | Choice2Of2 s ->
            s.OutputToConsole()
            None

    /// Parse the arguments type, outputting errors to standard error and throwing if there is an error.
    let parse<'r> args =
        match (parseOrError<'r> args) with
        | Choice1Of2 o -> o
        | Choice2Of2 s ->
            s.OutputToConsole()
            failwith "Invalid arguments"
