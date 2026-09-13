using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class MenuSceneNavigationFix : MonoBehaviour
{
    enum PendingNavigation
    {
        None,
        RestartChapter,
        MainMenu
    }

    static PendingNavigation pendingNavigation;
    bool navigating;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        if (FindFirstObjectByType<MenuSceneNavigationFix>() != null) return;
        new GameObject("MenuSceneNavigationFix").AddComponent<MenuSceneNavigationFix>();
    }

    IEnumerator Start()
    {
        // Runtime UI is built from Start(), and the UX enhancer rewires it one frame later.
        // Wait for the runtime graph before binding or restoring post-reload navigation.
        yield return null;
        yield return null;
        yield return null;

        RebindCriticalButtons();

        PendingNavigation requested = pendingNavigation;
        pendingNavigation = PendingNavigation.None;

        if (requested == PendingNavigation.RestartChapter)
        {
            yield return RestoreUiIfNeeded();
            StartChapterOneFromLevelButton();
        }
        else if (requested == PendingNavigation.MainMenu)
        {
            yield return RestoreUiIfNeeded();
            ForceShowMainMenu();
        }
    }

    void RebindCriticalButtons()
    {
        Canvas menuCanvas = FindMenuCanvas();
        if (menuCanvas == null) return;

        foreach (Button button in menuCanvas.GetComponentsInChildren<Button>(true))
        {
            Text label = button.GetComponentInChildren<Text>(true);
            if (label == null) continue;

            string value = label.text != null ? label.text.Trim() : string.Empty;
            if (IsRestartLabel(value))
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(RestartChapter);
            }
            else if (IsMainMenuLabel(value))
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(ReturnToMainMenu);
            }
        }
    }

    static bool IsRestartLabel(string value)
    {
        return value == "RESTART CHAPTER" ||
               value == "ПЕРЕЗАПУСТИТЬ ГЛАВУ" ||
               value == "RETRY" ||
               value == "ПОВТОРИТЬ";
    }

    static bool IsMainMenuLabel(string value)
    {
        return value == "MAIN MENU" ||
               value == "ГЛАВНОЕ МЕНЮ" ||
               value == "CHAPTER SELECT" ||
               value == "ВЫБОР ГЛАВЫ";
    }

    public void RestartChapter()
    {
        if (navigating) return;
        pendingNavigation = PendingNavigation.RestartChapter;
        RuntimeFileLogger.Event("MENU", "Restart Chapter requested through direct navigation handler");
        ReloadCurrentScene();
    }

    public void ReturnToMainMenu()
    {
        if (navigating) return;
        pendingNavigation = PendingNavigation.MainMenu;
        RuntimeFileLogger.Event("MENU", "Main Menu requested; clean reload will explicitly restore MainMenu UI");
        ReloadCurrentScene();
    }

    void ReloadCurrentScene()
    {
        navigating = true;
        Time.timeScale = 1f;

        Scene activeScene = SceneManager.GetActiveScene();
        string sceneName = activeScene.name;
        int buildIndex = activeScene.buildIndex;

        try
        {
            if (!string.IsNullOrEmpty(sceneName) && Application.CanStreamedLevelBeLoaded(sceneName))
            {
                RuntimeFileLogger.Event("MENU", $"Reloading scene by name: {sceneName}");
                SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
                return;
            }

            if (buildIndex >= 0 && Application.CanStreamedLevelBeLoaded(buildIndex))
            {
                RuntimeFileLogger.Event("MENU", $"Reloading scene by build index: {buildIndex}");
                SceneManager.LoadScene(buildIndex, LoadSceneMode.Single);
                return;
            }

            RuntimeFileLogger.Event("MENU", $"Scene reload target unavailable; name={sceneName}, buildIndex={buildIndex}");
            pendingNavigation = PendingNavigation.None;
            navigating = false;
        }
        catch (System.Exception ex)
        {
            RuntimeFileLogger.Event("MENU", $"Scene reload failed: {ex.GetType().Name}: {ex.Message}");
            Debug.LogException(ex);
            pendingNavigation = PendingNavigation.None;
            navigating = false;
        }
    }

    IEnumerator RestoreUiIfNeeded()
    {
        // Give GameBootstrap enough frames to construct the menu graph. If another
        // bootstrap component starts later in the same frame, this avoids racing it.
        for (int frame = 0; frame < 30; frame++)
        {
            if (FindFirstObjectByType<GameMenuController>() != null && FindMenuCanvas() != null)
                yield break;
            yield return null;
        }

        GameMenuController menu = FindFirstObjectByType<GameMenuController>();
        if (menu == null)
        {
            RuntimeFileLogger.Event("MENU", "Post-reload recovery: GameMenuController missing; recreating runtime menu controller");
            new GameObject("GameMenuRecovery").AddComponent<GameMenuController>();
            yield return null;
            yield return null;
        }

        if (FindMenuCanvas() == null)
            RuntimeFileLogger.Event("MENU", "Post-reload recovery failed: MenuCanvas is still missing");
    }

    void ForceShowMainMenu()
    {
        GameMenuController menu = FindFirstObjectByType<GameMenuController>();
        Canvas menuCanvas = FindMenuCanvas();

        if (menu == null || menuCanvas == null)
        {
            RuntimeFileLogger.Event("MENU", $"Main Menu restore failed. controller={(menu != null)}, canvas={(menuCanvas != null)}");
            return;
        }

        // ShowMainMenu is intentionally owned by GameMenuController. SendMessage is
        // used only as a post-reload recovery call; the button itself no longer relies
        // on the old confirmation -> SendMessage navigation chain.
        menu.SendMessage("ShowMainMenu", SendMessageOptions.RequireReceiver);

        Transform mainMenu = menuCanvas.transform.Find("MainMenu");
        bool visible = mainMenu != null && mainMenu.gameObject.activeSelf;
        RuntimeFileLogger.Event("MENU", $"Main Menu explicitly restored after reload. visible={visible}");

        if (!visible && mainMenu != null)
        {
            mainMenu.gameObject.SetActive(true);
            RuntimeFileLogger.Event("MENU", "Main Menu recovery fallback activated MainMenu root directly");
        }

        RebindCriticalButtons();
    }

    void StartChapterOneFromLevelButton()
    {
        Canvas menuCanvas = FindMenuCanvas();
        if (menuCanvas == null)
        {
            RuntimeFileLogger.Event("MENU", "Restart Chapter failed: MenuCanvas not found after reload");
            return;
        }

        foreach (Button button in menuCanvas.GetComponentsInChildren<Button>(true))
        {
            Text label = button.GetComponentInChildren<Text>(true);
            if (label == null) continue;

            string value = label.text != null ? label.text.Trim() : string.Empty;
            if (value.Contains("THE LANDING") || value.Contains("ВЫСАДКА"))
            {
                RuntimeFileLogger.Event("MENU", "Restart Chapter: invoking Chapter I level button after reload");
                button.onClick.Invoke();
                return;
            }
        }

        RuntimeFileLogger.Event("MENU", "Restart Chapter failed: Chapter I level button not found after reload");
    }

    static Canvas FindMenuCanvas()
    {
        GameObject menuCanvas = GameObject.Find("MenuCanvas");
        return menuCanvas != null ? menuCanvas.GetComponent<Canvas>() : null;
    }
}
