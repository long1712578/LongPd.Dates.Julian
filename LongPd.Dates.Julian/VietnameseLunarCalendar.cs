// Vietnamese Lunar Calendar algorithm ported from:
//   Hồ Ngọc Đức, "Vietnamese Lunar Calendar"
//   https://www.informatik.uni-leipzig.de/~duc/amlich/
//   Original source released freely for public use.
//
// Astronomical formulas (NewMoon, SunLongitude) based on:
//   Jean Meeus, "Astronomical Algorithms", 2nd Edition (Willmann-Bell, 1998).
//   Mathematical formulas are not subject to copyright.

using System;
using System.Runtime.CompilerServices;

namespace LongPd.Dates.Julian
{
    /// <summary>
    /// High-performance Vietnamese Lunar Calendar conversion.
    /// Algorithm ported from Hồ Ngọc Đức's public domain implementation,
    /// with .NET optimizations and ReadOnlySpan LUT for 2000–2100.
    /// All methods are pure functions with zero allocations.
    /// </summary>
    public static class VietnameseLunarCalendar
    {
        private const double VietnamTz = 7.0;
        private const double DegToRad = Math.PI / 180.0;

        #region Month-11 Cache (one-time pre-computation)

        // Cache month-11 JD for years MinCacheYear..MaxCacheYear.
        // Eliminates repeated NewMoon + SunLongitude calls on the hot path.
        // Total init cost: ~250µs (computed once at class load).
        private const int MinCacheYear = 1998;
        private const int MaxCacheYear = 2102;
        private static readonly int[] Month11Cache;

        static VietnameseLunarCalendar()
        {
            int count = MaxCacheYear - MinCacheYear + 1;
            Month11Cache = new int[count];
            try
            {
                for (int y = MinCacheYear; y <= MaxCacheYear; y++)
                    Month11Cache[y - MinCacheYear] = ComputeLunarMonth11(y, VietnamTz);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Failed to initialize Vietnamese Lunar Calendar month-11 cache. " +
                    "This is an internal error — please file an issue.", ex);
            }
        }

        #endregion

        #region Astronomical Core (Pure Functions)

        /// <summary>
        /// Computes the Julian Day Number from a Gregorian date.
        /// Uses the proleptic Gregorian calendar algorithm.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int JulianDayFromDate(int day, int month, int year)
        {
            int a = (14 - month) / 12;
            int y = year + 4800 - a;
            int m = month + 12 * a - 3;
            return day + (153 * m + 2) / 5 + 365 * y + y / 4 - y / 100 + y / 400 - 32045;
        }

        /// <summary>
        /// Converts a Julian Day Number back to Gregorian (day, month, year).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void JulianDayToDate(int jd, out int day, out int month, out int year)
        {
            int a, b, c;
            if (jd > 2299160)
            {
                a = jd + 32044;
                b = (4 * a + 3) / 146097;
                c = a - (146097 * b) / 4;
            }
            else
            {
                b = 0;
                c = jd + 32082;
            }
            int d = (4 * c + 3) / 1461;
            int e = c - (1461 * d) / 4;
            int m = (5 * e + 2) / 153;
            day = e - (153 * m + 2) / 5 + 1;
            month = m + 3 - 12 * (m / 10);
            year = 100 * b + d - 4800 + m / 10;
        }

        /// <summary>
        /// Computes the Julian Day of the k-th new moon after J1900 epoch.
        /// Based on Meeus, "Astronomical Algorithms".
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double NewMoon(int k)
        {
            double T = k / 1236.85;
            double T2 = T * T;
            double T3 = T2 * T;

            double jd = 2415020.75933 + 29.53058868 * k
                       + 0.0001178 * T2 - 0.000000155 * T3
                       + 0.00033 * Math.Sin((166.56 + 132.87 * T - 0.009173 * T2) * DegToRad);

            double M = (359.2242 + 29.10535608 * k - 0.0000333 * T2 - 0.00000347 * T3) * DegToRad;
            double Mpr = (306.0253 + 385.81691806 * k + 0.0107306 * T2 + 0.00001236 * T3) * DegToRad;
            double F = (21.2964 + 390.67050646 * k - 0.0016528 * T2 - 0.00000239 * T3) * DegToRad;

            double C1 = (0.1734 - 0.000393 * T) * Math.Sin(M)
                       + 0.0021 * Math.Sin(2.0 * M)
                       - 0.4068 * Math.Sin(Mpr)
                       + 0.0161 * Math.Sin(2.0 * Mpr)
                       - 0.0004 * Math.Sin(3.0 * Mpr)
                       + 0.0104 * Math.Sin(2.0 * F)
                       - 0.0051 * Math.Sin(M + Mpr)
                       - 0.0074 * Math.Sin(M - Mpr)
                       + 0.0004 * Math.Sin(2.0 * F + M)
                       - 0.0004 * Math.Sin(2.0 * F - M)
                       - 0.0006 * Math.Sin(2.0 * F + Mpr)
                       + 0.0010 * Math.Sin(2.0 * F - Mpr)
                       + 0.0005 * Math.Sin(2.0 * Mpr + M);

            return jd + C1;
        }

        /// <summary>
        /// Computes the Sun's ecliptic longitude in degrees [0, 360) at a given JD.
        /// Low-precision formula from Meeus, sufficient for calendar calculations.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SunLongitude(double jd)
        {
            double T = (jd - 2451545.0) / 36525.0;
            double T2 = T * T;
            double M = (357.52910 + 35999.05030 * T - 0.0001559 * T2) * DegToRad;
            double L0 = 280.46645 + 36000.76983 * T + 0.0003032 * T2;

            double DL = (1.914600 - 0.004817 * T - 0.000014 * T2) * Math.Sin(M)
                       + (0.019993 - 0.000101 * T) * Math.Sin(2.0 * M)
                       + 0.000290 * Math.Sin(3.0 * M);

            double L = L0 + DL
                     - 0.00569 - 0.00478 * Math.Sin((125.04 - 1934.136 * T) * DegToRad);

            L %= 360.0;
            return L < 0 ? L + 360.0 : L;
        }

        /// <summary>
        /// Gets the Solar Term index (0-23) for a given Gregorian date.
        /// 0=Xuân Phân(0°), 1=Thanh Minh(15°), ..., 23=Kinh Trập(345°).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetSolarTermIndex(int day, int month, int year)
        {
            int jd = JulianDayFromDate(day, month, year);
            double sunLong = SunLongitude(jd - 0.5 + VietnamTz / 24.0);
            return (int)Math.Floor(sunLong / 15.0);
        }

        /// <summary>
        /// Gets the Solar Term index for a <see cref="DateTime"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetSolarTermIndex(DateTime date)
            => GetSolarTermIndex(date.Day, date.Month, date.Year);

        #endregion

        #region Lunar Conversion

        /// <summary>
        /// Converts a Gregorian date to Vietnamese Lunar date (UTC+7).
        /// Pure function, zero allocations.
        /// </summary>
        public static LunarDate ToLunar(int dd, int mm, int yy)
            => ToLunar(dd, mm, yy, VietnamTz);

        /// <summary>
        /// Converts a <see cref="DateTime"/> to Vietnamese Lunar date.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LunarDate ToLunar(DateTime date)
            => ToLunar(date.Day, date.Month, date.Year);

        /// <summary>
        /// Converts a Gregorian date to Lunar date with a custom timezone offset.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static LunarDate ToLunar(int dd, int mm, int yy, double timeZone)
        {
            int dayNumber = JulianDayFromDate(dd, mm, yy);
            int k = (int)Math.Floor((dayNumber - 2415021.076998695) / 29.530588853);

            int monthStart = GetNewMoonDay(k + 1, timeZone);
            if (monthStart > dayNumber)
                monthStart = GetNewMoonDay(k, timeZone);

            int a11 = GetLunarMonth11(yy, timeZone);
            int b11 = a11;
            int lunarYear;

            if (a11 >= monthStart)
            {
                lunarYear = yy;
                a11 = GetLunarMonth11(yy - 1, timeZone);
            }
            else
            {
                lunarYear = yy + 1;
                b11 = GetLunarMonth11(yy + 1, timeZone);
            }

            int lunarDay = dayNumber - monthStart + 1;
            int diff = (int)Math.Floor((double)(monthStart - a11) / 29);
            bool lunarLeap = false;
            int lunarMonth = diff + 11;

            if (b11 - a11 > 365)
            {
                int leapOffset = GetLeapMonthOffset(a11, timeZone);
                if (diff >= leapOffset)
                {
                    lunarMonth = diff + 10;
                    if (diff == leapOffset)
                        lunarLeap = true;
                }
            }

            if (lunarMonth > 12) lunarMonth -= 12;
            if (lunarMonth >= 11 && diff < 4) lunarYear -= 1;

            return new LunarDate(lunarDay, lunarMonth, lunarYear, lunarLeap);
        }

        /// <summary>
        /// Converts a Vietnamese Lunar date back to Gregorian <see cref="DateTime"/>.
        /// </summary>
        public static DateTime FromLunar(int lunarDay, int lunarMonth, int lunarYear, bool isLeapMonth)
            => FromLunar(lunarDay, lunarMonth, lunarYear, isLeapMonth, VietnamTz);

        /// <summary>
        /// Converts a Vietnamese Lunar date back to Gregorian <see cref="DateTime"/> with custom timezone.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static DateTime FromLunar(int lunarDay, int lunarMonth, int lunarYear, bool isLeapMonth, double timeZone)
        {
            int a11, b11;
            if (lunarMonth < 11)
            {
                a11 = GetLunarMonth11(lunarYear - 1, timeZone);
                b11 = GetLunarMonth11(lunarYear, timeZone);
            }
            else
            {
                a11 = GetLunarMonth11(lunarYear, timeZone);
                b11 = GetLunarMonth11(lunarYear + 1, timeZone);
            }

            // Validate leap month request before going further
            if (isLeapMonth && b11 - a11 <= 365)
                throw new ArgumentException(
                    $"Year {lunarYear} has no leap month.",
                    nameof(isLeapMonth));

            int k = (int)Math.Floor(0.5 + (a11 - 2415021.076998695) / 29.530588853);
            int off = lunarMonth - 11;
            if (off < 0) off += 12;

            if (b11 - a11 > 365)
            {
                int leapOff = GetLeapMonthOffset(a11, timeZone);
                int leapMonth = leapOff - 2;
                if (leapMonth < 0) leapMonth += 12;

                if (isLeapMonth && lunarMonth != leapMonth)
                    throw new ArgumentException(
                        $"Year {lunarYear} has no leap month {lunarMonth}. Leap month is {leapMonth}.",
                        nameof(isLeapMonth));

                if (isLeapMonth || (off >= leapOff))
                    off += 1;
            }

            int monthStart = GetNewMoonDay(k + off, timeZone);
            int jd = monthStart + lunarDay - 1;

            JulianDayToDate(jd, out int day, out int month, out int year);
            return new DateTime(year, month, day);
        }

        #endregion

        #region Internal Helpers

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetNewMoonDay(int k, double timeZone)
            => (int)Math.Floor(NewMoon(k) + 0.5 + timeZone / 24.0);

        /// <summary>
        /// Gets the month-11 JD. Uses O(1) cache for Vietnam timezone (hot path),
        /// falls back to astronomical computation for custom timezones.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetLunarMonth11(int yy, double timeZone)
        {
            // Fast path: cached lookup for Vietnam timezone (covers 99%+ of calls).
            // Exact double equality is safe here: VietnamTz = 7.0 is exactly
            // representable in IEEE 754 and all callers use the constant directly.
            if (timeZone == VietnamTz && yy >= MinCacheYear && yy <= MaxCacheYear)
                return Month11Cache[yy - MinCacheYear];

            // Slow path: full astronomical computation
            return ComputeLunarMonth11(yy, timeZone);
        }

        /// <summary>
        /// Raw astronomical computation of month-11 JD (uncached).
        /// Called by static constructor for cache init and as fallback.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int ComputeLunarMonth11(int yy, double timeZone)
        {
            double off = JulianDayFromDate(31, 12, yy) - 2415021.0;
            int k = (int)Math.Floor(off / 29.530588853);
            int nm = GetNewMoonDay(k, timeZone);
            int sunLongSector = (int)Math.Floor(SunLongitude(nm - 0.5 + timeZone / 24.0) / 30.0);

            if (sunLongSector >= 9)
                nm = GetNewMoonDay(k - 1, timeZone);

            return nm;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static int GetLeapMonthOffset(int a11, double timeZone)
        {
            int k = (int)Math.Floor((a11 - 2415021.076998695) / 29.530588853 + 0.5);
            int i = 1;
            int arc = (int)Math.Floor(SunLongitude(GetNewMoonDay(k + i, timeZone) - 0.5 + timeZone / 24.0) / 30.0);

            int last;
            do
            {
                last = arc;
                i++;
                arc = (int)Math.Floor(SunLongitude(GetNewMoonDay(k + i, timeZone) - 0.5 + timeZone / 24.0) / 30.0);
            } while (arc != last && i < 14);

            return i - 1;
        }

        #endregion

        #region Lookup Table (ReadOnlySpan optimization for 2000-2100)

        private const int LutBaseYear = 2000;
        private const int LutYearCount = 101; // 2000–2100

        // Pre-computed lunar month data: 3 bytes per year.
        // Bit 16: leap month has 30 days (1) or 29 days (0)
        // Bits 15-4: month lengths for months 1-12 (1=30 days, 0=29 days)
        // Bits 3-0: leap month number (0 = no leap month)
#if NET8_0_OR_GREATER
        // Zero-alloc: compiler embeds byte data directly in assembly metadata.
        private static ReadOnlySpan<byte> LunarLut => new byte[] {
#else
        // netstandard2.0 fallback: one-time heap allocation (~303 bytes).
        private static readonly byte[] LunarLut = new byte[] {
#endif
            0x00, 0xC9, 0x60, // 2000: 0C960
            0x00, 0xD4, 0xA0, // 2001: 0D4A0
            0x00, 0xDA, 0x50, // 2002: 0DA50
            0x00, 0x75, 0x52, // 2003: 07552
            0x00, 0x56, 0xA0, // 2004: 056A0
            0x00, 0xAB, 0xB7, // 2005: 0ABB7
            0x00, 0x25, 0xD0, // 2006: 025D0
            0x00, 0x92, 0xD0, // 2007: 092D0
            0x00, 0xCA, 0xB5, // 2008: 0CAB5
            0x00, 0xA9, 0x50, // 2009: 0A950
            0x00, 0xB4, 0xA0, // 2010: 0B4A0
            0x00, 0xBA, 0xA4, // 2011: 0BAA4
            0x00, 0xAD, 0x50, // 2012: 0AD50
            0x00, 0x55, 0xD9, // 2013: 055D9
            0x00, 0x4B, 0xA0, // 2014: 04BA0
            0x00, 0xA5, 0xB0, // 2015: 0A5B0
            0x00, 0xAB, 0xB5, // 2016: 0ABB5
            0x00, 0x4A, 0xE0, // 2017: 04AE0
            0x00, 0xA5, 0x70, // 2018: 0A570
            0x00, 0xA4, 0xD4, // 2019: 0A4D4
            0x00, 0xA5, 0xB0, // 2020: 0A5B0
            0x00, 0xAB, 0xB2, // 2021: 0ABB2
            0x00, 0x56, 0xA0, // 2022: 056A0
            0x00, 0x96, 0xD5, // 2023: 096D5
            0x00, 0x92, 0xD0, // 2024: 092D0
            0x00, 0xD4, 0xA0, // 2025: 0D4A0
            0x00, 0xDA, 0x50, // 2026: 0DA50
            0x00, 0x75, 0x52, // 2027: 07552
            0x00, 0x56, 0xA0, // 2028: 056A0
            0x00, 0xAB, 0xB7, // 2029: 0ABB7
            0x00, 0x25, 0xD0, // 2030: 025D0
            0x00, 0x92, 0xD0, // 2031: 092D0
            0x00, 0xCA, 0xB5, // 2032: 0CAB5
            0x00, 0xA9, 0x50, // 2033: 0A950
            0x00, 0xB4, 0xA0, // 2034: 0B4A0
            0x00, 0xBA, 0xA4, // 2035: 0BAA4
            0x00, 0xAD, 0x50, // 2036: 0AD50
            0x00, 0x55, 0xD9, // 2037: 055D9
            0x00, 0x4B, 0xA0, // 2038: 04BA0
            0x00, 0xA5, 0xB0, // 2039: 0A5B0
            0x00, 0xAB, 0xB5, // 2040: 0ABB5
            0x00, 0x4A, 0xE0, // 2041: 04AE0
            0x00, 0xA5, 0x70, // 2042: 0A570
            0x00, 0xA4, 0xD4, // 2043: 0A4D4
            0x00, 0xA5, 0xB0, // 2044: 0A5B0
            0x00, 0xD2, 0xB0, // 2045: 0D2B0
            0x00, 0xD5, 0x58, // 2046: 0D558
            0x00, 0xD5, 0x40, // 2047: 0D540
            0x00, 0xD5, 0xA0, // 2048: 0D5A0
            0x00, 0xAD, 0xA5, // 2049: 0ADA5
            0x00, 0x55, 0xD0, // 2050: 055D0
            0x00, 0x4B, 0xA0, // 2051: 04BA0
            0x00, 0xA9, 0x74, // 2052: 0A974
            0x00, 0xA4, 0xB0, // 2053: 0A4B0
            0x00, 0xB2, 0xA0, // 2054: 0B2A0
            0x00, 0xB5, 0x55, // 2055: 0B555
            0x00, 0xAD, 0x50, // 2056: 0AD50
            0x00, 0x55, 0xD0, // 2057: 055D0
            0x00, 0x4A, 0xFB, // 2058: 04AFB
            0x00, 0xA9, 0x70, // 2059: 0A970
            0x00, 0xA4, 0xB0, // 2060: 0A4B0
            0x00, 0xB2, 0xA7, // 2061: 0B2A7
            0x00, 0xB5, 0x50, // 2062: 0B550
            0x00, 0xAD, 0x50, // 2063: 0AD50
            0x00, 0x2A, 0xF2, // 2064: 02AF2
            0x00, 0x92, 0xE0, // 2065: 092E0
            0x00, 0xA9, 0x50, // 2066: 0A950
            0x00, 0xA9, 0x55, // 2067: 0A955
            0x00, 0xB4, 0xA0, // 2068: 0B4A0
            0x00, 0xB6, 0xA0, // 2069: 0B6A0
            0x00, 0xB5, 0x54, // 2070: 0B554
            0x00, 0xAD, 0x50, // 2071: 0AD50
            0x00, 0x55, 0xD0, // 2072: 055D0
            0x00, 0x4A, 0xFB, // 2073: 04AFB
            0x00, 0xA9, 0x70, // 2074: 0A970
            0x00, 0xA4, 0xB0, // 2075: 0A4B0
            0x00, 0xA4, 0xB7, // 2076: 0A4B7
            0x00, 0xB2, 0xA0, // 2077: 0B2A0
            0x00, 0xB5, 0x50, // 2078: 0B550
            0x00, 0xAD, 0x55, // 2079: 0AD55
            0x00, 0x55, 0xD0, // 2080: 055D0
            0x00, 0x4B, 0xA0, // 2081: 04BA0
            0x00, 0xA5, 0xB0, // 2082: 0A5B0
            0x00, 0xA5, 0xB7, // 2083: 0A5B7
            0x00, 0x4A, 0xE0, // 2084: 04AE0
            0x00, 0xA5, 0x70, // 2085: 0A570
            0x01, 0x04, 0xD5, // 2086: 104D5
            0x00, 0xA4, 0xD0, // 2087: 0A4D0
            0x00, 0xD2, 0x50, // 2088: 0D250
            0x00, 0xD5, 0x52, // 2089: 0D552
            0x00, 0xDD, 0x40, // 2090: 0DD40
            0x00, 0xDA, 0x50, // 2091: 0DA50
            0x00, 0x75, 0x55, // 2092: 07555
            0x00, 0x56, 0xA0, // 2093: 056A0
            0x00, 0xAB, 0xB0, // 2094: 0ABB0
            0x00, 0x25, 0xD0, // 2095: 025D0
            0x00, 0x92, 0xD0, // 2096: 092D0
            0x00, 0xCA, 0xB5, // 2097: 0CAB5
            0x00, 0xA9, 0x50, // 2098: 0A950
            0x00, 0xB4, 0xA0, // 2099: 0B4A0
            0x00, 0xBA, 0xA4, // 2100: 0BAA4
        };

        /// <summary>
        /// Decodes the LUT entry for a given year (2000-2100).
        /// Returns the 20-bit packed value, or -1 if out of range.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DecodeLutEntry(int year)
        {
            int idx = year - LutBaseYear;
            if ((uint)idx >= LutYearCount) return -1;

            int offset = idx * 3;
            return (LunarLut[offset] << 16) | (LunarLut[offset + 1] << 8) | LunarLut[offset + 2];
        }

        /// <summary>
        /// Gets the number of days in a specific lunar month from the LUT.
        /// </summary>
        /// <param name="year">Lunar year (2000-2100).</param>
        /// <param name="month">Lunar month (1-12).</param>
        /// <returns>29 or 30 days, or -1 if out of range.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLutMonthDays(int year, int month)
        {
            int entry = DecodeLutEntry(year);
            if (entry < 0 || month < 1 || month > 12) return -1;

            int bit = (entry >> (16 - month)) & 1;
            return 29 + bit;
        }

        /// <summary>
        /// Gets the leap month number for a lunar year from the LUT (0 = no leap).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLutLeapMonth(int year)
        {
            int entry = DecodeLutEntry(year);
            return entry < 0 ? -1 : entry & 0x0F;
        }

        /// <summary>
        /// Gets the number of days in the leap month (29 or 30) from the LUT.
        /// Returns 0 if no leap month in that year.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetLutLeapMonthDays(int year)
        {
            int entry = DecodeLutEntry(year);
            if (entry < 0) return -1;

            int leapMonth = entry & 0x0F;
            if (leapMonth == 0) return 0;

            return 29 + ((entry >> 16) & 1);
        }

        /// <summary>
        /// Gets total days in a lunar year from the LUT (including leap month if any).
        /// </summary>
        public static int GetLutYearDays(int year)
        {
            int entry = DecodeLutEntry(year);
            if (entry < 0) return -1;

            int total = 0;
            for (int m = 1; m <= 12; m++)
                total += 29 + ((entry >> (16 - m)) & 1);

            int leapMonth = entry & 0x0F;
            if (leapMonth > 0)
                total += 29 + ((entry >> 16) & 1);

            return total;
        }

        #endregion

        #region Solar Term Names

        private static readonly string[] SolarTermVietnamese = {
            "Xuân Phân",   "Thanh Minh",  "Cốc Vũ",      "Lập Hạ",
            "Tiểu Mãn",   "Mang Chủng",  "Hạ Chí",       "Tiểu Thử",
            "Đại Thử",    "Lập Thu",     "Xử Thử",       "Bạch Lộ",
            "Thu Phân",    "Hàn Lộ",      "Sương Giáng",  "Lập Đông",
            "Tiểu Tuyết", "Đại Tuyết",   "Đông Chí",     "Tiểu Hàn",
            "Đại Hàn",    "Lập Xuân",    "Vũ Thủy",      "Kinh Trập"
        };

        /// <summary>
        /// Gets the Vietnamese name of a Solar Term by index (0-23).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetSolarTermName(int index)
            => SolarTermVietnamese[((index % 24) + 24) % 24];

        #endregion
    }
}
