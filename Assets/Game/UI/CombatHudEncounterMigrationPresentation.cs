using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Presentation-only migration layer for the large ModernCombatHud.
/// Removes obsolete in-map patron/gift selection and guarantees Encounter player copy
/// while the remaining serialized/runtime Wave-prefixed compatibility API is retired safely.
/// </summary>
public sealed class CombatHudEncounterMigrationPresentation : MonoBehaviour
{
    ModernCombatHud hud;
    EnemySpawner spawner;
    Transform hudRoot;
    Text encounterTitle;
    Text threatText;
    Text previewText;
    Text progressText;
    Button startButton;
    GameObject firstEncounterPreparation;
    Text firstPreparationButtonText;

    void Start()
    {
        hud = FindFirstObjectByType<ModernCombatHud>();
        spawner = FindFirstObjectByType<EnemySpawner>();
        ApplyLegacyGiftRemoval();
        BindHud();
    }

    void LateUpdate()
    {
        if (hud == null) hud = FindFirstObjectByType<ModernCombatHud>();
        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();
        if (hudRoot == null) BindHud();
        if (hudRoot == null || GameManager.Instance == null) return;

        ApplyLegacyGiftRemoval();
        RefreshEncounterCopy();
    }

    void BindHud()
    {
        GameObject root = GameObject.Find("ModernCombatHUD");
        if (root == null) return;
        hudRoot = root.transform;

        Transform status = hudRoot.Find("WaveStatus");
        if (status != null)
        {
            Text[] texts = status.GetComponentsInChildren<Text>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                Text text = texts[i];
                string name = text.gameObject.name;
                if (name == "SpeedValue") continue;

                if (encounterTitle == null && text.fontSize >= 18) encounterTitle = text;
                else if (threatText == null && text.fontSize == 12) threatText = text;
                else if (previewText == null && text.fontSize == 11 && text.alignment == TextAnchor.MiddleCenter) previewText = text;
                else if (progressText == null && text.fontSize == 11) progressText = text;
            }

            Button[] buttons = status.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < buttons.Length; i++)
            {
                Text label = buttons[i].GetComponentInChildren<Text>(true);
                if (label != null && (label.text == "START" || label.text == "СТАРТ"))
                {
                    startButton = buttons[i];
                    break;
                }
            }
        }

        firstEncounterPreparation = hudRoot.Find("FirstWavePreparation")?.gameObject;
        if (firstEncounterPreparation != null)
        {
            Button button = firstEncounterPreparation.GetComponentInChildren<Button>(true);
            firstPreparationButtonText = button != null ? button.GetComponentInChildren<Text>(true) : null;
        }
    }

    void ApplyLegacyGiftRemoval()
    {
        GameObject root = GameObject.Find("ModernCombatHUD");
        if (root == null) return;

        Transform obsoleteActions = root.transform.Find("CombatActions");
        if (obsoleteActions != null && obsoleteActions.gameObject.activeSelf)
            obsoleteActions.gameObject.SetActive(false);

        Transform obsoleteChoice = root.transform.Find("DivineGiftChoiceOverlay");
        if (obsoleteChoice != null && obsoleteChoice.gameObject.activeSelf)
            obsoleteChoice.gameObject.SetActive(false);
    }

    void RefreshEncounterCopy()
    {
        GameManager gm = GameManager.Instance;
        int encounter = EncounterRuntime.CurrentEncounter(spawner);
        int maxEncounters = EncounterRuntime.MaxEncounters(gm);
        bool active = EncounterRuntime.EncounterActive(spawner);

        if (encounterTitle != null)
        {
            int elapsed = spawner != null ? Mathf.RoundToInt(spawner.CurrentWaveElapsed) : 0;
            encounterTitle.text = $"{GameLanguage.T("ENCOUNTER", "БОЙ")} {encounter}/{Mathf.Max(1, maxEncounters)}   •   {elapsed / 60:00}:{elapsed % 60:00}";
        }

        if (spawner != null && threatText != null)
        {
            string threat = EncounterRuntime.NextEncounterHasBoss(spawner)
                ? GameLanguage.T("BOSS APPROACHING: MENELAUS", "ПРИБЛИЖАЕТСЯ БОСС: МЕНЕЛАЙ")
                : spawner.NextWaveHasHeavy
                    ? GameLanguage.T("HEAVY FORMATION EXPECTED", "ОЖИДАЕТСЯ ТЯЖЁЛАЯ ФОРМАЦИЯ")
                    : GameLanguage.T("STANDARD ENEMY FORMATION", "ОБЫЧНАЯ ВРАЖЕСКАЯ ФОРМАЦИЯ");

            if (active)
            {
                threatText.text = $"{EnemyRegistry.AliveCount} {GameLanguage.T("ENEMIES REMAIN", "ВРАГОВ В СТРОЮ")} • {threat}";
            }
            else if (EncounterRuntime.InterEncounterCountdown(spawner) > 0f)
            {
                int seconds = Mathf.CeilToInt(EncounterRuntime.InterEncounterCountdown(spawner));
                threatText.text = $"{GameLanguage.T("NEXT ENCOUNTER IN", "СЛЕДУЮЩИЙ БОЙ ЧЕРЕЗ")} {seconds}{GameLanguage.T("s", "с")} • {EncounterRuntime.NextEncounterEnemyCount(spawner)} {GameLanguage.T("enemies", "врагов")}";
            }
            else
            {
                threatText.text = $"{GameLanguage.T("READY", "ГОТОВО")} • {EncounterRuntime.NextEncounterEnemyCount(spawner)} {GameLanguage.T("enemies", "врагов")} • {threat}";
            }
        }

        if (previewText != null)
            previewText.text = CombatHudEncounterFormatter.BuildPreview(spawner);

        if (progressText != null && spawner != null)
        {
            float progress = spawner.CurrentWaveProgress;
            progressText.text = active
                ? $"{EncounterRuntime.CurrentEncounterResolvedEnemies(spawner)} / {Mathf.Max(1, EncounterRuntime.CurrentEncounterTotalEnemies(spawner))}   •   {Mathf.RoundToInt(progress * 100f)}%"
                : encounter > 0 ? "100%" : "0%";
        }

        if (startButton != null)
        {
            Text label = startButton.GetComponentInChildren<Text>(true);
            if (label != null) label.text = GameLanguage.T("START ENCOUNTER", "НАЧАТЬ БОЙ");
        }

        if (firstPreparationButtonText != null)
            firstPreparationButtonText.text = GameLanguage.T("START ENCOUNTER", "НАЧАТЬ БОЙ");
    }
}
