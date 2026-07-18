# ADR 0007: Separate Trusted Executable Games from Public UGC

- **Decision status:** Accepted
- **Implementation status:** Not started
- **Date:** 2026-07-18
- **Decision level:** Product
- **Owners:** Robusta maintainers
- **Related decisions:** ADR 0001, ADR 0004, ADR 0005

## The question

What kinds of creator content do we trust, and what powers should each kind receive?

## The promise

Players and operators can understand when they are installing executable software, while public add-ons can extend games without silently gaining general access to the computer.

## Why this matters

“Mod” and “UGC” can describe very different things: a full game assembly, a server operator plugin, a map, a texture pack, or a small script. Treating all of these as one trust category either cripples full game development or grants untrusted public material far too much power.

Robust Toolbox inspects downloaded assemblies against a sandbox policy before loading them. The Robusta prototype's accepted trust design makes a more explicit distinction: assembly loading, static scanning, signatures, and provenance improve policy and accountability but do not confine hostile full-power code.

## Options considered

### Data-only creation for everyone

This is easier to secure but prevents independent teams from building complete games in normal C# and pushes behavior into engine changes.

### Let every server or workshop item provide in-process code

This maximizes flexibility but makes joining or browsing equivalent to executing arbitrary code with the process's permissions.

### Use distinct trust and capability tiers

Full games retain ordinary programming power when intentionally installed. Public content begins with validated data and declarative behavior. Later scripting requires genuine isolation and explicit capabilities.

## Decision

Robusta defines these creator and execution categories:

1. **Platform code** — trusted Robusta runtime and tools.
2. **Full game package** — intentionally installed executable game software with shared, client, and server code.
3. **Operator extension** — executable functionality intentionally installed by a server operator, normally server-side.
4. **Public add-on / UGC** — untrusted community content limited to game-approved data and declarative capabilities.
5. **Isolated script** — a possible future category running behind a true isolation boundary with explicit host capabilities and resource budgets.
6. **Editor extension** — trusted development-time code that is not automatically included in player releases.

A publisher signature establishes identity and integrity. It does not prove that executable code is harmless.

The launcher, updater, credentials, and package management authority remain outside game processes. Trust decisions must be visible, understandable, and revocable.

## What we deliberately will not do

- Describe an assembly load context, static scanner, or signature as a security sandbox.
- Let public add-ons access arbitrary files, processes, networks, credentials, or native code.
- Make full game developers express all gameplay as data merely to simplify public UGC safety.
- Allow editor extensions to appear in release packages by accident.
- Hide executable-code consent behind vague “content download” language.

## Consequences

### Benefits

- Full game development remains powerful.
- Public creator safety has an honest foundation.
- Player and operator consent is meaningful.
- Future scripting can be added without pretending in-process code is confined.

### Costs and limitations

- Public creators initially receive fewer capabilities than full game teams.
- Package metadata and user interfaces must communicate trust clearly.
- Process isolation and capability enforcement add platform work.

## How we will prove the decision works

- The launcher process never loads managed game assemblies.
- Public add-on fixtures cannot read undeclared files, open arbitrary network connections, or start processes.
- A full game installation is clearly presented as executable software.
- Operator extensions are excluded from client packages unless separately declared and trusted.
- Revoking a publisher or package prevents future launches without corrupting other installations.
