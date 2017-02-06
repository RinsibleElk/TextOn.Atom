namespace TextOn.Atom

open System
open System.IO
open FSharp.Reflection
open FSharp.Quotations
open FSharp.Quotations.Patterns

/// Add a description to an argument.
type ArgDescriptionAttribute(description:string) =
    inherit Attribute()
    member __.Description = description

module internal ReflectionUtils =
    let rec invokeStatic e t p =
        match e with
        | Patterns.Lambda(_, e) -> invokeStatic e t p
        | Patterns.Call(_, mi, _) -> mi.GetGenericMethodDefinition().MakeGenericMethod(t).Invoke(null, p)
        | _ -> failwith "Dunno"

[<RequireQualifiedAccess>]
module ArgParser =
    type private Primitive =
        | PrimitiveString
        | PrimitiveInt
        | PrimitiveDouble
        | PrimitiveBool
        | PrimitiveDateTime
        | PrimitiveUnion of Type
        with
            override this.ToString() =
                match this with
                | PrimitiveString -> "String"
                | PrimitiveInt -> "Int"
                | PrimitiveBool -> "Bool"
                | PrimitiveDouble -> "Double"
                | PrimitiveDateTime -> "DateTime"
                | PrimitiveUnion(t) ->
                    t
                    |> FSharpType.GetUnionCases
                    |> Array.map (fun case -> case.Name)
                    |> fun a -> String.Join("/", a)

    type private ArgParserTypeInfo =
        | ArgRequired of string * string * Primitive
        | ArgOptional of string * string * Primitive
        | ArgList of string * string * Primitive
        | ArgOptionalBool of string * string
        | ArgRecord of Type * (string * ArgParserTypeInfo)[]
        | ArgUnion of Type * (string * ArgParserTypeInfo)[]
        | ArgInvalid
        with
            member private this.Print(i) =
                match this with
                | ArgRequired(arg, desc, ty) ->
                    sprintf "%s%s <argument> (%s) : %s\n" (String.Join("", [1 .. i] |> List.toArray |> Array.map (fun _ -> " "))) arg (ty.ToString()) desc
                | ArgOptional(arg, desc, ty) ->
                    sprintf "%s[optional] %s <argument> (%s) : %s\n" (String.Join("", [1 .. i] |> List.toArray |> Array.map (fun _ -> " "))) arg (ty.ToString()) desc
                | ArgList(arg, desc, ty) ->
                    sprintf "%s[*] %s <argument> (%s) : %s\n" (String.Join("", [1 .. i] |> List.toArray |> Array.map (fun _ -> " "))) arg (ty.ToString()) desc
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
        | RequiredData of Primitive * string option
        | OptionalData of Primitive * string option
        | ListData of Primitive * string list
        | OptionalBoolData of bool
        | RecordData of Type * ArgParserDataInfo[]
        | UnionData of (UnionCaseInfo * ArgParserDataInfo) option
    let private makeName (s:string) =
        s.ToCharArray()
        |> Array.mapi (fun i c -> if c >= 'A' && c <= 'Z' then (if i = 0 then "" else "-") + (Char.ToLower(c).ToString()) else c.ToString())
        |> fun a -> "--" + String.Join("", a)
    let rec private isFilledIn data =
        match data with
        | RequiredData(_, o) -> o.IsSome
        | RecordData (_, r) ->
            r
            |> Array.tryFind (isFilledIn >> not)
            |> Option.isNone
        | UnionData (o) -> o.IsSome
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
                    if ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<_ option> then
                        let genTy = ty.GetGenericArguments().[0]
                        if genTy = typeof<string> then
                            (field.Name, ArgOptional((makeName field.Name), description, PrimitiveString))
                        else if genTy = typeof<int> then
                            (field.Name, ArgOptional((makeName field.Name), description, PrimitiveInt))
                        else if genTy = typeof<double> then
                            (field.Name, ArgOptional((makeName field.Name), description, PrimitiveDouble))
                        else if genTy = typeof<DateTime> then
                            (field.Name, ArgOptional((makeName field.Name), description, PrimitiveDateTime))
                        else if genTy = typeof<bool> then
                            (field.Name, ArgOptionalBool((makeName field.Name), description))
                        else if genTy |> FSharpType.IsUnion then
                            let cases = FSharpType.GetUnionCases genTy
                            let isSimple = cases |> Array.map (fun caseInfo -> caseInfo.GetFields() |> Array.isEmpty) |> Array.tryFind not |> Option.isNone
                            if isSimple then
                                (field.Name, ArgOptional((makeName field.Name), description, PrimitiveUnion(genTy)))
                            else
                                (field.Name, ArgInvalid)
                        else
                            (field.Name, ArgInvalid)
                    else if ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<_ list> then
                        let genTy = ty.GetGenericArguments().[0]
                        if genTy = typeof<string> then
                            (field.Name, ArgList((makeName field.Name), description, PrimitiveString))
                        else if genTy = typeof<int> then
                            (field.Name, ArgList((makeName field.Name), description, PrimitiveInt))
                        else if genTy = typeof<double> then
                            (field.Name, ArgList((makeName field.Name), description, PrimitiveDouble))
                        else if genTy = typeof<DateTime> then
                            (field.Name, ArgList((makeName field.Name), description, PrimitiveDateTime))
                        else if genTy = typeof<bool> then
                            (field.Name, ArgList((makeName field.Name), description, PrimitiveBool))
                        else if genTy |> FSharpType.IsUnion then
                            let cases = FSharpType.GetUnionCases genTy
                            let isSimple = cases |> Array.map (fun caseInfo -> caseInfo.GetFields() |> Array.isEmpty) |> Array.tryFind not |> Option.isNone
                            if isSimple then
                                (field.Name, ArgList((makeName field.Name), description, PrimitiveUnion(genTy)))
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
                            (field.Name, ArgRequired((makeName field.Name), description, PrimitiveUnion(ty)))
                        else
                            (field.Name, getArgs ty)
                    else if ty = typeof<string> then
                        (field.Name, ArgRequired((makeName field.Name), description, PrimitiveString))
                    else if ty = typeof<int> then
                        (field.Name, ArgRequired((makeName field.Name), description, PrimitiveInt))
                    else if ty = typeof<double> then
                        (field.Name, ArgRequired((makeName field.Name), description, PrimitiveDouble))
                    else if ty = typeof<DateTime> then
                        (field.Name, ArgRequired((makeName field.Name), description, PrimitiveDateTime))
                    else if ty = typeof<bool> then
                        (field.Name, ArgRequired((makeName field.Name), description, PrimitiveBool))
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
        | ArgRequired(matchString, _, ty) ->
            let i = args |> Array.tryFindIndex (fun x -> x = matchString)
            if i |> Option.isNone then
                (args, RequiredData(ty, None))
            else if i.Value = args.Length - 1 then
                ((args |> Array.take (args.Length - 1)), RequiredData(ty, None))
            else
                let v = args.[i.Value + 1]
                ((Array.append (args |> Array.take i.Value) (args |> Array.skip (i.Value + 2))), RequiredData(ty, Some v))
        | ArgOptional(matchString, _, ty) ->
            let i = args |> Array.tryFindIndex (fun x -> x = matchString)
            if i |> Option.isNone then
                (args, OptionalData(ty, None))
            else if i.Value = args.Length - 1 then
                ((args |> Array.take (args.Length - 1)), RequiredData(ty, None)) // cheese alert
            else
                let v = args.[i.Value + 1]
                ((Array.append (args |> Array.take i.Value) (args |> Array.skip (i.Value + 2))), OptionalData(ty, Some v))
        | ArgList(matchString, _, ty) ->
            let mutable li = []
            let mutable anymore = true
            let mutable isvalid = true
            let mutable newArgs = args
            while anymore do
                let i = newArgs |> Array.tryFindIndex (fun x -> x = matchString)
                if i |> Option.isNone then
                    anymore <- false
                else if i.Value = newArgs.Length - 1 then
                    isvalid <- false
                    anymore <- false
                else
                    let v = newArgs.[i.Value + 1]
                    newArgs <- Array.append (newArgs |> Array.take i.Value) (newArgs |> Array.skip (i.Value + 2))
                    li <- v::li
            if isvalid then
                (newArgs, ListData(ty, li |> List.rev))
            else
                (newArgs, RequiredData(ty, None)) // hack
        | ArgOptionalBool(matchString, _) ->
            let i = args |> Array.tryFindIndex (fun x -> x = matchString)
            if i |> Option.isNone then
                (args, OptionalBoolData(false))
            else
                ((Array.append (args |> Array.take i.Value) (args |> Array.skip (i.Value + 1))), OptionalBoolData(true))
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

    let private buildSimple ty s =
        match ty with
        | PrimitiveString -> (box s)
        | PrimitiveInt -> (box (Int32.Parse s))
        | PrimitiveDouble -> (box (Double.Parse s))
        | PrimitiveDateTime -> (box (DateTime.Parse s))
        | PrimitiveBool -> (box (Boolean.Parse s))
        | PrimitiveUnion ty ->
            FSharpType.GetUnionCases(ty)
            |> Array.tryFind (fun case -> case.Name = s)
            |> Option.get
            |> fun c -> FSharpValue.MakeUnion(c, [||])

    let private boxUnionOption<'t> (t:obj option) =
        box (t |> Option.map (unbox<'t>))

    let private buildSimpleOptional ty s =
        match ty with
        | PrimitiveString -> (box (Some s))
        | PrimitiveInt -> (box (Some (Int32.Parse s)))
        | PrimitiveDateTime -> (box (Some (DateTime.Parse s)))
        | PrimitiveDouble -> (box (Some (Double.Parse s)))
        | PrimitiveBool -> (box (Some (Boolean.Parse s)))
        | PrimitiveUnion ty ->
            FSharpType.GetUnionCases(ty)
            |> Array.tryFind (fun case -> case.Name = s)
            |> Option.map (fun c -> FSharpValue.MakeUnion(c, [||]))
            |> fun a -> ReflectionUtils.invokeStatic <@@ boxUnionOption @@> [|ty|] [|a|]

    let private boxUnionList<'t> (t:obj list) =
        box (t |> List.map (unbox<'t>))

    let private buildSimpleList ty s =
        match ty with
        | PrimitiveString -> (box s)
        | PrimitiveInt -> (box (s |> List.map Int32.Parse))
        | PrimitiveDateTime -> (box (s |> List.map DateTime.Parse))
        | PrimitiveDouble -> (box (s |> List.map Double.Parse))
        | PrimitiveBool -> (box (s |> List.map Boolean.Parse))
        | PrimitiveUnion ty ->
            s
            |> List.map
                (fun x ->
                    FSharpType.GetUnionCases(ty)
                    |> Array.tryFind (fun case -> case.Name = x)
                    |> Option.get
                    |> fun c -> FSharpValue.MakeUnion(c, [||]))
            |> fun a -> ReflectionUtils.invokeStatic <@@ boxUnionList @@> [|ty|] [|a|]

    let rec private buildType data =
        match data with
        | RequiredData(ty, s) ->
            buildSimple ty s.Value
        | OptionalData(ty, s) ->
            if s.IsNone then box None
            else buildSimpleOptional ty s.Value
        | OptionalBoolData(v) ->
            if v then (box (Some(true))) else box None
        | ListData(ty, li) ->
            li |> buildSimpleList ty
        | RecordData(ty, data) ->
            FSharpValue.MakeRecord(ty, data |> Array.map buildType)
        | UnionData(o) ->
            let (case, data) = o.Value
            FSharpValue.MakeUnion(case, [|(buildType data)|])

    /// Parse command line arguments into a record.
    let parse<'r>(args) =
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
                    eprintfn "%s" (info.ToString())
                    None
                else if args |> Array.isEmpty |> not then
                    eprintfn "%s" (info.ToString())
                    eprintfn "Extra args: %A" args
                    None
                else
                    (Some (unbox<'r>(buildType data)))
