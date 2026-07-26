# ADR 0047: Evaluate dimensional compatibility through bounded exact policy profiles

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-23
- **Decision level:** Technical
- **Owners:** SDK, packaging, and runtime workstreams
- **Program ID:** `FND-COMPAT`
- **Source queue IDs:** `F-COMPAT-01`, `F-COMPAT-02`, `SDK-COMPAT-01`
- **Supersedes:** None
- **Refines:** ADR 0043
- **Product decisions served:** 0002-0010, 0014, 0027, 0034-0041
- **Related decisions:** 0018, 0021-0025, 0034-0038, 0043, 0044

## The question

How will every Robusta tool and runtime derive the same bounded, explainable, operation-specific compatibility report from exact descriptors and policy inputs without collapsing dimensions into SemVer or creating bespoke evaluators?

## The promise preserved

Players, creators, and operators receive the same compatibility answer for the same exact operation and inputs on every supported machine. A failure names every relevant mismatch and the exact migration, adapter, restart, reconnect, or inspection action that may make progress possible. Compatibility never changes because a channel moved, a directory changed, a clock advanced, or one tool happened to load different policy code.

## Why this matters

ADR 0043 accepts separate compatibility dimensions, one operation-specific rules engine, a closed outcome vocabulary, exact policy identity, fail-closed checks, and bounded generated parsing. It deliberately leaves the descriptor envelope, policy language, reason taxonomy, profile extension, and evaluation mechanics open.

SemVer alone cannot answer whether a package may build, a process may launch, peers may connect, a checkpoint may restore, a catalog may be adopted, or a mesh member may join. Those operations compare different roles and dimensions and may require several simultaneous actions. Independently coded evaluators would eventually disagree and make receipts, diagnostics, and support claims untrustworthy.

The common decision must select one pure mechanism while leaving each owning subsystem to review its exact required dimensions and policy profile. It must also preserve the product boundary: a future `Supported` compatibility window and deprecation promise require `PRD-EVOLUTION`; a technical evaluator cannot invent them.

## How the current Robusta implementation answers today

The repository currently proves project references, external package restoration, and artifact-feed hashing. SDK packages use `0.1.0-preview.0`, target `net10.0`, and compile with `LangVersion` set to `latest`; `global.json` permits `latestFeature` roll-forward. Those labels and configuration texts are not exact compatibility inputs by themselves.

The artifact-feed index records package SHA-256 values but also contains an observational generation timestamp. `PackageVerifier` is scaffolding. No canonical compatibility descriptor, exact request, policy identity, evaluator, API fingerprint, generated-manifest fingerprint, stable reason registry, or compatibility report exists.

## Options considered

### Option A: Versioned declarative compatibility IR and one pure evaluator

Author structured profiles, validate and normalize them to a typed finite intermediate representation, compile that IR to an acyclic bounded decision graph, and evaluate it with one pure engine shared by tools and runtime. Profiles contain exact data and cannot load executable policy plugins.

This best satisfies ADR 0043's same-input, same-policy, same-result requirement. It supports source-quality validation, canonical policy identity, bounded evaluation, and complete explanations. It requires new schemas, a small policy compiler, fixtures, and careful profile governance.

### Option B: Generated typed C# policy evaluators

Use a builder or attribute DSL to generate typed evaluator code for each operation.

This offers strong C# authoring diagnostics and efficient execution. Exact semantics become coupled to compiler and runtime behavior, ambient API access is harder to exclude, and package, content, migration, and inspection tools must load policy code. Exporting a canonical declarative IR to solve those problems converges on Option A.

### Option C: Fixed dimension comparators and one universal result lattice

Give every dimension a fixed comparator and combine results through one global severity order. Profiles only mark dimensions required or optional.

This is small and auditable, but restart, reconnect, migration, adapter, and read-only outcomes are not one honest severity line. Cross-dimension rules such as schema plus adapter availability or native dependency plus target platform would become hidden special cases. The result would over-reject or recreate bespoke operation engines.

## Decision

Robusta will use Option A: a versioned declarative compatibility IR and one pure evaluator.

The common technical contract is:

1. Compatibility evaluation consumes only immutable, exact, schema-validated descriptors, requests, policies, profile state, migration or adapter facts, and environment facts. It performs no discovery, range resolution, network access, filesystem access, installation, loading, migration, restart, reconnect, or authorization.
2. A `CompatibilityDescriptor` contains envelope schema and version, exact subject identity and kind, and sorted unique entries keyed by stable `CompatibilityDimensionId`.
3. A descriptor entry contains exact value-schema identity and version plus a canonical bounded value or exact artifact identity. Descriptors contain facts only. Requiredness and acceptable absence belong to the operation profile, never to the producer.
4. Mutable channel names, paths, repository slots, current directories, loaded assembly enumeration, current clock, locale, and unresolved version ranges are not descriptor facts.
5. The containing operation authority selects the policy definition, resolved profile, exact policy-state snapshot, and admitted migration or adapter set from its verified receipt, configuration, or accepted owner contract. A candidate package, peer, descriptor producer, or identifier holder may advertise facts but cannot choose the evaluator policy, required dimensions, admission modes, or admitted remediation set.
6. A `CompatibilityRequest` identifies the exact operation, policy definition, resolved profile, policy-state snapshot, and evaluator semantic version; names participant roles such as candidate, current, environment, peer, source, or target; binds each role to one exact descriptor; and lists only the exact remediation facts admitted by the containing operation authority.
7. Requests are directional and may involve more than two roles. The evaluator never assumes compatibility is symmetric or reducible to one pairwise comparison.
8. A `CompatibilityReport` records every exact input identity; canonical ordered coverage records and findings; the operation-specific admitted mode or denial; all outcomes and stable reasons; exact required routes, actions, or artifacts; and language-semantic evaluation counters.
9. A finding references one or more role-and-dimension paths and carries one outcome plus one or more stable reasons. Cross-dimension policy may produce a finding spanning several dimensions, and one dimension may participate in several findings.
10. Coverage spans the canonical union of profile-declared role-and-dimension requirements and all present descriptor entries. An expected path with no entry receives `MissingRequired` or `AbsentOptional`; a present path receives `Evaluated`, `IgnoredPresent`, or `UnmatchedRejected` as applicable. Every present nonignorable entry participates in at least one finding, and every missing required path produces a blocking finding. Optional means absence is allowed; a present optional value is still evaluated and cannot be ignored merely because it is optional.
11. Human and localized text is a projection of stable reason data. It is not evaluated and cannot change the result.
12. The closed ADR 0043 outcomes retain these meanings:

| Outcome | Meaning |
|---|---|
| `Exact` | The required facts are exactly equal under this policy |
| `Compatible` | The requested operation may proceed now under a declared non-exact rule |
| `MigrationRequired` | An exact versioned migration route must be followed, then compatibility is re-evaluated |
| `AdapterRequired` | A named exact adapter must be admitted, then compatibility is re-evaluated |
| `RestartRequired` | A fresh process or owner lifecycle boundary is required, then re-evaluation occurs |
| `ReconnectRequired` | A fresh connection or attachment boundary is required, then re-evaluation occurs |
| `ReadOnlyInspection` | Mutating or ordinary use is denied; a profile may admit a named read-only mode or direct an explicit inspection operation |
| `Incompatible` | This policy exposes no admitted path for the requested operation |

13. Migration, adapter, restart, reconnect, and a redirect to a different inspection operation are not actions. The evaluator mutates nothing; a containing workflow performs the authorized action and submits a new exact request. A report that directly selects a declared named read-only mode for the current operation authorizes exactly that mode under the current request without a redundant re-evaluation. A report that redirects to a different inspection operation requires a new exact request for that operation.
14. A migration route is an exact versioned contract classified as automated, assisted, or documented manual work and may name a tool, rule set, procedure, expected inputs, and post-migration descriptor. If no route is admitted, the applicable finding is `Incompatible`; the engine never invents a migration from version distance.
15. Each operation profile declares named modes classified as ordinary target use or read-only inspection and maps complete normalized finding sets to those modes. The policy compiler enforces non-overridable admission rules: `Incompatible` admits no mode; `MigrationRequired`, `AdapterRequired`, `RestartRequired`, and `ReconnectRequired` cannot admit the original target-publication mode before the required action and exact re-evaluation; `ReadOnlyInspection` may admit only a declared read-only mode or redirect to another inspection operation; and an ordinary target mode requires complete coverage with no missing or rejected path and only `Exact` or `Compatible` findings. When ordinary use is not admissible, a direct read-only mode additionally requires explicit `ReadOnlyInspection` findings covering every role-and-dimension path that blocks ordinary use; remediation outcomes, missing coverage, or rejected coverage never imply read-only safety by themselves. `CanProceedNow` is derived only when those invariants and the profile select an admitted mode for the current operation. A UI may choose one primary display summary through stable presentation precedence, but control flow consumes every coverage record and finding.
16. Stable reasons use package-qualified identities and versioned bounded parameter schemas. Free text, exception type, tool name, or log wording is never a reason identity.
17. Core reason families cover exact and declared-compatible values; missing, unknown, optional, ignored, unmatched, or unsupported dimensions; value, schema, side, platform, or policy mismatch; migration or adapter availability and ambiguity; restart or reconnect boundaries; read-only fallback; support-policy mismatch; and policy or input limits.
18. A reviewed profile may add a bounded explanatory reason schema that maps to existing outcomes and does not alter admission semantics. A reason requiring new control meaning, authority, operator behavior, or outcome requires an amending ADR.
19. Evaluation returns a discriminated `CompatibilityEvaluationResult`: either one valid report or one bounded `CompatibilityEvaluationFailure`. A failure records stable reason identities, redacted bounded parameters, failed stage, and consumed semantic limits. It carries no report, admitted mode, or `CanProceedNow`, and the containing operation fails closed before publication.
20. The policy language is a typed total finite algebra over exact identity equality, exact finite set or graph membership, set inclusion, bounded integer and declared-enumeration relations, Boolean composition, coverage emission, and finding outcome/reason emission.
21. The language permits no loop, recursion, regex, reflection, executable plugin, arbitrary callback, I/O, network, filesystem, environment lookup, clock, randomness, locale, process state, or dynamic CLR type.
22. Any authored range or channel selection resolves at policy publication to exact identities or an exact immutable graph snapshot. Evaluation never asks what a range means "now."
23. Policies compile to an acyclic decision graph. Conflicting overlapping rules, ambiguous actions, uncovered required states, admission mappings that violate clause 15, cycles, type errors, unreachable rules, and unknown schemas are validation failures rather than first-match behavior.
24. The evaluator is pure and bounded. For the same exact request, descriptors, policy definition, resolved profile, policy-state snapshot, and evaluator semantics, it returns the same result independently of operating system, worker scheduling, filesystem order, machine path, culture, and current time.
25. `CompatibilityPolicyDefinitionId` identifies the language schema, normalized decision rules, referenced dimension and reason schemas, evaluator semantic version, and semantic limits. It does not include one operation profile or mutable policy-state snapshot.
26. `CompatibilityProfileId` identifies one flattened resolved profile: parent and extension composition, participant roles, dimension requirements, admitted operation modes, and its exact policy-definition reference.
27. `CompatibilityPolicyStateId` identifies one immutable external-fact snapshot such as admitted migrations or adapters, support or revocation facts, vulnerability data, platform graph, or an explicitly evaluated time-window state. Ambient time cannot mutate it or a completed result.
28. A request binds the exact policy-definition, resolved-profile, policy-state, evaluator, descriptor, and remediation identities. That tuple is the effective evaluation key and the complete semantic cache key; no overlapping "effective policy" identity is inferred.
29. Logical `CompatibilityDimensionId`, `CompatibilityOperationId`, `CompatibilityReasonId`, and envelope-schema identities use ADR 0044's `qualified-logical` profile. Exact descriptor, policy-definition, resolved-profile, policy-state, request, report, and persisted evaluation-failure identities use its `canonical-artifact` profile with issuers owned by their compiler, operation authority, or evaluator as declared by the schema.
30. The common schema specification activates only bounded `compatibility-artifact` serialization for these envelopes. Receipt, network, checkpoint, catalog, document, package, and public diagnostic surfaces remain forbidden or explicitly reserved until their owning reviewed profiles. Logical IDs have public diagnostic names; descriptor, state, request, report, and failure projections inherit the most restrictive redaction of their inputs.
31. The common schema specification must select domain-separated canonical bytes, SHA-256 algorithm identity, exact normalization, issuer rules, envelope bounds, and known-answer fixtures before implementation. Whitespace, source property order, comments, file path, observational timestamps, and operational counters cannot affect identity; semantic change must.
32. A profile declares participant roles and marks each known dimension required, optional, or explicitly ignorable. Missing data is allowed only for a profile-declared optional entry.
33. An unknown required dimension, schema, value version, operator, reason schema, or policy version fails closed. A parseable unknown semantic dimension is fail-closed by default and receives rejected coverage; malformed or unsupported envelope semantics produce evaluation failure.
34. A profile may explicitly allow a named extension namespace or identity to be ignored for that operation. The report records every ignored present entry. The descriptor producer cannot self-label a safety-relevant dimension optional or ignorable.
35. Profile extensions flatten to one canonical profile independent of load order. A new profile using existing envelopes, operators, outcomes, admission semantics, and reason-extension rules is a reviewed subordinate specification.
36. A new operator or outcome, executable policy, weaker unknown handling, trust or authority side effect, changed admission semantics, or changed envelope meaning requires an amending ADR. A profile cannot silently broaden its parent.
37. Every schema sets maxima for envelope bytes, roles, dimensions, value bytes, strings, nesting, policy nodes and depth, exact-set or graph edges, coverage records, findings, reason parameters, semantic evaluation steps, and cache entries.
38. Semantic counters use an exact language-versioned algorithm over operator evaluations and declared graph edges. Cache hits, allocations, elapsed time, implementation node visits, and workflow request rates are operational metrics outside canonical reports and identities.
39. Parsing validates count, uniqueness, canonical order, schema, and cycles before expensive lookup or evaluation. Comparisons are ordinal and culture-invariant; Unicode normalization occurs only where the dimension schema explicitly owns it.
40. Caches key the complete effective evaluation tuple, are bounded, and cannot turn absence, expiration, or an old mutable channel result into compatibility. Request and lookup rates are enforced by the containing workflow's explicit budget, not by the clockless semantic evaluator.
41. Possession or successful parsing of a descriptor, policy, signature, migration route, adapter, request, or report is not trust or authority. Trust verification supplies separately identified admitted facts; the evaluator neither grants permission nor loads code.
42. Reports apply ADR 0044 redaction to parameter projections. Compatibility diagnostics cannot become a raw identity, receipt, session, document, path, or private package data leak.
43. No target process, session, world, catalog generation, package load, restore, adoption, preview, extension, or mesh member becomes visible until the operation authority receives a valid report whose complete coverage and finding set selects a mode permitted by clause 15 for that publication.

## Common ADR versus subordinate profiles

This ADR selects the immutable envelope model, closed outcomes, reason identity, bounded declarative language, pure evaluator, policy-definition/profile/state identity model, extension rules, unknown handling, and security/resource invariants. It does not select any operation's concrete roles, dimensions, values, or support promise.

The separately reviewed CP01 core/Preview profile must instantiate only external SDK package restore/build, analyzer or generator admission once those artifacts exist, and repository verifier/tool invocation. It names exact SDK/API, generated-manifest, package-set, side/reference-graph, target-framework, resolved SDK/compiler, tool platform, and profile-label dimensions.

The CP01 profile starts exact-only where no accepted comparer exists. A breaking Preview change becomes `MigrationRequired` only when the request names an admitted exact automated, assisted, or documented-manual migration route; otherwise it is `Incompatible`. It cannot relabel an Experimental capability as Preview or create a `Supported` window.

Install, launch, handshake, restore, catalog adoption, map preview, extension admission, and mesh join each require later reviewed profiles after their owning decisions. `PRD-EVOLUTION` is required before a 1.0 `Supported` compatibility, deprecation, or support-window profile.

## What we deliberately will not do

- Infer compatibility from SemVer, package version, receipt equality, successful deserialization, current channel, path, or loaded assembly list.
- Collapse every outcome into Boolean compatible/incompatible or one global severity.
- Let each operation hard-code its own evaluator or dynamically load policy plugins.
- Let a descriptor producer choose which receiver requirements are optional.
- Execute migration, load adapters, restart, reconnect, install, fetch, authorize, or trust as part of evaluation.
- Treat a signature, policy, identity, or report as authority.
- Promise `Supported` compatibility or deprecation windows before `PRD-EVOLUTION`.
- Freeze package receipts, network handshakes, checkpoint formats, catalog adoption, preview semantics, or mesh membership here.
- Include observational timestamps or localized text in semantic identities.

## Consequences

### Compatibility and migration

Compatibility becomes an explicit artifact family rather than scattered version checks. Existing checks must migrate to descriptors and exact profiles. A decision-rule change creates a new policy-definition identity; a role, dimension, extension, or admission change creates a new resolved-profile identity; and an external-fact change creates a new policy-state identity. Only a new exact evaluation may produce a different result.

Migration and adapter availability remain inputs. A report can explain a possible path without claiming the action succeeded.

### Security and failure handling

The evaluator processes untrusted structured data under hard bounds and cannot execute policy code. Unknown or malformed inputs fail before publication. This limits algorithmic and confused-deputy risk but does not replace signature verification, authentication, authorization, package trust, or process isolation.

### Operations and developer experience

Every tool can show the same complete dimension findings, stable reasons, and exact remediation requirements. The added schemas and profiles are more work than one version comparison, but they prevent different tools from offering contradictory advice.

## Bounded first implementation scope

This decision authorizes the common descriptor, request, policy-definition, resolved-profile, policy-state, report, and evaluation-failure schemas; their ADR 0044 identity and bounded compatibility-artifact codec specification; policy compiler; bounded evaluator; stable core reason registry; synthetic conformance profiles; and cross-platform known-answer fixtures.

This decision does not authorize repository, package, or SDK compatibility behavior until the separate CP01 core/Preview profile is reviewed and approved. It does not implement package receipt, install, launch, networking, checkpoint, adoption, preview, extension, `Supported`, or mesh profiles.

## How we will prove the decision works

- Known-answer descriptor, policy-definition, resolved-profile, policy-state, request, report, evaluation-failure, canonical-byte, and identity fixtures match across Windows and Linux.
- Whitespace, source order, comments, machine paths, locale, worker scheduling, wall time, and observational timestamps do not change semantic identities or reports.
- Semantic descriptor, policy, reason, profile, or limit changes do change the appropriate exact identity.
- Policy validation rejects overlaps, gaps, cycles, ambiguity, type errors, unsupported operators, hidden ranges, and unbounded graphs.
- Truth-table tests preserve cross-dimension and simultaneous findings and derive the operation mode and `CanProceedNow` only from the profile's complete coverage-and-finding admission rule.
- Compiler fixtures reject ordinary target admission with any blocking or remediation outcome and reject degraded read-only admission unless explicit `ReadOnlyInspection` findings cover every blocking path, while valid direct read-only admission and redirected inspection prove their distinct request semantics.
- Unknown, missing, optional, explicitly ignored, unsupported, and extension dimensions produce the exact fail-closed or reported behavior selected by the profile.
- Fuzz and adversarial fixtures enforce every parsing, policy, graph, finding, cache, and evaluation bound without reflective construction or code loading.
- CLI, package verifier, and runtime-facing adapters produce byte-equivalent report semantics for identical exact fixtures.
- Mutating environment variables, current time, current channel, current directory, installed assemblies, or host locale cannot change a completed request.
- A dimension-independence matrix varies ADR 0043's runtime, SDK, component, message, package, catalog, network, checkpoint, document, extension, native, and platform facts independently and preserves all reasons.
- CP01 evidence later proves exact build success and fail-closed side leakage, unsupported environment, unknown generator contract, breaking API without migration, and admitted migration requirements without partially publishing restore or build outputs.

## Implementation notes

No common compatibility artifact or evaluator exists. Current package and project-graph tests prove only restoration and architecture boundaries. Artifact-feed timestamps must remain observational and outside future descriptor and policy identities.

Implementation status remains `Not started`. Expected markers include:

- `// TODO(FND-COMPAT): replace local version checks with exact descriptors and one bounded evaluator.`
- `// TODO(FND-COMPAT/CP01): emit exact SDK, package, toolchain, side, and generator facts under the reviewed profile.`
- `// TODO(PACKAGE-SCHEMA): bind compatibility descriptors and policies into exact receipts.`
- `// TODO(PRD-EVOLUTION): define Supported compatibility, deprecation, and support-window promises.`

Logical folder organization is expected; the descriptor model, policy compiler, and evaluator require separate projects only where a real artifact or dependency boundary justifies them.

## Dependencies and interaction with queued decisions

Accepted ADRs 0018, 0022, 0043, and 0044 provide the SDK, exact artifact, compatibility-outcome, and identity foundations. `PRD-EVOLUTION` blocks only the future `Supported` profile, not the common engine or CP01 Preview profile.

`PACKAGE-SCHEMA` precedes receipt and install profiles; package trust precedes trust-state integration; networking decisions precede handshake; persistence decisions precede restore; content and adoption decisions precede catalog adoption; spatial and authoring decisions precede preview; extension decisions precede extension admission; and mesh decisions precede mesh join.

## Follow-up decisions and specifications

- Descriptor, request, policy-definition, resolved-profile, policy-state, report, evaluation-failure, dimension, operation, and reason JSON Schemas.
- Domain-separated canonical encoding and SHA-256 known-answer specification.
- Policy compiler diagnostics, conformance corpus, and evaluator API.
- Reviewed CP01 core/Preview exact profile.
- SDK/API and generated-manifest fingerprint and comparison tools.
- Artifact-feed descriptor emission and `PackageVerifier` integration.
- Localized diagnostic and remediation projection.
- Later install, launch, handshake, restore, adoption, preview, extension, and mesh profiles.
- `PRD-EVOLUTION` followed by the 1.0 `Supported` profile.

## References

- [ADR 0018](0018-publish-layered-game-sdk-and-capability-boundaries.md)
- [ADR 0022](0022-install-exact-receipts-into-immutable-content-addressed-layouts.md)
- [ADR 0043](0043-use-a-typed-identity-and-compatibility-spine.md)
- [ADR 0044](0044-generate-bounded-identity-declarations.md)
- [`FND-COMPAT` program package](../../status/adr-development-program.md#foundation-compatibility-and-lifecycle)
- [Product quality bar](../../product/quality-bar.md)
- [Artifact-feed builder](../../../eng/build-artifact-feed.ps1)
- [External-consumption verifier](../../../eng/verify-external-consumption.ps1)
- [Package verifier scaffold](../../../tools/PackageVerifier/Program.cs)
