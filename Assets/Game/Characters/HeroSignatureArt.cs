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
        Color bronze = new Color(.78f, .53f, .18f);
        Color darkBronze = new Color(.46f, .27f, .09f);
        Color trojanRed = new Color(.58f, .055f, .035f);

        Part(art, "Hector Chest Sun", PrimitiveType.Cylinder, new Vector3(0f, 1.15f, .29f), new Vector3(.16f, .025f, .16f), bronze, Quaternion.Euler(90f, 0f, 0f));
        Part(art, "Hector Mantle L", PrimitiveType.Sphere, new Vector3(-.34f, 1.36f, -.03f), new Vector3(.15f, .09f, .15f), darkBronze);
        Part(art, "Hector Mantle R", PrimitiveType.Sphere, new Vector3(.34f, 1.36f, -.03f), new Vector3(.15f, .09f, .15f), darkBronze);
        if (!HasExistingCrest(art.parent, art))
            Part(art, "Hector Horsehair Crest", PrimitiveType.Cube, new Vector3(0f, 2.03f, -.02f), new Vector3(.09f, .34f, .44f), trojanRed, Quaternion.Euler(-8f, 0f, 0f));
        Part(art, "Hector Belt Emblem", PrimitiveType.Cylinder, new Vector3(0f, .84f, .25f), new Vector3(.095f, .022f, .095f), bronze, Quaternion.Euler(90f, 0f, 0f));
    }

    static void BuildMenelaus(Transform art)
    {
        Color gold = new Color(.88f, .66f, .20f);
        Color royalBlue = new Color(.18f, .30f, .58f);
        Color deepRed = new Color(.48f, .045f, .035f);

        Part(art, "Menelaus Royal Chest", PrimitiveType.Cube, new Vector3(0f, 1.17f, .30f), new Vector3(.34f, .31f, .055f), gold);
        Part(art, "Menelaus Royal Belt", PrimitiveType.Cylinder, new Vector3(0f, .84f, .25f), new Vector3(.11f, .025f, .11f), gold, Quaternion.Euler(90f, 0f, 0f));
        Part(art, "Menelaus Crest", PrimitiveType.Cube, new Vector3(0f, 2.08f, -.02f), new Vector3(.14f, .37f, .50f), deepRed);
        Part(art, "Menelaus Mantle L", PrimitiveType.Sphere, new Vector3(-.36f, 1.39f, -.03f), new Vector3(.18f, .11f, .18f), gold);
        Part(art, "Menelaus Mantle R", PrimitiveType.Sphere, new Vector3(.36f, 1.39f, -.03f), new Vector3(.18f, .11f, .18f), gold);
        Part(art, "Menelaus Command Sash", PrimitiveType.Cube, new Vector3(.12f, 1.07f, -.22f), new Vector3(.17f, .58f, .035f), royalBlue, Quaternion.Euler(4f, 0f, -18f));
    }

    static void BuildAchilles(Transform art)
    {
        Color gold = new Color(.91f, .69f, .22f);
        Color black = new Color(.10f, .09f, .08f);
        Part(art, "Achilles Chest Mark", PrimitiveType.Cylinder, new Vector3(0f, 1.17f, .29f), new Vector3(.14f, .025f, .14f), gold, Quaternion.Euler(90f, 0f, 0f));
        Part(art, "Achilles Crest", PrimitiveType.Cube, new Vector3(0f, 2.08f, -.02f), new Vector3(.10f, .40f, .48f), black);
    }

    static bool HasExistingCrest(Transform root, Transform generatedArt)
    {
        if (root == null) return false;
        foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
        {
            if (transform == null || transform == generatedArt || transform.IsChildOf(generatedArt)) continue;
            if (transform.name.IndexOf("crest", StringComparison.OrdinalIgnoreCase) >= 0) return true;
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
        if (collider != null) Object.Destroy(collider);
        TowerFactory.SetColor(part, color);
        return part;
    }
}
