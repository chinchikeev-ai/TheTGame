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

        GameObject fallback = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        fallback.name = prefabName;
        fallback.transform.localScale = heroId == TroyHeroId.Hector ? new Vector3(.75f, .75f, .75f) : Vector3.one;
        TowerFactory.SetColor(fallback, GetFallbackColor(heroId));
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

    static Color GetFallbackColor(TroyHeroId heroId)
    {
        switch (heroId)
        {
            case TroyHeroId.Achilles: return new Color(.78f, .67f, .32f);
            case TroyHeroId.Menelaus: return new Color(.42f, .55f, .78f);
            default: return new Color(.72f, .48f, .12f);
        }
    }
}
