using LongPd.Dates.Julian;

Console.WriteLine("=== Astronomical Julian Date (JD) ===");

var j2000 = new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc);
Console.WriteLine($"J2000 epoch   : {j2000:yyyy-MM-dd HH:mm:ss} UTC  =>  JD {j2000.ToAstronomicalJD()}");

var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
Console.WriteLine($"Unix epoch    : {unixEpoch:yyyy-MM-dd HH:mm:ss} UTC  =>  JD {unixEpoch.ToAstronomicalJD()}");

var now = DateTime.UtcNow;
Console.WriteLine($"Now (UTC)     : {now:yyyy-MM-dd HH:mm:ss} UTC  => JD {now.ToAstronomicalJD()}");

Console.WriteLine("\n=== Modified Julian Date (MJD) ===");
Console.WriteLine($"J2000 epoch   =>  MJD {j2000.ToModifiedJulianDate()}");
Console.WriteLine($"Now (UTC)     =>  MJD {now.ToModifiedJulianDate()}");

Console.WriteLine("\n=== JD => DateTime (round-trip) ===");
double jd = j2000.ToAstronomicalJD();
Console.WriteLine($"JD {jd}  =>  {jd.FromAstronomicalJD():yyyy-MM-dd HH:mm:ss} UTC");

double mjd = j2000.ToModifiedJulianDate();
Console.WriteLine($"MJD {mjd}  =>  {mjd.FromModifiedJulianDate():yyyy-MM-dd HH:mm:ss} UTC");

Console.WriteLine("\n=== Ordinal Date (YYYYDDD) ===");
var sampleDates = new[]
{
    new DateTime(2025, 1, 1),
    new DateTime(2025, 4, 29),
    new DateTime(2026, 12, 31),
};

foreach (var d in sampleDates)
{
    int ordinal = d.ToOrdinalDate();
    DateTime back = ordinal.FromOrdinalDate();
    Console.WriteLine($"{d:yyyy-MM-dd}  =>  ordinal {ordinal}  =>  {back:yyyy-MM-dd}");
}
