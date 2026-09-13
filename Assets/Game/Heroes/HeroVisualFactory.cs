using UnityEngine;

public enum TroyHeroId
{
    Hector,
    Achilles,
    Menelaus
}

public static class HeroVisualFactory
{
    const string ProductionResourceRoot = "TroyProduction/Characters/Heroes/";
    const string GeneratedResourceRoot = "TroyCharacters/Heroes/";

    public static GameObject Create(TroyHeroId heroId)
    {
        string prefabName = GetPrefabName(heroId);
        GameObject prefab = Resources.Load<GameObject>(ProductionResourceRoot + prefabName);
        if (prefab == null)
            prefab = Resources.Load<GameObject>(GeneratedResourceRoot + prefabName);

        GameObject result = prefab != null
            ? Object.Instantiate(prefab)
            : RuntimeWarriorVisualFactory.CreateHeroFallback(heroId);

        result.name = prefabName;
        HeroSignatureArt.Enhance(result, heroId);
        return result;
    }

    public static string GetPrefabName(TroyHeroId heroId)
    {
        switch (heroId)
        {
            case TroyHeroId.Achilles: return "Hero_Achilles";
            case TroyHeroId.Menelaus: return "Hero_Menelaus";
            default: return "Hero_Hector";
        }
    }
}
