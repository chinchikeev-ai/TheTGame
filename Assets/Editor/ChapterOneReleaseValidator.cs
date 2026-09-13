#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ChapterOneReleaseValidator
{
    static readonly string[] RequiredPresentationScripts =
    {
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
        "Assets/Editor/ChapterOneCharacterAnimationBuilder.cs"
    };

    static readonly string[] RequiredBootstrapTokens =
    {
        "ChapterOneVisualEnhancer.Enhance()",
        "TroyGateHeroBuilder.Build()",
        "ChapterOneWallLife.Build()",
        "ChapterOneAtmosphereController",
        "LandingPresentation",
        "ChapterOneCinematicCamera"
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
        ValidateFinalWave(ref errors, log);
        ValidatePresentationFiles(ref errors, log);
        ValidateBootstrapWiring(ref errors, log);
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
        Check(chapter.combatEvents == 5, "Chapter I must contain exactly 5 combat events.", ref errors, log);
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

    static void ValidateFinalWave(ref int errors, bool log)
    {
        for (int wave = 1; wave <= 5; wave++)
        {
            WaveData data = BalanceCatalog.GetWave(wave, 5);
            if (data == null)
            {
                Fail($"Wave {wave}/5 data is missing.", ref errors, log);
                continue;
            }
            Check(data.enemyCount > 0, $"Wave {wave} must contain enemies.", ref errors, log);
            Check(data.spawnInterval > 0f, $"Wave {wave} spawn interval must be positive.", ref errors, log);
            if (wave < 5) Check(!data.hasBoss, $"Only final wave may carry the Chapter I boss flag; wave {wave} is marked as boss.", ref errors, log);
        }

        WaveData finalWave = BalanceCatalog.GetWave(5, 5);
        Check(finalWave != null && finalWave.hasBoss, "Final wave must contain the boss encounter.", ref errors, log);

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
                Fail($"Required Chapter I presentation script missing: {path}", ref errors, log);
        }
    }

    static void ValidateBootstrapWiring(ref int errors, bool log)
    {
        const string bootstrapPath = "Assets/Game/Core/Bootstrap/GameBootstrap.cs";
        if (!File.Exists(bootstrapPath))
        {
            Fail("GameBootstrap.cs is missing.", ref errors, log);
            return;
        }

        string source = File.ReadAllText(bootstrapPath);
        foreach (string token in RequiredBootstrapTokens)
            Check(source.Contains(token), $"Chapter I presentation is not wired in GameBootstrap: {token}", ref errors, log);
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

        CheckSourceContains(
            "Assets/Editor/ChapterOneCharacterAnimationBuilder.cs",
            "\"Block\"",
            "Spear animation profile must retain the Guard Block hook.",
            ref errors, log);
        CheckSourceContains(
            "Assets/Editor/ChapterOneCharacterAnimationBuilder.cs",
            "\"Poke\"",
            "Spear animation profile must retain the Spear Wall/Guard Poke hook.",
            ref errors, log);
        CheckSourceContains(
            "Assets/Editor/ChapterOneCharacterAnimationBuilder.cs",
            "\"Draw\"",
            "Archer animation profile must retain the Draw hook.",
            ref errors, log);
        CheckSourceContains(
            "Assets/Editor/ChapterOneCharacterAnimationBuilder.cs",
            "\"Release\"",
            "Archer animation profile must retain the Release hook.",
            ref errors, log);

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
