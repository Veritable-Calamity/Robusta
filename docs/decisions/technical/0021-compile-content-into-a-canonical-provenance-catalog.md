# ADR 0021: Compile content into a canonical provenance catalog

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Technical
- **Owners:** Content workstream
- **Supersedes:** None
- **Product decisions served:** 0002, 0003, 0005, 0008, 0010, 0012, 0014
- **Related decisions:** 0018, 0022, 0023, 0024, 0025

## The question

How will readable package content become one deterministic, validated, inspectable catalog with stable identity, provenance, side classification, and fingerprints?

## The promise preserved

Creators edit readable files, while every runtime consumes a precompiled exact catalog whose values, references, sources, and compatibility identities are reproducible and inspectable.

## Why this matters

Parser choice, filesystem ordering, inheritance, patches, and serializer defaults can otherwise make equivalent builds differ or let invalid content survive until gameplay.

## Options considered

### Option A: Front ends normalize into one typed IR and canonical binary catalog

Parse supported authoring formats into a source-located intermediate representation, resolve all semantics there, validate, and emit canonical data plus a separate inspection/provenance index.

### Option B: Interpret source files at runtime

This shortens the prototype path but repeats resolution on every machine and makes release identity and diagnostics unstable.

### Option C: Treat one authoring syntax as the runtime schema

This conflates human syntax with durable semantics and makes future authoring formats or migration importers costly.

## Decision

Robusta will use Option A:

1. Every definition has a structured package-qualified identity: package identity, definition kind, and case-sensitive local identity. Physical paths are provenance, never identity.
2. YAML and any later front end produce one typed IR containing normalized scalar values, explicit operation kinds, side classification, dependency edges, and source spans.
3. Resolution runs in declared package-dependency order and stable identity order. Inheritance and patches use documented merge, replace, remove, and reset operations; undeclared cross-package changes fail.
4. Validation resolves every typed reference, side rule, required field, schema constraint, and supported resource or localization reference before emission.
5. Diagnostics have stable codes and carry package, logical identity, original file, span, provenance chain, and corrective context.
6. Canonical emission fixes encoding, field order, collection order, numeric representation, and absence/default representation. Timestamps, machine paths, locale, and enumeration order are excluded.
7. The catalog fingerprint is a domain-separated cryptographic digest of canonical semantic output and schema identities. Provenance and inspection data have separate fingerprints so source paths do not change runtime identity.
8. Client, server, and shared projections are emitted from the same resolved catalog. A client projection cannot contain server-only definitions or resources.
9. The runtime memory-maps or loads only the canonical compiled representation and verifies its fingerprint before use. It does not reinterpret authoring files.
10. Catalog generations are immutable. Development compilation produces a new generation for transactional adoption or restart.

The initial binary encoding and digest algorithm will be versioned in the catalog envelope and may be replaced through an explicit format migration.

## What we deliberately will not do

- Make YAML node order, filesystem order, or parser-specific objects part of runtime semantics.
- Permit ambiguous short identities to resolve by whichever package loads first.
- Hash source paths into the semantic catalog identity.
- Emit client material before server-only leakage checks pass.

## Consequences

### Compatibility and migration

Authoring schema, canonical catalog format, and semantic fingerprint are separate compatibility dimensions. Importers target the IR and must report lossy conversions.

### Security

Compilation rejects malformed structure, undeclared package reach, and side leakage before launch. Resource limits are required for hostile or oversized input; compiled catalogs are still data, not executable trust.

### Operations

Builds publish fingerprints, normalized inspection output, diagnostic summaries, and cache hit information. Cache keys include compiler, schema, exact dependency lock, and source digests.

## How we will prove the decision works

- `CatalogReproducibility` produces byte-identical semantic catalogs on clean Windows and Ubuntu machines.
- Fuzzed file order, locale, line endings, and source roots do not change semantic output.
- `SourceDiagnostic` and `ResolvedCatalogInspection` prove source-quality diagnostics and final provenance.
- Side-projection audits find no server-only content in client artifacts.
- Migration fixtures classify every imported value and compare resolved behavior, not parse success alone.

## Implementation notes

`tools/ContentCompiler` is a command scaffold. No authoring schema, IR, canonical encoding, or runtime reader exists.

## Follow-up decisions

- Exact IR and catalog schemas.
- Map and grid authoring after the space product gate.
- Save/reference schema after the persistence product gate.

## References

- [ADR 0005](../product/0005-compile-readable-content.md)
- [ADR 0008](../product/0008-explicit-versions-migrations-and-rollback.md)
- [Content behavioral scenarios](../../specifications/product-behavior-scenarios.json)
