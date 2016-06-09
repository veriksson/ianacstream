open Suave
open Suave.Filters
open Suave.Files
open Suave.Operators
open Suave.Logging
open Suave.Writers
open HtmlGen
open Streams
open Users

type StandardOutLogging() = 
    interface Logger with
        member x.Log level fLine = printfn "%d: %s" (level.ToInt()) (fLine()).message


let streams : StreamService =
    { 
        FindStream = Streams.InMemoryStreams.findStream
        FindStream' = Streams.InMemoryStreams.findStream'
        StartStream = Streams.InMemoryStreams.startStream
        StopStream = Streams.InMemoryStreams.stopStream   
        UpdateStream = Streams.InMemoryStreams.updateStream
    }

let app = 
    choose [ GET >=> choose [ path "/" >=> Actions.index
                              pathScan "/stream/%s" (Actions.showStream streams)
                              path "/signUp" >=> Actions.showSignUp
                              Files.browseHome ] 
             POST >=> choose [ path "/signUp" >=> request (fun r -> (Actions.signUp streams) r) 
                               path "/updateStream" >=> request (fun r -> (Actions.updateStream streams) r)
                               path "/stopStream" >=> request (fun r -> (Actions.stopStream streams) r)
             ]]

let mimeTypes = 
    defaultMimeTypesMap @@ (function 
    | ".swf" -> mkMimeType "application/x-shockwave-flash" false
    | _ -> None)

let appConf = 
    { defaultConfig with logger = StandardOutLogging()
                         mimeTypesMap = mimeTypes }

[<EntryPoint>]
let main argv = 
    startWebServer appConf app
    0 // return an integer exit code
