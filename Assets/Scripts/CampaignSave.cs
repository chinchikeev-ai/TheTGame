using UnityEngine;

public static class CampaignSave
{
    const string UnlockedKey = "campaign_unlocked_chapter";
    const string ScorePrefix = "chapter_score_";

    public static int UnlockedChapter => Mathf.Max(1, PlayerPrefs.GetInt(UnlockedKey, 1));

    public static bool IsUnlocked(int chapter) => chapter <= UnlockedChapter;

    public static int GetBestScore(int chapter) => PlayerPrefs.GetInt(ScorePrefix + chapter, 0);

    public static void CompleteChapter(int chapter, int score, int unlockChapter)
    {
        if (score > GetBestScore(chapter)) PlayerPrefs.SetInt(ScorePrefix + chapter, score);
        if (unlockChapter > UnlockedChapter) PlayerPrefs.SetInt(UnlockedKey, unlockChapter);
        PlayerPrefs.Save();
        RuntimeFileLogger.Event("SAVE", $"Chapter {chapter} completed. score={score}, unlocked={UnlockedChapter}");
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(UnlockedKey);
        for (int i = 1; i <= 7; i++) PlayerPrefs.DeleteKey(ScorePrefix + i);
        PlayerPrefs.Save();
    }
}
