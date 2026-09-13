using UnityEngine;
using UnityEngine.UI;

public sealed class ResultScreenPresentation : MonoBehaviour
{
    Canvas canvas;
    GameObject generated;
    bool builtForCurrentResult;

    string L(string en, string ru) => GameLanguage.T(en, ru);

    void Start()
    {
        canvas = FindMenuCanvas();
    }

    void Update()
    {
        if (canvas == null) canvas = FindMenuCanvas();
        if (canvas == null) return;

        Transform endMenu = canvas.transform.Find("EndMenu");
        bool visible = endMenu != null && endMenu.gameObject.activeInHierarchy;
        GameManager gm = GameManager.Instance;

        if (!visible || gm == null || !gm.GameEnded)
        {
            builtForCurrentResult = false;
            return;
        }

        if (!builtForCurrentResult)
        {
            Build(endMenu, gm);
            builtForCurrentResult = true;
        }
    }

    Canvas FindMenuCanvas()
    {
        GameObject menu = GameObject.Find("MenuCanvas");
        return menu != null ? menu.GetComponent<Canvas>() : null;
    }

    void Build(Transform endMenu, GameManager gm)
    {
        Transform card = endMenu.Find("ResultCard");
        if (card == null) return;

        if (generated != null) Destroy(generated);
        generated = new GameObject("ResultCards");
        generated.transform.SetParent(card, false);
        RectTransform root = generated.AddComponent<RectTransform>();
        root.anchorMin = root.anchorMax = root.pivot = new Vector2(.5f, .5f);
        root.anchoredPosition = new Vector2(0, 65);
        root.sizeDelta = new Vector2(860, 420);

        foreach (Text text in card.GetComponentsInChildren<Text>(true))
        {
            if (text.transform.parent == card && text.name != "Text")
                continue;
            if (text.text != null && text.text.Contains(L("MAP", "КАРТА")) && text.text.Contains(L("SCORE", "СЧЁТ")))
                text.gameObject.SetActive(false);
        }

        int totalSeconds = Mathf.RoundToInt(gm.RunTime);
        string time = $"{totalSeconds / 60:00}:{totalSeconds % 60:00}";
        string gate = $"{gm.BaseHealth}/{gm.MaxBaseHealth}";
        string waves = $"{gm.CurrentWave}/{gm.MaxWaves}";

        AddHeroScore(root, gm.FinalScore);
        AddCard(root, new Vector2(-220, 35), L("WAVES", "ВОЛНЫ"), waves, L("Defense progress", "Прогресс обороны"));
        AddCard(root, new Vector2(0, 35), L("TIME", "ВРЕМЯ"), time, L("Battle duration", "Время битвы"));
        AddCard(root, new Vector2(220, 35), L("GATE", "ВОРОТА"), gate, L("Health remaining", "Остаток прочности"));
        AddCard(root, new Vector2(-220, -105), L("KILLS", "УБИТО"), gm.Kills.ToString("N0"), L("Enemies defeated", "Врагов побеждено"));
        AddCard(root, new Vector2(0, -105), L("LEAKS", "ПРОПУЩЕНО"), gm.Leaks.ToString("N0"), L("Reached Troy", "Прошло к Трое"));
        AddCard(root, new Vector2(220, -105), L("GOLD", "ЗОЛОТО"), gm.GoldEarned.ToString("N0"), L($"Spent {gm.GoldSpent:N0}", $"Потрачено {gm.GoldSpent:N0}"));
    }

    void AddHeroScore(RectTransform parent, int score)
    {
        Text label = MakeText(parent, L("FINAL SCORE", "ИТОГОВЫЙ СЧЁТ"), new Vector2(0, 165), new Vector2(540, 36), 18, new Color(.82f, .72f, .62f, .95f), FontStyle.Bold);
        label.characterSpacingCompat();
        MakeText(parent, score.ToString("N0"), new Vector2(0, 120), new Vector2(600, 64), 46, new Color(1f, .67f, .23f, 1f), FontStyle.Bold);
    }

    void AddCard(RectTransform parent, Vector2 pos, string title, string value, string caption)
    {
        GameObject go = new GameObject(title + "Card");
        go.transform.SetParent(parent, false);
        Image bg = go.AddComponent<Image>();
        bg.color = new Color(.11f, .065f, .035f, .95f);
        RectTransform rt = bg.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(200, 118);

        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(.55f, .31f, .12f, .45f);
        outline.effectDistance = new Vector2(1f, -1f);

        MakeText(go.transform, title, new Vector2(0, 35), new Vector2(180, 26), 14, new Color(.82f, .72f, .62f, 1f), FontStyle.Bold);
        MakeText(go.transform, value, new Vector2(0, 3), new Vector2(180, 44), 28, new Color(1f, .86f, .60f, 1f), FontStyle.Bold);
        MakeText(go.transform, caption, new Vector2(0, -37), new Vector2(182, 24), 12, new Color(.68f, .60f, .53f, .9f), FontStyle.Normal);
    }

    Text MakeText(Transform parent, string value, Vector2 pos, Vector2 size, int fontSize, Color color, FontStyle style)
    {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(parent, false);
        Text text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        RectTransform rt = text.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return text;
    }
}

static class LegacyTextCompat
{
    public static void characterSpacingCompat(this Text text) { }
}
