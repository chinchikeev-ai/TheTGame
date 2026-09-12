using UnityEngine;
using UnityEngine.UI;

public class BossHUD : MonoBehaviour
{
    GameObject root;
    Text label;
    Text subtitle;
    Image fill;

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
        canvas.sortingOrder = 30;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        root = new GameObject("BossBar");
        root.transform.SetParent(canvasObj.transform, false);
        Image bg = root.AddComponent<Image>();
        bg.color = new Color(.11f, .025f, .025f, .96f);
        RectTransform rr = bg.rectTransform;
        rr.anchorMin = rr.anchorMax = rr.pivot = new Vector2(.5f, 1f);
        rr.anchoredPosition = new Vector2(0, -92);
        rr.sizeDelta = new Vector2(820, 92);

        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(root.transform, false);
        fill = fillObj.AddComponent<Image>();
        fill.color = new Color(.72f, .10f, .07f, 1f);
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        RectTransform fr = fill.rectTransform;
        fr.anchorMin = new Vector2(0, 0);
        fr.anchorMax = new Vector2(1, 1);
        fr.offsetMin = new Vector2(8, 30);
        fr.offsetMax = new Vector2(-8, -8);

        label = MakeText(root.transform, "Label", 22, TextAnchor.MiddleCenter);
        RectTransform lr = label.rectTransform;
        lr.anchorMin = new Vector2(0, .48f);
        lr.anchorMax = Vector2.one;
        lr.offsetMin = Vector2.zero;
        lr.offsetMax = Vector2.zero;

        subtitle = MakeText(root.transform, "Subtitle", 14, TextAnchor.MiddleCenter);
        RectTransform sr = subtitle.rectTransform;
        sr.anchorMin = Vector2.zero;
        sr.anchorMax = new Vector2(1, .34f);
        sr.offsetMin = new Vector2(8, 2);
        sr.offsetMax = new Vector2(-8, -2);

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

        fill.fillAmount = boss.Health01;
        label.text = GameLanguage.T(
            $"MENELAUS  •  COMMANDER OF THE ASSAULT   {Mathf.CeilToInt(boss.Health)} / {Mathf.CeilToInt(boss.maxHealth)}",
            $"МЕНЕЛАЙ  •  КОМАНДУЮЩИЙ ШТУРМОМ   {Mathf.CeilToInt(boss.Health)} / {Mathf.CeilToInt(boss.maxHealth)}");
        subtitle.text = GameLanguage.T(
            "COMMANDER AURA • CALLS REINFORCEMENTS • IF HE REACHES THE GATE, TROY FALLS",
            "АУРА КОМАНДИРА • ВЫЗЫВАЕТ ПОДКРЕПЛЕНИЯ • ЕСЛИ ДОЙДЁТ ДО ВОРОТ — ТРОЯ ПАДЁТ");
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
        return text;
    }
}
