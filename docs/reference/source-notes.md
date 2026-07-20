# Source Notes

**Snapshot date:** 2026-07-19

These sources informed the foundational product decisions. They are references and historical inputs, not a claim that either predecessor already fulfills the new constitution.

## Project mission

- Robusta project note: a lessons-learned successor to Robust Toolbox with stronger packaging of user-created content into distinct games.
- Robusta repository: <https://github.com/Veritable-Calamity/Robusta>
- Robust Toolbox repository: <https://github.com/space-wizards/RobustToolbox>
- Space Station 14 game repository: <https://github.com/space-wizards/space-station-14>

## Robusta prototype branch

The `Robusta-UGC` branch was treated as the current prototype baseline during the workshop. The coherence audit pins the inspected state to commit `61c71c068202c61575e48d6587ba53f300bed69b`; branch names alone are not reproducible migration evidence:

- <https://github.com/Veritable-Calamity/Robusta/tree/61c71c068202c61575e48d6587ba53f300bed69b>

At the snapshot date, the repository described itself as an experimental modular .NET game engine and contained side-specific Game SDK projects, thin client/server hosts, source generation, validation, tests, networking work, sample content, and eleven architecture decisions.

## Prototype-era decisions carried forward as lessons

- Isolated engine hosts:  
  <https://github.com/Veritable-Calamity/Robusta/blob/61c71c068202c61575e48d6587ba53f300bed69b/docs/architecture/decisions/0004-isolated-engine-hosts.md>
- Server-authoritative network session:  
  <https://github.com/Veritable-Calamity/Robusta/blob/61c71c068202c61575e48d6587ba53f300bed69b/docs/architecture/decisions/0005-server-authoritative-network-session.md>
- Content trust and execution model:  
  <https://github.com/Veritable-Calamity/Robusta/blob/61c71c068202c61575e48d6587ba53f300bed69b/docs/architecture/decisions/0006-content-trust-and-execution-model.md>
- Public content API boundary:  
  <https://github.com/Veritable-Calamity/Robusta/blob/61c71c068202c61575e48d6587ba53f300bed69b/docs/architecture/decisions/0007-public-content-api-boundary.md>
- Content versioning and package contract:  
  <https://github.com/Veritable-Calamity/Robusta/blob/61c71c068202c61575e48d6587ba53f300bed69b/docs/architecture/decisions/0008-content-versioning-and-package-contract.md>
- Package-aware prototypes and resources:  
  <https://github.com/Veritable-Calamity/Robusta/blob/61c71c068202c61575e48d6587ba53f300bed69b/docs/architecture/decisions/0009-package-aware-prototypes-and-resources.md>
- Creator development loop:  
  <https://github.com/Veritable-Calamity/Robusta/blob/61c71c068202c61575e48d6587ba53f300bed69b/docs/architecture/decisions/0010-creator-development-loop.md>
- Validation and untrusted scripting boundary:  
  <https://github.com/Veritable-Calamity/Robusta/blob/61c71c068202c61575e48d6587ba53f300bed69b/docs/architecture/decisions/0011-validation-and-untrusted-scripting-boundary.md>

## Robust Toolbox lesson source

The Robust Toolbox repository describes itself as the engine portion of Space Station 14, primarily developed for that game, and directs actual development through the content repository. This is important evidence for both the productivity of a real game-driven engine and the coupling that a general platform must manage deliberately.

- <https://github.com/space-wizards/RobustToolbox/blob/537c4cb02f9555fa18f489e7b05694d288887d0e/README.md>

## Use of sources

The new product ADRs deliberately distinguish:

- behavior proven by Robust Toolbox and Space Station 14;
- code currently present in the Robusta prototype;
- design intent documented by the Robusta prototype;
- the newly accepted direction for a release-grade greenfield platform.

## World and object-model sources

The world-model workshops also used the following primary references:

- Robust Toolbox entity identity:  
  <https://github.com/space-wizards/RobustToolbox/blob/537c4cb02f9555fa18f489e7b05694d288887d0e/Robust.Shared/GameObjects/EntityUid.cs>
- Robust Toolbox ECS guide:  
  <https://docs.spacestation14.com/en/robust-toolbox/ecs.html>
- Robust Toolbox entity inspection and examples:  
  <https://docs.spacestation14.com/en/community/admin/admin-tooling.html#view-variables>
- Robust Toolbox grids and compact tile storage:  
  <https://docs.spacestation14.com/en/robust-toolbox/transform/grids.html>
- Robust Toolbox grid component:  
  <https://github.com/space-wizards/RobustToolbox/blob/537c4cb02f9555fa18f489e7b05694d288887d0e/Robust.Shared/Map/Components/MapGridComponent.cs>
- Robust Toolbox chunked tile and anchored-entity storage:  
  <https://github.com/space-wizards/RobustToolbox/blob/537c4cb02f9555fa18f489e7b05694d288887d0e/Robust.Shared/Map/MapChunk.cs>
- Robust Toolbox tile value:  
  <https://github.com/space-wizards/RobustToolbox/blob/537c4cb02f9555fa18f489e7b05694d288887d0e/Robust.Shared/Map/Tile.cs>
- Space Station 14 tile definitions and base-turf rules:  
  <https://github.com/space-wizards/space-station-14/blob/b587d28e41ec33ffda6c1cac32e138e136d232ef/Content.Shared/Maps/ContentTileDefinition.cs>
- Space Station 14 lattice and plating prototypes:  
  <https://github.com/space-wizards/space-station-14/blob/b587d28e41ec33ffda6c1cac32e138e136d232ef/Resources/Prototypes/Tiles/plating.yml>
- Space Station 14 floor prototypes:  
  <https://github.com/space-wizards/space-station-14/blob/b587d28e41ec33ffda6c1cac32e138e136d232ef/Resources/Prototypes/Tiles/floors.yml>
- Space Station 14 floor placement and grid creation:  
  <https://github.com/space-wizards/space-station-14/blob/b587d28e41ec33ffda6c1cac32e138e136d232ef/Content.Shared/Tiles/FloorTileSystem.cs>
- Space Station 14 tile replacement, history, and deconstruction:  
  <https://github.com/space-wizards/space-station-14/blob/b587d28e41ec33ffda6c1cac32e138e136d232ef/Content.Shared/Maps/TileSystem.cs>
- Robust Toolbox local and network entity distinction:  
  <https://docs.spacestation14.com/en/robust-toolbox/netcode/net-entities.html>
- Robusta prototype Game SDK world facade:  
  <https://github.com/Veritable-Calamity/Robusta/blob/61c71c068202c61575e48d6587ba53f300bed69b/Robusta.Game.Shared/GameWorld.cs>
- Robusta prototype entity manager:  
  <https://github.com/Veritable-Calamity/Robusta/blob/61c71c068202c61575e48d6587ba53f300bed69b/Robusta.Shared/ECS/EntityManager.cs>
