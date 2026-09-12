using UnityEngine;
using UnityEngine.UI;

public class ChapterFlowUI : MonoBehaviour
{
    Text objectiveText;
    Text tutorialText;
    Canvas canvas;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<ChapterFlowUI>() == null)
            new GameObject("ChapterFlowUI").AddComponent<ChapterFlowUI>();
    }

    void Start()
    {
        BuildUI();
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || gm.Chapter == null) return;
        ChapterData chapter = gm.Chapter;
        int wave = Mathf.Clamp(gm.CurrentWave, 0, chapter.combatEvents);

        string[] objectives = GameLanguage.Russian ? chapter.objectiveRussian : chapter.objectiveEnglish;
        string[] tutorials = GameLanguage.Russian ? chapter.tutorialRussian : chapter.tutorialEnglish;

        if (objectives != null && objectives.Length > 0)
        {
            int oi = wave >= chapter.combatEvents ? Mathf.Min(2, objectives.Length - 1) : wave >= 3 ? Mathf.Min(1, objectives.Length - 1) : 0;
            objectiveText.text = GameLanguage.T("OBJECTIVE: ", "ЦЕЛЬ: ") + objectives[oi];
        }

        bool showTutorial = !gm.GameEnded && wave <= 2;
        tutorialText.gameObject.SetActive(showTutorial);
        if (showTutorial && tutorials != null && tutorials.Length > 0)
        {
            int ti = wave <= 0 ? 0 : Mathf.Min(wave, tutorials.Length - 1);
            tutorialText.text = GameLanguage.T("TIP: ", "ПОДСКАЗКА: ") + tutorials[ti];
        }
    }

    void BuildUI()
    {
        GameObject go = new GameObject("ChapterFlowCanvas");
        go.transform.SetParent(transform, false);
        canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 15;
        CanvasScaler scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        objectiveText = MakeText(canvas.transform, new Vector2(24, -118), new Vector2(900, 42), 20);
        tutorialText = MakeText(canvas.transform, new Vector2(24, -160), new Vector2(1050, 72), 18);
    }

    Text MakeText(Transform parent, Vector2 pos, Vector2 size, int fontSize)
    {
        GameObject go = new GameObject("ChapterText");
        go.transform.SetParent(parent, false);
        Text t = go.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = fontSize;
        t.fontStyle = FontStyle.Bold;
        t.color = Color.white;
        t.alignment = TextAnchor.UpperLeft;
        RectTransform rt = t.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return t;
    }
}
