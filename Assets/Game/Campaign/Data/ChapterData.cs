using UnityEngine;

[CreateAssetMenu(menuName = "TheTroyGame/Chapter Data")]
public class ChapterData : ScriptableObject
{
    public int chapterNumber = 1;
    public string chapterId = "chapter_01_landing";
    public string titleEnglish = "The Landing";
    public string titleRussian = "Высадка";
    public int combatEvents = 5;
    public float targetDurationMinutes = 12f;
    public string[] objectiveEnglish;
    public string[] objectiveRussian;
    public string[] tutorialEnglish;
    public string[] tutorialRussian;
    public int unlockChapter = 2;
}
