# AI Log

## S1 — Foundation (Claude Code)
- Generated: packages (Extenject, UniTask via OpenUPM), asmdefs FB.Core / FB.Game / FB.Editor, ProjectInstaller + GameplayInstaller, generic `ComponentPool<T>` over Zenject `MonoMemoryPool`, class-based `StateMachine<T>` + round flow (Briefing → Playing → Report), `InputService` with actions built in code, Rigidbody first-person player, configs (Palette, Player, Round, LevelLayout).
- Editor tooling: `Tools/Flame&Blame/Build All` creates configs, palette materials, low-poly meshes, Player and ProjectContext prefabs and `Game.unity`; the apartment (hall, kitchen, living room) is pure data in `LevelLayoutConfig`, rebuilt idempotently.
- Decision: movement steers velocity with a *capped* acceleration, so impulses from the hose/electricity really push the player and `GroundControl` (0..1) makes puddles slippery without extra code.
- Decision: the stove sits next to a wide kitchen→living doorway so the planned fire chain (pan → cabinets → curtain → sofa) is spatially possible.
- Human: chose the plan (2-day scope), reviewed layout, tests in Play Mode.
