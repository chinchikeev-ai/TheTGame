using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuController : MonoBehaviour
{
    const string MainMenuBackgroundResource = "Menu/Main_screen";

    static bool openLevelSelectAfterReload;
    static bool startLevelAfterReload;

    Canvas canvas;
    EnemySpawner spawner;
    GameObject mainMenu;
    GameObject levelMenu;
    GameObject settingsMenu;
    GameObject pauseMenu;
    GameObject endMenu;
    Text endTitle;
    Text endSummary;
    Text map2Label;
    Text map2Info;

    bool levelStarted;
    bool paused;
    bool reloading;

    string L(string en, string ru) => GameLanguage.T(en, ru);
    CampaignDifficulty CurrentDifficulty => CampaignController.Instance != null ? CampaignController.Instance.Difficulty : CampaignDifficulty.Story;
    bool IsChapterUnlocked(int chapter) => CampaignController.Instance != null && CampaignController.Instance.IsChapterUnlocked(chapter);

    void Start()
    {
        GameUserSettings.ApplySaved();
        spawner = FindFirstObjectByType<EnemySpawner>();
        BuildUI();

        if (startLevelAfterReload)
        {
            startLevelAfterReload = false;
            StartLevel();
        }
        else if (openLevelSelectAfterReload)
        {
            openLevelSelectAfterReload = false;
            ShowLevels();
        }
        else
        {
            ShowMainMenu();
        }
    }

    void Update()
    {
        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();

        if (levelStarted && GameManager.Instance != null && !GameManager.Instance.GameEnded && GameInput.PausePressed())
            TogglePause();

        if (levelStarted && GameManager.Instance != null && GameManager.Instance.GameEnded && endMenu != null && !endMenu.activeSelf)
            ShowEnd();
    }

    void BuildUI()
    {
        GameObject canvasObj = new GameObject("MenuCanvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;
        canvasObj.AddComponent<GraphicRaycaster>();

        BuildMainMenu();
        BuildLevelMenu();
        BuildSettingsMenu();
        BuildPauseMenu();
        BuildEndMenu();

        levelMenu.SetActive(false);
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(false);
        endMenu.SetActive(false);
    }

    void BuildMainMenu()
    {
        mainMenu = MakeMainMenuScreen();

        GameObject heroPanel = MakePanel(mainMenu.transform, "HeroPanel", new Vector2(.18f, .5f), new Vector2(620, 820), new Color(.055f, .028f, .018f, .74f));
        heroPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(40f, 0f);

        AddTitle(heroPanel.transform, "THE TROY GAME", new Vector2(0, 270), 66, MenuTextStyle.Logo, new Vector2(560, 100));
        AddTitle(heroPanel.transform, "SIEGE DEFENSE", new Vector2(0, 205), 24, MenuTextStyle.Subtitle, new Vector2(560, 50));
        AddDivider(heroPanel.transform, new Vector2(0, 160), 440);
        AddTitle(heroPanel.transform, L("DEFEND TROY • MASTER THE FIRE", "ЗАЩИТИ ТРОЮ • ПОВЕЛЕВАЙ ОГНЁМ"), new Vector2(0, 115), 18, MenuTextStyle.Muted, new Vector2(520, 46));

        AddButton(heroPanel.transform, L("CONTINUE", "ПРОДОЛЖИТЬ"), new Vector2(0, 20), ShowLevels, new Vector2(430, 68), MenuButtonStyle.Highlight);
        AddButton(heroPanel.transform, L("NEW CAMPAIGN", "НОВАЯ КАМПАНИЯ"), new Vector2(0, -64), ShowLevels, new Vector2(430, 62), MenuButtonStyle.Stone);
        AddButton(heroPanel.transform, L("CHAPTER SELECT", "ВЫБОР ГЛАВЫ"), new Vector2(0, -140), ShowLevels, new Vector2(430, 62), MenuButtonStyle.Stone);
        AddButton(heroPanel.transform, L("SETTINGS", "НАСТРОЙКИ"), new Vector2(0, -216), ShowSettingsFromMain, new Vector2(430, 62), MenuButtonStyle.Ghost);
        AddButton(heroPanel.transform, L("EXIT", "ВЫХОД"), new Vector2(0, -292), QuitGame, new Vector2(430, 58), MenuButtonStyle.Ghost);

        AddTitle(mainMenu.transform, L("Troy still stands.", "Троя ещё стоит."), new Vector2(560, -430), 22, MenuTextStyle.Subtitle, new Vector2(620, 50));
        AddTitle(mainMenu.transform, "v0.6 • PRE-ALPHA", new Vector2(770, -500), 16, MenuTextStyle.Muted, new Vector2(280, 38));
    }

    void BuildLevelMenu()
    {
        levelMenu = MakeScreen("LevelSelect", new Color(.025f, .018f, .014f, .95f));
        GameObject panel = MakePanel(levelMenu.transform, "LevelCard", new Vector2(.5f, .5f), new Vector2(980, 700), new Color(.08f, .045f, .025f, .97f));

        AddTitle(panel.transform, L("CHAPTER SELECT", "ВЫБОР ГЛАВЫ"), new Vector2(0, 270), 48, MenuTextStyle.Logo);
        AddTitle(panel.transform, L("Choose where the defense of Troy continues", "Выберите этап обороны Трои"), new Vector2(0, 215), 20, MenuTextStyle.Muted);
        AddDivider(panel.transform, new Vector2(0, 180), 700);

        AddButton(panel.transform, L("I  •  THE LANDING", "I  •  ВЫСАДКА"), new Vector2(0, 90), StartLevel, new Vector2(700, 78), MenuButtonStyle.Highlight);
        Button map2Button = AddButton(panel.transform, "", new Vector2(0, -10), OnMap2Clicked, new Vector2(700, 78), MenuButtonStyle.Stone);
        map2Label = map2Button.GetComponentInChildren<Text>();
        map2Info = AddTitle(panel.transform, "", new Vector2(0, -82), 18, MenuTextStyle.Muted, new Vector2(760, 58));
        AddButton(panel.transform, L("BACK", "НАЗАД"), new Vector2(0, -235), ShowMainMenu, new Vector2(280, 58), MenuButtonStyle.Ghost);
        RefreshLevelSelect();
    }

    void BuildSettingsMenu()
    {
        // ModernSettingsPresentation is the sole visual owner of settings content.
        settingsMenu = MakeScreen("Settings", new Color(.02f, .015f, .012f, .97f));
    }

    void BuildPauseMenu()
    {
        pauseMenu = MakeScreen("PauseMenu", new Color(.02f, .015f, .012f, .93f));
        GameObject panel = MakePanel(pauseMenu.transform, "PauseCard", new Vector2(.5f, .5f), new Vector2(620, 680), new Color(.07f, .04f, .025f, .98f));

        AddTitle(panel.transform, L("PAUSED", "ПАУЗА"), new Vector2(0, 250), 52, MenuTextStyle.Logo);
        AddTitle(panel.transform, L("The battle waits for your command", "Битва ждёт вашего приказа"), new Vector2(0, 202), 18, MenuTextStyle.Muted);
        AddButton(panel.transform, L("RESUME", "ВЕРНУТЬСЯ В ИГРУ"), new Vector2(0, 110), Resume, new Vector2(420, 64), MenuButtonStyle.Highlight);
        AddButton(panel.transform, L("SETTINGS", "НАСТРОЙКИ"), new Vector2(0, 32), ShowSettingsFromPause, new Vector2(420, 60), MenuButtonStyle.Stone);
        AddButton(panel.transform, L("RESTART CHAPTER", "ПЕРЕЗАПУСТИТЬ ГЛАВУ"), new Vector2(0, -46), RestartChapter, new Vector2(420, 60), MenuButtonStyle.Stone);
        AddButton(panel.transform, L("MAIN MENU", "ГЛАВНОЕ МЕНЮ"), new Vector2(0, -124), ReturnToMainMenu, new Vector2(420, 60), MenuButtonStyle.Ghost);
        AddButton(panel.transform, L("EXIT", "ВЫХОД"), new Vector2(0, -202), QuitGame, new Vector2(420, 56), MenuButtonStyle.Ghost);
    }

    void BuildEndMenu()
    {
        endMenu = MakeScreen("EndMenu", new Color(.02f, .015f, .012f, .95f));
        GameObject panel = MakePanel(endMenu.transform, "ResultCard", new Vector2(.5f, .5f), new Vector2(980, 820), new Color(.07f, .04f, .025f, .98f));

        endTitle = AddTitle(panel.transform, L("RESULT", "РЕЗУЛЬТАТ"), new Vector2(0, 320), 58, MenuTextStyle.Logo);
        endSummary = AddTitle(panel.transform, "", new Vector2(0, 80), 22, MenuTextStyle.Normal, new Vector2(840, 420));
        AddButton(panel.transform, L("RETRY", "ПОВТОРИТЬ"), new Vector2(-175, -300), RestartChapter, new Vector2(300, 64), MenuButtonStyle.Highlight);
        AddButton(panel.transform, L("CHAPTER SELECT", "ВЫБОР ГЛАВЫ"), new Vector2(175, -300), ReturnToMainMenu, new Vector2(300, 64), MenuButtonStyle.Stone);
    }

    void ToggleLanguage()
    {
        bool wasSettings = settingsMenu != null && settingsMenu.activeSelf;
        bool wasPause = pauseMenu != null && pauseMenu.activeSelf;
        bool wasLevels = levelMenu != null && levelMenu.activeSelf;

        GameLanguage.Toggle();
        RuntimeFileLogger.Event("LANGUAGE", $"Changed language to {GameLanguage.Code}");
        RebuildUiAfterLanguageChange(wasSettings, wasPause, wasLevels);
    }

    void RebuildUiAfterLanguageChange(bool wasSettings, bool wasPause, bool wasLevels)
    {
        GameObject oldCanvas = canvas != null ? canvas.gameObject : null;
        BuildUI();

        if (wasSettings)
        {
            mainMenu.SetActive(false);
            levelMenu.SetActive(false);
            pauseMenu.SetActive(false);
            settingsMenu.SetActive(true);
        }
        else if (wasPause)
        {
            mainMenu.SetActive(false);
            levelMenu.SetActive(false);
            settingsMenu.SetActive(false);
            pauseMenu.SetActive(true);
        }
        else if (wasLevels)
        {
            mainMenu.SetActive(false);
            settingsMenu.SetActive(false);
            pauseMenu.SetActive(false);
            levelMenu.SetActive(true);
        }
        else
        {
            ShowMainMenu();
        }

        if (oldCanvas != null) Destroy(oldCanvas);
    }

    void RefreshLevelSelect()
    {
        if (map2Label == null) return;

        map2Label.text = IsChapterUnlocked(2)
            ? L("II  •  ROAD TO TROY  •  IN PRODUCTION", "II  •  ДОРОГА К ТРОЕ  •  В РАЗРАБОТКЕ")
            : L("II  •  ROAD TO TROY  •  LOCKED", "II  •  ДОРОГА К ТРОЕ  •  ЗАКРЫТА");

        if (map2Info != null) map2Info.text = "";
    }

    void OnMap2Clicked()
    {
        if (map2Info != null)
        {
            map2Info.text = IsChapterUnlocked(2)
                ? L("Chapter II is unlocked, but this build is focused on polishing Chapter I.", "Глава II открыта, но эта сборка пока доводит Главу I.")
                : L("Complete Chapter I to unlock the road to Troy.", "Пройдите Главу I, чтобы открыть дорогу к Трое.");
        }

        RuntimeFileLogger.Event("CAMPAIGN", "Chapter II selected but content is not implemented yet");
    }

    void StartLevel()
    {
        levelStarted = true;
        paused = false;
        CombatControlsUI.ResumeConfiguredSpeed();
        GameManager.Instance?.BeginRun();
        mainMenu.SetActive(false);
        levelMenu.SetActive(false);
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(false);
        endMenu.SetActive(false);
        spawner?.ActivateLevel();
    }

    void ShowMainMenu()
    {
        Time.timeScale = 0f;
        paused = false;
        RefreshLevelSelect();
        SetScreenState(main: true);
    }

    void ReturnToMainMenu()
    {
        if (!levelStarted)
        {
            ShowLevels();
            return;
        }

        RuntimeFileLogger.Event("MENU", "Returning to main menu through clean scene reset");
        if (GameManager.Instance != null && GameManager.Instance.GameEnded)
            openLevelSelectAfterReload = true;
        RestartScene();
    }

    void ShowLevels()
    {
        RefreshLevelSelect();
        SetScreenState(levels: true);
    }

    void TogglePause()
    {
        if (paused) Resume(); else Pause();
    }

    void Pause()
    {
        paused = true;
        Time.timeScale = 0f;
        if (pauseMenu != null) pauseMenu.SetActive(true);
    }

    void Resume()
    {
        paused = false;
        CombatControlsUI.ResumeConfiguredSpeed();
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (settingsMenu != null) settingsMenu.SetActive(false);
    }

    void ShowSettingsFromMain()
    {
        if (mainMenu != null) mainMenu.SetActive(false);
        if (settingsMenu != null) settingsMenu.SetActive(true);
    }

    void ShowSettingsFromPause()
    {
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (settingsMenu != null) settingsMenu.SetActive(true);
    }

    // Called by ModernSettingsPresentation via SendMessage. Keep method name stable.
    void BackFromSettings()
    {
        if (settingsMenu != null) settingsMenu.SetActive(false);
        if (levelStarted && paused)
        {
            if (pauseMenu != null) pauseMenu.SetActive(true);
        }
        else if (mainMenu != null)
        {
            mainMenu.SetActive(true);
        }
    }

    void ShowEnd()
    {
        Time.timeScale = 0f;
        paused = true;
        GameManager gm = GameManager.Instance;
        bool victory = gm.EndMessage == "VICTORY";
        endTitle.text = victory ? L("VICTORY", "ПОБЕДА") : L("GAME OVER", "ПОРАЖЕНИЕ");

        int totalSeconds = Mathf.RoundToInt(gm.RunTime);
        int min = totalSeconds / 60;
        int sec = totalSeconds % 60;
        string unlock = victory && IsChapterUnlocked(2) ? "\n" + L("CHAPTER II UNLOCKED", "ГЛАВА II ОТКРЫТА") : "";

        if (victory && IsChapterUnlocked(2))
            unlock += "\n" + L("Chapter II is in production. Return to Chapter Select.", "Глава II в разработке. Вернитесь к выбору главы.");

        endSummary.text =
            $"{L("MAP", "КАРТА")} {gm.MapNumber}    {L("DIFFICULTY", "СЛОЖНОСТЬ")}: {DifficultyRules.Label(CurrentDifficulty)}\n" +
            $"{L("WAVES", "ВОЛНЫ")}: {gm.CurrentWave}/{gm.MaxWaves}    {L("TIME", "ВРЕМЯ")}: {min:00}:{sec:00}\n\n" +
            $"{L("SCORE", "СЧЁТ")}: {gm.FinalScore}\n" +
            $"{L("KILLS", "УБИТО")}: {gm.Kills}    {L("LEAKS", "ПРОПУЩЕНО")}: {gm.Leaks}\n" +
            $"{L("GOLD EARNED", "ЗОЛОТО ПОЛУЧЕНО")}: {gm.GoldEarned}    {L("SPENT", "ПОТРАЧЕНО")}: {gm.GoldSpent}\n" +
            $"{L("TOWERS BUILT", "ПОСТРОЕНО БАШЕН")}: {gm.TowersBuilt}    {L("SOLD", "ПРОДАНО")}: {gm.TowersSold}\n" +
            $"{L("GATE HP", "HP ВОРОТ")}: {gm.BaseHealth}/{gm.MaxBaseHealth}{unlock}";

        if (endMenu != null) endMenu.SetActive(true);
    }

    public void RestartScene()
    {
        if (reloading) return;
        reloading = true;
        Scene scene = SceneManager.GetActiveScene();
        RuntimeFileLogger.Event("MENU", $"Reloading scene buildIndex={scene.buildIndex}, name={scene.name}");
        Time.timeScale = 1f;
        paused = false;

        if (canvas != null)
        {
            foreach (Button button in canvas.GetComponentsInChildren<Button>(true))
                button.interactable = false;
        }

        int buildIndex = scene.buildIndex >= 0 ? scene.buildIndex : 0;
        SceneManager.LoadScene(buildIndex, LoadSceneMode.Single);
    }

    public void RestartChapter()
    {
        startLevelAfterReload = true;
        RestartScene();
    }

    void QuitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        Debug.Log("EXIT requested. Application.Quit() is ignored inside the Unity Editor.");
#else
        Application.Quit();
#endif
    }

    void SetScreenState(bool main = false, bool levels = false, bool settings = false, bool pause = false, bool end = false)
    {
        if (mainMenu != null) mainMenu.SetActive(main);
        if (levelMenu != null) levelMenu.SetActive(levels);
        if (settingsMenu != null) settingsMenu.SetActive(settings);
        if (pauseMenu != null) pauseMenu.SetActive(pause);
        if (endMenu != null) endMenu.SetActive(end);
    }

    GameObject MakeScreen(string name, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(canvas.transform, false);
        Image image = go.AddComponent<Image>();
        image.color = color;
        StretchToParent(image.rectTransform);
        return go;
    }

    GameObject MakeMainMenuScreen()
    {
        GameObject go = new GameObject("MainMenu");
        go.transform.SetParent(canvas.transform, false);
        RectTransform root = go.AddComponent<RectTransform>();
        StretchToParent(root);

        Texture2D background = Resources.Load<Texture2D>(MainMenuBackgroundResource);
        if (background != null)
        {
            GameObject backgroundObject = new GameObject("Background");
            backgroundObject.transform.SetParent(go.transform, false);
            Image backgroundImage = backgroundObject.AddComponent<Image>();
            backgroundImage.sprite = Sprite.Create(background, new Rect(0f, 0f, background.width, background.height), new Vector2(.5f, .5f));
            backgroundImage.type = Image.Type.Simple;
            StretchToParent(backgroundImage.rectTransform);

            AspectRatioFitter fitter = backgroundObject.AddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            fitter.aspectRatio = (float)background.width / background.height;
        }

        GameObject shade = new GameObject("CinematicShade");
        shade.transform.SetParent(go.transform, false);
        Image shadeImage = shade.AddComponent<Image>();
        shadeImage.color = new Color(.018f, .008f, .004f, .29f);
        StretchToParent(shadeImage.rectTransform);
        return go;
    }

    GameObject MakePanel(Transform parent, string name, Vector2 anchor, Vector2 size, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = color;

        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = anchor;
        rect.sizeDelta = size;

        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(.73f, .43f, .16f, .45f);
        outline.effectDistance = new Vector2(2f, -2f);
        return go;
    }

    static void StretchToParent(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }

    Text AddTitle(Transform parent, string value, Vector2 position, int size, MenuTextStyle style = MenuTextStyle.Normal, Vector2? customSize = null)
    {
        GameObject go = new GameObject(string.IsNullOrEmpty(value) ? "Text" : value);
        go.transform.SetParent(parent, false);
        Text text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = size;
        text.fontStyle = style == MenuTextStyle.Muted ? FontStyle.Normal : FontStyle.Bold;
        text.color = TextColor(style);
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;

        RectTransform rect = text.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = customSize ?? new Vector2(1000, 80);
        return text;
    }

    void AddDivider(Transform parent, Vector2 position, float width)
    {
        GameObject go = new GameObject("Divider");
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = new Color(.82f, .49f, .19f, .55f);

        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(width, 2f);
    }

    Button AddButton(Transform parent, string label, Vector2 position, UnityEngine.Events.UnityAction action, Vector2? customSize = null, MenuButtonStyle style = MenuButtonStyle.Default)
    {
        GameObject go = new GameObject(string.IsNullOrEmpty(label) ? "Button" : label);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = ButtonColor(style);

        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.transition = Selectable.Transition.ColorTint;

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, .95f, .86f, 1f);
        colors.pressedColor = new Color(.78f, .58f, .42f, 1f);
        colors.selectedColor = new Color(1f, .91f, .76f, 1f);
        colors.disabledColor = new Color(.45f, .45f, .45f, .65f);
        colors.fadeDuration = .12f;
        button.colors = colors;
        button.onClick.AddListener(action);

        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = customSize ?? new Vector2(430, 64);

        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = style == MenuButtonStyle.Highlight ? new Color(1f, .52f, .15f, .75f) : new Color(.62f, .38f, .18f, .35f);
        outline.effectDistance = new Vector2(1f, -1f);

        Text text = AddTitle(go.transform, label, Vector2.zero, 21, MenuTextStyle.Button);
        text.color = style == MenuButtonStyle.Highlight
            ? new Color(1f, .90f, .55f, 1f)
            : style == MenuButtonStyle.Ghost
                ? new Color(.93f, .78f, .60f, 1f)
                : new Color(.17f, .07f, .03f, 1f);

        RectTransform textRect = text.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = textRect.offsetMax = Vector2.zero;
        textRect.pivot = new Vector2(.5f, .5f);
        return button;
    }

    static Color TextColor(MenuTextStyle style)
    {
        switch (style)
        {
            case MenuTextStyle.Logo: return new Color(1f, .62f, .18f, 1f);
            case MenuTextStyle.Subtitle: return new Color(1f, .84f, .56f, 1f);
            case MenuTextStyle.Muted: return new Color(.82f, .72f, .62f, .92f);
            default: return new Color(.97f, .92f, .84f, 1f);
        }
    }

    static Color ButtonColor(MenuButtonStyle style)
    {
        switch (style)
        {
            case MenuButtonStyle.Highlight: return new Color(.58f, .105f, .055f, .98f);
            case MenuButtonStyle.Stone: return new Color(.72f, .56f, .39f, .98f);
            case MenuButtonStyle.Ghost: return new Color(.10f, .055f, .03f, .82f);
            default: return new Color(.32f, .18f, .09f, .98f);
        }
    }

    enum MenuTextStyle { Normal, Logo, Subtitle, Button, Muted }
    enum MenuButtonStyle { Default, Stone, Highlight, Ghost }
}
