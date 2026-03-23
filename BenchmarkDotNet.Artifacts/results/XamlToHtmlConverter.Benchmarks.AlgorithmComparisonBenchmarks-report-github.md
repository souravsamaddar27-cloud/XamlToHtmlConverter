``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.26200.7840)
Unknown processor
.NET SDK=10.0.201
  [Host]     : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2
  Job-TEQRER : .NET 8.0.25 (8.0.2526.11203), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
|                                   Method |         Mean |        Error |       StdDev | Rank |   Gen0 |   Gen1 | Allocated |
|----------------------------------------- |-------------:|-------------:|-------------:|-----:|-------:|-------:|----------:|
|                XmlToIrConverterRecursive | 23,000.57 ns | 8,543.632 ns | 2,218.754 ns |    3 | 6.3477 | 1.9226 |   79928 B |
|                XmlToIrConverterLinqStyle | 20,634.05 ns | 7,091.349 ns | 1,841.601 ns |    3 | 5.6763 | 1.5564 |   71512 B |
|                  &#39;Binding Parse: Simple&#39; |     16.58 ns |     1.537 ns |     0.399 ns |    1 | 0.0070 |      - |      88 B |
| &#39;Style Registry: Multiple Registrations&#39; |  7,398.29 ns |   355.544 ns |    55.021 ns |    2 | 0.4044 |      - |    5096 B |
