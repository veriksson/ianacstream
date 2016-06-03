// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
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

// implementations of the services
let jsonUserService : UserService = 
    {
        findUser = Users.JsonUsers.findUser
        saveUser = Users.JsonUsers.saveUser
        approveUser = (fun u -> false) // not implemented yet
    }

let streams : StreamService =
    { 
        findStream = Streams.InMemoryStreams.findStream
        findStream' = Streams.InMemoryStreams.findStream'
        startStream = Streams.InMemoryStreams.startStream
        stopStream = Streams.InMemoryStreams.stopStream   
        updateStream = Streams.InMemoryStreams.updateStream
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
