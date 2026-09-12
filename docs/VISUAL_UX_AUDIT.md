# TheTroyGame — Visual / UX Audit

Last reviewed: 2026-09-12

Scope: current local Unity project at `D:\Unity\My project`, with Chapter I / vertical-slice code under `Assets/Game`, authored data under `Assets/Resources/Data`, one active scene `Assets/Scenes/SampleScene.unity`, and URP settings in `Assets/Settings`.

## Summary

The project has moved past a throwaway prototype: systems are modular, data exists for towers/enemies/waves, Chapter I has menu, HUD, boss, Hector, music, and runtime telemetry. The remaining gap is presentation consistency. The gameplay loop exists, but the player-facing layer still feels partly runtime-generated: UI is code-built, combat readability is functional but busy, and environment/tower visuals need stronger Troy identity.

North star: **Troy = Fire**. Chapter I should read as a warm Trojan coastal defense with bronze soldiers, dark red banners, gold/terracotta UI accents, smoke, fire, readable build pads, and clear Greek assault routes.

## Scores

| Area | Score | Notes |
|---|---:|---|
| Environment | 5/10 | Coastline and routes exist, but the scene needs stronger landmark composition, readable lanes, Trojan gate focus, banners, ships, smoke, and foreground/background separation. |
| Lighting | 5/10 | URP is configured and usable. Needs a deliberate warm Mediterranean/fire palette, stronger shadows, and menu/gameplay consistency. |
| Materials | 5/10 | Runtime material fallback fixed the purple-screen risk, but many visuals still rely on simple generated materials. Need a small curated material set. |
| Tower-Units | 5/10 | Roster exists and is data-driven. Silhouettes and faction identity need more visual differentiation and soldier/crew readability. |
| Characters | 5/10 | Hector and Menelaus exist mechanically. They need clearer silhouettes, health/ability feedback, and stronger intro/presence. |
| UI | 6/10 | Menu now has art direction and combat HUD is improving. Still needs a unified visual system, better spacing, icons, settings polish, and result-screen pass. |
| UX | 6/10 | Core loop is understandable. Current P0 issues are build selection ergonomics, duplicated information, restart reliability, and PlayMode validation. |
| Camera | 6/10 | Orthographic strategy view is stable. Needs final bounds, zoom tuning, and composition against authored Chapter I landmarks. |
| VFX | 4/10 | Functional feedback exists, but projectile trails, hit/death effects, fire, boss aura, and tower identity need a production pass. |
| Animation | 3/10 | Mostly static/generated primitives. Needs at least simple idle/attack/death motion and tower-crew feedback for commercial feel. |
| Audio | 5/10 | Procedural music and imported track support exist. Needs separated music/SFX volume, licensed/clear source tracking, tower/enemy/boss SFX. |
| Performance | 6/10 | Registries and architecture guardrails exist. Pooling and high-density validation are still pending. |
| Commercial Readiness | 4/10 | The systems foundation is promising, but art consistency, feedback, menus, full build validation, and one real 1x playthrough are required before it feels commercially presentable. |

## P0 Findings

- Unity compile must stay at zero errors before any commit.
- PlayMode acceptance test currently needs a more reliable run path; MCP test calls can time out even after compilation succeeds.
- Combat HUD must avoid duplicate wave/map/time displays.
- Build selection should be compact and ergonomic: one right-side `+` opening the tower list is the current direction.
- Restart and return-to-menu must work in compiled builds via scene `buildIndex` fallback.
- Generated/imported assets need tracked `.meta` files.
- Local MCP settings must remain ignored and out of Git.

## P1 Findings

- Main menu needs consistent buttons, readable background, and a proper Continue flow.
- Chapter I environment needs stronger Trojan/Greek staging: gate, beachhead, ships, banners, fires, smoke.
- Tower-Units need recognizable silhouettes and role colors.
- Menelaus needs a more obvious boss aura and dedicated encounter readability.
- Hector needs movement bounds and clearer cooldown/ability feedback.

## P2 Findings

- Result screen should summarize chapter performance with cleaner hierarchy.
- Audio needs separate music/SFX controls and legal/source notes for imported tracks.
- UI needs iconography and color-independent threat indicators.
- Build pads and route preview should be readable without clutter.

## P3 Findings

- Add weather/fire visual variants after the Chapter I RC pass.
- Expand animation and VFX libraries.
- Prepare Chapter II only after Chapter I passes compile, PlayMode, build, and a real pacing run.
