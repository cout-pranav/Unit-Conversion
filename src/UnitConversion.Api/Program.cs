using UnitConversion.Api.Conversion;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/api/health");

app.MapGet("/api/units", () => UnitConverter.Units);

app.MapGet("/api/conversions", (double? value, string? from, string? to) =>
{
    if (value is null || string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
    {
        return Results.Problem("Query parameters 'value', 'from' and 'to' are required.", statusCode: 400);
    }

    try
    {
        var result = UnitConverter.Convert(value.Value, from, to);
        return Results.Ok(new { value, from, to, result });
    }
    catch (ArgumentException ex)
    {
        return Results.Problem(ex.Message, statusCode: 400);
    }
});

app.Run();
