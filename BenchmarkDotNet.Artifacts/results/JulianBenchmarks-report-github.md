```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26200.8246)
Unknown processor
.NET SDK 10.0.103
  [Host]    : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  MediumRun : .NET 8.0.24 (8.0.2426.7010), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=MediumRun  IterationCount=15  LaunchCount=2  
WarmupCount=10  

```
| Method                 | Mean          | Error     | StdDev    | Allocated |
|----------------------- |--------------:|----------:|----------:|----------:|
| ToAstronomicalJD       |    11.7999 ns | 0.0076 ns | 0.0109 ns |         - |
| FromAstronomicalJD     |    12.0183 ns | 0.0229 ns | 0.0313 ns |         - |
| ToModifiedJulianDate   |    12.7018 ns | 1.3517 ns | 1.9814 ns |         - |
| FromModifiedJulianDate |    19.6568 ns | 0.3602 ns | 0.5280 ns |         - |
| ToOrdinalDate          |     2.9040 ns | 0.3871 ns | 0.5794 ns |         - |
| FromOrdinalDate        |     2.0899 ns | 0.5736 ns | 0.8041 ns |         - |
| ToVietnameseLunar      | 1,155.3710 ns | 0.8304 ns | 1.2171 ns |         - |
| FromVietnameseLunar    | 1,095.1973 ns | 0.5368 ns | 0.8035 ns |         - |
| GetSolarTerm           |    36.3261 ns | 0.0315 ns | 0.0461 ns |         - |
| GetSolarTermName       |    38.1493 ns | 0.0240 ns | 0.0344 ns |         - |
| SunLongitude           |    20.8553 ns | 0.0271 ns | 0.0397 ns |         - |
| NewMoon                |     0.0000 ns | 0.0000 ns | 0.0000 ns |         - |
| LutDecode              |     0.0009 ns | 0.0007 ns | 0.0010 ns |         - |
