// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.
// Define your library scripting code here
module Script

#load "Html.fs"

open Html
open System.Text
open System.IO

let cool name = 
    div [ Class [ "avatar" ] ] [ img 
                                     [ Src 
                                           "https://lh3.googleusercontent.com/-TXoWkFG5oug/AAAAAAAAAAI/AAAAAAAAAAA/AMW9IgdtCOlWe0-48Ah5XNXibl_i2Vh7_A/s64-c-mo/photo.jpg" ]
                                 p [] [ text (sprintf "User %d" name) ] 
                                 hr [] ]

let template () = 
    document [ head []
               body [ div [ Style [ "margin", "0 auto" ] ] [ h2 [] [ text "Welcome to my site" ]
                                                             p [] [ text "This is a test" ]
                                                             div [] [ for x in 1..12 -> cool x ] ] ] ]

let test () = 
    let render = Html.renderFn template
    File.WriteAllText(@"C:\temp\test2.html", render)

let test2() = Html.renderFn template