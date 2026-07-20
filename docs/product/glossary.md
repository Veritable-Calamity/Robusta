# Plain-Language Glossary

This glossary defines product terms without relying on engine implementation details.

## Add-on

A separately packaged addition to a game. A public add-on receives only capabilities explicitly exposed by that game. It is not assumed to contain unrestricted executable code.

## Avatar

The game object currently controlled or represented by a player session inside a particular world. A player session may survive after its avatar is removed or replaced.

## Component

A piece of information attached to a game object. Examples: “has health,” “is a door,” “can be picked up,” or “is currently open.”

## Content catalog

The checked, resolved description of a game's prototypes, resources, maps, localization, and related data after the content compiler has processed the human-authored source files.

## Content catalog generation

One immutable, exact version of a content catalog supplied to running worlds. Development changes create a new generation that a world adopts through an explicit reload, migration, restart, or rejection outcome.

## Catalog adoption

The explicit act of making a content catalog generation govern future births in a running world. Existing live state remains unchanged unless a declared, prepared migration commits; incompatible changes require restart.

## Content compiler

The tool that reads authoring files, resolves references and inheritance, validates rules, separates client and server material, and produces the content catalog used by released games.

## Collaborative mapping session

An authenticated creator session in which a designated creator authority orders revisioned commands against one canonical map document. Its live map view is a derived editing projection; ordinary gameplay mutations are not automatically written into map source.

## Durable identity

An opaque, scoped identity granted only when a game or platform contract promises continuity across checkpoints, reconstruction, or transfer. It is distinct from a runtime entity handle, network identity, package identity, and credential.

## Durable service

An explicitly named service whose state is intended to outlive one world or be shared across several worlds, such as an account, campaign, or persistent economy service. It must not expose one world's private objects to another.

## Entity / game object

An independent participant in one running world with its own identity and lifecycle. Components describe its state and capabilities. Position, rendering, networking, saving, and prototype origin are optional rather than part of the minimum meaning of an entity.

## Grid

A spatial structure containing addressed cells and anchored objects. A grid may be an entity when it needs identity, movement, physics, lifecycle, or whole-grid tooling. Its ordinary cells do not therefore need separate entity identities.

## Tile / grid cell

The installed state at one address within a grid or tilemap. Robust Toolbox currently stores one active tile value per coordinate and uses base-turf rules plus optional history for construction and deconstruction; a future Robusta map design may instead expose explicit logical layers or a hybrid. A cell can support construction, collision, saving, networking, inspection, and editing without being a separate general-purpose entity. Loose construction material may be an entity before placement or after deconstruction.

## Event

A notification that something happened or is about to happen, such as an object being used, damaged, moved, created, or removed.

## Exact release receipt / lock

The record of the precise runtime, packages, versions, hashes, and schemas used to build and run a published game release.

## Full game package

An intentionally installed application package containing game code and content. It is treated as executable software, not as harmless data.

## Game SDK

The supported set of contracts, build tools, analyzers, generators, and documentation used by game projects. It is the normal front door into Robusta.

## Host

A client or server program that combines a Robusta runtime with one exact game package and runs it.

## Map definition

The immutable, package-qualified compiled form of a map source. One definition can be instantiated as several independent runtime maps.

## Map source

The readable canonical document authored by creators. It records declared map content and relations, not an arbitrary snapshot of a running gameplay world.

## Operator extension

Executable functionality intentionally installed by a server operator, normally for that server. It is not automatically trusted by players or other servers.

## Package

A versioned, identifiable collection of code, content, metadata, dependencies, and verification information.

## Player session

The authenticated player connection and host-level state that may continue while worlds or avatars are replaced. World membership and the current avatar remain world-specific.

## Prediction

A temporary client-side guess used to make controls feel immediate before the authoritative server result arrives.

## Prototype

A reusable recipe for creating a kind of game object. A prototype can describe which components an object has and the starting values of those components.

## Public UGC

Player- or community-created material that is not automatically trusted as executable software. Initially this means validated data and declarative behavior through game-approved extension points.

## Runtime

The released Robusta software that executes a game. A game release records the exact compatible runtime it uses.

## Runtime map

One world-owned live instantiation of a map definition, with its own world-local identity, root spatial frame, mutable spatial state, and lifecycle.

## Server authority

The rule that the server is the final source of truth for multiplayer game state, even when clients temporarily predict outcomes.

## System

Game rules or behavior that operate on components and events. Examples: opening doors, applying damage, processing movement, or advancing a round.

## Scope / lifetime

The part of the running platform that owns some state and decides when that state is created and destroyed. Examples include the game installation, host, player session, and world.

## Spatial frame

The named reference that gives a position or direction spatial meaning. Conversion between frames is explicit and may fail when frames are disconnected, stale, unavailable, or from different worlds.

## Typed relation

An explicitly classified connection between objects. Spatial parentage, logical containment, physical attachment or anchoring, lifecycle ownership, and non-owning references remain separate and declare their own mutation, visibility, transfer, and ending behavior.

## World checkpoint

A versioned durable representation of one declared committed world boundary, together with the identities, schemas, compatibility information, and migration data required to validate and reconstruct it atomically.

## World lineage

The declared continuity domain connecting reconstructed versions of a world where the product promises that continuity. It does not make runtime entity handles portable between world instances.

## World

One isolated mutable simulation. A world owns its game objects, world-specific systems, simulation time, maps, spatial and physics state, timers, events, and other mutable state that advances with that simulation. A world may contain several maps.
