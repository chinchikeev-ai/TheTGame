using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class GameMenuController : MonoBehaviour
{
    Canvas canvas;
    EnemySpawner spawner;
    GameObject mainMenu, levelMenu, settingsMenu, pauseMenu, endMenu;
    Button startWaveButton;
    Text countdownText, endTitle;
    bool levelStarted;
    bool paused;

    string L(string en, string ru) => GameLanguage.T(en, ru);

    void Start()
    {
        spawner = FindFirstObjectByType<EnemySpawner>();
        BuildUI();
        ShowMainMenu();
    }

    void Update()
    {
        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();
        if (levelStarted && GameManager.Instance != null && !GameManager.Instance.GameEnded && ReadEscape())
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

    bool ReadEscape()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
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
        canvasObj.AddComponent<GraphicRaycaster>();

        mainMenu = MakeScreen("MainMenu", new Color(.02f,.03f,.05f,.97f));
        AddTitle(mainMenu.transform, "THE TROY GAME", new Vector2(0,180), 62);
        AddButton(mainMenu.transform, L("PLAY", "ИГРАТЬ"), new Vector2(0,55), ShowLevels);
        AddButton(mainMenu.transform, L("SETTINGS", "НАСТРОЙКИ"), new Vector2(0,-25), ShowSettingsFromMain);
        AddButton(mainMenu.transform, L("EXIT", "ВЫХОД"), new Vector2(0,-105), QuitGame);

        levelMenu = MakeScreen("LevelSelect", new Color(.02f,.03f,.05f,.97f));
        AddTitle(levelMenu.transform, L("LEVEL SELECT", "ВЫБОР УРОВНЯ"), new Vector2(0,190), 50);
        AddButton(levelMenu.transform, L("LEVEL 1 - TWIN ROUTE", "УРОВЕНЬ 1 - ДВА ПУТИ"), new Vector2(0,55), StartLevel);
        AddButton(levelMenu.transform, L("LEVEL 2 - LOCKED", "УРОВЕНЬ 2 - ЗАКРЫТ"), new Vector2(0,-35), delegate { });
        AddButton(levelMenu.transform, L("BACK", "НАЗАД"), new Vector2(0,-155), ShowMainMenu);

        settingsMenu = MakeScreen("Settings", new Color(.02f,.03f,.05f,.97f));
        AddTitle(settingsMenu.transform, L("SETTINGS", "НАСТРОЙКИ"), new Vector2(0,215), 50);
        AddButton(settingsMenu.transform, L("VOLUME +", "ГРОМКОСТЬ +"), new Vector2(0,105), delegate { AudioListener.volume = Mathf.Clamp01(AudioListener.volume + .1f); });
        AddButton(settingsMenu.transform, L("VOLUME -", "ГРОМКОСТЬ -"), new Vector2(0,30), delegate { AudioListener.volume = Mathf.Clamp01(AudioListener.volume - .1f); });
        AddButton(settingsMenu.transform, L("FULLSCREEN", "ПОЛНЫЙ ЭКРАН"), new Vector2(0,-45), delegate { Screen.fullScreen = !Screen.fullScreen; });
        AddButton(settingsMenu.transform, GameLanguage.Russian ? "LANGUAGE: РУССКИЙ" : "LANGUAGE: ENGLISH", new Vector2(0,-120), ToggleLanguage);
        AddButton(settingsMenu.transform, L("BACK", "НАЗАД"), new Vector2(0,-195), BackFromSettings);

        pauseMenu = MakeScreen("PauseMenu", new Color(.02f,.03f,.05f,.9f));
        AddTitle(pauseMenu.transform, L("PAUSED", "ПАУЗА"), new Vector2(0,210), 54);
        AddButton(pauseMenu.transform, L("RESUME", "ПРОДОЛЖИТЬ"), new Vector2(0,95), Resume);
        AddButton(pauseMenu.transform, L("SETTINGS", "НАСТРОЙКИ"), new Vector2(0,20), ShowSettingsFromPause);
        AddButton(pauseMenu.transform, L("RESTART", "ПЕРЕЗАПУСК"), new Vector2(0,-55), RestartScene);
        AddButton(pauseMenu.transform, L("MAIN MENU", "ГЛАВНОЕ МЕНЮ"), new Vector2(0,-130), ShowMainMenu);
        AddButton(pauseMenu.transform, L("EXIT", "ВЫХОД"), new Vector2(0,-205), QuitGame);

        endMenu = MakeScreen("EndMenu", new Color(.02f,.03f,.05f,.92f));
        endTitle = AddTitle(endMenu.transform, L("RESULT", "РЕЗУЛЬТАТ"), new Vector2(0,145), 60);
        AddButton(endMenu.transform, L("RETRY", "ПОВТОРИТЬ"), new Vector2(0,-20), RestartScene);
        AddButton(endMenu.transform, L("MAIN MENU", "ГЛАВНОЕ МЕНЮ"), new Vector2(0,-105), ShowMainMenu);

        GameObject wavePanel = new GameObject("WaveControls");
        wavePanel.transform.SetParent(canvas.transform, false);
        RectTransform wr = wavePanel.AddComponent<RectTransform>();
        wr.anchorMin = wr.anchorMax = wr.pivot = new Vector2(.5f,0);
        wr.anchoredPosition = new Vector2(0,150);
        wr.sizeDelta = new Vector2(340,100);
        startWaveButton = AddButton(wavePanel.transform, L("START WAVE", "НАЧАТЬ ВОЛНУ"), new Vector2(0,20), delegate { if (spawner != null) spawner.StartWaveNow(); }, new Vector2(250,56));
        countdownText = AddTitle(wavePanel.transform, L("READY", "ГОТОВО"), new Vector2(0,-28), 18);
        startWaveButton.gameObject.SetActive(false);

        levelMenu.SetActive(false); settingsMenu.SetActive(false); pauseMenu.SetActive(false); endMenu.SetActive(false);
    }

    void ToggleLanguage()
    {
        GameLanguage.Toggle();
        RestartScene();
    }

    void StartLevel()
    {
        levelStarted = true; paused = false; Time.timeScale = 1f;
        mainMenu.SetActive(false); levelMenu.SetActive(false); settingsMenu.SetActive(false); pauseMenu.SetActive(false); endMenu.SetActive(false);
        if (spawner != null) spawner.ActivateLevel();
    }

    void ShowMainMenu()
    {
        Time.timeScale = 0f; paused = false;
        if (mainMenu != null) mainMenu.SetActive(true);
        if (levelMenu != null) levelMenu.SetActive(false);
        if (settingsMenu != null) settingsMenu.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (endMenu != null) endMenu.SetActive(false);
    }

    void ShowLevels(){ mainMenu.SetActive(false); levelMenu.SetActive(true); }
    void TogglePause(){ if (paused) Resume(); else Pause(); }
    void Pause(){ paused = true; Time.timeScale = 0f; pauseMenu.SetActive(true); }
    void Resume(){ paused = false; Time.timeScale = 1f; pauseMenu.SetActive(false); settingsMenu.SetActive(false); }
    void ShowSettingsFromMain(){ mainMenu.SetActive(false); settingsMenu.SetActive(true); }
    void ShowSettingsFromPause(){ pauseMenu.SetActive(false); settingsMenu.SetActive(true); }
    void BackFromSettings(){ settingsMenu.SetActive(false); if (levelStarted && paused) pauseMenu.SetActive(true); else mainMenu.SetActive(true); }
    void ShowEnd()
    {
        Time.timeScale = 0f;
        paused = true;
        bool victory = GameManager.Instance.EndMessage == "VICTORY";
        endTitle.text = victory ? L("VICTORY", "ПОБЕДА") : L("GAME OVER", "ПОРАЖЕНИЕ");
        endMenu.SetActive(true);
    }
    void RestartScene(){ Time.timeScale = 1f; Scene s = SceneManager.GetActiveScene(); if (!string.IsNullOrEmpty(s.name)) SceneManager.LoadScene(s.name); }

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
        GameObject go = new GameObject(name); go.transform.SetParent(canvas.transform,false);
        Image img = go.AddComponent<Image>(); img.color = color;
        RectTransform rt = img.rectTransform; rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = rt.offsetMax = Vector2.zero;
        return go;
    }

    Text AddTitle(Transform parent, string text, Vector2 pos, int size)
    {
        GameObject go = new GameObject(text); go.transform.SetParent(parent,false);
        Text t = go.AddComponent<Text>(); t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); t.text = text; t.fontSize = size; t.fontStyle = FontStyle.Bold; t.color = Color.white; t.alignment = TextAnchor.MiddleCenter;
        RectTransform rt = t.rectTransform; rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f,.5f); rt.anchoredPosition = pos; rt.sizeDelta = new Vector2(1000,80);
        return t;
    }

    Button AddButton(Transform parent, string label, Vector2 pos, UnityEngine.Events.UnityAction action, Vector2? customSize = null)
    {
        GameObject go = new GameObject(label); go.transform.SetParent(parent,false);
        Image img = go.AddComponent<Image>(); img.color = new Color(.15f,.25f,.38f,.98f);
        Button b = go.AddComponent<Button>(); b.targetGraphic = img; b.onClick.AddListener(action);
        RectTransform rt = img.rectTransform; rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f,.5f); rt.anchoredPosition = pos; rt.sizeDelta = customSize ?? new Vector2(400,64);
        Text txt = AddTitle(go.transform,label,Vector2.zero,18); RectTransform tr = txt.rectTransform; tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one; tr.offsetMin = tr.offsetMax = Vector2.zero; tr.pivot = new Vector2(.5f,.5f);
        return b;
    }
}
