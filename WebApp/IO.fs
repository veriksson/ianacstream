module IanacStream.IO

open System.IO

let tryElseFalse fn =
    try 
        fn (); true
    with
        | :? IOException -> false