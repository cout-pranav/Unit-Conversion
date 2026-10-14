using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace UnitConversion.Tests;

public class ConversionEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetConversion_CelsiusToFahrenheit_ReturnsResult()
    {
        var response = await _client.GetAsync("/api/conversions?value=100&from=C&to=F");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ConversionResponse>();
        Assert.Equal(212, body!.Result);
    }

    [Theory]
    [InlineData("/api/conversions?value=1&from=X&to=C")]
    [InlineData("/api/conversions?from=C&to=F")]
    public async Task GetConversion_InvalidInput_ReturnsBadRequest(string url)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private record ConversionResponse(double Value, string From, string To, double Result);
}
