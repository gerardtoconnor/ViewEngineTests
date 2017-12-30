/// ---------------------------
/// Attribution to original authors of this code
/// ---------------------------
/// This egine follows syntax of Suave ViewEngine but completly re-written by gerardtoconnor@gmail.com.
///

module GiraffeViewEngine

open System
open System.Net
open System.IO
open System.Xml
open System.Xml
open System.Text


/// ---------------------------
/// Definition of different HTML content
///
/// For more info check:
/// - https://developer.mozilla.org/en-US/docs/Web/HTML/Element
/// - https://www.w3.org/TR/html5/syntax.html#void-elements
/// ---------------------------

// type OXmlAttribute =
//     | KeyValue of string * string
//     | Boolean  of string

// type OXmlElement   = string * OXmlAttribute[]    // Name * XML attributes

// type OXmlNode =
//     | ParentNode  of OXmlElement * OXmlNode list // An XML element which contains nested XML elements
//     | VoidElement of OXmlElement                // An XML element which cannot contain nested XML (e.g. <hr /> or <br />)
//     | EncodedText of string                    // XML encoded text content
//     | RawText     of string                    // Raw text content

// Assumptions to provide best performance
// Each load of list item should be a struct for caching perf
// cannot use easier interface option as struct would be boxed
// test both interface objects and DU struct types
// Unions are manual to reuse type fields as don't believe DU re-uses but creates unique for each case 


/// Helper operator to add strings to writer
// let inline (++) (sr:StreamWriter) (str:string) = sr.Write(str) ; sr
// let inline (+>) (sr:StreamWriter) (fn:StreamWriter -> StreamWriter) = fn sr

type AttrType =
| KeyValue = 0uy
| Boolean = 1uy
type XmlAttribute =
    struct
        val AType : AttrType
        val AField : string
        val AValue : string
    end
    new (atype,afield,avalue) = {AType = atype;AField = afield ; AValue = avalue} 
    static member KeyValue afield avalue = XmlAttribute(AttrType.KeyValue,afield,avalue )
    static member Boolean  afield        = XmlAttribute(AttrType.Boolean,afield,null)
    static member Write (writer:StreamWriter,attr:XmlAttribute) =
        match attr.AType with
        | AttrType.KeyValue -> 
            writer.Write " "
            writer.Write attr.AField 
            writer.Write "=" 
            writer.Write attr.AValue
        | AttrType.Boolean -> 
            writer.Write " "
            writer.Write attr.AField
        | x -> failwith ("Unknown Attribute provided:" + x.ToString())

// type NodeType =
// | ParentNode = 0uy
// | VoidNode = 1uy
// | EncodedText = 2uy
// | RawTest = 3uy
// | Comment = 4uy

type NodeTag =
| Html       = 0uy
| Base       = 1uy
| Head       = 2uy
| Link       = 3uy
| Meta       = 4uy
| Style      = 5uy
| Title      = 6uy
| Body       = 7uy
| Address    = 8uy
| Article    = 9uy
| Aside      = 10uy
| Footer     = 11uy
| Hgroup     = 12uy
| H1         = 13uy
| H2         = 14uy
| H3         = 15uy
| H4         = 16uy
| H5         = 17uy
| H6         = 18uy
| Header     = 19uy
| Nav        = 20uy
| Section    = 21uy
| Dd         = 22uy
| Div        = 23uy
| Dl         = 24uy
| Dt         = 25uy
| Figcaption = 26uy
| Figure     = 27uy
| Hr         = 28uy
| Li         = 29uy
| Main       = 30uy
| Ol         = 31uy
| P          = 32uy
| Pre        = 33uy
| Ul         = 34uy
| A          = 35uy
| Abbr       = 36uy
| B          = 37uy
| Bdi        = 38uy
| Bdo        = 39uy
| Br         = 40uy
| Cite       = 41uy
| Code       = 42uy
| Data       = 43uy
| Dfn        = 44uy
| Em         = 45uy
| I          = 46uy
| Kbd        = 47uy
| Mark       = 48uy
| Q          = 49uy
| Rp         = 50uy
| Rt         = 51uy
| Rtc        = 52uy
| Ruby       = 53uy
| S          = 54uy
| Samp       = 55uy
| Small      = 56uy
| Span       = 57uy
| Strong     = 58uy
| Sub        = 59uy
| Sup        = 60uy
| Time       = 61uy
| U          = 62uy
| Var        = 63uy
| Wbr        = 64uy
| Area       = 65uy
| Audio      = 66uy
| Img        = 67uy
| Map        = 68uy
| Track      = 69uy
| Video      = 70uy
| Embed      = 71uy
| Object     = 72uy
| Param      = 73uy
| Source     = 74uy
| Canvas     = 75uy
| Noscript   = 76uy
| Script     = 77uy
| Del        = 78uy
| Ins        = 79uy
| Caption    = 80uy
| Col        = 81uy
| Colgroup   = 82uy
| Table      = 83uy
| Tbody      = 84uy
| Td         = 85uy
| Tfoot      = 86uy
| Th         = 87uy
| Thead      = 88uy
| Tr         = 89uy
| Button     = 90uy
| Datalist   = 91uy
| Fieldset   = 92uy
| Form       = 93uy
| Input      = 94uy
| Label      = 95uy
| Legend     = 96uy
| Meter      = 97uy
| Optgroup   = 98uy
| Option     = 99uy
| Output     = 100uy
| Progress   = 101uy
| Select     = 102uy
| Textarea   = 103uy
| Details    = 104uy
| Dialog     = 105uy
| Menu       = 106uy
| Menuitem   = 107uy
| Summary    = 108uy
| Encodedtext = 109uy
| Rawtext    = 110uy
| Emptytext  = 111uy
| Comment    = 112uy

type XmlNode =
    struct
        val NTag     : NodeTag
        val Attrs : XmlAttribute list
        val Childern  : XmlNode list
        val TextVal      : string
    end
    new (ntag,attr,children,text) = { NTag = ntag ; Attrs = attr ; Childern = children ; TextVal = text } 
    static member inline ParentNode ntype attr children = XmlNode(ntype,attr,children,null)
    static member inline VoidNode ntype attr = XmlNode(ntype,attr,Unchecked.defaultof<_ list>,null)
    static member inline Text ntype text = XmlNode(ntype,Unchecked.defaultof<_ list>,Unchecked.defaultof<_ list>,text)

    static member private WriteParentNode (node:XmlNode,writer:StreamWriter) =
        for attr in node.Attrs do
            XmlAttribute.Write(writer,attr)
        writer.Write ">"
        for child in node.Childern do
            XmlNode.Write(child,writer)

    static member private WriteVoidNode (node:XmlNode,writer:StreamWriter) =
        for attr in node.Attrs do
            XmlAttribute.Write(writer,attr)

    static member Write (node:XmlNode,writer:StreamWriter) =
        let parentNode (tag:string,node) = 
            writer.Write "<" 
            writer.Write tag 
            XmlNode.WriteParentNode(node,writer)
            writer.Write "</" 
            writer.Write tag 
            writer.Write ">"
        let voidNode (tag:string,node) = 
            writer.Write "<" 
            writer.Write tag 
            XmlNode.WriteVoidNode (node,writer) 
            writer.Write " />"

        match node.NTag with
        | NodeTag.Html       -> parentNode ("html",node)
        | NodeTag.Base       -> voidNode ("base",node)
        | NodeTag.Head       -> parentNode ("head",node)
        | NodeTag.Link       -> voidNode ("link",node)
        | NodeTag.Meta       -> voidNode ("meta",node)
        | NodeTag.Style      -> parentNode ("style",node)
        | NodeTag.Title      -> parentNode ("title",node)
        | NodeTag.Body       -> parentNode ("body",node)
        | NodeTag.Address    -> parentNode ("address",node)
        | NodeTag.Article    -> parentNode ("article",node)
        | NodeTag.Aside      -> parentNode ("aside",node)
        | NodeTag.Footer     -> parentNode ("footer",node)
        | NodeTag.Hgroup     -> parentNode ("hgroup",node)
        | NodeTag.H1         -> parentNode ("h1",node)
        | NodeTag.H2         -> parentNode ("h2",node)
        | NodeTag.H3         -> parentNode ("h3",node)
        | NodeTag.H4         -> parentNode ("h4",node)
        | NodeTag.H5         -> parentNode ("h5",node)
        | NodeTag.H6         -> parentNode ("h6",node)
        | NodeTag.Header     -> parentNode ("header",node)
        | NodeTag.Nav        -> parentNode ("nav",node)
        | NodeTag.Section    -> parentNode ("section",node)
        | NodeTag.Dd         -> parentNode ("dd",node)
        | NodeTag.Div        -> parentNode ("div",node)
        | NodeTag.Dl         -> parentNode ("dl",node)
        | NodeTag.Dt         -> parentNode ("dt",node)
        | NodeTag.Figcaption -> parentNode ("figcaption",node)
        | NodeTag.Figure     -> parentNode ("figure",node)
        | NodeTag.Hr         -> voidNode ("hr",node)
        | NodeTag.Li         -> parentNode ("li",node)
        | NodeTag.Main       -> parentNode ("main",node)
        | NodeTag.Ol         -> parentNode ("ol",node)
        | NodeTag.P          -> parentNode ("p",node)
        | NodeTag.Pre        -> parentNode ("pre",node)
        | NodeTag.Ul         -> parentNode ("ul",node)
        | NodeTag.A          -> parentNode ("a",node)
        | NodeTag.Abbr       -> parentNode ("abbr",node)
        | NodeTag.B          -> parentNode ("b",node)
        | NodeTag.Bdi        -> parentNode ("bdi",node)
        | NodeTag.Bdo        -> parentNode ("bdo",node)
        | NodeTag.Br         -> voidNode ("br",node)
        | NodeTag.Cite       -> parentNode ("cite",node)
        | NodeTag.Code       -> parentNode ("code",node)
        | NodeTag.Data       -> parentNode ("data",node)
        | NodeTag.Dfn        -> parentNode ("dfn",node)
        | NodeTag.Em         -> parentNode ("em",node)
        | NodeTag.I          -> parentNode ("i",node)
        | NodeTag.Kbd        -> parentNode ("kbd",node)
        | NodeTag.Mark       -> parentNode ("mark",node)
        | NodeTag.Q          -> parentNode ("q",node)
        | NodeTag.Rp         -> parentNode ("rp",node)
        | NodeTag.Rt         -> parentNode ("rt",node)
        | NodeTag.Rtc        -> parentNode ("rtc",node)
        | NodeTag.Ruby       -> parentNode ("ruby",node)
        | NodeTag.S          -> parentNode ("s",node)
        | NodeTag.Samp       -> parentNode ("samp",node)
        | NodeTag.Small      -> parentNode ("small",node)
        | NodeTag.Span       -> parentNode ("span",node)
        | NodeTag.Strong     -> parentNode ("strong",node)
        | NodeTag.Sub        -> parentNode ("sub",node)
        | NodeTag.Sup        -> parentNode ("sup",node)
        | NodeTag.Time       -> parentNode ("time",node)
        | NodeTag.U          -> parentNode ("u",node)
        | NodeTag.Var        -> parentNode ("var",node)
        | NodeTag.Wbr        -> voidNode ("wbr",node)
        | NodeTag.Area       -> voidNode ("area",node)
        | NodeTag.Audio      -> parentNode ("audio",node)
        | NodeTag.Img        -> voidNode ("img",node)
        | NodeTag.Map        -> parentNode ("map",node)
        | NodeTag.Track      -> voidNode ("track",node)
        | NodeTag.Video      -> parentNode ("video",node)
        | NodeTag.Embed      -> voidNode ("embed",node)
        | NodeTag.Object     -> parentNode ("object",node)
        | NodeTag.Param      -> voidNode ("param",node)
        | NodeTag.Source     -> voidNode ("source",node)
        | NodeTag.Canvas     -> parentNode ("canvas",node)
        | NodeTag.Noscript   -> parentNode ("noscript",node)
        | NodeTag.Script     -> parentNode ("script",node)
        | NodeTag.Del        -> parentNode ("del",node)
        | NodeTag.Ins        -> parentNode ("ins",node)
        | NodeTag.Caption    -> parentNode ("caption",node)
        | NodeTag.Col        -> voidNode ("col",node)
        | NodeTag.Colgroup   -> parentNode ("colgroup",node)
        | NodeTag.Table      -> parentNode ("table",node)
        | NodeTag.Tbody      -> parentNode ("tbody",node)
        | NodeTag.Td         -> parentNode ("td",node)
        | NodeTag.Tfoot      -> parentNode ("tfoot",node)
        | NodeTag.Th         -> parentNode ("th",node)
        | NodeTag.Thead      -> parentNode ("thead",node)
        | NodeTag.Tr         -> parentNode ("tr",node)
        | NodeTag.Button     -> parentNode ("button",node)
        | NodeTag.Datalist   -> parentNode ("datalist",node)
        | NodeTag.Fieldset   -> parentNode ("fieldset",node)
        | NodeTag.Form       -> parentNode ("form",node)
        | NodeTag.Input      -> voidNode ("input",node)
        | NodeTag.Label      -> parentNode ("label",node)
        | NodeTag.Legend     -> parentNode ("legend",node)
        | NodeTag.Meter      -> parentNode ("meter",node)
        | NodeTag.Optgroup   -> parentNode ("optgroup",node)
        | NodeTag.Option     -> parentNode ("option",node)
        | NodeTag.Output     -> parentNode ("output",node)
        | NodeTag.Progress   -> parentNode ("progress",node)
        | NodeTag.Select     -> parentNode ("select",node)
        | NodeTag.Textarea   -> parentNode ("textarea",node)
        | NodeTag.Details    -> parentNode ("details",node)
        | NodeTag.Dialog     -> parentNode ("dialog",node)
        | NodeTag.Menu       -> parentNode ("menu",node)
        | NodeTag.Menuitem   -> voidNode ("menuitem",node)
        | NodeTag.Summary    -> parentNode ("summary",node)
        | NodeTag.Encodedtext -> writer.Write ( WebUtility.HtmlEncode node.TextVal )
        | NodeTag.Rawtext    -> writer.Write node.TextVal
        | NodeTag.Emptytext  -> writer.Write "\"\""
        | NodeTag.Comment    -> 
            writer.Write "<!-- " 
            writer.Write node.TextVal 
            writer.Write " -->"

and StreamWriter with
    member inline x.Write(doc:XmlNode) =
        XmlNode.Write(doc,x)

/// ---------------------------
/// Building blocks
/// ---------------------------
/// ---------------------------
/// Render XML string
/// ---------------------------

let html       = XmlNode.ParentNode NodeTag.Html
let ``base``   = XmlNode.VoidNode NodeTag.Base
let head       = XmlNode.ParentNode NodeTag.Head
let link       = XmlNode.VoidNode NodeTag.Link
let meta       = XmlNode.VoidNode NodeTag.Meta
let style      = XmlNode.ParentNode NodeTag.Style
let title      = XmlNode.ParentNode NodeTag.Title
let body       = XmlNode.ParentNode NodeTag.Body
let address    = XmlNode.ParentNode NodeTag.Address
let article    = XmlNode.ParentNode NodeTag.Article
let aside      = XmlNode.ParentNode NodeTag.Aside
let footer     = XmlNode.ParentNode NodeTag.Footer
let hgroup     = XmlNode.ParentNode NodeTag.Hgroup
let h1         = XmlNode.ParentNode NodeTag.H1
let h2         = XmlNode.ParentNode NodeTag.H2
let h3         = XmlNode.ParentNode NodeTag.H3
let h4         = XmlNode.ParentNode NodeTag.H4
let h5         = XmlNode.ParentNode NodeTag.H5
let h6         = XmlNode.ParentNode NodeTag.H6
let header     = XmlNode.ParentNode NodeTag.Header
let nav        = XmlNode.ParentNode NodeTag.Nav
let section    = XmlNode.ParentNode NodeTag.Section
let dd         = XmlNode.ParentNode NodeTag.Dd
let div        = XmlNode.ParentNode NodeTag.Div
let dl         = XmlNode.ParentNode NodeTag.Dl
let dt         = XmlNode.ParentNode NodeTag.Dt
let figcaption = XmlNode.ParentNode NodeTag.Figcaption
let figure     = XmlNode.ParentNode NodeTag.Figure
let hr         = XmlNode.VoidNode NodeTag.Hr
let li         = XmlNode.ParentNode NodeTag.Li
let main       = XmlNode.ParentNode NodeTag.Main
let ol         = XmlNode.ParentNode NodeTag.Ol
let p          = XmlNode.ParentNode NodeTag.P
let pre        = XmlNode.ParentNode NodeTag.Pre
let ul         = XmlNode.ParentNode NodeTag.Ul
let a          = XmlNode.ParentNode NodeTag.A
let abbr       = XmlNode.ParentNode NodeTag.Abbr
let b          = XmlNode.ParentNode NodeTag.B
let bdi        = XmlNode.ParentNode NodeTag.Bdi
let bdo        = XmlNode.ParentNode NodeTag.Bdo
let br         = XmlNode.VoidNode NodeTag.Br
let cite       = XmlNode.ParentNode NodeTag.Cite
let code       = XmlNode.ParentNode NodeTag.Code
let data       = XmlNode.ParentNode NodeTag.Data
let dfn        = XmlNode.ParentNode NodeTag.Dfn
let em         = XmlNode.ParentNode NodeTag.Em
let i          = XmlNode.ParentNode NodeTag.I
let kbd        = XmlNode.ParentNode NodeTag.Kbd
let mark       = XmlNode.ParentNode NodeTag.Mark
let q          = XmlNode.ParentNode NodeTag.Q
let rp         = XmlNode.ParentNode NodeTag.Rp
let rt         = XmlNode.ParentNode NodeTag.Rt
let rtc        = XmlNode.ParentNode NodeTag.Rtc
let ruby       = XmlNode.ParentNode NodeTag.Ruby
let s          = XmlNode.ParentNode NodeTag.S
let samp       = XmlNode.ParentNode NodeTag.Samp
let small      = XmlNode.ParentNode NodeTag.Small
let span       = XmlNode.ParentNode NodeTag.Span
let strong     = XmlNode.ParentNode NodeTag.Strong
let sub        = XmlNode.ParentNode NodeTag.Sub
let sup        = XmlNode.ParentNode NodeTag.Sup
let time       = XmlNode.ParentNode NodeTag.Time
let u          = XmlNode.ParentNode NodeTag.U
let var        = XmlNode.ParentNode NodeTag.Var
let wbr        = XmlNode.VoidNode NodeTag.Wbr
let area       = XmlNode.VoidNode NodeTag.Area
let audio      = XmlNode.ParentNode NodeTag.Audio
let img        = XmlNode.VoidNode NodeTag.Img
let map        = XmlNode.ParentNode NodeTag.Map
let track      = XmlNode.VoidNode NodeTag.Track
let video      = XmlNode.ParentNode NodeTag.Video
let embed      = XmlNode.VoidNode NodeTag.Embed
let object     = XmlNode.ParentNode NodeTag.Object
let param      = XmlNode.VoidNode NodeTag.Param
let source     = XmlNode.VoidNode NodeTag.Source
let canvas     = XmlNode.ParentNode NodeTag.Canvas
let noscript   = XmlNode.ParentNode NodeTag.Noscript
let script     = XmlNode.ParentNode NodeTag.Script
let del        = XmlNode.ParentNode NodeTag.Del
let ins        = XmlNode.ParentNode NodeTag.Ins
let caption    = XmlNode.ParentNode NodeTag.Caption
let col        = XmlNode.VoidNode NodeTag.Col
let colgroup   = XmlNode.ParentNode NodeTag.Colgroup
let table      = XmlNode.ParentNode NodeTag.Table
let tbody      = XmlNode.ParentNode NodeTag.Tbody
let td         = XmlNode.ParentNode NodeTag.Td
let tfoot      = XmlNode.ParentNode NodeTag.Tfoot
let th         = XmlNode.ParentNode NodeTag.Th
let thead      = XmlNode.ParentNode NodeTag.Thead
let tr         = XmlNode.ParentNode NodeTag.Tr
let button     = XmlNode.ParentNode NodeTag.Button
let datalist   = XmlNode.ParentNode NodeTag.Datalist
let fieldset   = XmlNode.ParentNode NodeTag.Fieldset
let form       = XmlNode.ParentNode NodeTag.Form
let input      = XmlNode.VoidNode NodeTag.Input
let label      = XmlNode.ParentNode NodeTag.Label
let legend     = XmlNode.ParentNode NodeTag.Legend
let meter      = XmlNode.ParentNode NodeTag.Meter
let optgroup   = XmlNode.ParentNode NodeTag.Optgroup
let option     = XmlNode.ParentNode NodeTag.Option
let output     = XmlNode.ParentNode NodeTag.Output
let progress   = XmlNode.ParentNode NodeTag.Progress
let select     = XmlNode.ParentNode NodeTag.Select
let textarea   = XmlNode.ParentNode NodeTag.Textarea
let details    = XmlNode.ParentNode NodeTag.Details
let dialog     = XmlNode.ParentNode NodeTag.Dialog
let menu       = XmlNode.ParentNode NodeTag.Menu
let menuitem   = XmlNode.VoidNode NodeTag.Menuitem
let summary    = XmlNode.ParentNode NodeTag.Summary
let encodedText = XmlNode.Text NodeTag.Encodedtext
let rawText    = XmlNode.Text NodeTag.Rawtext
let emptyText  = XmlNode.Text NodeTag.Emptytext
let comment    = XmlNode.Text NodeTag.Comment


let renderHtmlDocument ( document : XmlNode) (writer : StreamWriter) =
    writer.Write "<!DOCTYPE html>" 
    writer.Write document 
    writer.Write Environment.NewLine

/// Uses the Giraffe.XmlViewEngine to compile and render a HTML Document from
/// an given XmlNode. The HTTP response is of Content-Type text/html.

