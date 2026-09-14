using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public sealed class MenuNavigationPlayModeTests
{
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        Time.timeScale = 1f;
        Scene scene = SceneManager.GetActiveScene();
        Assert.IsFalse(string.IsNullOrEmpty(scene.name), "PlayMode navigation tests require a named scene.");

        SceneManager.LoadScene(scene.name, LoadSceneMode.Single);
        yield return null;
        yield return null;
        yield return null;

        Assert.NotNull(Object.FindFirstObjectByType<GameMenuController>(), "GameMenuController was not rebuilt during test setup.");
        Assert.NotNull(GameObject.Find("MenuCanvas"), "MenuCanvas was not rebuilt during test setup.");
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Time.timeScale = 1f;
        yield return null;
    }

    [UnityTest]
    public IEnumerator RestartChapter_ReloadsScene_AndStartsFreshChapter()
    {
        GameMenuController previousMenu = Object.FindFirstObjectByType<GameMenuController>();
        Assert.NotNull(previousMenu);

        previousMenu.RestartChapter();
        yield return WaitForReplacementMenu(previousMenu);

        GameMenuController menu = Object.FindFirstObjectByType<GameMenuController>();
        GameObject canvas = GameObject.Find("MenuCanvas");
        Assert.NotNull(menu, "Restart reload did not rebuild GameMenuController.");
        Assert.NotNull(canvas, "Restart reload did not rebuild MenuCanvas.");
        Transform mainMenu = canvas.transform.Find("MainMenu");
        Transform levelSelect = canvas.transform.Find("LevelSelect");
        Assert.NotNull(mainMenu);
        Assert.NotNull(levelSelect);
        Assert.IsFalse(mainMenu.gameObject.activeSelf, "Restart must not return to the main menu.");
        Assert.IsFalse(levelSelect.gameObject.activeSelf, "Restart must not stop at chapter select.");
        Assert.Greater(Time.timeScale, 0f, "Restarted chapter must resume simulation instead of remaining paused on a blank frame.");
        Assert.NotNull(GameManager.Instance, "Restarted chapter did not rebuild GameManager.");
        Assert.NotNull(Object.FindFirstObjectByType<EnemySpawner>(), "Restarted chapter did not rebuild EnemySpawner.");
    }

    [UnityTest]
    public IEnumerator MainMenu_FromActiveChapter_ReloadsScene_AndShowsMainMenu()
    {
        GameMenuController previousMenu = Object.FindFirstObjectByType<GameMenuController>();
        Assert.NotNull(previousMenu);

        previousMenu.SendMessage("StartLevel", SendMessageOptions.RequireReceiver);
        yield return null;
        Assert.Greater(Time.timeScale, 0f, "Chapter did not start before main-menu navigation test.");

        previousMenu.SendMessage("ReturnToMainMenu", SendMessageOptions.RequireReceiver);
        yield return WaitForReplacementMenu(previousMenu);

        GameMenuController menu = Object.FindFirstObjectByType<GameMenuController>();
        GameObject canvas = GameObject.Find("MenuCanvas");
        Assert.NotNull(menu, "Main-menu reload did not rebuild GameMenuController.");
        Assert.NotNull(canvas, "Main-menu reload did not rebuild MenuCanvas.");
        Transform mainMenu = canvas.transform.Find("MainMenu");
        Transform levelSelect = canvas.transform.Find("LevelSelect");
        Transform pauseMenu = canvas.transform.Find("PauseMenu");
        Transform endMenu = canvas.transform.Find("EndMenu");
        Assert.NotNull(mainMenu);
        Assert.NotNull(levelSelect);
        Assert.NotNull(pauseMenu);
        Assert.NotNull(endMenu);
        Assert.IsTrue(mainMenu.gameObject.activeSelf, "Main Menu navigation finished with no visible main menu.");
        Assert.IsFalse(levelSelect.gameObject.activeSelf);
        Assert.IsFalse(pauseMenu.gameObject.activeSelf);
        Assert.IsFalse(endMenu.gameObject.activeSelf);
        Assert.AreEqual(0f, Time.timeScale, .001f, "Main menu should leave gameplay paused after the clean reload.");
    }

    static IEnumerator WaitForReplacementMenu(GameMenuController previousMenu)
    {
        float timeout = Time.realtimeSinceStartup + 5f;
        while (Time.realtimeSinceStartup < timeout)
        {
            GameMenuController current = Object.FindFirstObjectByType<GameMenuController>();
            if (current != null && current != previousMenu && GameObject.Find("MenuCanvas") != null)
            {
                yield return null;
                yield return null;
                yield break;
            }
            yield return null;
        }

        Assert.Fail("Scene reload did not produce a replacement GameMenuController and MenuCanvas within timeout.");
    }
}
