using UnityEngine;

public static class RuntimeWarriorVisualFactory
{
    public static GameObject CreateEnemyFallback(EnemyArchetype archetype, Color factionColor)
    {
        if (archetype == EnemyArchetype.BatteringRam)
            return CreateRam(factionColor);

        bool boss = archetype == EnemyArchetype.Boss;
        bool heavy = archetype == EnemyArchetype.HeavyHoplite || archetype == EnemyArchetype.ShieldBearer || boss;
        bool archer = archetype == EnemyArchetype.Archer;
        bool runner = archetype == EnemyArchetype.Runner;

        GameObject root = CreateHumanoidRoot(boss ? "Procedural_Commander" : "Procedural_Warrior", factionColor,
            boss ? 1.18f : runner ? .92f : heavy ? 1.06f : 1f,
            heavy ? .44f : .38f);

        if (archer)
        {
            AddBow(root.transform, new Color(.40f,.24f,.10f));
            AddQuiver(root.transform, factionColor * .75f);
        }
        else if (runner)
        {
            AddSword(root.transform, .75f, new Color(.72f,.70f,.62f));
        }
        else
        {
            AddSpear(root.transform, boss ? 1.35f : 1.15f);
            AddShield(root.transform, heavy ? .48f : .40f, boss ? new Color(.80f,.56f,.16f) : factionColor * .85f);
        }

        if (heavy || boss) AddHelmet(root.transform, boss ? new Color(.78f,.57f,.20f) : new Color(.48f,.40f,.28f), boss);
        if (boss) AddCape(root.transform, new Color(.52f,.05f,.04f));
        return root;
    }

    public static GameObject CreateHeroFallback(TroyHeroId heroId)
    {
        Color body;
        switch (heroId)
        {
            case TroyHeroId.Achilles: body = new Color(.78f,.61f,.20f); break;
            case TroyHeroId.Menelaus: body = new Color(.34f,.49f,.72f); break;
            default: body = new Color(.66f,.32f,.14f); break;
        }

        GameObject root = CreateHumanoidRoot("Procedural_" + heroId, body, 1.16f, .46f);
        AddHelmet(root.transform, new Color(.74f,.55f,.22f), true);

        if (heroId == TroyHeroId.Hector)
        {
            AddSpear(root.transform, 1.38f);
            AddShield(root.transform, .50f, new Color(.70f,.20f,.12f));
            AddCape(root.transform, new Color(.50f,.06f,.04f));
        }
        else if (heroId == TroyHeroId.Achilles)
        {
            AddSword(root.transform, 1.02f, new Color(.80f,.78f,.67f));
            AddShield(root.transform, .48f, new Color(.78f,.58f,.18f));
            AddCape(root.transform, new Color(.66f,.46f,.10f));
        }
        else
        {
            AddSword(root.transform, 1.10f, new Color(.82f,.80f,.70f));
            AddShield(root.transform, .50f, new Color(.30f,.46f,.72f));
            AddCape(root.transform, new Color(.46f,.05f,.05f));
        }

        return root;
    }

    static GameObject CreateHumanoidRoot(string name, Color bodyColor, float scale, float colliderRadius)
    {
        GameObject root = new GameObject(name);
        root.transform.localScale = Vector3.one * scale;

        CapsuleCollider collider = root.AddComponent<CapsuleCollider>();
        collider.center = new Vector3(0f,.9f,0f);
        collider.height = 1.8f;
        collider.radius = colliderRadius;

        GameObject torso = Part(PrimitiveType.Capsule, root.transform, "Torso", new Vector3(0f,1.05f,0f), new Vector3(.44f,.58f,.34f), bodyColor);
        torso.transform.localRotation = Quaternion.identity;
        Part(PrimitiveType.Sphere, root.transform, "Head", new Vector3(0f,1.72f,0f), new Vector3(.34f,.34f,.34f), new Color(.82f,.67f,.50f));
        Part(PrimitiveType.Cube, root.transform, "Belt", new Vector3(0f,.78f,0f), new Vector3(.48f,.10f,.35f), new Color(.25f,.16f,.10f));
        Part(PrimitiveType.Cube, root.transform, "Skirt", new Vector3(0f,.58f,0f), new Vector3(.50f,.34f,.36f), bodyColor * .78f);

        AddLimb(root.transform, "ArmL", new Vector3(-.38f,1.14f,0f), new Vector3(.12f,.43f,.12f), new Vector3(0f,0f,-10f), bodyColor * .9f);
        AddLimb(root.transform, "ArmR", new Vector3(.38f,1.14f,0f), new Vector3(.12f,.43f,.12f), new Vector3(0f,0f,10f), bodyColor * .9f);
        AddLimb(root.transform, "LegL", new Vector3(-.16f,.22f,0f), new Vector3(.13f,.43f,.13f), Vector3.zero, new Color(.31f,.22f,.16f));
        AddLimb(root.transform, "LegR", new Vector3(.16f,.22f,0f), new Vector3(.13f,.43f,.13f), Vector3.zero, new Color(.31f,.22f,.16f));

        return root;
    }

    static void AddLimb(Transform parent, string name, Vector3 pos, Vector3 scale, Vector3 euler, Color color)
    {
        GameObject limb = Part(PrimitiveType.Cylinder, parent, name, pos, scale, color);
        limb.transform.localRotation = Quaternion.Euler(euler);
    }

    static void AddHelmet(Transform root, Color color, bool crest)
    {
        Part(PrimitiveType.Sphere, root, "Helmet", new Vector3(0f,1.80f,0f), new Vector3(.38f,.24f,.38f), color);
        Part(PrimitiveType.Cube, root, "NoseGuard", new Vector3(0f,1.69f,.18f), new Vector3(.06f,.18f,.05f), color * .8f);
        if (crest)
            Part(PrimitiveType.Cube, root, "Crest", new Vector3(0f,2.05f,0f), new Vector3(.13f,.30f,.45f), new Color(.58f,.08f,.05f));
    }

    static void AddShield(Transform root, float size, Color color)
    {
        GameObject shield = Part(PrimitiveType.Cylinder, root, "Shield", new Vector3(-.48f,1.02f,.10f), new Vector3(size,.07f,size), color);
        shield.transform.localRotation = Quaternion.Euler(90f,0f,0f);
        Part(PrimitiveType.Sphere, shield.transform, "ShieldBoss", new Vector3(0f,.08f,0f), new Vector3(.22f,.10f,.22f), new Color(.76f,.56f,.20f));
    }

    static void AddSpear(Transform root, float length)
    {
        GameObject shaft = Part(PrimitiveType.Cylinder, root, "Spear", new Vector3(.47f,1.08f,.02f), new Vector3(.035f,length,.035f), new Color(.32f,.18f,.08f));
        shaft.transform.localRotation = Quaternion.Euler(8f,0f,-10f);
        Part(PrimitiveType.Cube, shaft.transform, "SpearTip", new Vector3(0f,1.02f,0f), new Vector3(.09f,.16f,.04f), new Color(.72f,.61f,.38f)).transform.localRotation = Quaternion.Euler(0f,0f,45f);
    }

    static void AddSword(Transform root, float length, Color bladeColor)
    {
        GameObject blade = Part(PrimitiveType.Cube, root, "Sword", new Vector3(.48f,1.02f,.12f), new Vector3(.07f,length,.035f), bladeColor);
        blade.transform.localRotation = Quaternion.Euler(0f,0f,-18f);
        Part(PrimitiveType.Cube, blade.transform, "Guard", new Vector3(0f,-.56f,0f), new Vector3(.28f,.05f,.07f), new Color(.58f,.39f,.15f));
    }

    static void AddBow(Transform root, Color color)
    {
        GameObject upper = Part(PrimitiveType.Cylinder, root, "BowUpper", new Vector3(.48f,1.28f,.08f), new Vector3(.035f,.40f,.035f), color);
        upper.transform.localRotation = Quaternion.Euler(0f,0f,24f);
        GameObject lower = Part(PrimitiveType.Cylinder, root, "BowLower", new Vector3(.48f,.82f,.08f), new Vector3(.035f,.40f,.035f), color);
        lower.transform.localRotation = Quaternion.Euler(0f,0f,-24f);
    }

    static void AddQuiver(Transform root, Color color)
    {
        GameObject q = Part(PrimitiveType.Cylinder, root, "Quiver", new Vector3(-.26f,1.10f,-.25f), new Vector3(.11f,.38f,.11f), color);
        q.transform.localRotation = Quaternion.Euler(15f,0f,-18f);
    }

    static void AddCape(Transform root, Color color)
    {
        Part(PrimitiveType.Cube, root, "Cape", new Vector3(0f,1.10f,-.24f), new Vector3(.54f,.88f,.05f), color);
    }

    static GameObject CreateRam(Color factionColor)
    {
        GameObject root = new GameObject("Procedural_BatteringRam");
        BoxCollider collider = root.AddComponent<BoxCollider>();
        collider.center = new Vector3(0f,.55f,0f);
        collider.size = new Vector3(1.4f,1.1f,2.2f);
        Part(PrimitiveType.Cube, root.transform, "Frame", new Vector3(0f,.62f,0f), new Vector3(1.25f,.75f,1.85f), factionColor * .72f);
        GameObject beam = Part(PrimitiveType.Cylinder, root.transform, "RamBeam", new Vector3(0f,.58f,.65f), new Vector3(.16f,.95f,.16f), new Color(.28f,.18f,.10f));
        beam.transform.localRotation = Quaternion.Euler(90f,0f,0f);
        Part(PrimitiveType.Cube, root.transform, "BronzeRamHead", new Vector3(0f,.58f,1.62f), new Vector3(.30f,.28f,.48f), new Color(.66f,.48f,.18f));
        return root;
    }

    static GameObject Part(PrimitiveType type, Transform parent, string name, Vector3 position, Vector3 scale, Color color)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent,false);
        go.transform.localPosition = position;
        go.transform.localScale = scale;
        Collider collider = go.GetComponent<Collider>();
        if (collider != null) Object.Destroy(collider);
        TowerFactory.SetColor(go,color);
        return go;
    }
}
