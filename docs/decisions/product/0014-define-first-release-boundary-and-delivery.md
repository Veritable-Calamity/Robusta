# ADR 0014: Define the First-Release Boundary and Delivery Responsibilities

- **Decision status:** Accepted
- **Implementation status:** In progress
- **Date:** 2026-07-18
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Related decisions:** ADR 0000, ADR 0001, ADR 0002, ADR 0004, ADR 0008, ADR 0009

## The question

What exactly must Robusta 1.0 ship, which machines are supported, and where do discovery, installation, verification, and launch responsibilities live?

## The promise

An independent team can develop and operate a 2D desktop multiplayer game on the documented Windows and Linux baseline using published artifacts, and players can install and start an exact verified game release without cloning Robusta.

## Decision

Robusta 1.0 supports Windows 11 x64 (24H2 or later) and Ubuntu 24.04 LTS x64 for creator tools, desktop clients, and dedicated servers. Headless Linux containers built from the Ubuntu baseline are a supported server distribution. Other desktop Linux distributions may work but are not support claims for 1.0.

The required feature boundary is the complete journey already named by the constitution: published Game SDK packages; 2D client and dedicated-server runtimes; deterministic content compilation; server-authoritative multiplayer; creator CLI; application packaging, verification, installation, update, exact rollback, and diagnostics; dedicated-server operation; capability-limited declarative public add-ons; and assisted Robust Toolbox migration. Offline play uses a local authority. Both external reference games and all M8 clean-machine journeys are release requirements.

Robusta 1.0 does not promise 3D rendering, mobile or console clients, arbitrary public scripting, a centralized marketplace, hosted accounts or servers, full Space Station 14 parity, binary Robust Toolbox compatibility, or live preservation of arbitrary worlds across code or schema changes.

SDK and creator-tool artifacts are distributed by a NuGet-compatible registry. Runtime and game application bundles are distributed as immutable signed release artifacts through publisher-selected HTTPS origins. A publisher may expose metadata through any compatible registry; Robusta does not require one centralized commercial service.

The package registry stores and serves immutable artifacts and discovery metadata. The launcher resolves an exact receipt, downloads, verifies identity and hashes, installs side by side, selects a compatible runtime, starts the game, and performs update or rollback transactions. It never loads managed game assemblies and is not itself a package registry. Publisher credentials and publication remain outside the game runtime.

## What we deliberately will not do

- Describe unlisted operating systems or architectures as supported.
- Make a marketplace or hosted service a condition of creating or distributing a game.
- Let the launcher execute game code while inspecting or installing a package.
- Treat a mutable channel name as an exact release identity.

## How we will prove the decision works

- Clean Windows and Ubuntu CI jobs build, test, pack, and restore an external consumer from the generated feed.
- M2 proves the creator journey on both supported operating systems.
- M5 proves immutable installation, process separation, verification, update, rollback, and server operation.
- M8 publishes the complete support scorecard and known limitations.

