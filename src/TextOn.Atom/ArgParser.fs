﻿namespace TextOn.Atom

open System
open System.IO
open FSharp.Reflection
open FSharp.Quotations
open FSharp.Quotations.Patterns

/// Add a description to an argument.
[<Sealed>]
type ArgDescriptionAttribute(description:string) =
    inherit Attribute()
    member __.Description = description

/// Base class for arg range.
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
        | PrimitiveUnion of Type
        with
            member this.Type =
                match this with
                | PrimitiveString -> typeof<string>
                | PrimitiveInt -> typeof<int>
                | PrimitiveBool -> typeof<bool>
                | PrimitiveDouble -> typeof<float>
                | PrimitiveUnion ty -> ty
            override this.ToString() =
                match this with
                | PrimitiveString -> "String"
                | PrimitiveInt -> "Int"
                | PrimitiveBool -> "Bool"
                | PrimitiveDouble -> "Double"
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
            member private this.Print(i) =
                match this with
                | ArgRequired(arg, desc, r, ty) ->
                    let rangeString = r |> Option.map (fun r -> " " + r.PrintRange()) |> defaultArg <| ""
                    sprintf "%s%s <argument> (%s%s) : %s\n" (String.Join("", [1 .. i] |> List.toArray |> Array.map (fun _ -> " "))) arg (ty.ToString()) rangeString desc
                | ArgOptional(arg, desc, r, ty) ->
                    let rangeString = r |> Option.map (fun r -> " " + r.PrintRange()) |> defaultArg <| ""
                    sprintf "%s[optional] %s <argument> (%s%s) : %s\n" (String.Join("", [1 .. i] |> List.toArray |> Array.map (fun _ -> " "))) arg (ty.ToString()) rangeString desc
                | ArgList(arg, desc, r, ty) ->
                    let rangeString = r |> Option.map (fun r -> " " + r.PrintRange()) |> defaultArg <| ""
                    sprintf "%s[*] %s <argument> (%s%s) : %s\n" (String.Join("", [1 .. i] |> List.toArray |> Array.map (fun _ -> " "))) arg (ty.ToString()) rangeString desc
                | ArgOptionalBool(arg, desc) ->
                    sprintf "%s[optional] %s (%s) : %s\n" (String.Join("", [1 .. i] |> List.toArray |> Array.map (fun _ -> " "))) arg (PrimitiveBool.ToString()) desc
                | ArgRecord(ty, info) ->
                    let s = sprintf "%s{\n" (String.Join("", [1 .. i] |> List.toArray |> Array.map (fun _ -> " ")))
                    let a =
                        info
                        |> Array.fold
                            (fun a (name, info) ->
                                let infoLine = info.Print(i + 2)
                                (a + infoLine))
                            ""
                    let e = sprintf "%s}\n" (String.Join("", [1 .. i] |> List.toArray |> Array.map (fun _ -> " ")))
                    s + a + e
                | ArgUnion(ty, info) ->
                    info
                    |> Array.fold
                        (fun a (name, info) ->
                            let nameLine = sprintf "%s| %s:\n" (String.Join("", [1 .. i] |> List.toArray |> Array.map (fun _ -> " "))) name
                            let infoLine = info.Print(i + 2)
                            (a + nameLine + infoLine))
                        ""
                | ArgInvalid ->
                    sprintf "%sInvalid\n" (String.Join("", [1 .. i] |> List.toArray |> Array.map (fun _ -> " ")))
            override this.ToString() = this.Print(0)

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
                | UnionData(o) -> o |> Option.map (fun (_,s) -> s.Errors) |> Option.bind id
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
    let rec private getArgs (ty:Type) =
        if (FSharpType.IsUnion ty) then
            FSharpType.GetUnionCases(ty)
            |> Array.map
                (fun caseInfo ->
                    let fields = caseInfo.GetFields()
                    if fields.Length <> 1 then
                        (caseInfo.Name, ArgInvalid)
                    else
                        let field = fields.[0]
                        let ty = field.PropertyType
                        (caseInfo.Name, (getArgs ty)))
            |> fun x ->
                let isInvalid = x |> Array.map snd |> Array.tryFind (function | ArgInvalid -> true | _ -> false) |> Option.isSome
                if isInvalid then ArgInvalid
                else ArgUnion(ty, x)
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
                        let genTy = ty.GetGenericArguments().[0]
                        if genTy = typeof<string> then
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
                    else if ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<_ list> then
                        let genTy = ty.GetGenericArguments().[0]
                        if genTy = typeof<string> then
                            (field.Name, ArgList((makeName field.Name), description, range, PrimitiveString))
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
                ((Array.append (args |> Array.take i.Value) (args |> Array.skip (i.Value + 2))), OptionalData(ty, Some v))
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
                |> Array.tryFind (fun (_, _, a) -> isFilledIn a)
            if data.IsSome then
                let (unionCaseInfo, args, data) = data.Value
                (args, UnionData(Some (unionCaseInfo, data)))
            else
                (args, UnionData(None))
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
        | InvalidData(s) -> failwith ""

    /// Parse command line arguments into a record.
    let parse<'r> args =
        let info = getArgs (typeof<'r>)
        match info with
        | ArgInvalid ->
            failwith "Not a valid ArgParser type"
        | _ ->
            let help = args |> Array.tryFind (fun x -> x = "--help")
            if help.IsSome then
                eprintfn "Usage:"
                eprintfn "%s" (info.ToString())
                None
            else
                let (args, data) = doParse info args
                if (not (isFilledIn data)) then
                    eprintfn "%s" (data.Errors |> Option.get)
                    eprintfn "%s" (info.ToString())
                    None
                else if args |> Array.isEmpty |> not then
                    eprintfn "%s" (info.ToString())
                    eprintfn "Extra args: %A" args
                    None
                else
                    (Some (unbox<'r>(buildType data)))
