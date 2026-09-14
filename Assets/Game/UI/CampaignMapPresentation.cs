using UnityEngine;
using UnityEngine.UI;

public sealed class CampaignMapPresentation : MonoBehaviour
{
    Canvas canvas;

    string L(string en, string ru) => GameLanguage.T(en, ru);
    CampaignController Campaign => CampaignController.Instance;

    void Start()
    {
        canvas = FindMenuCanvas();
        if (canvas == null) return;
        Build();
    }

    public void Refresh()
    {
        if (canvas == null) canvas = FindMenuCanvas();
        if (canvas == null) return;
        Build();
    }

    Canvas FindMenuCanvas()
    {
        GameObject menuCanvas = GameObject.Find("MenuCanvas");
        return menuCanvas != null ? menuCanvas.GetComponent<Canvas>() : null;
    }

    void Build()
    {
        Transform levelSelect = canvas.transform.Find("LevelSelect");
        if (levelSelect == null) return;

        Transform levelCard = levelSelect.Find("LevelCard");
        if (levelCard != null) RecomposeLevelCard(levelCard);

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
        shade.color = new Color(.045f, .025f, .015f, .30f);
        shade.raycastTarget = false;

        GameObject mapField = new GameObject("MapField");
        mapField.transform.SetParent(layer.transform, false);
        Image mapFieldImage = mapField.AddComponent<Image>();
        mapFieldImage.color = new Color(.07f, .042f, .025f, .74f);
        mapFieldImage.raycastTarget = false;
        RectTransform field = mapFieldImage.rectTransform;
        field.anchorMin = field.anchorMax = field.pivot = new Vector2(.29f, .5f);
        field.anchoredPosition = Vector2.zero;
        field.sizeDelta = new Vector2(920f, 700f);
        Outline fieldOutline = mapField.AddComponent<Outline>();
        fieldOutline.effectColor = new Color(.58f, .31f, .12f, .38f);
        fieldOutline.effectDistance = new Vector2(2f, -2f);

        Vector2[] points =
        {
            new Vector2(-300, 150),
            new Vector2(-105, 150),
            new Vector2( 100, 125),
            new Vector2( 285,  45),
            new Vector2( 155, -120),
            new Vector2( -65, -155),
            new Vector2(-285, -105)
        };

        int unlockedChapter = Campaign != null ? Campaign.UnlockedChapter : 1;
        for (int i = 0; i < points.Length - 1; i++)
            MakeConnector(mapField.transform, points[i], points[i + 1], i + 1 < unlockedChapter);

        for (int i = 0; i < points.Length; i++)
            MakeNode(mapField.transform, i + 1, points[i]);

        MakeLegend(mapField.transform);
        MakeProgressHeader(mapField.transform);
    }

    void RecomposeLevelCard(Transform levelCard)
    {
        RectTransform card = levelCard as RectTransform;
        if (card == null) return;
        card.anchorMin = card.anchorMax = card.pivot = new Vector2(.82f, .5f);
        card.anchoredPosition = new Vector2(-24f, 0f);
        card.sizeDelta = new Vector2(590f, 720f);

        Image image = levelCard.GetComponent<Image>();
        if (image != null) image.color = new Color(.075f, .042f, .025f, .965f);

        Text[] texts = levelCard.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            RectTransform rt = texts[i].rectTransform;
            string value = texts[i].text;
            if (value.Contains("CHAPTER SELECT") || value.Contains("ВЫБОР ГЛАВЫ"))
            {
                rt.anchoredPosition = new Vector2(0, 280);
                rt.sizeDelta = new Vector2(520, 70);
            }
            else if (value.Contains("Choose where") || value.Contains("Выберите этап"))
            {
                rt.anchoredPosition = new Vector2(0, 225);
                rt.sizeDelta = new Vector2(500, 50);
            }
        }

        Button[] buttons = levelCard.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < buttons.Length; i++)
        {
            RectTransform rt = buttons[i].transform as RectTransform;
            if (rt == null) continue;
            Text label = buttons[i].GetComponentInChildren<Text>(true);
            if (label == null) continue;
            if (label.text.Contains("THE LANDING") || label.text.Contains("ВЫСАДКА"))
            {
                rt.anchoredPosition = new Vector2(0, 82);
                rt.sizeDelta = new Vector2(470, 74);
            }
            else if (label.text.Contains("ROAD TO TROY") || label.text.Contains("ДОРОГА К ТРОЕ"))
            {
                rt.anchoredPosition = new Vector2(0, -18);
                rt.sizeDelta = new Vector2(470, 70);
            }
            else if (label.text == "BACK" || label.text == "НАЗАД")
            {
                rt.anchoredPosition = new Vector2(0, -286);
                rt.sizeDelta = new Vector2(240, 54);
            }
        }
    }

    void MakeNode(Transform parent, int chapter, Vector2 pos)
    {
        bool unlocked = Campaign != null && Campaign.IsChapterUnlocked(chapter);
        ChapterProgress progress = Campaign != null ? Campaign.GetProgress(chapter) : null;
        bool completed = progress != null && progress.completed;

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
        float nodeSize = chapter == 1 ? 92f : 74f;
        rt.sizeDelta = new Vector2(nodeSize, nodeSize);
        image.raycastTarget = false;

        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = unlocked ? new Color(1f, .52f, .16f, .75f) : new Color(.27f, .20f, .15f, .45f);
        outline.effectDistance = new Vector2(2f, -2f);

        MakeText(go.transform, ToRoman(chapter), new Vector2(0, 3), chapter == 1 ? 28 : 22,
            unlocked ? new Color(1f, .84f, .55f, 1f) : new Color(.52f, .46f, .40f, 1f));

        Text titleText = MakeText(parent, ChapterName(chapter), pos + new Vector2(0, -54), 12,
            unlocked ? new Color(.94f, .83f, .67f, 1f) : new Color(.48f, .43f, .38f, .9f));
        titleText.rectTransform.sizeDelta = new Vector2(150f, 34f);

        string state;
        if (completed) state = progress.bestScore > 0 ? L($"BEST {progress.bestScore:N0}", $"ЛУЧШИЙ {progress.bestScore:N0}") : L("COMPLETED", "ПРОЙДЕНО");
        else if (unlocked) state = L("AVAILABLE", "ДОСТУПНО");
        else state = L("LOCKED", "ЗАКРЫТО");
        Text stateText = MakeText(parent, state, pos + new Vector2(0, -78), 10,
            completed ? new Color(1f, .52f, .20f, .95f) : new Color(.67f, .59f, .50f, .85f));
        stateText.rectTransform.sizeDelta = new Vector2(140f, 24f);
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
        Text text = MakeText(parent, L("THE WAR FOR TROY", "ВОЙНА ЗА ТРОЮ"), new Vector2(0, 302), 24,
            new Color(1f, .73f, .34f, .96f));
        text.rectTransform.sizeDelta = new Vector2(440f, 44f);
    }

    void MakeProgressHeader(Transform parent)
    {
        int unlocked = Campaign != null ? Mathf.Clamp(Campaign.UnlockedChapter, 1, 7) : 1;
        int completed = 0;
        if (Campaign != null)
        {
            for (int i = 1; i <= 7; i++)
            {
                ChapterProgress progress = Campaign.GetProgress(i);
                if (progress != null && progress.completed) completed++;
            }
        }
        Text text = MakeText(parent,
            L($"CHAPTER {unlocked}/7  •  COMPLETED {completed}/7", $"ГЛАВА {unlocked}/7  •  ПРОЙДЕНО {completed}/7"),
            new Vector2(0, 265), 13, new Color(.82f, .70f, .56f, .90f));
        text.rectTransform.sizeDelta = new Vector2(440f, 34f);
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
