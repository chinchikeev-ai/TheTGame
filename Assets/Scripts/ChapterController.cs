using UnityEngine;

public class ChapterController : MonoBehaviour
{
    public static ChapterController Instance { get; private set; }

    public ChapterData ActiveChapter { get; private set; }

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

    public ChapterData LoadChapter(int chapterNumber)
    {
        string resourcePath = chapterNumber == 1
            ? "Chapters/Chapter01_Landing"
            : $"Chapters/Chapter{chapterNumber:00}";

        ActiveChapter = Resources.Load<ChapterData>(resourcePath);
        if (ActiveChapter == null && chapterNumber == 1)
            Debug.LogError("Chapter I data is missing at Resources/Chapters/Chapter01_Landing.");

        if (ActiveChapter != null)
            RuntimeFileLogger.Event("CHAPTER", $"Loaded {ActiveChapter.chapterId} events={ActiveChapter.combatEvents}");
        return ActiveChapter;
    }
}
