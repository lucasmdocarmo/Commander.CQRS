# Contributing to Commander.CQRS

Thanks for considering a contribution. This document covers the workflow and the conventions the repo enforces.

## Prerequisites

- .NET SDK as pinned in [`global.json`](./global.json) (currently `10.0.x`).
- Any modern IDE: Visual Studio 2022+, JetBrains Rider, or VS Code with the C# Dev Kit.

## One-time setup

```bash
git clone https://github.com/lucasmdocarmo/Commander.CQRS.git
cd Commander.CQRS
dotnet restore
```

## Build, test, format

```bash
dotnet build                                            # multi-target net8.0;net10.0
dotnet test                                             # xUnit suite
dotnet format                                           # apply .editorconfig
dotnet run -c Release --project Commander.Benchmarks    # benchmarks vs MediatR
```

CI runs the same commands on every PR — please run them locally first.

## Project layout

```
Commander/                    # core library (the published NuGet)
Commander.FluentValidation/   # optional FluentValidation adapter
Commander.Playground.Tests/   # xUnit test suite
Commander.Benchmarks/         # BenchmarkDotNet — vs MediatR comparisons
```

Repo-wide conventions live in `Directory.Build.props`, `Directory.Packages.props`, `.editorconfig`, and `.gitattributes`.

## Conventions

- **Nullable**: enabled. New types must annotate references correctly.
- **Async**: prefer `ValueTask` on hot paths. Always accept and forward `CancellationToken`.
- **Public API**: changes that affect the public surface require a corresponding `CHANGELOG.md` entry and a SemVer-appropriate version bump.
- **Tests**: every fix should ship with a test. Test names follow `Method_Scenario_Expected`.
- **Performance**: if you touch the dispatch hot path or notification publishers, please justify with a `Commander.Benchmarks` run.
- **Commits**: imperative mood, ≤ 72 chars subject, optional body explaining motivation.
- **PRs**: one logical change per PR; rebase on `main` before requesting review.

## Adding a dependency

All package versions live in [`Directory.Packages.props`](./Directory.Packages.props) (Central Package Management). Add a new `<PackageVersion>` there, then reference it from a csproj **without** a `Version` attribute.

## Reporting an issue

- Searched first to make sure it isn't already filed?
- Repro steps with **the smallest possible** code sample.
- `dotnet --info` output.

## Code of conduct

By participating you agree to keep the project welcoming and harassment-free. Be excellent to each other.
