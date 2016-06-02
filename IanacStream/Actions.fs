module Actions
open System

open Suave

let showStream username =
    render <| Views.stream username |> Successful.OK

let showSignUp = 
    render Views.signUp |> Successful.OK

