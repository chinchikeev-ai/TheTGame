public static class CombatHudRecommendationPolicy
{
    public static TowerType Recommend(EnemySpawner spawner, TowerType[] candidates)
    {
        if (spawner == null || candidates == null || candidates.Length == 0)
            return TowerType.MachineGun;

        float best = float.MinValue;
        TowerType winner = TowerType.MachineGun;
        for (int i = 0; i < candidates.Length; i++)
        {
            TowerType type = candidates[i];
            float score = Score(spawner, type);
            if (score <= best) continue;
            best = score;
            winner = type;
        }
        return winner;
    }

    static float Score(EnemySpawner spawner, TowerType type)
    {
        float infantry = spawner.NextWaveInfantryCount;
        float runners = spawner.NextWaveRunnerCount;
        float heavy = spawner.NextWaveHeavyCount;
        float shields = spawner.NextWaveShieldCount;
        float archers = spawner.NextWaveArcherCount;
        float bosses = spawner.NextWaveBossCount;
        float total = UnityEngine.Mathf.Max(1f, infantry + runners + heavy + shields + archers + bosses);

        switch (type)
        {
            case TowerType.MachineGun: return infantry * 1.35f + runners * 2.1f + archers * 1.1f;
            case TowerType.SpearThrower: return heavy * 2.35f + shields * 2.0f + bosses * 1.25f;
            case TowerType.Cannon: return bosses * 4.2f + heavy * 1.65f + shields * 1.25f;
            case TowerType.Slow: return runners * 1.75f + total * .28f;
            case TowerType.FireTower: return infantry * 1.55f + runners * 1.25f + archers + total * .22f;
            case TowerType.TrojanGuard: return heavy * .9f + shields * .7f + total * .18f;
            default: return 0f;
        }
    }
}
