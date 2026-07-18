# Behavioral Specifications

Specifications translate accepted product promises into observable scenarios without selecting the mechanisms that implement them.

## Authority and status

- Accepted product and technical ADRs remain authoritative when a specification summarizes them.
- A scenario marked `specified` has a stable name and observable outcome. It does not claim that the behavior is implemented.
- `implementationClaim` remains `none` until executable evidence is linked through the evidence ledger.
- `openDecisionGates` names product or technical decisions that must be accepted before the scenario can freeze a public API or durable format.

## Milestone 1 catalog

[`m1-behavioral-scenarios.json`](m1-behavioral-scenarios.json) converts the proof statements from product ADRs 0003-0009 and 0011-0013 into technology-neutral Given/When/Then contracts and stable test names. The catalog is validated against [`behavioral-scenarios.schema.json`](behavioral-scenarios.schema.json) and the M0 evidence ledger by architecture tests.

The catalog deliberately says what an external user or test must observe, not which entity store, scheduler, serializer, protocol, package layout, or process supervisor Robusta must use. Those choices remain behind the listed M1 decision gates.
