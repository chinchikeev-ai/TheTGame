using UnityEngine;

[CreateAssetMenu(menuName = "TheTroyGame/Chapter Data")]
public class ChapterData : ScriptableObject
{
    public int chapterNumber = 1;
    public string chapterId = "chapter_01_landing";
    public string runtimeProfile = "chapter01_landing";
    public string titleEnglish = "The Landing";
    public string titleRussian = "Высадка";
    public int combatEvents = 5;
    public float targetDurationMinutes = 12f;
    public EncounterData[] encounters;
    public string[] objectiveEnglish;
    public string[] objectiveRussian;
    public string[] tutorialEnglish;
    public string[] tutorialRussian;
    public int unlockChapter = 2;

    public int EncounterCount => encounters != null && encounters.Length > 0 ? encounters.Length : combatEvents;

    public EncounterData GetEncounter(int encounterNumber)
    {
        if (encounters == null || encounterNumber <= 0) return null;
        for (int i = 0; i < encounters.Length; i++)
        {
            EncounterData encounter = encounters[i];
            if (encounter != null && encounter.encounterNumber == encounterNumber) return encounter;
        }
        return encounterNumber <= encounters.Length ? encounters[encounterNumber - 1] : null;
    }
}
