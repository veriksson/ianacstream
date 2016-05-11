// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open Suave
open Suave.Filters
open Suave.Files
open Suave.Operators
open Suave.Logging
open Suave.Embedded

open HtmlGen

open System.Reflection

type HackyBacky () = 
    interface Logger with
        member x.Log level fLine =
            printfn "%d: %s" (level.ToInt()) (fLine()).message

let index = 
    document [
        head [ 
            title "IANAC  &ndash; Streaming"
            script [Src "flowplayer-3.2.13.min.js"] ""
            link [Type "text/css"; Rel "stylesheet"; Href "/style/stream.css"]
            ]
        body [
            header [] [
                h1 [] [text "IANAC"; br []; text "Streaming for the few"]
                ]
            nav [] [
                ul [] [
                    li [] [text "Front"]
                    li [] [text "Browse"]
                    li [] [text "Sign up"]
                    ]
                ]
            section [Id "content"] [
                div [Id "stream"; Style ["text-align", "center"]] []
                ]
            footer [] [
                 text "? &copy; 2016 IANAC"
                ]
            ]
        ]

 
let app =
    choose [
        GET >=> choose [
            path "/" >=>  (render index |> Successful.OK)
            path "/test" >=> (Successful.OK "vafan")            
            pathRegex "/style/stream.css" >=> Writers.setMimeType "text/css" >=> Embedded.sendResourceFromDefaultAssembly "stream.css" true
        ]
    ]

[<EntryPoint>]
let main argv = 
    let devConf =
        { defaultConfig with
            logger = HackyBacky()
        }
    startWebServer devConf app
    0 // return an integer exit code

