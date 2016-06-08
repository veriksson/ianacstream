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

type UserService = 
    {
        FindUser: string -> User option
        SaveUser: User -> bool
        ApproveUser: User -> bool
    }