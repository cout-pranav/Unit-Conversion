# Unit Conversion

An ASP.NET Core Web API (.NET 10) for converting values between units, with a simple React front-end.
See [project-scope.md](project-scope.md) for the scope and [tech-stack.md](tech-stack.md) for the technologies used.

> **Current status:** temperature (Celsius, Fahrenheit, Kelvin) and length (mm, cm, m, km, in, ft, yd, mi). Mass comes next.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) LTS (includes npm)

## Project structure

```
Unit-Conversion/
├── src/
│   ├── UnitConversion.Api/      # ASP.NET Core Web API
│   └── UnitConversion.Web/      # React front-end (Vite + TypeScript)
├── tests/
│   └── UnitConversion.Tests/    # xUnit tests
└── UnitConversion.sln
```

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

Open http://localhost:5173. The page should show **API: Healthy** and a converter with a category picker (temperature, length).
If the API isn't running, the page shows **API unreachable** instead.

During development, the Vite dev server forwards every `/api/*` request to the API at `http://localhost:5080` (see `vite.config.ts`), so no CORS setup is needed.

## Test

```bash
dotnet test
```

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
| GET    | `/api/units`  | Lists the supported units (`C`, `F`, `K`)      |
| GET    | `/api/conversions?value=100&from=C&to=F` | Converts a value, e.g. `{ "value": 100, "from": "C", "to": "F", "result": 212 }`. Unknown units, mixed categories or a missing value return `400` with a ProblemDetails body. |

In Development, the OpenAPI document is available at http://localhost:5080/openapi/v1.json.
You can also send requests from [UnitConversion.Api.http](src/UnitConversion.Api/UnitConversion.Api.http) in VS Code or Visual Studio.
