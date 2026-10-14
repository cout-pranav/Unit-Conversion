# Tech Stack

## Backend
- **ASP.NET Core Web API** on **.NET 10** (latest LTS), C#, using minimal APIs
- Built-in **health checks** (`/api/health`) and **OpenAPI** document (Development only)
- **xUnit** for unit tests, plus `Microsoft.AspNetCore.Mvc.Testing` for endpoint tests (`dotnet test`)

## Frontend
- **React 19** with **TypeScript**, scaffolded with **Vite 8**
- **oxlint** for linting
- **Node.js** (LTS) and **npm**
- During development, the Vite dev server proxies `/api` requests to the Web API, so no CORS setup is needed

## Tooling
- .NET SDK 10
- Node.js LTS
- `.editorconfig` for consistent code style
- Git

## Principle
All conversion logic lives in the API. The front-end only collects input, calls the API and shows the result or the error.
