namespace UnitConversion.Api.Conversion;

/// <summary>
/// A unit of measurement, defined relative to its category's base unit:
/// <c>base = (value + Offset) * Factor</c>.
/// </summary>
public record Unit(string Symbol, string Name, string Category, double Factor, double Offset = 0)
{
    public double ToBase(double value) => (value + Offset) * Factor;

    public double FromBase(double baseValue) => baseValue / Factor - Offset;
}
