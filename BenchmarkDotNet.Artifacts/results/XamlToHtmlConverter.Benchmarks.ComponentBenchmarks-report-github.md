``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-TEQRER : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                  Method |      Mean |     Error |   StdDev | Rank |   Gen0 |   Gen1 | Allocated |
|------------------------ |----------:|----------:|---------:|-----:|-------:|-------:|----------:|
|  &#39;Simple Binding Parse&#39; |  17.65 ns |  2.080 ns | 0.540 ns |    1 | 0.0070 |      - |      88 B |
| &#39;Complex Binding Parse&#39; |  65.55 ns |  2.627 ns | 0.682 ns |    4 | 0.0095 |      - |     120 B |
|    &#39;Style Registration&#39; |  30.09 ns |  9.092 ns | 1.407 ns |    2 |      - |      - |         - |
|       &#39;Style Cache Hit&#39; |  40.90 ns |  6.391 ns | 1.660 ns |    3 |      - |      - |         - |
|     &#39;Parser: XML to IR&#39; | 250.36 ns | 38.358 ns | 5.936 ns |    5 | 0.1135 | 0.0005 |    1424 B |
|      &#39;XamlLoader: Load&#39; |        NA |        NA |       NA |    ? |      - |      - |         - |

Benchmarks with issues:
  ComponentBenchmarks.'XamlLoader: Load': Job-TEQRER(IterationCount=5, WarmupCount=3)
