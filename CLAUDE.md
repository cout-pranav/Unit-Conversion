# CLAUDE.md

Project memory for Claude Code. See [project-scope.md](project-scope.md) for scope and [tech-stack.md](tech-stack.md) for technologies.

## What this is
Unit conversion app: an ASP.NET Core Web API (.NET 10, minimal APIs) and a thin React + TypeScript (Vite) front-end.
Currently supported: **temperature** (C, F, K), **length** (mm, cm, m, km, in, ft, yd, mi) and **mass** (mg, g, kg, t, oz, lb, st).

## Commands (run from repo root unless noted)
- API: `dotnet run --project src/UnitConversion.Api` → http://localhost:5080
- Tests: `dotnet test`
- Front-end (in `src/UnitConversion.Web`): `npm install`, `npm run dev` (http://localhost:5173), `npm run lint`, `npm run build`

## Layout
- `src/UnitConversion.Api/Program.cs`: all endpoints (`GET /api/health`, `GET /api/units`, `POST /api/conversions` with a JSON body `{ "value": 10, "from": "m", "to": "ft" }`, bound to the `ConversionRequest` record)
- `src/UnitConversion.Api/Conversion/`: `Unit` record and the static `UnitConverter` (unit list and logic)
- `src/UnitConversion.Web/src/api/`: one file per API area (`conversions.ts`), each wrapping `fetch`
- `src/UnitConversion.Web/src/App.tsx`: the single page (category picker + converter). The categories come from `/api/units`, so a new category needs no front-end change
- `tests/UnitConversion.Tests/`: xUnit tests. Unit tests for the converter, plus endpoint tests using `WebApplicationFactory<Program>` (conversion requests sent with `PostAsJsonAsync`)

## Rules
- **All conversion logic lives in the API.** The front-end never does unit math. It loads units from `/api/units` and POSTs JSON to `/api/conversions` (never query parameters).
- **Adding a unit means adding one entry** to `UnitConverter.Units`. Each unit has a `Factor` and an optional `Offset` relative to its category's base unit (kelvin, meter or kilogram): `base = (value + Offset) * Factor`. Don't add per-unit code.
- Results are rounded to 5 decimals in `UnitConverter.Convert`.
- Invalid input (unknown unit, mixed categories, missing value/field or body) returns `400` with a ProblemDetails body (`Results.Problem`). The converter signals these cases with `ArgumentException`. Framework-level errors (malformed JSON → 400, missing `Content-Type` → 415, GET → 405) also return ProblemDetails via `AddProblemDetails()` + `UseStatusCodePages()`.
- Every new unit or behavior gets a test case (`[Theory]`/`[InlineData]` in `UnitConverterTests`). The `xunit-test-writer` subagent (`.claude/agents/xunit-test-writer.md`) can write and run them.
- Keep changes minimal. Don't add extras (CI, Docker, extra libraries) unless asked.

## Gotchas
- Vite proxies `/api` to `http://localhost:5080` (`vite.config.ts`), so there's no CORS setup. If the API port changes, update `launchSettings.json`, `vite.config.ts`, `UnitConversion.Api.http` and README together.
- `dotnet run` doesn't hot-reload. Restart the API after backend changes. On Windows, a running API locks the build output, so `dotnet test`/`dotnet build` fail (MSB3027). Either stop it first, or run `dotnet test --artifacts-path <temp dir>` to build elsewhere and leave it running.
- Unit symbols are matched case-insensitively and must be unique that way (e.g. `Mm` for megameter would clash with `mm`). A duplicate throws when `UnitConverter` loads.
- Rounding to 5 decimals turns very small results into `0` (e.g. `1 mg → kg`, `1 mm → km`). Use larger values in tests for these pairs.
- The front-end linter is oxlint, and its `set-state-in-effect` rule is on. Set state in promise callbacks or event handlers, not synchronously inside `useEffect`.
