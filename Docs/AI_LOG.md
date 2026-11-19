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
