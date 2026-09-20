# AI Task Contract

## Task ID
`ARCH-007-OWNED-ROOT-REFERENCES`

## Goal
Remove name-based lookup for owned runtime roots and make composition/owner APIs the source of references.

## Why
Even after lifecycle ownership was centralized, name-based binding such as `GameObject.Find("Chapter01_...")`, `GameObject.Find("MenuCanvas")`, `GameObject.Find("ModernCombatHUD")`, `GameObject.Find("HectorHUD")`, and `GameObject.Find("SelectedTowerCard")` remained a compile-silent coupling risk.

## Result
Chapter I world roots are now produced/exposed by their builders/presenters and injected through `ChapterOneRuntimeInstaller`. Chapter I UI roots are exposed by their owners and injected into dependent presentation passes. Menu consumers bind through `GameMenuController`; combat-HUD consumers bind through `ModernCombatHud`.

The last legacy `GameCanvas` lookup in `ModernCombatHud` was removed.

## Architecture guard
`tools/check-architecture.py` now rejects:
- any runtime `FindObjectsByType` / `FindObjectsOfType`;
- direct scene lookup inside `Update`, `LateUpdate`, `FixedUpdate`;
- owned-root name lookup for Chapter I roots and the canonical menu/HUD roots.

## Acceptance criteria
- [x] No Chapter I world presenter depends on `GameObject.Find("Chapter01_...")`.
- [x] Menu/HUD-owned roots are passed through owner APIs instead of rediscovered by name.
- [x] `ModernCombatHud` no longer searches for legacy `GameCanvas`.
- [x] Guard prevents reintroduction of owned-root name lookup.
- [x] No gameplay/balance/save semantics changed.
- [ ] Run architecture checker in a real checkout.
- [ ] Run Unity EditMode/PlayMode validation.
- [ ] Verify clean start and Restart Chapter I.

## Status
`IMPLEMENTED — UNITY VALIDATION PENDING`
