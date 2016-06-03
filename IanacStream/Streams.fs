module Streams

open System.IO
open System
open FSharp.Data
open Types

let findStream streamFn user = 
    streamFn user
let startStream streamFn user name = 
    streamFn user name
let stopStream streamFn = 
    streamFn 

module JsonStreams = 
    type private StreamJson = JsonProvider<"./stream.json">
    let private fromJson user (json:StreamJson.Root) = 
        { User = user; StreamKey = json.StreamKey; Started = json.Started; Name = json.Name}
    let private fromJson' userFn (json:StreamJson.Root) = 
        let user = userFn json.Username |> Option.get
        fromJson user json
    let getFilename key = 
        Path.Combine("streams", (sprintf "%s.json" key))
    let startStream user name = 
        let sjson = 
            StreamJson.Root(name = name, 
                            username = user.Name, 
                            streamKey = user.StreamKey, 
                            started = DateTime.UtcNow)
        let json = sjson.JsonValue.ToString()
        IO.tryElseFalse (fun () -> 
            File.WriteAllText((getFilename sjson.StreamKey), json))
    let stopStream stream =
        IO.tryElseFalse (fun () -> 
            File.Delete((getFilename stream.StreamKey)))
    let findStream (user:User option) = 
        match user with
        | Some(u)->
            let file = FileInfo((getFilename u.StreamKey))
            match file.Exists with
            | true -> fromJson u (StreamJson.Load(file.FullName)) |> Some
            | false -> None
        | None -> None