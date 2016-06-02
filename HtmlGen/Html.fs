[<AutoOpen>]
module Html

open System.Text

type Attr = 
    | Style of (string * string) seq
    | Id of string
    | Class of string seq
    | Type of string
    | Src of string
    | Action of string
    | Method of string
    | Rel of string
    | Href of string

type Node = 
    | Document of Node seq
    | Element of string * Attr seq * Node seq
    | ClosedElement of string * Attr seq
    | Text of string

let renderAttrs (sb : StringBuilder) attrs = 
    for attr in attrs do
        match attr with
        | Style(kvps) -> 
            sb.Append(" style=\"") |> ignore
            kvps |> Seq.iter (fun (k, v) -> sb.AppendFormat("{0}:{1};", k, v) |> ignore)
            sb.Append("\"") |> ignore
        | Id(id) -> sb.AppendFormat(" id=\"{0}\"", id) |> ignore
        | Class(cls) -> sb.AppendFormat(" class=\"{0}\"", (String.concat " " cls)) |> ignore
        | Type(typ) -> sb.AppendFormat(" type=\"{0}\"", typ) |> ignore
        | Src(src) -> sb.AppendFormat(" src=\"{0}\"", src) |> ignore
        | Action(act) -> sb.AppendFormat(" action=\"{0}\"", act) |> ignore
        | Method(met) -> sb.AppendFormat(" method=\"{0}\"", met) |> ignore
        | Rel(rel) -> sb.AppendFormat(" rel=\"{0}\"", rel) |> ignore
        | Href(href) -> sb.AppendFormat(" href=\"{0}\"", href) |> ignore
    sb

let rec renderNode (sb : StringBuilder) = 
    function 
    | Document(children) -> 
        sb.Append("<!DOCTYPE html>") |> ignore
        sb.Append("<html>") |> ignore
        for child in children do
            renderNode sb child |> ignore
        sb.Append("</html>") |> ignore
        sb
    | Element(name, attrs, children) -> 
        sb.AppendFormat("<{0}", name) |> ignore
        renderAttrs sb attrs |> ignore
        sb.Append(">") |> ignore
        for child in children do
            renderNode sb child |> ignore
        sb.AppendFormat("</{0}>", name)
    | ClosedElement(name, attrs) -> 
        sb.AppendFormat("<{0}", name) |> ignore
        renderAttrs sb attrs |> ignore
        sb.Append(" />")
    | Text(s) -> sb.Append(s)

let render node = (renderNode (StringBuilder()) node).ToString()
let renderFn (nodeFn : unit -> Node) = render <| nodeFn()
let elem name attrs children = Element(name, attrs, children)
let elem' name attrs = ClosedElement(name, attrs)
let text s = Text(s)
let document children = Document(children)
let body children = elem "body" [] children
let head children = elem "head" [] children
let title txt = elem "title" [] [ text txt ]
let h1 attrs children = elem "h1" attrs children
let h2 attrs children = elem "h2" attrs children
let h3 attrs children = elem "h3" attrs children
let h4 attrs children = elem "h4" attrs children
let h5 attrs children = elem "h5" attrs children
let table attrs children = elem "table" attrs children
let th attrs children = elem "th" attrs children
let tr attrs children = elem "tr" attrs children
let td attrs children = elem "td" attrs children
let div attrs children = elem "div" attrs children
let p attrs children = elem "p" attrs children
let hr attrs = elem' "hr" attrs
let li attrs children = elem "li" attrs children
let ul attrs children = elem "ul" attrs children
let ol attrs children = elem "ol" attrs children
let form attrs children = elem "form" attrs children
let input attrs children = elem "input" attrs children
let password (attrs : Attr seq) children = input (Seq.append [ Type "password" ] attrs) children
let img attrs = elem' "img" attrs
let script attrs code = elem "script" attrs [text code]
let link attrs = elem' "link" attrs
let br attrs = elem' "br" attrs
let a attrs children = elem "a" attrs children

(* html5 wank *)
let header attrs children = elem "header" attrs children
let nav attrs children = elem "nav" attrs children
let section attrs children = elem "section" attrs children
let footer attrs children = elem "footer" attrs children
