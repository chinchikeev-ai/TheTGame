using UnityEngine;

public class CampaignController : MonoBehaviour
{
    public static CampaignController Instance { get; private set; }

    public CampaignDifficulty Difficulty => CampaignSave.Difficulty;
    public int UnlockedChapter => CampaignSave.UnlockedChapter;

    public bool HasProgress
    {
        get
        {
            if (UnlockedChapter > 1) return true;
            for (int chapter = 1; chapter <= 7; chapter++)
            {
                ChapterProgress progress = GetProgress(chapter);
                if (progress != null && (progress.completed || progress.bestScore > 0 || progress.completions > 0)) return true;
            }
            return false;
        }
    }

    public int TotalBestScore
    {
        get
        {
            int score = 0;
            for (int chapter = 1; chapter <= 7; chapter++)
            {
                ChapterProgress progress = GetProgress(chapter);
                if (progress != null) score += progress.bestScore;
            }
            return score;
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public bool IsChapterUnlocked(int chapter) => CampaignSave.IsUnlocked(chapter);
    public ChapterProgress GetProgress(int chapter) => CampaignSave.GetChapterProgress(chapter);

    public void CompleteChapter(ChapterData chapter, int score, float timeSeconds, int gateHealth)
    {
        if (chapter == null) return;
        CampaignSave.RecordChapterResult(chapter.chapterNumber, score, timeSeconds, gateHealth, chapter.unlockChapter);
    }

    public void SetDifficulty(CampaignDifficulty difficulty) => CampaignSave.SetDifficulty(difficulty);
    public CampaignDifficulty CycleDifficulty() => CampaignSave.CycleDifficulty();
    public void ResetProgress() => CampaignSave.ResetProgress();
}
