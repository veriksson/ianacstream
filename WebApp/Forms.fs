module IanacStream.Forms

open Suave.Form
open Suave.Html

let divClass c = divAttr ["class", c]
let submitInput value = inputAttr ["type", "submit"; "value", value; "class", "btn btn-primary"]
let form x = tag "form" ["method", "POST"] (flatten x)
let fieldset x = tag "fieldset" [] (flatten x)
let legend txt = tag "legend" [] (text txt)

type Field<'a> = {
    Label : string
    Xml : Form<'a> -> Suave.Html.Xml
}

type Fieldset<'a> = {
    Legend : string
    Fields : Field<'a> list
}

type FormLayout<'a> = {
    Fieldsets : Fieldset<'a> list
    SubmitText : string
    Form : Form<'a>
}

let renderForm (layout : FormLayout<_>) =    
    divClass "form-horizontal" [
        form [
            for set in layout.Fieldsets -> 
                fieldset [
                    yield legend set.Legend

                    for field in set.Fields do
                        yield divClass "form-group" [
                            yield divClass "editor-label control-label col-sm-2" [
                                text field.Label
                            ]
                            yield divClass "editor-field col-sm-10" [
                                field.Xml layout.Form
                            ]
                        ]
                ]

            yield divClass "col-sm-offset-2 col-sm-10" [submitInput layout.SubmitText]
        ]
    ]

type Register = {
    Username : string
    Password : Password
    ConfirmPassword : Password
}

let pattern = @".{6,}"

let passwordsMatch = 
    (fun f -> f.Password = f.ConfirmPassword), "Passwords must match"

let register : Form<Register> = 
    Form([TextProp ((fun f -> <@ f.Username @>), [maxLength 30])
          PasswordProp ((fun f -> <@ f.Password @>), [ passwordRegex pattern ] )
          PasswordProp ((fun f -> <@ f.ConfirmPassword @>), [ passwordRegex pattern ] )
    ], [passwordsMatch])

type Logon = {
    Username : string
    Password : Password
}

let logon : Form<Logon> = Form ([], [])

