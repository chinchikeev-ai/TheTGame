using UnityEngine;

public static class DifficultyRules
{
    public static int StartingGold(CampaignDifficulty difficulty)
    {
        switch (difficulty)
        {
            case CampaignDifficulty.Story: return 350;
            case CampaignDifficulty.Legendary: return 240;
            default: return 300;
        }
    }

    public static int StartingGateHealth(CampaignDifficulty difficulty)
    {
        switch (difficulty)
        {
            case CampaignDifficulty.Story: return 22;
            case CampaignDifficulty.Legendary: return 16;
            default: return 20;
        }
    }

    public static float EnemyHpMultiplier(CampaignDifficulty difficulty)
    {
        switch (difficulty)
        {
            case CampaignDifficulty.Story: return .95f;
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
            case CampaignDifficulty.Story: return 1.15f;
            case CampaignDifficulty.Legendary: return 1.15f;
            default: return 1f;
        }
    }

    public static float RewardMultiplier(CampaignDifficulty difficulty)
    {
        switch (difficulty)
        {
            case CampaignDifficulty.Story: return 1.05f;
            case CampaignDifficulty.Legendary: return .90f;
            default: return 1f;
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
            case CampaignDifficulty.Story: return "STORY";
            case CampaignDifficulty.Legendary: return "LEGENDARY";
            default: return "STRATEGOS";
        }
    }
}
