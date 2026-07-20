# ADR 0034: Use a declared ladder for advanced game extensions

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Supersedes:** None
- **Related decisions:** 0003, 0004, 0007-0010, 0018, 0022, 0025, 0026, 0029, 0033

## The question

How should an advanced game add unusual engine-facing behavior without depending on Robusta internals or weakening the support and fault-containment boundary?

## The promise

Teams have a visible path from ordinary game code to supported advanced integration and, when needed, a platform contribution. The path identifies compatibility, trust, scheduling, packaging, and support consequences before a game ships.

## Why this matters

No public SDK can anticipate every renderer, physics adapter, codec, device, native library, or specialized simulation. A hidden internal escape hatch would make ADRs 0003, 0008, 0010, 0018, 0025, and 0026 unenforceable. A total ban would push serious games toward forks.

## How Robust Toolbox answers today

Robust Toolbox and Space Station 14 evolve together, so engine contributions and direct engine assumptions are practical within their shared development culture. Robusta must support separately versioned games whose maintainers cannot rely on repository friendship or synchronized source changes.

## How the Robusta prototype answers today

The predecessor exposes implementation assemblies and extension patterns but does not yet prove a versioned advanced-extension contract, compatibility declaration, package audit, or fault classification.

## Options considered

### Option A: A declared extension ladder

Use an ordinary SDK system first, then a published advanced extension or adapter, then a platform contribution. Anything else is explicitly nonconforming and unsupported rather than silently tolerated.

### Option B: A supported internal escape hatch

Allow trusted games to access internal services, reflection, or service location when necessary. This is flexible but turns internal layout into a de facto public API and makes upgrades and fault ownership unpredictable.

### Option C: No advanced extension path

Require every unusual need to wait for a platform release. This protects the boundary but makes forks and abandoned migrations likely.

## Decision

Robusta will use Option A.

This decision is consistent with accepted technical ADR 0018's still-unimplemented requirement for a small, separately versioned advanced capability interface and now supplies its governing product constraints.

The product contract is:

1. Extension choices follow this order: ordinary supported Game SDK code; a documented advanced extension or adapter contract; a reviewed platform contribution; or an explicitly nonconforming unsupported integration.
2. A game does not move up the ladder merely for convenience or performance speculation. It must document the missing capability, required guarantees, affected sides, and why a lower rung is insufficient.
3. Every advanced extension has a manifest identity and version, compatible platform range, execution side, trust requirement, requested capabilities, resource ownership, thread or phase affinity, determinism class, fault policy, packaging rules, and inspection surface.
4. Advanced extensions use published contracts. They do not gain friend assembly access, reflection over private state, undocumented service location, layout patches, or direct mutation of another scope's internals.
5. The scheduler admits extension work under ADR 0029's declared access and effect model. `ExclusiveWorld` is a conservative schedule for otherwise conforming work whose admitted world access is widened; it does not make undeclared effects, private access, unsafe code, native calls, or unknown ownership conforming.
6. Unknown or unverifiable private, unsafe, native, or external-effect work is rejected from the conforming extension path or explicitly classified as nonconforming. Native or unsafe integration is allowed only through a later approved adapter contract and may require exclusive execution, a separate process, narrower platforms, reduced recovery guarantees, and additional supply-chain review.
7. ADR 0026 classifies faults honestly. An in-process trusted extension is not sandboxed, and an integrity-unknown fault may still require host termination.
8. Public UGC cannot load advanced executable extensions. ADR 0007's data-and-declared-operation boundary remains intact.
9. Extension packages obey the same exact receipt, side-specific payload, verification, upgrade, diagnostics, and rollback rules as other release artifacts.
10. Nonconforming integration is detectable in builds, release manifests, and conformance reports. The affected integration and every capability whose determinism, isolation, fault, packaging, portability, reload, or compatibility result it can influence cannot use the `Supported` label; it remains `Experimental` or is excluded from the capability register until a conforming boundary has evidence. Unaffected capabilities retain their labels only when an enforced process or capability boundary proves they cannot be influenced.
11. Installation and operator diagnostics disclose the nonconforming package, affected sides and platforms, process boundary, requested operating-system powers, native dependencies, lost guarantees, support owner, and upgrade or removal path before activation. In-process nonconforming code invalidates world-local isolation and fault-containment claims for that game-host process.
12. ADR 0025 migration results classify an unconverted nonconforming legacy integration as `Manual port` or `Unsupported`; it cannot count as `Exact`, `Renamed`, or `Converted with warning` evidence until it uses a supported equivalent contract. The product-facing labels `Manual port` and `Converted with warning` map exactly to ADR 0025's machine categories `ManualPort` and `ConvertedWithWarning`.
13. A broadly reusable extension may become a platform contribution only through normal review, compatibility, documentation, and evidence requirements; promotion does not grant its prior private contract permanent support.

## What we deliberately will not do

- Publish a general-purpose `GetInternalService`, reflection, memory-patching, or private-assembly escape hatch.
- Mark unknown code parallel-safe, deterministic, or fault-contained without enforceable evidence.
- Allow native or advanced executable extensions in public UGC.
- Promise cross-platform availability for an extension whose dependencies do not support it.
- Emulate Robust Toolbox internals or binaries to preserve an unsupported extension.

## Consequences

### Benefits

- Advanced teams can integrate unusual technology without normalizing engine forks.
- Compatibility and fault consequences are visible before deployment.
- The ordinary SDK remains the ergonomic and most strongly supported path.
- Migration tooling can distinguish adaptable extensions from manual platform work.

### Costs and limitations

- Extension contracts, manifests, analyzers, packaging checks, and compatibility tests require ongoing maintenance.
- Some integrations receive narrower support guarantees or require a separate process.
- Platform contribution review can be slower than a private engine patch.
- Unsupported cases remain possible and must be reported candidly.
- One nonconforming in-process extension can prevent the containing execution profile and affected capabilities from being Supported even when unrelated platform capabilities remain supported elsewhere.

## How we will prove the decision works

- An external game ships one unusual adapter through a published advanced contract without internal assembly access.
- The package audit reports side, trust, capabilities, native dependencies, compatibility range, and exact receipt membership.
- An analyzer rejects private reflection, undocumented service access, and undeclared authoritative effects.
- A deterministic extension matches the serial oracle; otherwise conforming work with conservatively widened access can use its declared exclusive boundary, while undeclared native or external effects are rejected.
- A fault exercise demonstrates the documented world-local or host-level outcome for the extension's class.
- Migration tooling classifies an unsupported Robust Toolbox integration as manual platform work instead of claiming binary emulation.
- A native-adapter fixture proves side-specific payload projection, installation, upgrade, rollback, crash cleanup, and compatibility reporting on every supported operating-system profile it claims.

## Implementation notes

No advanced-extension ABI, manifest, adapter API, analyzer, native boundary, or contribution process is implemented.

## Follow-up decisions

- Advanced-extension manifest, versioning, and compatibility policy.
- Native-library loading, process isolation, ABI, resource lifetime, and crash reporting.
- Renderer, physics, codec, device, and external-service adapter contracts.
- Platform contribution and experimental-capability graduation policy.
- Conformance tests and migration classifications for advanced integrations.

## References

- [ADR 0003](0003-preserve-straightforward-game-authoring.md)
- [ADR 0007](0007-separate-trusted-games-from-public-ugc.md)
- [ADR 0025](../technical/0025-migrate-through-a-source-located-intermediate-model-and-conformance-corpus.md)
- [ADR 0026](0026-define-supported-game-code-conformance-and-fault-containment.md)
- [ADR 0029](../technical/0029-enforce-phase-scoped-access-and-buffered-effects.md)
- [World-model question 19](../../workshops/world-model-question-set.md#19-how-should-advanced-games-extend-foundations-without-bypassing-the-sdk)
