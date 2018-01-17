// Learn more about F# at http://fsharp.org

open System
open System.IO
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open BenchmarkDotNet.Reports
open BenchmarkDotNet.Order
open BenchmarkDotNet.Validators
open System.Text
open System.Threading
open Common
open ByteViews
open Giraffe.Tasks

[<Literal>] let dir = @"C:\Temp\" //  Environment.CurrentDirectory

[<MemoryDiagnoser>]
type ViewTests() =

    let counter = ref 0

    let index () = Interlocked.Increment(counter) |> string

    let person =     {
        FirstName = "Snake"
        LastName  = "Plisken"
        BirthDate = DateTime(1954,7,12)
        Height    = 201.
        Piercings = [||]
    }

    let buffer = Array.zeroCreate<byte>(1000)

    [<Benchmark>]
    member x.GiraffeView () = 
        //use fs = new MemoryStream(buffer)
        use fs =  new FileStream(dir + @"giraffeView1.html",FileMode.OpenOrCreate)
        use writer = new StreamWriter(fs)
        let document = GiraffeViews.personView person
        GiraffeViewEngine.renderHtmlDocument document writer |> ignore

    [<Benchmark>]
    member x.XmlView () =
        //use fs = new MemoryStream(buffer)
        use fs =  new FileStream(dir + @"XmlView1.html",FileMode.OpenOrCreate)
        use writer = new StreamWriter(fs)
        let document = XmlViews.personView person
        let str = XmlViewEngine.renderHtmlDocument document 
        writer.Write str

    [<Benchmark(Baseline=true)>]
    member x.ByteView () =
        //use fs = new MemoryStream(buffer)
        use fs =  new FileStream(dir + @"ByteView1.html",FileMode.OpenOrCreate)
        //use writer = new StreamWriter(fs)
        let document = ByteViews.personView person
        ByteViewEngine.renderHtmlDocument document fs

    [<Benchmark>]
    member x.ByteViewAsync () =
        task {
            //use fs = new MemoryStream(buffer)
            use fs =  new FileStream(dir + @"ByteViewAsync1.html",FileMode.OpenOrCreate)
            //use writer = new StreamWriter(fs)
            let document = ByteViewsAsync.personView person
            return! ByteViewEngineAsync.renderHtmlDocument document fs            
        }


    [<Benchmark>]
    member x.TemplateView () = 
        //use fs = new MemoryStream(buffer)
        use fs =  new FileStream(dir + @"TemplateView1.html",FileMode.OpenOrCreate)
        let document = TemplateViews.personView
        TemplateViewEngine.renderHtmlDocument person document fs

[<EntryPoint>]
let main argv =
    printfn "Running Benchmarks, output at '%s' ..." dir

    //printfn "%A" TemplateViews.view1
    BenchmarkRunner.Run<ViewTests>()
    0 // return an integer exit code
