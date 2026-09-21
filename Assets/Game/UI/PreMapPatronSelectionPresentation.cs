using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public sealed class PreMapPatronSelectionPresentation : MonoBehaviour
{
    GameMenuController menu;
    Canvas menuCanvas;
    GameObject difficultyOverlay;
    GameObject patronOverlay;
    Text patronDifficultyLabel;
    Button chapterOneButton;
    bool bound;

    IEnumerator Start()
    {
        while (!bound)
        {
            bound = TryBind();
            if (!bound) yield return null;
        }
    }

    void Update()
    {
        if (!bound || menu == null || menuCanvas == null)
        {
            bound = TryBind();
            return;
        }

        GameManager gm = GameManager.Instance;
        if (GameMenuController.QuitRequested)
        {
            HideAll();
            return;
        }
        if (gm == null || gm.GameEnded || gm.GiftSelected) return;

        if (Time.timeScale > 0f)
            ShowDifficulty();
    }

    bool TryBind()
    {
        menu = GameMenuController.Instance;
        menuCanvas = menu != null ? menu.MenuCanvas : null;
        if (menu == null || menuCanvas == null) return false;

        Transform levelSelect = menuCanvas.transform.Find("LevelSelect");
        if (levelSelect == null) return false;

        if (difficultyOverlay == null) BuildDifficultyOverlay();
        if (patronOverlay == null) BuildPatronOverlay();

        return BindChapterAction(levelSelect) && difficultyOverlay != null && patronOverlay != null;
    }

    bool BindChapterAction(Transform levelSelect)
    {
        if (chapterOneButton == null)
        {
            var artwork = levelSelect.GetComponent<ChapterSelectionArtwork>();
            if (artwork != null)
            {
                chapterOneButton = artwork.StartButton;
                artwork.SetStartAction(ShowDifficulty);
                return chapterOneButton != null;
            }
            Button[] buttons = levelSelect.GetComponentsInChildren<Button>(true);
            for (int i = 0; i < buttons.Length; i++)
            {
                Text label = buttons[i].GetComponentInChildren<Text>(true);
                if (label == null) continue;
                if (!label.text.Contains("THE LANDING") && !label.text.Contains("ВЫСАДКА")) continue;
                chapterOneButton = buttons[i];
                chapterOneButton.onClick.RemoveAllListeners();
                chapterOneButton.onClick.AddListener(ShowDifficulty);
                break;
            }
        }

        return chapterOneButton != null;
    }

    void BuildDifficultyOverlay()
    {
        difficultyOverlay = MakeOverlay("PreMapDifficultySelection");
        DifficultySelectionArtwork artwork = difficultyOverlay.AddComponent<DifficultySelectionArtwork>();
        CampaignDifficulty current = CampaignController.Instance != null
            ? CampaignController.Instance.Difficulty
            : CampaignDifficulty.Story;
        artwork.Build(HideDifficulty, ConfirmDifficulty, current);
        difficultyOverlay.SetActive(false);
    }

    void BuildPatronOverlay()
    {
        patronOverlay = MakeOverlay("PreMapPatronSelection");
        PatronSelectionArtwork artwork = patronOverlay.AddComponent<PatronSelectionArtwork>();
        artwork.Build(BackToDifficulty, Choose);
        patronDifficultyLabel = artwork.DifficultyLabel;
        patronOverlay.SetActive(false);
    }

    GameObject MakeOverlay(string name)
    {
        GameObject overlay = new GameObject(name);
        overlay.transform.SetParent(menuCanvas.transform, false);
        Image backdrop = overlay.AddComponent<Image>();
        backdrop.color = new Color(.015f, .009f, .006f, .985f);
        RectTransform rect = backdrop.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        return overlay;
    }

    void ShowDifficulty()
    {
        if (GameMenuController.QuitRequested) return;
        GameManager gm = GameManager.Instance;
        if (gm != null && gm.GiftSelected)
        {
            StartMap();
            return;
        }

        Time.timeScale = 0f;
        if (patronOverlay != null) patronOverlay.SetActive(false);
        if (difficultyOverlay != null)
        {
            difficultyOverlay.transform.SetAsLastSibling();
            difficultyOverlay.SetActive(true);
        }
        RuntimeFileLogger.Event("MENU", "Pre-map difficulty selection opened");
    }

    void HideDifficulty()
    {
        if (difficultyOverlay != null) difficultyOverlay.SetActive(false);
    }

    void ConfirmDifficulty(CampaignDifficulty difficulty)
    {
        CampaignController.Instance?.SetDifficulty(difficulty);
        GameManager.Instance?.ApplyDifficultyBeforeRun(difficulty);
        EnemySpawner.Instance?.RefreshPreRunDifficulty();
        RuntimeFileLogger.Event("MENU", $"Pre-map difficulty confirmed: {difficulty}");
        HideDifficulty();
        ShowPatron();
    }

    void ShowPatron()
    {
        if (GameMenuController.QuitRequested) return;
        GameManager gm = GameManager.Instance;
        if (gm != null && gm.GiftSelected)
        {
            StartMap();
            return;
        }

        Time.timeScale = 0f;
        if (difficultyOverlay != null) difficultyOverlay.SetActive(false);
        if (patronDifficultyLabel != null)
        {
            CampaignDifficulty difficulty = CampaignController.Instance != null
                ? CampaignController.Instance.Difficulty
                : CampaignDifficulty.Story;
            patronDifficultyLabel.text = GameLanguage.T("Difficulty: ", "Сложность: ") + DifficultyLabel(difficulty);
        }
        if (patronOverlay != null)
        {
            patronOverlay.GetComponent<PatronSelectionArtwork>()?.ResetSelection();
            patronOverlay.transform.SetAsLastSibling();
            patronOverlay.SetActive(true);
        }
        RuntimeFileLogger.Event("PATRON", "Pre-map patron selection opened");
    }

    string DifficultyLabel(CampaignDifficulty difficulty)
    {
        switch (difficulty)
        {
            case CampaignDifficulty.Story: return GameLanguage.T("STORY", "ИСТОРИЯ");
            case CampaignDifficulty.Legendary: return GameLanguage.T("LEGENDARY", "ЛЕГЕНДА");
            default: return GameLanguage.T("STRATEGOS", "СТРАТЕГ");
        }
    }

    void BackToDifficulty()
    {
        if (patronOverlay != null) patronOverlay.SetActive(false);
        ShowDifficulty();
    }

    void Choose(DivineGiftType gift)
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || !gm.UseGift(gift)) return;
        RuntimeFileLogger.Event("PATRON", $"Pre-map patron confirmed: {gift}");
        HideAll();
        StartMap();
    }

    void HideAll()
    {
        if (difficultyOverlay != null) difficultyOverlay.SetActive(false);
        if (patronOverlay != null) patronOverlay.SetActive(false);
    }

    void StartMap()
    {
        if (menu == null) return;
        MethodInfo startLevel = typeof(GameMenuController).GetMethod("StartLevel", BindingFlags.Instance | BindingFlags.NonPublic);
        if (startLevel == null)
        {
            RuntimeFileLogger.Event("PATRON", "Could not locate GameMenuController.StartLevel; map start aborted.");
            ShowDifficulty();
            return;
        }
        startLevel.Invoke(menu, null);
    }


}
