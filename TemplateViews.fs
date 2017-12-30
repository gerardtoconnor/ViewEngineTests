module TemplateViews

open TemplateViewEngine
open Common
let view1 : CompiledNode<unit> [] = 
    div [] [
        comment "this is a test"
        h1 [] [ encodedText "Header" ]
        p [] [
            encodedText "Lorem "
            strong [] [ encodedText "Ipsum" ]
            rawText " dollar"
    ] ] |> compile


let personView =
        html [] [
            head [] [
                title [] [ encodedText "Html Node" ]
            ]
            body [] [
                p [] [ 
                    bind<Person>(fun model -> sprintf "%s %s is %i years old." model.FirstName model.LastName (let ds = System.DateTime.Now - model.BirthDate in ds.Days / 365 ))  
                    ]
            ]
        ] |> compile

//let johnDoe = { Foo = "John"; Bar = "Doe"; Age = 30 }