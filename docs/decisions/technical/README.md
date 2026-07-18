# Technical ADRs

No greenfield technical ADRs have been accepted yet.

Technical ADRs will select mechanisms that fulfill the accepted product decisions. They should remain more specific and more replaceable than the product ADRs.

Every technical ADR must include:

- the product ADRs it serves;
- the user-visible behavior it must preserve;
- alternatives considered;
- measurable acceptance evidence;
- compatibility, migration, security, and operational consequences;
- an implementation status separate from decision status.

Likely early technical ADR groups include:

1. process and host model;
2. public Game SDK topology;
3. entity and component lifecycle;
4. event model and system scheduling;
5. content intermediate representation and compiler;
6. package manifest, lock, and installation layout;
7. network schema and replication model;
8. renderer, windowing, input, audio, UI, and physics library choices;
9. save/map identity and migration;
10. creator CLI and process supervision.
