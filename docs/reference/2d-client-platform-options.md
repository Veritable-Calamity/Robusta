# 2D client and platform options

- **Status:** Research note; not a decision or adoption
- **Assessment date:** 2026-07-19
- **Target profiles:** Windows 11 x64 24H2 or later, Ubuntu 24.04 LTS x64, and Ubuntu-derived headless Linux containers
- **Scope:** Windowing, input, 2D rendering, audio, game UI and accessibility, and 2D collision/physics

## Purpose and governing constraints

This note identifies plausible implementation approaches and evidence work for Robusta's 2D client and physics floor. It does not select a library, create an ADR, authorize a dependency, or claim implementation.

The assessment is constrained by these accepted decisions:

- [ADR 0001](../decisions/product/0001-build-a-complete-game-platform.md) requires a complete platform usable by independent teams, not a set of engine parts that every game must assemble.
- [ADR 0003](../decisions/product/0003-preserve-straightforward-game-authoring.md) keeps low-level renderer, host, and transport types out of ordinary game code. UI, appearance, and audio must remain natural Game SDK concepts.
- [ADR 0014](../decisions/product/0014-define-first-release-boundary-and-delivery.md) fixes the supported desktop and server operating-system profiles and requires published, exact, installable artifacts.
- [ADR 0026](../decisions/product/0026-define-supported-game-code-conformance-and-fault-containment.md) prevents a native crash, blocked platform thread, or unsafe callback from being described as a safely world-local failure.
- [ADR 0033](../decisions/product/0033-provide-platform-mechanics-with-game-defined-semantics.md) requires cameras, sprites, input, UI composition, audio, localization, accessibility-relevant extension points, spatial queries, and basic physics as neutral platform capabilities. Unadmitted capabilities must impose no initialization, update, thread, native-service, or payload cost.
- [ADR 0034](../decisions/product/0034-use-a-declared-ladder-for-advanced-game-extensions.md) requires unusual render, audio, device, or physics integrations to use a declared adapter contract rather than private runtime access.

The current repository contains client, server, shared-runtime, and Game SDK project scaffolds, but no client-platform or physics implementation. There is therefore no compatibility reason to expose a candidate library's public types.

## Executive assessment

There is a credible technical route, but the architecture boundary should be accepted before a library is selected.

For later evidence bakeoffs, the strongest client-platform candidates are:

1. a direct SDL3 stack using SDL window/input, SDL_GPU or SDL's 2D renderer, and SDL audio;
2. a modular Silk.NET stack using its window/input utilities plus an explicitly chosen graphics and audio API; and
3. MonoGame as a mature, integrated 2D-framework comparator.

For UI, a Robusta-owned semantic retained-mode UI rendered by the selected backend should be compared with an Avalonia-composed client shell. Avalonia is also a plausible tooling UI regardless of the game-client outcome. Neither approach is selected here.

For physics, native Box2D 3, Box2D.NET, and the maintained Aether.Physics2D fork merit controlled comparison. Native Box2D has the strongest upstream engine and current determinism claims; the managed candidates avoid an unmanaged fault and distribution boundary but need independent parity, maintenance, and determinism evidence.

No candidate is ready for adoption without Windows and Ubuntu evidence, exact native packaging, lifecycle and affinity tests, accessibility proof, SDK-leakage checks, and representative station-like and contrasting-game measurements.

## Recommended architecture boundary

The recommendation in this note is a boundary, not a backend:

```text
Published Game SDK
  cameras, sprites, draw descriptions, input actions, UI semantics, audio intents,
  collision/physics capabilities, identity-free values, diagnostics
                           |
Internal platform services
  presentation snapshots, render preparation, UI layout/focus/accessibility,
  resource lifetime, audio mixing policy, physics command/event translation
                           |
Replaceable implementation adapters
  window/input | graphics | audio | accessibility bridge | physics
                           |
Candidate managed and native libraries
```

The boundary should enforce the following rules:

1. **No backend types in the Game SDK.** SDL handles, Silk interfaces, MonoGame `GraphicsDevice`/`SpriteBatch` values, Box2D IDs, native pointers, and backend enums remain runtime implementation details. Game-facing values use Robusta identities, units, resource handles, actions, and immutable descriptions.
2. **Presentation consumes committed state.** The client prepares render and audio work from immutable confirmed or explicitly predicted presentation snapshots. Platform callbacks never advance authoritative simulation.
3. **The platform lane owns window and graphics state.** Event pumping, text input, swapchain/device work, and backend calls with main-thread or creation-thread requirements execute on a named platform affinity lane. SDL, for example, requires event pumping and text-input activation on the main thread, and some GPU swapchain operations on the window-creating thread ([event pumping](https://wiki.libsdl.org/SDL3/SDL_PumpEvents), [text input](https://wiki.libsdl.org/SDL3/SDL_StartTextInput), [GPU swapchain wait](https://wiki.libsdl.org/SDL3/SDL_WaitForGPUSwapchain)).
4. **Native resources have explicit owners.** Window, graphics device, swapchain, texture, buffer, audio device/stream, and physics world resources have typed owners and deterministic close order. Finalizers may report leaks but are not the correctness path. Native callbacks cannot allow exceptions to cross the ABI.
5. **Audio callbacks are isolated from gameplay.** The audio adapter consumes bounded prepared buffers or commands. It does not query mutable world state, take gameplay locks, or publish authoritative events from an audio thread.
6. **Physics is a world capability, not a client-library feature.** An admitted world owns its authoritative physics adapter and fixed-step invocation. Dedicated servers may carry physics without carrying window, renderer, input, UI, or audio code. Client physics, if admitted, is a separately declared prediction or presentation capability.
7. **UI semantics are independent of pixels.** Layout, focus order, keyboard/gamepad navigation, text editing, localization, accessible name/role/state/value, actions, live-region announcements, scaling, and contrast metadata belong to a backend-neutral semantic tree. A renderer consumes visual output; an accessibility adapter exposes the same semantics to UI Automation on Windows and AT-SPI on Linux.
8. **Input is normalized before game delivery.** Physical keys, scan codes, controllers, pointer/touch input, IME composition, and clipboard operations become device-qualified input records and game-declared actions. Gameplay does not poll a native global input singleton.
9. **Backend versions and native artifacts enter the exact receipt.** The receipt records the implementation family, exact managed packages, native binaries, hashes, licenses/notices, graphics or audio backend, and any compatibility-relevant shader or physics configuration.
10. **Advanced adapters remain narrow.** A trusted game needing a different renderer, device, or physics engine uses the later ADR 0034 adapter contract with declared side, lifetime, affinity, determinism, fault, package, and support consequences. It does not receive the built-in backend's private service graph.

This design allows Robusta to ship one supported backend initially without promising that all internal adapters are public plugins. Replaceability is proven by conformance tests and the absence of backend types in game packages, not by exposing a general service locator.

## Client-platform approaches

### Comparison

| Approach | Windows 11 and Ubuntu 24.04 | Headless omission | Native lifetime and thread affinity | License and distribution | UI and accessibility | Maturity and change risk | SDK abstraction leakage |
|---|---|---|---|---|---|---|---|
| **Direct SDL3** | SDL officially supports Windows and Linux and exposes window, input, controller, audio, render, and GPU subsystems ([SDL3 overview](https://wiki.libsdl.org/SDL3/FrontPage)). Ubuntu needs explicit Wayland and X11 coverage; SDL documents meaningful Wayland behavior differences ([Wayland notes](https://wiki.libsdl.org/SDL3/README-wayland)). | SDL subsystems are individually admitted; leaving off a subsystem prevents SDL from touching it ([initialization contract](https://wiki.libsdl.org/SDL3/CategoryInit)). A dedicated server should not reference or load SDL at all unless a separately admitted server capability needs it. | Window/event/GPU calls include main- or creation-thread restrictions. The C API and callback boundaries require explicit handles, pinning, disposal, and exception containment. | SDL and SDL3-CS use the zlib license. SDL publishes native release artifacts; the listed [SDL3-CS binding](https://github.com/flibitijibibo/SDL3-CS) is generated, function-for-function interop with no published releases or packages and warns that some definitions require manual intervention. Robusta would need a pinned, reviewed binding and native packaging plan. | SDL provides text/IME, clipboard, input, high-DPI, and window primitives, but no retained game UI or operating-system accessibility tree. Robusta supplies both. | SDL3 is released and actively maintained. SDL_GPU is newer than SDL's longstanding platform APIs and must earn device-loss, shader, and driver evidence. Binding maturity is materially lower than SDL itself. | Low if wrapped immediately; extreme if raw SDL enums, pointers, callbacks, or resource rules escape the adapter. |
| **Silk.NET 2.x modular stack** | Silk supplies low-level bindings and cross-platform window/input utilities, with graphics and audio APIs chosen separately ([documentation](https://dotnet.github.io/Silk.NET/docs/)). Windows and Ubuntu behavior still depends on the selected windowing, graphics, and audio backends. | Packages are modular, so client-only assemblies can be excluded. A dependency and module-load audit is still needed because selecting a broad metapackage would defeat clean omission. | Most APIs closely mirror native signatures. Robusta still owns graphics-context affinity, explicit resource lifetime, callback containment, and the audio implementation. | Silk.NET is MIT/X11 and distributed as NuGet packages. Version 2.23 is the current 2.x release, but the project states that 2.x investment is limited while 3.0 is developed ([project status](https://github.com/dotnet/Silk.NET)). Silk 3 windowing documentation currently describes SDL3 as its reference implementation, so 3.x does not automatically remove the SDL dependency ([3.0 preview windowing](https://dotnet.github.io/Silk.NET/docs/v3/for-contributors/Windowing/)). | No game UI or accessibility tree is supplied. OpenAL is a binding, not a complete Robusta audio policy or resource pipeline. | Established bindings and .NET Foundation governance, but a significant 2.x/3.x transition risk. The bakeoff must pin one line and must not rely on preview APIs for a Supported claim. | Low only behind Robusta adapters. The 1:1 native surface makes accidental leakage particularly easy. |
| **MonoGame** | `DesktopGL` supports 64-bit Linux and Windows and combines SDL windowing, OpenGL graphics, and OpenAL Soft audio ([supported platforms](https://docs.monogame.net/articles/getting_started/platforms.html)). Ubuntu 24.04 fits the documented Linux baseline. | It can remain client-only, but its `Game` lifecycle, content system, graphics, input, and audio are integrated. The server project must not reference the framework merely to share math or content types. | MonoGame manages more lifetime than direct bindings, but Robusta would still need to own its platform lane, presentation separation, device-reset behavior, and process fault policy. | MonoGame publishes NuGet packages under the Microsoft Public License with third-party notices ([project and license](https://github.com/MonoGame/MonoGame)). Native dependency projection must be inspected per target. | It supplies sprites, graphics, input, text-input events, and audio. Its current 2D UI material teaches a custom game UI and accessibility design practices, not an operating-system automation-peer contract ([UI fundamentals](https://docs.monogame.net/articles/tutorials/building_2d_games/19_user_interface_fundamentals/)). Robusta still owns accessible semantics and bridges. | Long-lived and used by shipped games, with a recent 3.8.5 line. Its roadmap says the future native backend will replace `DesktopGL` with `DesktopVK`, creating a backend-transition risk for a new platform ([roadmap](https://docs.monogame.net/roadmap/)). | Highest risk. `Game`, `GameTime`, `ContentManager`, `Vector`/`Color`, `GraphicsDevice`, `SpriteBatch`, and input types are ergonomic enough to become accidental SDK contracts unless architecture tests forbid them. |

### Secondary evidence sources, not current shortlist members

- **FNA** is a mature XNA-compatible framework using SDL3, FNA3D, FAudio, and other native libraries. It supports Windows and GNU/Linux, but its official guidance rejects NuGet distribution and recommends direct project inclusion and monthly source updates ([setup](https://fna-xna.github.io/docs/1%3A-Setting-Up-FNA/), [FAQ](https://fna-xna.github.io/docs/0%3A-FAQ/)). That upstream and distribution model conflicts with Robusta's normal external-package and exact-receipt path. FNA remains valuable evidence for SDL-based desktop packaging and game compatibility, but adoption would require a separately justified packaging model.
- **Silk.NET 3 preview or MonoGame's future native/Vulkan platform** may become stronger candidates, but neither should gate Robusta 1.0 until a stable release and supported-package story exist.

### Rendering bakeoff shortlist

The later renderer evidence bakeoff should compare these coherent stacks rather than isolated API calls:

1. **SDL3 platform + SDL_GPU.** SDL_GPU selects D3D12, Vulkan, or Metal-style backends and supports Windows and Linux ([GPU API](https://wiki.libsdl.org/SDL3/CategoryGPU)). The bakeoff must test precompiled shader variants, device selection/loss, frame pacing, resource upload, clipping, render targets, and low-end hardware. SDL's simpler 2D Render API should be retained as a low-complexity comparison because SDL itself recommends it when advanced GPU control is unnecessary.
2. **Silk.NET window/input + OpenGL baseline.** This is the simplest modular route with broad desktop graphics support. A Vulkan variant is useful as a stress comparison, not an assumed first-release requirement; Silk's own documentation notes the substantial complexity jump from OpenGL to Vulkan.
3. **MonoGame `DesktopGL` + `SpriteBatch`.** This is the mature integrated reference for authoring effort, sprite batching, content use, and cross-platform behavior. A successful benchmark does not authorize its public types or content pipeline.

An operating-system support statement is not a graphics support matrix. A later decision must state minimum GPU features, driver expectations, software-rendering behavior, and whether Windows and Linux may use different graphics APIs while preserving the same Game SDK contract.

## Audio approaches

| Approach | Advantages | Risks and evidence needs |
|---|---|---|
| **SDL3 audio** | Same platform dependency as an SDL client stack; zlib license; Windows/Linux device backends; stream-based conversion, buffering, and mixing; logical devices can follow default-device changes ([audio API](https://wiki.libsdl.org/SDL3/CategoryAudio)). | Provides only a WAV loader for asset decoding, so Robusta still needs a declared resource/codec path. Callback isolation, device loss, stream limits, latency, and shutdown must be tested. |
| **miniaudio** | Single-source C library, Windows and Linux backends, playback/capture, mixer/node graph, decoding for common formats, null backend, and public-domain or MIT-0 choice ([official repository](https://github.com/mackron/miniaudio)). | No official .NET binding is identified here; its documentation says ABI compatibility is not guaranteed across versions and recommends source integration. It therefore needs a Robusta-owned native adapter, exact build, binding audit, and upgrade policy. |
| **OpenAL Soft through Silk.NET or MonoGame** | Mature cross-platform spatial-audio implementation, current releases, and established use by MonoGame `DesktopGL` ([official repository](https://github.com/kcat/openal-soft)). | LGPL licensing and third-party notice/distribution obligations require review. It is a 3D audio API rather than a complete asset, music, voice-budget, and accessibility policy. Context/device affinity and recovery need explicit tests. |

SDL audio and miniaudio are the primary standalone bakeoff candidates. OpenAL Soft remains a required comparator when evaluating Silk.NET or MonoGame. No decoder, codec, or streaming format is selected here.

## UI and accessibility approaches

Low-level game libraries do not fulfill ADR 0033's UI and accessibility-relevant platform obligation by themselves.

### Robusta-owned retained semantic UI

This approach keeps layout, style, focus, input actions, localization, text editing, semantic roles, and accessibility properties in the Game SDK and renders the resulting draw descriptions through the chosen backend.

Benefits:

- exact fit with entity-bound game UI, gamepad navigation, content packages, localization, and backend independence;
- no dependency on a desktop-widget model inside world presentation; and
- one semantic tree can feed rendering, inspection, automated UI tests, and operating-system accessibility adapters.

Costs:

- Robusta must implement and support layout, focus, clipping, text shaping, IME composition, clipboard, scaling, themes, and assistive-technology bridges;
- Windows UI Automation and Linux AT-SPI behavior require platform-specific code and real assistive-technology tests; and
- a custom-drawn interface is inaccessible unless the semantic bridge is treated as a release feature, not documentation alone.

### Avalonia-composed client shell

Avalonia is a mature MIT-licensed .NET UI framework supporting Windows and Linux ([project](https://github.com/AvaloniaUI/Avalonia)). Its current accessibility contract uses automation peers and names UI Automation on Windows and AT-SPI on Linux ([accessibility documentation](https://docs.avaloniaui.net/docs/app-development/accessibility)). This makes it a serious comparator for launcher, creator, inspector, and possibly client-shell UI.

It is not automatically a good in-game UI engine. A client-shell bakeoff must prove that an embedded or composed game surface preserves frame pacing, GPU ownership, input and IME routing, DPI behavior, fullscreen transitions, clipping, accessibility-tree synchronization, Linux Wayland/X11 behavior, and clean shutdown. Avalonia types must still not become general gameplay or authoritative simulation types.

### Recommended UI bakeoff

Compare:

1. a Robusta retained semantic UI rendered directly by each graphics candidate; and
2. an Avalonia-owned desktop shell hosting the game surface and exposing native accessibility semantics.

Both must run the same UI scenario: localized menus, nested panels, scrolling inventory, entity-bound dialog, text field with IME composition, keyboard and gamepad-only navigation, UI scaling, high contrast, Narrator on Windows, and Orca/AT-SPI on Ubuntu. Pixel similarity is insufficient; focus, roles, names, values, actions, announcements, and input behavior must agree.

## Physics approaches

### Required authority and lifetime boundary

Regardless of library:

- the server or separate local authority owns authoritative collision and physics state;
- the fixed-step scheduler invokes physics at a declared phase and affinity, with buffered contact/movement results;
- client prediction, if supported, is bounded and corrected from authoritative state rather than trusted as final;
- physics IDs are adapter-local and never network, save, entity, or durable identities;
- transforms, attachments, grid colliders, teleport/reparent operations, and body state need one declared writer and atomic commit order;
- raw solver memory and contacts are not a save or wire format; and
- native failure is integrity-unknown unless a later adapter contract proves a narrower outcome.

### Comparison

| Approach | Windows/Ubuntu and headless | Lifetime, threading, and faults | License/distribution | Determinism and authority limits | Maturity and leakage |
|---|---|---|---|---|---|
| **Native Box2D 3** | Upstream builds on Windows and Linux, is portable C17, and supports optional multithreading/SIMD ([upstream repository](https://github.com/erincatto/box2d)). It can ship in physics-admitted server and client projections without any renderer dependency. | The adapter owns native world/body/shape handles, job callbacks, buffers, close ordering, and ABI containment. A native crash or memory corruption may require host termination under ADR 0026. | MIT. Upstream lists external language bindings as unsupported; Robusta needs a pinned generated or maintained C# adapter and exact native builds. | Upstream currently claims deterministic results for the same input and binary, deterministic multithreading, and cross-platform determinism from 3.1, but explicitly does not claim rollback determinism ([Box2D FAQ](https://box2d.org/documentation/md_faq.html)). Pre-solve callbacks can be order-sensitive and nondeterministic. Robusta must validate its exact build, integration, worker callbacks, creation ordering, and inputs rather than inheriting a marketing-level promise. Server authority remains required. | Strong upstream feature and performance story, including data-oriented events and queries. The C handle API maps cleanly to an internal adapter, but must never leak through the Game SDK. |
| **Box2D.NET** | Pure C# port claiming Windows, Linux, macOS, Unity, and server use, with no native physics payload ([project](https://github.com/ikpil/Box2D.NET)). | Managed memory removes ABI crash and native-loading risk, but scheduler ownership, callbacks, allocation, and fault boundaries remain. | MIT. The project says source integration is currently the normal path and NuGet support is future work, which conflicts with Robusta's preferred exact package consumption until resolved. | It mirrors Box2D, but Robusta needs independent serial/parallel, cross-runtime, cross-OS, and long-run parity evidence. Upstream Box2D determinism claims do not automatically transfer to a port. | Active recent releases, but a smaller ecosystem and an incomplete package story. Its Box2D-shaped API remains implementation-private. |
| **Aether.Physics2D maintained fork** | Pure C# with a standalone NuGet package and no required third-party math library; suitable for headless use ([maintained fork](https://github.com/nkast/Aether.Physics2D)). | Avoids unmanaged lifetime and crash boundaries. Its callback-heavy model still needs scheduler buffering, ownership, and allocation analysis. | Open source with published NuGet packages; exact license and transitive notices must be captured by the receipt audit. | No current cross-platform or rollback determinism guarantee was found in the reviewed primary materials. Treat it as a managed behavior/performance comparator, not a deterministic oracle. | Derived from an older, established Box2D/Farseer line and has a broad feature set. It may offer lower integration cost but greater semantic distance from current Box2D 3. |

Native Box2D 3 and Box2D.NET are the primary current-engine comparison. Aether is the mature managed-line comparator. No physics engine, binding, solver configuration, or determinism class is selected.

## Headless and side-projection requirements

Every bakeoff must build and inspect at least these exact projections:

| Projection | Permitted platform material |
|---|---|
| Minimal non-spatial dedicated server | No window, graphics, input, UI, audio, or physics managed/native dependency; no initialization, threads, or module loads for them |
| Physics-admitted dedicated server | Physics adapter and declared codec/resource dependencies only; no window, graphics, input, UI, or audio material |
| Ordinary desktop client without prediction physics | Window, input, graphics, UI, audio as admitted; no authoritative server physics dependency unless a client presentation feature explicitly needs a physics projection |
| Desktop client with declared prediction physics | Client-safe physics projection with no server-only state, callbacks, or administration material |
| Creator/editor client | Explicit creator UI and adapter projection; never an implicit dependency of production game clients |

CI evidence should inspect managed references, native binaries, dynamic module loads, process threads, startup logs, package size, memory, and idle step time. A runtime flag that skips initialization is not sufficient if the forbidden payload remains in a profile that ADR 0033 says must omit it.

## Unresolved decisions

The following decisions remain open and should precede adoption:

1. Minimum GPU feature, driver, and hardware baseline for Windows 11 and Ubuntu 24.04; OpenGL versus modern D3D12/Vulkan expectations; and software-rendering or unsupported-device behavior.
2. Render API contract: sprite batching, cameras, texture arrays/atlases, render targets, clipping, blend modes, custom shaders, particles, lighting, post-processing, and diagnostic capture.
3. Shader source, offline compilation, reflection, per-backend formats, cache identity, and receipt fingerprints.
4. Platform-thread model, render preparation/execution split, device-loss recovery, and multi-window requirements.
5. Input identity, action rebinding, controller mapping, hot-plug, text/IME, clipboard, focus, and remote-input boundaries.
6. Audio voices, buses, priorities, spatialization, codecs, streaming, device changes, capture/voice chat, and accessibility behavior.
7. UI composition, styling, text shaping/font fallback, semantic automation tree, Narrator and AT-SPI support, and whether Avalonia participates in the production client or tools only.
8. Physics units, body/shape/joint floor, collision filtering, contact ordering, grid collider generation, worker integration, numerical guarantee, prediction/rollback boundary, and version migration.
9. Native adapter ABI, supported binding ownership, loading, resource lifetime, affinity, crash reporting, supply-chain review, licensing/notices, and upgrade/rollback policy under ADR 0034.
10. Whether the first supported backend is an internal fixed implementation or a published advanced adapter; internal replaceability does not require a public plugin ABI.

World-model question 26 remains relevant: a physics library's repeatability claim does not by itself decide Robusta replay formats, cross-version reproduction, saved solver state, or migration comparison.

## Proposed evidence bakeoffs

Each bakeoff should produce raw results on clean Windows 11 and Ubuntu 24.04 machines, exact receipts, logs, and a short conclusion. Spike code remains disposable and cannot become a public contract by accident.

1. **Clean package and startup:** publish each coherent client stack as an external-consumer bundle; measure restore/build/publish complexity, cold start, installed size, loaded modules, threads, and idle memory. Repeat the headless side-projection audits above.
2. **Representative 2D rendering:** station-like dense map with intact rigid-grid motion (not topology split/merge), layered sprites, lights, particles, text, UI clipping, and multiple cameras; contrasting sparse/non-grid game; measure CPU preparation, GPU frame time, draw/dispatch count, upload bandwidth, allocations, and frame pacing at declared resolutions.
3. **Window and input matrix:** Wayland and X11 on Ubuntu plus Windows; resize, high DPI, fullscreen, alt-tab, multiple monitors, cursor modes, clipboard, Unicode and IME composition, keyboard layout changes, controller hot-plug/remap, focus loss, and graceful close.
4. **Shader and resource reproducibility:** compile the same declared shader/resources twice on both operating systems; compare normalized artifacts and fingerprints; exercise cache invalidation, missing capabilities, malformed input, and backend mismatch diagnostics.
5. **Audio stress and recovery:** many short voices, music streaming, bus changes, spatial pan, default-device change, unplug/replug, underrun, decode failure, callback overrun, and shutdown; measure latency, dropouts, CPU, allocations, and cleanup.
6. **UI and accessibility:** run the shared scenario described above with keyboard, gamepad, pointer, Narrator, and Orca; inspect roles, focus, labels, values, actions, announcements, text input, scaling, localization, contrast, and frame pacing.
7. **Native lifetime and fault injection:** fail every initialization/acquisition step; close during upload/callback; simulate device loss; throw in managed callback boundaries; leak and double-close handles; hang a native call under supervisor observation; verify diagnostics and ADR 0026 escalation.
8. **Physics behavior and scale:** identical fixtures for nested contacts, sensors, fast bodies, stacks, joints, moving frames, static grid chains, collider rebuild, queries, teleport/reparent, and world disposal; measure step time, memory, allocation, and contact/event stability.
9. **Physics reproducibility:** replay canonical input streams across worker counts, repeated runs, Windows/Ubuntu, Debug/Release, runtime versions, and SIMD settings; compare committed Robusta state and ordered events. Record divergence honestly and distinguish same-build repeatability from rollback and cross-version guarantees.
10. **Authoritative multiplayer:** run server physics plus two clients under latency/loss/reorder; demonstrate declared client prediction and correction, interest entry/exit, late-contact rejection, and no client authority over shared collision outcomes.
11. **SDK leakage:** compile both external reference games against published SDK packages without backend packages; scan public signatures and generated metadata; substitute a fake conformance backend without changing game code; reject raw native handles and backend namespaces.
12. **Distribution and licensing:** build exact Windows, Ubuntu client, minimal server, and physics-server receipts; inventory native dependencies and notices; verify side-specific omission, clean install, update, rollback, crash cleanup, and unsupported-platform diagnostics.

## Evidence tiers and decision gates

The bakeoff list is a release-qualification catalogue, not one pre-M2 blocking batch.

| Gate | Evidence required before the gate | Deliberately later evidence |
|---|---|---|
| **Pre-M2 initial client-stack ADR** | Accept presentation-snapshot, platform-thread, backend-neutral render/input, semantic UI/accessibility, audio-ownership, and applicable native-lifetime contracts. Run bounded Windows and Ubuntu slices of bakeoffs 1-3, 5-7, and 11, plus the clean-package, headless-omission, native-inventory, and notice portions of bakeoff 12. Both external games must complete W0 through published SDK artifacts. | Dense native gameplay scale, physics, multiplayer, update/rollback, and full release fault qualification. Physics is not an M2 prerequisite when the W0 interaction does not admit it. |
| **Pre-M3 rendering and physics ADRs** | Complete the representative rendering and shader/resource work in bakeoffs 2 and 4, then physics behavior and same-domain reproducibility in bakeoffs 8 and 9 for any selected physics candidate. Complete the relevant native-lifetime cases from bakeoff 7. | Prediction/correction and network-fault proof remain M4; installer rollback and final support qualification remain M5/M8. |
| **M4 multiplayer qualification** | Complete bakeoff 10 and the client-side prediction, correction, interest, secrecy, reconnect, and fault portions of the ordinary network workload. | Distribution support promotion remains later. |
| **M5 delivery qualification** | Complete the install, update, rollback, native inventory, licensing, crash-cleanup, and unsupported-platform portions of bakeoffs 1, 7, and 12 against exact release receipts. | Long-run and release-wide promotion evidence remains M8. |
| **M8 support promotion** | Complete every applicable bakeoff at the approved fixture sizes and budgets for both supported operating systems and both external games. | No omitted applicable result may be inferred from an earlier spike. |

## Recommendation for the next decision step

Before opening the initial client-stack selection ADR, accept technical contracts for:

1. client presentation snapshots and platform-thread ownership;
2. backend-neutral rendering/resource and input-action surfaces;
3. semantic game UI plus accessibility bridges;
4. audio ownership and command/mixing boundaries; and
5. native adapter manifests, packaging, fault classification, and support disclosure for the candidates under review.

Then run the pre-M2 evidence tier. That ADR should select the smallest coherent window, input, rendering, UI, and audio stack that passes its bounded gate; it need not preselect physics or claim M4/M5 qualification. Before M3, separately accept world-scoped physics authority, phase ordering, numerical guarantees, and adapter lifetime, then run the physics tier. A dependency becomes `Supported` only after all applicable later tiers pass. Familiarity, an attractive sample, or success in only the station-like game is not sufficient.
