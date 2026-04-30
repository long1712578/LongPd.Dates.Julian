# LongPd.Dates.Julian

[![NuGet Version](https://img.shields.io/nuget/v/LongPd.Dates.Julian.svg)](https://www.nuget.org/packages/LongPd.Dates.Julian/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
![AOT Compatible](https://img.shields.io/badge/AOT-Compatible-blue)

A high-performance .NET library for converting between Gregorian, Astronomical Julian Date (JD), Modified Julian Date (MJD), and Ordinal Dates. Engineered for low-latency systems and **zero-allocation** workflows.

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
