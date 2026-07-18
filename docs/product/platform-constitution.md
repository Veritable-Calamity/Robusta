# Robusta Platform Constitution

**Status:** Accepted  
**Accepted:** 2026-07-18  
**Applies to:** the greenfield, release-grade Robusta platform  
**Implementation status:** Not started

## Mission

Robusta is a complete platform for creating, testing, packaging, distributing, running, and operating games.

It carries forward the productive game-authoring ideas proven by Robust Toolbox while applying the lessons learned from both Robust Toolbox and the first Robusta experiment. Its long-term goal is not merely cleaner internals, but a better experience for game teams, players, creators, and server operators.

## Primary creator

Robusta is designed first for independent game developers and game teams.

They must be able to create complete games without maintaining an engine fork or changing Robusta source for ordinary gameplay features.

Player and operator safety remain non-negotiable. Full game development, operator-installed extensions, and public user-created add-ons are separate capability levels.

## Core promises

1. **Robusta is a platform, not only a library.**  
   The supported product includes the Game SDK, client and server runtimes, content compiler, creator tools, packaging, installation, diagnostics, testing support, and server operation.

2. **Ordinary game work stays in the game.**  
   Developers normally create components, systems, events, prototypes, interfaces, maps, artwork, audio, localization, and game-specific services. They do not edit engine startup code, registries, packet encoders, or internal engine modules for normal features.

3. **Games use a supported front door.**  
   Game code depends on the published Game SDK and explicit extension points, not on engine implementation details.

4. **A game is an installable application.**  
   Every published game has its own identity, version, exact dependency receipt, client and server contents, provenance, and writable-data boundary.

5. **Content is readable and checked.**  
   Human-authored content is compiled into a deterministic, validated, package-aware catalog with source-quality diagnostics and inspectable results.

6. **The server is the referee.**  
   Multiplayer game state is authoritative on the server. Clients may make bounded temporary predictions so the game feels responsive.

7. **Routine networking is a platform responsibility.**  
   Game developers declare synchronization intent; Robusta supplies ordinary state transfer, interest, prediction, correction, and reconnection behavior.

8. **Executable games and public add-ons are different.**  
   A full game package is intentionally trusted software. Public add-ons receive only the capabilities that the game explicitly exposes.

9. **Signatures identify; they do not make code safe.**  
   Verification and provenance show who produced a package and whether it changed. They are not described as a sandbox.

10. **Upgrades are explicit and reversible.**  
    Exact working sets are recorded, compatible runtimes may coexist, data migrations are explicit, and rollback is supported.

11. **The development workflow is part of the product.**  
    Robusta provides one supported creator workflow in which every change is reloaded, rebuilt, restarted, rejected, or explicitly ignored with a clear reason.

12. **Quality is demonstrated through outcomes.**  
    Robusta does not claim superiority because of an architectural label. Improvements are demonstrated through creator tasks, external games, reliability, compatibility, safety, measured performance, and operational results.

13. **A feature is not complete until an external game can use it.**  
    Supported API, diagnostics, tests, documentation, tooling, packaging behavior, and upgrade treatment are part of the feature.

14. **Migration is a product, not a slogan.**  
    Robusta supports familiar concepts and assisted transfer from Robust Toolbox, but does not promise unchanged binaries or preservation of harmful legacy behavior.

## How conflicts are resolved

When desirable goals compete, decisions should normally preserve them in this order:

1. Safety, data integrity, and honest trust boundaries.
2. Published compatibility promises and deterministic behavior.
3. Clear creator, player, and operator outcomes.
4. Reliable diagnostics and recoverability.
5. Developer convenience and iteration speed.
6. Measured performance and resource efficiency.
7. Internal elegance.

This order does not make performance or elegance unimportant. It prevents them from being used to justify unsafe, incompatible, or unusable behavior without evidence.

## Early product limits

The first release does not need to include every possible platform feature. It may deliberately exclude 3D rendering, mobile and console support, arbitrary public scripting, a centralized marketplace, full Space Station 14 parity, and live preservation of every world across arbitrary code changes.

Those exclusions are acceptable only when they are documented honestly and do not contradict the promises above.

## Proof of adherence

The constitution is considered demonstrated only when:

- an independent game can be created from published tools without cloning the engine;
- a second, substantially different game can use the same public contracts;
- packages can be installed, verified, updated, and rolled back;
- a dedicated server can be operated from documented release artifacts;
- public add-ons are prevented from receiving undeclared capabilities;
- migration tooling reports what transfers automatically and what requires manual work;
- released behavior is backed by conformance tests and user-visible diagnostics.
