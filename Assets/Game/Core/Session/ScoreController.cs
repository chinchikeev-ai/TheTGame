using UnityEngine;

public struct ChapterScoreInput
{
    public int kills;
    public int leaks;
    public int gateHealth;
    public int goldEarned;
    public int goldSpent;
    public float runTimeSeconds;
    public float targetDurationSeconds;
    public CampaignDifficulty difficulty;
}

public static class ScoreController
{
    public static int Calculate(ChapterScoreInput input)
    {
        int score = input.kills * 100;
        score += Mathf.Max(0, input.gateHealth) * 250;
        score += Mathf.Max(0, 3000 - input.leaks * 350);
        score += Mathf.Max(0, input.goldEarned - input.goldSpent / 2);

        if (input.targetDurationSeconds > 0f && input.runTimeSeconds > 0f)
        {
            float pace = Mathf.Clamp(input.targetDurationSeconds / input.runTimeSeconds, .5f, 1.5f);
            score += Mathf.RoundToInt(pace * 2000f);
        }

        score = Mathf.RoundToInt(score * DifficultyRules.ScoreMultiplier(input.difficulty));
        return Mathf.Max(0, score);
    }
}
