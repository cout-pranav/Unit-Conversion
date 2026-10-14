using UnitConversion.Api.Conversion;

namespace UnitConversion.Tests;

public class UnitConverterTests
{
    [Theory]
    // C -> F
    [InlineData(100, "C", "F", 212)]
    [InlineData(0, "C", "F", 32)]
    [InlineData(37, "C", "F", 98.6)]
    [InlineData(-273.15, "C", "F", -459.67)]
    [InlineData(-40, "C", "F", -40)]
    // F -> C
    [InlineData(32, "F", "C", 0)]
    [InlineData(212, "F", "C", 100)]
    [InlineData(98.6, "F", "C", 37)]
    [InlineData(-459.67, "F", "C", -273.15)]
    [InlineData(-40, "F", "C", -40)]
    // C -> K
    [InlineData(0, "C", "K", 273.15)]
    [InlineData(100, "C", "K", 373.15)]
    [InlineData(37, "C", "K", 310.15)]
    [InlineData(-273.15, "C", "K", 0)]
    // K -> C
    [InlineData(0, "K", "C", -273.15)]
    [InlineData(273.15, "K", "C", 0)]
    [InlineData(373.15, "K", "C", 100)]
    // F -> K
    [InlineData(32, "F", "K", 273.15)]
    [InlineData(212, "F", "K", 373.15)]
    [InlineData(-459.67, "F", "K", 0)]
    [InlineData(-40, "F", "K", 233.15)]
    // K -> F
    [InlineData(0, "K", "F", -459.67)]
    [InlineData(273.15, "K", "F", 32)]
    [InlineData(373.15, "K", "F", 212)]
    [InlineData(310.15, "K", "F", 98.6)]
    // Same unit (identity)
    [InlineData(25, "c", "c", 25)]
    [InlineData(-12.5, "F", "F", -12.5)]
    [InlineData(0, "K", "K", 0)]
    // Case-insensitive symbols
    [InlineData(100, "c", "f", 212)]
    [InlineData(0, "k", "C", -273.15)]
    // Fractional values and rounding to 5 decimals
    [InlineData(1, "C", "F", 33.8)]
    [InlineData(-17.5, "C", "F", 0.5)]
    [InlineData(0, "F", "C", -17.77778)]
    [InlineData(1, "F", "C", -17.22222)]
    [InlineData(100, "F", "K", 310.92778)]
    [InlineData(1, "K", "F", -457.87)]
    public void Convert_Temperature_ReturnsExpected(double value, string from, string to, double expected)
    {
        Assert.Equal(expected, UnitConverter.Convert(value, from, to));
    }

    [Theory]
    [InlineData(12.345, "C", "F")]
    [InlineData(-40, "C", "K")]
    [InlineData(98.6, "F", "K")]
    [InlineData(-3.5, "F", "C")]
    [InlineData(500.25, "K", "C")]
    [InlineData(0, "K", "F")]
    public void Convert_TemperatureRoundTrip_ReturnsOriginalValue(double value, string from, string to)
    {
        var converted = UnitConverter.Convert(value, from, to);

        var roundTripped = UnitConverter.Convert(converted, to, from);

        Assert.Equal(value, roundTripped);
    }

    [Theory]
    [InlineData("X", "C", "X")]
    [InlineData("C", "X", "X")]
    [InlineData("", "K", "")]
    [InlineData("F", "Celsius", "Celsius")]
    public void Convert_UnknownUnit_Throws(string from, string to, string unknown)
    {
        var ex = Assert.Throws<ArgumentException>(() => UnitConverter.Convert(1, from, to));
        Assert.Contains($"Unknown unit '{unknown}'", ex.Message);
    }
}
