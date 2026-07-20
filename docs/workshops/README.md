# Design Workshops

Workshop documents capture discussion and unresolved questions. They are informative until their conclusions are recorded in accepted ADRs.

## Standard comparison format

Each top-level question should be discussed in this order:

1. **The question** — stated in plain language.
2. **Why it matters** — the practical effect on developers, players, creators, or operators.
3. **How Robust Toolbox answers it** — strengths, limitations, and lessons.
4. **How the Robusta prototype answers it** — implementation and design intent kept separate.
5. **How the new Robusta should answer it** — the recommended product direction.
6. **Draft decision statement** — short enough to become an ADR summary.
7. **How it will be proved** — user tasks, external games, tests, or operational evidence.
8. **Technical questions deferred** — mechanisms that belong in later ADRs.

## Ground rules

- Remain at product and layman level until the user-visible promise is accepted.
- Do not choose a library, data structure, protocol, or file format before the desired behavior is clear.
- Preserve successful ideas from both predecessors without treating either implementation as automatically correct.
- Record what is implemented separately from what is intended.
- Prefer explicit limits over vague future promises.
- Convert accepted workshop conclusions into ADRs promptly.

## Workshop status

- `2026-07-19-world-model-05-space-persistence-and-preview.md` — accepted ADRs 0030–0038 via Option A for questions 13–23; ADR 0037 amends ADR 0024, the ADR 0033 public-SDK/no-privileged-internals and ADR 0038 collaborative-source-editing qualifications are recorded, implementation remains not started, and questions 24–26 are queued next.

- `2026-07-18-world-model-04-entity-lifecycle-and-simulation-time.md` — accepted ADRs 0015 and 0016: atomic entity lifecycle and fixed simulation time separated from host and presentation time.

- `2026-07-18-world-model-01-what-is-a-world.md` — accepted and recorded as ADR 0011: a world is an isolated mutable simulation that may contain multiple maps.
- `2026-07-18-world-model-02-what-belongs-where.md` — accepted and recorded as ADR 0012: immutable game definitions, host and player-session state, and mutable world state have separate owners.
- `2026-07-18-world-model-03-what-is-a-game-object.md` — accepted and recorded as ADR 0013: entities represent independent world participants; constructed grids may be entities while ordinary tile cells and construction layers remain purpose-built data.
