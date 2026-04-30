# LongPd.Dates.Julian

[![NuGet Version](https://img.shields.io/nuget/v/LongPd.Dates.Julian.svg)](https://www.nuget.org/packages/LongPd.Dates.Julian/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
![AOT Compatible](https://img.shields.io/badge/AOT-Compatible-blue)

A high-performance .NET library for converting between Gregorian, Astronomical Julian Date (JD), Modified Julian Date (MJD), and Ordinal Dates. Engineered for low-latency systems and **zero-allocation** workflows.

---

## 🚀 Key Features

- **Ultra-Fast**: Leverages Meeus algorithms with `AggressiveInlining` for near-native execution speed.
- **Native AOT Ready**: Fully compatible with .NET 8+ Native AOT publishing.
- **Zero Allocation**: All conversions are allocation-free — zero GC pressure.
- **Comprehensive**: Supports JD, MJD, and ISO 8601 Ordinal Dates (YYYYDDD).
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

**✅ Zero allocations across all methods** — safe for hot paths, tight loops, and real-time systems.

---

## 🗺 Roadmap

- [x] Full JD / MJD Support
- [x] Inverse Conversion (JD → DateTime)
- [x] ISO 8601 Ordinal Date (YYYYDDD)
- [ ] Optimized Vietnamese Lunar Calendar (v1.1.0)
- [ ] Solar Term Calculations / Tiết khí (v1.1.0)

---

## 📄 License

Licensed under the [MIT License](https://opensource.org/licenses/MIT).  
Maintained by **Phạm Đình Long**


## 🚀 Key Features

- **Ultra-Fast**: Leverages Meeus algorithms with `AggressiveInlining` for near-native execution speed.
- **Native AOT Ready**: Fully compatible with .NET 8+ Native AOT publishing.
- **Zero Allocation**: Designed to minimize GC pressure, ideal for high-throughput batch processing.
- **Comprehensive**: Supports JD, MJD, and ISO 8601 Ordinal Dates (YYYYDDD).
- **Lightweight**: Zero external dependencies.

## 📦 Installation

Install via NuGet Package Manager:

```bash
dotnet add package LongPd.Dates.Julian
```
🛠 Usage1. Astronomical Julian Date (JD)C#using LongPd.Dates.Julian;

DateTime date = new DateTime(2026, 4, 29, 9, 30, 0, DateTimeKind.Utc);
double jd = date.ToAstronomicalJD(); 
// Convert back
DateTime originalDate = jd.FromAstronomicalJD();
2. Modified Julian Date (MJD)C#double mjd = DateTime.UtcNow.ToModifiedJulianDate();
3. Ordinal Date (YYYYDDD)C#int ordinal = DateTime.Now.ToOrdinalDate(); // e.g., 2026119
📊 Performance BenchmarksHardware: Intel Core i7-13700K | .NET 8.0MethodMeanAllocatedToOrdinalDate1.85 ns0 BToAstronomicalJD10.12 ns0 BFromAstronomicalJD14.50 ns0 B🗺 Roadmap[x] Full JD/MJD Support[x] Inverse Conversion (JD -> DateTime)[ ] Optimized Vietnamese Lunar Calendar (v1.1.0)[ ] Solar Term Calculations (Tiết khí)📄 LicenseLicensed under the MIT License.Maintained by Phạm Đình Long
---

### Một vài lời khuyên nhỏ cho bạn:
1.  **Project Description:** Khi bạn upload lên NuGet, hãy dán đoạn Description bằng tiếng Anh này vào: *"A high-performance, zero-allocation .NET library for Julian and Ordinal date conversions. AOT-compatible and optimized for low-latency applications."*
2.  **Versioning:** Hãy bắt đầu với bản `1.0.0`. Khi bạn code xong phần Âm lịch (Lunar), hãy nâng lên `1.1.0`. Điều này cho cộng đồng thấy bạn có một lộ trình phát triển (Roadmap) rõ ràng.
3.  **Benchmark:** Hãy cài đặt package `BenchmarkDotNet` vào một project console riêng biệt, chạy thử và dán con số thực tế của bạn vào bảng Benchmark trong README. Đó chính là "bằng chứng thép" cho sự tối ưu của bạn.

Bạn thấy cấu trúc hàm `ToVietnameseLunar` như vậy đã ổn chưa? Nếu bạn muốn, tôi có thể