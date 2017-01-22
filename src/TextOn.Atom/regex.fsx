#I __SOURCE_DIRECTORY__
#r "bin/Debug/TextOn.Atom.exe"
open TextOn.Atom

let source =
    seq [
        "#include \"D:\\NodeJs\TextOn.Atom\examples\cities.texton\""
    ]
let s = Preprocessor.preprocess Preprocessor.realFileResolver "" None source
