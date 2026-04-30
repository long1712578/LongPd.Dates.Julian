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

    [Fact]
    public void ToVietnameseLunar_Always_ThrowsNotImplementedException()
    {
        Assert.Throws<NotImplementedException>(() => DateTime.Now.ToVietnameseLunar());
    }
}