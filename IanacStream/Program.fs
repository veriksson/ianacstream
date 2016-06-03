open Suave
open Suave.Filters
open Suave.Files
open Suave.Operators
open Suave.Logging
open Suave.Embedded
open HtmlGen
open Users
open Suave.Writers

type StandardOutLogging() = 
    interface Logger with
        member x.Log level fLine = printfn "%d: %s" (level.ToInt()) (fLine()).message

// DI? di.
let findUser = Users.findUser Users.JsonUsers.findUser
let findStream = Streams.findStream Streams.JsonStreams.findStream

let app = 
    choose [ GET >=> choose [ path "/" >=> (render Views.index |> Successful.OK)
                              pathScan "/stream/%s" (Actions.showStream findUser findStream)
                              path "/signUp" >=> (render Views.signUp |> Successful.OK)
                              Files.browseHome ] ]

let mimeTypes = 
    defaultMimeTypesMap @@ (function 
    | ".swf" -> mkMimeType "application/x-shockwave-flash" false
    | _ -> None)

let devConf = 
    { defaultConfig with logger = StandardOutLogging()
                         mimeTypesMap = mimeTypes }

[<EntryPoint>]
let main argv = 
    startWebServer devConf app
    0
