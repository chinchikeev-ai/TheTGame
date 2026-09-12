using UnityEngine;

public enum TroyHeroId
{
    Hector,
    Achilles,
    Menelaus
}

public static class HeroVisualFactory
{
    const string ResourceRoot = "TroyCharacters/Heroes/";

    public static GameObject Create(TroyHeroId heroId)
    {
        string prefabName = GetPrefabName(heroId);
        GameObject prefab = Resources.Load<GameObject>(ResourceRoot + prefabName);
        if (prefab != null)
            return Object.Instantiate(prefab);

        GameObject fallback = RuntimeWarriorVisualFactory.CreateHeroFallback(heroId);
        fallback.name = prefabName;
        return fallback;
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
