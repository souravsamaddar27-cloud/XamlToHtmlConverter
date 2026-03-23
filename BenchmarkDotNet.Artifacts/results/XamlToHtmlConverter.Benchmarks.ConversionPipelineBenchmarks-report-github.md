``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-TEQRER : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                   Method |     Mean |    Error |  StdDev | Rank |    Gen0 |   Gen1 | Allocated |
|------------------------- |---------:|---------:|--------:|-----:|--------:|-------:|----------:|
| &#39;Sample XAML Conversion&#39; | 310.1 μs | 32.20 μs | 8.36 μs |    2 | 24.4141 | 4.8828 |  300.2 KB |
|  &#39;Large XAML Conversion&#39; | 340.8 μs | 19.10 μs | 2.96 μs |    3 | 30.2734 | 6.8359 | 374.14 KB |
|     &#39;Parsing Phase Only&#39; | 187.2 μs | 19.16 μs | 2.96 μs |    1 |  9.2773 | 2.9297 | 119.26 KB |
|   &#39;Rendering Phase Only&#39; | 303.6 μs | 21.75 μs | 5.65 μs |    2 | 24.4141 | 4.8828 |  300.2 KB |
