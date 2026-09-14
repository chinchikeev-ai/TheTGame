using UnityEngine;

public static class DifficultyRules
{
    public static int StartingGold(CampaignDifficulty difficulty)
    {
        switch (difficulty)
        {
            case CampaignDifficulty.Story: return 190;
            case CampaignDifficulty.Legendary: return 120;
            default: return 150;
        }
    }

    public static int StartingGateHealth(CampaignDifficulty difficulty)
    {
        switch (difficulty)
        {
            case CampaignDifficulty.Story: return 24;
            case CampaignDifficulty.Legendary: return 16;
            default: return 20;
        }
    }

    public static float EnemyHpMultiplier(CampaignDifficulty difficulty)
    {
        switch (difficulty)
        {
            case CampaignDifficulty.Story: return .85f;
            case CampaignDifficulty.Legendary: return 1.25f;
            default: return 1f;
        }
    }

    public static float EnemySpeedMultiplier(CampaignDifficulty difficulty)
    {
        switch (difficulty)
        {
            case CampaignDifficulty.Story: return .95f;
            case CampaignDifficulty.Legendary: return 1.10f;
            default: return 1f;
        }
    }

    public static float EnemyCountMultiplier(CampaignDifficulty difficulty)
    {
        switch (difficulty)
        {
            case CampaignDifficulty.Story: return 1.35f;
            case CampaignDifficulty.Legendary: return 1.725f;
            default: return 1.50f;
        }
    }

    public static float RewardMultiplier(CampaignDifficulty difficulty)
    {
        switch (difficulty)
        {
            case CampaignDifficulty.Story: return .575f;
            case CampaignDifficulty.Legendary: return .45f;
            default: return .50f;
        }
    }

    public static float ScoreMultiplier(CampaignDifficulty difficulty)
    {
        switch (difficulty)
        {
            case CampaignDifficulty.Story: return .85f;
            case CampaignDifficulty.Legendary: return 1.35f;
            default: return 1f;
        }
    }

    public static string Label(CampaignDifficulty difficulty)
    {
        switch (difficulty)
        {
            case CampaignDifficulty.Story: return GameLanguage.T("STORY", "СЮЖЕТ");
            case CampaignDifficulty.Legendary: return GameLanguage.T("LEGENDARY", "ЛЕГЕНДАРНАЯ");
            default: return GameLanguage.T("STRATEGOS", "СТРАТЕГ");
        }
    }
}
