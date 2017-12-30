module TemplateViewEngine
open System.IO
open System.Text
open System.Net

/////////// Testing

type CompiledNode<'T> =
| CText of byte []
| CAttr of ('T -> string * string)
| CBind of ('T -> string)
| CBindIf of ('T -> bool) * CompiledNode<'T> [] * CompiledNode<'T> []
| CBindFor of ('T * StreamWriter -> unit)

type XmlAttr<'T> =
| KeyValue of string * string
| BindAttr of ('T -> string * string)

type XmlNode<'T> =
| ParentNode of  string * XmlAttr<'T> list * XmlNode<'T> list
| VoidNode of  string * XmlAttr<'T> list 
| EncodedText of string
| RawText of string
| Bind of ('T -> string)
| BindIf of ('T -> bool) * CompiledNode<'T> [] * CompiledNode<'T> []
| BindFor of ('T * StreamWriter -> unit)

let inline (+>) (str:string) (ls: byte []) = Encoding.UTF8.GetBytes str 

let writeFlush (sb:StringBuilder,acc:CompiledNode<'T> list) =
    if sb.Length > 0 
    then 
        let nacc = (sb.ToString() |> Encoding.UTF8.GetBytes |> CText) :: acc
        sb.Clear() |> ignore
        nacc
    else acc      
let compile (raw:XmlNode<'T>) : CompiledNode<'T> [] =
    let rec go node (sb:StringBuilder) acc =
        match node with
        | ParentNode (name,attrs,children) ->
            let mutable acc = acc 
            sb.Append("<" + name) |> ignore
            for attr in attrs do
                match attr with
                | KeyValue (key,value) -> sb.Append(key + "=" + value) |> ignore
                | BindAttr (fn) -> 
                    acc <- CAttr fn :: writeFlush(sb,acc)
                //| add bool flag
            sb.Append ">" |> ignore

            for child in children do
                acc <- go child sb acc
            
            sb.Append("</" + name + ">") |> ignore
            acc
                
        | VoidNode (name,attrs) ->
            let mutable acc = acc 
            sb.Append("<" + name) |> ignore
            for attr in attrs do
                match attr with
                | KeyValue (key,value) -> sb.Append(key + "=" + value) |> ignore
                | BindAttr (fn) -> 
                    acc <- CAttr fn :: writeFlush(sb,acc)
                //| add bool flag            
            sb.Append(" />") |> ignore
            acc
        | EncodedText txt -> sb.Append (WebUtility.HtmlEncode txt) |> ignore ; acc
        | RawText txt    -> sb.Append txt |> ignore; acc
        | Bind fn        -> CBind fn :: writeFlush(sb,acc)
        | BindIf (p,t,f) -> CBindIf(p,t,f) :: writeFlush(sb,acc)
        | BindFor fn     -> CBindFor(fn) :: writeFlush(sb,acc)
    
    let sb = StringBuilder() // re-usable stringbuilder for building string parts
    let acc = go raw sb [] // node list in reverse order so position backwards down array
    let acc' = writeFlush(sb,acc)
    let result = Array.zeroCreate<_>(acc'.Length)
    let rec roll (ls,index) =
        match ls, index with
        | [] , -1 -> ()
        | h :: t, i -> result.[i] <- h ; roll (t,i - 1)
        | _,_ -> failwith "unexpected unroll error"
    roll (acc',result.Length - 1)
    result

let rec processNodes (item:'T,sw:StreamWriter,nodes:CompiledNode<'T> [] ) =
    for node in nodes do
        match node with                
        | CText v -> sw.Write v
        | CBind fn -> sw.Write ( fn item )
        | CBindIf (pred,trueFns,falseFns) ->
            if pred item then
                processNodes(item,sw,trueFns)
            else
                processNodes(item,sw,falseFns)
        | CBindFor (fn) -> fn(item,sw)

let inline bindFor<'T> (enumFn:'T -> #seq<'U>) (template:XmlNode<'U>) =
    let compiledNodes = compile template
    BindFor (fun (model:'T,sw:StreamWriter) ->
        for item in enumFn model do
            processNodes(item,sw,compiledNodes)
    )

let inline bindIf<'T> (predicate:'T -> bool,trueTemplate:XmlNode<'T>,falseTemplate:XmlNode<'T>) =
    let trueNodes = compile trueTemplate
    let falseNodes = compile falseTemplate
    BindIf(predicate,trueNodes,falseNodes)

let inline bind<'T>(map:'T -> string) = Bind(map)
let inline htmlx attr children = ParentNode("html", attr, children) 

let template = 
    htmlx [] [
        bindFor<string>
            (fun model -> model.ToCharArray())
            (htmlx [] [
                bind(fun ch -> ch.ToString() )                
                bind<_>(fun ch -> ch.ToString())
                ]
            )
        
    ] |> compile
