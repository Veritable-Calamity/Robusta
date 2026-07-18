# ADR 0002: Judge Quality Through User Outcomes and External Use

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-18
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Related decisions:** ADR 0000, ADR 0001, ADR 0009, ADR 0010

## The question

What does “better” mean, and when may Robusta call a feature or release complete?

## The promise

Claims about ease, quality, compatibility, performance, safety, or release readiness are backed by observable user results and external evidence.

## Why this matters

Robust Toolbox has powerful evidence: a large production game, mature tooling, releases, tests, and years of operational experience. The Robusta prototype has several cleaner architectural ideas but no released platform product. It can reasonably claim promising design; it cannot infer overall superiority from that design alone.

A greenfield project is especially vulnerable to measuring progress by internal subsystem completion while ordinary developers still cannot build a game.

## Options considered

### Measure progress by subsystem completion

This is easy to track but encourages isolated prototypes and leaves integration, diagnostics, packaging, and user workflow until late.

### Measure progress by architectural properties

Useful for internal quality, but terms such as modularity or data orientation do not prove that developers, players, or operators have a better experience.

### Measure progress by end-to-end outcomes

Internal metrics remain important, but a feature is considered complete only when an ordinary external consumer can use it successfully.

## Decision

Robusta quality is judged through user-visible outcomes, external game use, compatibility evidence, reliability, security boundaries, operational results, and representative performance measurements.

A supported capability is not complete until all applicable elements exist:

- published Game SDK surface;
- external game use;
- creator-facing diagnostics;
- automated tests;
- documentation;
- inspection or debugging support;
- known package and side behavior;
- compatibility and upgrade classification;
- performance evidence where relevant;
- security and trust treatment where relevant.

Before 1.0, at least two separately maintained external reference games are required: a station-like multiplayer slice and a meaningfully different game.

## What we deliberately will not do

- Claim performance superiority because of a chosen ECS label or storage pattern.
- call a subsystem finished when only an internal sample can access it.
- use code volume, project count, or number of features as the sole release measure.
- hide missing product pieces behind an “experimental” label while marketing the capability as supported.

## Consequences

### Benefits

- Work is organized around complete user journeys.
- Architectural claims are tested against reality.
- External games expose accidental repository privileges and station-specific assumptions.
- Release readiness becomes auditable.

### Costs and limitations

- Features take longer to declare complete because documentation, diagnostics, tooling, and packaging are part of the work.
- Maintaining external reference games adds effort.
- Some attractive internal experiments may be rejected when they do not improve measurable outcomes.

## How we will prove the decision works

The project will publish and track measures such as:

- installation-to-playable-sample time;
- ordinary edit-to-visible-result time;
- percentage of game features requiring no engine change;
- diagnostic accuracy;
- package reproducibility and rollback success;
- tick stability and resource budgets under stated workloads;
- migration results for representative Robust Toolbox features.

The full quality bar is maintained in `docs/product/quality-bar.md`.
