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

Console.WriteLine(lunar); // "01/01/2025"
```

### 5. Solar Terms (Tiết Khí) ☀️

```csharp
// Get Solar Term index (0–23)
int term = new DateTime(2025, 3, 21).GetSolarTerm();        // 0 = Xuân Phân

// Get Vietnamese name
string name = new DateTime(2025, 6, 22).GetSolarTermName(); // "Hạ Chí"
```

---

## 📊 Performance Benchmarks

> Measured with **BenchmarkDotNet v0.14.0** — `.NET 8.0.24`, X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
> `ShortRunJob` (3 iterations). Run on Windows 11.

| Method                   | Mean        | StdDev     | Allocated |
|------------------------- |------------:|-----------:|----------:|
| `LutDecode`              |   ~0.000 ns |      —     |      0 B  |
| `NewMoon`                |   ~0.000 ns |      —     |      0 B  |
| `ToOrdinalDate`          |    2.801 ns |  0.155 ns  |      0 B  |
| `FromOrdinalDate`        |    3.289 ns |  0.105 ns  |      0 B  |
| `ToModifiedJulianDate`   |   19.285 ns |  0.215 ns  |      0 B  |
| `ToAstronomicalJD`       |   19.947 ns |  1.159 ns  |      0 B  |
| `FromModifiedJulianDate` |   21.222 ns |  1.063 ns  |      0 B  |
| `FromAstronomicalJD`     |   21.368 ns |  1.298 ns  |      0 B  |
| `SunLongitude`           |   34.763 ns |  0.436 ns  |      0 B  |
| `GetSolarTermName`       |   38.407 ns |  0.053 ns  |      0 B  |
| `GetSolarTerm`           |   53.821 ns | 13.512 ns  |      0 B  |
| `ToVietnameseLunar`      | 2,272 ns    | 43.029 ns  |      0 B  |
| `FromVietnameseLunar`    | 2,397 ns    | 87.677 ns  |      0 B  |

**✅ Zero allocations across all methods** — safe for hot paths, tight loops, and real-time systems.

> **Note:** `LutDecode` and `NewMoon` show ~0 ns because the JIT eliminates them as compile-time constants — ideal behavior.
> `ToVietnameseLunar` at ~2.3 µs reflects full astronomical computation (multiple `NewMoon` + `SunLongitude` calls) with **zero heap allocations**.

---

## 📐 Architecture

```
JulianExtensions (Extension Methods)
├── ToAstronomicalJD / FromAstronomicalJD
├── ToModifiedJulianDate / FromModifiedJulianDate
├── ToOrdinalDate / FromOrdinalDate
├── ToVietnameseLunar / FromVietnameseLunar
└── GetSolarTerm / GetSolarTermName

VietnameseLunarCalendar (Pure Static)
├── SunLongitude(jd)      ← pure function
├── NewMoon(k)            ← pure function
├── ToLunar / FromLunar   ← astronomical algorithm
├── GetSolarTermIndex     ← based on Sun longitude
└── ReadOnlySpan LUT      ← zero-alloc month data 2000–2100

LunarDate (readonly struct)
├── Day, Month, Year, IsLeapMonth
└── IEquatable, ToString()
```

---

## 🗺 Roadmap

- [x] Full JD / MJD Support
- [x] Inverse Conversion (JD → DateTime)
- [x] ISO 8601 Ordinal Date (YYYYDDD)
- [x] Vietnamese Lunar Calendar (Âm Lịch)
- [x] 24 Solar Terms (Tiết Khí)
- [x] ReadOnlySpan LUT (2000–2100)
- [ ] Can Chi / Heavenly Stems & Earthly Branches (v1.2.0)
- [ ] Holiday Detection — Tết, Trung Thu (v1.2.0)

---

## 📄 License

Licensed under the [MIT License](https://opensource.org/licenses/MIT).  
Maintained by **Phạm Đình Long**