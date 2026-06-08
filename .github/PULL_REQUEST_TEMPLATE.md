<!-- Thanks for the PR. Please complete this checklist. -->

## Summary

<!-- One paragraph: what changed and why. Link related issues with `Closes #N`. -->

## Type of change

- [ ] Bug fix (non-breaking change which fixes an issue)
- [ ] Feature (non-breaking change which adds functionality)
- [ ] Breaking change (fix or feature that changes existing behavior)
- [ ] Documentation only
- [ ] Build / CI / tooling

## Checklist

- [ ] `dotnet build` passes locally with **zero warnings** (CI is `TreatWarningsAsErrors=true`).
- [ ] `dotnet test` passes locally on `net10.0` (and on `net8.0` if you targeted it).
- [ ] New / changed public API is documented with XML doc comments.
- [ ] `CHANGELOG.md` updated under `[Unreleased]`.
- [ ] If hot-path or publisher behavior changed: ran `Commander.Benchmarks` and pasted the relevant rows below.

## Benchmarks (if applicable)

```
<!-- paste BDN summary table here -->
```
