module Views

open HtmlGen

let buttons user = 
    let staticButtons = ["/", "Front"; "/browse", "Active streams"]
    match user with
    | Some(u) -> staticButtons @ ["/settings", "Settings"]
    | None -> staticButtons @ ["/login", "Log in"]

let baseTemplate (ctx:Types.Context) headContent content footerContent = 
    document [
        head (
            [ 
            title "IANAC  &ndash; Streaming"
            link [Type "text/css" 
                  Rel "stylesheet"
                  Href "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css"
                  Integrity "sha384-1q8mTJOASx8j1Au+a5WDVnPi2lkFfwwEAa8hDDdjZlpLegxhjVME1fgjWPGmkzs7"
                  CrossOrigin "anonymous"]
            link [Type "text/css" 
                  Rel "stylesheet"
                  Href "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap-theme.min.css"
                  Integrity "sha384-fLW2N01lMqjakBkx3l/M9EahuwpSfeNvV63J5ezn3uZzapT0u7EYsXMjQV+0En5r"
                  CrossOrigin "anonymous"]
            ] @ headContent)
        body ([
                div [Class ["container"]; 
                     Style ["width", "800px"]] [
                    div [Class ["row"; "text-center"]] [
                        h1 [] [text "IANAC"; br []; text "Streaming for the few"]]
                    div [Class ["row"; "text-center"]] [
                        div [Class ["btn-group"]] ((buttons ctx.User)
                                    |> List.map (fun (link, linkText) -> a [Href link; Class ["btn btn-primary"]] [text linkText]))]
                    section [Id "content"; Style ["margin", "10px"]] content
                    div [Class ["row";"text-center"]] 
                        ([  hr []
                            text "2016 IANAC" ] @ footerContent)]
              ])]


let streamScript user = 
    script [] (sprintf """
    var playerInstance = jwplayer("stream");
        playerInstance.setup({
        "file": "rtmp://www.iamnotacrook.net:1935/live/%s",
        "width": 764,
        "height": 430,
        "autostart": true
    });""" user)
   
let formTextInput id placeholder = 
    div [Class ["form-group"]] [
                label [For id; Class ["col-sm-2"; "control-label"]] [text placeholder]
                div [Class ["col-sm-10"]] [
                    input [Type "text"; Class ["form-control"]; Id id; Name id; Placeholder (placeholder + "...")] []
                ]
            ]

let formTextInput' id placeholder value = 
    div [Class ["form-group"]] [
                label [For id; Class ["col-sm-2"; "control-label"]] [text placeholder]
                div [Class ["col-sm-10"]] [
                    input [Type "text"; Class ["form-control"]; Id id; Name id; Placeholder (placeholder + "..."); Value value] []
                ]
            ]

let formPasswordInput id placeholder = 
    div [Class ["form-group"]] [
                label [For id; Class ["col-sm-2"; "control-label"]] [text placeholder]
                div [Class ["col-sm-10"]] [
                    password [Class ["form-control"]; Id id; Name id; Placeholder (placeholder + "...")] []
                ]
            ] 

let formHiddenInput id value = 
    div [Class ["form-group"]] [
            div [Class ["col-sm-12"]] [
                hidden [Id id; Name id; Value value] []
            ]
        ]
         
let formSubmitButton buttonText = 
    div [Class ["form-group"]] [
            div [Class ["col-sm-offset-2 col-sm-10"]] [
                button [Type "submit"; Class ["btn btn-primary"]; ] [text buttonText]
            ]
        ] 

let signUpForm = 
    div [Id "sign-up-form"; Class ["form-horizontal"]] 
        [
        form [Action "/signUp"; Method "POST"] [
            formTextInput "yourName" "Username"
            formTextInput "yourEmail" "Email"
            formPasswordInput "password" "Password"
            formSubmitButton "Sign up"
            ]
        ]

let loginForm = 
    div [Id "login-form"; Class ["form-horizontal"]] 
        [
        form [Action "/login"; Method "POST"] [
            formTextInput "yourName" "Username"
            formPasswordInput "password" "Password"
            formSubmitButton "Log in"
            ]
        ]
let index = 
    baseTemplate Context.defaultCtx
        [] 
        [div [Class ["col-12"; "text-center"]] 
            [   
                p [] [text "IANAC is a private streaming service"]
                p [] [text "Get an account by going to the sign in page"]
                p [] [text "You need to be approved to start streaming"]
               
            ]
        ] 
        []



let streamAdminForm (stream:Types.Stream) = 
    div [Class ["row"]][
        hr []
        div [Class ["form-horizontal"]] [
            h3 [] [text "Update stream info"]
            form [Action "/updateStream"; Method "POST"] [
                formTextInput' "yourName" "Your name" stream.Username
                formTextInput' "streamName" "Stream name" stream.Name
                formHiddenInput "streamKey" stream.StreamKey
                formSubmitButton "Submit"]
        ]

        div [Class ["form-horizontal"]] [
            h3 [] [text "Stop streaming"]
            form [Action "/stopStream"; Method "POST"] [
                formHiddenInput "streamKey" stream.StreamKey
                formSubmitButton "Stop"]
            ]
        ]
   
let streamAdmin (stream:Types.Stream) =
    let streamName =
        h2 [] [ 
            text stream.Name
            span [Class ["small"]] [text (" &ndash; " + stream.Username)]
            hr []
            ]
    let streamLink = sprintf "http://stream.iamnotacrook.net/stream/%s" stream.StreamKey
    baseTemplate Context.defaultCtx
        [script [Src "/player/jwplayer.js"] ""; script [] "jwplayer.key=\"U2wFWwmt9Zd3C/ATxa6y0FcFAkGCiSoCAzhYog==\""] 
        [   div [Id "stream-info"; Class ["row"; "text-center"]] [
                streamName
                div [] [text (sprintf "To let other people join, send them this link <pre>%s</pre>" streamLink)]]
            div [Id "stream"; Style ["text-align", "center"]] []
            streamAdminForm stream
        ] [(streamScript stream.StreamKey)]

let stream (stream:Types.Stream) =
    let streamName =
        h2 [] [ 
            text stream.Name
            span [Class ["small"]] [text (" &ndash; " + stream.Username)]
            ]
    baseTemplate Context.defaultCtx
        [script [Src "/player/jwplayer.js"] ""; script [] "jwplayer.key=\"U2wFWwmt9Zd3C/ATxa6y0FcFAkGCiSoCAzhYog==\""] 
        [   div [Id "stream-info"; Class ["row"; "text-center"]] [streamName]
            div [Id "stream"; Style ["text-align", "center"]] []] [(streamScript stream.StreamKey)]

let login = 
    baseTemplate Context.defaultCtx
        []
        [
            div [Class ["col-12"]]  
                [
                    h3 [Class ["text-center"]] [text "Login"]
                    loginForm
                    hr []
                    h3 [Class ["text-center"]] [text "Don't have an account? Request one here."]
                    signUpForm
                ]
        ]
        []

let signUp =
    baseTemplate Context.defaultCtx
        [] 
        [
        div [Id "sign-up-form"; Class ["form-horizontal"]] 
            [
            form [Action "/signUp"; Method "POST"] [
                formTextInput "yourName" "Your name"
                formTextInput "streamName" "Stream name"
                formPasswordInput "password" "Password"
                formSubmitButton "Submit"
                ]
            ]
        ]
        []

    