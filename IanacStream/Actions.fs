module Actions
open System

open Suave
open Types

let showStream (userFn: string -> User option) streamFn (username:string) =
    let stream = userFn username |> streamFn
    match stream with
    | Some(s) ->
        render <| Views.stream s |> Successful.OK
    | None -> RequestErrors.NOT_FOUND "Stream not found"

let showSignUp = 
    render Views.signUp |> Successful.OK