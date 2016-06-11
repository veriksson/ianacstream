module Users

open System
open Types

open DevOne.Security.Cryptography.BCrypt 

let genKey () = 
    Guid.NewGuid().ToString().Replace("-", "").ToLower()

let hashPassword password = 
    BCryptHelper.HashPassword(password, BCryptHelper.GenerateSalt())

let checkPassword user password =
    BCryptHelper.CheckPassword(password, user.Password)

let createNew name password = 
    let hash = hashPassword password
    { Name = name
      Joined = DateTime.UtcNow
      Approved = false
      StreamKey = genKey()
      Password = hash
      Admin =false }

type UserService = 
    {
        FindUser: string -> User option
        SaveUser: User -> bool
        ListUsers : unit -> User seq
    }

module JsonUsers =
    open FSharp.Data
    open System.IO

    type JUser = JsonProvider<"user.json">
    let userFolder = 
        if not <| Directory.Exists("users") then
            Directory.CreateDirectory("users")
        else
            DirectoryInfo("users")
    let private toUser (juser:JUser.Root) =
          { Name        = juser.Name
            Joined      = juser.Joined
            Approved    = juser.Approved
            StreamKey   = juser.StreamKey
            Password    = juser.Password
            Admin       = juser.Admin }
    let private toJUser (user:User) =
        JUser.Root(name = user.Name, joined = user.Joined, approved = user.Approved, streamKey = user.StreamKey, password = user.Password, admin = user.Admin)
    let private file name =
        Path.Combine(userFolder.FullName, (sprintf "%s.json" name))
    let findUser username = 
        let userfile = file username
        match File.Exists(userfile) with
        | true -> JUser.Load userfile |> toUser |> Some
        | false -> None
    let saveUser user =
        let json = toJUser user
        let userfile = file user.Name
        try
            File.WriteAllText(userfile, json.JsonValue.ToString())
            true
        with
            | :? IOException -> false
    let listUsers () =
        userFolder.EnumerateFiles()
        |> Seq.map (fun f -> JUser.Load f.FullName |> toUser)

        
