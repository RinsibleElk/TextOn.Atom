#I __SOURCE_DIRECTORY__
#r "bin/Debug/TextOn.Atom.exe"
open TextOn.Atom

let a =
    {
        Category = PreprocessorError
        Index = 0
        File = "example.texton"
        StartLine = 1
        EndLine = 1
        Lines =
            Seq.singleton
                {
                    TopLevelFileLineNumber = 1
                    CurrentFileLineNumber = 1
                    CurrentFile = "example.texton"
                    Contents =
                        Error {
                            StartLocation = 1
                            EndLocation = 5
                            ErrorText = "Some error text" }
            }
    }
let b =
    {
        Category = PreprocessorError
        Index = 0
        File = "example.texton"
        StartLine = 1
        EndLine = 1
        Lines =
            Seq.singleton
                {
                    TopLevelFileLineNumber = 1
                    CurrentFileLineNumber = 1
                    CurrentFile = "example.texton"
                    Contents =
                        Error {
                            StartLocation = 1
                            EndLocation = 5
                            ErrorText = "Some error text" }
            }
    }



