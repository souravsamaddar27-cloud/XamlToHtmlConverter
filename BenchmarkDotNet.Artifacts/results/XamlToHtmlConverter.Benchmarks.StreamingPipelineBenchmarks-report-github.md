``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-ZGAKQL : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=3  WarmupCount=2  

```
|                                  Method |      Mean |      Error |   StdDev |   Gen0 | Allocated |
|---------------------------------------- |----------:|-----------:|---------:|-------:|----------:|
|   &#39;End-to-End: XAML → File (Streaming)&#39; | 636.88 μs | 111.477 μs | 6.110 μs |      - |    1168 B |
| &#39;End-to-End: XAML → String (Streaming)&#39; |  14.57 μs |   7.582 μs | 0.416 μs | 0.0610 |     872 B |
