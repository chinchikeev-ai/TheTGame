using System;
using UnityEngine;

public static class HeroSignatureArt
{
    public static void Enhance(GameObject root, TroyHeroId heroId)
    {
        if (root == null || root.transform.Find("HeroSignatureArt") != null) return;

        Transform art = new GameObject("HeroSignatureArt").transform;
        art.SetParent(root.transform, false);

        switch (heroId)
        {
            case TroyHeroId.Hector:
                BuildHector(art);
                break;
            case TroyHeroId.Menelaus:
                BuildMenelaus(art);
                break;
            case TroyHeroId.Achilles:
                BuildAchilles(art);
                break;
        }
    }

    static void BuildHector(Transform art)
    {
        Color bronze = new Color(.82f, .57f, .20f);
        Color darkBronze = new Color(.46f, .27f, .09f);
        Color trojanRed = new Color(.58f, .055f, .035f);
        Color deepRed = new Color(.34f, .025f, .02f);
        Color wood = new Color(.30f, .17f, .07f);

        // Large upper-body shapes are intentional: Hector must read as the hero
        // from the real tactical camera before facial/detail inspection.
        Part(art, "Hector Hero Cape", PrimitiveType.Cube, new Vector3(0f, 1.14f, -.30f), new Vector3(.72f, 1.10f, .055f), trojanRed, Quaternion.Euler(3f, 0f, 0f));
        Part(art, "Hector Cape Lower Mass", PrimitiveType.Cube, new Vector3(.05f, .69f, -.32f), new Vector3(.62f, .50f, .045f), deepRed, Quaternion.Euler(0f, 0f, -5f));
        Part(art, "Hector Chest Sun", PrimitiveType.Cylinder, new Vector3(0f, 1.17f, .31f), new Vector3(.20f, .028f, .20f), bronze, Quaternion.Euler(90f, 0f, 0f));
        Part(art, "Hector Mantle L", PrimitiveType.Sphere, new Vector3(-.39f, 1.39f, -.03f), new Vector3(.22f, .13f, .21f), darkBronze);
        Part(art, "Hector Mantle R", PrimitiveType.Sphere, new Vector3(.39f, 1.39f, -.03f), new Vector3(.22f, .13f, .21f), darkBronze);
        if (!HasExistingToken(art.parent, art, "crest"))
            Part(art, "Hector Horsehair Crest", PrimitiveType.Cube, new Vector3(0f, 2.09f, -.02f), new Vector3(.13f, .44f, .56f), trojanRed, Quaternion.Euler(-8f, 0f, 0f));
        Part(art, "Hector Belt Emblem", PrimitiveType.Cylinder, new Vector3(0f, .84f, .25f), new Vector3(.11f, .024f, .11f), bronze, Quaternion.Euler(90f, 0f, 0f));

        if (!HasExistingToken(art.parent, art, "shield"))
        {
            GameObject shield = Part(art, "Hector Signature Round Shield", PrimitiveType.Cylinder,
                new Vector3(-.58f, 1.08f, .10f), new Vector3(.64f, .075f, .64f), trojanRed, Quaternion.Euler(90f, 0f, 0f));
            Part(shield.transform, "Hector Signature Shield Boss", PrimitiveType.Sphere,
                new Vector3(0f, .085f, 0f), new Vector3(.23f, .10f, .23f), bronze);
        }

        if (!HasExistingToken(art.parent, art, "spear"))
        {
            GameObject spear = Part(art, "Hector Signature Long Spear", PrimitiveType.Cylinder,
                new Vector3(.55f, 1.10f, .02f), new Vector3(.040f, 1.55f, .040f), wood, Quaternion.Euler(7f, 0f, -9f));
            Part(spear.transform, "Hector Signature Spear Tip", PrimitiveType.Cube,
                new Vector3(0f, 1.03f, 0f), new Vector3(.10f, .18f, .04f), bronze, Quaternion.Euler(0f, 0f, 45f));
        }
    }

    static void BuildMenelaus(Transform art)
    {
        Color gold = new Color(.90f, .68f, .21f);
        Color royalBlue = new Color(.16f, .30f, .61f);
        Color paleBlue = new Color(.36f, .53f, .78f);
        Color deepRed = new Color(.48f, .045f, .035f);
        Color darkBlue = new Color(.08f, .15f, .31f);

        Part(art, "Menelaus Command Aura", PrimitiveType.Cylinder, new Vector3(0f, .035f, 0f), new Vector3(.90f, .025f, .90f), royalBlue);
        Part(art, "Menelaus Royal Cape", PrimitiveType.Cube, new Vector3(0f, 1.15f, -.30f), new Vector3(.78f, 1.18f, .06f), royalBlue, Quaternion.Euler(2f, 0f, 0f));
        Part(art, "Menelaus Cape Trim", PrimitiveType.Cube, new Vector3(0f, .62f, -.34f), new Vector3(.80f, .09f, .045f), gold);
        Part(art, "Menelaus Royal Chest", PrimitiveType.Cube, new Vector3(0f, 1.18f, .31f), new Vector3(.42f, .36f, .065f), gold);
        Part(art, "Menelaus Royal Belt", PrimitiveType.Cylinder, new Vector3(0f, .84f, .26f), new Vector3(.13f, .028f, .13f), gold, Quaternion.Euler(90f, 0f, 0f));
        Part(art, "Menelaus Crest", PrimitiveType.Cube, new Vector3(0f, 2.13f, -.02f), new Vector3(.20f, .48f, .64f), deepRed);
        Part(art, "Menelaus Mantle L", PrimitiveType.Sphere, new Vector3(-.42f, 1.42f, -.03f), new Vector3(.25f, .15f, .24f), gold);
        Part(art, "Menelaus Mantle R", PrimitiveType.Sphere, new Vector3(.42f, 1.42f, -.03f), new Vector3(.25f, .15f, .24f), gold);
        Part(art, "Menelaus Command Sash", PrimitiveType.Cube, new Vector3(.14f, 1.07f, -.22f), new Vector3(.20f, .66f, .040f), paleBlue, Quaternion.Euler(4f, 0f, -18f));

        if (!HasExistingToken(art.parent, art, "shield"))
        {
            GameObject shield = Part(art, "Menelaus Royal Shield", PrimitiveType.Cylinder,
                new Vector3(-.61f, 1.08f, .11f), new Vector3(.66f, .080f, .66f), royalBlue, Quaternion.Euler(90f, 0f, 0f));
            Part(shield.transform, "Menelaus Royal Shield Boss", PrimitiveType.Sphere,
                new Vector3(0f, .09f, 0f), new Vector3(.24f, .11f, .24f), gold);
            Part(shield.transform, "Menelaus Shield Command Bar", PrimitiveType.Cube,
                new Vector3(0f, .095f, .18f), new Vector3(.46f, .035f, .10f), gold);
        }

        Part(art, "Menelaus Shadow Mantle", PrimitiveType.Cube, new Vector3(0f, 1.36f, -.26f), new Vector3(.46f, .20f, .04f), darkBlue);
    }

    static void BuildAchilles(Transform art)
    {
        Color gold = new Color(.91f, .69f, .22f);
        Color black = new Color(.10f, .09f, .08f);
        Part(art, "Achilles Chest Mark", PrimitiveType.Cylinder, new Vector3(0f, 1.17f, .29f), new Vector3(.14f, .025f, .14f), gold, Quaternion.Euler(90f, 0f, 0f));
        Part(art, "Achilles Crest", PrimitiveType.Cube, new Vector3(0f, 2.08f, -.02f), new Vector3(.10f, .40f, .48f), black);
    }

    static bool HasExistingToken(Transform root, Transform generatedArt, string token)
    {
        if (root == null) return false;
        foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
        {
            if (transform == null || transform == generatedArt || transform.IsChildOf(generatedArt)) continue;
            if (transform.name.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0) return true;
        }
        return false;
    }

    static GameObject Part(Transform parent, string name, PrimitiveType type, Vector3 position, Vector3 scale, Color color, Quaternion? rotation = null)
    {
        GameObject part = GameObject.CreatePrimitive(type);
        part.name = name;
        part.transform.SetParent(parent, false);
        part.transform.localPosition = position;
        part.transform.localScale = scale;
        if (rotation.HasValue) part.transform.localRotation = rotation.Value;
        Collider collider = part.GetComponent<Collider>();
        if (collider != null) UnityEngine.Object.Destroy(collider);
        TowerFactory.SetColor(part, color);
        return part;
    }
}
