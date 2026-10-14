# CLAUDE.md

Project memory for Claude Code. See [project-scope.md](project-scope.md) for scope and [tech-stack.md](tech-stack.md) for technologies.

## What this is
Unit conversion app: an ASP.NET Core Web API (.NET 10, minimal APIs) and a thin React + TypeScript (Vite) front-end.
Currently supported: **temperature** (C, F, K). Next: length and mass.

## Commands (run from repo root unless noted)
- API: `dotnet run --project src/UnitConversion.Api` → http://localhost:5080
- Tests: `dotnet test`
- Front-end (in `src/UnitConversion.Web`): `npm install`, `npm run dev` (http://localhost:5173), `npm run lint`, `npm run build`

## Layout
- `src/UnitConversion.Api/Program.cs`: all endpoints (`/api/health`, `/api/units`, `/api/conversions`)
- `src/UnitConversion.Api/Conversion/`: `Unit` record and the static `UnitConverter` (unit list and logic)
- `src/UnitConversion.Web/src/api/`: one file per API area (`health.ts`, `conversions.ts`), each wrapping `fetch`
- `src/UnitConversion.Web/src/App.tsx`: the single page
- `tests/UnitConversion.Tests/`: xUnit tests. Unit tests for the converter, plus endpoint tests using `WebApplicationFactory<Program>`

## Rules
- **All conversion logic lives in the API.** The front-end never does unit math. It loads units from `/api/units` and calls `/api/conversions`.
- **Adding a unit means adding one entry** to `UnitConverter.Units`. Each unit has a `Factor` and an optional `Offset` relative to its category's base unit (kelvin, meter or kilogram): `base = (value + Offset) * Factor`. Don't add per-unit code.
- Results are rounded to 5 decimals in `UnitConverter.Convert`.
- Invalid input (unknown unit, mixed categories, missing value) returns `400` with a ProblemDetails body (`Results.Problem`). The converter signals these cases with `ArgumentException`.
- Every new unit or behavior gets a test case (`[Theory]`/`[InlineData]` in `UnitConverterTests`).
- Keep changes minimal. Don't add extras (CI, Docker, extra libraries) unless asked.

## Gotchas
- Vite proxies `/api` to `http://localhost:5080` (`vite.config.ts`), so there's no CORS setup. If the API port changes, update `launchSettings.json`, `vite.config.ts`, `UnitConversion.Api.http` and README together.
- `dotnet run` doesn't hot-reload. Restart the API after backend changes. On Windows, stop a running API before `dotnet test`/`dotnet build`, because it locks the build output.
- The front-end linter is oxlint, and its `set-state-in-effect` rule is on. Set state in promise callbacks or event handlers, not synchronously inside `useEffect`.
