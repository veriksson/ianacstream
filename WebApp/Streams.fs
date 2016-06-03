module Streams
// Defines a set of operations 
// That works on streams
// The actual implementation is up to the library, in this case JsonStreams
open Types

type StreamService = 
    {
        findStream:     User    -> Stream option
        findStream':    string  -> Stream option
        startStream:    string  -> string -> string -> bool
        stopStream:     Stream  -> bool
        updateStream:   Stream  -> bool
    }

module InMemoryStreams =
    open System
    let mutable streams: Stream list = []

    let findStream' streamKey = 
        List.tryFind (fun s -> s.StreamKey = streamKey) streams
    let findStream (user:User) =
        findStream' user.StreamKey 
    let startStream streamKey streamName userName = 
        let stream = { Username = userName
                       StreamKey = streamKey 
                       Started = DateTime.UtcNow 
                       Name = streamName }
        streams <- stream :: streams
        true
    let stopStream stream =
        streams <- List.filter (fun s -> s.StreamKey <> stream.StreamKey) streams
        true
    let updateStream stream =
        let old = findStream' stream.StreamKey
        match old with
        | Some(s) -> 
            stopStream s |> ignore
            streams <- stream :: streams
            true
        | None -> false

module JsonStreams = 
    open System.Configuration
    open FSharp.Data
    open System.IO
    open System

    let private streamPath = ConfigurationManager.AppSettings.Get("WebApp.StreamPath")
    type private StreamJson = JsonProvider<"./stream.json">
    let private fromJson (json:StreamJson.Root) = 
        { Username = json.Username; StreamKey = json.StreamKey; Started = json.Started; Name = json.Name}
    let getFilename key = 
        Path.Combine(streamPath, (sprintf "%s.json" key))
    let startStream streamKey streamName userName = 
        let sjson = 
            StreamJson.Root(name = streamName,
                            username = userName,
                            streamKey = streamKey, 
                            started = DateTime.UtcNow)
        let json = sjson.JsonValue.ToString()
        IO.tryElseFalse (fun () -> 
            File.WriteAllText((getFilename sjson.StreamKey), json))
    let stopStream stream =
        IO.tryElseFalse (fun () -> 
            File.Delete((getFilename stream.StreamKey)))
    let findStream' streamKey =
        let file = FileInfo((getFilename streamKey))
        match file.Exists with
        | true -> fromJson (StreamJson.Load(file.FullName)) |> Some
        | false -> None
    let findStream (user:User) = findStream' user.StreamKey
    