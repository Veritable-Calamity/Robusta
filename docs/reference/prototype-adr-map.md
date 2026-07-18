# Prototype-Era ADR Map

The greenfield ADR set starts at `0000` and does not reuse the prototype repository's decision numbering. This table shows how useful prototype-era lessons are carried forward without treating the old implementation as binding.

| Prototype-era decision | Greenfield product decision | Treatment |
|---|---|---|
| 0001 Modular project topology | ADR 0001, ADR 0003 | Preserve clear composition and ownership as technical input; do not freeze the old project graph. |
| 0002 Dependency rules | ADR 0003 | Preserve the supported SDK boundary and explicit dependency direction; select the new module topology later. |
| 0003 Versioning and artifacts | ADR 0002, ADR 0008 | Carry forward reproducible release artifacts and explicit compatibility; redesign exact release products as needed. |
| 0004 Isolated engine hosts | ADR 0004 and future world-model ADRs | Preserve host/world isolation as a strong lesson; define product-visible world semantics before choosing implementation. |
| 0005 Server-authoritative network session | ADR 0006 | Preserve server authority, bounded behavior, explicit schemas, prediction, interpolation, and interest as product requirements. |
| 0006 Content trust and execution model | ADR 0007 | Carry forward the trust-tier distinction substantially unchanged at product level. |
| 0007 Public content API boundary | ADR 0003 | Carry forward side-specific public contracts and the prohibition on ordinary internal references. |
| 0008 Content versioning and package contract | ADR 0004, ADR 0008 | Carry forward exact locks, side separation, hashes, provenance, and compatibility coordinates. |
| 0009 Package-aware prototypes and resources | ADR 0005 | Carry forward deterministic package-aware identity, provenance, and explicit overrides. |
| 0010 Creator development loop | ADR 0009 | Carry forward the single `robusta dev` workflow and honest reload/restart matrix. |
| 0011 Validation and untrusted scripting boundary | ADR 0007 | Carry forward the distinction between validation, provenance, and actual isolation. |

## Rule for code reuse

A carried-forward decision is a requirement input, not permission to transplant its implementation automatically. Code should be moved only after the new product contract and destination architecture exist, and only after license, provenance, compatibility, and test coverage are reviewed.
