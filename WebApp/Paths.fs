module IanacStream.Paths

type IntPath = PrintfFormat<int -> string, unit, string, string, int>

type StringPath = PrintfFormat<string -> string, unit, string, string, string>

let home = "/"

module Stream = 
    let browse = "/streams/browse"
    let userStream : StringPath = "/streams/user/%s"

module User  =
    let login = "/users/login"
    let logout = "/users/logout"
    let settings = "/users/settings"
