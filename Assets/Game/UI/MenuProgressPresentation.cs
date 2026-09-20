using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class MenuProgressPresentation : MonoBehaviour
{
    Canvas canvas;

    string L(string en, string ru) => GameLanguage.T(en, ru);

    void Start()
    {
        canvas = FindMenuCanvas();
        if (canvas == null) return;
        RefreshMainMenuProgress();
        RefreshChapterSelect();
    }

    public void RefreshAll()
    {
        if (canvas == null) canvas = FindMenuCanvas();
        if (canvas == null) return;
        RefreshMainMenuProgress();
        RefreshChapterSelect();
    }

    Canvas FindMenuCanvas()
    {
        return GameMenuController.Instance != null ? GameMenuController.Instance.MenuCanvas : null;
    }

    void RefreshMainMenuProgress()
    {
        Transform mainMenu = canvas.transform.Find("MainMenu");
        if (mainMenu == null) return;
        Transform mainPanel = mainMenu.Find("MainPanel");
        Transform heroPanel = mainPanel != null ? mainPanel.Find("HeroPanel") : mainMenu.Find("HeroPanel");
        if (heroPanel == null) return;
        Transform old = heroPanel.Find("CampaignProgressSummary");
        if (old != null) Destroy(old.gameObject);

        CampaignController campaign = CampaignController.Instance;
        bool hasProgress = campaign != null && campaign.HasProgress;
        GameObject root = new GameObject("CampaignProgressSummary");
        root.transform.SetParent(heroPanel, false);
        RectTransform rr = root.AddComponent<RectTransform>();
        rr.anchorMin = rr.anchorMax = rr.pivot = new Vector2(.5f, .5f);
        rr.anchoredPosition = new Vector2(0f, -32f);
        rr.sizeDelta = new Vector2(470f, 118f);

        Image bg = root.AddComponent<Image>();
        bg.color = new Color(.075f, .042f, .026f, .78f);
        Outline outline = root.AddComponent<Outline>();
        outline.effectColor = new Color(.55f, .31f, .12f, .32f);
        outline.effectDistance = new Vector2(1f, -1f);

        if (!hasProgress)
        {
            MakeText(root.transform, L("CAMPAIGN", "КАМПАНИЯ"), new Vector2(0, 24), 13,
                new Color(1f, .70f, .28f, 1f), 430f);
            MakeText(root.transform, L("No campaign in progress", "Кампания ещё не начата"), new Vector2(0, -16), 16,
                new Color(.84f, .75f, .66f, .92f), 430f);
            return;
        }

        int chapter = Mathf.Clamp(campaign.UnlockedChapter, 1, 7);
        int score = campaign.TotalBestScore;
        string line1 = L($"Continue from Chapter {chapter}", $"Продолжить с главы {chapter}");
        string line2 = L($"Campaign score {score:N0}", $"Счёт кампании {score:N0}");
        MakeText(root.transform, line1, new Vector2(0, 22), 17, new Color(1f, .84f, .56f, 1f), 430f);
        MakeText(root.transform, line2, new Vector2(0, -18), 14, new Color(.78f, .68f, .58f, .9f), 430f);
    }

    void RefreshChapterSelect()
    {
        Transform levelSelect = canvas.transform.Find("LevelSelect");
        if (levelSelect == null) return;
        Transform card = levelSelect.Find("LevelCard");
        if (card == null) return;
        Transform old = card.Find("CampaignStats");
        if (old != null) Destroy(old.gameObject);

        GameObject stats = new GameObject("CampaignStats");
        stats.transform.SetParent(card, false);
        RectTransform sr = stats.AddComponent<RectTransform>();
        sr.anchorMin = sr.anchorMax = sr.pivot = new Vector2(.5f, .5f);
        sr.anchoredPosition = new Vector2(0, -155);
        sr.sizeDelta = new Vector2(500, 62);

        CampaignController campaign = CampaignController.Instance;
        ChapterProgress chapter1 = campaign != null ? campaign.GetProgress(1) : null;
        int bestValue = chapter1 != null ? chapter1.bestScore : 0;
        int completionValue = chapter1 != null ? chapter1.completions : 0;
        string best = bestValue > 0 ? bestValue.ToString("N0") : "—";
        string completions = completionValue > 0 ? completionValue.ToString() : "0";
        string text = L($"CHAPTER I  •  BEST {best}  •  CLEARS {completions}",
                        $"ГЛАВА I  •  ЛУЧШИЙ {best}  •  ПРОХОЖДЕНИЙ {completions}");
        MakeText(stats.transform, text, Vector2.zero, 14, new Color(.82f, .72f, .62f, .92f), 500f);
    }

    Text MakeText(Transform parent, string value, Vector2 pos, int fontSize, Color color, float width)
    {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(parent, false);
        Text text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;
        RectTransform rt = text.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(width, 30);
        return text;
    }
}
