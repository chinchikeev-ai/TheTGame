using UnityEngine;
using UnityEngine.UI;

public class CombatControlsUI : MonoBehaviour
{
    static readonly float[] Speeds = { 1f, 2f, 5f, 10f, 20f, 50f };
    static int speedIndex;

    Canvas canvas;
    GameObject root;
    Text speedLabel;
    Text magicLabel;
    Text giftLabel;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<CombatControlsUI>() == null)
            new GameObject("CombatControlsUI").AddComponent<CombatControlsUI>();
    }

    public static float CurrentSpeed => Speeds[Mathf.Clamp(speedIndex, 0, Speeds.Length - 1)];

    void Start()
    {
        BuildUI();
    }

    void Update()
    {
        bool active = GameManager.Instance != null && !GameManager.Instance.GameEnded && Time.timeScale > 0f;
        if (root != null) root.SetActive(active);
        if (!active) return;

        if (Mathf.Abs(Time.timeScale - CurrentSpeed) > .01f) Time.timeScale = CurrentSpeed;

        speedLabel.text = GameLanguage.T($"SPEED {CurrentSpeed:0}x", $"СКОРОСТЬ {CurrentSpeed:0}x");
        float cd = GameManager.Instance.MagicCooldownRemaining;
        magicLabel.text = cd > 0f
            ? GameLanguage.T($"MAGIC {Mathf.CeilToInt(cd)}s", $"МАГИЯ {Mathf.CeilToInt(cd)}с")
            : GameLanguage.T("MAGIC READY", "МАГИЯ ГОТОВА");
        giftLabel.text = GameManager.Instance.GiftAvailable
            ? GameLanguage.T("GIFT +100 / +2 GATE", "ПОДАРОК +100 / +2 ВОРОТА")
            : GameLanguage.T("GIFT USED", "ПОДАРОК ИСПОЛЬЗОВАН");
    }

    void BuildUI()
    {
        GameObject canvasObj = new GameObject("CombatControlsCanvas");
        canvasObj.transform.SetParent(transform, false);
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 20;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        root = new GameObject("ControlsRoot");
        root.transform.SetParent(canvas.transform, false);
        RectTransform rr = root.AddComponent<RectTransform>();
        rr.anchorMin = rr.anchorMax = rr.pivot = new Vector2(1f, 1f);
        rr.anchoredPosition = new Vector2(-20f, -20f);
        rr.sizeDelta = new Vector2(360f, 250f);

        speedLabel = AddButton(root.transform, new Vector2(0, 0), CycleSpeed);
        magicLabel = AddButton(root.transform, new Vector2(0, -72), () => GameManager.Instance?.UseMagic());
        giftLabel = AddButton(root.transform, new Vector2(0, -144), () => GameManager.Instance?.UseGift());
    }

    Text AddButton(Transform parent, Vector2 pos, UnityEngine.Events.UnityAction action)
    {
        GameObject go = new GameObject("ControlButton");
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = new Color(.13f, .18f, .24f, .96f);
        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1f, 1f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(330f, 58f);
        return AddText(go.transform, 18);
    }

    Text AddText(Transform parent, int size)
    {
        GameObject go = new GameObject("Label");
        go.transform.SetParent(parent, false);
        Text t = go.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = size;
        t.fontStyle = FontStyle.Bold;
        t.color = Color.white;
        t.alignment = TextAnchor.MiddleCenter;
        RectTransform rt = t.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        return t;
    }

    void CycleSpeed()
    {
        speedIndex = (speedIndex + 1) % Speeds.Length;
        Time.timeScale = CurrentSpeed;
    }

    public static void ResumeConfiguredSpeed() => Time.timeScale = CurrentSpeed;
}
