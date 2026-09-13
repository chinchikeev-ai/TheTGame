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
        {
            boats[i] = GreekLandingShipVisualFactory.Create(root.transform, new Vector3(-19.2f - i * .55f, .12f, z[i]), "Incoming Achaean Galley");
            CreateBoatWake(boats[i].transform,i);
        }

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
                float yaw = -3.5f + i * 2.2f + Mathf.Sin(elapsed + i) * 1.4f;
                boats[i].transform.rotation = Quaternion.Euler(0f, yaw, Mathf.Sin(elapsed * 2f + i) * 1.8f);
            }
            yield return null;
        }

        CreateLandingDebris(root.transform);
        RuntimeEffects.Instance?.PlayHeroAbilitySound();
        CombatImpactPresentation.Pulse(new Vector3(-13.6f,.12f,0f), new Color(.72f,.55f,.28f), 6.5f, .55f);
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

    void CreateBoatWake(Transform boat,int index)
    {
        GameObject wakeA = DecorPrimitive(boat,"Cinematic Bow Wake",PrimitiveType.Sphere,
            new Vector3(1.18f,-.27f,0f),new Vector3(.72f,.012f,.48f),new Color(.70f,.80f,.79f));
        ChapterOneAmbientMotion motionA = wakeA.AddComponent<ChapterOneAmbientMotion>();
        motionA.kind = ChapterOneAmbientMotion.MotionKind.Sea;
        motionA.phase = .4f + index * .7f;

        GameObject wakeB = DecorPrimitive(boat,"Cinematic Stern Wake",PrimitiveType.Sphere,
            new Vector3(-1.55f,-.28f,0f),new Vector3(1.45f,.010f,.10f),new Color(.54f,.70f,.71f));
        ChapterOneAmbientMotion motionB = wakeB.AddComponent<ChapterOneAmbientMotion>();
        motionB.kind = ChapterOneAmbientMotion.MotionKind.Sea;
        motionB.phase = 1.1f + index * .63f;
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
            GameObject shield = DecorPrimitive(parent,"Discarded Greek Shield",PrimitiveType.Cylinder,new Vector3(-11.95f,.16f,z+.48f),new Vector3(.24f,.045f,.24f),cloth,Quaternion.Euler(82f,0f,18f+i*7f));
            DecorPrimitive(shield.transform,"Shield Boss",PrimitiveType.Sphere,new Vector3(0f,.06f,0f),new Vector3(.42f,.12f,.42f),bronze);
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
