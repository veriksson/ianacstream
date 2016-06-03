module Views

open HtmlGen

let baseTemplate headContent content footerContent = 
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
                div [Class ["container"]] [
                    div [Class ["row"; "text-center"]] [
                        h1 [] [text "IANAC"; br []; text "Streaming for the few"]]
                    div [Class ["row"; "text-center"]] [
                        div [Class ["btn-group"]] (["/", "Front"; "/signUp", "Sign Up"]
                                    |> List.map (fun (link, linkText) -> button [Class ["btn btn-primary"]] [text linkText]))]
                    section [Id "content"] content
                    div [Class ["row";"text-center"]] ([ text "2016 IANAC" ] @ footerContent)]])
              ]

let index = 
    baseTemplate [] 
        [div [Class ["col-12"; "text-center"]] 
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
    });""" user)
                
let stream (stream:Types.Stream) =
    let streamName =
        h2 [] [ 
            text stream.Name
            span [Class ["small"]] [text stream.Name]
            ]
    baseTemplate 
        [script [Src "/player/jwplayer.js"] ""; script [] "jwplayer.key=\"U2wFWwmt9Zd3C/ATxa6y0FcFAkGCiSoCAzhYog==\""] 
        [   div [Id "stream-info"; Style ["text-align", "center"]] [streamName]
            div [Id "stream"; Style ["text-align", "center"]] [text "This is the stream page"]] [(streamScript stream.StreamKey)]

let signUp =
    baseTemplate [] 
        [div [Id "stream"; Style ["text-align", "center"]] [text "This is the signUp page"]] []

let browse (streams:Types.Stream seq) =
    let streamImgDiv idx (s:Types.Stream) = 
        div [Style 
               ["width", "150px"
                "height", "120px" 
                "background", "#d1d2f9"
                "position", "relative"]] 
            [div [Style 
                    ["position", "absolute"
                     "bottom", "5px"
                     "right", "10px" 
                     "color", "#7796cb"]
                ] [text s.Name]

             (if idx % 3 = 0 then
                br [] else empty)
            ]
    baseTemplate [] 
        [
            (if (Seq.length streams > 0) then
                div [Id "streams"; Style ["text-align", "center"]] (streams |> Seq.mapi streamImgDiv)
            else
                div [Id "streams"; Style ["text-align", "center"]] [text "No streams live"])
        ]
        []
    