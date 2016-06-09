module Streams

open Types

type StreamService = 
    {
        FindStream:     User    -> Stream option
        FindStream':    string  -> Stream option
        StartStream:    string  -> string -> string -> bool
        StopStream:     Stream  -> bool
        UpdateStream:   Stream  -> bool
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
                       Name = streamName
                       Live = false}
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