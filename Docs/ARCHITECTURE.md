# Flame and Blame — Architecture

A first-person firefighter physics comedy where **every effect remembers who caused it**.
Unity 6 (URP, WebGL), Extenject (DI + SignalBus + MemoryPool), UniTask, new Input System. All visuals are procedural.

## Assemblies

| Assembly | Contents | Depends on |
|---|---|---|
| `FB.Core` | Project-agnostic building blocks: `IPool<T>` / `ComponentPool<T>`, `StateMachine<T>`, `InputService` (actions in code), `CursorService`, low-poly mesh generation, `ProjectInstaller` | Zenject, UniTask, Input System |
| `FB.Game` | Gameplay, blame, UI (by namespace: `Player`, `Hose`, `Surfaces`, `Interactions`, `Fire`, `Puddles`, `Power`, `Blame`, `UI`, `Flow`, `CameraFx`) | `FB.Core` |
| `FB.Editor` | `Tools/Flame&Blame/*`: Build All, Rebuild Level, WebGL/Windows builds, Zenject validation | `FB.Game`, `FB.Core` |

## Composition roots

- **ProjectContext** (`Resources/ProjectContext.prefab`, `ProjectInstaller`) — SignalBus, input, cursor. Survives restarts.
- **SceneContext** (`Game.unity`) — `GameplayInstaller` (all simulation systems) + `UiInstaller` (views + presenters).
  Pressing **R** reloads the scene: the SceneContext is destroyed, every `IDisposable` unsubscribes and cancels its UniTasks.

No singletons, no `Find*`, no static mutable state. Scene objects the code needs (player, hose rig, surfaces, breaker,
UI views) are referenced by installers, and the editor build scripts fill those references.

## Systems and signals

```mermaid
flowchart LR
  subgraph Player
    Input[InputService] --> Move[PlayerMovement]
    Input --> Hose[HoseController<br/>IHoseFacade]
  end
  Hose -- WaterHitSignal --> SurfWater[SurfaceWaterHandler]
  Hose -- WaterHitSignal --> Puddles[PuddleSimulation<br/>water grid]
  SurfWater --> Rules[InteractionRuleRunner<br/>Strategy assets]
  FireSim[FireSimulation 10 Hz] --> Rules
  Rules --> Actions[ISurfaceActions]
  Actions -- FireSpreadSignal --> Blame
  Actions -- ItemRuinedSignal --> Blame
  Actions -- FireExtinguishedSignal --> FireVfx[FireVisualService]
  Actions -- SurfaceStatusChangedSignal --> Tint[SurfaceTintService]
  Puddles --> Elec[ElectricityService BFS]
  Puddles --> Soak[PuddleSoaking] --> Rules
  Grid[PowerGrid + Breaker] --> Elec
  Elec --> Shock[ShockService]
  Shock -- ElectrocutionSignal --> Blame[BlameService]
  Blame --> Graph[CauseGraph]
  Blame --> Ledger[BlameLedger]
  Blame --> Narrator[ChainNarrator]
  Blame -- BlameEventSignal --> Feed[FeedPresenter]
  Blame -- BlameEventSignal --> TimeFx[TimeScaleService]
  Flow[GameStateMachine] -- GamePhaseChangedSignal --> UI[Briefing / HUD / Report presenters]
  Flow -- GamePhaseChangedSignal --> Starter[FireStarter]
  Hose -- WaterHitSignal --> Npc[CivilianBrain<br/>Wander / Panic / Knocked / Shocked]
  Elec --> Npc
  Npc -- CivilianIncidentSignal --> Blame
  Shock -- ElectrocutionSignal --> ShockUi[ShockOverlayPresenter]
  Ledger --> Report[ReportPresenter]
```

| Signal | Fired by | Consumers |
|---|---|---|
| `GamePhaseChangedSignal` | `GameStateMachine` | `FireStarter`, presenters, `TimeScaleService` |
| `WaterHitSignal` | `WaterJetCollisionHandler` (1 per hit collider per frame) | `SurfaceWaterHandler`, `PuddleSimulation` |
| `FireSpreadSignal` | `SurfaceActions` | `BlameService`, `FireVisualService` |
| `FireExtinguishedSignal` | `SurfaceActions` | `FireVisualService` (steam) |
| `ItemRuinedSignal` | `SurfaceActions` | `BlameService`, `FireVisualService` (sparks) |
| `SurfaceStatusChangedSignal` | `SurfaceActions` | `SurfaceTintService` |
| `ElectrocutionSignal` | `ShockService` | `BlameService`, `ShockOverlayPresenter` |
| `BlameEventSignal` | `BlameService` | `FeedPresenter`, `TimeScaleService`, debug overlay |
| `CivilianIncidentSignal` | `CivilianChaos`, `CivilianBrain` | `BlameService` |

All signals are immutable `readonly struct`s in `Scripts/Game/Signals`.

## The blame model

```mermaid
flowchart LR
  W[Ти: HoseSpray] --> P[Ти: Puddle]
  O[Стара проводка: FaultyWiring] --> E[Electrocution]
  P --> E
  E --> V((victim: Ти))
```

- `Cause` is an immutable node `{Id, Action, Instigator, Parents}`; only `ICauseFactory` creates them.
- Every effect carries a cause: water per trigger press, `Puddle` (per grid cell) derived from the water that filled it, fire spread with the
  burning source as parent, shocks with **two** parents (puddle + live power source).
- `CauseGraph.CollectRootShares` walks to the roots and splits weight equally between parents at every node:
  the shock above is 50% the player, 50% the old wiring. If the water was made live by a TV the player had
  short-circuited, the power cause itself leads back to the player, so the verdict is 100% "Ти".
- `BlameLedger` adds $ and points per root actor; $ of the player = "firefighter damage", the rest = "fire damage".
- `ChainNarrator` picks Ukrainian lines from `BlameTextConfig` (actor-aware, no repeats in a row).
- The report shows the most expensive event as a chain of cards (`BlameChainBuilder`): root culprit -> causes
  (repeats merged, "Вогонь ×3"; long chains collapse to root, "…", last steps) -> damaged target and price. At a
  branch (puddle + power) it follows the parent that leads to the record's main culprit, so the chain explains the verdict.
- The blame core (`CauseGraph`, `BlameLedger`, `ChainNarrator`, `CauseFormatter`) is plain C#.

## Patterns, where to look

| Pattern | Where |
|---|---|
| Dependency Injection | `ProjectInstaller`, `GameplayInstaller`, `UiInstaller` |
| Observer | Zenject SignalBus, all signals above |
| MVP | `*View` (passive MonoBehaviours) + `*Presenter` (plain C#) in `UI/`, `Debugging/`; every UI animation (UniTask tweens, `UiPulse` punches) lives in presenters, views only expose setters |
| Object Pool | `ComponentPool<T>` over `MonoMemoryPool`: splashes, steam, sparks, fire, arcs, feed lines |
| Factory | `CauseFactory`, editor builders |
| State Machine | `StateMachine<T>` + `BriefingState` / `PlayingState` / `ReportState`; neighbour AI `CivilianWanderState` / `CivilianPanicState` / `CivilianKnockedState` / `CivilianShockedState` |
| Strategy | `IInteractionRule` assets (Extinguish, WaterCooling, WaterDamage x2, NeighborIgnition) |
| Facade | `IHoseFacade`, `ISurfaceActions` (single mutation point for surfaces) |
| Data-driven | ScriptableObject configs for every number, rule, text, color and the level layout |

## Simulation

- Central ticking: `FireSimulation` (fixed 10 Hz), `ElectricityService` (5 Hz), `PuddleSimulation`, `PuddleSoaking`, `ShockService` are
  plain C# `ITickable`s; no gameplay `Update()` on props.
- Fire: heat / ignition / fuel per surface, neighbours cached once with `OverlapBoxNonAlloc`, wet surfaces heat 4x slower.
- Hose: pressure curve, recoil curve against capped player braking (impulses really push), verlet rope with capsule
  segment collisions, leash force past the max length.
- Puddles: the floor is a shallow-water grid (`WaterGrid`, 10 cm cells, 30 Hz). Water lands where the jet hits (or
  drips below), flows to lower neighbours, stops at the film thickness (natural puddle edges), merges, evaporates;
  walls and static furniture are obstacles (found once with `OverlapBox`). Every cell remembers its Cause(Puddle).
  `PuddleSoaking` passes puddle water to objects standing in it through the same interaction rules.
- Electricity: BFS over wet cells from the power strip - only the puddle that touches the strip (and whatever it has
  merged with) is live; a short-circuited appliance replaces the wiring as the blamed power source.
- Water rendering: hand-written URP shaders - `FB/Puddle` (reads the grid texture: calm surface, organic edges,
  meniscus highlight, reflection and glint, ring ripples and foam at impacts, electric veins on live cells) and
  `FB/WaterStream` (aerated fire-hose jet on a LineRenderer traced along the ballistic arc, bubbles, ragged spray
  edges) plus a mist particle cloud around the jet.

## Level and assets

`Tools/Flame&Blame/Build All` generates everything from code and configs, idempotently: palette materials, low-poly
meshes, textures (droplet, flame flipbook, UI sprites), two TMP fonts with Cyrillic (Russo One + Rubik, static atlases, outline/shadow presets), prefabs, and
`Game.unity` from `LevelLayoutConfig` (rooms, walls with doorway cut-outs, props with surface data).
Furniture with complex silhouettes (sofa, TV, fridge, bed, wardrobe, bathroom, lamps...) uses CC0 models from the
Kenney Furniture Kit: `ModelPropBuilder` fits them into the
prop box, recolors their materials into the palette and adds a box collider; gameplay data stays identical.

## WebGL

- No threads; UniTask on the PlayerLoop only; every async takes a CancellationToken (unscaled time for UI).
- Gzip + decompression fallback, Minimal stripping, `link.xml` keeps Zenject / UniTask / FB.* / TMP.
- Draw calls: shared GPU-instanced palette materials, static batching, max 4 fire point lights, capped particle counts.
- Hit-stop / slow-mo always restore `Time.timeScale = 1` on report, restart and dispose.
- Shadows are off in the WebGL quality level (`Apply WebGL Settings`): URP 17 on WebGL2 bound a shadow texture to a
  comparison sampler with a mismatching format (`GL_INVALID_OPERATION`) and every Lit draw call was dropped.
  The Windows build keeps shadows.

## Look and atmosphere

- Shell: every floor/wall piece is a box mesh with UVs in meters (`BoxMeshFactory`), so the procedural parquet,
  ceramic/stone tiles and striped wallpaper (`SurfaceTextures`, 512 px = 1 m) keep their real scale; skirting boards
  and door casings are generated with the walls. Five rooms: hall, kitchen, living room, bathroom, bedroom.
- Fire: layered pooled VFX (base glow, 4x4 animated flame flipbook, additive HDR core, turbulent embers, thick smoke);
  `FireVisualService` scales each fire with the heat of what burns, so fires grow from a flicker to a blaze.
- Smoke: `SmokeService` accumulates smoke from burning objects; `FB/SmokeLayer` (stacked swirling planes with soft
  depth intersections) lowers from the ceiling, fog density rises and a "smoke" post-processing volume fades in.
- Post-processing (code-generated `VolumeProfile`s): warm "evening fire" grade - ACES, orange-tinted bloom, warm white
  balance and color filter, brown shadows (shadows/midtones/highlights), warm vignette; FXAA on the camera. A third
  "shock" profile (chromatic aberration, lens distortion, cold flash) is driven by `ShockOverlayPresenter` together
  with on-screen lightning bolts when the player is electrocuted.
- Toon fire: the flame flipbook is painted in three hard color bands with wavy licking edges. Shadows stay off in the WebGL quality level (see WebGL).
- Life: ceiling lamps with real lights and glowing shades, TV/laptop screens - all switched off by the breaker
  (`RoomLightsController`); a kitchen clock showing real time (`ClockController`); dust drifting in every room.
- Ambient light is generated at build time (`Lightmapping.Bake` with GI off) so the player build has the same
  ambient probe as the editor.

## Audio

- All sounds are synthesized offline by `Tools/Flame&Blame/Build All` (`Editor/Audio`: seeded noise, RBJ biquads,
  envelopes) into WAVs in `Audio/`; no third-party audio.
- `AudioConfig` is the mixing table (variants, volume, random pitch, anti-spam cooldown, 2D/3D per `SoundId`).
- `AudioService` plays on a fixed pool of 12 pre-built `AudioSource`s (`AudioRig`), stealing the oldest voice.
- `GameSoundEvents` is an Observer: fire spread, extinguish, shocks, short circuits and neighbour incidents each get a
  sound; it also drives the hose loop (volume/pitch = pressure) and the fire loop (on the nearest fire, louder with
  more fires). `ImpactSensor` (passive collision bridge on loose props) feeds `ImpactSounds` (speed -> volume,
  mass -> pitch). The neighbour has no voice on purpose (user decision); his shocks and ignition use the zap and whoomph. UI presenters add ticks, pops, whooshes, cash and the stamp.

## The neighbour (NPC)

- `CivilianBrain` (plain C#, ticked centrally) owns a `StateMachine<CivilianState>`: **Wander** between room waypoints,
  **Panic** (runs screaming away from the nearest fire, arms flailing) and **Knocked** (on the floor with spiral eyes,
  then panics again) and **Shocked** (twitches on the floor of a live puddle under repeated arcs until the current
  stops, then jumps up and panics). States share a `CivilianBody` context and switch through `ICivilianStates`.
- He lives on his own physics layer (`GameLayers.Civilian`): the hose rope, puddles and chaos queries use
  `EnvironmentMask` and ignore him, while water particles still hit him.
- Hose water shoves him along the nav mesh (`CivilianBody.Shove` + `NavMeshAgent.Move`), even while he lies on the floor.
  `CivilianBurning` sets him on fire next to a burning item (`Cause(Вогонь: Burn)` with that fire as parent), and he
  then ignites the loose items he knocks over; water puts him out and keeps him fireproof for a few seconds.
- `CivilianChaos` flings any loose gameplay item he runs into and reports it as `Cause(Сусід: KnockOver)`, so he is
  a fourth actor in the verdict. Hose water knocks him down (`Cause(Ти: Soak)` with the spray as parent); a live
  puddle shocks him (two parents: puddle + power source), exactly like the player's shock.
- Navigation: the nav mesh is baked by `CivilianBuilder` with the core `NavMeshBuilder` API (loose props ignored so he
  runs through them), saved as an asset and added at runtime; no extra package code at runtime.
- The character is original: primitives in palette colors (big head, red hoodie, jeans, sneakers) and a
  procedural `CivilianAnimator` (limb swing by speed, panic flailing, screaming mouth, fall) - no animator controller.
- Fires: `FireStarter` lights one random candidate per room on a schedule (`FireConfig.Outbreaks`), and the round
  cannot end as "all clear" while outbreaks are pending (`IFireOutbreaks`).
