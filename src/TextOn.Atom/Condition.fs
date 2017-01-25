namespace TextOn.Atom

/// Unique identifier for an attribute in some cache we prepare.
type AttributeIdentity = int

/// Represents a tree of decisions.
type Condition =
    | True
    | Both of Condition * Condition
    | Either of Condition * Condition
    | AreEqual of AttributeIdentity * string
    | AreNotEqual of AttributeIdentity * string

/// Resolve a condition from a cache of values.
[<RequireQualifiedAccess>]
module ConditionResolver =
    /// Resolve a condition's value.
    let rec resolve (cache:Map<int, string>) condition : bool =
        match condition with
        | True -> true
        | Both(c1, c2) -> (c1 |> resolve cache) && (c2 |> resolve cache)
        | Either(c1, c2) -> (c1 |> resolve cache) || (c2 |> resolve cache)
        | AreEqual(identity, value) -> (cache |> Map.find identity) = value
        | AreNotEqual(identity, value) -> (cache |> Map.find identity) <> value
