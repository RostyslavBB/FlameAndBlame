# AI Log

## S1 — Foundation (Claude Code)
- Generated: packages (Extenject, UniTask via OpenUPM), asmdefs FB.Core / FB.Game / FB.Editor, ProjectInstaller + GameplayInstaller, generic `ComponentPool<T>` over Zenject `MonoMemoryPool`, class-based `StateMachine<T>` + round flow (Briefing → Playing → Report), `InputService` with actions built in code, Rigidbody first-person player, configs (Palette, Player, Round, LevelLayout).
- Editor tooling: `Tools/Flame&Blame/Build All` creates configs, palette materials, low-poly meshes, Player and ProjectContext prefabs and `Game.unity`; the apartment (hall, kitchen, living room) is pure data in `LevelLayoutConfig`, rebuilt idempotently.
- Decision: movement steers velocity with a *capped* acceleration, so impulses from the hose/electricity really push the player and `GroundControl` (0..1) makes puddles slippery without extra code.
- Decision: the stove sits next to a wide kitchen→living doorway so the planned fire chain (pan → cabinets → curtain → sofa) is spatially possible.
- Human: chose the plan (2-day scope), reviewed layout, tests in Play Mode.

## S2 — Hose and water (Claude Code)
- Generated: immutable `Cause` + `CauseFactory` (blame DAG starts here), `HoseConfig` (pressure/recoil curves, jet, rope, splashes), `HoseController` as `IHoseFacade`, verlet `HoseRope` (25 points, LineRenderer), `HoseLeash`, `WaterJetCollisionHandler` → `WaterHitSignal`, pooled `WaterSplash`, trauma `CameraShaker`.
- Editor: Build All adds the tap, first-person nozzle with a Shuriken jet (world collision, sendCollisionMessages), procedural droplet texture, transparent particle material, splash prefab, `Player` physics layer; added batch Zenject validation of the scene.
- Decision: recoil uses a steep curve so only near-max pressure overpowers the player's capped braking — at max pressure the player is visibly pushed back.
- Decision: particle hits are aggregated per collider per frame into one signal (bus stays quiet, no per-particle boxing); one water `Cause` per trigger press, so each spray is a separate blame chain.

## S3 - Fire and statuses (Claude Code)
- Generated: `Surface` model + passive `SurfaceEntity`/`SurfaceCatalog`, `StatusContainer` (each status keeps its Cause), `MaterialProfileConfig` x6, `ISurfaceActions` as the single mutation point, `FireSimulation` (10 Hz, heat/fuel/drying, neighbours cached with OverlapBox), `FireStarter` (pan ignites with Cause(Building, Ignite)), signals FireSpread / FireExtinguished / ItemRuined / SurfaceStatusChanged.
- Strategy: `IInteractionRule` assets in `InteractionRulesConfig` - Extinguish, WaterCooling, WaterDamage (ShortCircuit for electronics, Drown for plants), NeighborIgnition (wet heats 4x slower).
- Visuals: procedural 2x2 flame flipbook, pooled `FireVfx` (flames + smoke + rationed point lights, max 4), generic `OneShotEffectSpawner<T>` for splash/steam/sparks, property-block tint (wet darker, burned black).
- Decision: rules never touch state directly - they call `ISurfaceActions`, which always derives a Cause and fires a signal, so blame (S4) and visuals cannot miss an effect.

## S4 - Puddles, electricity, Blame (Claude Code)
- Generated: `PuddleService` (pooled discs, merge/grow/dry, Cause(Puddle) derived from the last water), slip via `IPlayerTraction`; `PowerGrid` + breaker (E), `ElectricityService` (live = power strip or short-circuited electronics, BFS over overlapping puddles), `ShockService` (impulse, shake, pooled jitter arcs, `ElectrocutionSignal` with 2 parent causes).
- Blame (pure C# core): `ActorRegistry`, `CauseGraph` (DAG, weight split equally between parents up to the roots), `BlameLedger` ($ and points per actor, fire vs firefighter damage, records), `BlameService` (observer of ruin/spread/shock), `ChainNarrator` + `BlameTextConfig` (26 Ukrainian lines, actor-aware, no repeats in a row), `CauseFormatter`, `BlameEventSignal`.
- Debug: F1 / backquote IMGUI overlay (passive view + presenter) with the last chain and per-actor totals; every blame event is also logged to the console.
- Decision: blame is computed only from the Cause graph, never from event types - any new system that passes causes correctly is blamed correctly for free.
