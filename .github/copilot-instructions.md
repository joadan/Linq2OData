# Copilot Instructions for Linq2OData

## Project Overview

Linq2OData is a modern, type-safe .NET library for building OData queries using LINQ expressions. It supports OData v2, v3, and v4 with automatic syntax adaptation. The library also includes a code generator (`Linq2OData.Generator`) to produce typed clients from OData `$metadata` XML.

## Repository Structure

```
src/
  Linq2OData.Core/        # Core library: query builders, expression handlers, HTTP client
  Linq2OData.Generator/   # Code generator: parses OData $metadata and produces C# client files
test/
  Linq2OData.Tests/       # xUnit tests covering filters, expands, projections, ordering, CRUD, metadata parsing
  Linq2OData.TestClients/ # Sample generated clients used in integration tests
docs/                     # Markdown documentation for specific features
```

## Tech Stack

- **Language:** C# on .NET 10
- **Test framework:** xUnit
- **Build system:** `dotnet` CLI with `Linq2OData.slnx` solution file
- **Nullable reference types and implicit usings** are enabled across all projects

## Build & Test

```bash
dotnet build          # build all projects
dotnet test           # run all tests
```

Run these commands from the repository root. Tests live under `test/Linq2OData.Tests/`.

## Coding Conventions

- Follow existing file-level namespace declarations and indentation (tabs).
- Use `nullable enable` and handle nullability correctly.
- Keep LINQ expression handling in `src/Linq2OData.Core/Expressions/`.
- Builder classes in `src/Linq2OData.Core/Builders/` follow a fluent API pattern — maintain that pattern when adding new builder methods.
- OData version differences (v2/v3 vs v4) are handled internally; new features should respect `ODataVersion` branching already present in the codebase.

## Testing

- Add tests in `test/Linq2OData.Tests/` using xUnit (`[Fact]` and `[Theory]`).
- Follow the existing test file naming pattern (e.g., `FilterExpressionTests.cs`, `ExpandTests.cs`).
- Integration tests that require a real HTTP endpoint are in `ODataClientIntegrationTests.cs`; unit tests should not require network access.
- The `test/Linq2OData.TestClients/` project contains generated client code used by tests — regenerate it with the `Linq2OData.Generator` if metadata changes.
