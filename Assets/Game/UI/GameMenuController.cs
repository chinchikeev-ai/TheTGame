using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenuController : MonoBehaviour
{
    const string MainMenuBackgroundResource = "Menu/Main_screen";
    static bool openLevelSelectAfterReload;

    Canvas canvas;
    EnemySpawner spawner;
    GameObject mainMenu, levelMenu, settingsMenu, pauseMenu, endMenu;
    Button startWaveButton;
    Text countdownText, endTitle, endSummary, map2Label, map2Info, difficultyLabel;
    Text masterVolumeValue, musicVolumeValue, displayValue, vsyncValue, fpsValue, languageValue, qualityValue;
    Slider masterVolumeSlider, musicVolumeSlider;
    int resolutionIndex;
    int fpsIndex;
    int qualityIndex;
    bool levelStarted;
    bool paused;
    bool reloading;

    static readonly Vector2[] SupportedResolutions =
    {
        new Vector2(1366, 768),
        new Vector2(1600, 900),
        new Vector2(1920, 1080),
        new Vector2(2560, 1440),
        new Vector2(3840, 2160)
    };

    static readonly int[] FpsOptions = { 30, 60, 120, 144, -1 };

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
        if (levelStarted && GameManager.Instance != null && !GameManager.Instance.GameEnded && GameInput.PausePressed()) TogglePause();

        if (spawner != null && startWaveButton != null)
        {
            startWaveButton.gameObject.SetActive(levelStarted && !paused && !spawner.WaveActive && GameManager.Instance != null && !GameManager.Instance.GameEnded);
            if (spawner.InterWaveCountdown > 0f)
                countdownText.text = L("AUTO START  ", "АВТОСТАРТ  ") + Mathf.CeilToInt(spawner.InterWaveCountdown) + L("s", "с");
            else if (!spawner.WaveActive)
                countdownText.text = L("READY", "ГОТОВО");
            else countdownText.text = "";
        }

        if (levelStarted && GameManager.Instance != null && GameManager.Instance.GameEnded && !endMenu.activeSelf) ShowEnd();
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

        GameObject heroPanel = MakePanel(mainMenu.transform, "HeroPanel", new Vector2(.18f, .5f), new Vector2(620, 820), new Color(.055f, .028f, .018f, .72f));
        RectTransform heroRect = heroPanel.GetComponent<RectTransform>();
        heroRect.anchoredPosition = new Vector2(40, 0);

        AddTitle(heroPanel.transform, "THE TROY GAME", new Vector2(0, 270), 66, MenuTextStyle.Logo, new Vector2(560, 100));
        AddTitle(heroPanel.transform, "SIEGE DEFENSE", new Vector2(0, 205), 24, MenuTextStyle.Subtitle, new Vector2(560, 50));
        AddDivider(heroPanel.transform, new Vector2(0, 160), 440);

        Text campaignTag = AddTitle(heroPanel.transform, L("DEFEND TROY • MASTER THE FIRE", "ЗАЩИТИ ТРОЮ • ПОВЕЛЕВАЙ ОГНЁМ"), new Vector2(0, 115), 18, MenuTextStyle.Muted, new Vector2(520, 46));
        campaignTag.fontStyle = FontStyle.Normal;

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
        levelMenu = MakeScreen("LevelSelect", new Color(.025f, .018f, .014f, .94f));
        GameObject panel = MakePanel(levelMenu.transform, "LevelCard", new Vector2(.5f, .5f), new Vector2(980, 700), new Color(.08f, .045f, .025f, .96f));
        AddTitle(panel.transform, L("CHAPTER SELECT", "ВЫБОР ГЛАВЫ"), new Vector2(0, 270), 48, MenuTextStyle.Logo);
        AddTitle(panel.transform, L("Choose where the defense of Troy continues", "Выберите этап обороны Трои"), new Vector2(0, 215), 20, MenuTextStyle.Muted);
        AddDivider(panel.transform, new Vector2(0, 180), 700);

        AddButton(panel.transform, L("I  •  THE LANDING", "I  •  ВЫСАДКА"), new Vector2(0, 90), StartLevel, new Vector2(700, 78), MenuButtonStyle.Highlight);
        Button map2Button = AddButton(panel.transform, "", new Vector2(0, -10), OnMap2Clicked, new Vector2(700, 78), MenuButtonStyle.Stone);
        map2Label = map2Button.GetComponentInChildren<Text>();
        map2Info = AddTitle(panel.transform, "", new Vector2(0, -78), 18, MenuTextStyle.Muted, new Vector2(760, 55));
        AddButton(panel.transform, L("BACK", "НАЗАД"), new Vector2(0, -235), ShowMainMenu, new Vector2(280, 58), MenuButtonStyle.Ghost);
        RefreshLevelSelect();
    }

    void BuildSettingsMenu()
    {
        settingsMenu = MakeScreen("Settings", new Color(.02f, .015f, .012f, .96f));
        GameObject panel = MakePanel(settingsMenu.transform, "SettingsPanel", new Vector2(.5f, .5f), new Vector2(1280, 900), new Color(.065f, .035f, .022f, .98f));

        AddTitle(panel.transform, L("SETTINGS", "НАСТРОЙКИ"), new Vector2(0, 380), 52, MenuTextStyle.Logo);
        AddTitle(panel.transform, L("Tune the experience without leaving Troy", "Настройте игру под себя"), new Vector2(0, 330), 19, MenuTextStyle.Muted);
        AddDivider(panel.transform, new Vector2(0, 295), 1010);

        AddSectionLabel(panel.transform, L("AUDIO", "ЗВУК"), new Vector2(-390, 245));
        masterVolumeSlider = AddSliderRow(panel.transform, L("Master Volume", "Общая громкость"), new Vector2(-340, 185), GameUserSettings.MasterVolume, OnMasterVolumeChanged, out masterVolumeValue);
        musicVolumeSlider = AddSliderRow(panel.transform, L("Music", "Музыка"), new Vector2(-340, 90), GameUserSettings.MusicVolume, OnMusicVolumeChanged, out musicVolumeValue);

        AddSectionLabel(panel.transform, L("DISPLAY", "ЭКРАН"), new Vector2(300, 245));
        displayValue = AddSelectorRow(panel.transform, L("Resolution", "Разрешение"), new Vector2(355, 185), CycleResolution);
        vsyncValue = AddSelectorRow(panel.transform, "VSync", new Vector2(355, 90), ToggleVSync);
        fpsValue = AddSelectorRow(panel.transform, L("FPS Limit", "Лимит FPS"), new Vector2(355, -5), CycleFpsLimit);
        qualityValue = AddSelectorRow(panel.transform, L("Graphics Quality", "Качество графики"), new Vector2(355, -100), CycleQuality);

        AddSectionLabel(panel.transform, L("GAME", "ИГРА"), new Vector2(-390, -35));
        languageValue = AddSelectorRow(panel.transform, L("Language", "Язык"), new Vector2(-340, -95), ToggleLanguage);
        Button difficultyButton = AddButton(panel.transform, "", new Vector2(-340, -190), CycleDifficulty, new Vector2(470, 62), MenuButtonStyle.Stone);
        difficultyLabel = difficultyButton.GetComponentInChildren<Text>();

        AddButton(panel.transform, L("RESET DEFAULTS", "СБРОСИТЬ"), new Vector2(355, -195), ResetSettings, new Vector2(470, 62), MenuButtonStyle.Ghost);

        AddDivider(panel.transform, new Vector2(0, -285), 1010);
        AddButton(panel.transform, L("APPLY", "ПРИМЕНИТЬ"), new Vector2(-150, -350), ApplySettingsAndBack, new Vector2(280, 64), MenuButtonStyle.Highlight);
        AddButton(panel.transform, L("BACK", "НАЗАД"), new Vector2(170, -350), BackFromSettings, new Vector2(280, 64), MenuButtonStyle.Ghost);

        RefreshDifficultyLabel();
    }

    void BuildPauseMenu()
    {
        pauseMenu = MakeScreen("PauseMenu", new Color(.02f, .015f, .012f, .92f));
        GameObject panel = MakePanel(pauseMenu.transform, "PauseCard", new Vector2(.5f, .5f), new Vector2(620, 680), new Color(.07f, .04f, .025f, .98f));
        AddTitle(panel.transform, L("PAUSED", "ПАУЗА"), new Vector2(0, 250), 52, MenuTextStyle.Logo);
        AddTitle(panel.transform, L("The battle waits for your command", "Битва ждёт вашего приказа"), new Vector2(0, 202), 18, MenuTextStyle.Muted);
        AddButton(panel.transform, L("RESUME", "ПРОДОЛЖИТЬ"), new Vector2(0, 110), Resume, new Vector2(420, 64), MenuButtonStyle.Highlight);
        AddButton(panel.transform, L("SETTINGS", "НАСТРОЙКИ"), new Vector2(0, 32), ShowSettingsFromPause, new Vector2(420, 60), MenuButtonStyle.Stone);
        AddButton(panel.transform, L("RESTART CHAPTER", "ПЕРЕЗАПУСТИТЬ ГЛАВУ"), new Vector2(0, -46), RestartScene, new Vector2(420, 60), MenuButtonStyle.Stone);
        AddButton(panel.transform, L("MAIN MENU", "ГЛАВНОЕ МЕНЮ"), new Vector2(0, -124), ReturnToMainMenu, new Vector2(420, 60), MenuButtonStyle.Ghost);
        AddButton(panel.transform, L("EXIT", "ВЫХОД"), new Vector2(0, -202), QuitGame, new Vector2(420, 56), MenuButtonStyle.Ghost);
    }

    void BuildEndMenu()
    {
        endMenu = MakeScreen("EndMenu", new Color(.02f, .015f, .012f, .94f));
        GameObject panel = MakePanel(endMenu.transform, "ResultCard", new Vector2(.5f, .5f), new Vector2(980, 820), new Color(.07f, .04f, .025f, .98f));
        endTitle = AddTitle(panel.transform, L("RESULT", "РЕЗУЛЬТАТ"), new Vector2(0, 320), 58, MenuTextStyle.Logo);
        endSummary = AddTitle(panel.transform, "", new Vector2(0, 80), 22, MenuTextStyle.Normal, new Vector2(840, 420));
        AddButton(panel.transform, L("RETRY", "ПОВТОРИТЬ"), new Vector2(-175, -300), RestartScene, new Vector2(300, 64), MenuButtonStyle.Highlight);
        AddButton(panel.transform, L("CHAPTER SELECT", "ВЫБОР ГЛАВЫ"), new Vector2(175, -300), ReturnToMainMenu, new Vector2(300, 64), MenuButtonStyle.Stone);
    }

    void BuildWaveControls()
    {
        GameObject wavePanel = MakePanel(canvas.transform, "WaveControls", new Vector2(.5f, 0f), new Vector2(390, 120), new Color(.06f, .035f, .02f, .90f));
        RectTransform wr = wavePanel.GetComponent<RectTransform>();
        wr.anchoredPosition = new Vector2(0, 84);
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
        qualityIndex = (QualitySettings.GetQualityLevel() + 1) % count;
        QualitySettings.SetQualityLevel(qualityIndex, true);
        RefreshSettingsLabels();
    }

    void ResetSettings()
    {
        GameUserSettings.ResetToDefaults();
        qualityIndex = Mathf.Clamp(QualitySettings.GetQualityLevel(), 0, Mathf.Max(0, QualitySettings.names.Length - 1));
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
        qualityIndex = Mathf.Clamp(QualitySettings.GetQualityLevel(), 0, Mathf.Max(0, QualitySettings.names.Length - 1));

        if (masterVolumeSlider != null) masterVolumeSlider.SetValueWithoutNotify(GameUserSettings.MasterVolume);
        if (musicVolumeSlider != null) musicVolumeSlider.SetValueWithoutNotify(GameUserSettings.MusicVolume);
        RefreshSettingsLabels();
        RefreshDifficultyLabel();
    }

    void RefreshSettingsLabels()
    {
        if (masterVolumeValue != null) masterVolumeValue.text = Mathf.RoundToInt(GameUserSettings.MasterVolume * 100f) + "%";
        if (musicVolumeValue != null) musicVolumeValue.text = Mathf.RoundToInt(GameUserSettings.MusicVolume * 100f) + "%";

        Vector2 resolution = SupportedResolutions[Mathf.Clamp(resolutionIndex, 0, SupportedResolutions.Length - 1)];
        if (displayValue != null) displayValue.text = $"{(int)resolution.x} × {(int)resolution.y}  •  {(GameUserSettings.Fullscreen ? L("FULLSCREEN", "ПОЛНЫЙ ЭКРАН") : L("WINDOWED", "ОКНО"))}";
        if (vsyncValue != null) vsyncValue.text = GameUserSettings.VSync ? L("ON", "ВКЛ") : L("OFF", "ВЫКЛ");
        if (fpsValue != null) fpsValue.text = GameUserSettings.FpsLimit <= 0 ? L("UNLIMITED", "БЕЗ ЛИМИТА") : GameUserSettings.FpsLimit.ToString();
        if (languageValue != null) languageValue.text = GameLanguage.Russian ? "Русский" : "English";
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
        GameLanguage.Toggle();
        RuntimeFileLogger.Event("LANGUAGE", $"Changed language to {GameLanguage.Code}");
        RefreshMenuLanguage();
        RefreshLevelSelect();
        RefreshDifficultyLabel();
        RefreshSettingsLabels();
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
        if (difficultyLabel == null) return;
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

    void RefreshMenuLanguage()
    {
        if (canvas == null) return;
        foreach (Text text in canvas.GetComponentsInChildren<Text>(true))
        {
            switch (text.text)
            {
                case "CONTINUE": case "ПРОДОЛЖИТЬ": text.text = L("CONTINUE", "ПРОДОЛЖИТЬ"); break;
                case "NEW CAMPAIGN": case "НОВАЯ КАМПАНИЯ": text.text = L("NEW CAMPAIGN", "НОВАЯ КАМПАНИЯ"); break;
                case "CHAPTER SELECT": case "ВЫБОР ГЛАВЫ": text.text = L("CHAPTER SELECT", "ВЫБОР ГЛАВЫ"); break;
                case "SETTINGS": case "НАСТРОЙКИ": text.text = L("SETTINGS", "НАСТРОЙКИ"); break;
                case "EXIT": case "ВЫХОД": text.text = L("EXIT", "ВЫХОД"); break;
                case "BACK": case "НАЗАД": text.text = L("BACK", "НАЗАД"); break;
                case "PAUSED": case "ПАУЗА": text.text = L("PAUSED", "ПАУЗА"); break;
                case "RESUME": case "ПРОДОЛЖИТЬ": text.text = L("RESUME", "ПРОДОЛЖИТЬ"); break;
                case "RESTART CHAPTER": case "ПЕРЕЗАПУСТИТЬ ГЛАВУ": text.text = L("RESTART CHAPTER", "ПЕРЕЗАПУСТИТЬ ГЛАВУ"); break;
                case "MAIN MENU": case "ГЛАВНОЕ МЕНЮ": text.text = L("MAIN MENU", "ГЛАВНОЕ МЕНЮ"); break;
                case "RETRY": case "ПОВТОРИТЬ": text.text = L("RETRY", "ПОВТОРИТЬ"); break;
                case "START WAVE": case "НАЧАТЬ ВОЛНУ": text.text = L("START WAVE", "НАЧАТЬ ВОЛНУ"); break;
                case "READY": case "ГОТОВО": text.text = L("READY", "ГОТОВО"); break;
                case "APPLY": case "ПРИМЕНИТЬ": text.text = L("APPLY", "ПРИМЕНИТЬ"); break;
                case "RESET DEFAULTS": case "СБРОСИТЬ": text.text = L("RESET DEFAULTS", "СБРОСИТЬ"); break;
            }
        }
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
    }

    void TogglePause(){ if (paused) Resume(); else Pause(); }
    void Pause(){ paused = true; Time.timeScale = 0f; pauseMenu.SetActive(true); }
    void Resume(){ paused = false; CombatControlsUI.ResumeConfiguredSpeed(); pauseMenu.SetActive(false); settingsMenu.SetActive(false); }
    void ShowSettingsFromMain(){ mainMenu.SetActive(false); SyncSettingsUi(); settingsMenu.SetActive(true); }
    void ShowSettingsFromPause(){ pauseMenu.SetActive(false); SyncSettingsUi(); settingsMenu.SetActive(true); }
    void BackFromSettings(){ settingsMenu.SetActive(false); if (levelStarted && paused) pauseMenu.SetActive(true); else mainMenu.SetActive(true); }

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
        Scene s = SceneManager.GetActiveScene();
        RuntimeFileLogger.Event("MENU", $"Reloading scene buildIndex={s.buildIndex}, name={s.name}");
        Time.timeScale = 1f;
        foreach (Button button in canvas.GetComponentsInChildren<Button>(true))
            button.interactable = false;

        AsyncOperation op = s.buildIndex >= 0
            ? SceneManager.LoadSceneAsync(s.buildIndex)
            : SceneManager.LoadSceneAsync(s.name);
        if (op == null)
        {
            RuntimeFileLogger.Event("MENU", "Scene reload failed to start");
            reloading = false;
            yield break;
        }
        while (!op.isDone) yield return null;
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
        Image img = go.AddComponent<Image>();
        img.color = color;
        StretchToParent(img.rectTransform);
        return go;
    }

    GameObject MakeMainMenuScreen()
    {
        GameObject go = new GameObject("MainMenu");
        go.transform.SetParent(canvas.transform, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        StretchToParent(rt);

        Texture2D background = Resources.Load<Texture2D>(MainMenuBackgroundResource);
        if (background != null)
        {
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(go.transform, false);
            Image bgImage = bg.AddComponent<Image>();
            bgImage.sprite = Sprite.Create(background, new Rect(0f, 0f, background.width, background.height), new Vector2(.5f, .5f));
            bgImage.preserveAspect = false;
            bgImage.type = Image.Type.Simple;
            StretchToParent(bgImage.rectTransform);
            AspectRatioFitter fitter = bg.AddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            fitter.aspectRatio = (float)background.width / background.height;
        }

        GameObject shade = new GameObject("CinematicShade");
        shade.transform.SetParent(go.transform, false);
        Image shadeImage = shade.AddComponent<Image>();
        shadeImage.color = new Color(.018f, .008f, .004f, .28f);
        StretchToParent(shadeImage.rectTransform);

        return go;
    }

    GameObject MakePanel(Transform parent, string name, Vector2 anchor, Vector2 size, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = color;
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = anchor;
        rt.sizeDelta = size;

        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(.73f, .43f, .16f, .45f);
        outline.effectDistance = new Vector2(2f, -2f);
        return go;
    }

    void StretchToParent(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    Text AddTitle(Transform parent, string text, Vector2 pos, int size, MenuTextStyle style = MenuTextStyle.Normal, Vector2? customSize = null)
    {
        GameObject go = new GameObject(string.IsNullOrEmpty(text) ? "Text" : text);
        go.transform.SetParent(parent, false);
        Text t = go.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.text = text;
        t.fontSize = size;
        t.fontStyle = style == MenuTextStyle.Muted ? FontStyle.Normal : FontStyle.Bold;
        t.color = TextColor(style);
        t.alignment = TextAnchor.MiddleCenter;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Truncate;
        RectTransform rt = t.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = customSize ?? new Vector2(1000, 80);
        return t;
    }

    void AddDivider(Transform parent, Vector2 pos, float width)
    {
        GameObject go = new GameObject("Divider");
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = new Color(.82f, .49f, .19f, .55f);
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(width, 2f);
    }

    void AddSectionLabel(Transform parent, string label, Vector2 pos)
    {
        Text text = AddTitle(parent, label, pos, 18, MenuTextStyle.Subtitle, new Vector2(470, 40));
        text.alignment = TextAnchor.MiddleLeft;
    }

    Slider AddSliderRow(Transform parent, string label, Vector2 pos, float value, UnityEngine.Events.UnityAction<float> onChanged, out Text valueText)
    {
        Text labelText = AddTitle(parent, label, pos + new Vector2(-65, 30), 18, MenuTextStyle.Normal, new Vector2(330, 34));
        labelText.alignment = TextAnchor.MiddleLeft;

        GameObject sliderObj = new GameObject(label + " Slider");
        sliderObj.transform.SetParent(parent, false);
        Slider slider = sliderObj.AddComponent<Slider>();
        RectTransform rt = sliderObj.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos + new Vector2(-25, -12);
        rt.sizeDelta = new Vector2(330, 28);

        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(sliderObj.transform, false);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(.20f, .12f, .08f, 1f);
        StretchToParent(bgImage.rectTransform);

        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fa = fillArea.AddComponent<RectTransform>();
        StretchToParent(fa);
        fa.offsetMin = new Vector2(4, 7);
        fa.offsetMax = new Vector2(-4, -7);

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(.75f, .22f, .08f, 1f);
        StretchToParent(fillImage.rectTransform);
        slider.fillRect = fillImage.rectTransform;

        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderObj.transform, false);
        RectTransform ha = handleArea.AddComponent<RectTransform>();
        StretchToParent(ha);
        ha.offsetMin = new Vector2(8, 0);
        ha.offsetMax = new Vector2(-8, 0);

        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = new Color(1f, .72f, .28f, 1f);
        RectTransform hr = handleImage.rectTransform;
        hr.sizeDelta = new Vector2(22, 38);
        slider.handleRect = hr;
        slider.targetGraphic = handleImage;
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = value;
        slider.onValueChanged.AddListener(onChanged);

        valueText = AddTitle(parent, Mathf.RoundToInt(value * 100f) + "%", pos + new Vector2(195, -12), 18, MenuTextStyle.Subtitle, new Vector2(80, 34));
        return slider;
    }

    Text AddSelectorRow(Transform parent, string label, Vector2 pos, UnityEngine.Events.UnityAction action)
    {
        Text labelText = AddTitle(parent, label, pos + new Vector2(-70, 28), 18, MenuTextStyle.Normal, new Vector2(330, 34));
        labelText.alignment = TextAnchor.MiddleLeft;
        Button button = AddButton(parent, "", pos + new Vector2(10, -15), action, new Vector2(430, 54), MenuButtonStyle.Stone);
        return button.GetComponentInChildren<Text>();
    }

    Button AddButton(Transform parent, string label, Vector2 pos, UnityEngine.Events.UnityAction action, Vector2? customSize = null, MenuButtonStyle style = MenuButtonStyle.Default)
    {
        GameObject go = new GameObject(string.IsNullOrEmpty(label) ? "Button" : label);
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        img.color = ButtonColor(style);
        Button b = go.AddComponent<Button>();
        b.targetGraphic = img;
        b.transition = Selectable.Transition.ColorTint;
        ColorBlock colors = b.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1.12f, 1.05f, .94f, 1f);
        colors.pressedColor = new Color(.78f, .58f, .42f, 1f);
        colors.selectedColor = new Color(1.08f, .98f, .82f, 1f);
        colors.disabledColor = new Color(.45f, .45f, .45f, .65f);
        colors.fadeDuration = .12f;
        b.colors = colors;
        b.onClick.AddListener(action);

        RectTransform rt = img.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = customSize ?? new Vector2(430, 64);

        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = style == MenuButtonStyle.Highlight ? new Color(1f, .52f, .15f, .75f) : new Color(.62f, .38f, .18f, .35f);
        outline.effectDistance = new Vector2(1f, -1f);

        Text txt = AddTitle(go.transform, label, Vector2.zero, 21, MenuTextStyle.Button);
        txt.color = style == MenuButtonStyle.Highlight ? new Color(1f, .90f, .55f, 1f)
            : style == MenuButtonStyle.Ghost ? new Color(.93f, .78f, .60f, 1f)
            : new Color(.17f, .07f, .03f, 1f);
        RectTransform tr = txt.rectTransform;
        tr.anchorMin = Vector2.zero;
        tr.anchorMax = Vector2.one;
        tr.offsetMin = tr.offsetMax = Vector2.zero;
        tr.pivot = new Vector2(.5f, .5f);
        return b;
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
