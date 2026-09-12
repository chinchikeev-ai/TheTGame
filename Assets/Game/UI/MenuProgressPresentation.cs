using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class MenuProgressPresentation : MonoBehaviour
{
    Canvas canvas;

    string L(string en, string ru) => GameLanguage.T(en, ru);

    void Start()
    {
        canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;
        RefreshMainMenuProgress();
        RefreshChapterSelect();
    }

    public void RefreshAll()
    {
        if (canvas == null) canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;
        RefreshMainMenuProgress();
        RefreshChapterSelect();
    }

    void RefreshMainMenuProgress()
    {
        Transform mainMenu = canvas.transform.Find("MainMenu");
        if (mainMenu == null) return;

        Transform heroPanel = mainMenu.Find("HeroPanel");
        if (heroPanel == null) return;

        Transform old = heroPanel.Find("CampaignProgressSummary");
        if (old != null) Destroy(old.gameObject);

        CampaignSaveData save = CampaignSave.Data;
        bool hasProgress = HasProgress(save);
        GameObject root = new GameObject("CampaignProgressSummary");
        root.transform.SetParent(heroPanel, false);
        RectTransform rr = root.AddComponent<RectTransform>();
        rr.anchorMin = rr.anchorMax = rr.pivot = new Vector2(.5f, .5f);
        rr.anchoredPosition = new Vector2(0f, 72f);
        rr.sizeDelta = new Vector2(500f, 64f);

        if (!hasProgress)
        {
            MakeText(root.transform, L("No campaign in progress", "Кампания ещё не начата"), Vector2.zero, 16,
                new Color(.78f, .68f, .58f, .85f));
            return;
        }

        int chapter = Mathf.Clamp(save.unlockedChapter, 1, 7);
        int score = 0;
        if (save.chapters != null)
            foreach (ChapterProgress progress in save.chapters)
                if (progress != null) score += progress.bestScore;

        string line1 = L($"Continue from Chapter {chapter}", $"Продолжить с главы {chapter}");
        string line2 = L($"Campaign score {score:N0}", $"Счёт кампании {score:N0}");
        MakeText(root.transform, line1, new Vector2(0, 13), 17, new Color(1f, .84f, .56f, 1f));
        MakeText(root.transform, line2, new Vector2(0, -14), 14, new Color(.78f, .68f, .58f, .9f));
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
        sr.sizeDelta = new Vector2(720, 70);

        ChapterProgress chapter1 = CampaignSave.GetChapterProgress(1);
        string best = chapter1.bestScore > 0 ? chapter1.bestScore.ToString("N0") : "—";
        string completions = chapter1.completions > 0 ? chapter1.completions.ToString() : "0";
        string text = L($"CHAPTER I  •  BEST SCORE {best}  •  COMPLETIONS {completions}",
                        $"ГЛАВА I  •  ЛУЧШИЙ СЧЁТ {best}  •  ПРОХОЖДЕНИЙ {completions}");
        MakeText(stats.transform, text, Vector2.zero, 16, new Color(.82f, .72f, .62f, .92f));
    }

    bool HasProgress(CampaignSaveData save)
    {
        if (save == null) return false;
        if (save.unlockedChapter > 1) return true;
        if (save.finalResult != null && save.finalResult.completed) return true;
        if (save.chapters == null) return false;
        foreach (ChapterProgress chapter in save.chapters)
            if (chapter != null && (chapter.completed || chapter.bestScore > 0 || chapter.completions > 0)) return true;
        return false;
    }

    Text MakeText(Transform parent, string value, Vector2 pos, int fontSize, Color color)
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
        RectTransform rt = text.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(700, 30);
        return text;
    }
}
