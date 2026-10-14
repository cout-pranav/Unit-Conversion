# Tech Stack

## Backend
- **ASP.NET Core Web API** on **.NET 10** (latest LTS), C#
- **xUnit** for unit tests (`dotnet test`)

## Frontend
- **React** with **TypeScript**, scaffolded with **Vite**
- **Node.js** (LTS) and **npm**
- During development, the Vite dev server proxies `/api` requests to the Web API, so no CORS setup is needed

## Tooling
- .NET SDK 10
- Node.js LTS
- `.editorconfig` for consistent code style
- Git

## Principle
All conversion logic lives in the API. The front-end only collects input, calls the API and shows the result or the error.
