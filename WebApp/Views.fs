module IanacStream.Views

open Suave.Html
open Types
open Suave.Form
open Forms

let divId id = divAttr ["id", id]

let h1 xml = tag "h1" [] xml
let h2 s = tag "h2" [] (text s)

let ul xml = tag "ul" [] (flatten xml)
let li = tag "li" []

let aHref href = tag "a" ["href", href]
let buttonHref href = tag "a" ["href", href; " class", "btn btn-primary"]

let cssLink href = linkAttr ["href", href; " rel", "stylesheet"; " type", "text/css"]

let hrAttr attr =  tag "hr" attr empty
let hr = hrAttr []

let p s = tag "p" [] (text s)

let partHead headFn =
     head [ 
                title  "IANAC  &ndash; Streaming"
                cssLink "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css"
                cssLink "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap-theme.min.css"
                headFn ()
            ]

let getUserButton (user:User option) =
    match user with
    | Some(u) -> Paths.User.settings, "Settings"
    | None -> Paths.User.login, "Log in"

let index user headFn container footerFn = 
    html [
        partHead headFn

        body [
            divAttr ["class", "container"; "style", "width:800px"] [
                divAttr ["class", "row text-center"] [
                    h1  (flatten [
                            text "IANAC"
                            br
                            text "Streaming for the few"])
                ]
                divAttr  ["class", "row text-center"] [
                    divAttr ["class","btn-group"] ([Paths.home, "Front"; Paths.Stream.browse, "Active streams"; (getUserButton user)]
                                |> List.map (fun (link, linkText) -> buttonHref link (text linkText)))
                ]

                divAttr ["id", "content"; "style", "margin: 10px"] container

                divAttr ["class", "row text-center"] [
                    hr                        
                    text "2016 IANAC" 
                ]

                divId "footer" [footerFn ()]
            ]        
        ]
    ]
    |> xmlToString

let home = [
    divAttr ["class", "col-12 text-center"] [
        p "IANAC is a private streaming service"
        p "Get an account by going to the sign in page"
        p "You need to be approved to start streaming"
    ]
]

let browseStreams = [
    text "Browse streams"
]

let viewStream name = [
    text (sprintf "Viewing stream from %s" name)
]

let streamScript user = 
    scriptAttr [ "type", "text/javascript" ] [
        (text (sprintf """
                var playerInstance = jwplayer("stream");
                    playerInstance.setup({
                    "file": "rtmp://www.iamnotacrook.net:1935/live/%s",
                    "width": 764,
                    "height": 430,
                    "autostart": true
                });""" user) )
                ]

let logon = [
    h2 "Log on"
    p "Please enter your user name and password"

    renderForm 
        { Form = Forms.logon
          Fieldsets = 
            [{ Legend = "Account Information"
               Fields = 
                    [ { Label = "User name"
                        Xml = input (fun f -> <@ f.Username @>) ["class", "form-control"] }
                      { Label = "Password"
                        Xml = input (fun f -> <@ f.Password @>) ["class", "form-control"] } ] } ]
          SubmitText = "Log on" }
          
]
//let formTextInput id placeholder = 
//    divAttr ["class", "form-group"] [
//                label [For id; Class ["col-sm-2"; "control-label"]] [text placeholder]
//                div [Class ["col-sm-10"]] [
//                    input [Type "text"; Class ["form-control"]; Id id; Name id; Placeholder (placeholder + "...")] []
//                ]
//            ]
   
(*
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

    *)