# Unit Conversion

A small web app that converts values between units of **temperature**, **length** and **mass**. It has two parts:

- **API:** an ASP.NET Core Web API (.NET 10, minimal APIs) that holds the unit definitions and does all the conversion math.
- **Front-end:** a thin React + TypeScript page (Vite). You pick a category, enter a value, choose the from/to units, and it shows the result or the API's error.

See [project-scope.md](project-scope.md) for the scope and [tech-stack.md](tech-stack.md) for the technologies used.

## Supported units

| Category    | Base unit | Symbols                                    |
| ----------- | --------- | ------------------------------------------ |
| Temperature | kelvin    | `C`, `F`, `K`                              |
| Length      | meter     | `mm`, `cm`, `m`, `km`, `in`, `ft`, `yd`, `mi` |
| Mass        | kilogram  | `mg`, `g`, `kg`, `t` (tonne), `oz`, `lb`, `st` (stone) |

Symbols are case-insensitive. Results are rounded to 5 decimals.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) LTS (includes npm)

## Run locally

Use two terminals from the repository root.

**1. Start the API** (http://localhost:5080)

```bash
dotnet run --project src/UnitConversion.Api
```

**2. Start the front-end** (http://localhost:5173)

```bash
cd src/UnitConversion.Web
npm install
npm run dev
```

Open http://localhost:5173 and you'll see a converter with a category picker (temperature, length, mass). If the API isn't running, the page shows an error and the unit lists stay empty.

During development, the Vite dev server forwards every `/api/*` request to the API at `http://localhost:5080` (see `vite.config.ts`), so no CORS setup is needed.

## Test

```bash
dotnet test
```

There are 139 xUnit tests. They cover the converter (reference values, convert-and-back, same unit, zero/negative values, rounding, case-insensitive symbols, unknown units, mixed categories) and the HTTP endpoints.

On Windows, stop a running API before `dotnet test`, because it locks the build output.

Front-end checks:

```bash
cd src/UnitConversion.Web
npm run lint
npm run build
```

## API endpoints

| Method | Path          | Description                                    |
| ------ | ------------- | ---------------------------------------------- |
| GET    | `/api/health` | Returns `200 Healthy` when the API is running  |
| GET    | `/api/units`  | Lists all supported units with their symbol, name and category |
| POST   | `/api/conversions` | Converts a value sent as a JSON body, e.g. `{ "value": 10, "from": "m", "to": "ft" }` returns `{ "value": 10, "from": "m", "to": "ft", "result": 32.8084 }`. An unknown unit, units from different categories, or a missing `value`/`from`/`to` returns `400` with a ProblemDetails body. |

Example error:

```json
{ "status": 400, "title": "Bad Request", "detail": "Cannot convert from 'm' (length) to 'kg' (mass)." }
```

In Development, the OpenAPI document is available at http://localhost:5080/openapi/v1.json.
You can also send requests from [UnitConversion.Api.http](src/UnitConversion.Api/UnitConversion.Api.http) in VS Code or Visual Studio.

## Design decisions and trade-offs

- **Every unit converts through its category's base unit.** Each unit is one data entry with a `Factor` and an optional `Offset`: `base = (value + Offset) * Factor`. A conversion is always `from → base → to`, so adding a unit means adding one line to `UnitConverter.Units`, not new code. Temperature needs the offset; length and mass use only the factor.
  *Trade-off:* the model only handles linear units (`a·x + b`). Something like decibels would need the model to change.
- **All conversion logic lives in the API.** The front-end never does unit math. It loads units from `/api/units` and builds the category picker from that data, so a new category shows up in the UI without front-end changes.
  *Trade-off:* every conversion is a network round trip. That's fine at this scale and keeps one source of truth.
- **Minimal APIs and a static converter.** The API is three endpoints in `Program.cs`, and the converter is a static class over an in-memory list. That keeps the code small and easy to read for a stateless service.
  *Trade-off:* there's no interface or dependency-injection seam. If units moved to a database or configuration, I'd introduce an injectable service.
- **`double` arithmetic, rounded to 5 decimals.** Rounding removes floating-point noise (e.g. `212.00000000000006` becomes `212`) and gives clean, predictable results.
  *Trade-off:* results smaller than 0.000005 round to `0` (e.g. `1 mg → kg`). Returning significant figures, or letting the client choose the precision, would fix that.
- **Case-insensitive symbols.** `KM`, `km` and `Km` all work, which is friendlier for a query-string API.
  *Trade-off:* symbols must be unique ignoring case. For example, `Mm` (megameter) can't be added next to `mm` as-is.
- **Consistent errors.** Invalid input returns `400` with an RFC 9110 ProblemDetails body. The converter throws `ArgumentException`, the endpoint maps it to ProblemDetails, and the front-end shows the `detail` text as-is.
- **A simple `GET` with query parameters.** A conversion is a read-only calculation, so `GET` fits, and responses are easy to try in a browser or cache.
- **Kept to scope.** Units are hardcoded, and there's no database, authentication, Docker or CI. These are listed as out of scope in [project-scope.md](project-scope.md).
- **Vite proxy instead of CORS.** Local development needs no CORS configuration.
  *Trade-off:* a production deployment would need either the front-end served from the same origin as the API, or a CORS policy.
- **Tests.** The converter tests are xUnit `[Theory]`/`[InlineData]` tables per category, so each new unit is a new row. The endpoint tests run the real app in memory with `WebApplicationFactory`.

## Project structure

```
Unit-Conversion/
├── src/
│   ├── UnitConversion.Api/      # ASP.NET Core Web API
│   │   ├── Program.cs           # Endpoints
│   │   └── Conversion/          # Unit record + UnitConverter (unit list and logic)
│   └── UnitConversion.Web/      # React front-end (Vite + TypeScript)
├── tests/
│   └── UnitConversion.Tests/    # xUnit tests (converter + endpoints)
└── UnitConversion.sln
```
