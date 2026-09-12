using UnityEngine;

public class CampaignController : MonoBehaviour
{
    public static CampaignController Instance { get; private set; }

    public CampaignDifficulty Difficulty => CampaignSave.Difficulty;
    public int UnlockedChapter => CampaignSave.UnlockedChapter;

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

    public CampaignDifficulty CycleDifficulty() => CampaignSave.CycleDifficulty();
}
