``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-ZGAKQL : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=3  WarmupCount=2  

```
|                               Method |      Mean |      Error |    StdDev |    Gen0 |    Gen1 |  Allocated |
|------------------------------------- |----------:|-----------:|----------:|--------:|--------:|-----------:|
| &#39;High Element Count (500+ elements)&#39; | 320.33 μs | 369.936 μs | 20.277 μs | 82.5195 | 38.0859 | 1015.08 KB |
| &#39;High Style Diversity (100+ unique)&#39; |  94.21 μs |   3.175 μs |  0.174 μs | 23.6816 |  4.1504 |  290.38 KB |
|     &#39;Repeated Binding Parse (1000x)&#39; | 165.30 μs |  59.723 μs |  3.274 μs | 25.3906 |       - |   312.5 KB |
