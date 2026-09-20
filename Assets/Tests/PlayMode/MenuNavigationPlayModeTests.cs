using System.Collections;
using System.Linq;
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
    public IEnumerator RestartChapter_RebuildsSingleRuntimeGraphAndEventSystem()
    {
        GameBootstrap previousBootstrap = Object.FindFirstObjectByType<GameBootstrap>();
        Assert.NotNull(previousBootstrap);
        Assert.NotNull(previousBootstrap.Runtime);

        GameRuntimeContext previousRuntime = previousBootstrap.Runtime;
        GameMenuController previousMenu = previousRuntime.Menu;
        Assert.NotNull(previousMenu);

        if (GameManager.Instance != null && GameManager.Instance.GiftAvailable)
            Assert.IsTrue(GameManager.Instance.UseGift(DivineGiftType.Athena));

        previousMenu.RestartChapter();
        yield return WaitForReplacementMenu(previousMenu);

        GameBootstrap bootstrap = Object.FindFirstObjectByType<GameBootstrap>();
        Assert.NotNull(bootstrap);
        Assert.AreNotSame(previousBootstrap, bootstrap);
        Assert.NotNull(bootstrap.Runtime);
        Assert.AreNotSame(previousRuntime, bootstrap.Runtime);

        GameBootstrap[] bootstraps = Resources.FindObjectsOfTypeAll<GameBootstrap>()
            .Where(x => x != null && x.gameObject.scene.IsValid()).ToArray();
        RuntimeInputBootstrap[] inputs = Resources.FindObjectsOfTypeAll<RuntimeInputBootstrap>()
            .Where(x => x != null && x.gameObject.scene.IsValid()).ToArray();
        UnityEngine.EventSystems.EventSystem[] eventSystems =
            Resources.FindObjectsOfTypeAll<UnityEngine.EventSystems.EventSystem>()
                .Where(x => x != null && x.gameObject.scene.IsValid()).ToArray();

        Assert.AreEqual(1, bootstraps.Length, "Restart must leave one GameBootstrap.");
        Assert.AreEqual(1, inputs.Length, "Restart must leave one RuntimeInputBootstrap.");
        Assert.AreEqual(1, eventSystems.Length, "Restart must leave one EventSystem.");

        Assert.AreSame(GameManager.Instance, bootstrap.Runtime.Game);
        Assert.AreSame(EnemySpawner.Instance, bootstrap.Runtime.Spawner);
        Assert.AreSame(TowerPlacement.Instance, bootstrap.Runtime.Chapter.Placement);
        Assert.AreSame(HectorController.Instance, bootstrap.Runtime.Chapter.Hector);
        Assert.AreSame(GameMenuController.Instance, bootstrap.Runtime.Menu);
        Assert.AreSame(ModernCombatHud.Instance, bootstrap.Runtime.CombatHud);
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
