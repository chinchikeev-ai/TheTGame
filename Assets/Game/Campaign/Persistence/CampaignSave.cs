using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum CampaignDifficulty
{
    Story,
    Strategos,
    Legendary
}

[Serializable]
public class ChapterProgress
{
    public int chapter;
    public bool completed;
    public int bestScore;
    public int lastScore;
    public float bestTimeSeconds;
    public int bestGateHealth;
    public int completions;
}

[Serializable]
public class CampaignChoice
{
    public string key;
    public string value;
}

[Serializable]
public class CampaignModifier
{
    public string key;
    public float value;
}

[Serializable]
public class CampaignFinalResult
{
    public bool completed;
    public int totalScore;
    public int civiliansSaved;
    public int heroesDefeated;
    public int structuresPreserved;
    public float defenseDurationSeconds;
    public string grade;
    public string endingId;
}

[Serializable]
public class CampaignSaveData
{
    public int version = 2;
    public int unlockedChapter = 1;
    public CampaignDifficulty difficulty = CampaignDifficulty.Story;
    public List<ChapterProgress> chapters = new List<ChapterProgress>();
    public List<CampaignChoice> narrativeChoices = new List<CampaignChoice>();
    public List<CampaignModifier> modifiers = new List<CampaignModifier>();
    public CampaignFinalResult finalResult = new CampaignFinalResult();
    public string lastSavedUtc;
}

public static class CampaignSave
{
    const int CurrentVersion = 2;
    const string LegacyUnlockedKey = "campaign_unlocked_chapter";
    const string LegacyScorePrefix = "chapter_score_";
    const string FileName = "campaign_save.json";
    const string BackupFileName = "campaign_save.backup.json";

    static CampaignSaveData data;
    static string storageRootOverride;

    static string StorageRoot => string.IsNullOrEmpty(storageRootOverride) ? Application.persistentDataPath : storageRootOverride;
    static string SavePath => Path.Combine(StorageRoot, FileName);
    static string BackupPath => Path.Combine(StorageRoot, BackupFileName);

    public static CampaignSaveData Data
    {
        get
        {
            EnsureLoaded();
            return data;
        }
    }

    public static int UnlockedChapter => Mathf.Clamp(Data.unlockedChapter, 1, 7);
    public static CampaignDifficulty Difficulty => Data.difficulty;
    public static CampaignFinalResult FinalResult => Data.finalResult;
    public static string CurrentSavePath => SavePath;
    public static string CurrentBackupPath => BackupPath;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RuntimeLoad()
    {
        EnsureLoaded();
    }

    public static void ConfigureStorageForTests(string rootPath)
    {
        storageRootOverride = rootPath;
        data = null;
        if (!string.IsNullOrEmpty(storageRootOverride)) Directory.CreateDirectory(storageRootOverride);
    }

    public static void ClearStorageOverrideForTests()
    {
        storageRootOverride = null;
        data = null;
    }

    public static void ResetRuntimeCacheForTests() => data = null;

    public static bool IsUnlocked(int chapter) => chapter >= 1 && chapter <= UnlockedChapter;

    public static bool IsCompleted(int chapter)
    {
        ChapterProgress progress = FindChapter(chapter, false);
        return progress != null && progress.completed;
    }

    public static int GetBestScore(int chapter)
    {
        ChapterProgress progress = FindChapter(chapter, false);
        return progress != null ? progress.bestScore : 0;
    }

    public static ChapterProgress GetChapterProgress(int chapter)
    {
        ChapterProgress progress = FindChapter(chapter, false);
        return progress ?? new ChapterProgress { chapter = chapter };
    }

    public static void CompleteChapter(int chapter, int score, int unlockChapter)
    {
        RecordChapterResult(chapter, score, 0f, 0, unlockChapter);
    }

    public static void RecordChapterResult(int chapter, int score, float timeSeconds, int gateHealth, int unlockChapter)
    {
        EnsureLoaded();
        ChapterProgress progress = FindChapter(chapter, true);
        progress.completed = true;
        progress.lastScore = Mathf.Max(0, score);
        progress.completions++;

        if (score > progress.bestScore) progress.bestScore = score;
        if (timeSeconds > 0f && (progress.bestTimeSeconds <= 0f || timeSeconds < progress.bestTimeSeconds))
            progress.bestTimeSeconds = timeSeconds;
        if (gateHealth > progress.bestGateHealth) progress.bestGateHealth = gateHealth;

        if (unlockChapter > data.unlockedChapter)
            data.unlockedChapter = Mathf.Clamp(unlockChapter, 1, 7);

        Save();
        RuntimeFileLogger.Event("SAVE", $"Chapter {chapter} completed. score={score}, best={progress.bestScore}, time={timeSeconds:0.0}s, unlocked={data.unlockedChapter}");
    }

    public static void SetDifficulty(CampaignDifficulty difficulty)
    {
        EnsureLoaded();
        data.difficulty = difficulty;
        Save();
        RuntimeFileLogger.Event("SAVE", $"Difficulty={difficulty}");
    }

    public static CampaignDifficulty CycleDifficulty()
    {
        CampaignDifficulty next = (CampaignDifficulty)(((int)Difficulty + 1) % 3);
        SetDifficulty(next);
        return next;
    }

    public static void SetNarrativeChoice(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key)) return;
        EnsureLoaded();
        CampaignChoice choice = data.narrativeChoices.Find(c => c.key == key);
        if (choice == null)
        {
            choice = new CampaignChoice { key = key };
            data.narrativeChoices.Add(choice);
        }
        choice.value = value ?? "";
        Save();
        RuntimeFileLogger.Event("SAVE", $"Choice {key}={choice.value}");
    }

    public static string GetNarrativeChoice(string key, string fallback = "")
    {
        EnsureLoaded();
        CampaignChoice choice = data.narrativeChoices.Find(c => c.key == key);
        return choice != null ? choice.value : fallback;
    }

    public static void SetModifier(string key, float value)
    {
        if (string.IsNullOrWhiteSpace(key)) return;
        EnsureLoaded();
        CampaignModifier modifier = data.modifiers.Find(m => m.key == key);
        if (modifier == null)
        {
            modifier = new CampaignModifier { key = key };
            data.modifiers.Add(modifier);
        }
        modifier.value = value;
        Save();
    }

    public static float GetModifier(string key, float fallback = 0f)
    {
        EnsureLoaded();
        CampaignModifier modifier = data.modifiers.Find(m => m.key == key);
        return modifier != null ? modifier.value : fallback;
    }

    public static void SaveHorseChapterChoices(bool reinforcedInnerCity, bool repairedGate, bool preparedFireDefense, int evacuationReadiness, float internalSpawnDelayBonus)
    {
        SetNarrativeChoice("chapter6.reinforced_inner_city", reinforcedInnerCity ? "1" : "0");
        SetNarrativeChoice("chapter6.repaired_gate", repairedGate ? "1" : "0");
        SetNarrativeChoice("chapter6.prepared_fire_defense", preparedFireDefense ? "1" : "0");
        SetModifier("chapter7.evacuation_readiness", evacuationReadiness);
        SetModifier("chapter7.internal_spawn_delay_bonus", internalSpawnDelayBonus);
        RuntimeFileLogger.Event("SAVE", "Chapter VI choices persisted for Chapter VII");
    }

    public static void SetFinalResult(int totalScore, int civiliansSaved, int heroesDefeated, int structuresPreserved, float defenseDurationSeconds, string grade, string endingId)
    {
        EnsureLoaded();
        data.finalResult.completed = true;
        data.finalResult.totalScore = Mathf.Max(0, totalScore);
        data.finalResult.civiliansSaved = Mathf.Max(0, civiliansSaved);
        data.finalResult.heroesDefeated = Mathf.Max(0, heroesDefeated);
        data.finalResult.structuresPreserved = Mathf.Max(0, structuresPreserved);
        data.finalResult.defenseDurationSeconds = Mathf.Max(0f, defenseDurationSeconds);
        data.finalResult.grade = grade ?? "";
        data.finalResult.endingId = endingId ?? "troy_burns";
        Save();
        RuntimeFileLogger.Event("SAVE", $"Campaign completed. score={data.finalResult.totalScore}, grade={data.finalResult.grade}, ending={data.finalResult.endingId}");
    }

    public static int GetCampaignScore()
    {
        EnsureLoaded();
        int total = 0;
        foreach (ChapterProgress chapter in data.chapters) total += chapter.bestScore;
        return total;
    }

    public static void Save()
    {
        EnsureLoaded(false);
        try
        {
            Directory.CreateDirectory(StorageRoot);
            data.version = CurrentVersion;
            data.lastSavedUtc = DateTime.UtcNow.ToString("O");
            string json = JsonUtility.ToJson(data, true);
            string tempPath = SavePath + ".tmp";
            File.WriteAllText(tempPath, json);

            if (File.Exists(SavePath))
                File.Copy(SavePath, BackupPath, true);

            if (File.Exists(SavePath)) File.Delete(SavePath);
            File.Move(tempPath, SavePath);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Campaign save failed: {ex}");
        }
    }

    public static void Reload()
    {
        data = null;
        EnsureLoaded();
    }

    public static void ResetProgress()
    {
        data = CreateDefault();
        try
        {
            Directory.CreateDirectory(StorageRoot);
            if (File.Exists(SavePath)) File.Delete(SavePath);
            if (File.Exists(BackupPath)) File.Delete(BackupPath);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Could not delete campaign save files: {ex.Message}");
        }

        if (string.IsNullOrEmpty(storageRootOverride))
        {
            PlayerPrefs.DeleteKey(LegacyUnlockedKey);
            for (int i = 1; i <= 7; i++) PlayerPrefs.DeleteKey(LegacyScorePrefix + i);
            PlayerPrefs.Save();
        }

        Save();
        RuntimeFileLogger.Event("SAVE", "Campaign progress reset");
    }

    static void EnsureLoaded(bool allowDiskLoad = true)
    {
        if (data != null) return;
        data = allowDiskLoad ? LoadFromDisk() : CreateDefault();
    }

    static CampaignSaveData LoadFromDisk()
    {
        CampaignSaveData loaded = TryLoad(SavePath);
        if (loaded == null)
        {
            loaded = TryLoad(BackupPath);
            if (loaded != null) RuntimeFileLogger.Event("SAVE", "Primary save invalid; loaded backup");
        }
        if (loaded == null) loaded = string.IsNullOrEmpty(storageRootOverride) ? MigrateLegacyOrCreateDefault() : CreateDefault();
        Normalize(loaded);
        RuntimeFileLogger.Event("SAVE", $"Loaded save v{loaded.version}. unlocked={loaded.unlockedChapter}, difficulty={loaded.difficulty}, campaignScore={CampaignScore(loaded)}");
        return loaded;
    }

    static CampaignSaveData TryLoad(string path)
    {
        try
        {
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json)) return null;
            CampaignSaveData loaded = JsonUtility.FromJson<CampaignSaveData>(json);
            return loaded != null && loaded.version > 0 ? loaded : null;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Campaign save load failed for {path}: {ex.Message}");
            return null;
        }
    }

    static CampaignSaveData MigrateLegacyOrCreateDefault()
    {
        CampaignSaveData migrated = CreateDefault();
        bool hasLegacy = PlayerPrefs.HasKey(LegacyUnlockedKey);
        migrated.unlockedChapter = Mathf.Clamp(PlayerPrefs.GetInt(LegacyUnlockedKey, 1), 1, 7);

        for (int chapter = 1; chapter <= 7; chapter++)
        {
            int score = PlayerPrefs.GetInt(LegacyScorePrefix + chapter, 0);
            if (score <= 0) continue;
            hasLegacy = true;
            migrated.chapters.Add(new ChapterProgress
            {
                chapter = chapter,
                completed = true,
                bestScore = score,
                lastScore = score,
                completions = 1
            });
        }

        if (hasLegacy)
        {
            data = migrated;
            Save();
            RuntimeFileLogger.Event("SAVE", "Migrated legacy PlayerPrefs campaign progress to JSON");
        }
        return migrated;
    }

    static CampaignSaveData CreateDefault()
    {
        return new CampaignSaveData
        {
            version = CurrentVersion,
            unlockedChapter = 1,
            difficulty = CampaignDifficulty.Story,
            finalResult = new CampaignFinalResult()
        };
    }

    static void Normalize(CampaignSaveData save)
    {
        if (save.chapters == null) save.chapters = new List<ChapterProgress>();
        if (save.narrativeChoices == null) save.narrativeChoices = new List<CampaignChoice>();
        if (save.modifiers == null) save.modifiers = new List<CampaignModifier>();
        if (save.finalResult == null) save.finalResult = new CampaignFinalResult();
        save.unlockedChapter = Mathf.Clamp(save.unlockedChapter, 1, 7);
        save.version = CurrentVersion;
    }

    static ChapterProgress FindChapter(int chapter, bool create)
    {
        EnsureLoaded();
        ChapterProgress progress = data.chapters.Find(c => c.chapter == chapter);
        if (progress == null && create)
        {
            progress = new ChapterProgress { chapter = chapter };
            data.chapters.Add(progress);
        }
        return progress;
    }

    static int CampaignScore(CampaignSaveData save)
    {
        int total = 0;
        if (save.chapters != null)
            foreach (ChapterProgress chapter in save.chapters) total += chapter.bestScore;
        return total;
    }
}
