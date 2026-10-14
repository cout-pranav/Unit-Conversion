using UnitConversion.Api.Conversion;

namespace UnitConversion.Tests;

public class UnitConverterTests
{
    [Theory]
    [InlineData(100, "C", "F", 212)]
    [InlineData(32, "F", "C", 0)]
    [InlineData(0, "C", "K", 273.15)]
    [InlineData(0, "K", "C", -273.15)]
    [InlineData(-40, "C", "F", -40)]
    [InlineData(25, "c", "c", 25)]
    public void Convert_Temperature_ReturnsExpected(double value, string from, string to, double expected)
    {
        Assert.Equal(expected, UnitConverter.Convert(value, from, to));
    }

    [Fact]
    public void Convert_UnknownUnit_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() => UnitConverter.Convert(1, "X", "C"));
        Assert.Contains("Unknown unit 'X'", ex.Message);
    }
}
