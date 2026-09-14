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
    public IEnumerator TowerPlacement_BuildModeRequiresExplicitSelection()
    {
        TowerPlacement placement = Object.FindFirstObjectByType<TowerPlacement>();
        Assert.NotNull(placement);
        Assert.IsFalse(placement.BuildModeActive);

        placement.SelectBuildType(TowerType.MachineGun);
        yield return null;
        Assert.IsTrue(placement.BuildModeActive);
        Assert.AreEqual(TowerType.MachineGun, placement.SelectedBuildType);

        placement.CancelBuildMode();
        yield return null;
        Assert.IsFalse(placement.BuildModeActive);
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
    public IEnumerator Hector_RemainsInsidePlayableBounds()
    {
        HectorController hector = Object.FindFirstObjectByType<HectorController>();
        Assert.NotNull(hector);

        hector.transform.position = new Vector3(999f, hector.transform.position.y, -999f);
        yield return null;

        Vector3 min = MapBuilder.CellToWorld(new Vector2Int(0, 0));
        Vector3 max = MapBuilder.CellToWorld(new Vector2Int(MapBuilder.GridWidth - 1, MapBuilder.GridHeight - 1));
        Assert.GreaterOrEqual(hector.transform.position.x, min.x + hector.battlefieldMargin - .01f);
        Assert.LessOrEqual(hector.transform.position.x, max.x - hector.battlefieldMargin + .01f);
        Assert.GreaterOrEqual(hector.transform.position.z, min.z + hector.battlefieldMargin - .01f);
        Assert.LessOrEqual(hector.transform.position.z, max.z - hector.battlefieldMargin + .01f);
    }

    [UnityTest]
    public IEnumerator TrojanGuard_BlockCapacityNeverExceedsThree_AndRefills()
    {
        Assert.NotNull(GameManager.Instance);
        GameManager.Instance.BeginRun();

        GameObject guardObject = new GameObject("Acceptance Trojan Guard");
        TrojanGuardSquad guard = guardObject.AddComponent<TrojanGuardSquad>();
        guard.transform.position = Vector3.zero;
        guard.blockCapacity = 3;
        guard.blockRadius = 2f;
        guard.Initialize(null);

        List<Enemy> enemies = new List<Enemy>();
        for (int i = 0; i < 7; i++)
        {
            GameObject enemyObject = new GameObject($"Acceptance Enemy {i}");
            enemyObject.transform.position = new Vector3((i - 3) * .18f, 0f, .35f);
            Enemy enemy = enemyObject.AddComponent<Enemy>();
            enemy.InitFromData(new Transform[0], null, 1f, 1f);
            enemies.Add(enemy);
        }

        yield return null;
        yield return null;

        int blocked = 0;
        Enemy firstBlocked = null;
        foreach (Enemy enemy in enemies)
        {
            if (enemy == null || !enemy.IsBlockedByGuard) continue;
            blocked++;
            if (firstBlocked == null) firstBlocked = enemy;
        }
        Assert.AreEqual(3, guard.BlockedCount);
        Assert.AreEqual(3, blocked);

        Assert.NotNull(firstBlocked);
        Object.Destroy(firstBlocked.gameObject);
        yield return null;
        yield return null;

        Assert.LessOrEqual(guard.BlockedCount, guard.blockCapacity);
        Assert.AreEqual(3, guard.BlockedCount, "Guard should refill a released block slot from nearby enemies.");

        foreach (Enemy enemy in enemies)
            if (enemy != null) Object.Destroy(enemy.gameObject);
        Object.Destroy(guardObject);
    }

    [UnityTest]
    public IEnumerator EnemyDeath_UnregistersImmediately_ButKeepsBriefPresentation()
    {
        Assert.NotNull(GameManager.Instance);
        GameManager.Instance.BeginRun();

        GameObject enemyObject = new GameObject("Acceptance Death Enemy");
        Enemy enemy = enemyObject.AddComponent<Enemy>();
        enemy.InitFromData(new Transform[0], null, 1f, 1f);
        int aliveBefore = EnemyRegistry.AliveCount;

        enemy.TakeDamage(100000f);

        Assert.AreEqual(aliveBefore - 1, EnemyRegistry.AliveCount, "Dying enemies must stop counting toward wave completion immediately.");
        Assert.IsTrue(enemy != null, "Enemy should remain briefly for its death presentation.");

        yield return new WaitForSeconds(.85f);
        Assert.IsTrue(enemy == null, "Enemy should be destroyed after its short death presentation.");
    }

    [UnityTest]
    public IEnumerator FinalEncounterData_ContainsMenelausBoss()
    {
        ChapterData chapter = Resources.Load<ChapterData>("Chapters/Chapter01_Landing");
        EncounterData finalEncounter = chapter != null ? chapter.GetEncounter(5) : null;
        EnemyData boss = BalanceCatalog.GetEnemy(EnemyArchetype.Boss);
        yield return null;

        Assert.NotNull(chapter);
        Assert.NotNull(finalEncounter);
        Assert.IsTrue(finalEncounter.HasBoss);
        Assert.IsTrue(finalEncounter.Contains(EnemyArchetype.Boss));
        Assert.NotNull(boss);
        Assert.AreEqual("menelaus", boss.id);
    }
}
