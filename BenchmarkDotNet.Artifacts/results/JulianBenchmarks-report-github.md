```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.8246)
Unknown processor
.NET SDK 10.0.103
  [Host]   : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                 | Mean      | Error     | StdDev    | Allocated |
|----------------------- |----------:|----------:|----------:|----------:|
| ToAstronomicalJD       | 11.613 ns | 0.1321 ns | 0.0072 ns |         - |
| FromAstronomicalJD     | 11.804 ns | 0.1591 ns | 0.0087 ns |         - |
| ToModifiedJulianDate   | 11.955 ns | 2.0601 ns | 0.1129 ns |         - |
| FromModifiedJulianDate | 12.247 ns | 0.1727 ns | 0.0095 ns |         - |
| ToOrdinalDate          |  1.775 ns | 0.2895 ns | 0.0159 ns |         - |
| FromOrdinalDate        |  2.129 ns | 0.0285 ns | 0.0016 ns |         - |
