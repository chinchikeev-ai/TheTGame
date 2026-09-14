#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ChapterOneReleaseValidator
{
    static readonly string[] RequiredPresentationScripts =
    {
        "Assets/Game/Campaign/Runtime/ChapterRuntimeInstaller.cs",
        "Assets/Game/Campaign/Runtime/ChapterOneRuntimeInstaller.cs",
        "Assets/Game/World/CoastEnvironmentBuilder.cs",
        "Assets/Game/World/LandingPresentation.cs",
        "Assets/Game/World/ChapterOneVisualEnhancer.cs",
        "Assets/Game/World/TroyGateHeroBuilder.cs",
        "Assets/Game/World/ChapterOneWallLife.cs",
        "Assets/Game/World/ChapterOneAtmosphereController.cs",
        "Assets/Game/World/ChapterOneCinematicCamera.cs",
        "Assets/Game/UI/BossHUD.cs",
        "Assets/Game/UI/ChapterFlowUI.cs",
        "Assets/Game/Characters/CharacterPresentationState.cs",
        "Assets/Game/Heroes/Hector/HectorPresentationBridge.cs",
        "Assets/Game/Towers/TowerCrewAnimationBridge.cs",
        "Assets/Game/Towers/TowerSupportMechanismPresentation.cs",
        "Assets/Editor/ChapterOneCharacterAnimationBuilder.cs"
    };

    static readonly string[] RequiredChapterRuntimeTokens =
    {
        "ChapterOneVisualEnhancer.Enhance()",
        "TroyGateHeroBuilder.Build()",
        "ChapterOneWallLife.Build()",
        "ChapterOneAtmosphereController",
        "ChapterOnePlaythroughReporter",
        "LandingPresentation",
        "ChapterOneCinematicCamera",
        "ChapterOneGuidancePresentation"
    };

    [MenuItem("TheTroyGame/Validation/Validate Chapter I Release Candidate")]
    public static void Run()
    {
        int errors = Validate(true);
        if (errors == 0)
            Debug.Log("[CHAPTER I RC] PASS - Chapter I release-candidate contract is intact.");
        else
            Debug.LogError($"[CHAPTER I RC] FAIL - {errors} release-candidate contract error(s).");
    }

    public static int Validate(bool log)
    {
        int errors = 0;
        ValidateChapterData(ref errors, log);
        ValidateEncounterData(ref errors, log);
        ValidatePresentationFiles(ref errors, log);
        ValidateBootstrapWiring(ref errors, log);
        ValidateEncounterRuntimeWiring(ref errors, log);
        ValidateProductionArtWiring(ref errors, log);
        ValidateLegacyPlaceholderFallbacks(ref errors, log);
        return errors;
    }

    static void ValidateChapterData(ref int errors, bool log)
    {
        ChapterData chapter = Resources.Load<ChapterData>("Chapters/Chapter01_Landing");
        if (chapter == null)
        {
            Fail("Missing Resources/Chapters/Chapter01_Landing ChapterData.", ref errors, log);
            return;
        }

        Check(chapter.chapterNumber == 1, "Chapter number must remain 1.", ref errors, log);
        Check(string.Equals(chapter.chapterId, "chapter_01_landing", StringComparison.Ordinal), "Chapter id must remain chapter_01_landing.", ref errors, log);
        Check(string.Equals(chapter.runtimeProfile, ChapterOneRuntimeInstaller.ProfileId, StringComparison.Ordinal),
            $"Chapter I runtimeProfile must remain {ChapterOneRuntimeInstaller.ProfileId}.", ref errors, log);
        Check(chapter.combatEvents == 5, "Chapter I must contain exactly 5 combat events.", ref errors, log);
        Check(chapter.EncounterCount == 5, "Chapter I must reference exactly 5 authored encounters.", ref errors, log);
        Check(chapter.targetDurationMinutes >= 11f && chapter.targetDurationMinutes <= 13f,
            $"Target duration must remain 11-13 minutes, current={chapter.targetDurationMinutes:0.0}.", ref errors, log);
        Check(chapter.unlockChapter == 2, "Chapter I victory must unlock Chapter II.", ref errors, log);
        Check(HasText(chapter.titleEnglish) && HasText(chapter.titleRussian), "Chapter I requires EN/RU titles.", ref errors, log);
        Check(HasEntries(chapter.objectiveEnglish) && HasEntries(chapter.objectiveRussian), "Chapter I requires EN/RU objectives.", ref errors, log);
        Check(HasEntries(chapter.tutorialEnglish) && HasEntries(chapter.tutorialRussian), "Chapter I requires EN/RU tutorial copy.", ref errors, log);
        Check(chapter.objectiveEnglish != null && chapter.objectiveRussian != null && chapter.objectiveEnglish.Length == chapter.objectiveRussian.Length,
            "EN/RU objective counts must match.", ref errors, log);
        Check(chapter.tutorialEnglish != null && chapter.tutorialRussian != null && chapter.tutorialEnglish.Length == chapter.tutorialRussian.Length,
            "EN/RU tutorial counts must match.", ref errors, log);
    }

    static void ValidateEncounterData(ref int errors, bool log)
    {
        ChapterData chapter = Resources.Load<ChapterData>("Chapters/Chapter01_Landing");
        if (chapter == null) return;

        int[] expectedCounts = { 8, 12, 16, 20, 25 };
        float totalPacingSeconds = 0f;

        for (int encounterNumber = 1; encounterNumber <= 5; encounterNumber++)
        {
            EncounterData encounter = chapter.GetEncounter(encounterNumber);
            if (encounter == null)
            {
                Fail($"Authored encounter {encounterNumber}/5 is missing from ChapterData.", ref errors, log);
                continue;
            }

            Check(encounter.encounterNumber == encounterNumber,
                $"Encounter slot {encounterNumber} has encounterNumber={encounter.encounterNumber}.", ref errors, log);
            Check(encounter.BaseEnemyCount == expectedCounts[encounterNumber - 1],
                $"Encounter {encounterNumber} base count changed: expected={expectedCounts[encounterNumber - 1]}, current={encounter.BaseEnemyCount}.", ref errors, log);
            Check(encounter.spawnInterval > 0f, $"Encounter {encounterNumber} spawn interval must be positive.", ref errors, log);
            Check(encounter.targetDuration > 0f, $"Encounter {encounterNumber} target duration must be positive.", ref errors, log);
            Check(encounter.preparationTime >= 0f, $"Encounter {encounterNumber} preparation time cannot be negative.", ref errors, log);
            Check(encounter.spawnGroups != null && encounter.spawnGroups.Length > 0,
                $"Encounter {encounterNumber} must contain authored spawn groups.", ref errors, log);

            if (encounterNumber < 5)
                Check(!encounter.HasBoss, $"Only encounter 5 may contain the Chapter I boss; encounter {encounterNumber} contains Boss.", ref errors, log);

            totalPacingSeconds += encounter.preparationTime + encounter.targetDuration;
        }

        Check(totalPacingSeconds >= 11f * 60f && totalPacingSeconds <= 13f * 60f,
            $"Authored auto-start pacing must remain 11-13 minutes; current target={totalPacingSeconds:0}s.", ref errors, log);

        EncounterData finalEncounter = chapter.GetEncounter(5);
        Check(finalEncounter != null && finalEncounter.HasBoss, "Final encounter must contain the boss archetype.", ref errors, log);
        Check(finalEncounter != null && HasBehavior(finalEncounter, "menelaus"),
            "Final encounter must explicitly bind behaviorId=menelaus instead of relying on a spawner special case.", ref errors, log);

        EnemyData boss = BalanceCatalog.GetEnemy(EnemyArchetype.Boss);
        Check(boss != null, "Boss EnemyData is missing.", ref errors, log);
        if (boss != null)
            Check(string.Equals(boss.id, "menelaus", StringComparison.OrdinalIgnoreCase), $"Chapter I boss must be Menelaus, current id={boss.id}.", ref errors, log);
    }

    static void ValidatePresentationFiles(ref int errors, bool log)
    {
        foreach (string path in RequiredPresentationScripts)
        {
            if (AssetDatabase.LoadAssetAtPath<MonoScript>(path) == null)
                Fail($"Required Chapter I presentation/runtime script missing: {path}", ref errors, log);
        }
    }

    static void ValidateBootstrapWiring(ref int errors, bool log)
    {
        const string bootstrapPath = "Assets/Game/Core/Bootstrap/GameBootstrap.cs";
        CheckSourceContains(bootstrapPath, "ChapterRuntimeInstaller.Install", "GameBootstrap must delegate chapter-specific setup through ChapterRuntimeInstaller.", ref errors, log);
        CheckSourceDoesNotContain(bootstrapPath, "ChapterOneVisualEnhancer", "GameBootstrap must not directly own Chapter I world presentation.", ref errors, log);
        CheckSourceDoesNotContain(bootstrapPath, "ChapterOneCinematicCamera", "GameBootstrap must not directly own the Chapter I cinematic.", ref errors, log);
        CheckSourceDoesNotContain(bootstrapPath, "LandingPresentation", "GameBootstrap must not directly own Chapter I landing presentation.", ref errors, log);

        const string chapterRuntimePath = "Assets/Game/Campaign/Runtime/ChapterOneRuntimeInstaller.cs";
        foreach (string token in RequiredChapterRuntimeTokens)
            CheckSourceContains(chapterRuntimePath, token, $"Chapter I runtime installer lost required wiring: {token}", ref errors, log);
    }

    static void ValidateEncounterRuntimeWiring(ref int errors, bool log)
    {
        CheckSourceContains(
            "Assets/Game/Enemies/EnemySpawner.cs",
            "chapter.GetEncounter(wave)",
            "EnemySpawner must consume authored EncounterData from the active ChapterData.",
            ref errors, log);
        CheckSourceContains(
            "Assets/Game/Enemies/EnemySpawner.cs",
            "EnemyRuntimeBehaviorRegistry.Attach",
            "EnemySpawner must bind special behaviors through the runtime behavior registry.",
            ref errors, log);
        CheckSourceDoesNotContain(
            "Assets/Game/Core/Balance/BalanceCatalog.cs",
            "GetEnemyForWave",
            "BalanceCatalog must not restore hidden wave-composition rules.",
            ref errors, log);
        CheckSourceDoesNotContain(
            "Assets/Game/Enemies/EnemySpawner.cs",
            "AddComponent<MenelausBossController>",
            "EnemySpawner must not hardcode Menelaus as the final spawn.",
            ref errors, log);
    }

    static void ValidateProductionArtWiring(ref int errors, bool log)
    {
        CheckSourceContains(
            "Assets/Game/Enemies/EnemyVisualFactory.cs",
            "TroyProduction/Characters/Greek/",
            "Enemy visuals must prefer the Chapter I production-character resource path.",
            ref errors, log);

        CheckSourceContains(
            "Assets/Game/Heroes/HeroVisualFactory.cs",
            "TroyProduction/Characters/Heroes/",
            "Hero visuals must prefer the Chapter I production-character resource path.",
            ref errors, log);

        CheckSourceContains(
            "Assets/Game/World/LandingPresentation.cs",
            "ProductionGreekRoot",
            "Landing presentation must use production-first Greek decorative characters.",
            ref errors, log);

        CheckSourceContains(
            "Assets/Game/World/LandingPresentation.cs",
            "collider.enabled = false",
            "Decorative landing characters must explicitly disable gameplay colliders.",
            ref errors, log);

        CheckSourceContains(
            "Assets/Editor/CartoonCharacterAutoBuilder.cs",
            "ChapterOneCharacterAnimationBuilder.BuildAll()",
            "Character auto-build must create/assign the Chapter I animation controller.",
            ref errors, log);

        CheckSourceContains(
            "Assets/Editor/ChapterOneCharacterAnimationBuilder.cs",
            "ChapterOneCharacter.controller",
            "Chapter I animation builder must own the shared candidate controller path.",
            ref errors, log);

        string animationBuilder = "Assets/Editor/ChapterOneCharacterAnimationBuilder.cs";
        CheckSourceContains(animationBuilder, "\"Block\"", "Spear animation profile must retain the Guard Block hook.", ref errors, log);
        CheckSourceContains(animationBuilder, "\"Poke\"", "Spear animation profile must retain the Spear Wall/Guard Poke hook.", ref errors, log);
        CheckSourceContains(animationBuilder, "\"Draw\"", "Archer animation profile must retain the Draw hook.", ref errors, log);
        CheckSourceContains(animationBuilder, "\"Release\"", "Archer animation profile must retain the Release hook.", ref errors, log);
        CheckSourceContains(animationBuilder, "\"Reload\"", "Ballista animation profile must retain the Reload hook.", ref errors, log);
        CheckSourceContains(animationBuilder, "\"Tension\"", "Ballista animation profile must retain the Tension hook.", ref errors, log);
        CheckSourceContains(animationBuilder, "\"Fire\"", "Ballista animation profile must retain the Fire hook.", ref errors, log);
        CheckSourceContains(animationBuilder, "\"Cast\"", "Apollo Priest animation profile must retain the Cast hook.", ref errors, log);
        CheckSourceContains(animationBuilder, "\"Channel\"", "Apollo Priest animation profile must retain the Channel hook.", ref errors, log);
        CheckSourceContains(animationBuilder, "\"Stoke\"", "Fire Keeper animation profile must retain the Stoke hook.", ref errors, log);
        CheckSourceContains(animationBuilder, "\"Throw\"", "Fire Keeper animation profile must retain the Throw hook.", ref errors, log);

        CheckSourceContains(
            "Assets/Game/Towers/Tower.cs",
            "crewAnimation?.PlayTowerAttack(Type)",
            "Tower attacks must drive defensive-unit crew animation hooks.",
            ref errors, log);
        CheckSourceContains(
            "Assets/Game/Towers/TrojanGuardSquad.cs",
            "PlayGuardBlock()",
            "Trojan Guard reservations must drive the Block presentation hook.",
            ref errors, log);
        CheckSourceContains(
            "Assets/Game/Towers/TrojanGuardSquad.cs",
            "PlayGuardPoke()",
            "Trojan Guard attacks must drive the Poke presentation hook.",
            ref errors, log);
        CheckSourceContains(
            "Assets/Game/Towers/TowerFactory.cs",
            "TowerSupportMechanismPresentation",
            "Tower factory must wire support-tower mechanism presentation.",
            ref errors, log);
        CheckSourceContains(
            "Assets/Game/Towers/TowerProductionArtBinder.cs",
            "RefreshCrew()",
            "Production crew replacement must refresh tower animation bindings.",
            ref errors, log);
        CheckSourceContains(
            "Assets/Editor/MythicAndSupportArtCandidateBuilder.cs",
            "ChapterOneCharacterAnimationBuilder.BuildAll()",
            "Support candidate build must refresh role-specific Chapter I animation profiles.",
            ref errors, log);
    }

    static void ValidateLegacyPlaceholderFallbacks(ref int errors, bool log)
    {
        CheckSourceDoesNotContain(
            "Assets/Game/Heroes/HeroVisualFactory.cs",
            "GameObject.CreatePrimitive(PrimitiveType.Capsule)",
            "HeroVisualFactory must not restore the legacy capsule fallback.",
            ref errors, log);

        CheckSourceDoesNotContain(
            "Assets/Game/Enemies/EnemyVisualFactory.cs",
            "GameObject.CreatePrimitive(primitive)",
            "EnemyVisualFactory must not restore the legacy primitive-only fallback.",
            ref errors, log);

        CheckSourceDoesNotContain(
            "Assets/Game/World/LandingPresentation.cs",
            "Landing Greek Silhouette",
            "Landing presentation must not restore capsule soldier silhouettes.",
            ref errors, log);
    }

    static bool HasBehavior(EncounterData encounter, string behaviorId)
    {
        if (encounter == null || encounter.spawnGroups == null) return false;
        for (int i = 0; i < encounter.spawnGroups.Length; i++)
        {
            EncounterSpawnGroup group = encounter.spawnGroups[i];
            if (group != null && string.Equals(group.behaviorId, behaviorId, StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }

    static void CheckSourceContains(string path, string token, string message, ref int errors, bool log)
    {
        if (!File.Exists(path))
        {
            Fail($"Required source missing: {path}", ref errors, log);
            return;
        }
        string source = File.ReadAllText(path);
        Check(source.Contains(token), message, ref errors, log);
    }

    static void CheckSourceDoesNotContain(string path, string token, string message, ref int errors, bool log)
    {
        if (!File.Exists(path))
        {
            Fail($"Required source missing: {path}", ref errors, log);
            return;
        }
        string source = File.ReadAllText(path);
        Check(!source.Contains(token), message, ref errors, log);
    }

    static bool HasEntries(string[] values)
    {
        if (values == null || values.Length == 0) return false;
        foreach (string value in values)
            if (!HasText(value)) return false;
        return true;
    }

    static bool HasText(string value) => !string.IsNullOrWhiteSpace(value);

    static void Check(bool condition, string message, ref int errors, bool log)
    {
        if (condition) return;
        Fail(message, ref errors, log);
    }

    static void Fail(string message, ref int errors, bool log)
    {
        errors++;
        if (log) Debug.LogError($"[CHAPTER I RC] {message}");
    }
}
#endif
