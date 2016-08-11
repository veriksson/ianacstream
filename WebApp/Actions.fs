module IanacStream.Actions
open System
open System.Configuration

open Suave
open Suave.Html
open Types
open Streams
open Users

let empty () = emptyText 

let html user headFn container footerFn = 
    Successful.OK (Views.index user headFn container footerFn)

let standardHtml container = 
    html None empty container empty

let home =
    standardHtml Views.home 

let browseStreams = 
    standardHtml Views.browseStreams

let viewStream name = 
    standardHtml (Views.viewStream name)

let login =
    standardHtml Views.logon
(*let showStream streams (streamKey:string) =
    match streams.FindStream' streamKey with
    | Some(s) ->
        render <| Views.stream s |> Successful.OK
    | None -> RequestErrors.NOT_FOUND "Stream not found"

let showLogin = 
    render Views.login |> Successful.OK

let showSignUp = 
    render Views.signUp |> Successful.OK

let getDataOrDefault def choices choice = 
    match choices choice with
    | Choice1Of2 d -> d
    | Choice2Of2 _ -> def

let getDataOrEmpty =
    getDataOrDefault ""

let signUp (streams:StreamService) (request:HttpRequest) = 
    let configPassword = ConfigurationManager.AppSettings.Get("WebApp.StartStreamPassword")
    let getform = getDataOrEmpty request.formData 
    let password = getform "password"
    if password <> configPassword then
            RequestErrors.FORBIDDEN "Invalid password!"
    else
        let yourName = getform "yourName"
        let streamName = getform "streamName"
        let key = genKey ()
        match streams.StartStream key streamName yourName with
        | true -> 
            let stream = streams.FindStream' key |> Option.get
            render <| Views.streamAdmin stream |> Successful.OK
        | false -> ServerErrors.INTERNAL_ERROR "Could not start stream"

let updateStream (streams:StreamService) (request:HttpRequest) = 
    let getform = getDataOrEmpty request.formData 
    let streamKey = getform "streamKey"
    if streamKey = "" then 
        RequestErrors.BAD_REQUEST "Missing stream key!"
    else
        let currentStream = streams.FindStream' streamKey
        match currentStream with
        | Some(cs) -> 
            let username = getform "yourName"
            let streamName = getform "streamName"
            let ns = {
                        Name = streamName
                        Username = username
                        Started = cs.Started
                        StreamKey = cs.StreamKey
                        Live = cs.Live
                      }
            if streams.UpdateStream ns then
                render <| Views.streamAdmin ns |> Successful.OK
            else
                ServerErrors.INTERNAL_ERROR "Could not update stream"
        | None -> RequestErrors.BAD_REQUEST "No such key!"

let stopStream (streams:StreamService) (request:HttpRequest) = 
    let getform = getDataOrEmpty request.formData 
    let streamKey = getform "streamKey"
    let stream = streams.FindStream' streamKey
    match stream with
    | Some(s) -> streams.StopStream s |> ignore; Redirection.FOUND "/"
    | None -> Redirection.FOUND "/"
                *)