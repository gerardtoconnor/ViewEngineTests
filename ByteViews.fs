module ByteViews

open ByteViewEngine 

let view1 () = div [] [
        comment "this is a test"
        h1 [] [ encodedText "Header" ]
        p [] [
            encodedText "Lorem "
            strong [] [ encodedText "Ipsum" ]
            rawText " dollar"
    ] ]

