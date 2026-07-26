# Simulation Kernel Review 01: State, storage, queries, and structural commit

- **Workshop status:** Accepted technical-decision record
- **Date:** 2026-07-24
- **Last reconciled:** 2026-07-26
- **Checkpoint:** CP03
- **ADRs:** 0048, 0049, 0050, and 0051
- **Decision status:** Accepted independently via Option A
- **Implementation status:** Not started
- **Review order:** 0048 → 0049 → 0050 → 0051

## Recorded review outcome

ADRs 0048, 0049, 0050, and 0051 were each approved via Option A on 2026-07-26. Their decisions remain independent, and batch acceptance supplies no implementation or evidence claim.

The four-decision CP03 design gate is satisfied. Production CP03 work remains closed until the CP02 predecessor/evidence boundary is satisfied and each ADR's retained identity, activation, compatibility, cleanup/fault, budget, SDK, and subordinate-specification gates are met. Read-only research, reviewable prototypes, and private measurement spikes remain non-contractual.

## Why this is four decisions

| ADR | Program package | Independent boundary | Why it should remain replaceable |
|---|---|---|---|
| [0048](../decisions/technical/0048-generate-stable-component-and-world-resource-schemas.md) | `SIM-STATE` | What component and world-resource state means | Semantic declarations and compatibility can remain stable while storage and query implementations change |
| [0049](../decisions/technical/0049-keep-ecs-storage-private-behind-world-owned-envelopes.md) | `SIM-STORAGE` | How world-owned state is stored without leaking layout | Dense, sparse, tag, resource, allocation, and compaction policy can evolve behind the SDK |
| [0050](../decisions/technical/0050-generate-phase-scoped-queries-with-canonical-iteration.md) | `SIM-QUERY` | How systems borrow and iterate state with one deterministic meaning | Query order, lifetime, changes, and partitions are public semantics rather than consequences of one store |
| [0051](../decisions/technical/0051-plan-and-publish-structural-changes-through-atomic-commit-frontiers.md) | `SIM-COMMIT` | How structural intent becomes one complete published outcome | Planner, conflict, reversal, and publication mechanics refine ADR 0042 without becoming storage or query APIs |

Combining these would make a storage optimization capable of changing schema meaning, query order, or transaction semantics. Splitting them further now would separate facts that share one compatibility and failure boundary inside each package.

## Accepted Option A positions

### ADR 0048 — generated semantic state manifests

Use typed source declarations to generate one normalized, bounded, language-neutral semantic manifest. Components and world resources receive distinct nominal identities, explicit versions, side and authority rules, semantic defaults and limits, deterministic normalization, and allow-listed projection eligibility. A required world resource remains present while its world owner is `Open`, while permitted value writes or atomic replacement cannot create an absent interval. CLR names, reflection order, storage layout, and serializer conventions are not schema authority.

### ADR 0049 — private world-owned hybrid storage

Keep dense component, sparse-or-packed component, tag-membership, and world-resource roles behind one world-owned storage envelope and one authoritative entity handle/generation table. Mutable pools are world-private in the first release; buffers do not transfer between worlds. Physical rows, chunks, sparse indexes, allocators, reuse, relocation, compaction, and fragmentation remain private and measurable. Stale, wrong-world, ending, ended, and exhausted references never alias replacements.

### ADR 0050 — generated phase-scoped canonical queries

Generate typed query and resource views from the state manifest and ADR 0029 access declarations. Logical `QuerySchemaId` and exact `QueryDescriptorId` values follow ADR 0044 and grant no access by possession. Borrows cannot escape their phase. Required, optional, and excluded terms have explicit behavior. Canonical logical order and scheduler-issued ordered partitions are independent of storage layout, hash order, worker assignment, and completion timing. Change baselines are conservative, versioned, bounded, and non-wrapping. Owned observations are bounded immutable values, not checkpoints.

### ADR 0051 — prepared structural plans and one publication gate

Normalize structural commands into immutable typed plans with closed resource keys, deterministic conflict handling, bounded transaction groups, prepared fallible work, declared reversal classes, and a bounded apply journal. Every command receives exactly one terminal result, including every member of a transaction group. Committing work publishes one new structural version only after every affected core store and index agrees; rejection-only or semantic-no-op frontiers retain the prior version. Results have a world-owned retrieval, expiry, tombstone, and close/fault lifecycle whose numeric horizons remain budget-profile work. Unknown integrity follows the accepted fault contract; it is never reported as ordinary rejection.

## Dependency and scope checks

1. ADR 0048 supplies semantic schema inputs without selecting storage.
2. ADR 0049 consumes the accepted schema contract but exposes no public layout.
3. ADR 0050 consumes the accepted schema and storage-adapter contracts but independently defines public query meaning.
4. ADR 0051 consumes the three accepted predecessors to implement accepted ADR 0042's structural semantics.
5. CP03 implementation initially covers entity lifecycle, component presence and values, world resources, generated queries, core query indexes, and entity generation tables.
6. Maps, spatial frames and relations, timers, catalog adoption, replication, persistence, inspection transport, Test SDK packaging, and replay artifacts join these envelopes only under their later checkpoint decisions.
7. Exact numeric limits remain workload-profile work. Decision acceptance does not turn unmeasured values into support claims.

## Repository organization

These decision packages do not imply one project per package or one project-specific directory per project. Implement related code in logical folders and namespaces within an existing project when its artifact and dependency boundary remains the same. Add a project only for a real publication, dependency, generator-hosting, executable, platform, side, trust, or deployment boundary.

## Recorded answers

1. ADR 0048: Option A accepted.
2. ADR 0049: Option A accepted.
3. ADR 0050: Option A accepted.
4. ADR 0051: Option A accepted.

The batch approval named all four ADRs. Their decision and implementation statuses remain independent, and every implementation status remains `Not started`.

## Work opened by acceptance

- Write and review the exact `SIM-STATE` manifest, canonical encoding, diagnostic catalog, identity declarations, and known-answer fixtures.
- Build the minimal private storage reference model and family-equivalence, stale-handle, churn, allocation-failure, and cleanup fixtures.
- Generate phase query views and access manifests, then prove non-escape, canonical iteration, ordered partition recomposition, change-baseline behavior, and invalidation.
- Implement the core structural planner, prepared store adapters, bounded journal, agreement validator, publication token, immutable results, and injected-failure matrix.
- Review the applicable CP03 budget and CP04 world-fault profiles before making scale, continuation, cleanup, or production scheduler claims.

## Non-goals of this batch

- Selecting a renderer, physics backend, network transport, checkpoint encoding, or replay format.
- Publishing raw ECS stores, archetypes, chunks, rows, pointers, or custom storage providers.
- Treating query observations as checkpoints or replay artifacts.
- Claiming rollback for arbitrary component-value writes or real external effects.
- Pulling CP04, CP05, CP08, CP11, or CP12 mechanisms into the CP03 implementation slice.
- Claiming implementation evidence from an accepted design.

## References

- [ADR development program](../status/adr-development-program.md)
- [Platform development roadmap](../status/platform-development-roadmap.md)
- [First-release technical scope matrix](../status/first-release-technical-scope-matrix.md)
- [Technical evaluation workloads](../specifications/technical-evaluation-workloads.md)
