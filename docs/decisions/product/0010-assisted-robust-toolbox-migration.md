# ADR 0010: Target Assisted Robust Toolbox Migration Rather than Binary Compatibility

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-18
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Related decisions:** ADR 0002, ADR 0003, ADR 0005, ADR 0006, ADR 0008

## The question

How compatible should the new Robusta be with Robust Toolbox and existing games such as Space Station 14?

## The promise

A Robust Toolbox developer encounters familiar concepts, receives meaningful automated help, and can see a finite report of what transfers exactly, what is converted, and what requires redesign.

## Why this matters

Robust Toolbox has a proven authoring model and a large body of content. Ignoring it would discard valuable lessons and make adoption unnecessarily painful.

However, loading existing compiled assemblies unchanged would require Robusta to reproduce Robust Toolbox type identities, service access, lifecycle, event behavior, serialization, networking, maps, physics, UI, and many accidental implementation assumptions. That would make the rewrite an alternate implementation of Robust Toolbox rather than a new platform able to improve those contracts.

## Options considered

### No migration support

This preserves total freedom but discards a large potential creator community and prevents real legacy content from testing the new abstractions.

### Full binary compatibility

This minimizes source edits in theory but freezes many internals and makes correctness dependent on reproducing undocumented behavior.

### Familiar concepts plus source- and data-assisted migration

The native API remains clean. Dedicated tools convert common patterns and classify unsupported behavior explicitly.

## Decision

Robusta targets conceptual familiarity, automated content conversion, compiler-assisted source migration, and explicit manual redesign points. It does not target binary compatibility with Robust Toolbox assemblies.

The platform will provide a migration product containing:

- a usage census over Robust Toolbox game source and content;
- importers for prototypes, resources, maps, localization, configuration, and selected UI data;
- Roslyn analyzers and code fixes for common component, system, event, dependency, serialization, and networking patterns;
- a temporary compatibility package implemented above the native Game SDK;
- conformance scenarios that compare important observable behavior;
- a report classifying every migrated item as `Exact`, `Renamed`, `Converted with warning`, `Manual port`, or `Unsupported`.

Familiar names and concepts should be retained where they remain good. Harmful or accidental legacy behavior is not automatically preserved.

## What we deliberately will not do

- Load existing Space Station 14 content assemblies unchanged as the primary migration strategy.
- Hide semantic differences behind source-compatible names.
- Make the native Game SDK a permanent clone of Robust Toolbox internals.
- Silently approximate prototype inheritance, lifecycle ordering, prediction, maps, or saved data.
- Delay all platform releases until full Space Station 14 parity exists.

## Consequences

### Benefits

- Migration work is measurable and tool-supported.
- The new platform retains freedom to improve public contracts.
- Real legacy features continuously test the Game SDK.
- Developers receive honest diagnostics instead of runtime surprises.

### Costs and limitations

- A substantial game still requires a source port.
- Deeply engine-integrated systems require manual redesign.
- Maintaining importers and transitional APIs is ongoing product work.

## How we will prove the decision works

A representative migration corpus must include:

- a simple data component;
- an event-driven interactive entity;
- predicted movement;
- a networked inventory item;
- a prototype-heavy entity family;
- entity-bound UI;
- containers and transforms;
- map/grid interaction;
- physics collision;
- localization and appearance;
- server-only administration;
- saved-data migration.

For each scenario, the migration report must state the automation level and resulting behavior. Compile success alone is not sufficient; observable conformance must be tested.
