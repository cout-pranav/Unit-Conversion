namespace UnitConversion.Api.Conversion;

/// <summary>
/// Converts values between units by going from unit → base unit → target unit.
/// Adding a unit only requires a new entry in <see cref="Units"/>.
/// </summary>
public static class UnitConverter
{
    private const string Temperature = "temperature";
    private const string Length = "length";

    // Base units: kelvin for temperature, meter for length.
    public static IReadOnlyList<Unit> Units { get; } =
    [
        new("C", "Celsius", Temperature, Factor: 1, Offset: 273.15),
        new("F", "Fahrenheit", Temperature, Factor: 5.0 / 9.0, Offset: 459.67),
        new("K", "Kelvin", Temperature, Factor: 1),

        new("mm", "Millimeter", Length, Factor: 0.001),
        new("cm", "Centimeter", Length, Factor: 0.01),
        new("m", "Meter", Length, Factor: 1),
        new("km", "Kilometer", Length, Factor: 1000),
        new("in", "Inch", Length, Factor: 0.0254),
        new("ft", "Foot", Length, Factor: 0.3048),
        new("yd", "Yard", Length, Factor: 0.9144),
        new("mi", "Mile", Length, Factor: 1609.344),
    ];

    private static readonly Dictionary<string, Unit> UnitsBySymbol =
        Units.ToDictionary(u => u.Symbol, StringComparer.OrdinalIgnoreCase);

    public static double Convert(double value, string from, string to)
    {
        var fromUnit = Find(from);
        var toUnit = Find(to);

        if (fromUnit.Category != toUnit.Category)
        {
            throw new ArgumentException(
                $"Cannot convert from '{fromUnit.Symbol}' ({fromUnit.Category}) to '{toUnit.Symbol}' ({toUnit.Category}).");
        }

        // Rounding removes floating-point noise, e.g. 212.00000000000006 → 212.
        return Math.Round(toUnit.FromBase(fromUnit.ToBase(value)), 5);
    }

    private static Unit Find(string symbol) =>
        UnitsBySymbol.TryGetValue(symbol, out var unit)
            ? unit
            : throw new ArgumentException($"Unknown unit '{symbol}'.");
}
