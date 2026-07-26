# Robusta Platform Design Documentation

**Working baseline:** 2026-07-18
**Decision scope:** the greenfield, release-grade Robusta platform
**Runtime capability status:** ownership groundwork only; gameplay capabilities remain unproven
**Release scope and evidence baseline:** established

The repository contains a buildable scaffold and an established first-release scope and evidence baseline. The baseline defines scope, support, artifact-feed, measurement, migration-census, and evidence rules; it does not claim a gameplay capability.

## Project layout

```text
sdk/                         Published game-facing projects
  Robusta.Game.Sdk/          Common contracts and build integration
  Robusta.Game.Shared/       Contracts available to client and server game code
  Robusta.Game.Client/       Client-only game contracts
  Robusta.Game.Server/       Server-only game contracts
src/                         Internal platform runtime projects
  Robusta.Runtime.Shared/    Runtime foundations shared by both sides
  Robusta.Runtime.Client/    Client host entry point
  Robusta.Runtime.Server/    Dedicated-server host entry point
tools/                       Product tool entry points
  Cli/                       Supported creator workflow
  ContentCompiler/           Deterministic content compilation
  Migration/                 Assisted Robust Toolbox migration
  PackageVerifier/           Package integrity and provenance checks
tests/
  Robusta.Architecture.Tests/ Repository topology and dependency-boundary tests
  Robusta.Runtime.Tests/      Internal runtime behavior tests
docs/                        Product direction and decision records
```

Directories provide logical organization; they do not define assembly boundaries. A
subsystem can live in folders and namespaces within an existing project, and does
not require its own project or project-specific directory. Add a project only when
the code needs an independently enforced publication, dependency, executable,
platform, side, trust, or deployment boundary.

The current projects use separate directories as a convenience, not as a repository
requirement. `Robusta.slnx` groups projects by their major logical area, but solution
folders, project boundaries, and physical source folders do not need to mirror one
another one-to-one.

Build the scaffold with:

```powershell
dotnet restore Robusta.slnx
dotnet build Robusta.slnx --no-restore
dotnet test Robusta.slnx --no-restore
```

Build the versioned bootstrap artifact feed and verify ordinary package consumption with:

```powershell
./eng/build-artifact-feed.ps1
./eng/verify-external-consumption.ps1
```

See [`docs/status/first-release-scope-and-evidence-baseline.md`](docs/status/first-release-scope-and-evidence-baseline.md) for the accepted first-release support matrix and auditable evidence contract.

This repository-ready documentation set records the product-level decisions agreed during the Robusta platform design workshops and evolves as each new question is reviewed.

Robusta's continuing mission is to take a lessons-learned approach to Robust Toolbox while making independently packaged games and user-created content first-class concerns. The new effort keeps that mission but treats the desired result as a complete product rather than another engine prototype.

## What this set contains

- `docs/product/platform-constitution.md` — the promises that govern the platform.
- `docs/product/quality-bar.md` — what “done” and “release-quality” mean.
- `docs/product/glossary.md` — plain-language terms used throughout the design.
- `docs/decisions/` — the ADR process, template, accepted decisions, and active proposals.
- `docs/workshops/` — accepted workshop records, active proposals, and the remaining question set.
- `docs/status/development-plan.md` — the evidence-gated implementation roadmap derived from the accepted ADRs.
- `docs/reference/` — source notes and a map from the prototype-era ADRs.
- `CODEX-HANDOFF.md` — the quick entry point for continuing the workshop in Codex.

## Important distinction

An **accepted decision** states the direction the product has chosen. It does not claim that the code already satisfies that decision.

Every ADR therefore carries two separate fields:

- **Decision status** — whether the design direction is accepted.
- **Implementation status** — whether the behavior has been built and demonstrated.

The initial product ADRs are accepted. Their implementation status is `Not started` unless a future review updates it with evidence.

## How this set should be used

1. Keep the platform constitution short and stable.
2. Use product ADRs for promises visible to developers, players, creators, and operators.
3. Use later technical ADRs to choose mechanisms that fulfill those promises.
4. Require technical ADRs to name the product ADRs they serve.
5. Record implementation evidence separately from design acceptance.
6. Supersede decisions rather than quietly rewriting their history.

## Suggested repository placement

This bundle can be copied into a new Robusta repository as-is. The `docs/decisions/product/` numbering begins at `0000` because this is a new decision record rather than a continuation of the prototype repository's ADR sequence.
