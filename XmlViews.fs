module XmlViews

open XmlViewEngine
open Common

let view1 () = div [] [
        comment "this is a test"
        h1 [] [ encodedText "Header" ]
        p [] [
            EncodedText "Lorem "
            strong [] [ encodedText "Ipsum" ]
            RawText " dollar"
    ] ]

let personView (model:Person) =
        html [] [
            head [] [
                title [] [ encodedText "Html Node" ]
            ]
            body [] [
                p [] [ 
                    sprintf "%s %s is %i years old." model.FirstName model.LastName (let ds = System.DateTime.Now - model.BirthDate in ds.Days / 365 ) |> EncodedText  
                    ]
            ]
        ]