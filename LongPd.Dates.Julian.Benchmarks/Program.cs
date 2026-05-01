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
    private readonly DateTime _lunarDate = new DateTime(2025, 5, 1);

    // ── Julian / MJD / Ordinal ──────────────────────────────────────────────

    [Benchmark] public double ToAstronomicalJD()     => _date.ToAstronomicalJD();
    [Benchmark] public DateTime FromAstronomicalJD() => _jd.FromAstronomicalJD();
    [Benchmark] public double ToModifiedJulianDate() => _date.ToModifiedJulianDate();
    [Benchmark] public DateTime FromModifiedJulianDate() => _mjd.FromModifiedJulianDate();
    [Benchmark] public int ToOrdinalDate()           => _date.ToOrdinalDate();
    [Benchmark] public DateTime FromOrdinalDate()    => _ord.FromOrdinalDate();

    // ── Vietnamese Lunar Calendar ───────────────────────────────────────────

    [Benchmark] public LunarDate ToVietnameseLunar() => _lunarDate.ToVietnameseLunar();
    [Benchmark] public DateTime FromVietnameseLunar() => JulianExtensions.FromVietnameseLunar(5, 4, 2025);
    [Benchmark] public int GetSolarTerm()             => _lunarDate.GetSolarTerm();
    [Benchmark] public string GetSolarTermName()      => _lunarDate.GetSolarTermName();

    // ── Astronomical Core ───────────────────────────────────────────────────

    [Benchmark] public double SunLongitude()  => VietnameseLunarCalendar.SunLongitude(_jd);
    [Benchmark] public double NewMoon()       => VietnameseLunarCalendar.NewMoon(1533);
    [Benchmark] public int LutDecode()        => VietnameseLunarCalendar.DecodeLutEntry(2025);
}
