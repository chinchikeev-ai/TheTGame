using UnityEngine;
using UnityEngine.UI;

public class BossHUD : MonoBehaviour
{
    GameObject root;
    Text label;
    Text hpText;
    Text subtitle;
    Image fill;
    Image dangerGlow;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<BossHUD>() == null)
            new GameObject("BossHUD").AddComponent<BossHUD>();
    }

    void Start()
    {
        GameObject canvasObj = new GameObject("BossHUDCanvas");
        canvasObj.transform.SetParent(transform, false);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 70;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;

        root = new GameObject("BossBar");
        root.transform.SetParent(canvasObj.transform, false);
        Image bg = root.AddComponent<Image>();
        bg.color = new Color(.045f, .012f, .010f, .97f);
        RectTransform rr = bg.rectTransform;
        rr.anchorMin = rr.anchorMax = rr.pivot = new Vector2(.5f, 1f);
        rr.anchoredPosition = new Vector2(0, -182);
        rr.sizeDelta = new Vector2(920, 124);

        Outline outline = root.AddComponent<Outline>();
        outline.effectColor = new Color(.85f, .20f, .06f, .72f);
        outline.effectDistance = new Vector2(2f, -2f);

        GameObject glowObj = new GameObject("DangerGlow");
        glowObj.transform.SetParent(root.transform, false);
        dangerGlow = glowObj.AddComponent<Image>();
        dangerGlow.color = new Color(.55f, .04f, .02f, .18f);
        RectTransform gr = dangerGlow.rectTransform;
        gr.anchorMin = Vector2.zero;
        gr.anchorMax = Vector2.one;
        gr.offsetMin = new Vector2(-5, -5);
        gr.offsetMax = new Vector2(5, 5);
        gr.SetAsFirstSibling();

        GameObject trackObj = new GameObject("Track");
        trackObj.transform.SetParent(root.transform, false);
        Image track = trackObj.AddComponent<Image>();
        track.color = new Color(.15f, .055f, .035f, 1f);
        RectTransform tr = track.rectTransform;
        tr.anchorMin = new Vector2(0, 0);
        tr.anchorMax = new Vector2(1, 0);
        tr.pivot = new Vector2(.5f, 0);
        tr.offsetMin = new Vector2(14, 36);
        tr.offsetMax = new Vector2(-14, 60);

        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(trackObj.transform, false);
        fill = fillObj.AddComponent<Image>();
        fill.color = new Color(.78f, .12f, .045f, 1f);
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        RectTransform fr = fill.rectTransform;
        fr.anchorMin = Vector2.zero;
        fr.anchorMax = Vector2.one;
        fr.offsetMin = new Vector2(3, 3);
        fr.offsetMax = new Vector2(-3, -3);

        label = MakeText(root.transform, "Label", 24, TextAnchor.MiddleLeft);
        label.color = new Color(1f, .72f, .30f, 1f);
        RectTransform lr = label.rectTransform;
        lr.anchorMin = new Vector2(0, .58f);
        lr.anchorMax = new Vector2(.72f, 1f);
        lr.offsetMin = new Vector2(20, 0);
        lr.offsetMax = new Vector2(0, -4);

        hpText = MakeText(root.transform, "HpText", 19, TextAnchor.MiddleRight);
        hpText.color = new Color(1f, .88f, .72f, 1f);
        RectTransform hr = hpText.rectTransform;
        hr.anchorMin = new Vector2(.70f, .58f);
        hr.anchorMax = new Vector2(1, 1f);
        hr.offsetMin = Vector2.zero;
        hr.offsetMax = new Vector2(-20, -4);

        subtitle = MakeText(root.transform, "Subtitle", 13, TextAnchor.MiddleCenter);
        subtitle.color = new Color(.90f, .76f, .64f, 1f);
        RectTransform sr = subtitle.rectTransform;
        sr.anchorMin = Vector2.zero;
        sr.anchorMax = new Vector2(1, .29f);
        sr.offsetMin = new Vector2(12, 2);
        sr.offsetMax = new Vector2(-12, -2);

        root.SetActive(false);
    }

    void Update()
    {
        Enemy boss = null;
        foreach (Enemy enemy in EnemyRegistry.All)
        {
            if (enemy != null && enemy.Archetype == EnemyArchetype.Boss)
            {
                boss = enemy;
                break;
            }
        }

        bool visible = boss != null && boss.Health > 0f;
        root.SetActive(visible);
        if (!visible) return;

        float health01 = boss.Health01;
        fill.fillAmount = health01;
        fill.color = health01 < .30f
            ? new Color(.98f, .18f, .035f, 1f)
            : new Color(.78f, .12f, .045f, 1f);

        dangerGlow.color = health01 < .30f
            ? new Color(.78f, .04f, .01f, .30f + Mathf.PingPong(Time.unscaledTime * .20f, .12f))
            : new Color(.55f, .04f, .02f, .16f);

        label.text = GameLanguage.T(
            "MENELAUS  •  COMMANDER OF THE ASSAULT",
            "МЕНЕЛАЙ  •  КОМАНДУЮЩИЙ ШТУРМОМ");

        hpText.text = $"{Mathf.CeilToInt(boss.Health)} / {Mathf.CeilToInt(boss.maxHealth)}   •   {Mathf.RoundToInt(health01 * 100f)}%";

        subtitle.text = GameLanguage.T(
            "COMMANDER AURA   •   CALLS REINFORCEMENTS   •   STOP HIM BEFORE THE GATE",
            "АУРА КОМАНДИРА   •   ВЫЗЫВАЕТ ПОДКРЕПЛЕНИЯ   •   НЕ ДАЙТЕ ЕМУ ДОЙТИ ДО ВОРОТ");
    }

    Text MakeText(Transform parent, string name, int fontSize, TextAnchor alignment)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        Text text = textObj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.color = Color.white;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        return text;
    }
}
