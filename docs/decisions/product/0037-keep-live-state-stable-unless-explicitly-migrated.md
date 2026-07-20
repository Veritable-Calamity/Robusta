# ADR 0037: Keep live state stable unless explicitly migrated

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Supersedes:** None
- **Amends:** ADR 0024, decision clause 6 and its rollback proof
- **Related decisions:** 0005, 0008, 0009, 0012, 0015, 0021, 0024, 0026, 0029, 0035, 0036

## The question

What happens to already-running objects when a creator changes a prototype or adopts a new immutable catalog generation?

## The promise

Changing a recipe never silently resets or rebases live world state. New births use the adopted definition; existing objects change only through an explicit, validated migration or an honest restart outcome.

## Why this matters

Prototype defaults become mutable gameplay state after birth. Automatically reapplying them cannot reliably distinguish an untouched default from damage, player customization, system state, or an intentional override. Conversely, restart-only development would make ordinary content iteration unnecessarily slow.

## How Robust Toolbox answers today

Robust Toolbox provides productive runtime prototype reload facilities, but prototype inheritance, serialization, component mutation, and content callbacks make the effect on existing SS14 entities feature-specific. Robusta needs one inspectable product rule rather than inherited callback behavior.

## How the Robusta prototype answers today

The predecessor's prototype manager is mutable and does not implement immutable catalog generations, an adoption classifier, reversible live migration, or the creator transaction promised by ADR 0024.

## Options considered

### Option A: Stable existing state with explicit catalog adoption and migration

Treat prototypes as birth recipes. A new generation affects future births by default; a declared migration may update existing objects only after full preparation, with atomic per-world visibility and explicit multi-world failure outcomes.

### Option B: Automatically rebase existing objects onto changed defaults

This can make simple edits appear instantly, but override tracking and state reconciliation become ambiguous and may reset legitimate gameplay changes.

### Option C: Pin every world until restart

This is safe and simple, but rejects useful resource, future-birth, and explicitly migratable changes and weakens the accepted creator workflow.

## Decision

Robusta will use Option A.

The product contract is:

1. A prototype is an immutable catalog recipe used to prepare birth. Values copied into a live object become world state, not a live view over prototype defaults.
2. Compiling creator changes produces a new immutable catalog generation. No running world's shared generation changes in place.
3. Adoption of a compatible definition generation changes future births. Existing live objects keep their current values, capabilities, relationships, and lifecycle state by default.
4. A field or capability that intentionally consults current catalog data at runtime must declare that behavior. It is a catalog reference, not an implicitly rebound birth default.
5. A live-state migration names its source and target generations, target population, eligible before state, resulting state, admission authority, prepared inverse, and failure behavior. Preparation validates every affected object before any adoption is published.
6. Migration may use only reversible world-local changes admitted by the lifecycle and scheduler contracts. External effects, irreversible callbacks, or unknown aliases make the change restart-only.
7. A **prepare failure** changes no targeted world. If any target cannot prepare, the supervisor discards every prepared delta and leaves all targets on their prior generation and state.
8. Before a multi-world commit begins, every targeted world is fenced at a declared boundary from authoritative advancement, new births, input admission, external effects, and client publication. It remains fenced until global success or completed reversal. If one coordinator cannot fence every target and keep every prepared inverse valid, the change uses independent per-world outcomes or restart instead of claiming all-or-none live adoption.
9. Before commit, every relevant client must possess and validate the target catalog generation and compatible schemas. An unready client reconnects, restarts, or remains detached according to the declared outcome. No new-generation birth or migrated state is replicated to a client that has not admitted that generation.
10. While all targets remain fenced, the supervisor commits each prepared delta. A **known commit failure before success is published** uses the prepared inverses to return every committed target to its prior generation and state. Only after every target acknowledges the new generation may the supervisor publish success, release the fences, resume advancement, and permit external or client effects.
11. A **post-commit fault or integrity-unknown commit outcome** does not trigger arbitrary world rewind. ADR 0026 faults the affected world or host at its safe boundary, diagnostics report each known generation and disposition, and restart or operator recovery uses explicit checkpoints where applicable.
12. These limits amend ADR 0024's statement that all affected worlds roll back "on failure": it means prepare rejection or proven reversible commit failure while all targets remain fenced, not arbitrary postcommit gameplay rollback.
13. Code, component-layout, network-schema, persistence-schema, or other incompatible changes require restart. Preserving a world across that restart requires the explicit checkpoint and forward-migration path of ADR 0035.
14. Inspection reports the active world generation, each object's birth definition and generation, applied migration provenance, transaction stage and outcome, client admission state, and whether a value is live state or an explicit current-catalog reference.

## What we deliberately will not do

- Reapply prototype defaults silently to damaged, customized, or otherwise changed live objects.
- Mutate an immutable catalog underneath a running world.
- Claim arbitrary code, layout, network, or persistence changes are safely hot-reloadable.
- Run irreversible external effects inside a catalog-adoption transaction.
- Describe fault recovery as whole-world rollback when no such transaction exists.

## Consequences

### Benefits

- Creator changes cannot silently destroy legitimate live state.
- Safe future-birth and explicit migration changes remain available without restart.
- Reload, restart, and rejection outcomes become explainable and testable.
- Saves and inspectors retain exact catalog and migration provenance.

### Costs and limitations

- A world may contain objects born under different catalog generations.
- Creators must author migrations for intentional live changes.
- Catalog metadata and old resources may need bounded retention.
- The reload classifier will conservatively restart when reversibility is unproven.
- Multi-world live adoption pauses every targeted world and may be less usable than a coordinated restart for large or distributed fleets.

## How we will prove the decision works

- Changing a door prototype's initial durability does not reset damage or open state on existing doors; newly born doors use the new value.
- A declared compatible migration updates every eligible object in one committed outcome.
- A prepare failure in one affected world leaves all targeted worlds on their prior catalog and state.
- While a multi-world commit is in progress, targeted worlds cannot advance or publish effects; fault injection during the known reversible commit restores every target before success is published.
- A client missing the target generation cannot receive a new-generation birth or migrated state and follows the declared reconnect, restart, or detach outcome.
- A postcommit game fault stops under ADR 0026 and reports the committed generation without claiming arbitrary world rollback.
- A component-layout or network-schema change reports restart rather than pretending to reload.
- A restart-preserved world follows ADR 0035 migration and backup behavior.
- Inspection distinguishes birth values, current world state, current-catalog references, and applied migrations.

## Implementation notes

No catalog-adoption matrix, prototype-state provenance, live migration API, reversible delta, or world generation switch exists.

## Follow-up decisions

- Catalog-adoption impact matrix and compatibility classifier.
- Live migration declaration, preparation, commit, and diagnostic schemas.
- Catalog and resource lifetime, retention, and garbage collection.
- Client/server generation agreement and reconnect behavior.
- Save migration interaction and creator-supervisor transaction details.

## References

- [ADR 0009](0009-one-supported-creator-workflow.md)
- [ADR 0012](0012-separate-game-host-and-world-state.md)
- [ADR 0021](../technical/0021-compile-content-into-a-canonical-provenance-catalog.md)
- [ADR 0024](../technical/0024-supervise-the-creator-loop-as-an-observable-transaction.md)
- [ADR 0026](0026-define-supported-game-code-conformance-and-fault-containment.md)
- [ADR 0035](0035-persist-declared-world-state-through-versioned-checkpoints.md)
- [World-model question 22](../../workshops/world-model-question-set.md#22-what-happens-when-prototypes-change-while-objects-already-exist)
