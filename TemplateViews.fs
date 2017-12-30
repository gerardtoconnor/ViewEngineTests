module TemplateViews

open TemplateViewEngine
let view1 : CompiledNode<unit> [] = 
    div [] [
        comment "this is a test"
        h1 [] [ encodedText "Header" ]
        p [] [
            encodedText "Lorem "
            strong [] [ encodedText "Ipsum" ]
            rawText " dollar"
    ] ] |> compile