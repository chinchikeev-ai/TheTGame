using UnityEngine;
using UnityEngine.UI;

public class CampaignProgressUI : MonoBehaviour
{
    Canvas canvas;
    Text resultText;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<CampaignProgressUI>() == null)
            new GameObject("CampaignProgressUI").AddComponent<CampaignProgressUI>();
    }

    void Start()
    {
        GameObject go = new GameObject("CampaignProgressCanvas");
        go.transform.SetParent(transform, false);
        canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 120;
        CanvasScaler scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        resultText = MakeText(canvas.transform);
        resultText.gameObject.SetActive(false);
    }

    void Update()
    {
        RefreshLevelSelectLabels();
        GameManager gm = GameManager.Instance;
        bool show = gm != null && gm.GameEnded;
        resultText.gameObject.SetActive(show);
        if (!show) return;

        bool victory = gm.EndMessage == "VICTORY";
        string unlock = victory && CampaignSave.IsUnlocked(2)
            ? GameLanguage.T("CHAPTER II UNLOCKED", "ГЛАВА II ОТКРЫТА")
            : "";
        resultText.text = $"{GameLanguage.T("SCORE", "СЧЁТ")}: {gm.FinalScore}\n{unlock}";
    }

    void RefreshLevelSelectLabels()
    {
        if (!CampaignSave.IsUnlocked(2)) return;
        foreach (Text text in FindObjectsByType<Text>(FindObjectsSortMode.None))
        {
            if (text == null || string.IsNullOrEmpty(text.text)) continue;
            if (text.text.Contains("MAP 2 - LOCKED")) text.text = "MAP 2 - ROAD TO TROY • UNLOCKED";
            else if (text.text.Contains("КАРТА 2 - ЗАКРЫТА")) text.text = "КАРТА 2 - ДОРОГА К ТРОЕ • ОТКРЫТА";
        }
    }

    Text MakeText(Transform parent)
    {
        GameObject go = new GameObject("CampaignResult");
        go.transform.SetParent(parent, false);
        Text t = go.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = 28;
        t.fontStyle = FontStyle.Bold;
        t.color = Color.white;
        t.alignment = TextAnchor.MiddleCenter;
        RectTransform rt = t.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = new Vector2(0, -105);
        rt.sizeDelta = new Vector2(850, 100);
        return t;
    }
}
