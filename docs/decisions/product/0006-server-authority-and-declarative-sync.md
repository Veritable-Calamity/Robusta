# ADR 0006: Use Server Authority with Declarative Synchronization Intent

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-18
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Related decisions:** ADR 0003, ADR 0004, ADR 0008

## The question

Who decides what is true in multiplayer, and how much networking machinery should ordinary game developers have to write?

## The promise

The server is the final referee, clients remain responsive through controlled prediction and smoothing, and ordinary synchronized game state does not require custom packet code.

## Why this matters

Robust Toolbox uses a server-authoritative model and supports client prediction so controls do not wait for a full network round trip. That model is proven but can be difficult when game developers must reason about low-level state transfer, dirty tracking, and correction details.

The Robusta prototype also chooses server authority and already explores bounded transport lanes, schema fingerprints, input sequencing, snapshots, interest, interpolation, and limited prediction. The greenfield platform must generalize those ideas into a stable game-facing contract.

## Options considered

### Let each client own its local view of truth

This can feel responsive but creates cheating, disagreement, and reconciliation problems for shared simulation.

### Keep the server authoritative but require hand-written networking

This is flexible but repetitive, error-prone, and difficult to keep compatible across packages and versions.

### Server authority plus declared synchronization intent

Game developers classify state and actions; generated schemas and runtime services handle routine transfer, prediction, correction, and visibility.

## Decision

The server owns authoritative multiplayer game state.

Clients may make bounded temporary predictions and visually smooth remote state. The server may confirm or correct those guesses.

The Game SDK will let developers express understandable synchronization intent, including categories such as:

- server-only;
- shared authoritative state;
- locally predicted state;
- remotely interpolated state;
- client-only cosmetic state.

Robusta will generate or supply routine:

- stable network identities and codecs;
- full and changed state transfer;
- dirty tracking;
- entity creation and removal;
- ownership and input sequencing;
- interest and visibility handling;
- prediction history and correction;
- reconnection and resynchronization;
- entity-bound UI messages;
- compatibility checks before joining.

Single-player and offline play use the same game rules. The platform may host an invisible local authority, but the user experience remains one-click play.

## What we deliberately will not do

- Let clients authoritatively decide shared gameplay outcomes.
- Require packet encoders for normal replicated components.
- Expose transport lanes and sockets as the ordinary game API.
- Promise that every game action is automatically safe to predict.
- Maintain separate, divergent game rules for offline and multiplayer modes.

## Consequences

### Benefits

- Security and shared-world consistency are clearer.
- Routine multiplayer code becomes declarative and generated.
- Schema compatibility can be checked before gameplay begins.
- Offline and online behavior remain aligned.

### Costs and limitations

- Prediction requires strict behavioral rules and excellent diagnostics.
- Some systems remain server-only or require explicit manual design.
- Network compatibility becomes a formal release concern.

## How we will prove the decision works

A two-client external game must demonstrate:

- predicted local movement;
- smoothed remote movement;
- authoritative interaction with a shared object;
- entity entering and leaving interest;
- reconnect and resynchronization;
- clear rejection for incompatible package or schema;
- stable behavior under simulated latency, loss, duplication, and reordering.
