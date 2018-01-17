Testing various alternative view engine implimentations

- XmlView - original Suave style template
- GiraffeView - struct/enum variation
- ByteView - lightwieght streamwriter nodes
- TemplateView - Alt syntax templateing engine of compiled components 

1 - using memory stream to assess raw performance without IO 

``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.192)
Processor=Intel Core i7-4770K CPU 3.50GHz (Haswell), ProcessorCount=8
Frequency=3415989 Hz, Resolution=292.7410 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT DEBUG
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|        Method |     Mean |     Error |    StdDev | Scaled | ScaledSD |  Gen 0 | Allocated |
|-------------- |---------:|----------:|----------:|-------:|---------:|-------:|----------:|
|   GiraffeView | 4.739 us | 0.0504 us | 0.0447 us |   3.11 |     0.03 | 1.4801 |    6240 B |
|       XmlView | 8.586 us | 0.1155 us | 0.1081 us |   5.63 |     0.07 | 3.3417 |   14040 B |
|      ByteView | 1.525 us | 0.0053 us | 0.0044 us |   1.00 |     0.00 | 0.2899 |    1224 B |
| ByteViewAsync | 5.287 us | 0.0427 us | 0.0400 us |   3.47 |     0.03 | 1.1673 |    4904 B |
|  TemplateView | 1.407 us | 0.0122 us | 0.0114 us |   0.92 |     0.01 | 0.1469 |     624 B |


2 - Test on filesystem to see how much IO impacts perf and if benfit material

``` ini

BenchmarkDotNet=v0.10.11, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.192)
Processor=Intel Core i7-4770K CPU 3.50GHz (Haswell), ProcessorCount=8
Frequency=3415989 Hz, Resolution=292.7410 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT DEBUG
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|        Method |     Mean |    Error |   StdDev | Scaled | ScaledSD |  Gen 0 | Allocated |
|-------------- |---------:|---------:|---------:|-------:|---------:|-------:|----------:|
|   GiraffeView | 240.4 us | 4.754 us | 6.664 us |   1.00 |     0.04 | 2.4414 |   10.3 KB |
|       XmlView | 256.0 us | 3.271 us | 3.060 us |   1.07 |     0.03 | 3.9063 |  17.91 KB |
|      ByteView | 239.5 us | 4.627 us | 6.017 us |   1.00 |     0.00 | 1.2207 |   5.39 KB |
| ByteViewAsync | 337.9 us | 5.047 us | 4.214 us |   1.41 |     0.04 | 4.8828 |   2.69 KB |
|  TemplateView | 244.2 us | 4.527 us | 5.031 us |   1.02 |     0.03 | 0.9766 |   4.81 KB |