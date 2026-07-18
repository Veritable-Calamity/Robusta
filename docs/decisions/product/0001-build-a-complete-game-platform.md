# ADR 0001: Build a Complete Game Platform for Independent Teams

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-18
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Related decisions:** ADR 0000, ADR 0003, ADR 0004, ADR 0009

## The question

Are we building an engine library, or a complete product that people can use to create and operate games? Who is the primary customer?

## The promise

An independent game team can install Robusta, create a game, develop it, package it, distribute it, run clients and servers, and operate releases without maintaining an engine fork.

## Why this matters

Robust Toolbox is an effective engine for Space Station 14 and directs developers to work in the game's content repository, including when changing the engine. That close relationship gives the project a real customer and productive tooling, but it also makes the wider engine-and-game ecosystem part of the practical product.

The Robusta prototype describes itself as an experimental modular engine, while its later decisions reach toward a Game SDK, packages, a launcher, a creator command, validation, and trust boundaries. Calling the result only an engine would leave important user journeys outside the product definition.

## Options considered

### Build reusable engine libraries only

This narrows scope, but every game must assemble its own creator workflow, packaging, launcher, update behavior, server tooling, and compatibility story. The user experience becomes an accident of each game.

### Build an engine plus optional examples

This helps onboarding, but still does not make installation, upgrades, package verification, or operations supported contracts.

### Build a complete game platform

The engine remains an internal foundation, while the supported product also includes the Game SDK, runtimes, content compiler, CLI, package and launch workflow, testing support, diagnostics, and server operation.

## Decision

Robusta is a complete game platform designed first for independent game developers and game teams.

The platform includes, as supported products:

- a shared/client/server Game SDK;
- client and dedicated-server runtimes;
- a content compiler;
- creator CLI and development orchestration;
- package verification, installation, and launch behavior;
- testing and diagnostics support;
- dedicated-server release and operation support;
- migration tooling for Robust Toolbox users.

Player and operator safety remain non-negotiable. Public add-on creation is supported through a lower-trust capability model rather than by granting every creator full application permissions.

## What we deliberately will not do

- Define success as a set of libraries that only engine contributors can assemble.
- Require a game team to clone or edit Robusta for ordinary gameplay work.
- Make a marketplace, hosted cloud service, or commercial ecosystem a requirement for the first platform release.
- Optimize solely for Space Station 14 at the expense of unrelated games.

## Consequences

### Benefits

- User journeys have clear owners and release gates.
- Packaging, installation, operations, and upgrades are designed early rather than added as afterthoughts.
- Independent games become normal consumers instead of repository fixtures.

### Costs and limitations

- The product is broader than an engine and requires disciplined scope.
- CLI, packaging, launcher, server operations, and documentation require sustained ownership.
- The first release must deliberately exclude non-essential platform ambitions.

## How we will prove the decision works

- A clean machine can install tools and create a game without cloning Robusta.
- An external game publishes a client and dedicated server from documented commands.
- A second unrelated game uses the same public platform.
- Server operators can install, configure, observe, update, and roll back a release without engine source knowledge.

## Follow-up decisions

- Define the exact 1.0 feature boundary.
- Define supported operating systems and distribution channels.
- Define launcher and package-registry responsibilities.
