using UnityEngine;

// Compatibility entry point retained for older bootstrap/scene references.
// Chapter I visual ownership is intentionally split by domain:
// coast -> CoastEnvironmentBuilder, shore motion -> ChapterOneShoreLife,
// battlefield dressing -> ChapterOneBattlefieldDetails,
// Troy gate -> TroyGateHeroBuilder, wall -> ChapterOneWallLife,
// city -> TroyCityBackdropPresentation, fire/lights -> TroyFireLifePresentation.
public static class ChapterOneVisualEnhancer
{
    public static void Enhance()
    {
        if (GameObject.Find("Chapter01_CoastalDressing") != null) return;
        GameObject root = new GameObject("Chapter01_CoastalDressing");
        AddCoastalScrub(root.transform);
        AddBeachStakes(root.transform);
    }

    static void AddCoastalScrub(Transform parent)
    {
        Vector3[] scrub =
        {
            new Vector3(-8.8f,.05f,8.0f), new Vector3(-7.2f,.05f,-7.2f), new Vector3(-3.2f,.05f,8.5f),
            new Vector3(1.8f,.05f,-8.2f), new Vector3(6.2f,.05f,8.0f), new Vector3(9.6f,.05f,-7.4f)
        };
        for (int i = 0; i < scrub.Length; i++) CreateScrub(parent, scrub[i], .8f + (i % 3) * .15f);
    }

    static void AddBeachStakes(Transform parent)
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 p = new Vector3(-12.0f + i * .7f, .05f, -7.6f + (i % 2) * .8f);
            GameObject stake = Part(parent, "Beach Stake", PrimitiveType.Cylinder, p, new Vector3(.035f,.52f,.035f), new Color(.30f,.18f,.08f));
            stake.transform.rotation = Quaternion.Euler(10f, 0f, i % 2 == 0 ? 12f : -12f);
        }
    }

    static void CreateScrub(Transform parent, Vector3 position, float scale)
    {
        Color green = new Color(.26f,.31f,.16f);
        for (int i = 0; i < 5; i++)
        {
            GameObject leaf = Part(parent, "Coastal Scrub", PrimitiveType.Cube,
                position + new Vector3((i - 2) * .08f, .12f, ((i % 2) - .5f) * .12f),
                new Vector3(.04f,.30f,.07f) * scale, green * (.88f + i * .025f));
            leaf.transform.rotation = Quaternion.Euler(i * 7f, i * 31f, (i - 2) * 11f);
        }
    }

    static GameObject Part(Transform parent, string name, PrimitiveType type, Vector3 position, Vector3 scale, Color color)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.position = position;
        go.transform.localScale = scale;
        Object.Destroy(go.GetComponent<Collider>());
        TowerFactory.SetColor(go, color);
        return go;
    }
}
