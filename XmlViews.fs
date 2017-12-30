module XmlViews

open XmlViewEngine

let view1 () = div [] [
        comment "this is a test"
        h1 [] [ encodedText "Header" ]
        p [] [
            EncodedText "Lorem "
            strong [] [ encodedText "Ipsum" ]
            RawText " dollar"
    ] ]