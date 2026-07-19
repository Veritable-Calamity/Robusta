# Behavioral Specifications

Specifications translate accepted product promises into observable scenarios without selecting the mechanisms that implement them.

## Authority and status

- Accepted product and technical ADRs remain authoritative when a specification summarizes them.
- A scenario whose `specificationStatus` is `specified` has a stable name and observable outcome. It does not claim that the behavior is implemented.
- Implementation status and executable evidence belong in the evidence ledger rather than the behavioral catalog.
- `decisionDependencies` names product or technical decisions that must be accepted before the scenario can freeze a public API or durable format.

## Product behavior scenario catalog

[`product-behavior-scenarios.json`](product-behavior-scenarios.json) converts the proof statements from its declared `sourceDecisionIds` into technology-neutral Given/When/Then contracts and stable conformance-test identifiers. Architecture tests validate the catalog against [`behavioral-scenarios.schema.json`](behavioral-scenarios.schema.json) and compare its exact decision/scenario pairs with the evidence ledger.

The catalog deliberately says what an external user or test must observe, not which entity store, scheduler, serializer, protocol, package layout, or process supervisor Robusta must use. Those choices remain behind the listed decision dependencies.
