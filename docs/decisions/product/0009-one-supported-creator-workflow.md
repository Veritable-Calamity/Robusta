# ADR 0009: Provide One Supported Creator Development Workflow

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-18
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Related decisions:** ADR 0001, ADR 0002, ADR 0003, ADR 0005, ADR 0006

## The question

What should everyday game development feel like?

## The promise

A developer can build, validate, run, inspect, change, and test a game through one documented workflow. Every observed change has an explicit result.

## Why this matters

Robust Toolbox and Space Station 14 provide powerful in-game consoles, inspectors, spawning tools, overlays, and debugging commands. Their broader workflow also assumes familiarity with a substantial repository, scripts, and client/server process setup.

The Robusta prototype's creator-loop design proposes a single `robusta dev` command that builds code, validates content, starts a server and clients, watches changes, reloads safe changes, restarts when needed, reconnects, and reports what happened. That product idea should be preserved and shipped early.

## Options considered

### Document a collection of build and launch commands

This is easy for the platform team but pushes process management, configuration, and error interpretation onto every game team.

### Promise universal hot reload

This sounds ideal but arbitrary component-layout, network-schema, and persistence changes cannot always preserve a live world safely.

### Provide supervised development with an honest reload matrix

Safe changes reload. Other changes rebuild and restart automatically. Every outcome is visible and uses the same validation as release packaging.

## Decision

Robusta provides one repository-supported creator workflow centered on a stable `robusta dev` command.

The workflow will:

- discover the selected game workspace;
- restore and build code;
- generate metadata and compile content;
- start a supervised server and configurable clients;
- stream structured, tagged logs and diagnostics;
- watch declared source inputs;
- classify changes through an explicit reload matrix;
- apply safe reloads transactionally;
- rebuild or restart affected processes when required;
- reconnect clients where practical;
- shut down the full process tree cleanly.

Every change produces one of these visible outcomes:

- reloaded;
- rebuilt;
- restarted;
- reconnecting/reconnected;
- rejected with a diagnostic;
- ignored for an explicit reason.

The development path uses the same semantic validation as tests and release packaging. Development-only powers are absent or safely disabled in release artifacts.

## What we deliberately will not do

- Claim that every arbitrary C# or data-schema change can preserve the live world.
- Silently ignore file changes.
- Create a permissive development parser that disagrees with release behavior.
- Leave orphaned server or client processes after failure.
- Require creators to manually coordinate ordinary client/server restarts.

## Consequences

### Benefits

- Onboarding and daily iteration become predictable.
- Restart boundaries are honest and testable.
- Diagnostics are consistent from the editor to release packaging.
- Creator tooling becomes a first-class product rather than an afterthought.

### Costs and limitations

- Cross-platform process supervision and file watching require significant engineering.
- Some changes will interrupt the session.
- Reconnection and development session restoration need stable contracts.

## How we will prove the decision works

An end-to-end test must:

1. start a server and two clients;
2. apply a resource change and a compatible prototype change;
3. apply a component-layout or network-schema change requiring restart;
4. return both clients to a playable session;
5. report every transition clearly;
6. leave no orphaned processes after success or failure.

A new developer must be able to reach the same workflow from a clean machine using the documented installation and template path.
