module ByteViewEngineAsync
open System.Text
open System.IO
open System
open System.Xml
open System.Threading.Tasks
open Giraffe.Tasks


type Stream with
    member inline x.WriteAsync(buffer:byte []) = x.WriteAsync(buffer,0,buffer.Length)
    member inline x.WriteAsync(value:string) = let buffer = value |> Encoding.UTF8.GetBytes in x.WriteAsync(buffer,0,buffer.Length)

type VNode = (list<Stream -> Task>) -> Stream -> Task
type PNode = (list<Stream -> Task>) -> VNode

let attrClosetTag = ">" |> Encoding.UTF8.GetBytes
let voidCloseTag = " />" |> Encoding.UTF8.GetBytes
let commentOpenTag = "<!-- " |> Encoding.UTF8.GetBytes
let commentCloseTag = " -->" |> Encoding.UTF8.GetBytes
let equalsTag = "=" |> Encoding.UTF8.GetBytes

let attr (key:string) (value:string) (wr:Stream) = 
    task {
        do! key |> Encoding.UTF8.GetBytes |> wr.WriteAsync
        do! wr.WriteAsync(equalsTag,0,equalsTag.Length)
        return! value |> Encoding.UTF8.GetBytes |> wr.WriteAsync
    } :> Task


let attrbool (key:string) (wr:Stream) =
    wr.WriteAsync key

let parentNode (tag:string) : (list<Stream -> Task>) -> (list<Stream -> Task>) -> Stream -> Task  = 
    let opentag = ("<" + tag) |> Encoding.UTF8.GetBytes
    let closetag = ("</" + tag + ">") |> Encoding.UTF8.GetBytes
    fun (attrs:list<Stream -> Task>) (children:list<Stream -> Task>) (wr:Stream) -> 
        task {
            do! wr.WriteAsync opentag
            for attr in attrs do
                do! attr wr
            do! wr.WriteAsync attrClosetTag
            for child in children do
                do! child wr
            return! wr.WriteAsync closetag        
        } :> Task


let voidNode (tag:string) : ((Stream -> Task) list) -> Stream -> Task   = 
    let opentag = ("<" + tag) |> Encoding.UTF8.GetBytes
    fun (attrs:(Stream -> Task) list) (wr:Stream) -> 
        task {
            do! wr.WriteAsync opentag
            for attr in attrs do
                do! attr wr
            return! wr.WriteAsync voidCloseTag
        } :> Task

let encodedText (txt:string) (wr:Stream) = wr.WriteAsync txt // include encoder
let rawText (txt:string) (wr:Stream) = wr.WriteAsync txt
let comment (txt:string) (wr:Stream) = 
    task {
        do! wr.WriteAsync commentOpenTag
        do! wr.WriteAsync txt
        return! wr.WriteAsync commentCloseTag
    } :> Task

let html : PNode         =  parentNode "html"
let ``base`` : VNode     =  voidNode "base"
let head : PNode         =  parentNode "head"
let link : VNode         =  voidNode "link"
let meta : VNode         =  voidNode "meta"
let style : PNode        =  parentNode "style"
let title : PNode        =  parentNode "title"
let body : PNode         =  parentNode "body"
let address : PNode      =  parentNode "address"
let article : PNode      =  parentNode "article"
let aside : PNode        =  parentNode "aside"
let footer : PNode       =  parentNode "footer"
let hgroup : PNode       =  parentNode "hgroup"
let h1 : PNode           =  parentNode "h1"
let h2 : PNode           =  parentNode "h2"
let h3 : PNode           =  parentNode "h3"
let h4 : PNode           =  parentNode "h4"
let h5 : PNode           =  parentNode "h5"
let h6 : PNode           =  parentNode "h6"
let header : PNode       =  parentNode "header"
let nav : PNode          =  parentNode "nav"
let section : PNode      =  parentNode "section"
let dd : PNode           =  parentNode "dd"
let div : PNode          =  parentNode "div"
let dl : PNode           =  parentNode "dl"
let dt : PNode           =  parentNode "dt"
let figcaption : PNode   =  parentNode "figcaption"
let figure : PNode       =  parentNode "figure"
let hr : VNode           =  voidNode "hr"
let li : PNode           =  parentNode "li"
let main : PNode         =  parentNode "main"
let ol : PNode           =  parentNode "ol"
let p : PNode            =  parentNode "p"
let pre : PNode          =  parentNode "pre"
let ul : PNode           =  parentNode "ul"
let a : PNode            =  parentNode "a"
let abbr : PNode         =  parentNode "abbr"
let b : PNode            =  parentNode "b"
let bdi : PNode          =  parentNode "bdi"
let bdo : PNode          =  parentNode "bdo"
let br : VNode           =  voidNode "br"
let cite : PNode         =  parentNode "cite"
let code : PNode         =  parentNode "code"
let data : PNode         =  parentNode "data"
let dfn : PNode          =  parentNode "dfn"
let em : PNode           =  parentNode "em"
let i : PNode            =  parentNode "i"
let kbd : PNode          =  parentNode "kbd"
let mark : PNode         =  parentNode "mark"
let q : PNode            =  parentNode "q"
let rp : PNode           =  parentNode "rp"
let rt : PNode           =  parentNode "rt"
let rtc : PNode          =  parentNode "rtc"
let ruby : PNode         =  parentNode "ruby"
let s : PNode            =  parentNode "s"
let samp : PNode         =  parentNode "samp"
let small : PNode        =  parentNode "small"
let span : PNode         =  parentNode "span"
let strong : PNode       =  parentNode "strong"
let sub : PNode          =  parentNode "sub"
let sup : PNode          =  parentNode "sup"
let time : PNode         =  parentNode "time"
let u : PNode            =  parentNode "u"
let var : PNode          =  parentNode "var"
let wbr : VNode          =  voidNode "wbr"
let area : VNode         =  voidNode "area"
let audio : PNode        =  parentNode "audio"
let img : VNode          =  voidNode "img"
let map : PNode          =  parentNode "map"
let track : VNode        =  voidNode "track"
let video : PNode        =  parentNode "video"
let embed : VNode        =  voidNode "embed"
let object : PNode       =  parentNode "object"
let param : VNode        =  voidNode "param"
let source : VNode       =  voidNode "source"
let canvas : PNode       =  parentNode "canvas"
let noscript : PNode     =  parentNode "noscript"
let script : PNode       =  parentNode "script"
let del : PNode          =  parentNode "del"
let ins : PNode          =  parentNode "ins"
let caption : PNode      =  parentNode "caption"
let col : VNode          =  voidNode "col"
let colgroup : PNode     =  parentNode "colgroup"
let table : PNode        =  parentNode "table"
let tbody : PNode        =  parentNode "tbody"
let td : PNode           =  parentNode "td"
let tfoot : PNode        =  parentNode "tfoot"
let th : PNode           =  parentNode "th"
let thead : PNode        =  parentNode "thead"
let tr : PNode           =  parentNode "tr"
let button : PNode       =  parentNode "button"
let datalist : PNode     =  parentNode "datalist"
let fieldset : PNode     =  parentNode "fieldset"
let form : PNode         =  parentNode "form"
let input : VNode        =  voidNode "input"
let label : PNode        =  parentNode "label"
let legend : PNode       =  parentNode "legend"
let meter : PNode        =  parentNode "meter"
let optgroup : PNode     =  parentNode "optgroup"
let option : PNode       =  parentNode "option"
let output : PNode       =  parentNode "output"
let progress : PNode     =  parentNode "progress"
let select : PNode       =  parentNode "select"
let textarea : PNode     =  parentNode "textarea"
let details : PNode      =  parentNode "details"
let dialog : PNode       =  parentNode "dialog"
let menu : PNode         =  parentNode "menu"
let menuitem : VNode     =  voidNode "menuitem"
let summary : PNode      =  parentNode "summary"

//////////////
let private docTypeTag = "<!DOCTYPE html>" |> Encoding.UTF8.GetBytes
let private newLineTag = Environment.NewLine |> Encoding.UTF8.GetBytes

let renderHtmlDocument ( document : Stream -> Task ) (writer : Stream) = 
    task {
        do! writer.WriteAsync docTypeTag
        do! document writer 
        return! writer.WriteAsync newLineTag
    } :> Task
