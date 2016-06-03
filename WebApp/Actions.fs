module Actions
open System

open Suave
open Types
open Streams
open Users

let showStream streams (streamKey:string) =
    match streams.findStream' streamKey with
    | Some(s) ->
        render <| Views.stream s |> Successful.OK
    | None -> RequestErrors.NOT_FOUND "Stream not found"

let showSignUp = 
    render Views.signUp |> Successful.OK