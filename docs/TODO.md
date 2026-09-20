# TheTroyGame TODO

This is the single working task list for day-to-day development.

Keep it short. Update this file instead of creating GitHub Issues, Projects, Jira tickets, or duplicate planning documents unless the developer explicitly asks for them.

## Сейчас

- Выполнить `ART-007-CHAPTER-I-GOLDEN-REFERENCE-ALIGNMENT`: сначала Visual Gap Audit текущего Chapter I против `ART_BIBLE.md` v2.0 и `GOLDEN_REFERENCES.md`, затем production-pass Battlefield -> HUD -> Units -> Tower-Units -> Menus -> VFX -> Polish без изменения gameplay.
- Скопировать утверждённые 10 Golden Reference PNG в `docs/art/golden-references/` под canonical именами `GR-001`…`GR-010`; до появления бинарников в репозитории они считаются approved candidates, а не repository-tracked Golden References.
- Проверить в игре главное меню с графикой MainScreen: ИГРАТЬ / ГЕРОИ / НАСТРОЙКИ / ВЫХОД (RU/EN Canvas и переходы проверены автоматически).
- Полировка боя Chapter I: графика, анимации, читаемость атак и попаданий.
- Провести ручной проход Chapter I после последних gameplay/balance изменений.

## Сломано

- Подтверждённых новых багов сейчас нет. Добавлять сюда только воспроизводимые проблемы.

## Проверить

- ARCH-006: clean start/Restart создают один menu presentation stack и один combat-HUD presentation stack; нет дубликатов BossHUD/notifications/HUD skin/preview/follower.
- ARCH-006: после Pause/Settings/Resume menu-owned presentation остаётся привязан к текущему `GameMenuController`, а combat-owned presentation — к текущему `ModernCombatHud`.
- ARCH-005: clean start и Restart Chapter I создают ровно по одному экземпляру каждого Chapter I-owned presenter; ни один из них не появляется до выбора/установки chapter runtime profile.
- ARCH-005: после централизации lifecycle море, берег, Troy backdrop/fire/gate damage, faction staging, Menelaus entrance, encounter presentation и compact HUD визуально появляются в том же порядке и без пропусков.
- ARCH-004: после запуска/Restart Chapter I остаётся по одному `EnemySpawner`, `TowerPlacement`, `GameMenuController`, `ModernCombatHud`; их static `Instance` не остаётся stale после уничтожения.
- ARCH-004: Aegean sea replacement по-прежнему скрывает legacy water без `FindObjectsByType`, а компактный HUD применяется после startup-bind.
- Runtime input: в сцене существует ровно один `RuntimeInputBootstrap` и один `EventSystem`; после Pause/Settings/Resume и Restart Chapter I не появляются дубликаты и управление Гектором сохраняется.
- Combat HUD: декоративные панели/тексты/иконки не перехватывают pointer; только `Button/Selectable` остаются raycast-target.
- ART-007: поле боя остаётся крупнейшей визуальной областью; море -> берег -> две линии -> зона обороны -> ворота/Троя читаются за один взгляд.
- ART-007: свободное тактическое пространство не заполняется декором; tertiary props не конкурируют с юнитами, build points и telegraphs.
- ART-007: блок Гектора снизу слева компактен и стремится к `<=18%` ширины и `<=22%` высоты экрана на 1920x1080, не теряя HP/downed/ability readability.
- ART-007: `MAGIC` и `DEFENDERS` находятся рядом снизу справа, примерно одинаковы по размеру/визуальному весу и показывают ready/cooldown/locked state прямо на контролах.
- ART-007: Patron God остаётся привязан к верхнему углу, смотрит по диагонали внутрь карты и не превращается в большой фронтальный ID-card.
- ART-007: визуально сравнить runtime с `GR-002`, `GR-003`, `GR-006`, `GR-010`; supporting references `GR-007/008/009` использовать только по правилам `GOLDEN_REFERENCES.md`.
- Главное меню показывает только четыре рабочих пункта: ИГРАТЬ / АРМИЯ / НАСТРОЙКИ / ВЫХОД; старые HEROES / TOWERS / UPGRADES / SHOP не кликаются и визуально не мешают.
- ИГРАТЬ — самая крупная и заметная кнопка; АРМИЯ вторична; НАСТРОЙКИ / ВЫХОД визуально не конкурируют с запуском кампании.
- АРМИЯ открывает единый экран с левой навигацией ГЕКТОР / ЗАЩИТНИКИ / ВРАГИ и правой информационной карточкой; активная вкладка визуально выделена.
- В АРМИИ нет технических/dev-текстов; весь текст обращён к игроку.
- Back и Esc из АРМИИ возвращают в главное меню.
- Путь в бой: ИГРАТЬ -> карта кампании -> Глава I -> сложность -> бог-покровитель -> бой.
- На экране сложности работают ИСТОРИЯ / СТРАТЕГ / ЛЕГЕНДА и выбранная сложность сохраняется.
- Back из выбора бога возвращает к сложности, Back из сложности возвращает к карте кампании.
- Стартовое золото: Story 190, Strategos 150, Legendary 120.
- Золото за врагов уменьшено в 2 раза.
- Количество обычных противников увеличено в 1.5 раза относительно прежнего difficulty scaling.
- Apollo даёт +50 золота.
- Блок Гектора виден снизу слева.
- Финальная Play Mode матрица unified combat HUD: 1920x1080 RU/EN, 1376x768 RU/EN и по возможности 1366x768; проверить все состояния UX-005 без пересечений, обрезанного текста и перекрытия важной области боя.
- Верхний левый utility cluster читается как единая строка Gold -> Speed -> Settings; Gate расположен ниже и визуально важнее utility-кнопок.
- Combat Settings открывает существующий экран настроек, ставит бой на паузу; Back возвращает в Pause, Resume восстанавливает выбранную скорость.
- Encounter center не содержит Speed/Settings и остаётся свободным для encounter/progress/start state.
- Карточки шести защитников, hover-tooltip и selected-defense card читаются как portrait-led Trojan command UI; длинные RU названия не обрезаются на 1366/1376x768.
- Context tutorial использует parchment treatment и остаётся читаемым на светлых/тёмных участках карты.
- Menelaus BossHUD показывает крупный портрет, имя/роль, HP и mechanics badges без конфликта с encounter и Patron blocks.
- В матрице отдельно проверить: guidance ниже ресурсов, уведомления не перекрывают Гектора, next-encounter cards не конфликтуют с прогрессом/START, Patron не конфликтует с BossHUD/Divine Power, enemy inspector не конфликтует с selected-defense card.
- «Божественная буря» показывает действие на всех врагов, 120 урона, замедление 50% на 5 секунд и корректный ready/cooldown state.
- Клик по врагу открывает portrait-led характеристики; инспектор скрывается за паузой/настройками и возвращается корректно.
- Бог-покровитель Арес / Афина / Аполлон / Посейдон выбирается до запуска карты; после старта сменить покровителя нельзя.
- Проверить restart Chapter I: выбор сложности/покровителя не должен обходиться.
- Opening flow Chapter I: cinematic camera -> приближение четырёх греческих кораблей -> декоративная высадка/строй на берегу -> остаток authored preparation -> Encounter 1.
- Декоративная высадка не вызывает `StartWaveNow` и не может форсировать Encounter 1; первый бой начинается только когда обязательная подготовка из `EncounterData` закончена.
- Первый Encounter не начинается раньше завершения 30-секундной подготовки.
- После Encounter 1 показывается явная передышка «ПЕРВЫЙ ШТУРМ ОТБИТ», затем визуальное усиление греческого плацдарма (корабли/строй) и cue перед Encounter 2.
- При фактическом старте Encounter 2 временный regroup-строй убирается, а presentation передаёт управление обычному encounter flow без изменения состава или тайминга боя.
- Opening flow и переход к Encounter 2 читаются корректно в RU/EN и не конфликтуют с HUD/guidance на 1920x1080 и 1366/1376x768.
- Guidance показывает подготовку берега, Encounter 1 и переход ко второму бою без старой player-facing Wave-терминологии и без дублирования gate/encounter telemetry.
- Полный RU/EN localization pass: в RU нет сырых английских enum/displayName/тегов/приоритетов/сложностей; в EN нет русских утечек.
- Новый Story-проход на 1x без паузы: целевая длительность Chapter I — 11–13 минут.
- На обычном игровом масштабе хорошо читаются попадания стрел/обычных снарядов, копий, огня и slow-снарядов.
- Удары Trojan Guard и Гектора визуально читаются лучше обычного контакта персонажей.
- Тяжёлые попадания, удары по воротам и смерть босса заметно сильнее обычных попаданий.
- В плотном encounter VFX не пропадают заметно из-за нехватки пула.

## Потом

- После ART-007 пройти остальные Golden Reference области: Main Menu, Pause, Settings, Character sheets и Tower-Units как отдельные production-alignment passes, не смешивая их с gameplay/balance задачами.
- Второй проход полировки боя: улучшить конкретные attack/hit/death анимации персонажей после визуальной проверки первого impact-pass.
- Добавлять сюда только конкретные отложенные задачи, которые действительно планируется делать.

## Правило ведения

- Новая текущая работа -> `Сейчас`.
- Найденный воспроизводимый баг -> `Сломано`.
- То, что нужно подтвердить вручную -> `Проверить`.
- Не срочная конкретная задача -> `Потом`.
- Выполненные пункты удалять, а не превращать файл в бесконечный архив.
