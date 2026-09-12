using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

public class GameUIController : MonoBehaviour
{
    Text statsText, waveText, nextWaveText, selectedText, endText;
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
        GameManager gm = GameManager.Instance;
        if (gm == null) return;
        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();
        if (placement == null) placement = FindFirstObjectByType<TowerPlacement>();

        statsText.text =
            $"{L("GOLD", "ЗОЛОТО")}  <b>{gm.Money}</b>     " +
            $"{L("GATE", "ВОРОТА")}  <b>{gm.BaseHealth}/{gm.MaxBaseHealth}</b>     " +
            $"{L("ENEMIES", "ВРАГИ")}  <b>{EnemyRegistry.AliveCount}</b>";

        int runSeconds = Mathf.RoundToInt(gm.RunTime);
        waveText.text =
            $"{L("THE LANDING", "ВЫСАДКА")}   •   " +
            $"{L("WAVE", "ВОЛНА")} {gm.CurrentWave}/{gm.MaxWaves}   •   " +
            $"{runSeconds / 60:00}:{runSeconds % 60:00}";

        if (spawner != null)
        {
            string threat = spawner.NextWaveHasBoss
                ? L("  •  BOSS: MENELAUS", "  •  БОСС: МЕНЕЛАЙ")
                : spawner.NextWaveHasHeavy
                    ? L("  •  HEAVY HOPLITES", "  •  ТЯЖЁЛЫЕ ГОПЛИТЫ")
                    : "";
            string target = $"  •  {L("TARGET", "ЦЕЛЬ")} {Mathf.RoundToInt(spawner.TargetWaveDuration)}{L("s", "с")}";

            if (spawner.WaveActive)
                nextWaveText.text = $"{L("WAVE ACTIVE", "ВОЛНА ИДЁТ")}  •  {L("ALIVE", "ВРАГОВ")} {EnemyRegistry.AliveCount}{target}{threat}";
            else if (spawner.InterWaveCountdown > 0f)
                nextWaveText.text = $"{L("NEXT WAVE", "СЛЕДУЮЩАЯ ВОЛНА")}  {Mathf.CeilToInt(spawner.InterWaveCountdown)}{L("s", "с")}  •  {spawner.NextWaveEnemyCount} {L("enemies", "врагов")}{threat}";
            else
                nextWaveText.text = $"{L("DEFENSE READY", "ОБОРОНА ГОТОВА")}  •  {spawner.NextWaveEnemyCount} {L("enemies", "врагов")}{threat}";
        }

        Tower selected = placement != null ? placement.SelectedTower : null;
        selectedPanel.gameObject.SetActive(selected != null);
        if (selected != null)
        {
            string upgrade = selected.Level >= 3
                ? L("MAX LEVEL", "МАКС. УРОВЕНЬ")
                : $"{L("UPGRADE", "УЛУЧШИТЬ")} {selected.UpgradeCost}";
            string priority = selected.Type == TowerType.TrojanGuard
                ? L("BLOCKING SQUAD", "БЛОКИРУЮЩИЙ ОТРЯД")
                : $"{L("PRIORITY", "ПРИОРИТЕТ")}: {selected.Priority}";
            selectedText.text =
                $"<b>{selected.DisplayName}</b>   {L("LVL", "УР.")} {selected.Level}/3\n" +
                $"{L("DMG", "УРОН")} {selected.damage:0}    {L("RANGE", "ДАЛЬНОСТЬ")} {selected.range:0.0}    {L("RATE", "СКОРОСТЬ")} {selected.fireRate:0.0}/s\n" +
                $"{priority}\n{upgrade}   •   {L("SELL", "ПРОДАТЬ")} {selected.SellValue}";
        }

        endText.gameObject.SetActive(gm.GameEnded);
        if (gm.GameEnded)
            endText.text = gm.EndMessage == "VICTORY" ? L("VICTORY", "ПОБЕДА") : L("TROY'S DEFENSE BROKEN", "ОБОРОНА ТРОИ ПРОРВАНА");
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
        canvas.sortingOrder = 10;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = .5f;
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject topLeft = CreatePanel(canvas.transform, "TopLeftStatus", new Vector2(20, -18), new Vector2(610, 92));
        Anchor(topLeft.GetComponent<RectTransform>(), new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1));
        statsText = CreateText(topLeft.transform, "Stats", new Vector2(18, -12), new Vector2(575, 36), 24, TextAnchor.UpperLeft);
        waveText = CreateText(topLeft.transform, "Wave", new Vector2(18, -48), new Vector2(575, 30), 18, TextAnchor.UpperLeft);

        GameObject topCenter = CreatePanel(canvas.transform, "WaveStatus", new Vector2(0, -18), new Vector2(760, 54));
        Anchor(topCenter.GetComponent<RectTransform>(), new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(.5f, 1));
        nextWaveText = CreateText(topCenter.transform, "NextWave", Vector2.zero, new Vector2(730, 46), 19, TextAnchor.MiddleCenter);
        Anchor(nextWaveText.rectTransform, Vector2.zero, Vector2.one, new Vector2(.5f, .5f));
        nextWaveText.rectTransform.offsetMin = new Vector2(12, 4);
        nextWaveText.rectTransform.offsetMax = new Vector2(-12, -4);

        CreateBuildBar(canvas.transform);
        CreateSelectedPanel(canvas.transform);

        Text help = CreateText(canvas.transform, "Help", new Vector2(-22, 18), new Vector2(900, 30), 15, TextAnchor.LowerRight);
        Anchor(help.rectTransform, new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0));
        help.text = L(
            "HECTOR  LMB SELECT • RMB MOVE • Q WAR CRY • E SHIELD WALL • R SPEAR • F FOR TROY!",
            "ГЕКТОР  ЛКМ ВЫБОР • ПКМ ДВИЖЕНИЕ • Q КЛИЧ • E ЩИТЫ • R КОПЬЁ • F ЗА ТРОЮ!");

        endText = CreateText(canvas.transform, "End", Vector2.zero, new Vector2(1100, 160), 58, TextAnchor.MiddleCenter);
        Anchor(endText.rectTransform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f));
        endText.gameObject.SetActive(false);
    }

    void CreateBuildBar(Transform parent)
    {
        GameObject panel = CreatePanel(parent, "BuildBar", new Vector2(0, 18), new Vector2(1280, 118));
        Anchor(panel.GetComponent<RectTransform>(), new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(.5f, 0));

        TowerButton(panel.transform, TowerType.MachineGun, -510, L("ARCHERS", "ЛУЧНИКИ"));
        TowerButton(panel.transform, TowerType.Cannon, -305, L("BALLISTA", "БАЛЛИСТА"));
        TowerButton(panel.transform, TowerType.Slow, -100, L("PRIESTS", "ЖРЕЦЫ"));
        TowerButton(panel.transform, TowerType.SpearThrower, 105, L("SPEARS", "КОПЬЯ"));
        TowerButton(panel.transform, TowerType.FireTower, 310, L("FIRE", "ОГОНЬ"));
        TowerButton(panel.transform, TowerType.TrojanGuard, 515, L("GUARD", "СТРАЖА"));
    }

    void TowerButton(Transform parent, TowerType type, float x, string title)
    {
        TowerData data = BalanceCatalog.GetTower(type);
        int cost = data != null ? data.cost : 0;
        CreateButton(parent, $"{title}\n{cost} {L("GOLD", "ЗОЛОТА")}", new Vector2(x, 0), new Vector2(186, 76), () => placement?.SelectBuildType(type));
    }

    void CreateSelectedPanel(Transform parent)
    {
        GameObject panel = CreatePanel(parent, "SelectedTowerPanel", new Vector2(-20, -112), new Vector2(430, 250));
        RectTransform rt = panel.GetComponent<RectTransform>();
        Anchor(rt, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1));
        selectedPanel = rt;
        selectedText = CreateText(panel.transform, "SelectedInfo", new Vector2(18, -16), new Vector2(394, 132), 17, TextAnchor.UpperLeft);
        CreateButton(panel.transform, L("UPGRADE", "УЛУЧШИТЬ"), new Vector2(-102, -78), new Vector2(180, 46), () => placement?.UpgradeSelected());
        CreateButton(panel.transform, L("SELL", "ПРОДАТЬ"), new Vector2(102, -78), new Vector2(180, 46), () => placement?.SellSelected());
        CreateButton(panel.transform, L("TARGET PRIORITY", "ПРИОРИТЕТ ЦЕЛИ"), new Vector2(0, -137), new Vector2(384, 44), () => placement?.CycleSelectedPriority());
        selectedPanel.gameObject.SetActive(false);
    }

    GameObject CreatePanel(Transform parent, string name, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = new Color(.035f, .045f, .060f, .92f);
        RectTransform rt = image.rectTransform;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return go;
    }

    void CreateButton(Transform parent, string label, Vector2 pos, Vector2 size, UnityEngine.Events.UnityAction action)
    {
        GameObject go = new GameObject(label.Replace("\n", "_"));
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = new Color(.18f, .22f, .28f, .98f);
        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(.31f, .36f, .43f, 1f);
        colors.pressedColor = new Color(.45f, .34f, .18f, 1f);
        button.colors = colors;
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        Text text = CreateText(go.transform, "Label", Vector2.zero, size, 16, TextAnchor.MiddleCenter);
        Anchor(text.rectTransform, Vector2.zero, Vector2.one, new Vector2(.5f, .5f));
        text.rectTransform.offsetMin = new Vector2(5, 4);
        text.rectTransform.offsetMax = new Vector2(-5, -4);
        text.text = label;
    }

    Text CreateText(Transform parent, string name, Vector2 pos, Vector2 size, int fontSize, TextAnchor alignment)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Text t = go.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = fontSize;
        t.fontStyle = FontStyle.Bold;
        t.color = Color.white;
        t.alignment = alignment;
        t.supportRichText = true;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform rt = t.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return t;
    }

    void Anchor(RectTransform rt, Vector2 min, Vector2 max, Vector2 pivot)
    {
        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.pivot = pivot;
    }
}
