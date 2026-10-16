using UnitConversion.Api.Conversion;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseStatusCodePages();

app.MapHealthChecks("/api/health");

app.MapGet("/api/units", () => UnitConverter.Units);

app.MapPost("/api/conversions", (ConversionRequest? request) =>
{
    if (request?.Value is not double value
        || string.IsNullOrWhiteSpace(request.From)
        || string.IsNullOrWhiteSpace(request.To))
    {
        return Results.Problem("Fields 'value', 'from' and 'to' are required.", statusCode: 400);
    }

    var (from, to) = (request.From, request.To);

    try
    {
        var result = UnitConverter.Convert(value, from, to);
        return Results.Ok(new { value, from, to, result });
    }
    catch (ArgumentException ex)
    {
        return Results.Problem(ex.Message, statusCode: 400);
    }
});

app.Run();

record ConversionRequest(double? Value, string? From, string? To);
