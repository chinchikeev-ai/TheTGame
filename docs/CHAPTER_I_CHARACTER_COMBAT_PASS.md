# Chapter I Character Combat Production Pass

Status: runtime production-candidate layer implemented; real Unity Play Mode visual QA is still required.

## Scope

This pass closes several related Chapter I presentation gaps as one production block:

- Greek Archer keeps a visible nocked-arrow candidate while ready.
- The nocked arrow is hidden at the animation Release phase.
- Projectile flight starts from the resolved bow/nock/release hierarchy.
- The nocked arrow is restored when the Draw cycle resumes.
- Hector keeps the carried spear visible before ability R.
- Hector's carried spear is hidden at the release phase.
- The spear projectile starts from the resolved spear release hierarchy.
- The carried spear is restored when flight reaches its target.
- Hector melee spear presentation is suppressed while the spear is visually in flight.
- Weapon presentation is separated from projectile-flight gameplay logic.
- Release timing remains driven by CharacterPresentationState with bounded fallback timing.

## Runtime ownership

CharacterWeaponSocketResolver resolves bow/spear roots and release/nock anchors. It prefers explicit socket markers and falls back to source weapon roots, humanoid RightHand, then bounded local offsets.

CharacterWeaponPresentation owns temporary visibility state of the carried spear and the procedural nocked-arrow candidate. It exposes release/restore hooks without applying gameplay damage.

CharacterPresentationState owns animation trigger/timing phases and exposes a redraw callback so bow ammunition presentation can reappear in sync with Draw.

Enemy connects Greek Archer Release to nocked-arrow hide, projectile launch and redraw restore.

HectorPresentationBridge connects ability-R release to carried-spear hide, projectile launch and restore-on-arrival, and prevents spear melee presentation while the spear is in flight.

## Acceptance limitations

This does not promote weapon art to DONE. Remaining production work:

1. final authored arrow and thrown-spear meshes;
2. exact bow hand/grip and string/nock alignment;
3. exact spear grip/hand IK;
4. final release/contact frames based on authored animation clips;
5. Play Mode inspection from gameplay camera;
6. EN/RU 16:9 QA where combat presentation overlaps HUD/tutorial framing.
