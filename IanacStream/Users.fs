namespace Users
module User = 

    open System

    type User = 
        { Name : string
          Joined : DateTime
          Approved : bool
          StreamKey : string }

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

   
module TextUsers = 
    open FSharp.Data
    open System.IO
    open User

    type JsonUser = JsonProvider<"./definitions/user.json">

    [<Literal>]
    let UserDir = "./users"
    
    let fileForUser = sprintf "%s/%s.json" UserDir
    
    let toUser json = 
        json
        |> JsonUser.Parse
        |> fun u -> 
            { Name = u.Name
              Joined = u.Joined
              StreamKey = u.StreamKey
              Approved = u.Approved }
    
    let toJson user = JsonUser.Root(name = user.Name, joined = user.Joined, streamKey = user.StreamKey, approved = user.Approved)
    let loadUser path = File.ReadAllText(path) |> toUser
    
    let findUser uid = 
        let file = fileForUser uid
        match File.Exists(file) with
        | true -> loadUser file |> Some
        | _ -> None
    
    let saveUser user = 
        let file = fileForUser user.Name
        let json = (toJson user ).JsonValue.ToString()
        try 
            File.WriteAllText(file, json)
            true
        with :? IOException as e -> printfn "%s" e.Message; false 

    let listUsers () = 
        Directory.EnumerateFiles(UserDir)
        |> Seq.map loadUser
