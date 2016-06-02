module Views

open HtmlGen

let baseTemplate headContent content footerContent = 
    document [
        head (
            [ 
            title "IANAC  &ndash; Streaming"
            link [Type "text/css"; Rel "stylesheet"; Href "/style/stream.css"]
            ] @ headContent)
        body ([
                header [] [
                    h1 [] [text "IANAC"; br []; text "Streaming for the few"]
                    ]
                nav [] [
                    ul [] (["/", "Front"; "/browse", "Browse Streams"; "/signUp", "Sign Up"]
                                |> List.map (fun (link, linkText) -> li [] [a [Href link] [text linkText]]))
                    ]
                section [Id "content"] content
                footer [] [ text "? &copy; 2016 IANAC" ]
            ] @ footerContent)
        ]

let index = 
    baseTemplate [] 
        [div [Style ["text-align", "center"]] 
            [   
                p [] [text "IANAC is a private streaming service"]
                p [] [text "Get an account by going to the sign in page"]
                p [] [text "You need to be approved to start streaming"]
            ]
        ] 
        []

let streamScript user = 
    script [] (sprintf """
    var playerInstance = jwplayer("stream");
        playerInstance.setup({
        "file": "rtmp://www.iamnotacrook.net:1935/live/%s",
        "width": 1280,
        "height": 720,
        "autostart": true
    });
    """ user)
                
let stream user =
    baseTemplate 
        [script [Src "/player/jwplayer.js"] ""; script [] "jwplayer.key=\"U2wFWwmt9Zd3C/ATxa6y0FcFAkGCiSoCAzhYog==\""] 
        [div [Id "stream"; Style ["text-align", "center"]] [text "This is the stream page"]] [(streamScript user)]

let signUp =
    baseTemplate [] 
        [div [Id "stream"; Style ["text-align", "center"]] [text "This is the signUp page"]] []

let browse =
    baseTemplate [] [div [Id "stream"; Style ["text-align", "center"]] [text "This is the browse page"]] []
    