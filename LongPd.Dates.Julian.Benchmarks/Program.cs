using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using LongPd.Dates.Julian;

BenchmarkRunner.Run<JulianBenchmarks>();

[MemoryDiagnoser]
[ShortRunJob]
public class JulianBenchmarks
{
    private readonly DateTime _date = new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private readonly double   _jd  = 2451545.0;
    private readonly double   _mjd = 51544.5;
    private readonly int      _ord = 2026119;

    [Benchmark] public double ToAstronomicalJD()     => _date.ToAstronomicalJD();
    [Benchmark] public DateTime FromAstronomicalJD() => _jd.FromAstronomicalJD();
    [Benchmark] public double ToModifiedJulianDate() => _date.ToModifiedJulianDate();
    [Benchmark] public DateTime FromModifiedJulianDate() => _mjd.FromModifiedJulianDate();
    [Benchmark] public int ToOrdinalDate()           => _date.ToOrdinalDate();
    [Benchmark] public DateTime FromOrdinalDate()    => _ord.FromOrdinalDate();
}
