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
    // Every unit -> m
    [InlineData(1000, "mm", "m", 1)]
    [InlineData(100, "cm", "m", 1)]
    [InlineData(1, "m", "m", 1)]
    [InlineData(1, "km", "m", 1000)]
    [InlineData(1, "in", "m", 0.0254)]
    [InlineData(1, "ft", "m", 0.3048)]
    [InlineData(1, "yd", "m", 0.9144)]
    [InlineData(1, "mi", "m", 1609.344)]
    // Metric <-> metric
    [InlineData(2.5, "km", "m", 2500)]
    [InlineData(1.5, "m", "cm", 150)]
    [InlineData(1, "cm", "mm", 10)]
    [InlineData(1500000, "mm", "km", 1.5)]
    // Imperial <-> imperial
    [InlineData(1, "ft", "in", 12)]
    [InlineData(1, "yd", "ft", 3)]
    [InlineData(1, "mi", "yd", 1760)]
    [InlineData(1, "mi", "ft", 5280)]
    // Metric <-> imperial
    [InlineData(1, "in", "cm", 2.54)]
    [InlineData(2, "ft", "m", 0.6096)]
    [InlineData(10, "m", "ft", 32.8084)]
    [InlineData(1, "mi", "km", 1.60934)]
    [InlineData(1, "m", "in", 39.37008)]
    // Same unit (identity)
    [InlineData(7.5, "km", "km", 7.5)]
    [InlineData(-3.25, "in", "in", -3.25)]
    // Zero
    [InlineData(0, "mi", "mm", 0)]
    // Negative values
    [InlineData(-5, "ft", "in", -60)]
    [InlineData(-1, "km", "m", -1000)]
    // Fractional values
    [InlineData(0.5, "mi", "ft", 2640)]
    [InlineData(2.5, "in", "cm", 6.35)]
    // Rounding to 5 decimals
    [InlineData(1, "mm", "in", 0.03937)]
    [InlineData(1, "ft", "mi", 0.00019)]
    [InlineData(1, "in", "mi", 0.00002)]
    // Case-insensitive symbols
    [InlineData(1, "KM", "M", 1000)]
    [InlineData(3, "Ft", "IN", 36)]
    public void Convert_Length_ReturnsExpected(double value, string from, string to, double expected)
    {
        Assert.Equal(expected, UnitConverter.Convert(value, from, to));
    }

    [Theory]
    [InlineData(12.345, "m", "ft")]
    [InlineData(2.5, "in", "cm")]
    [InlineData(-7.25, "yd", "m")]
    [InlineData(100, "mm", "cm")]
    [InlineData(3.5, "km", "mi")]
    public void Convert_LengthRoundTrip_ReturnsOriginalValue(double value, string from, string to)
    {
        var converted = UnitConverter.Convert(value, from, to);

        var roundTripped = UnitConverter.Convert(converted, to, from);

        Assert.Equal(value, roundTripped);
    }

    [Theory]
    // Every unit -> kg
    [InlineData(1000000, "mg", "kg", 1)]
    [InlineData(1000, "g", "kg", 1)]
    [InlineData(1, "kg", "kg", 1)]
    [InlineData(1, "t", "kg", 1000)]
    [InlineData(16, "oz", "kg", 0.45359)]
    [InlineData(1, "lb", "kg", 0.45359)]
    [InlineData(1, "st", "kg", 6.35029)]
    // Metric <-> metric
    [InlineData(2.5, "t", "kg", 2500)]
    [InlineData(1.5, "kg", "g", 1500)]
    [InlineData(1, "g", "mg", 1000)]
    [InlineData(2500, "mg", "kg", 0.0025)]
    // Imperial <-> imperial
    [InlineData(1, "lb", "oz", 16)]
    [InlineData(1, "st", "lb", 14)]
    [InlineData(1, "st", "oz", 224)]
    // Metric <-> imperial
    [InlineData(1, "kg", "lb", 2.20462)]
    [InlineData(1, "oz", "g", 28.34952)]
    [InlineData(1, "kg", "oz", 35.27396)]
    // Same unit (identity)
    [InlineData(7.5, "kg", "kg", 7.5)]
    [InlineData(-3.25, "oz", "oz", -3.25)]
    // Zero
    [InlineData(0, "st", "mg", 0)]
    // Negative values
    [InlineData(-2, "lb", "oz", -32)]
    [InlineData(-1, "t", "kg", -1000)]
    // Fractional values
    [InlineData(0.5, "st", "lb", 7)]
    [InlineData(2.5, "lb", "oz", 40)]
    // Rounding to 5 decimals
    [InlineData(1, "g", "oz", 0.03527)]
    [InlineData(1, "g", "st", 0.00016)]
    [InlineData(1, "mg", "oz", 0.00004)]
    // Case-insensitive symbols
    [InlineData(2, "KG", "Lb", 4.40925)]
    [InlineData(3, "G", "MG", 3000)]
    public void Convert_Mass_ReturnsExpected(double value, string from, string to, double expected)
    {
        Assert.Equal(expected, UnitConverter.Convert(value, from, to));
    }

    [Theory]
    [InlineData(12.345, "kg", "lb")]
    [InlineData(2.5, "lb", "oz")]
    [InlineData(-7.25, "st", "kg")]
    [InlineData(100, "g", "mg")]
    [InlineData(3.5, "t", "kg")]
    [InlineData(1.5, "oz", "g")]
    public void Convert_MassRoundTrip_ReturnsOriginalValue(double value, string from, string to)
    {
        var converted = UnitConverter.Convert(value, from, to);

        var roundTripped = UnitConverter.Convert(converted, to, from);

        Assert.Equal(value, roundTripped);
    }

    [Theory]
    [InlineData("m", "C", "length", "temperature")]
    [InlineData("F", "ft", "temperature", "length")]
    [InlineData("km", "K", "length", "temperature")]
    [InlineData("kg", "m", "mass", "length")]
    [InlineData("lb", "C", "mass", "temperature")]
    [InlineData("ft", "g", "length", "mass")]
    public void Convert_MixedCategories_Throws(string from, string to, string fromCategory, string toCategory)
    {
        var ex = Assert.Throws<ArgumentException>(() => UnitConverter.Convert(1, from, to));
        Assert.Contains("Cannot convert", ex.Message);
        Assert.Contains($"({fromCategory})", ex.Message);
        Assert.Contains($"({toCategory})", ex.Message);
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
