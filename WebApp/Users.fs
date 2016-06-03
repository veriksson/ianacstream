module Users
open System
open Types

let genKey () = 
    Guid.NewGuid().ToString().Replace("-", "").ToLower()
let createNew name = 
    { Name = name
      Joined = DateTime.UtcNow
      Approved = false
      StreamKey = genKey() }
let findUser findFn uid = 
    findFn uid
let saveUser saveFn user =
    saveFn user
let listUsers listFn = 
    listFn ()
   
type UserService = 
    {
        findUser: string -> User option
        saveUser: User -> bool
        approveUser: User -> bool
    }

module JsonUsers = 
    open FSharp.Data
    open System.IO
    open Types
    type JsonUser = JsonProvider<"./user.json">

    [<Literal>]
    let UserDir = "./users"
    let fileForUser = sprintf "%s/%s.json" UserDir
    let toUser (json:JsonUser.Root) : User = 
           {Name = json.Name
            Joined = json.Joined
            StreamKey = json.StreamKey
            Approved = json.Approved }
    let toJson (user:User) = JsonUser.Root(name = user.Name, joined = user.Joined, streamKey = user.StreamKey, approved = user.Approved)
    let loadUser (path:string) = 
        printfn "%s" path
        File.ReadAllText(path)
        |> JsonUser.Parse |> toUser
    let findUser uid = 
        let file = fileForUser uid
        match File.Exists(file) with
        | true -> loadUser file |> Some
        | _ -> None
    let saveUser (user:User) = 
        let file = fileForUser user.Name
        let json = (toJson user ).JsonValue.ToString()
        IO.tryElseFalse (fun () -> File.WriteAllText(file, json))
    let listUsers () = 
        Directory.EnumerateFiles(UserDir)
        |> Seq.map loadUser
