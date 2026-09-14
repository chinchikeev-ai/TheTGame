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
        string source;
        string detail;

        GameObject prefab = Resources.Load<GameObject>(ProductionResourceRoot + prefabName);
        if (prefab != null)
        {
            source = "PRODUCTION_RESOURCE";
            detail = ProductionResourceRoot + prefabName;
        }
        else
        {
            prefab = Resources.Load<GameObject>(GeneratedResourceRoot + prefabName);
            if (prefab != null)
            {
                source = "GENERATED_RESOURCE";
                detail = GeneratedResourceRoot + prefabName;
            }
            else
            {
                source = "PROCEDURAL_FALLBACK";
                detail = "RuntimeWarriorVisualFactory";
            }
        }

        GameObject result = prefab != null
            ? Object.Instantiate(prefab)
            : RuntimeWarriorVisualFactory.CreateHeroFallback(heroId);

        result.name = prefabName;
        HeroSignatureArt.Enhance(result, heroId);
        RuntimeVisualAudit.Report("Hero:" + heroId, source, detail);
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
