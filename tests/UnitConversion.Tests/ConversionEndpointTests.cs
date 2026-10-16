using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;

namespace UnitConversion.Tests;

public class ConversionEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private const string Url = "/api/conversions";

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task PostConversion_CelsiusToFahrenheit_ReturnsResult()
    {
        var response = await _client.PostAsJsonAsync(Url, new { value = 100, from = "C", to = "F" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ConversionResponse>();
        Assert.Equal(100, body!.Value);
        Assert.Equal("C", body.From);
        Assert.Equal("F", body.To);
        Assert.Equal(212, body.Result);
    }

    [Theory]
    [InlineData(0, "K", "F", -459.67)]
    [InlineData(-40, "f", "c", -40)]
    [InlineData(36.6, "C", "K", 309.75)]
    public async Task PostConversion_ValidTemperature_ReturnsResult(double value, string from, string to, double expected)
    {
        var response = await _client.PostAsJsonAsync(Url, new { value, from, to });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ConversionResponse>();
        Assert.Equal(expected, body!.Result);
    }

    [Fact]
    public async Task PostConversion_MetersToFeet_ReturnsResult()
    {
        var response = await _client.PostAsJsonAsync(Url, new { value = 10, from = "m", to = "ft" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ConversionResponse>();
        Assert.Equal(32.8084, body!.Result);
    }

    [Fact]
    public async Task PostConversion_KilogramsToPounds_ReturnsResult()
    {
        var response = await _client.PostAsJsonAsync(Url, new { value = 1, from = "kg", to = "lb" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ConversionResponse>();
        Assert.Equal(2.20462, body!.Result);
    }

    [Theory]
    [InlineData(1d, "X", "C", "Unknown unit 'X'")]
    [InlineData(1d, "C", "X", "Unknown unit 'X'")]
    [InlineData(1d, "m", "C", "Cannot convert")]
    [InlineData(1d, "kg", "m", "Cannot convert")]
    [InlineData(null, "C", "F", "required")]
    [InlineData(1d, null, "F", "required")]
    [InlineData(1d, "C", null, "required")]
    public async Task PostConversion_InvalidInput_ReturnsBadRequest(double? value, string? from, string? to, string expectedDetail)
    {
        var response = await _client.PostAsJsonAsync(Url, new { value, from, to });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemResponse>();
        Assert.Equal(400, problem!.Status);
        Assert.Contains(expectedDetail, problem.Detail);
    }

    [Fact]
    public async Task PostConversion_EmptyBody_ReturnsBadRequest()
    {
        var content = new StringContent("", Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(Url, content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemResponse>();
        Assert.Equal(400, problem!.Status);
    }

    [Fact]
    public async Task PostConversion_MalformedJson_ReturnsBadRequest()
    {
        var content = new StringContent("{bad json", Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(Url, content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemResponse>();
        Assert.Equal(400, problem!.Status);
    }

    [Fact]
    public async Task GetConversion_Any_ReturnsMethodNotAllowed()
    {
        var response = await _client.GetAsync($"{Url}?value=100&from=C&to=F");

        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }

    private record ConversionResponse(double Value, string From, string To, double Result);

    private record ProblemResponse(int Status, string? Detail);
}
