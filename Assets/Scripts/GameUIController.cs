using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

public class GameUIController : MonoBehaviour
{
    Text statsText;
    Text waveText;
    Text nextWaveText;
    Text selectedText;
    Text endText;
    EnemySpawner spawner;
    TowerPlacement placement;
    RectTransform selectedPanel;

    string L(string en, string ru) => GameLanguage.T(en, ru);

    void Start()
    {
        spawner = FindFirstObjectByType<EnemySpawner>();
        placement = FindFirstObjectByType<TowerPlacement>();
        EnsureEventSystem();
        BuildUI();
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();
        if (placement == null) placement = FindFirstObjectByType<TowerPlacement>();

        statsText.text = $"{L("GOLD", "ЗОЛОТО")}  {GameManager.Instance.Money}    {L("GATE", "ВОРОТА")}  {GameManager.Instance.BaseHealth}    {L("ALIVE", "ВРАГОВ")}  {EnemyRegistry.AliveCount}";
        waveText.text = $"{L("WAVE", "ВОЛНА")}  {GameManager.Instance.CurrentWave}/{GameManager.Instance.MaxWaves}";

        if (spawner != null)
        {
            string threat = spawner.NextWaveHasBoss ? "  •  MENELAUS" : spawner.NextWaveHasHeavy ? L("  •  HEAVY HOPLITES", "  •  ТЯЖЁЛЫЕ ГОПЛИТЫ") : "";
            string target = $"  •  {L("TARGET", "ЦЕЛЬ")} {Mathf.RoundToInt(spawner.TargetWaveDuration)}{L("s", "с")}";
            if (spawner.WaveActive)
                nextWaveText.text = $"{L("WAVE ACTIVE", "ВОЛНА ИДЁТ")}  •  {L("ALIVE", "ВРАГОВ")} {EnemyRegistry.AliveCount}{target}{threat}";
            else if (spawner.InterWaveCountdown > 0f)
                nextWaveText.text = $"{L("START IN", "СТАРТ ЧЕРЕЗ")} {Mathf.CeilToInt(spawner.InterWaveCountdown)}{L("s", "с")}  •  {spawner.NextWaveEnemyCount} {L("enemies", "врагов")}{target}{threat}";
            else
                nextWaveText.text = $"{L("READY", "ГОТОВО")}  •  {spawner.NextWaveEnemyCount} {L("enemies", "врагов")}{target}{threat}";
        }

        Tower selected = placement != null ? placement.SelectedTower : null;
        selectedPanel.gameObject.SetActive(selected != null);
        if (selected != null)
        {
            string upgrade = selected.Level >= 3 ? L("MAX LEVEL", "МАКС. УРОВЕНЬ") : $"{L("UPGRADE", "УЛУЧШИТЬ")} {selected.UpgradeCost} {L("GOLD", "ЗОЛОТА")}";
            selectedText.text = $"{selected.DisplayName}  •  {L("LVL", "УР.")} {selected.Level}/3\n{L("DMG", "УРОН")} {selected.damage:0}   {L("RANGE", "ДАЛЬНОСТЬ")} {selected.range:0.0}   {L("RATE", "СКОРОСТЬ")} {selected.fireRate:0.0}/s\n{upgrade}   •   {L("SELL", "ПРОДАТЬ")} {selected.SellValue} {L("GOLD", "ЗОЛОТА")}";
        }

        endText.gameObject.SetActive(GameManager.Instance.GameEnded);
        if (GameManager.Instance.GameEnded)
            endText.text = GameManager.Instance.EndMessage == "VICTORY" ? L("VICTORY", "ПОБЕДА") : L("GAME OVER", "ПОРАЖЕНИЕ");
    }

    void EnsureEventSystem()
    {
        if (EventSystem.current != null) return;
        GameObject es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
        es.AddComponent<InputSystemUIInputModule>();
#else
        es.AddComponent<StandaloneInputModule>();
#endif
    }

    void BuildUI()
    {
        GameObject canvasObj = new GameObject("GameCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObj.AddComponent<GraphicRaycaster>();

        statsText = CreateText(canvas.transform, "Stats", new Vector2(24, -22), new Vector2(760, 48), 28, TextAnchor.UpperLeft);
        waveText = CreateText(canvas.transform, "Wave", new Vector2(24, -68), new Vector2(360, 42), 24, TextAnchor.UpperLeft);
        nextWaveText = CreateText(canvas.transform, "NextWave", new Vector2(0, -22), new Vector2(1100, 48), 20, TextAnchor.UpperCenter);
        Anchor(nextWaveText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));

        CreateBuildBar(canvas.transform);
        CreateSelectedPanel(canvas.transform);

        Text help = CreateText(canvas.transform, "Help", new Vector2(-24, 24), new Vector2(900, 58), 18, TextAnchor.LowerRight);
        Anchor(help.rectTransform, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f));
        help.text = L("2D GRID • CLICK ANY FREE CELL TO BUILD • ROAD/GATE BLOCKED • HECTOR: LMB SELECT, RMB MOVE, Q WAR CRY", "2D СЕТКА • СТРОИТЬ МОЖНО НА ЛЮБОЙ СВОБОДНОЙ КЛЕТКЕ • ДОРОГА/ВОРОТА ЗАКРЫТЫ • ГЕКТОР: ЛКМ ВЫБОР, ПКМ ДВИЖЕНИЕ, Q БОЕВОЙ КЛИЧ");

        endText = CreateText(canvas.transform, "End", Vector2.zero, new Vector2(900, 180), 64, TextAnchor.MiddleCenter);
        Anchor(endText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        endText.gameObject.SetActive(false);
    }

    void CreateBuildBar(Transform parent)
    {
        GameObject panel = CreatePanel(parent, "BuildBar", new Vector2(0, 20), new Vector2(760, 105));
        Anchor(panel.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
        CreateButton(panel.transform, L("ARCHER TOWER\n100 GOLD", "БАШНЯ ЛУЧНИКОВ\n100 ЗОЛОТА"), new Vector2(-245, 0), new Vector2(220, 72), () => placement?.SelectBuildType(TowerType.MachineGun));
        CreateButton(panel.transform, L("BALLISTA\n220 GOLD", "БАЛЛИСТА\n220 ЗОЛОТА"), new Vector2(0, 0), new Vector2(220, 72), () => placement?.SelectBuildType(TowerType.Cannon));
        CreateButton(panel.transform, L("PRIESTS OF APOLLO\n160 GOLD", "ЖРЕЦЫ АПОЛЛОНА\n160 ЗОЛОТА"), new Vector2(245, 0), new Vector2(220, 72), () => placement?.SelectBuildType(TowerType.Slow));
    }

    void CreateSelectedPanel(Transform parent)
    {
        GameObject panel = CreatePanel(parent, "SelectedTowerPanel", new Vector2(-24, -120), new Vector2(430, 210));
        RectTransform rt = panel.GetComponent<RectTransform>();
        Anchor(rt, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f));
        selectedPanel = rt;
        selectedText = CreateText(panel.transform, "SelectedInfo", new Vector2(20, -18), new Vector2(390, 105), 19, TextAnchor.UpperLeft);
        CreateButton(panel.transform, L("UPGRADE", "УЛУЧШИТЬ"), new Vector2(-105, -65), new Vector2(175, 52), () => placement?.UpgradeSelected());
        CreateButton(panel.transform, L("SELL", "ПРОДАТЬ"), new Vector2(105, -65), new Vector2(175, 52), () => placement?.SellSelected());
        selectedPanel.gameObject.SetActive(false);
    }

    GameObject CreatePanel(Transform parent, string name, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(name); go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>(); image.color = new Color(0.06f, 0.08f, 0.11f, 0.88f);
        RectTransform rt = image.rectTransform; rt.anchoredPosition = pos; rt.sizeDelta = size; return go;
    }

    void CreateButton(Transform parent, string label, Vector2 pos, Vector2 size, UnityEngine.Events.UnityAction action)
    {
        GameObject go = new GameObject(label.Replace("\n", "_")); go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>(); image.color = new Color(0.18f, 0.25f, 0.34f, 0.96f);
        Button button = go.AddComponent<Button>(); button.targetGraphic = image; button.onClick.AddListener(action);
        RectTransform rt = image.rectTransform; rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f); rt.anchoredPosition = pos; rt.sizeDelta = size;
        Text text = CreateText(go.transform, "Label", Vector2.zero, size, 18, TextAnchor.MiddleCenter);
        Anchor(text.rectTransform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f)); text.rectTransform.offsetMin = Vector2.zero; text.rectTransform.offsetMax = Vector2.zero;
    }

    Text CreateText(Transform parent, string name, Vector2 pos, Vector2 size, int fontSize, TextAnchor alignment)
    {
        GameObject go = new GameObject(name); go.transform.SetParent(parent, false);
        Text t = go.AddComponent<Text>(); t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); t.fontSize = fontSize; t.fontStyle = FontStyle.Bold; t.color = Color.white; t.alignment = alignment;
        t.text = name == "Label" ? parent.name.Replace("_", "\n") : ""; t.horizontalOverflow = HorizontalWrapMode.Wrap; t.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform rt = t.rectTransform; rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0f, 1f); rt.anchoredPosition = pos; rt.sizeDelta = size; return t;
    }

    void Anchor(RectTransform rt, Vector2 min, Vector2 max, Vector2 pivot)
    {
        rt.anchorMin = min; rt.anchorMax = max; rt.pivot = pivot;
    }
}
