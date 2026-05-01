```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.8246)
Unknown processor
.NET SDK 10.0.103
  [Host]   : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                 | Mean          | Error      | StdDev    | Allocated |
|----------------------- |--------------:|-----------:|----------:|----------:|
| ToAstronomicalJD       |    11.7359 ns |  1.2807 ns | 0.0702 ns |         - |
| FromAstronomicalJD     |    11.9438 ns |  1.6087 ns | 0.0882 ns |         - |
| ToModifiedJulianDate   |    11.9534 ns |  1.2349 ns | 0.0677 ns |         - |
| FromModifiedJulianDate |    12.3780 ns |  0.6746 ns | 0.0370 ns |         - |
| ToOrdinalDate          |     3.2129 ns |  7.1746 ns | 0.3933 ns |         - |
| FromOrdinalDate        |     1.4742 ns |  0.8574 ns | 0.0470 ns |         - |
| ToVietnameseLunar      | 1,430.0815 ns | 24.1761 ns | 1.3252 ns |         - |
| FromVietnameseLunar    | 1,374.5595 ns | 63.5028 ns | 3.4808 ns |         - |
| GetSolarTerm           |    36.0802 ns |  0.1006 ns | 0.0055 ns |         - |
| GetSolarTermName       |    38.0078 ns |  1.0606 ns | 0.0581 ns |         - |
| SunLongitude           |    20.7244 ns |  0.3554 ns | 0.0195 ns |         - |
| NewMoon                |     0.0151 ns |  0.0061 ns | 0.0003 ns |         - |
| LutDecode              |     0.0000 ns |  0.0000 ns | 0.0000 ns |         - |
