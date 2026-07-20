# ADR 0018: Publish a layered Game SDK with capability boundaries

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Technical
- **Owners:** SDK and trust workstreams
- **Supersedes:** None
- **Product decisions served:** 0001, 0002, 0003, 0006, 0007, 0009, 0010, 0013, 0014
- **Related decisions:** 0017, 0019, 0021, 0023, 0024, 0025

## The question

How will Robusta expose ordinary game authoring, side-specific capabilities, and advanced extensions without making runtime internals or full-power code part of the public UGC model?

## The promise preserved

An external game implements ordinary features through published components, systems, events, definitions, and capabilities. Client and server code receive only their declared surfaces, while public add-ons cannot silently become executable host code.

## Why this matters

The current project scaffold already suggests common, shared, client, and server SDK layers. Without a technical rule, references can drift, generated registration can become manual, and an “advanced” escape hatch can erase the trust boundary.

## Options considered

### Option A: Layered reference assemblies plus generated declarations

Publish a minimal common contract, shared simulation surface, and client- and server-specific surfaces. Source generators produce registration and schema metadata; analyzers enforce side and lifetime rules.

### Option B: One universal SDK assembly

This simplifies discovery but exposes unavailable or privileged APIs everywhere and makes client/server leakage easy.

### Option C: Runtime internals as the advanced SDK

This offers maximum power but freezes implementation details and makes upgrades and migration unsafe.

## Decision

Robusta will use Option A:

1. `Robusta.Game.Sdk` contains side-neutral annotations, identity-free values, diagnostics contracts, and generation hooks.
2. `Robusta.Game.Shared` contains authoritative simulation contracts usable by shared game code and references only the common SDK.
3. `Robusta.Game.Client` and `Robusta.Game.Server` extend the shared surface with side-specific capabilities. Neither references the other.
4. Runtime projects may implement SDK contracts; SDK projects never reference runtime assemblies. External conformance games compile only against published reference packages.
5. Components, systems, events, synchronization intent, content bindings, and capability requirements use generated deterministic manifests. Manual registration is not required for ordinary authoring.
6. Analyzers fail builds for side leakage, undeclared capability use, forbidden service location, and references to Robusta internals.
7. Trusted advanced extensions use a small, separately versioned capability interface and are installed as executable game material. They are never described as sandboxed.
8. Public add-ons use declarative schemas and capability-limited interpreted operations supplied by the game. They do not ship loadable .NET assemblies.
9. Compatibility shims and migration packages sit above the native SDK and cannot introduce dependencies from native SDK assemblies back to Robust Toolbox.

## What we deliberately will not do

- Publish runtime implementation types as ordinary SDK contracts.
- Use assembly scanning as the only registration mechanism.
- Put server administration, filesystem, socket, or process APIs in the shared or client SDK.
- Treat analyzers, signatures, or load contexts as hostile-code containment.

## Consequences

### Compatibility and migration

Public packages require explicit versioning and API compatibility reports. Breaking Preview changes remain possible but must produce migration diagnostics. Legacy compatibility stays replaceable above the native SDK.

### Security

Side separation reduces accidental secret leakage. Public UGC capabilities require adversarial denial tests; trusted executable extensions retain the permissions of their game process.

### Operations

Generated manifests and package identities appear in diagnostics and release receipts. Generator failures must identify source declarations rather than generated files alone.

## How we will prove the decision works

- Architecture tests enforce the project-reference graph and reject runtime references from every SDK project.
- A clean external game implements the `ExternalInteractiveNetworkedObject` scenario using published packages only.
- Client artifact inspection finds no server-only assemblies or resources.
- Generator reproducibility tests produce byte-identical normalized declarations from identical source.
- A denial suite proves public add-ons cannot access files, sockets, processes, reflection, or undeclared capabilities.
- Migration corpus builds prove the compatibility package remains above the native SDK.

## Implementation notes

The project graph exists as a scaffold. No public contract, generator, analyzer, advanced extension, or UGC runtime is implemented.

## Follow-up decisions

- Declarative UGC instruction model and resource budgets.
- Public API compatibility policy after Preview.
- Renderer, input, audio, UI, and physics capability surfaces.

## References

- [ADR 0003](../product/0003-preserve-straightforward-game-authoring.md)
- [ADR 0007](../product/0007-separate-trusted-games-from-public-ugc.md)
- [ADR 0013](../product/0013-use-entities-for-independent-world-participants.md)
- [Project structure tests](../../../tests/Robusta.Architecture.Tests/ProjectStructureTests.cs)
