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

[<Literal>] let dir = @"C:\Temp" //  Environment.CurrentDirectory

[<MemoryDiagnoser>]
type ViewTests() =

    let counter = ref 0

    let index () = Interlocked.Increment(counter) |> string

    //[<Benchmark(Baseline=true)>]
    member x.GiraffeView () = 
        use fs =  new FileStream(dir + @"\giraffeView1.html",FileMode.OpenOrCreate)
        use writer = new StreamWriter(fs)
        let document = GiraffeViews.view1 ()
        GiraffeViewEngine.renderHtmlDocument document writer |> ignore

    //[<Benchmark>]
    member x.XmlView () = 
        let document = XmlViews.view1 ()
        let str = XmlViewEngine.renderHtmlDocument document 
        File.WriteAllText(dir + @"\xmlView1.html",str,Encoding.UTF8) 

    [<Benchmark(Baseline=true)>]
    member x.ByteView () = 
        use fs =  new FileStream(dir + @"\ByteView1.html",FileMode.OpenOrCreate)
        use writer = new StreamWriter(fs)
        let document = ByteViews.view1 ()
        ByteViewEngine.renderHtmlDocument document writer

    [<Benchmark>]
    member x.TemplateView () = 
        use fs =  new FileStream(dir + @"\TemplateView1.html",FileMode.OpenOrCreate)
        use writer = new StreamWriter(fs)
        let document = TemplateViews.view1
        TemplateViewEngine.renderHtmlDocument () document writer

[<EntryPoint>]
let main argv =
    printfn "Running Benchmarks, output at '%s' ..." dir

    printfn "%A" TemplateViews.view1
    //BenchmarkRunner.Run<ViewTests>()
    0 // return an integer exit code
