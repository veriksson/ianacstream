// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
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

let app = 
    choose [ GET >=> choose [ path "/" >=> (render Views.index |> Successful.OK)
                              pathScan "/stream/%s" Actions.showStream
                              path "/signUp" >=> (render Views.signUp |> Successful.OK)
                              path "/browse" >=> (render Views.browse |> Successful.OK)
                              Files.browseHome ] ]

[<EntryPoint>]
let main argv = 
    let mimeTypes = 
        defaultMimeTypesMap @@ (function 
        | ".swf" -> mkMimeType "application/x-shockwave-flash" false
        | _ -> None)
    
    let devConf = 
        { defaultConfig with logger = StandardOutLogging()
                             mimeTypesMap = mimeTypes }
    
    startWebServer devConf app
    0 // return an integer exit code
