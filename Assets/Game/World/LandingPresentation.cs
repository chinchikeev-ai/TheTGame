using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LandingPresentation : MonoBehaviour
{
    const string ProductionGreekRoot = "TroyProduction/Characters/Greek/";
    const string GeneratedGreekRoot = "TroyCharacters/Factions/Greek/";

    bool played;

    void Update()
    {
        if (played || GameManager.Instance == null || GameManager.Instance.GameEnded) return;
        if (GameManager.Instance.RunTime <= .01f) return;
        played = true;
        StartCoroutine(PlayLanding());
    }

    IEnumerator PlayLanding()
    {
        RuntimeFileLogger.Event("CHAPTER", "Chapter I landing presentation started");
        GameObject root = new GameObject("LandingPresentation_Runtime");
        GameObject title = CreateTitleCard();

        GameObject[] boats = new GameObject[4];
        float[] z = { 7.2f, 2.4f, -2.4f, -7.0f };
        for (int i = 0; i < boats.Length; i++)
            boats[i] = CreateBoat(root.transform, new Vector3(-19.2f - i * .55f, .12f, z[i]));

        float duration = 5.2f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = t * t * (3f - 2f * t);
            for (int i = 0; i < boats.Length; i++)
            {
                if (boats[i] == null) continue;
                Vector3 start = new Vector3(-19.2f - i * .55f, .12f, z[i]);
                Vector3 end = new Vector3(-14.2f, .10f, z[i] * .86f);
                Vector3 pos = Vector3.Lerp(start, end, eased);
                pos.y += Mathf.Sin((elapsed + i * .45f) * 2.4f) * .045f;
                boats[i].transform.position = pos;
                boats[i].transform.rotation = Quaternion.Euler(0f, 88f + Mathf.Sin(elapsed + i) * 2f, Mathf.Sin(elapsed * 2f + i) * 1.8f);
            }
            yield return null;
        }

        CreateLandingDebris(root.transform);
        RuntimeEffects.Instance?.PlayHeroPulse(new Vector3(-13.6f,.12f,0f), new Color(.72f,.55f,.28f), 6.5f, .55f);
        yield return StartCoroutine(DeployLandingParty(root.transform));

        RuntimeFileLogger.Event("CHAPTER", "Chapter I landing presentation completed");
        Destroy(title, 1.5f);
        Destroy(root, 10f);
    }

    IEnumerator DeployLandingParty(Transform parent)
    {
        const int count = 12;
        GameObject[] soldiers = new GameObject[count];
        Vector3[] starts = new Vector3[count];
        Vector3[] ends = new Vector3[count];

        for (int i = 0; i < count; i++)
        {
            EnemyArchetype archetype = i % 6 == 0 ? EnemyArchetype.ShieldBearer : i % 4 == 0 ? EnemyArchetype.Archer : EnemyArchetype.Infantry;
            GameObject soldier = CreateDecorativeGreek(archetype);
            soldier.name = "Landing Greek Warrior";
            soldier.transform.SetParent(parent);
            int lane = i % 4;
            starts[i] = new Vector3(-13.9f + lane * .16f, 0f, -6.8f + i * 1.12f);
            ends[i] = new Vector3(-10.8f + (i % 3) * .45f, 0f, -6.2f + i * 1.02f);
            soldier.transform.position = starts[i];
            soldier.transform.localScale *= .76f;
            soldier.transform.rotation = Quaternion.Euler(0f, 88f, 0f);
            EnemyMotionAnimator.Attach(soldier, archetype);
            soldiers[i] = soldier;
        }

        float elapsed = 0f;
        const float duration = 2.8f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            for (int i = 0; i < count; i++)
            {
                if (soldiers[i] == null) continue;
                float local = Mathf.Clamp01((t - i * .018f) / .78f);
                float eased = Mathf.SmoothStep(0f,1f,local);
                Vector3 pos = Vector3.Lerp(starts[i], ends[i], eased);
                pos.y += Mathf.Abs(Mathf.Sin((elapsed * 5f) + i)) * .025f;
                soldiers[i].transform.position = pos;
            }
            yield return null;
        }
    }

    GameObject CreateDecorativeGreek(EnemyArchetype archetype)
    {
        string prefabName = EnemyVisualFactory.GetPrefabName(archetype);
        GameObject prefab = Resources.Load<GameObject>(ProductionGreekRoot + prefabName);
        if (prefab == null) prefab = Resources.Load<GameObject>(GeneratedGreekRoot + prefabName);

        GameObject soldier = prefab != null
            ? Instantiate(prefab)
            : RuntimeWarriorVisualFactory.CreateEnemyFallback(archetype, new Color(.42f,.52f,.68f));

        foreach (Collider collider in soldier.GetComponentsInChildren<Collider>(true))
        {
            collider.enabled = false;
            Destroy(collider);
        }

        foreach (Rigidbody body in soldier.GetComponentsInChildren<Rigidbody>(true))
        {
            body.detectCollisions = false;
            body.isKinematic = true;
        }

        return soldier;
    }

    void CreateLandingDebris(Transform parent)
    {
        Color wood = new Color(.30f,.17f,.075f);
        Color bronze = new Color(.62f,.42f,.18f);
        Color cloth = new Color(.30f,.39f,.55f);

        for (int i = 0; i < 5; i++)
        {
            float z = -5.8f + i * 2.8f;
            DecorPrimitive(parent,"Landing Crate",PrimitiveType.Cube,new Vector3(-12.45f,.18f,z),new Vector3(.40f,.36f,.46f),wood,Quaternion.Euler(0f,8f*i,0f));

            GameObject shield = DecorPrimitive(parent,"Discarded Greek Shield",PrimitiveType.Cylinder,new Vector3(-11.95f,.16f,z+.48f),new Vector3(.24f,.045f,.24f),bronze,Quaternion.Euler(82f,0f,18f));
            shield.transform.localRotation = Quaternion.Euler(82f,0f,18f+i*7f);

            DecorPrimitive(parent,"Landing Oar",PrimitiveType.Cylinder,new Vector3(-12.10f,.12f,z-.55f),new Vector3(.025f,.72f,.025f),wood,Quaternion.Euler(82f,0f,28f));
        }

        DecorPrimitive(parent,"Greek Beach Standard",PrimitiveType.Cylinder,new Vector3(-11.35f,.72f,4.8f),new Vector3(.025f,.72f,.025f),wood);
        DecorPrimitive(parent,"Greek Standard Cloth",PrimitiveType.Cube,new Vector3(-11.18f,1.17f,4.8f),new Vector3(.32f,.28f,.035f),cloth);
    }

    GameObject CreateTitleCard()
    {
        GameObject canvasObj = new GameObject("LandingTitleCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 40;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        GameObject panelObj = new GameObject("LandingTitle");
        panelObj.transform.SetParent(canvasObj.transform, false);
        Image panel = panelObj.AddComponent<Image>();
        panel.sprite = TroyHudArt.Panel();
        panel.type = Image.Type.Sliced;
        panel.color = Color.white;
        RectTransform pr = panel.rectTransform;
        pr.anchorMin = pr.anchorMax = pr.pivot = new Vector2(.5f, .5f);
        pr.anchoredPosition = new Vector2(0, 160);
        pr.sizeDelta = new Vector2(760, 150);

        Image crest = new GameObject("GreekCrest").AddComponent<Image>();
        crest.transform.SetParent(panelObj.transform,false);
        crest.sprite = TroyHudArt.Enemy("shield");
        crest.raycastTarget = false;
        RectTransform er = crest.rectTransform;
        er.anchorMin = er.anchorMax = er.pivot = new Vector2(.5f,.5f);
        er.anchoredPosition = new Vector2(-310,0);
        er.sizeDelta = new Vector2(96,96);

        Text headline = MakeText(panelObj.transform, "Headline", 38, TextAnchor.MiddleCenter);
        headline.text = GameLanguage.T("CHAPTER I — THE LANDING", "ГЛАВА I — ВЫСАДКА");
        RectTransform hr = headline.rectTransform;
        hr.anchorMin = new Vector2(.12f, .45f);
        hr.anchorMax = Vector2.one;
        hr.offsetMin = new Vector2(10, 0);
        hr.offsetMax = new Vector2(-20, -8);

        Text sub = MakeText(panelObj.transform, "Subtitle", 18, TextAnchor.MiddleCenter);
        sub.text = GameLanguage.T("Hold the shore. Protect Troy. Menelaus is coming.", "Удержите берег. Защитите Трою. Менелай идёт.");
        RectTransform sr = sub.rectTransform;
        sr.anchorMin = new Vector2(.12f,0f);
        sr.anchorMax = new Vector2(1f, .45f);
        sr.offsetMin = new Vector2(10, 10);
        sr.offsetMax = new Vector2(-20, 0);
        return canvasObj;
    }

    Text MakeText(Transform parent, string name, int size, TextAnchor alignment)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Text text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = size;
        text.fontStyle = FontStyle.Bold;
        text.color = Color.white;
        text.alignment = alignment;
        return text;
    }

    GameObject CreateBoat(Transform parent, Vector3 position)
    {
        GameObject root = new GameObject("Incoming Achaean Galley");
        root.transform.SetParent(parent);
        root.transform.position = position;

        Color darkWood = new Color(.25f,.13f,.065f);
        Color warmWood = new Color(.42f,.20f,.08f);
        Color bronze = new Color(.56f,.31f,.10f);
        Color sailColor = new Color(.72f,.64f,.50f);

        DecorPrimitive(root.transform,"Lower Hull",PrimitiveType.Cube,Vector3.zero,new Vector3(2.5f,.28f,.78f),darkWood);
        DecorPrimitive(root.transform,"Deck",PrimitiveType.Cube,new Vector3(-.05f,.28f,0f),new Vector3(2.15f,.10f,.66f),warmWood);
        DecorPrimitive(root.transform,"Port Gunwale",PrimitiveType.Cube,new Vector3(-.05f,.48f,-.43f),new Vector3(2.18f,.12f,.08f),warmWood);
        DecorPrimitive(root.transform,"Starboard Gunwale",PrimitiveType.Cube,new Vector3(-.05f,.48f,.43f),new Vector3(2.18f,.12f,.08f),warmWood);
        DecorPrimitive(root.transform,"Keel",PrimitiveType.Cube,new Vector3(-.10f,-.22f,0f),new Vector3(2.18f,.08f,.12f),bronze);

        DecorPrimitive(root.transform,"Prow",PrimitiveType.Cube,new Vector3(1.45f,.22f,0f),new Vector3(.68f,.42f,.58f),warmWood,Quaternion.Euler(0f,0f,-23f));
        DecorPrimitive(root.transform,"Prow Horn",PrimitiveType.Cube,new Vector3(1.88f,.35f,0f),new Vector3(.44f,.09f,.09f),bronze,Quaternion.Euler(0f,0f,-17f));
        DecorPrimitive(root.transform,"Stern",PrimitiveType.Cube,new Vector3(-1.35f,.24f,0f),new Vector3(.45f,.50f,.62f),new Color(.36f,.17f,.07f),Quaternion.Euler(0f,0f,18f));

        DecorPrimitive(root.transform,"Mast",PrimitiveType.Cylinder,new Vector3(-.05f,1.02f,0f),new Vector3(.045f,.95f,.045f),new Color(.32f,.18f,.08f));
        DecorPrimitive(root.transform,"Yard",PrimitiveType.Cylinder,new Vector3(-.05f,1.55f,0f),new Vector3(.035f,.62f,.035f),warmWood,Quaternion.Euler(90f,0f,0f));
        DecorPrimitive(root.transform,"Square Sail",PrimitiveType.Cube,new Vector3(-.05f,1.12f,0f),new Vector3(.05f,.78f,.92f),sailColor);
        DecorPrimitive(root.transform,"Sail Stripe",PrimitiveType.Cube,new Vector3(-.085f,1.12f,0f),new Vector3(.012f,.13f,.94f),new Color(.34f,.40f,.50f));

        for (int i = -3; i <= 3; i++)
        {
            float x = i * .34f;
            DecorPrimitive(root.transform,"Bench",PrimitiveType.Cube,new Vector3(x,.39f,0f),new Vector3(.11f,.045f,.62f),warmWood);

            GameObject oar = DecorPrimitive(root.transform,"Oar",PrimitiveType.Cylinder,new Vector3(x,.31f,0f),new Vector3(.022f,.88f,.022f),warmWood,Quaternion.Euler(90f,0f,0f));
            oar.transform.localRotation = Quaternion.Euler(90f,0f,i%2==0 ? 7f : -7f);

            GameObject shield = DecorPrimitive(root.transform,"Hull Shield",PrimitiveType.Cylinder,new Vector3(x,.48f,-.48f),new Vector3(.17f,.04f,.17f),i % 2 == 0 ? bronze : new Color(.52f,.43f,.25f),Quaternion.Euler(90f,0f,0f));
            shield.transform.localRotation = Quaternion.Euler(90f,0f,0f);
        }

        return root;
    }

    GameObject DecorPrimitive(Transform parent, string name, PrimitiveType type, Vector3 localPosition, Vector3 localScale, Color color, Quaternion? localRotation = null)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPosition;
        go.transform.localScale = localScale;
        if (localRotation.HasValue) go.transform.localRotation = localRotation.Value;
        Collider collider = go.GetComponent<Collider>();
        if (collider != null) Destroy(collider);
        TowerFactory.SetColor(go, color);
        return go;
    }
}
