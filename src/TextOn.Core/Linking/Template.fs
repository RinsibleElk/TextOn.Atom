namespace TextOn.Core.Linking

open TextOn.Core.Parsing
open TextOn.Core.Conditions

/// The definition of an attribute value, compiled, with meta-data.
type AttributeValue =
    {
        Value : string
        Condition : Condition
    }

/// The definition of an attribute, compiled, with meta-data.
type AttributeDefinition =
    {
        Name : string
        Text : string
        Index : int
        File : string
        StartLine : int
        EndLine : int
        AttributeDependencies : int list
        Values : AttributeValue list
    }

/// The definition of a variable value, compiled, with meta-data.
type VariableValue =
    {
        Value : string
        Condition : VariableCondition
    }

/// The definition of a variable, compiled, with meta-data.
type VariableDefinition =
    {
        Name : string
        Index : int
        File : string
        StartLine : int
        EndLine : int
        PermitsFreeValue : bool
        Text : string
        AttributeDependencies : int list
        VariableDependencies : int list
        Values : VariableValue list
    }

/// A simple definition node - within a sentence.
type SimpleDefinitionNode =
    | VariableValue of int
    | SimpleChoice of SimpleDefinitionNode list
    | SimpleSeq of SimpleDefinitionNode list
    | SimpleText of string

type DefinitionNode =
    | Sentence of (string * int * SimpleDefinitionNode)
    | ParagraphBreak of string * int
    | Choice of string * int * (DefinitionNode * Condition) list
    | Seq of string * int * (DefinitionNode * Condition) list
    | Function of string * int * int

/// The definition of a function, compiled, with meta-data.
type FunctionDefinition =
    {
        Name : string
        Index : int
        File : string
        IsPrivate : bool
        StartLine : int
        EndLine : int
        FunctionDependencies : int list
        AttributeDependencies : int list
        VariableDependencies : int list
        Tree : DefinitionNode
    }

/// The final template for a file.
type Template =
    {
        Errors : ParseError list
        Warnings : ParseError list
        Attributes : AttributeDefinition list
        Variables : VariableDefinition list
        Functions : FunctionDefinition list
    }
