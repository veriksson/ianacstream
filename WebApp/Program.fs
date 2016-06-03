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

let jsonStreamService : StreamService =
    { 
        findStream = Streams.JsonStreams.findStream
        findStream' = Streams.JsonStreams.findStream'
        startStream = Streams.JsonStreams.startStream
        stopStream = Streams.JsonStreams.stopStream   
    }

let app = 
    choose [ GET >=> choose [ path "/" >=> (render Views.index |> Successful.OK)
                              pathScan "/stream/%s" (Actions.showStream jsonStreamService)
                              path "/signUp" >=> Actions.showSignUp
                              Files.browseHome ] ]

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
