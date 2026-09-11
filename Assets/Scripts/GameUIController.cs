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
    Text hectorText;
    Text endText;
    EnemySpawner spawner;
    TowerPlacement placement;
    HectorController hector;
    RectTransform selectedPanel;

    void Start()
    {
        spawner = FindFirstObjectByType<EnemySpawner>();
        placement = FindFirstObjectByType<TowerPlacement>();
        hector = FindFirstObjectByType<HectorController>();
        EnsureEventSystem();
        BuildUI();
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();
        if (placement == null) placement = FindFirstObjectByType<TowerPlacement>();
        if (hector == null) hector = FindFirstObjectByType<HectorController>();

        statsText.text = $"GOLD  {GameManager.Instance.Money}    GATE  {GameManager.Instance.BaseHealth}    ALIVE  {EnemyRegistry.AliveCount}";
        waveText.text = $"WAVE  {GameManager.Instance.CurrentWave}/{GameManager.Instance.MaxWaves}";

        if (spawner != null)
        {
            string threat = spawner.NextWaveHasBoss ? "  •  MENELAUS" : spawner.NextWaveHasHeavy ? "  •  MIXED UNITS" : "";
            string target = $"  •  TARGET {Mathf.RoundToInt(spawner.TargetWaveDuration)}s";
            if (spawner.WaveActive)
                nextWaveText.text = $"WAVE ACTIVE  •  ALIVE {EnemyRegistry.AliveCount}{target}{threat}";
            else if (spawner.InterWaveCountdown > 0f)
                nextWaveText.text = $"START IN {Mathf.CeilToInt(spawner.InterWaveCountdown)}s  •  {spawner.NextWaveEnemyCount} enemies{target}{threat}";
            else
                nextWaveText.text = $"READY  •  {spawner.NextWaveEnemyCount} enemies{target}{threat}";
        }

        if (hector != null)
        {
            string state = hector.Selected ? "SELECTED" : "CLICK HECTOR";
            string cry = hector.WarCryCooldownRemaining <= 0f ? "Q WAR CRY READY" : $"WAR CRY {hector.WarCryCooldownRemaining:0}s";
            hectorText.text = $"HECTOR  •  {state}  •  {cry}";
        }

        Tower selected = placement != null ? placement.SelectedTower : null;
        selectedPanel.gameObject.SetActive(selected != null);
        if (selected != null)
        {
            string upgrade = selected.Level >= 3 ? "MAX LEVEL" : $"UPGRADE {selected.UpgradeCost} GOLD";
            selectedText.text = $"{selected.DisplayName}  •  LVL {selected.Level}/3\nDMG {selected.damage:0}   RANGE {selected.range:0.0}   RATE {selected.fireRate:0.0}/s\n{upgrade}   •   SELL {selected.SellValue} GOLD";
        }

        endText.gameObject.SetActive(GameManager.Instance.GameEnded);
        if (GameManager.Instance.GameEnded) endText.text = GameManager.Instance.EndMessage;
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
        Anchor(nextWaveText.rectTransform, new Vector2(.5f, 1f), new Vector2(.5f, 1f), new Vector2(.5f, 1f));

        hectorText = CreateText(canvas.transform, "HectorStatus", new Vector2(-24, -22), new Vector2(620, 48), 20, TextAnchor.UpperRight);
        Anchor(hectorText.rectTransform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f));

        CreateBuildBar(canvas.transform);
        CreateSelectedPanel(canvas.transform);

        Text help = CreateText(canvas.transform, "Help", new Vector2(-24, 24), new Vector2(920, 58), 18, TextAnchor.LowerRight);
        Anchor(help.rectTransform, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f));
        help.text = "2D GRID • WASD PAN • WHEEL ZOOM • CLICK HECTOR • RIGHT CLICK MOVE • Q WAR CRY";

        endText = CreateText(canvas.transform, "End", Vector2.zero, new Vector2(900, 180), 64, TextAnchor.MiddleCenter);
        Anchor(endText.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f));
        endText.gameObject.SetActive(false);
    }

    void CreateBuildBar(Transform parent)
    {
        GameObject panel = CreatePanel(parent, "BuildBar", new Vector2(0, 20), new Vector2(760, 105));
        Anchor(panel.GetComponent<RectTransform>(), new Vector2(.5f, 0f), new Vector2(.5f, 0f), new Vector2(.5f, 0f));
        CreateButton(panel.transform, "ARCHER TOWER\n100 GOLD", new Vector2(-245, 0), new Vector2(220, 72), () => placement?.SelectBuildType(TowerType.MachineGun));
        CreateButton(panel.transform, "BALLISTA\n220 GOLD", new Vector2(0, 0), new Vector2(220, 72), () => placement?.SelectBuildType(TowerType.Cannon));
        CreateButton(panel.transform, "PRIESTS OF APOLLO\n160 GOLD", new Vector2(245, 0), new Vector2(220, 72), () => placement?.SelectBuildType(TowerType.Slow));
    }

    void CreateSelectedPanel(Transform parent)
    {
        GameObject panel = CreatePanel(parent, "SelectedTowerPanel", new Vector2(-24, -120), new Vector2(430, 210));
        RectTransform rt = panel.GetComponent<RectTransform>();
        Anchor(rt, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f));
        selectedPanel = rt;
        selectedText = CreateText(panel.transform, "SelectedInfo", new Vector2(20, -18), new Vector2(390, 105), 19, TextAnchor.UpperLeft);
        CreateButton(panel.transform, "UPGRADE", new Vector2(-105, -65), new Vector2(175, 52), () => placement?.UpgradeSelected());
        CreateButton(panel.transform, "SELL", new Vector2(105, -65), new Vector2(175, 52), () => placement?.SellSelected());
        selectedPanel.gameObject.SetActive(false);
    }

    GameObject CreatePanel(Transform parent, string name, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(name); go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>(); image.color = new Color(.06f, .08f, .11f, .88f);
        RectTransform rt = image.rectTransform; rt.anchoredPosition = pos; rt.sizeDelta = size; return go;
    }

    void CreateButton(Transform parent, string label, Vector2 pos, Vector2 size, UnityEngine.Events.UnityAction action)
    {
        GameObject go = new GameObject(label.Replace("\n", "_")); go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>(); image.color = new Color(.18f, .25f, .34f, .96f);
        Button button = go.AddComponent<Button>(); button.targetGraphic = image; button.onClick.AddListener(action);
        RectTransform rt = image.rectTransform; rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f); rt.anchoredPosition = pos; rt.sizeDelta = size;
        Text text = CreateText(go.transform, "Label", Vector2.zero, size, 18, TextAnchor.MiddleCenter);
        Anchor(text.rectTransform, Vector2.zero, Vector2.one, new Vector2(.5f, .5f)); text.rectTransform.offsetMin = Vector2.zero; text.rectTransform.offsetMax = Vector2.zero;
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
