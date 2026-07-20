# ADR 0017: Enforce explicit runtime ownership scopes

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Technical
- **Owners:** Runtime workstream
- **Supersedes:** None
- **Amended by:** 0028
- **Product decisions served:** 0000, 0001, 0002, 0004, 0007, 0011, 0012, 0014, 0015, 0016
- **Related decisions:** 0003, 0006, 0008, 0009

## The question

How will the runtime enforce process, installation, catalog, host, session, and world ownership without allowing mutable gameplay state to leak between worlds or into privileged delivery processes?

## The promise preserved

One exact trusted game installation may run several isolated worlds, while incompatible or untrusted games run in separate processes. Destroying one world releases its resources without changing another world, its host, player sessions, immutable definitions, or delivery infrastructure.

## Why this matters

The product ADRs assign state to distinct owners. If those assignments remain documentation only, static services, service locators, or overly broad dependency injection can quietly recreate one global simulation.

## Options considered

### Option A: Hierarchical scopes with capability-checked resolution

Create explicit process, catalog-generation, host, session, and world scopes. A child may use an immutable parent capability, but mutable services cannot be resolved from a broader lifetime unless their contract explicitly declares cross-world or durable behavior.

### Option B: One application service provider with lifetime conventions

This is familiar, but violations are discovered through discipline or shutdown bugs rather than structural checks.

### Option C: One process per world

This gives strong isolation but prevents the accepted trusted multi-world host and makes previews and isolated tests unnecessarily expensive.

## Decision

Robusta will use Option A:

1. The launcher, updater, verifier, and credential holder remain processes that never load game assemblies.
2. One game-host process loads one exact executable game installation and one or more immutable catalog generations. Different executable games or trust domains use different processes.
3. Runtime composition creates typed lifetime scopes in this order: process infrastructure, catalog generation, host, session, and world. Scope identity is carried in diagnostics.
4. Mutable gameplay capabilities default to world scope. A broader mutable capability must use an explicitly reviewed host, session, cross-world, or durable contract.
5. Public game code receives capabilities through construction or declared activation. It receives no global service locator and cannot resolve capabilities from an arbitrary scope.
6. Scope validation rejects a longer-lived service that captures a shorter-lived mutable service. Catalog services expose immutable snapshots only.
7. World disposal first closes admission, then ends world-owned entities and work, drains cleanup within a published budget, aggregates failures, and releases the scope. Cleanup cannot reach another world.
8. Player connections and sessions are host-owned; avatar attachment is an explicit operation referring to one world at a time.

The concrete dependency-injection library remains replaceable. Typed ownership metadata and validation behavior are the contract selected here.

## What we deliberately will not do

- Use ambient mutable globals or thread-local service collections as the ordinary game API.
- Treat an in-process scope as a security sandbox.
- Let scope disposal silently abandon failures or wait forever.
- Let catalog reload mutate definitions already supplied to a world.

## Consequences

### Compatibility and migration

Legacy global dependencies require analyzer-assisted conversion to declared capabilities. Catalog changes create a new generation and use an explicit adoption or restart path.

### Security

Privileged delivery processes remain separated from executable game code. Full-power trusted game code is still not confined merely because it uses a scope.

### Operations

Logs, metrics, traces, and cleanup reports carry game, host, session, catalog-generation, and world identities. Hosts need cleanup deadlines and leak detection.

## How we will prove the decision works

- `Robusta.Architecture.Ownership.ScopeCaptureRules` rejects invalid lifetime captures and public service-location APIs.
- `Robusta.Conformance.Worlds.TwoWorldIsolation` and `WorldDisposalIsolation` pass with one host and two worlds.
- `Robusta.Conformance.Ownership.SharedCatalogIsolatedWorldMutation` proves shared definitions remain immutable.
- A process-load audit proves delivery processes never load managed game assemblies.
- Fault injection in every disposal stage reports all failures and releases all remaining world-owned resources within the stated budget.

## Implementation notes

The repository currently contains project scaffolds only. No scope implementation or runtime evidence is claimed.

The [2026-07-19 coherence audit](../../status/adr-coherence-and-first-release-baseline-2026-07-19.md) found that the linear catalog/host/session/world order conflicts with ADR 0012's independent session and world lifetimes. [ADR 0028](0028-model-sessions-and-worlds-as-sibling-host-scopes.md) is accepted and replaces that linear order with sibling world and session scopes joined by explicit attachments.

## Follow-up decisions

- Public SDK capability declarations and advanced extension policy.
- Durable service transactions and cross-world transfer.
- Scope implementation library and generated activation metadata.

## References

- [ADR 0011](../product/0011-define-world-as-isolated-simulation.md)
- [ADR 0012](../product/0012-separate-game-host-and-world-state.md)
- [ADR 0014](../product/0014-define-first-release-boundary-and-delivery.md)
- [Two-world and ownership scenarios](../../specifications/product-behavior-scenarios.json)
