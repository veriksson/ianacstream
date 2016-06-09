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
        ApproveUser: User -> bool
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

    let findUser username = 
        let userfile = Path.Combine(userFolder.FullName, (sprintf "%s.json" username))
        match File.Exists(userfile) with
        | true -> JUser.Load userfile |> toUser |> Some
        | false -> None

