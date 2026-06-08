# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- `IStreamPipelineBehavior<TRequest, TResponse>` — composable behaviors around streaming queries.
- `Commander.SourceGenerator` — incremental Roslyn generator emitting reflection-free
  `services.AddCommanderHandlers()` registration. Ships inside the `CommanderCQRS` NuGet under
  `analyzers/dotnet/cs`. AOT / trim safe.
- `[RequiresUnreferencedCode]` / `[RequiresDynamicCode]` annotations on assembly-scanning APIs and
  the validation pipeline behavior; core library is now marked `IsAotCompatible=true`.
- `Microsoft.CodeAnalysis.PublicApiAnalyzers` baselines (`PublicAPI.Shipped.txt` /
  `PublicAPI.Unshipped.txt`) for `Commander` and `Commander.FluentValidation`.
- `Roslynator.Analyzers` + `Roslynator.Formatting.Analyzers` + `Roslynator.CodeAnalysis.Analyzers`
  on every project.
- Standard parameterless constructors on `CommandException` and `EventException` (RCS1194).
- Repository scaffolding: `.gitignore`, `.gitattributes`, `.editorconfig`, `global.json`,
  `Directory.Build.props`, `Directory.Packages.props`, `LICENSE`, `CONTRIBUTING.md`,
  `SECURITY.md`, GitHub Actions CI workflow, Dependabot config, PR / issue templates.

## [2.0.0] — 2026-06

Major rewrite focused on performance, modern .NET, and feature parity with (and beyond) MediatR.

### Added
- Multi-target `net8.0` (LTS) and `net10.0` (LTS).
- `ISender` / `IPublisher` split; `ICommander : ISender, IPublisher`.
- `IPipelineBehavior<TRequest, TResponse>` for cross-cutting concerns.
- `IStreamQueryHandler<TRequest, TResponse>` for `IAsyncEnumerable<T>` streaming queries.
- `INotificationPublisher` strategy with two built-in implementations:
  - `ForeachAwaitPublisher` (default — sequential, deterministic).
  - `TaskWhenAllPublisher` (parallel fan-out, exception-aggregating).
- `ICommandValidator<T>` provider-agnostic abstraction; built-in `ValidationPipelineBehavior<,>`.
- New options-based `services.AddCommander(cfg => ...)` registration with assembly scanning, behavior registration, lifetime, and publisher selection.
- Separate `Commander.FluentValidation` adapter package with `CommanderValidator<T>` and `cfg.AddFluentValidation()`.
- `CancellationToken` parameter on every public-facing dispatch and handler method.
- `Commander.Benchmarks` project comparing Commander vs MediatR with BenchmarkDotNet.

### Changed
- Dispatcher rewritten — fully generic so the JIT specializes call sites; no per-call reflection.
- Result types use cached singletons for `Success()` to reduce allocations.
- Default event publishing is now sequential (`ForeachAwaitPublisher`); previous behavior is opt-in via `cfg.UseNotificationPublisher<TaskWhenAllPublisher>()`.
- `Event.AggreggateId` typo renamed to `Event.AggregateId` (and `SetAggregateId`).
- `DateTime.Now` replaced with `DateTime.UtcNow` everywhere.

### Fixed
- Exception rethrows now preserve the original `Exception` instead of `ex.InnerException` (which was almost always `null`).
- `BaseType.Is(...)` NullReferenceException risk in registration eliminated.
- `QueryResult<T>.IsFailed` `new` modifier shadowed nothing — replaced with proper `Failed` / `FailedAsync`.
- `Task.WhenAll` no longer hides aggregate exceptions; `TaskWhenAllPublisher` surfaces the full `AggregateException`.
- Empty event subscriber list no longer throws — returns success.

### Removed
- Hard FluentValidation dependency from the core package — moved to optional adapter.

### Migration notes from 1.x
See README "Migration from 1.x" table.

## [1.0.2] — 2021

- Initial public release.

[Unreleased]: https://github.com/lucasmdocarmo/Commander.CQRS/compare/v2.0.0...HEAD
[2.0.0]: https://github.com/lucasmdocarmo/Commander.CQRS/releases/tag/v2.0.0
[1.0.2]: https://github.com/lucasmdocarmo/Commander.CQRS/releases/tag/v1.0.2
