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
  Hose -- WaterHitSignal --> Puddles[PuddleService]
  SurfWater --> Rules[InteractionRuleRunner<br/>Strategy assets]
  FireSim[FireSimulation 10 Hz] --> Rules
  Rules --> Actions[ISurfaceActions]
  Actions -- FireSpreadSignal --> Blame
  Actions -- ItemRuinedSignal --> Blame
  Actions -- FireExtinguishedSignal --> FireVfx[FireVisualService]
  Actions -- SurfaceStatusChangedSignal --> Tint[SurfaceTintService]
  Puddles --> Elec[ElectricityService BFS]
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
  Ledger --> Report[ReportPresenter]
```

| Signal | Fired by | Consumers |
|---|---|---|
| `GamePhaseChangedSignal` | `GameStateMachine` | `FireStarter`, presenters, `TimeScaleService` |
| `WaterHitSignal` | `WaterJetCollisionHandler` (1 per hit collider per frame) | `SurfaceWaterHandler`, `PuddleService` |
| `FireSpreadSignal` | `SurfaceActions` | `BlameService`, `FireVisualService` |
| `FireExtinguishedSignal` | `SurfaceActions` | `FireVisualService` (steam) |
| `ItemRuinedSignal` | `SurfaceActions` | `BlameService`, `FireVisualService` (sparks) |
| `SurfaceStatusChangedSignal` | `SurfaceActions` | `SurfaceTintService` |
| `ElectrocutionSignal` | `ShockService` | `BlameService` |
| `BlameEventSignal` | `BlameService` | `FeedPresenter`, `TimeScaleService`, debug overlay |

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
- Every effect carries a cause: water per trigger press, `Puddle` derived from the last water, fire spread with the
  burning source as parent, shocks with **two** parents (puddle + live power source).
- `CauseGraph.CollectRootShares` walks to the roots and splits weight equally between parents at every node:
  the shock above is 50% the player, 50% the old wiring. If the puddle was electrified by a TV the player had
  short-circuited, the power cause itself leads back to the player, so the verdict is 100% "Ти".
- `BlameLedger` adds $ and points per root actor; $ of the player = "firefighter damage", the rest = "fire damage".
- `ChainNarrator` picks Ukrainian lines from `BlameTextConfig` (actor-aware, no repeats in a row).
- The blame core (`CauseGraph`, `BlameLedger`, `ChainNarrator`, `CauseFormatter`) is plain C#.

## Patterns, where to look

| Pattern | Where |
|---|---|
| Dependency Injection | `ProjectInstaller`, `GameplayInstaller`, `UiInstaller` |
| Observer | Zenject SignalBus, all signals above |
| MVP | `*View` (passive MonoBehaviours) + `*Presenter` (plain C#) in `UI/`, `Debugging/` |
| Object Pool | `ComponentPool<T>` over `MonoMemoryPool`: splashes, steam, sparks, fire, puddles, arcs, feed lines |
| Factory | `CauseFactory`, editor builders |
| State Machine | `StateMachine<T>` + `BriefingState` / `PlayingState` / `ReportState` |
| Strategy | `IInteractionRule` assets (Extinguish, WaterCooling, WaterDamage x2, NeighborIgnition) |
| Facade | `IHoseFacade`, `ISurfaceActions` (single mutation point for surfaces) |
| Data-driven | ScriptableObject configs for every number, rule, text, color and the level layout |

## Simulation

- Central ticking: `FireSimulation` (fixed 10 Hz), `ElectricityService` (5 Hz), `PuddleService`, `ShockService` are
  plain C# `ITickable`s; no gameplay `Update()` on props.
- Fire: heat / ignition / fuel per surface, neighbours cached once with `OverlapBoxNonAlloc`, wet surfaces heat 4x slower.
- Hose: pressure curve, recoil curve against capped player braking (impulses really push), verlet rope with capsule
  segment collisions, leash force past the max length.
- Electricity: live sources = power strip or short-circuited electronics; BFS over overlapping puddle discs.

## Level and assets

`Tools/Flame&Blame/Build All` generates everything from code and configs, idempotently: palette materials, low-poly
meshes, textures (droplet, flame flipbook, UI sprites), TMP font with Cyrillic (static atlas), prefabs, and
`Game.unity` from `LevelLayoutConfig` (rooms, walls with doorway cut-outs, props with surface data).

## WebGL

- No threads; UniTask on the PlayerLoop only; every async takes a CancellationToken (unscaled time for UI).
- Gzip + decompression fallback, Minimal stripping, `link.xml` keeps Zenject / UniTask / FB.* / TMP.
- Draw calls: shared GPU-instanced palette materials, static batching, max 4 fire point lights, capped particle counts.
- Hit-stop / slow-mo always restore `Time.timeScale = 1` on report, restart and dispose.
- Shadows are off in the WebGL quality level (`Apply WebGL Settings`): URP 17 on WebGL2 bound a shadow texture to a
  comparison sampler with a mismatching format (`GL_INVALID_OPERATION`) and every Lit draw call was dropped.
  The Windows build keeps shadows.
