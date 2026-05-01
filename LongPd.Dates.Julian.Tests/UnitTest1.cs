using LongPd.Dates.Julian;

namespace LongPd.Dates.Julian.Tests;

/// <summary>
/// Unit tests for JulianExtensions covering JD, MJD, and Ordinal Date conversion.
/// </summary>
public class JulianExtensionsTests
{

    private const double Tolerance = 1e-6;

    [Fact]
    public void ToAstronomicalJD_J2000Epoch_ReturnsKnownValue()
    {
        // J2000.0 = 2000-01-01 12:00:00 UTC → JD 2451545.0
        var date = new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        double jd = date.ToAstronomicalJD();
        Assert.Equal(2451545.0, jd, 5);
    }

    [Fact]
    public void ToAstronomicalJD_UnixEpoch_ReturnsKnownValue()
    {
        // 1970-01-01 00:00:00 UTC → JD 2440587.5
        var date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(2440587.5, date.ToAstronomicalJD(), 5);
    }

    [Fact]
    public void ToAstronomicalJD_LeapDay_DoesNotThrow()
    {
        // 2000-02-29 exists (year 2000 is a leap year) — century divisible by 400
        var leapDay = new DateTime(2000, 2, 29, 0, 0, 0, DateTimeKind.Utc);
        double jd = leapDay.ToAstronomicalJD();
        Assert.True(jd > 0);
    }

    [Fact]
    public void ToAstronomicalJD_LocalKind_ConvertsToUtcFirst()
    {
        // Local and UTC of the same instant must produce the same JD
        var utc = new DateTime(2025, 6, 15, 8, 0, 0, DateTimeKind.Utc);
        var local = utc.ToLocalTime(); // DateTimeKind.Local

        double jdUtc = utc.ToAstronomicalJD();
        double jdLocal = local.ToAstronomicalJD();

        Assert.Equal(jdUtc, jdLocal, 5);
    }

    [Fact]
    public void FromAstronomicalJD_J2000Epoch_ReturnsCorrectDateTime()
    {
        double jd = 2451545.0;
        DateTime result = jd.FromAstronomicalJD();

        Assert.Equal(2000, result.Year);
        Assert.Equal(1, result.Month);
        Assert.Equal(1, result.Day);
        Assert.Equal(12, result.Hour);
        Assert.Equal(0, result.Minute);
        Assert.Equal(0, result.Second);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }

    [Fact]
    public void FromAstronomicalJD_UnixEpoch_ReturnsCorrectDateTime()
    {
        var result = 2440587.5.FromAstronomicalJD();

        Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result);
    }


    [Theory]
    [InlineData(2000, 1, 1, 0, 0, 0, 0)]       // Start of Y2K
    [InlineData(2000, 12, 31, 23, 59, 59, 999)] // End of Y2K (non-leap Dec)
    [InlineData(2000, 2, 29, 12, 30, 0, 0)]     // Leap day noon
    [InlineData(1900, 1, 1, 0, 0, 0, 0)]        // Non-leap century boundary
    [InlineData(2024, 2, 29, 6, 15, 30, 500)]   // Modern leap day
    [InlineData(1999, 12, 31, 23, 59, 59, 0)]   // Day before Y2K
    public void ToAstronomicalJD_ThenFromAstronomicalJD_RoundTrips(
        int year, int month, int day, int hour, int minute, int second, int ms)
    {
        var original = new DateTime(year, month, day, hour, minute, second, ms, DateTimeKind.Utc);
        var recovered = original.ToAstronomicalJD().FromAstronomicalJD();

        Assert.Equal(original, recovered);
    }


    [Fact]
    public void ToModifiedJulianDate_MjdEpoch_ReturnsZero()
    {
        // MJD epoch: 1858-11-17 00:00 UTC → MJD = 0
        var mjdEpoch = new DateTime(1858, 11, 17, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(0.0, mjdEpoch.ToModifiedJulianDate(), 5);
    }

    [Fact]
    public void ToModifiedJulianDate_J2000_Returns51544point5()
    {
        // J2000.0 noon → JD 2451545.0 → MJD = 2451545.0 - 2400000.5 = 51544.5
        var j2000 = new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        Assert.Equal(51544.5, j2000.ToModifiedJulianDate(), 5);
    }

    [Fact]
    public void FromModifiedJulianDate_RoundTrip()
    {
        var original = new DateTime(2026, 4, 29, 9, 30, 0, 0, DateTimeKind.Utc);
        double mjd = original.ToModifiedJulianDate();
        DateTime recovered = mjd.FromModifiedJulianDate();
        Assert.Equal(original, recovered);
    }

    [Theory]
    [InlineData(2026, 1, 1, 2026001)]   // First day of year
    [InlineData(2026, 12, 31, 2026365)] // Last day of non-leap year
    [InlineData(2000, 2, 29, 2000060)]  // Leap day (day 60 in leap year)
    [InlineData(2000, 12, 31, 2000366)] // Last day of leap year
    [InlineData(2025, 4, 29, 2025119)]  // Arbitrary mid-year date
    public void ToOrdinalDate_KnownDates_ReturnCorrectOrdinal(
        int year, int month, int day, int expected)
    {
        var date = new DateTime(year, month, day);
        Assert.Equal(expected, date.ToOrdinalDate());
    }


    [Theory]
    [InlineData(2026001, 2026, 1, 1)]
    [InlineData(2026365, 2026, 12, 31)]
    [InlineData(2000060, 2000, 2, 29)]  // Leap day
    [InlineData(2000366, 2000, 12, 31)]
    [InlineData(2025119, 2025, 4, 29)]
    public void FromOrdinalDate_KnownOrdinals_ReturnCorrectDate(
        int ordinal, int year, int month, int day)
    {
        DateTime result = ordinal.FromOrdinalDate();
        Assert.Equal(new DateTime(year, month, day), result);
    }

    [Fact]
    public void FromOrdinalDate_DayZero_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => 2026000.FromOrdinalDate());
    }

    [Fact]
    public void FromOrdinalDate_Day366InNonLeapYear_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => 2026366.FromOrdinalDate());
    }

    [Fact]
    public void FromOrdinalDate_Day367InLeapYear_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => 2000367.FromOrdinalDate());
    }

    [Theory]
    [InlineData(2000, 1, 1)]
    [InlineData(2000, 2, 29)]
    [InlineData(2000, 12, 31)]
    [InlineData(1900, 3, 1)]   // Century non-leap year
    [InlineData(2400, 2, 29)]  // Divisible-by-400 century
    public void ToOrdinalDate_ThenFromOrdinalDate_RoundTrips(int year, int month, int day)
    {
        var original = new DateTime(year, month, day);
        int ordinal = original.ToOrdinalDate();
        DateTime recovered = ordinal.FromOrdinalDate();
        Assert.Equal(original, recovered);
    }

    // ── Year 1 (earliest DateTime year) ─────────────────────────────────────

    [Fact]
    public void ToAstronomicalJD_Year1Jan1_RoundTrips()
    {
        // DateTime minimum year = 1 AD (no year 0 in DateTime)
        var date = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime recovered = date.ToAstronomicalJD().FromAstronomicalJD();
        Assert.Equal(date, recovered);
    }

    [Fact]
    public void ToAstronomicalJD_Year1Dec31_RoundTrips()
    {
        var date = new DateTime(1, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
        DateTime recovered = date.ToAstronomicalJD().FromAstronomicalJD();
        Assert.Equal(date, recovered);
    }

    // ── Leap year: 2000 (divisible by 400 → IS leap) ────────────────────────

    [Fact]
    public void ToAstronomicalJD_Year2000_IsLeapYear_Feb29RoundTrips()
    {
        // 2000 is divisible by 400 → leap year
        var leapDay = new DateTime(2000, 2, 29, 0, 0, 0, DateTimeKind.Utc);
        DateTime recovered = leapDay.ToAstronomicalJD().FromAstronomicalJD();
        Assert.Equal(leapDay, recovered);
    }

    [Fact]
    public void ToAstronomicalJD_Year2000_Mar1_IsOneDayAfterLeapDay()
    {
        var feb29 = new DateTime(2000, 2, 29, 0, 0, 0, DateTimeKind.Utc);
        var mar01 = new DateTime(2000, 3, 1, 0, 0, 0, DateTimeKind.Utc);
        double diff = mar01.ToAstronomicalJD() - feb29.ToAstronomicalJD();
        Assert.Equal(1.0, diff, 9);
    }

    // ── Leap year: 1900 (divisible by 100 but NOT 400 → NOT leap) ───────────

    [Fact]
    public void ToAstronomicalJD_Year1900_IsNotLeapYear_Feb28RoundTrips()
    {
        // 1900 is NOT a leap year → Feb 28 is the last day of February
        var feb28 = new DateTime(1900, 2, 28, 0, 0, 0, DateTimeKind.Utc);
        DateTime recovered = feb28.ToAstronomicalJD().FromAstronomicalJD();
        Assert.Equal(feb28, recovered);
    }

    [Fact]
    public void ToAstronomicalJD_Year1900_Mar1_IsOneDayAfterFeb28()
    {
        // In a non-leap year the gap Feb28 → Mar1 must be exactly 1 day
        var feb28 = new DateTime(1900, 2, 28, 0, 0, 0, DateTimeKind.Utc);
        var mar01 = new DateTime(1900, 3, 1, 0, 0, 0, DateTimeKind.Utc);
        double diff = mar01.ToAstronomicalJD() - feb28.ToAstronomicalJD();
        Assert.Equal(1.0, diff, 9);
    }

    [Fact]
    public void DateTime_Year1900_Feb29_ThrowsArgumentOutOfRange()
    {
        // Confirms at the .NET level that 1900 has no Feb 29
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new DateTime(1900, 2, 29));
    }

    // ── Leap year: 2024 (divisible by 4, not by 100 → IS leap) ─────────────

    [Fact]
    public void ToAstronomicalJD_Year2024_IsLeapYear_Feb29RoundTrips()
    {
        var leapDay = new DateTime(2024, 2, 29, 15, 45, 30, 123, DateTimeKind.Utc);
        DateTime recovered = leapDay.ToAstronomicalJD().FromAstronomicalJD();
        Assert.Equal(leapDay, recovered);
    }

    [Fact]
    public void ToAstronomicalJD_Year2024_Mar1_IsOneDayAfterLeapDay()
    {
        var feb29 = new DateTime(2024, 2, 29, 0, 0, 0, DateTimeKind.Utc);
        var mar01 = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc);
        double diff = mar01.ToAstronomicalJD() - feb29.ToAstronomicalJD();
        Assert.Equal(1.0, diff, 9);
    }

    // ── DateTime.MinValue / MaxValue ─────────────────────────────────────────

    [Fact]
    public void ToAstronomicalJD_DateTimeMinValue_RoundTrips()
    {
        // DateTime.MinValue = 0001-01-01 00:00:00.000 — use explicit UTC to avoid
        // ToUniversalTime() underflow on positive-offset timezones
        var min = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        DateTime recovered = min.ToAstronomicalJD().FromAstronomicalJD();
        Assert.Equal(min, recovered);
    }

    [Fact]
    public void ToAstronomicalJD_DateTimeMaxValue_RoundTrips()
    {
        // DateTime.MaxValue = 9999-12-31 23:59:59.999
        var max = new DateTime(9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
        DateTime recovered = max.ToAstronomicalJD().FromAstronomicalJD();
        Assert.Equal(max, recovered);
    }

    [Fact]
    public void ToAstronomicalJD_DateTimeMinValue_IsLessThanJ2000()
    {
        var min = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.True(min.ToAstronomicalJD() < 2451545.0);
    }

    [Fact]
    public void ToAstronomicalJD_DateTimeMaxValue_IsGreaterThanJ2000()
    {
        var max = new DateTime(9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Utc);
        Assert.True(max.ToAstronomicalJD() > 2451545.0);
    }

    // ── Gregorian reform boundary (1582-10-15) ───────────────────────────────

    [Fact]
    public void ToAstronomicalJD_GregorianReformFirstDay_RoundTrips()
    {
        // 1582-10-15 is the first day of the Gregorian calendar
        var reformDay = new DateTime(1582, 10, 15, 0, 0, 0, DateTimeKind.Utc);
        DateTime recovered = reformDay.ToAstronomicalJD().FromAstronomicalJD();
        Assert.Equal(reformDay, recovered);
    }
}

/// <summary>
/// Unit tests for Vietnamese Lunar Calendar conversion.
/// </summary>
public class VietnameseLunarTests
{
    // ── Known Tết (Lunar New Year) dates ──────────────────────────────────────

    [Theory]
    [InlineData(2025, 1, 29, 1, 1, 2025, false)]  // Tết Ất Tỵ 2025
    [InlineData(2024, 2, 10, 1, 1, 2024, false)]  // Tết Giáp Thìn 2024
    [InlineData(2023, 1, 22, 1, 1, 2023, false)]  // Tết Quý Mão 2023
    [InlineData(2020, 1, 25, 1, 1, 2020, false)]  // Tết Canh Tý 2020
    public void ToVietnameseLunar_TetDates_ReturnsLunarNewYear(
        int gYear, int gMonth, int gDay,
        int lDay, int lMonth, int lYear, bool isLeap)
    {
        var date = new DateTime(gYear, gMonth, gDay);
        var lunar = date.ToVietnameseLunar();

        Assert.Equal(lDay, lunar.Day);
        Assert.Equal(lMonth, lunar.Month);
        Assert.Equal(lYear, lunar.Year);
        Assert.Equal(isLeap, lunar.IsLeapMonth);
    }

    // ── Mid-month dates ──────────────────────────────────────────────────────

    [Fact]
    public void ToVietnameseLunar_MidAutumnFestival2024()
    {
        // Tết Trung Thu 2024 = 15/08 Âm lịch = Sep 17, 2024
        var date = new DateTime(2024, 9, 17);
        var lunar = date.ToVietnameseLunar();

        Assert.Equal(15, lunar.Day);
        Assert.Equal(8, lunar.Month);
        Assert.Equal(2024, lunar.Year);
        Assert.False(lunar.IsLeapMonth);
    }

    // ── Round-trip: Solar → Lunar → Solar ────────────────────────────────────

    [Theory]
    [InlineData(2025, 5, 1)]
    [InlineData(2024, 2, 10)]
    [InlineData(2000, 6, 15)]
    [InlineData(2050, 12, 31)]
    public void ToVietnameseLunar_ThenFromLunar_RoundTrips(int year, int month, int day)
    {
        var original = new DateTime(year, month, day);
        var lunar = original.ToVietnameseLunar();
        var recovered = JulianExtensions.FromVietnameseLunar(
            lunar.Day, lunar.Month, lunar.Year, lunar.IsLeapMonth);

        Assert.Equal(original, recovered);
    }

    // ── LunarDate struct ─────────────────────────────────────────────────────

    [Fact]
    public void LunarDate_Equality_Works()
    {
        var a = new LunarDate(1, 1, 2025, false);
        var b = new LunarDate(1, 1, 2025, false);
        var c = new LunarDate(2, 1, 2025, false);

        Assert.Equal(a, b);
        Assert.True(a == b);
        Assert.NotEqual(a, c);
        Assert.True(a != c);
    }

    [Fact]
    public void LunarDate_ToString_FormatsCorrectly()
    {
        var normal = new LunarDate(15, 8, 2024, false);
        var leap = new LunarDate(10, 4, 2025, true);

        Assert.Equal("15/08/2024", normal.ToString());
        Assert.Equal("10/04*/2025", leap.ToString());
    }

    // ── LUT decode tests ─────────────────────────────────────────────────────

    [Fact]
    public void DecodeLutEntry_Year2000_ReturnsCorrectValue()
    {
        int entry = VietnameseLunarCalendar.DecodeLutEntry(2000);
        Assert.Equal(0x0C960, entry);
    }

    [Fact]
    public void DecodeLutEntry_Year2086_HasBit16Set()
    {
        int entry = VietnameseLunarCalendar.DecodeLutEntry(2086);
        Assert.Equal(0x104D5, entry);
        Assert.Equal(1, (entry >> 16) & 1); // Leap month has 30 days
    }

    [Fact]
    public void DecodeLutEntry_OutOfRange_ReturnsMinusOne()
    {
        Assert.Equal(-1, VietnameseLunarCalendar.DecodeLutEntry(1999));
        Assert.Equal(-1, VietnameseLunarCalendar.DecodeLutEntry(2101));
    }

    [Fact]
    public void GetLutLeapMonth_Year2005_Returns7()
    {
        // 0x0ABB7 → bits 3-0 = 7
        Assert.Equal(7, VietnameseLunarCalendar.GetLutLeapMonth(2005));
    }

    [Fact]
    public void GetLutLeapMonth_Year2000_ReturnsZero()
    {
        Assert.Equal(0, VietnameseLunarCalendar.GetLutLeapMonth(2000));
    }

    [Fact]
    public void GetLutYearDays_NonLeapYear_Returns354OrLess()
    {
        int days = VietnameseLunarCalendar.GetLutYearDays(2001);
        Assert.InRange(days, 353, 355);
    }

    [Fact]
    public void GetLutYearDays_LeapYear_Returns383To385()
    {
        int days = VietnameseLunarCalendar.GetLutYearDays(2005); // Has leap month 7
        Assert.InRange(days, 383, 385);
    }
}

/// <summary>
/// Unit tests for Solar Term (Tiết Khí) calculations.
/// </summary>
public class SolarTermTests
{
    [Fact]
    public void GetSolarTerm_VernalEquinox2025_ReturnsZero()
    {
        // Vernal Equinox 2025 occurs March 20 at ~09:01 UTC = 16:01 UTC+7.
        // At midnight UTC+7 on March 20, sun is still at ~359° (term 23).
        // Use March 21 to ensure sun has crossed 0°.
        var date = new DateTime(2025, 3, 21);
        int term = date.GetSolarTerm();
        Assert.Equal(0, term); // Xuân Phân
    }

    [Fact]
    public void GetSolarTerm_SummerSolstice2025_Returns6()
    {
        // Summer Solstice 2025 ≈ June 21. Use June 22 to avoid boundary.
        var date = new DateTime(2025, 6, 22);
        int term = date.GetSolarTerm();
        Assert.Equal(6, term); // Hạ Chí
    }

    [Fact]
    public void GetSolarTermName_ReturnsVietnameseName()
    {
        // Use March 21 (safely past the Vernal Equinox boundary)
        var date = new DateTime(2025, 3, 21);
        string name = date.GetSolarTermName();
        Assert.Equal("Xuân Phân", name);
    }

    [Theory]
    [InlineData(0, "Xuân Phân")]
    [InlineData(6, "Hạ Chí")]
    [InlineData(12, "Thu Phân")]
    [InlineData(18, "Đông Chí")]
    public void GetSolarTermName_AllCardinalPoints_ReturnCorrectNames(
        int index, string expected)
    {
        Assert.Equal(expected, VietnameseLunarCalendar.GetSolarTermName(index));
    }

    [Fact]
    public void SunLongitude_J2000Epoch_ReturnsReasonableValue()
    {
        // At J2000.0 (2000-01-01 12:00 UTC), Sun longitude ≈ 280°
        double sunLong = VietnameseLunarCalendar.SunLongitude(2451545.0);
        Assert.InRange(sunLong, 278, 282);
    }

    [Fact]
    public void NewMoon_KnownNewMoon_ReturnsCloseJD()
    {
        // New Moon on 2024-01-11 → JD ≈ 2460320.5
        // k = round((2460320.5 - 2415020.75933) / 29.53058868) = 1534
        double nm = VietnameseLunarCalendar.NewMoon(1534);
        Assert.InRange(nm, 2460319.0, 2460322.0);
    }

    [Fact]
    public void JulianDayFromDate_J2000_ReturnsKnownValue()
    {
        // 2000-01-01 → JD 2451545 (noon)
        int jd = VietnameseLunarCalendar.JulianDayFromDate(1, 1, 2000);
        Assert.Equal(2451545, jd);
    }

    [Fact]
    public void JulianDayToDate_RoundTrips()
    {
        int jd = VietnameseLunarCalendar.JulianDayFromDate(29, 4, 2025);
        VietnameseLunarCalendar.JulianDayToDate(jd, out int d, out int m, out int y);
        Assert.Equal(29, d);
        Assert.Equal(4, m);
        Assert.Equal(2025, y);
    }
}