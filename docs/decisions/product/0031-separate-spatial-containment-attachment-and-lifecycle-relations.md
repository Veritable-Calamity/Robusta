# ADR 0031: Separate spatial, containment, attachment, and lifecycle relations

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Supersedes:** None
- **Related decisions:** 0003, 0006, 0011-0016, 0019, 0023, 0025, 0029, 0030

## The question

How should inventories, lockers, hands, vehicles, transform parents, anchored objects, and owned dependents relate without one relationship accidentally deciding position, visibility, replication, or deletion for all the others?

## The promise

Developers can state why two objects are related and what happens when that relationship changes or either endpoint ends. Spatial ancestry, logical containment, physical attachment, lifecycle ownership, and ordinary references do not silently substitute for one another.

## Why this matters

ADR 0015 requires every relationship to declare its ending disposition. SS14-like gameplay combines nested inventories, grid anchoring, vehicles, transform parenting, PVS, and deletion. A single universal parent edge makes a harmless reparent, container transfer, or spatial split capable of causing unintended visibility, physics, network, or lifecycle effects.

## How Robust Toolbox answers today

Robust Toolbox distinguishes transforms and containers but their behavior remains deeply integrated with entity parenting, recursive deletion, visibility, physics, and PVS. This is productive for SS14 yet creates semantic migration work beyond API renames.

## How the Robusta prototype answers today

The predecessor exposes transform and entity ownership concepts but does not implement the typed relation and atomic-disposition contract selected by ADR 0015.

## Options considered

### Option A: Distinct typed relationships with declared policies

Model spatial parentage, logical containment, attachment or anchoring, lifecycle ownership, and references separately. Each relationship type declares its invariants and observable consequences.

### Option B: One universal parent relationship

One tree controls position, containment, visibility, and deletion. This is simple until real gameplay needs different parents for different responsibilities.

### Option C: Leave every relationship to game code

Games build inventories, transforms, and ownership independently. This avoids platform policy but prevents consistent lifecycle, inspection, replication, migration, and tooling.

## Decision

Robusta will use Option A.

The product contract is:

1. Robusta treats these as distinct relationship categories: spatial parent, logical containment, attachment or anchoring, lifecycle ownership, and non-owning reference.
2. A relationship declares its cardinality, cycle rules, request and admission permissions, mutation boundary, spatial effect, gameplay-visibility effect, observer-authorization and secrecy effect, network-interest treatment, physics effect, replication treatment, transfer treatment, and ADR 0015 ending disposition. ADRs 0006 and 0015 keep final authoritative admission on the authority side; a relation policy cannot grant a client final authority over shared state.
3. Adding, removing, or replacing a relation is an atomic structural operation. Observers see the complete old or new relationship state, never a partially detached object.
4. No transform, containment, attachment, or reference edge implies lifecycle ownership. Only an explicit lifecycle-owned relationship may cascade an end.
5. No lifecycle-owned relationship automatically supplies a spatial parent, containment, visibility, or physics behavior.
6. Logical containment supports nested inventories, lockers, hands, and similar membership. Its policy separately decides ordinary spatial presence, gameplay visibility, observer authorization, network interest, and container-relative presentation. Hiding an object visually never by itself authorizes transmitting its secret state.
7. Attachment or anchoring expresses physical or structural connection, including an entity-to-address binding into compact grid data. The grid or structure owner has identity and lifecycle; an ordinary compact cell address does not become a relation endpoint with its own entity lifecycle. Attachment remains distinct from transform parenting so topology changes can reassign addresses without inventing deletion ownership.
8. Vehicles or composite objects may combine several declared relations. The platform does not force them into one universal tree.
9. Rooms and zones normally remain purpose-built spatial regions unless gameplay gives them independent identity and lifecycle.
10. A relation mutation that would create a forbidden cycle, violate cardinality, lose a required disposition, or cross a world boundary fails before commit with a diagnostic.
11. Inspection exposes each relationship separately, including its owner, policy, source, and pending committed change.

## What we deliberately will not do

- Use transform ancestry as an implicit deletion cascade.
- Treat containment as merely hiding a transform child.
- Make every region, slot, socket, or grid cell an entity.
- Permit relation cycles or cross-world endpoints by accident.
- Let network interest or save behavior infer semantics from an undocumented parent field.

## Consequences

### Benefits

- Lifecycle and spatial topology changes become safer and inspectable.
- Inventories, vehicles, grids, and non-spatial ownership can coexist.
- Migration reports can name the exact legacy assumption requiring redesign.
- Contrasting games can use only the relationship categories they need.

### Costs and limitations

- More than one relation may need to change in a single transaction.
- Every platform relationship needs a policy schema and conformance tests.
- Container visibility, grid anchoring, and physics integration remain technically complex.
- Convenience APIs must not obscure which relations they modify.

## How we will prove the decision works

- A nested item moves between a hand, backpack, locker, and world position with complete atomic outcomes.
- Ending a container applies its declared end, eject or detach, rehome or transfer, or block disposition without accidental transform-based cascade.
- Splitting a constructed grid reassigns attachments and spatial parents while preserving unrelated lifecycle ownership.
- A vehicle combines spatial frame, attachment, containment, and lifecycle relations without one edge serving every role.
- Invalid cycles, cardinality violations, cross-world endpoints, and missing ending policies fail before commit.
- Two clients and inspection tooling agree on the committed relation state and visibility outcome.
- A client without observer authorization receives neither secret contained state nor an interest side channel merely because it knows the container exists.

## Implementation notes

No transform, containment, attachment, ownership-relation, visibility, physics, or relation-inspection runtime is implemented.

Robusta 1.0 requires static compact-grid attachment and a simple nested-container disposition. Dynamic grid splitting and merging remain later capability proofs unless ADR 0014 is explicitly superseded; the relation contract is defined now so the 1.0 API does not preclude them.

## Follow-up decisions

- Exact standard relationship policy vocabulary and command ordering.
- Container visibility, secrecy, and network interest.
- Transform and physics integration.
- Grid attachment, split, merge, and compact cell storage.
- Relation serialization and durable-reference policy.

## References

- [ADR 0015](0015-give-entities-an-atomic-observable-lifecycle.md)
- [ADR 0019](../technical/0019-use-generational-entity-handles-and-transactional-structural-commits.md)
- [World-model question 15](../../workshops/world-model-question-set.md#15-can-objects-contain-other-objects)
- [Robust Toolbox container system at the audited revision](https://github.com/space-wizards/RobustToolbox/blob/537c4cb02f9555fa18f489e7b05694d288887d0e/Robust.Shared/Containers/SharedContainerSystem.cs)
- [Robust Toolbox grids](https://docs.spacestation14.com/en/robust-toolbox/transform/grids.html)
