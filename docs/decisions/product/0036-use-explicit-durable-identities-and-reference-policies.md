# ADR 0036: Use explicit durable identities and reference policies

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Supersedes:** None
- **Related decisions:** 0005, 0008, 0011-0015, 0019, 0021-0023, 0028, 0030-0032, 0035

## The question

How are objects and definitions referenced in saved data, and what happens when a target no longer exists under the release that restores it?

## The promise

Saved references never revive a reused runtime handle or silently retarget by name. Their scope, target kind, continuity promise, and missing-target behavior are explicit and validated before a restored world becomes visible.

## Why this matters

ADR 0019 makes `EntityRef` world-local and non-durable. Saves still need cycles, relationships, prototype references, and occasionally identities that persist across checkpoints or world transfer. One undifferentiated identifier would conflate those responsibilities and make missing content dangerous.

## How Robust Toolbox answers today

Robust Toolbox map serialization demonstrates practical entity-reference and prototype-relative data. Robusta must additionally support isolated worlds, package-qualified definitions, generational runtime handles, exact receipts, and explicit forward migration.

## How the Robusta prototype answers today

The predecessor uses world-local monotonic entity values and has no accepted durable-identity, missing-reference, or cross-release resolution contract.

## Options considered

### Option A: Typed, scope-aware durable references with declared failure policy

Use save-local record identities for one checkpoint, opt-in durable identities only for promised continuity, canonical catalog identities for definitions, and an explicit policy for every missing target.

### Option B: Serialize runtime handles or programming-language references

This is convenient inside one process but aliases reused storage, leaks implementation layout, and cannot safely cross world or process reconstruction.

### Option C: Store strings and resolve the nearest match during load

Human-readable names look flexible, but renames, package collisions, paths, and load order can silently select the wrong target.

## Decision

Robusta will use Option A.

The product contract is:

1. `EntityRef`, a network identity, memory address, storage slot, or dependency-scope object is never a save identity.
2. Every included record receives a checkpoint-local identity used for relationships and cycles inside that checkpoint. It has no continuity promise outside that artifact.
3. A stable durable identity exists only for a concept whose game or platform contract explicitly promises continuity across checkpoints, reconstruction, or transfer. Not every entity receives one.
4. Durable identities are opaque and carry a stable issuer and declared identity domain, such as a game-owned character domain, service-owned account domain, or world-lineage domain. A repository may store identities but is not implicitly their issuer or authority. Issuer and domain scope are part of validation and diagnostics.
5. Prototype, resource, component, package, and other definition references use canonical package-qualified catalog or generated schema identities, accompanied by the compatibility identities needed to interpret them.
6. Each reference field declares its target kind and missing-target behavior. A required local target fails migration or load before publication. An optional local target becomes absence only under its declared policy and produces an inspectable diagnostic. An external or deferred target becomes a typed unresolved proxy only through a contract that names its issuer, authorization and validation rules, resolution owner, retry limits, use-before-resolution behavior, and terminal failure outcome.
7. Removal, rename, merge, split, or substitution of a durable or catalog identity requires an explicit migration. Load never guesses from display name, CLR type name, filesystem path, registration order, or an ambiguous short identity.
8. Restore allocates fresh world-local entities, resolves the complete included local reference graph, validates every optional absence and external/deferred proxy, validates relationship invariants, and only then publishes runtime handles. Publication never makes an unresolved proxy usable as though it were a local entity.
9. Copying, duplicating, or importing a map or checkpoint remaps local identities and rejects durable-identity collisions unless an explicit clone or merge policy resolves them.
10. A durable identity proves continuity, not possession or authority. It is not a secret, credential, session identity, or permission grant.

## What we deliberately will not do

- Serialize `EntityRef` or network identities as durable references.
- Give every short-lived entity a permanent global identity.
- Resolve missing content by whichever package, path, or type name looks similar.
- Turn an optional-reference policy into silent loss without diagnostics.
- Treat durable identity as authentication or authorization.

## Consequences

### Benefits

- Runtime storage reuse cannot corrupt restored references.
- Cycles and internal relationships remain representable without global identifiers.
- Package moves and renames are explicit migration events.
- Cross-world continuity stays separate from entity identity and authority.

### Costs and limitations

- Schemas must distinguish local, durable, catalog, network, and external references.
- Games must choose missing-target and duplication policies.
- Durable registries and migrations require collision and lifecycle management.
- Some legacy strings or raw IDs cannot be imported automatically.

## How we will prove the decision works

- A cyclic entity and containment graph restores with fresh `EntityRef` values and identical declared relationships.
- Reusing a runtime slot cannot make an old saved reference target a later entity.
- Two packages with the same local prototype name never cross-resolve.
- A removed required target rejects the load, while a declared optional target becomes absence with a source-located diagnostic.
- A renamed prototype restores only after its explicit migration, and an unmigrated rename never guesses.
- Duplicate map import remaps local identities and reports durable-identity collisions before publication.
- An external deferred reference can publish only as its declared typed proxy; use before resolution, retry exhaustion, and terminal failure each produce the specified outcome without aliasing a local entity.

## Implementation notes

No durable identity type, checkpoint-local reference table, resolution pipeline, missing-target policy schema, or migration registry exists.

## Follow-up decisions

- Durable and checkpoint-local identity encodings and generation rules.
- Reference-table construction, cycle resolution, and resource limits.
- Durable-identity repositories, retention, tombstones, and cross-service lookup.
- Map duplication, import, merge, and authored-identity behavior.
- Rename aliases and migration rules for catalog and generated schema identities.

## References

- [ADR 0015](0015-give-entities-an-atomic-observable-lifecycle.md)
- [ADR 0019](../technical/0019-use-generational-entity-handles-and-transactional-structural-commits.md)
- [ADR 0021](../technical/0021-compile-content-into-a-canonical-provenance-catalog.md)
- [ADR 0032](0032-reconstruct-explicitly-across-world-transfers.md)
- [ADR 0035](0035-persist-declared-world-state-through-versioned-checkpoints.md)
- [World-model question 21](../../workshops/world-model-question-set.md#21-how-are-references-represented-in-saved-data)
