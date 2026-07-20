# ADR 0025: Migrate through a source-located intermediate model and conformance corpus

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-19
- **Decision level:** Technical
- **Owners:** Migration workstream
- **Supersedes:** None
- **Product decisions served:** 0002, 0003, 0005, 0008, 0010
- **Related decisions:** 0018, 0019, 0021, 0023, 0024

## The question

How will migration tools convert Robust Toolbox source and data, expose semantic differences, and prove behavior without introducing binary compatibility into the native SDK?

## The promise preserved

A migrating developer receives a finite source-located report classifying what transferred exactly, what was renamed or converted, and what requires manual redesign or remains unsupported.

## Why this matters

Direct text rewriting loses semantic context, while binary compatibility would force Robusta to reproduce predecessor internals. Migration must target native contracts and measure observable outcomes.

## Options considered

### Option A: Inventory, typed migration IR, analyzers/code fixes, and golden conformance corpus

Parse legacy code and data into source-located facts, map those facts into native content and SDK concepts, emit edits plus a classification ledger, and compare representative behavior.

### Option B: Namespace and text replacement

This is fast and retains limited value as a discovery or suggestion pass for mechanically obvious names and syntax. It cannot determine lifecycle, networking, content, service, or behavioral equivalence and therefore cannot stand alone or assign `Exact` classifications.

### Option C: Load legacy binaries behind an emulation layer — ruled out

This would make predecessor behavior a permanent runtime dependency and undermine the native platform. It must not be implemented. When a useful Robust Toolbox capability is missing, Robusta should implement an equivalent native method in its own SDK and runtime, then migrate callers to that method.

## Decision

Robusta will use Option A. Option B may exist only as a clearly labeled, reviewable accelerator inside the Option A pipeline; its output remains a suggestion until semantic analysis and applicable conformance evidence validate it. Option C is prohibited.

1. A versioned census records legacy API, attribute, content-key, service, and behavioral-pattern usage with source locations and representative frequency.
2. Code migration uses Roslyn semantic models; content migration uses format front ends that target ADR 0021's typed IR. An optional textual discovery pass may suggest reviewable edits for simple names or syntax, but cannot classify semantic equivalence or bypass the typed pipeline.
3. Every source item receives one stable identity and one classification: `Exact`, `Renamed`, `ConvertedWithWarning`, `ManualPort`, or `Unsupported`.
4. A migration rule declares matched legacy semantics, required context, native output, classification, diagnostics, confidence limitations, and conformance scenarios.
5. Re-running the same rule set is deterministic and idempotent for already migrated output. Generated edits preserve user-authored formatting where Roslyn or the source format can do so safely.
6. The temporary compatibility package references only published native SDK packages. Native SDK and runtime assemblies never reference Robust Toolbox types.
7. The report aggregates every item by package and feature, links generated edits and diagnostics to original spans, and never treats compile success as behavioral success.
8. A versioned corpus exercises components, events, prediction, inventory, prototypes, UI, containers, transforms, maps, physics, localization, appearance, administration, and saved data. Scenarios blocked by unaccepted Robusta product decisions remain visibly blocked rather than approximated.
9. Each automated rule ships golden input/output fixtures and, where behavior matters, executable predecessor-versus-Robusta observation comparisons.

## What we deliberately will not do

- Build, load, or emulate unchanged legacy game assemblies as a migration or compatibility path.
- Recreate predecessor internals when an equivalent native Robusta capability can serve the product behavior.
- Label a textually similar conversion as exact without semantic evidence.
- Hide unsupported behavior behind compatibility names.
- Rewrite user files without a reviewable patch and report.

## Consequences

### Compatibility and migration

Migration rule versions, legacy baseline versions, native target receipts, and report schemas are recorded separately. Upgrading the migration tool may change classifications only through an explained rule change.

### Security

Migration inspects source and data without executing imported game assemblies. Parsers and analyzers use resource limits, treat paths as untrusted, and emit patches outside immutable source inputs until the user applies them.

### Operations

CI publishes coverage by usage frequency, classification, rule, scenario, and target release. Manual and unsupported work remains visible in release planning.

## How we will prove the decision works

- The versioned migration corpus produces deterministic reports and reviewable patches on clean machines.
- Every corpus item has exactly one classification and source location.
- Observable conformance tests cover each automated behavior-bearing rule; compilation-only evidence is rejected.
- Architecture tests prove the native SDK and runtime have no Robust Toolbox dependency.
- Re-running migration on migrated output produces no unintended edits.

## Implementation notes

The repository contains census and corpus baselines plus a migration command scaffold. No semantic analyzer, importer, report generator, compatibility package, or conformance runner exists.

## Follow-up decisions

- Migration report and rule schemas.
- Predecessor execution harness and license/provenance handling.
- Map, save, and replay migration after their product contracts are accepted.

## References

- [ADR 0010](../product/0010-assisted-robust-toolbox-migration.md)
- [Migration census](../../status/migration/census-v1.json)
- [Migration conformance corpus](../../status/migration/conformance-corpus-v1.json)
