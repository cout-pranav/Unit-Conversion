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
    [InlineData("/api/conversions?value=0&from=K&to=F", -459.67)]
    [InlineData("/api/conversions?value=-40&from=f&to=c", -40)]
    [InlineData("/api/conversions?value=36.6&from=C&to=K", 309.75)]
    public async Task GetConversion_ValidTemperature_ReturnsResult(string url, double expected)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ConversionResponse>();
        Assert.Equal(expected, body!.Result);
    }

    [Fact]
    public async Task GetConversion_MetersToFeet_ReturnsResult()
    {
        var response = await _client.GetAsync("/api/conversions?value=10&from=m&to=ft");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ConversionResponse>();
        Assert.Equal(32.8084, body!.Result);
    }

    [Theory]
    [InlineData("/api/conversions?value=1&from=X&to=C", "Unknown unit 'X'")]
    [InlineData("/api/conversions?value=1&from=C&to=X", "Unknown unit 'X'")]
    [InlineData("/api/conversions?value=1&from=m&to=C", "Cannot convert")]
    [InlineData("/api/conversions?from=C&to=F", "required")]
    [InlineData("/api/conversions?value=1&to=F", "required")]
    [InlineData("/api/conversions?value=1&from=C", "required")]
    public async Task GetConversion_InvalidInput_ReturnsBadRequest(string url, string expectedDetail)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemResponse>();
        Assert.Equal(400, problem!.Status);
        Assert.Contains(expectedDetail, problem.Detail);
    }

    private record ConversionResponse(double Value, string From, string To, double Result);

    private record ProblemResponse(int Status, string Detail);
}
