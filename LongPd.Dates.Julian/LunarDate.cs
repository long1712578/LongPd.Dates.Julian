using System;

namespace LongPd.Dates.Julian
{
    /// <summary>
    /// Represents a Vietnamese Lunar Calendar date (immutable value type).
    /// </summary>
    public readonly struct LunarDate : IEquatable<LunarDate>
    {
        /// <summary>Day of the lunar month (1-30).</summary>
        public int Day { get; }

        /// <summary>Lunar month (1-12).</summary>
        public int Month { get; }

        /// <summary>Lunar year.</summary>
        public int Year { get; }

        /// <summary>True if this month is a leap (intercalary) month.</summary>
        public bool IsLeapMonth { get; }

        /// <summary>
        /// Initializes a new <see cref="LunarDate"/>.
        /// </summary>
        public LunarDate(int day, int month, int year, bool isLeapMonth)
        {
            Day = day;
            Month = month;
            Year = year;
            IsLeapMonth = isLeapMonth;
        }

        /// <inheritdoc/>
        public bool Equals(LunarDate other)
            => Day == other.Day && Month == other.Month
            && Year == other.Year && IsLeapMonth == other.IsLeapMonth;

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is LunarDate ld && Equals(ld);

        /// <inheritdoc/>
        public override int GetHashCode()
            => Day ^ (Month << 5) ^ (Year << 9) ^ (IsLeapMonth ? 1 << 20 : 0);

        /// <summary>Returns e.g. "15/01/2025" or "10/04*/2025" for leap months.</summary>
        public override string ToString()
            => IsLeapMonth
                ? $"{Day:D2}/{Month:D2}*/{Year}"
                : $"{Day:D2}/{Month:D2}/{Year}";

        /// <summary>Equality operator.</summary>
        public static bool operator ==(LunarDate left, LunarDate right) => left.Equals(right);

        /// <summary>Inequality operator.</summary>
        public static bool operator !=(LunarDate left, LunarDate right) => !left.Equals(right);
    }
}
