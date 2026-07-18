# ADR 0000: Adopt the Robusta Platform Constitution

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-18
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Supersedes:** None; begins the greenfield decision set
- **Related decisions:** ADR 0001 through ADR 0010

## The question

What durable promises should guide the new Robusta when individual design choices compete?

## The promise

Developers, players, creators, and operators can understand the platform's priorities without reading implementation code or reconstructing intent from many unrelated decisions.

## Why this matters

Starting from scratch creates freedom, but it also makes it easy to revisit foundational questions repeatedly or optimize one subsystem at the expense of the whole product.

The first Robusta effort produced valuable lessons about isolated hosts, game-facing boundaries, generated registration, packages, trust, networking, and creator workflow. Robust Toolbox demonstrates a productive systems-components-prototypes authoring model and the value of a demanding real game. The new effort needs a stable way to preserve those lessons while still allowing technical choices to evolve.

## Options considered

### Let implementation establish the direction

This is fast initially, but accidental early choices become hidden product commitments. Different subsystems may optimize for incompatible goals.

### Rely only on individual technical ADRs

This records local reasoning but does not provide a clear way to resolve conflicts between safety, compatibility, creator ease, performance, and internal simplicity.

### Adopt a short product constitution

This creates a stable set of user-visible promises while allowing technical implementations to change beneath them.

## Decision

Adopt `docs/product/platform-constitution.md` as the governing product direction for the greenfield Robusta platform.

Product and technical ADRs must be consistent with the constitution or explicitly propose an amendment. Technical elegance alone is not sufficient reason to violate a published user promise.

Decision acceptance and implementation progress are tracked separately. Accepting the constitution does not claim that the new platform has already fulfilled it.

## What we deliberately will not do

- Treat prototype code as the product specification merely because it already exists.
- Use architecture labels such as “modular,” “archetype,” or “secure” as substitutes for demonstrated outcomes.
- Quietly rewrite accepted principles when implementation becomes inconvenient.
- Freeze low-level mechanisms in the constitution.

## Consequences

### Benefits

- Future ADRs share a common purpose and vocabulary.
- Conflicting proposals can be evaluated against visible priorities.
- The project can replace internal mechanisms without losing its product identity.
- External contributors can understand why decisions exist.

### Costs and limitations

- Some seemingly expedient shortcuts will require explicit rejection.
- A constitutional amendment will require broad review.
- The constitution cannot answer detailed world, networking, content, or package semantics; later ADRs remain necessary.

## How we will prove the decision works

- Every accepted technical ADR names the product promise it serves.
- Release gates report evidence against the constitution.
- Two external games can use the released platform without privileged access.
- User-visible claims are supported by tests, diagnostics, or measured tasks.

## Implementation notes

The constitution is accepted as design direction. No greenfield implementation is claimed by this record.

## Follow-up decisions

ADR 0001 through ADR 0010 record the first concrete product decisions derived from the constitution.
