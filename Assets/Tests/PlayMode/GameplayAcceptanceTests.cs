using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class GameplayAcceptanceTests
{
    string testRoot;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        testRoot = Path.Combine(Path.GetTempPath(), "TheTroyGame_PlayModeTests", System.Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(testRoot);
        CampaignSave.ConfigureStorageForTests(testRoot);
        CampaignSave.ResetProgress();
        Time.timeScale = 1f;

        Scene scene = SceneManager.GetActiveScene();
        if (!string.IsNullOrEmpty(scene.name))
        {
            SceneManager.LoadScene(scene.name);
            yield return null;
            yield return null;
        }
        else
        {
            yield return null;
        }
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Time.timeScale = 1f;
        CampaignSave.ClearStorageOverrideForTests();
        if (Directory.Exists(testRoot)) Directory.Delete(testRoot, true);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ChapterOne_CanStartAndCompleteFirstWave()
    {
        EnemySpawner spawner = Object.FindFirstObjectByType<EnemySpawner>();
        Assert.NotNull(spawner);
        Assert.NotNull(GameManager.Instance);

        Time.timeScale = 100f;
        GameManager.Instance.BeginRun();
        spawner.ActivateLevel();
        spawner.StartWaveNow();

        float timeout = Time.realtimeSinceStartup + 6f;
        bool started = false;
        bool completed = false;

        while (Time.realtimeSinceStartup < timeout)
        {
            if (spawner.CurrentWave >= 1) started = true;

            List<Enemy> enemies = new List<Enemy>(EnemyRegistry.All);
            foreach (Enemy enemy in enemies)
                if (enemy != null) Object.Destroy(enemy.gameObject);

            if (started && spawner.CurrentWave == 1 && !spawner.WaveActive)
            {
                completed = true;
                break;
            }
            yield return null;
        }

        Assert.IsTrue(started, "Wave 1 never started.");
        Assert.IsTrue(completed, "Wave 1 did not complete inside the acceptance-test timeout.");
    }

    [UnityTest]
    public IEnumerator Victory_RequiresMenelausDefeat_AndUnlocksChapterTwo()
    {
        Assert.NotNull(GameManager.Instance);
        Assert.IsFalse(CampaignSave.IsUnlocked(2));

        GameManager.Instance.BeginRun();
        GameManager.Instance.RecordBossDefeated();
        GameManager.Instance.WinGame();
        yield return null;

        Assert.IsTrue(GameManager.Instance.GameEnded);
        Assert.AreEqual("VICTORY", GameManager.Instance.EndMessage);
        Assert.IsTrue(GameManager.Instance.BossDefeated);
        Assert.IsTrue(CampaignSave.IsUnlocked(2));
        Assert.IsTrue(CampaignSave.IsCompleted(1));
    }

    [UnityTest]
    public IEnumerator VictoryWithoutMenelausDefeat_IsRejected()
    {
        Assert.NotNull(GameManager.Instance);
        GameManager.Instance.BeginRun();
        GameManager.Instance.WinGame();
        yield return null;

        Assert.IsTrue(GameManager.Instance.GameEnded);
        Assert.AreEqual("GAME OVER", GameManager.Instance.EndMessage);
        Assert.IsFalse(GameManager.Instance.BossDefeated);
        Assert.IsFalse(CampaignSave.IsUnlocked(2));
    }

    [UnityTest]
    public IEnumerator MenelausGateBreach_DamagesGateUntilZero()
    {
        Assert.NotNull(GameManager.Instance);
        int hpBefore = GameManager.Instance.BaseHealth;
        GameManager.Instance.BeginRun();
        GameManager.Instance.BossReachedGate(2);
        yield return null;

        Assert.IsFalse(GameManager.Instance.GameEnded);
        Assert.IsFalse(GameManager.Instance.BossBreached);
        Assert.AreEqual(hpBefore - 2, GameManager.Instance.BaseHealth);

        GameManager.Instance.BossReachedGate(GameManager.Instance.MaxBaseHealth + 1000);
        yield return null;

        Assert.IsTrue(GameManager.Instance.GameEnded);
        Assert.AreEqual("GAME OVER", GameManager.Instance.EndMessage);
        Assert.IsTrue(GameManager.Instance.BossBreached);
        Assert.AreEqual(0, GameManager.Instance.BaseHealth);
        Assert.IsFalse(CampaignSave.IsUnlocked(2));
    }

    [UnityTest]
    public IEnumerator Defeat_DoesNotUnlockChapterTwo()
    {
        Assert.NotNull(GameManager.Instance);
        Assert.IsFalse(CampaignSave.IsUnlocked(2));

        GameManager.Instance.BeginRun();
        GameManager.Instance.DamageBase(GameManager.Instance.MaxBaseHealth + 1000);
        yield return null;

        Assert.IsTrue(GameManager.Instance.GameEnded);
        Assert.AreEqual("GAME OVER", GameManager.Instance.EndMessage);
        Assert.IsFalse(CampaignSave.IsUnlocked(2));
    }

    [UnityTest]
    public IEnumerator LanguageSwitch_DoesNotReloadScene()
    {
        Scene beforeScene = SceneManager.GetActiveScene();
        ulong beforeHandle = beforeScene.handle.GetRawData();
        string beforeCode = GameLanguage.Code;

        GameLanguage.Toggle();
        yield return null;

        Assert.AreEqual(beforeHandle, SceneManager.GetActiveScene().handle.GetRawData());
        Assert.AreNotEqual(beforeCode, GameLanguage.Code);
    }

    [UnityTest]
    public IEnumerator Hector_QERF_ExecuteWithoutExceptions()
    {
        HectorController hector = Object.FindFirstObjectByType<HectorController>();
        Assert.NotNull(hector);

        Assert.DoesNotThrow(() => hector.UseWarCry());
        Assert.DoesNotThrow(() => hector.UseShieldWall());
        Assert.DoesNotThrow(() => hector.UseSpearThrow());
        Assert.DoesNotThrow(() => hector.UseUltimate());
        yield return null;

        Assert.Greater(hector.WarCryCooldownRemaining, 0f);
        Assert.Greater(hector.ShieldWallCooldownRemaining, 0f);
        Assert.Greater(hector.UltimateCooldownRemaining, 0f);
    }

    [UnityTest]
    public IEnumerator FinalWaveData_ContainsMenelausBoss()
    {
        WaveData finalWave = BalanceCatalog.GetWave(5, 5);
        EnemyData boss = BalanceCatalog.GetEnemy(EnemyArchetype.Boss);
        yield return null;

        Assert.NotNull(finalWave);
        Assert.IsTrue(finalWave.hasBoss);
        Assert.NotNull(boss);
        Assert.AreEqual("menelaus", boss.id);
    }
}
