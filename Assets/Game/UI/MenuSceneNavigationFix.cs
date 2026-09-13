using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class MenuSceneNavigationFix : MonoBehaviour
{
    enum PendingNavigation
    {
        None,
        RestartChapter
    }

    static PendingNavigation pendingNavigation;
    static bool bootstrapInstalled;
    bool navigating;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        if (FindFirstObjectByType<MenuSceneNavigationFix>() != null) return;
        new GameObject("MenuSceneNavigationFix").AddComponent<MenuSceneNavigationFix>();
        bootstrapInstalled = true;
    }

    IEnumerator Start()
    {
        // GameMenuController builds its runtime canvas in Start and GameMenuUxEnhancer
        // rewires buttons one frame later. Wait until both have finished, then install
        // deterministic navigation handlers for the two critical scene actions.
        yield return null;
        yield return null;
        yield return null;

        RebindCriticalButtons();

        if (pendingNavigation == PendingNavigation.RestartChapter)
        {
            pendingNavigation = PendingNavigation.None;
            yield return null;
            StartChapterOneFromLevelButton();
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
            if (value == "RESTART CHAPTER" || value == "ПЕРЕЗАПУСТИТЬ ГЛАВУ")
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(RestartChapter);
            }
            else if (value == "MAIN MENU" || value == "ГЛАВНОЕ МЕНЮ")
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(ReturnToMainMenu);
            }
        }
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
        pendingNavigation = PendingNavigation.None;
        RuntimeFileLogger.Event("MENU", "Main Menu requested through direct navigation handler");
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
            navigating = false;
        }
        catch (System.Exception ex)
        {
            RuntimeFileLogger.Event("MENU", $"Scene reload failed: {ex.GetType().Name}: {ex.Message}");
            Debug.LogException(ex);
            navigating = false;
        }
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
