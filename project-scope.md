# Project Scope: Unit Conversion API

## Overview

An ASP.NET Core Web API (.NET 10) that converts a numeric value from one unit of measurement to another, for example meters to feet, Celsius to Fahrenheit, or kilograms to pounds. A simple React page lets users try the API.

## In Scope

- A REST endpoint for conversions:
  `GET /api/conversions?value=10&from=m&to=ft`
- A REST endpoint listing supported units by category (used by the front-end):
  `GET /api/units`
- Supported categories: **length**, **temperature**, **mass/weight**
- Units and conversion factors are hardcoded
- Input validation with clear error responses (unknown unit, units from different categories, missing value)
- Unit tests for the conversion logic (xUnit)
- A simple React front-end page: pick a category, enter a value, choose the from/to units, and see the result or the API error
- `README.md` explaining how to build, run and test locally
- `tech-stack.md` describing the technologies used

## Out of Scope

- Database or external storage
- Authentication / authorization
- Currency conversion
- Conversion logic in the front-end (the UI only calls the API)

## Design Note

Each category has a **base unit** (meter, kilogram, kelvin). Every unit defines how to convert to and from its base unit using a factor and an optional offset (the offset is needed for temperature).

A conversion is always `from unit → base unit → to unit`. Adding a new unit therefore only means adding one data entry, not new code. This keeps the design ready to grow to hundreds of units later.

## Project Structure

```
Unit-Conversion/
├── src/
│   ├── UnitConversion.Api/
│   └── UnitConversion.Web/      # React front-end (Vite + TypeScript)
├── tests/
│   └── UnitConversion.Tests/
├── UnitConversion.sln
├── .editorconfig
├── .gitignore
├── project-scope.md
├── tech-stack.md
└── README.md
```

## Done When

- `10 m → ft` returns `32.8084`
- `100 °C → °F` returns `212`
- `1 kg → lb` returns `2.20462`
- Converting between categories (e.g. `m → kg`) returns `400 Bad Request`
- `dotnet run` and `dotnet test` work from a clean clone
- `npm install` and `npm run dev` in `src/UnitConversion.Web` show the page, which converts values through the API
