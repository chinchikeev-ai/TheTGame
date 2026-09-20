using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameMenuUiFactory;

public class GameMenuController : MonoBehaviour
{
    public static GameMenuController Instance { get; private set; }

    const string MainMenuBackgroundResource = "Menu/Main_screen";

    static bool openLevelSelectAfterReload;
    static bool startLevelAfterReload;
    static bool restartGiftPending;
    static DivineGiftType restartGift;
    public static bool QuitRequested { get; private set; }

    Canvas canvas;
    public Canvas MenuCanvas => canvas;
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

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        QuitRequested = false;
        GameUserSettings.ApplySaved();
        spawner = EnemySpawner.Instance
        BuildUI();

        if (startLevelAfterReload)
        {
            startLevelAfterReload = false;
            openLevelSelectAfterReload = false;

            GameManager gm = GameManager.Instance;
            if (restartGiftPending && gm != null && !gm.GiftSelected)
            {
                gm.UseGift(restartGift);
                RuntimeFileLogger.Event("MENU", $"Restart restored patron={restartGift} before level start");
            }
            restartGiftPending = false;
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
        if (spawner == null) spawner = EnemySpawner.Instance;

        if (levelStarted && !paused && GameManager.Instance != null && !GameManager.Instance.GameEnded && Time.timeScale <= 0f)
            CombatControlsUI.ResumeConfiguredSpeed();

        if (levelStarted && GameManager.Instance != null && !GameManager.Instance.GameEnded && GameInput.PausePressed())
            TogglePause();

        if (levelStarted && GameManager.Instance != null && GameManager.Instance.GameEnded && endMenu != null && !endMenu.activeSelf)
            ShowEnd();
    }

    void BuildUI()
    {
        if (canvas == null)
        {
            GameObject stale = GameObject.Find("MenuCanvas");
            if (stale != null)
            {
                stale.SetActive(false);
                Destroy(stale);
            }
        }

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
        mainMenu = MakeMainMenuScreen(canvas, MainMenuBackgroundResource);

        GameObject panel = MakePanel(
            mainMenu.transform,
            "MainPanel",
            new Vector2(.76f, .50f),
            new Vector2(700f, 980f),
            new Color(.035f, .018f, .010f, .55f));

        Image panelImage = panel.GetComponent<Image>();
        if (panelImage != null) panelImage.raycastTarget = false;

        AddTitle(panel.transform, "THE TROY GAME", new Vector2(0f, 350f), 62, MenuTextStyle.Logo, new Vector2(640f, 86f));
        AddTitle(panel.transform, "GODS DEFENSE", new Vector2(0f, 285f), 26, MenuTextStyle.Subtitle, new Vector2(600f, 46f));
        AddTitle(
            panel.transform,
            L("DEFEND TROY. BREAK THE LANDING.", "ЗАЩИТИ ТРОЮ. СОРВИ ВЫСАДКУ."),
            new Vector2(0f, 228f),
            17,
            MenuTextStyle.Muted,
            new Vector2(610f, 42f));

        AddButton(panel.transform, L("CONTINUE", "ПРОДОЛЖИТЬ"), new Vector2(0f, 112f), ShowLevels, new Vector2(560f, 86f), MenuButtonStyle.Highlight, "Menu/Buttons/ButtonContinue");
        AddButton(panel.transform, L("NEW CAMPAIGN", "НОВАЯ КАМПАНИЯ"), new Vector2(0f, 8f), StartNewCampaign, new Vector2(520f, 72f), MenuButtonStyle.Stone, "Menu/Buttons/ButtonNewCampaign");
        AddButton(panel.transform, L("CHAPTER SELECT", "ВЫБОР ГЛАВЫ"), new Vector2(0f, -82f), ShowLevels, new Vector2(520f, 72f), MenuButtonStyle.Stone, "Menu/Buttons/ButtonChapterSelect");
        AddButton(panel.transform, L("SETTINGS", "НАСТРОЙКИ"), new Vector2(0f, -172f), ShowSettingsFromMain, new Vector2(520f, 72f), MenuButtonStyle.Ghost, "Menu/Buttons/ButtonSettings");
        AddButton(panel.transform, L("EXIT", "ВЫХОД"), new Vector2(0f, -262f), QuitGame, new Vector2(520f, 68f), MenuButtonStyle.Ghost, "Menu/Buttons/ButtonExit");

        AddTitle(
            panel.transform,
            L("Progress saves automatically", "Прогресс сохраняется автоматически"),
            new Vector2(0f, -350f),
            14,
            MenuTextStyle.Muted,
            new Vector2(580f, 34f));
        AddTitle(panel.transform, "v0.6 • PRE-ALPHA", new Vector2(0f, -405f), 13, MenuTextStyle.Muted, new Vector2(280f, 28f));
    }

    void BuildLevelMenu()
    {
        levelMenu = MakeScreen(canvas, "LevelSelect", new Color(.025f, .018f, .014f, .985f));
        levelMenu.AddComponent<ChapterSelectionArtwork>().Build(StartLevel, ShowMainMenu, IsChapterUnlocked, GameLanguage.Russian);
    }

    void BuildSettingsMenu()
    {
        settingsMenu = MakeScreen(canvas, "Settings", new Color(.02f, .015f, .012f, .99f));

        Texture2D background = Resources.Load<Texture2D>("Menu/Settings_background");
        if (background != null)
        {
            GameObject backgroundObject = new GameObject("Background");
            backgroundObject.transform.SetParent(settingsMenu.transform, false);
            Image backgroundImage = backgroundObject.AddComponent<Image>();
            backgroundImage.sprite = Sprite.Create(background, new Rect(0f, 0f, background.width, background.height), new Vector2(.5f, .5f));
            backgroundImage.type = Image.Type.Simple;
            backgroundImage.raycastTarget = false;
            StretchToParent(backgroundImage.rectTransform);

            AspectRatioFitter fitter = backgroundObject.AddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            fitter.aspectRatio = (float)background.width / background.height;
        }
    }

    void BuildPauseMenu()
    {
        pauseMenu = MakeScreen(canvas, "PauseMenu", new Color(.02f, .015f, .012f, .985f));
        GameObject panel = MakePanel(pauseMenu.transform, "PauseCard", new Vector2(.5f, .5f), new Vector2(1000, 720), new Color(.055f, .03f, .018f, .99f));

        AddTitle(panel.transform, L("PAUSED", "ПАУЗА"), new Vector2(0, 268), 48, MenuTextStyle.Logo);
        AddTitle(panel.transform, L("The battle waits for your command", "Битва ждёт вашего приказа"), new Vector2(0, 222), 17, MenuTextStyle.Muted, new Vector2(720, 38));
        AddDivider(panel.transform, new Vector2(0, 184), 700);

        AddButton(panel.transform, L("RESUME", "ВЕРНУТЬСЯ В ИГРУ"), new Vector2(0, 104), Resume, new Vector2(500, 62), MenuButtonStyle.Highlight);
        AddButton(panel.transform, L("SETTINGS", "НАСТРОЙКИ"), new Vector2(0, 30), ShowSettingsFromPause, new Vector2(500, 56), MenuButtonStyle.Stone);
        AddButton(panel.transform, L("RESTART CHAPTER", "ПЕРЕЗАПУСТИТЬ ГЛАВУ"), new Vector2(0, -40), RestartChapter, new Vector2(500, 56), MenuButtonStyle.Stone);
        AddButton(panel.transform, L("MAIN MENU", "ГЛАВНОЕ МЕНЮ"), new Vector2(0, -110), ReturnToMainMenu, new Vector2(500, 56), MenuButtonStyle.Ghost);
        AddButton(panel.transform, L("EXIT", "ВЫХОД"), new Vector2(0, -180), QuitGame, new Vector2(500, 52), MenuButtonStyle.Ghost);
        pauseMenu.AddComponent<PauseMenuArtwork>().Apply(panel.GetComponent<RectTransform>(), GameLanguage.Russian);
    }

    void BuildEndMenu()
    {
        endMenu = MakeScreen(canvas, "EndMenu", new Color(.02f, .015f, .012f, .99f));
        GameObject panel = MakePanel(endMenu.transform, "ResultCard", new Vector2(.5f, .5f), new Vector2(1160, 800), new Color(.055f, .03f, .018f, .99f));

        endTitle = AddTitle(panel.transform, L("RESULT", "РЕЗУЛЬТАТ"), new Vector2(0, 326), 52, MenuTextStyle.Logo);
        AddTitle(panel.transform, L("CHAPTER I • THE LANDING", "ГЛАВА I • ВЫСАДКА"), new Vector2(0, 278), 16, MenuTextStyle.Muted, new Vector2(760, 34));
        AddDivider(panel.transform, new Vector2(0, 244), 820);
        endSummary = AddTitle(panel.transform, "", new Vector2(0, 34), 18, MenuTextStyle.Normal, new Vector2(900, 390));

        AddButton(panel.transform, L("RETRY", "ПОВТОРИТЬ"), new Vector2(-330, -322), RestartChapter, new Vector2(280, 58), MenuButtonStyle.Highlight);
        AddButton(panel.transform, L("CHAPTER SELECT", "ВЫБОР ГЛАВЫ"), new Vector2(0, -322), ReturnToChapterSelect, new Vector2(300, 58), MenuButtonStyle.Stone);
        AddButton(panel.transform, L("MAIN MENU", "ГЛАВНОЕ МЕНЮ"), new Vector2(330, -322), ReturnToMainMenu, new Vector2(280, 58), MenuButtonStyle.Ghost);
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
        if (oldCanvas != null) oldCanvas.SetActive(false);
        BuildUI();

        if (wasSettings) SetScreenState(settings: true);
        else if (wasPause) SetScreenState(pause: true);
        else if (wasLevels) SetScreenState(levels: true);
        else ShowMainMenu();

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

    void StartNewCampaign()
    {
        restartGiftPending = false;
        CampaignController.Instance?.ResetProgress();
        RuntimeFileLogger.Event("CAMPAIGN", "New campaign started from main menu");
        ShowLevels();
    }

    void StartLevel()
    {
        levelStarted = true;
        paused = false;
        CombatControlsUI.ResumeConfiguredSpeed();
        if (Time.timeScale <= 0f) Time.timeScale = 1f;
        GameManager.Instance?.BeginRun();
        SetScreenState();
        spawner?.ActivateLevel();
    }

    void ShowMainMenu()
    {
        Time.timeScale = 0f;
        levelStarted = false;
        paused = false;
        RefreshLevelSelect();
        SetScreenState(main: true);
    }

    public void ReturnToMainMenu()
    {
        RuntimeFileLogger.Event("MENU", "Returning to main menu through scene reset");
        restartGiftPending = false;
        startLevelAfterReload = false;
        openLevelSelectAfterReload = false;
        RestartScene();
    }

    public void ReturnToChapterSelect()
    {
        RuntimeFileLogger.Event("MENU", "Returning to chapter select through scene reset");
        restartGiftPending = false;
        startLevelAfterReload = false;
        openLevelSelectAfterReload = true;
        RestartScene();
    }

    void ShowLevels()
    {
        Time.timeScale = 0f;
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
        SetScreenState(pause: true);
    }

    void Resume()
    {
        paused = false;
        CombatControlsUI.ResumeConfiguredSpeed();
        SetScreenState();
    }

    void ShowSettingsFromMain()
    {
        SetScreenState(settings: true);
    }

    void ShowSettingsFromPause()
    {
        SetScreenState(settings: true);
    }

    void BackFromSettings()
    {
        if (levelStarted && paused) SetScreenState(pause: true);
        else SetScreenState(main: true);
    }

    void ShowEnd()
    {
        Time.timeScale = 0f;
        paused = true;
        GameManager gm = GameManager.Instance;
        if (gm == null) return;

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
            $"{L("ENCOUNTERS", "БОИ")}: {gm.CurrentWave}/{gm.MaxWaves}    {L("TIME", "ВРЕМЯ")}: {min:00}:{sec:00}\n\n" +
            $"{L("SCORE", "СЧЁТ")}: {gm.FinalScore}\n" +
            $"{L("KILLS", "УБИТО")}: {gm.Kills}    {L("LEAKS", "ПРОПУЩЕНО")}: {gm.Leaks}\n" +
            $"{L("GOLD EARNED", "ЗОЛОТО ПОЛУЧЕНО")}: {gm.GoldEarned}    {L("SPENT", "ПОТРАЧЕНО")}: {gm.GoldSpent}\n" +
            $"{L("TOWERS BUILT", "ПОСТРОЕНО БАШЕН")}: {gm.TowersBuilt}    {L("SOLD", "ПРОДАНО")}: {gm.TowersSold}\n" +
            $"{L("GATE HEALTH", "ПРОЧНОСТЬ ВОРОТ")}: {gm.BaseHealth}/{gm.MaxBaseHealth}{unlock}";

        SetScreenState(end: true);
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

        if (scene.buildIndex >= 0)
            SceneManager.LoadScene(scene.buildIndex, LoadSceneMode.Single);
        else if (!string.IsNullOrEmpty(scene.name))
            SceneManager.LoadScene(scene.name, LoadSceneMode.Single);
        else
            SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void RestartChapter()
    {
        GameManager gm = GameManager.Instance;
        restartGiftPending = gm != null && gm.GiftSelected;
        if (restartGiftPending) restartGift = gm.SelectedGift;
        startLevelAfterReload = true;
        openLevelSelectAfterReload = false;
        RestartScene();
    }

    public void QuitGame()
    {
        RuntimeFileLogger.Event("MENU", "Exit requested");
        QuitRequested = true;
        restartGiftPending = false;
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
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
}
