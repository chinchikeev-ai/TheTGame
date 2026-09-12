using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuController : MonoBehaviour
{
    const string MainMenuBackgroundResource = "Menu/Main_screen";
    static bool openLevelSelectAfterReload;

    static readonly Vector2[] SupportedResolutions =
    {
        new Vector2(1366, 768),
        new Vector2(1600, 900),
        new Vector2(1920, 1080),
        new Vector2(2560, 1440),
        new Vector2(3840, 2160)
    };

    static readonly int[] FpsOptions = { 30, 60, 120, 144, -1 };

    Canvas canvas;
    EnemySpawner spawner;
    GameObject mainMenu;
    GameObject levelMenu;
    GameObject settingsMenu;
    GameObject pauseMenu;
    GameObject endMenu;
    Button startWaveButton;
    Text countdownText;
    Text endTitle;
    Text endSummary;
    Text map2Label;
    Text map2Info;
    Text difficultyLabel;
    Text masterVolumeValue;
    Text musicVolumeValue;
    Text resolutionValue;
    Text fullscreenValue;
    Text vsyncValue;
    Text fpsValue;
    Text languageValue;
    Text qualityValue;
    Slider masterVolumeSlider;
    Slider musicVolumeSlider;

    int resolutionIndex;
    int fpsIndex;
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
        SyncSettingsUi();

        if (openLevelSelectAfterReload)
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

        if (spawner != null && startWaveButton != null)
        {
            startWaveButton.gameObject.SetActive(levelStarted && !paused && !spawner.WaveActive && GameManager.Instance != null && !GameManager.Instance.GameEnded);

            if (spawner.InterWaveCountdown > 0f)
                countdownText.text = L("AUTO START  ", "АВТОСТАРТ  ") + Mathf.CeilToInt(spawner.InterWaveCountdown) + L("s", "с");
            else if (!spawner.WaveActive)
                countdownText.text = L("READY", "ГОТОВО");
            else
                countdownText.text = "";
        }

        if (levelStarted && GameManager.Instance != null && GameManager.Instance.GameEnded && !endMenu.activeSelf)
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
        BuildWaveControls();

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
        settingsMenu = MakeScreen("Settings", new Color(.02f, .015f, .012f, .97f));
        GameObject panel = MakePanel(settingsMenu.transform, "SettingsPanel", new Vector2(.5f, .5f), new Vector2(1320, 920), new Color(.065f, .035f, .022f, .98f));

        AddTitle(panel.transform, L("SETTINGS", "НАСТРОЙКИ"), new Vector2(0, 392), 52, MenuTextStyle.Logo);
        AddTitle(panel.transform, L("Tune the experience without leaving Troy", "Настройте игру под себя"), new Vector2(0, 342), 19, MenuTextStyle.Muted);
        AddDivider(panel.transform, new Vector2(0, 306), 1040);

        AddSectionLabel(panel.transform, L("AUDIO", "ЗВУК"), new Vector2(-395, 258));
        masterVolumeSlider = AddSliderRow(panel.transform, L("Master Volume", "Общая громкость"), new Vector2(-345, 195), GameUserSettings.MasterVolume, OnMasterVolumeChanged, out masterVolumeValue);
        musicVolumeSlider = AddSliderRow(panel.transform, L("Music", "Музыка"), new Vector2(-345, 95), GameUserSettings.MusicVolume, OnMusicVolumeChanged, out musicVolumeValue);

        AddSectionLabel(panel.transform, L("DISPLAY", "ЭКРАН"), new Vector2(305, 258));
        fullscreenValue = AddSelectorRow(panel.transform, L("Window Mode", "Режим экрана"), new Vector2(360, 205), ToggleFullscreen);
        resolutionValue = AddSelectorRow(panel.transform, L("Resolution", "Разрешение"), new Vector2(360, 125), CycleResolution);
        vsyncValue = AddSelectorRow(panel.transform, "VSync", new Vector2(360, 45), ToggleVSync);
        fpsValue = AddSelectorRow(panel.transform, L("FPS Limit", "Лимит FPS"), new Vector2(360, -35), CycleFpsLimit);
        qualityValue = AddSelectorRow(panel.transform, L("Graphics Quality", "Качество графики"), new Vector2(360, -115), CycleQuality);

        AddSectionLabel(panel.transform, L("GAME", "ИГРА"), new Vector2(-395, -25));
        languageValue = AddSelectorRow(panel.transform, L("Language", "Язык"), new Vector2(-345, -95), ToggleLanguage);
        Button difficultyButton = AddButton(panel.transform, "", new Vector2(-345, -192), CycleDifficulty, new Vector2(470, 62), MenuButtonStyle.Stone);
        difficultyLabel = difficultyButton.GetComponentInChildren<Text>();
        AddButton(panel.transform, L("RESET DEFAULTS", "СБРОСИТЬ"), new Vector2(360, -215), ResetSettings, new Vector2(470, 60), MenuButtonStyle.Ghost);

        AddDivider(panel.transform, new Vector2(0, -304), 1040);
        AddButton(panel.transform, L("APPLY", "ПРИМЕНИТЬ"), new Vector2(-155, -370), ApplySettingsAndBack, new Vector2(290, 64), MenuButtonStyle.Highlight);
        AddButton(panel.transform, L("BACK", "НАЗАД"), new Vector2(175, -370), BackFromSettings, new Vector2(290, 64), MenuButtonStyle.Ghost);

        RefreshDifficultyLabel();
    }

    void BuildPauseMenu()
    {
        pauseMenu = MakeScreen("PauseMenu", new Color(.02f, .015f, .012f, .93f));
        GameObject panel = MakePanel(pauseMenu.transform, "PauseCard", new Vector2(.5f, .5f), new Vector2(620, 680), new Color(.07f, .04f, .025f, .98f));

        AddTitle(panel.transform, L("PAUSED", "ПАУЗА"), new Vector2(0, 250), 52, MenuTextStyle.Logo);
        AddTitle(panel.transform, L("The battle waits for your command", "Битва ждёт вашего приказа"), new Vector2(0, 202), 18, MenuTextStyle.Muted);
        AddButton(panel.transform, L("RESUME", "ВЕРНУТЬСЯ В ИГРУ"), new Vector2(0, 110), Resume, new Vector2(420, 64), MenuButtonStyle.Highlight);
        AddButton(panel.transform, L("SETTINGS", "НАСТРОЙКИ"), new Vector2(0, 32), ShowSettingsFromPause, new Vector2(420, 60), MenuButtonStyle.Stone);
        AddButton(panel.transform, L("RESTART CHAPTER", "ПЕРЕЗАПУСТИТЬ ГЛАВУ"), new Vector2(0, -46), RestartScene, new Vector2(420, 60), MenuButtonStyle.Stone);
        AddButton(panel.transform, L("MAIN MENU", "ГЛАВНОЕ МЕНЮ"), new Vector2(0, -124), ReturnToMainMenu, new Vector2(420, 60), MenuButtonStyle.Ghost);
        AddButton(panel.transform, L("EXIT", "ВЫХОД"), new Vector2(0, -202), QuitGame, new Vector2(420, 56), MenuButtonStyle.Ghost);
    }

    void BuildEndMenu()
    {
        endMenu = MakeScreen("EndMenu", new Color(.02f, .015f, .012f, .95f));
        GameObject panel = MakePanel(endMenu.transform, "ResultCard", new Vector2(.5f, .5f), new Vector2(980, 820), new Color(.07f, .04f, .025f, .98f));

        endTitle = AddTitle(panel.transform, L("RESULT", "РЕЗУЛЬТАТ"), new Vector2(0, 320), 58, MenuTextStyle.Logo);
        endSummary = AddTitle(panel.transform, "", new Vector2(0, 80), 22, MenuTextStyle.Normal, new Vector2(840, 420));
        AddButton(panel.transform, L("RETRY", "ПОВТОРИТЬ"), new Vector2(-175, -300), RestartScene, new Vector2(300, 64), MenuButtonStyle.Highlight);
        AddButton(panel.transform, L("CHAPTER SELECT", "ВЫБОР ГЛАВЫ"), new Vector2(175, -300), ReturnToMainMenu, new Vector2(300, 64), MenuButtonStyle.Stone);
    }

    void BuildWaveControls()
    {
        GameObject wavePanel = MakePanel(canvas.transform, "WaveControls", new Vector2(.5f, 0f), new Vector2(390, 120), new Color(.06f, .035f, .02f, .90f));
        wavePanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 84);
        startWaveButton = AddButton(wavePanel.transform, L("START WAVE", "НАЧАТЬ ВОЛНУ"), new Vector2(0, 22), delegate { if (spawner != null) spawner.StartWaveNow(); }, new Vector2(290, 56), MenuButtonStyle.Highlight);
        countdownText = AddTitle(wavePanel.transform, L("READY", "ГОТОВО"), new Vector2(0, -32), 16, MenuTextStyle.Muted, new Vector2(330, 38));
        startWaveButton.gameObject.SetActive(false);
    }

    void OnMasterVolumeChanged(float value)
    {
        GameUserSettings.MasterVolume = value;
        if (masterVolumeValue != null) masterVolumeValue.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    void OnMusicVolumeChanged(float value)
    {
        GameUserSettings.MusicVolume = value;
        if (musicVolumeValue != null) musicVolumeValue.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    void ToggleFullscreen()
    {
        GameUserSettings.Fullscreen = !GameUserSettings.Fullscreen;
        Vector2 resolution = SupportedResolutions[Mathf.Clamp(resolutionIndex, 0, SupportedResolutions.Length - 1)];
        Screen.SetResolution((int)resolution.x, (int)resolution.y, GameUserSettings.Fullscreen);
        RefreshSettingsLabels();
    }

    void CycleResolution()
    {
        resolutionIndex = (resolutionIndex + 1) % SupportedResolutions.Length;
        Vector2 resolution = SupportedResolutions[resolutionIndex];
        GameUserSettings.SetResolution((int)resolution.x, (int)resolution.y);
        RefreshSettingsLabels();
    }

    void ToggleVSync()
    {
        GameUserSettings.VSync = !GameUserSettings.VSync;
        RefreshSettingsLabels();
    }

    void CycleFpsLimit()
    {
        fpsIndex = (fpsIndex + 1) % FpsOptions.Length;
        GameUserSettings.FpsLimit = FpsOptions[fpsIndex];
        RefreshSettingsLabels();
    }

    void CycleQuality()
    {
        int count = QualitySettings.names.Length;
        if (count <= 0) return;
        int next = (QualitySettings.GetQualityLevel() + 1) % count;
        QualitySettings.SetQualityLevel(next, true);
        RefreshSettingsLabels();
    }

    void ResetSettings()
    {
        GameUserSettings.ResetToDefaults();
        SyncSettingsUi();
    }

    void ApplySettingsAndBack()
    {
        GameUserSettings.Save();
        GameUserSettings.ApplySaved();
        BackFromSettings();
    }

    void SyncSettingsUi()
    {
        FindClosestResolutionIndex();
        fpsIndex = FindFpsIndex(GameUserSettings.FpsLimit);

        if (masterVolumeSlider != null)
            masterVolumeSlider.SetValueWithoutNotify(GameUserSettings.MasterVolume);
        if (musicVolumeSlider != null)
            musicVolumeSlider.SetValueWithoutNotify(GameUserSettings.MusicVolume);

        RefreshSettingsLabels();
        RefreshDifficultyLabel();
    }

    void RefreshSettingsLabels()
    {
        if (masterVolumeValue != null)
            masterVolumeValue.text = Mathf.RoundToInt(GameUserSettings.MasterVolume * 100f) + "%";
        if (musicVolumeValue != null)
            musicVolumeValue.text = Mathf.RoundToInt(GameUserSettings.MusicVolume * 100f) + "%";

        Vector2 resolution = SupportedResolutions[Mathf.Clamp(resolutionIndex, 0, SupportedResolutions.Length - 1)];
        if (resolutionValue != null)
            resolutionValue.text = $"{(int)resolution.x} × {(int)resolution.y}";
        if (fullscreenValue != null)
            fullscreenValue.text = GameUserSettings.Fullscreen ? L("FULLSCREEN", "ПОЛНЫЙ ЭКРАН") : L("WINDOWED", "ОКОННЫЙ");
        if (vsyncValue != null)
            vsyncValue.text = GameUserSettings.VSync ? L("ON", "ВКЛ") : L("OFF", "ВЫКЛ");
        if (fpsValue != null)
            fpsValue.text = GameUserSettings.FpsLimit <= 0 ? L("UNLIMITED", "БЕЗ ЛИМИТА") : GameUserSettings.FpsLimit.ToString();
        if (languageValue != null)
            languageValue.text = GameLanguage.Russian ? "Русский" : "English";
        if (qualityValue != null)
        {
            string[] names = QualitySettings.names;
            qualityValue.text = names.Length > 0 ? names[Mathf.Clamp(QualitySettings.GetQualityLevel(), 0, names.Length - 1)] : "Default";
        }
    }

    void FindClosestResolutionIndex()
    {
        float best = float.MaxValue;
        int bestIndex = 0;

        for (int i = 0; i < SupportedResolutions.Length; i++)
        {
            Vector2 candidate = SupportedResolutions[i];
            float distance = Mathf.Abs(candidate.x - GameUserSettings.ResolutionWidth) + Mathf.Abs(candidate.y - GameUserSettings.ResolutionHeight);
            if (distance < best)
            {
                best = distance;
                bestIndex = i;
            }
        }

        resolutionIndex = bestIndex;
    }

    int FindFpsIndex(int fps)
    {
        for (int i = 0; i < FpsOptions.Length; i++)
            if (FpsOptions[i] == fps) return i;
        return 1;
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
        SyncSettingsUi();

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

        if (oldCanvas != null)
            Destroy(oldCanvas);
    }

    void CycleDifficulty()
    {
        if (levelStarted)
        {
            RuntimeFileLogger.Event("DIFFICULTY", "Difficulty change ignored during active run");
            return;
        }
        if (CampaignController.Instance == null) return;
        CampaignDifficulty difficulty = CampaignController.Instance.CycleDifficulty();
        RuntimeFileLogger.Event("DIFFICULTY", $"Selected {difficulty}");
        RefreshDifficultyLabel();
    }

    void RefreshDifficultyLabel()
    {
        if (difficultyLabel != null)
            difficultyLabel.text = L("DIFFICULTY", "СЛОЖНОСТЬ") + ": " + DifficultyRules.Label(CurrentDifficulty);
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
        if (spawner != null) spawner.ActivateLevel();
    }

    void ShowMainMenu()
    {
        Time.timeScale = 0f;
        paused = false;
        RefreshLevelSelect();
        if (mainMenu != null) mainMenu.SetActive(true);
        if (levelMenu != null) levelMenu.SetActive(false);
        if (settingsMenu != null) settingsMenu.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (endMenu != null) endMenu.SetActive(false);
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
        mainMenu.SetActive(false);
        levelMenu.SetActive(true);
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(false);
    }

    void TogglePause()
    {
        if (paused) Resume(); else Pause();
    }

    void Pause()
    {
        paused = true;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
    }

    void Resume()
    {
        paused = false;
        CombatControlsUI.ResumeConfiguredSpeed();
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    void ShowSettingsFromMain()
    {
        mainMenu.SetActive(false);
        SyncSettingsUi();
        settingsMenu.SetActive(true);
    }

    void ShowSettingsFromPause()
    {
        pauseMenu.SetActive(false);
        SyncSettingsUi();
        settingsMenu.SetActive(true);
    }

    void BackFromSettings()
    {
        settingsMenu.SetActive(false);
        if (levelStarted && paused) pauseMenu.SetActive(true);
        else mainMenu.SetActive(true);
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

        endMenu.SetActive(true);
    }

    void RestartScene()
    {
        if (reloading) return;
        StartCoroutine(ReloadSceneRoutine());
    }

    IEnumerator ReloadSceneRoutine()
    {
        reloading = true;
        Scene scene = SceneManager.GetActiveScene();
        RuntimeFileLogger.Event("MENU", $"Reloading scene buildIndex={scene.buildIndex}, name={scene.name}");
        Time.timeScale = 1f;

        foreach (Button button in canvas.GetComponentsInChildren<Button>(true))
            button.interactable = false;

        AsyncOperation operation = scene.buildIndex >= 0
            ? SceneManager.LoadSceneAsync(scene.buildIndex)
            : SceneManager.LoadSceneAsync(scene.name);

        if (operation == null)
        {
            RuntimeFileLogger.Event("MENU", "Scene reload failed to start");
            reloading = false;
            yield break;
        }

        while (!operation.isDone) yield return null;
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

    void StretchToParent(RectTransform rect)
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

    void AddSectionLabel(Transform parent, string label, Vector2 position)
    {
        Text text = AddTitle(parent, label, position, 18, MenuTextStyle.Subtitle, new Vector2(470, 40));
        text.alignment = TextAnchor.MiddleLeft;
    }

    Slider AddSliderRow(Transform parent, string label, Vector2 position, float value, UnityEngine.Events.UnityAction<float> onChanged, out Text valueText)
    {
        Text labelText = AddTitle(parent, label, position + new Vector2(-65, 30), 18, MenuTextStyle.Normal, new Vector2(330, 34));
        labelText.alignment = TextAnchor.MiddleLeft;

        GameObject sliderObject = new GameObject(label + " Slider");
        sliderObject.transform.SetParent(parent, false);
        Slider slider = sliderObject.AddComponent<Slider>();
        RectTransform rect = sliderObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position + new Vector2(-25, -12);
        rect.sizeDelta = new Vector2(330, 28);

        GameObject background = new GameObject("Background");
        background.transform.SetParent(sliderObject.transform, false);
        Image backgroundImage = background.AddComponent<Image>();
        backgroundImage.color = new Color(.20f, .12f, .08f, 1f);
        StretchToParent(backgroundImage.rectTransform);

        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObject.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        StretchToParent(fillAreaRect);
        fillAreaRect.offsetMin = new Vector2(4, 7);
        fillAreaRect.offsetMax = new Vector2(-4, -7);

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(.75f, .22f, .08f, 1f);
        StretchToParent(fillImage.rectTransform);
        slider.fillRect = fillImage.rectTransform;

        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderObject.transform, false);
        RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
        StretchToParent(handleAreaRect);
        handleAreaRect.offsetMin = new Vector2(8, 0);
        handleAreaRect.offsetMax = new Vector2(-8, 0);

        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = new Color(1f, .72f, .28f, 1f);
        RectTransform handleRect = handleImage.rectTransform;
        handleRect.sizeDelta = new Vector2(22, 38);

        slider.handleRect = handleRect;
        slider.targetGraphic = handleImage;
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.SetValueWithoutNotify(value);
        slider.onValueChanged.AddListener(onChanged);

        valueText = AddTitle(parent, Mathf.RoundToInt(value * 100f) + "%", position + new Vector2(195, -12), 18, MenuTextStyle.Subtitle, new Vector2(80, 34));
        return slider;
    }

    Text AddSelectorRow(Transform parent, string label, Vector2 position, UnityEngine.Events.UnityAction action)
    {
        Text labelText = AddTitle(parent, label, position + new Vector2(-70, 28), 18, MenuTextStyle.Normal, new Vector2(330, 34));
        labelText.alignment = TextAnchor.MiddleLeft;
        Button button = AddButton(parent, "", position + new Vector2(10, -15), action, new Vector2(430, 54), MenuButtonStyle.Stone);
        return button.GetComponentInChildren<Text>();
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

    Color TextColor(MenuTextStyle style)
    {
        switch (style)
        {
            case MenuTextStyle.Logo: return new Color(1f, .62f, .18f, 1f);
            case MenuTextStyle.Subtitle: return new Color(1f, .84f, .56f, 1f);
            case MenuTextStyle.Muted: return new Color(.82f, .72f, .62f, .92f);
            default: return new Color(.97f, .92f, .84f, 1f);
        }
    }

    Color ButtonColor(MenuButtonStyle style)
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
