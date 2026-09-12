using UnityEngine;
using UnityEngine.UI;

public sealed class CampaignMapPresentation : MonoBehaviour
{
    Canvas canvas;

    string L(string en, string ru) => GameLanguage.T(en, ru);

    void Start()
    {
        canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;
        Build();
    }

    public void Refresh()
    {
        if (canvas == null) canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;
        Build();
    }

    void Build()
    {
        Transform levelSelect = canvas.transform.Find("LevelSelect");
        if (levelSelect == null) return;
        Transform old = levelSelect.Find("CampaignMapLayer");
        if (old != null) Destroy(old.gameObject);

        GameObject layer = new GameObject("CampaignMapLayer");
        layer.transform.SetParent(levelSelect, false);
        RectTransform root = layer.AddComponent<RectTransform>();
        root.anchorMin = Vector2.zero;
        root.anchorMax = Vector2.one;
        root.offsetMin = root.offsetMax = Vector2.zero;
        root.SetAsFirstSibling();

        Image shade = layer.AddComponent<Image>();
        shade.color = new Color(.045f, .025f, .015f, .22f);
        shade.raycastTarget = false;

        Vector2[] points =
        {
            new Vector2(-520, -160),
            new Vector2(-360,  -40),
            new Vector2(-180,   75),
            new Vector2(  20,  145),
            new Vector2( 230,  110),
            new Vector2( 420,   10),
            new Vector2( 540, -150)
        };

        for (int i = 0; i < points.Length - 1; i++)
            MakeConnector(layer.transform, points[i], points[i + 1], i + 1 < CampaignSave.UnlockedChapter);

        for (int i = 0; i < points.Length; i++)
            MakeNode(layer.transform, i + 1, points[i]);

        MakeLegend(layer.transform);
    }

    void MakeNode(Transform parent, int chapter, Vector2 pos)
    {
        bool unlocked = CampaignSave.IsUnlocked(chapter);
        bool completed = CampaignSave.IsCompleted(chapter);
        ChapterProgress progress = CampaignSave.GetChapterProgress(chapter);

        GameObject go = new GameObject("ChapterNode_" + chapter);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = completed
            ? new Color(.60f, .17f, .055f, .96f)
            : unlocked ? new Color(.38f, .19f, .08f, .94f)
            : new Color(.10f, .075f, .065f, .90f);
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(chapter == 1 ? 122f : 96f, chapter == 1 ? 122f : 96f);
        image.raycastTarget = false;

        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = unlocked ? new Color(1f, .52f, .16f, .75f) : new Color(.27f, .20f, .15f, .45f);
        outline.effectDistance = new Vector2(2f, -2f);

        string numeral = ToRoman(chapter);
        MakeText(go.transform, numeral, new Vector2(0, 8), chapter == 1 ? 34 : 28,
            unlocked ? new Color(1f, .84f, .55f, 1f) : new Color(.52f, .46f, .40f, 1f));

        string title = ChapterName(chapter);
        Text titleText = MakeText(parent, title, pos + new Vector2(0, -78), 16,
            unlocked ? new Color(.94f, .83f, .67f, 1f) : new Color(.48f, .43f, .38f, .9f));
        titleText.rectTransform.sizeDelta = new Vector2(180f, 45f);

        string state;
        if (completed) state = progress != null && progress.bestScore > 0 ? L($"BEST {progress.bestScore:N0}", $"ЛУЧШИЙ {progress.bestScore:N0}") : L("COMPLETED", "ПРОЙДЕНО");
        else if (unlocked) state = L("AVAILABLE", "ДОСТУПНО");
        else state = L("LOCKED", "ЗАКРЫТО");
        Text stateText = MakeText(parent, state, pos + new Vector2(0, -105), 12,
            completed ? new Color(1f, .52f, .20f, .95f) : new Color(.67f, .59f, .50f, .85f));
        stateText.rectTransform.sizeDelta = new Vector2(170f, 28f);
    }

    void MakeConnector(Transform parent, Vector2 a, Vector2 b, bool active)
    {
        GameObject go = new GameObject("Route");
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = active ? new Color(.75f, .31f, .10f, .55f) : new Color(.18f, .14f, .11f, .65f);
        image.raycastTarget = false;
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        Vector2 delta = b - a;
        rt.sizeDelta = new Vector2(delta.magnitude, active ? 5f : 3f);
        rt.anchoredPosition = (a + b) * .5f;
        rt.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
    }

    void MakeLegend(Transform parent)
    {
        Text text = MakeText(parent,
            L("THE WAR FOR TROY  •  7 CHAPTERS", "ВОЙНА ЗА ТРОЮ  •  7 ГЛАВ"),
            new Vector2(0, 300), 18, new Color(1f, .73f, .34f, .92f));
        text.rectTransform.sizeDelta = new Vector2(520f, 40f);
    }

    Text MakeText(Transform parent, string value, Vector2 pos, int size, Color color)
    {
        GameObject go = new GameObject("MapText");
        go.transform.SetParent(parent, false);
        Text text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = size;
        text.fontStyle = FontStyle.Bold;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        RectTransform rt = text.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(200f, 36f);
        return text;
    }

    string ChapterName(int chapter)
    {
        switch (chapter)
        {
            case 1: return L("THE LANDING", "ВЫСАДКА");
            case 2: return L("ROAD TO TROY", "ДОРОГА К ТРОЕ");
            case 3: return L("THE GATES", "ВРАТА");
            case 4: return L("HEROES OF GREECE", "ГЕРОИ ГРЕЦИИ");
            case 5: return L("THE GREAT ASSAULT", "ВЕЛИКИЙ ШТУРМ");
            case 6: return L("THE HORSE", "КОНЬ");
            case 7: return L("TROY BURNS", "ТРОЯ ГОРИТ");
            default: return "";
        }
    }

    string ToRoman(int chapter)
    {
        switch (chapter)
        {
            case 1: return "I";
            case 2: return "II";
            case 3: return "III";
            case 4: return "IV";
            case 5: return "V";
            case 6: return "VI";
            case 7: return "VII";
            default: return chapter.ToString();
        }
    }
}
