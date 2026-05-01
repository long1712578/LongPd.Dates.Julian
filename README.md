# LongPd.Dates.Julian

[![NuGet Version](https://img.shields.io/nuget/v/LongPd.Dates.Julian.svg)](https://www.nuget.org/packages/LongPd.Dates.Julian/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
![AOT Compatible](https://img.shields.io/badge/AOT-Compatible-blue)

A high-performance .NET library for converting between Gregorian, Astronomical Julian Date (JD), Modified Julian Date (MJD), Ordinal Dates, **Vietnamese Lunar Calendar (Âm Lịch)**, and **24 Solar Terms (Tiết Khí)**. Engineered for low-latency systems and **zero-allocation** workflows.

---

## 🚀 Key Features

- **Ultra-Fast**: Leverages Meeus algorithms with `AggressiveInlining` for near-native execution speed.
- **Native AOT Ready**: Fully compatible with .NET 8+ Native AOT publishing.
- **Zero Allocation**: All conversions are allocation-free — zero GC pressure.
- **Vietnamese Lunar Calendar**: Full Solar → Lunar → Solar conversion using astronomical algorithms.
- **24 Solar Terms (Tiết Khí)**: Pure-function calculation based on Sun's ecliptic longitude.
- **ReadOnlySpan LUT**: Pre-computed lunar month data (2000–2100) for O(1) lookups.
- **Comprehensive**: Supports JD, MJD, ISO 8601 Ordinal Dates, Lunar Dates.
- **Lightweight**: Zero external dependencies.

---

## 📦 Installation

```bash
dotnet add package LongPd.Dates.Julian
```

---

## 🛠 Usage

### 1. Astronomical Julian Date (JD)

```csharp
using LongPd.Dates.Julian;

DateTime date = new DateTime(2026, 4, 29, 9, 30, 0, DateTimeKind.Utc);

// DateTime → JD
double jd = date.ToAstronomicalJD();        // e.g. 2461161.8958333

// JD → DateTime
DateTime back = jd.FromAstronomicalJD();    // 2026-04-29 09:30:00 UTC
```

### 2. Modified Julian Date (MJD)

```csharp
// DateTime → MJD
double mjd = DateTime.UtcNow.ToModifiedJulianDate();

// MJD → DateTime
DateTime back = mjd.FromModifiedJulianDate();
```

### 3. Ordinal Date (YYYYDDD)

```csharp
// DateTime → Ordinal
int ordinal = new DateTime(2026, 4, 29).ToOrdinalDate(); // 2026119

// Ordinal → DateTime
DateTime back = 2026119.FromOrdinalDate();               // 2026-04-29
```

### 4. Vietnamese Lunar Calendar (Âm Lịch) 🌙

```csharp
// Gregorian → Vietnamese Lunar
var tet2025 = new DateTime(2025, 1, 29);
LunarDate lunar = tet2025.ToVietnameseLunar();
// lunar.Day=1, lunar.Month=1, lunar.Year=2025, lunar.IsLeapMonth=false

// Vietnamese Lunar → Gregorian
DateTime solar = JulianExtensions.FromVietnameseLunar(15, 8, 2024);
// Returns: 2024-09-17 (Tết Trung Thu)

// Display
Console.WriteLine(lunar); // "01/01/2025"
```

### 5. Solar Terms (Tiết Khí) ☀️

```csharp
// Get Solar Term index (0-23)
int term = new DateTime(2025, 3, 20).GetSolarTerm(); // 0 = Xuân Phân

// Get Vietnamese name
string name = new DateTime(2025, 6, 21).GetSolarTermName(); // "Hạ Chí"
```

### 6. Astronomical Functions (Pure)

```csharp
// Sun's ecliptic longitude at a given Julian Day
double sunLong = VietnameseLunarCalendar.SunLongitude(2451545.0); // ~280°

// Julian Day of k-th new moon since J1900
double nm = VietnameseLunarCalendar.NewMoon(1533);

// LUT: month days for lunar year 2025
int days = VietnameseLunarCalendar.GetLutMonthDays(2025, 1); // 29 or 30
int leapMonth = VietnameseLunarCalendar.GetLutLeapMonth(2025);
```

---

## 📊 Performance Benchmarks

> Measured with **BenchmarkDotNet v0.14.0** — `.NET 8.0.24`, X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

| Method                   | Mean      | Error     | StdDev    | Allocated |
|------------------------- |----------:|----------:|----------:|----------:|
| `ToOrdinalDate`          |  1.775 ns | 0.290 ns  | 0.016 ns  |      0 B  |
| `FromOrdinalDate`        |  2.129 ns | 0.029 ns  | 0.002 ns  |      0 B  |
| `ToAstronomicalJD`       | 11.613 ns | 0.132 ns  | 0.007 ns  |      0 B  |
| `FromAstronomicalJD`     | 11.804 ns | 0.159 ns  | 0.009 ns  |      0 B  |
| `ToModifiedJulianDate`   | 11.955 ns | 2.060 ns  | 0.113 ns  |      0 B  |
| `FromModifiedJulianDate` | 12.247 ns | 0.173 ns  | 0.010 ns  |      0 B  |
| `LutDecode`              |   ~2 ns   |     —     |     —     |      0 B  |
| `SunLongitude`           |  ~15 ns   |     —     |     —     |      0 B  |
| `GetSolarTerm`           |  ~20 ns   |     —     |     —     |      0 B  |
| `ToVietnameseLunar`      | ~500 ns   |     —     |     —     |      0 B  |

**✅ Zero allocations across all methods** — safe for hot paths, tight loops, and real-time systems.

---

## 📐 Architecture

```
JulianExtensions (Extension Methods)
├── ToAstronomicalJD / FromAstronomicalJD    ← Meeus algorithm
├── ToModifiedJulianDate / FromModifiedJulianDate
├── ToOrdinalDate / FromOrdinalDate
├── ToVietnameseLunar / FromVietnameseLunar  ← delegates to ↓
├── GetSolarTerm / GetSolarTermName          ← delegates to ↓
│
VietnameseLunarCalendar (Pure Static Class)
├── SunLongitude(jd)         ← pure function
├── NewMoon(k)               ← pure function
├── ToLunar / FromLunar      ← astronomical algorithm
├── GetSolarTermIndex        ← based on Sun longitude
├── ReadOnlySpan<byte> LUT   ← zero-alloc month data 2000-2100
│
LunarDate (readonly struct)
├── Day, Month, Year, IsLeapMonth
├── IEquatable<LunarDate>
└── ToString() formatting
```

---

## 🗺 Roadmap

- [x] Full JD / MJD Support
- [x] Inverse Conversion (JD → DateTime)
- [x] ISO 8601 Ordinal Date (YYYYDDD)
- [x] Vietnamese Lunar Calendar (Âm Lịch)
- [x] Solar Term Calculations (Tiết Khí)
- [x] ReadOnlySpan<byte> LUT (2000–2100)
- [ ] Can Chi (Heavenly Stems & Earthly Branches) (v1.2.0)
- [ ] Holiday Detection (Tết, Trung Thu, etc.) (v1.2.0)

---

## 📄 License

Licensed under the [MIT License](https://opensource.org/licenses/MIT).  
Maintained by **Phạm Đình Long**