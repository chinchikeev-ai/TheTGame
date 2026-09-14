# AI Task Contract

## Task ID
`UX-004-DIVINE-PATRON-COMMENTARY`

## Goal
Move battlefield event commentary out of the lower-left Hector block and give it to the selected Patron God in a dedicated upper-right observer panel.

## Acceptance criteria

- [x] Four transparent patron portraits exist under `Assets/Resources/UI/Patrons`.
- [x] The selected patron appears on the right and faces toward the battlefield.
- [x] A compact speech panel appears to the left of the portrait.
- [x] Ares, Athena, Apollo and Poseidon have distinct RU/EN voices.
- [x] Commentary reacts to wave flow, build/sell actions, gate damage, leaks, kill milestones, Divine Power, Hector and Menelaus.
- [x] Hector's lower-left panel contains hero controls/status only and no longer owns general event commentary.
- [x] The patron panel hides while blocking menus are open.
- [ ] Real Unity Play Mode visual QA at 1920x1080 and 1366/1376x768.

## Validation

- architecture guard;
- EditMode catalog completeness tests;
- PlayMode hierarchy, right anchoring, portrait loading and left-commentary removal test;
- real Unity visual QA remains required before final visual acceptance.
