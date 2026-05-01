using System;
using System.Runtime.CompilerServices;

namespace LongPd.Dates.Julian
{
    /// <summary>
    /// High-performance extension methods for Julian Date, Modified Julian Date,
    /// and Ordinal Date conversions. All algorithms based on Jean Meeus,
    /// "Astronomical Algorithms", 2nd Edition.
    /// </summary>
    public static class JulianExtensions
    {
        private const double JdOffset = 2400000.5;

        /// <summary>
        /// Converts a <see cref="DateTime"/> to its Astronomical Julian Date (JD).
        /// </summary>
        /// <param name="date">The date to convert. If not UTC, it will be converted automatically.</param>
        /// <returns>The Astronomical Julian Date as a <see cref="double"/>.</returns>
        /// <remarks>
        /// Uses the Meeus algorithm. Valid for all dates in the proleptic Gregorian calendar.
        /// Dates before 1582-10-15 are treated as Julian calendar dates.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToAstronomicalJD(this DateTime date)
        {
            DateTime utc = date.Kind == DateTimeKind.Utc ? date : date.ToUniversalTime();

            int y = utc.Year;
            int m = utc.Month;
            double d = utc.Day + (utc.Hour + (utc.Minute + (utc.Second + utc.Millisecond / 1000.0) / 60.0) / 60.0) / 24.0;

            if (m <= 2) { y--; m += 12; }

            int a = y / 100;
            int b = 2 - a + (a / 4);

            return Math.Floor(365.25 * (y + 4716)) + Math.Floor(30.6001 * (m + 1)) + d + b - 1524.5;
        }

        /// <summary>
        /// Converts <see cref="DateTime"/> to Modified Julian Date (MJD).
        /// </summary>
        /// <remarks>Formula: MJD = JD − 2400000.5 (epoch: 1858-11-17 00:00 UTC)</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToModifiedJulianDate(this DateTime date)
            => ToAstronomicalJD(date) - JdOffset;

        /// <summary>
        /// Converts a Modified Julian Date (MJD) back to <see cref="DateTime"/> (UTC).
        /// </summary>
        /// <param name="mjd">The Modified Julian Date value.</param>
        /// <returns>A <see cref="DateTime"/> in UTC.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime FromModifiedJulianDate(this double mjd)
            => FromAstronomicalJD(mjd + JdOffset);

        /// <summary>
        /// Converts an Astronomical Julian Date (JD) back to <see cref="DateTime"/> (UTC).
        /// </summary>
        /// <param name="jd">The Astronomical Julian Date value.</param>
        /// <returns>A <see cref="DateTime"/> in UTC corresponding to the given JD.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime FromAstronomicalJD(this double jd)
        {
            double z = Math.Floor(jd + 0.5);
            double f = (jd + 0.5) - z;

            // Always use proleptic Gregorian correction — consistent with ToAstronomicalJD
            // and with .NET DateTime which uses proleptic Gregorian for all dates.
            double alpha = Math.Floor((z - 1867216.25) / 36524.25);
            double a = z + 1 + alpha - Math.Floor(alpha / 4);

            double b = a + 1524;
            double c = Math.Floor((b - 122.1) / 365.25);
            double d = Math.Floor(365.25 * c);
            double e = Math.Floor((b - d) / 30.6001);

            double dayFraction = b - d - Math.Floor(30.6001 * e) + f;
            int month = (int)(e < 14 ? e - 1 : e - 13);
            int year = (int)(month > 2 ? c - 4716 : c - 4715);

            int day = (int)Math.Floor(dayFraction);
            double remainingDay = (dayFraction - day) * 24;
            int hour = (int)Math.Floor(remainingDay);
            double remainingHour = (remainingDay - hour) * 60;
            int minute = (int)Math.Floor(remainingHour);
            double remainingMinute = (remainingHour - minute) * 60;
            int second = (int)Math.Floor(remainingMinute);
            int millisecond = (int)Math.Round((remainingMinute - second) * 1000);

            return new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc);
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> to its ISO 8601 Ordinal Date integer (YYYYDDD).
        /// </summary>
        /// <param name="date">The date to convert.</param>
        /// <returns>An integer in the form YYYYDDD, e.g. 2026119 for April 29, 2026.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToOrdinalDate(this DateTime date) => date.Year * 1000 + date.DayOfYear;

        /// <summary>
        /// Converts an Ordinal Date integer (YYYYDDD) back to a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="ordinalDate">An integer in the form YYYYDDD, e.g. 2026119.</param>
        /// <returns>A <see cref="DateTime"/> with <see cref="DateTimeKind.Unspecified"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the day-of-year component is less than 1 or greater than 366.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime FromOrdinalDate(this int ordinalDate)
        {
            int year = ordinalDate / 1000;
            int doy  = ordinalDate % 1000;

            int daysInYear = DateTime.IsLeapYear(year) ? 366 : 365;
            if (doy < 1 || doy > daysInYear)
                throw new ArgumentOutOfRangeException(nameof(ordinalDate),
                    $"Day-of-year {doy} is out of range for year {year}.");

            return new DateTime(year, 1, 1).AddDays(doy - 1);
        }

        /// <summary>
        /// Converts a Gregorian <see cref="DateTime"/> to the Vietnamese Lunar Calendar date.
        /// Uses astronomical algorithms (New Moon + Sun Longitude) with UTC+7 timezone.
        /// </summary>
        /// <param name="date">The Gregorian date to convert.</param>
        /// <returns>A <see cref="LunarDate"/> with Day, Month, Year, and IsLeapMonth.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LunarDate ToVietnameseLunar(this DateTime date)
            => VietnameseLunarCalendar.ToLunar(date);

        /// <summary>
        /// Converts a Vietnamese Lunar date back to Gregorian <see cref="DateTime"/>.
        /// </summary>
        public static DateTime FromVietnameseLunar(int lunarDay, int lunarMonth, int lunarYear, bool isLeapMonth = false)
            => VietnameseLunarCalendar.FromLunar(lunarDay, lunarMonth, lunarYear, isLeapMonth);

        /// <summary>
        /// Gets the Solar Term index (0-23) for a given Gregorian date.
        /// Based on Sun's ecliptic longitude calculated from Julian Date.
        /// </summary>
        /// <param name="date">The Gregorian date.</param>
        /// <returns>Solar term index (0=Xuân Phân, 1=Thanh Minh, ..., 23=Kinh Trập).</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetSolarTerm(this DateTime date)
            => VietnameseLunarCalendar.GetSolarTermIndex(date);

        /// <summary>
        /// Gets the Vietnamese name of the Solar Term for a given date.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetSolarTermName(this DateTime date)
            => VietnameseLunarCalendar.GetSolarTermName(date.GetSolarTerm());
    }
}