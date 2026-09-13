using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LandingPresentation : MonoBehaviour
{
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
            GameObject soldier = RuntimeWarriorVisualFactory.CreateEnemyFallback(archetype, new Color(.42f,.52f,.68f));
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
        GameObject root = new GameObject("Incoming Landing Boat");
        root.transform.SetParent(parent);
        root.transform.position = position;

        GameObject hull = GameObject.CreatePrimitive(PrimitiveType.Cube);
        hull.transform.SetParent(root.transform, false);
        hull.transform.localScale = new Vector3(2.5f, .28f, .78f);
        Object.Destroy(hull.GetComponent<Collider>());
        TowerFactory.SetColor(hull, new Color(.25f, .13f, .065f));

        GameObject prow = GameObject.CreatePrimitive(PrimitiveType.Cube);
        prow.transform.SetParent(root.transform, false);
        prow.transform.localPosition = new Vector3(1.45f,.22f,0f);
        prow.transform.localScale = new Vector3(.68f,.42f,.58f);
        prow.transform.localRotation = Quaternion.Euler(0f,0f,-23f);
        Object.Destroy(prow.GetComponent<Collider>());
        TowerFactory.SetColor(prow,new Color(.42f,.20f,.08f));

        GameObject stern = GameObject.CreatePrimitive(PrimitiveType.Cube);
        stern.transform.SetParent(root.transform,false);
        stern.transform.localPosition = new Vector3(-1.35f,.24f,0f);
        stern.transform.localScale = new Vector3(.45f,.50f,.62f);
        stern.transform.localRotation = Quaternion.Euler(0f,0f,18f);
        Object.Destroy(stern.GetComponent<Collider>());
        TowerFactory.SetColor(stern,new Color(.36f,.17f,.07f));

        GameObject mast = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        mast.transform.SetParent(root.transform,false);
        mast.transform.localPosition = new Vector3(-.05f,1.02f,0f);
        mast.transform.localScale = new Vector3(.045f,.95f,.045f);
        Object.Destroy(mast.GetComponent<Collider>());
        TowerFactory.SetColor(mast,new Color(.32f,.18f,.08f));

        GameObject sail = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sail.transform.SetParent(root.transform,false);
        sail.transform.localPosition = new Vector3(-.05f,1.12f,0f);
        sail.transform.localScale = new Vector3(.05f,.78f,.92f);
        Object.Destroy(sail.GetComponent<Collider>());
        TowerFactory.SetColor(sail,new Color(.72f,.64f,.50f));

        for (int i = -3; i <= 3; i++)
        {
            GameObject shield = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            shield.transform.SetParent(root.transform,false);
            shield.transform.localPosition = new Vector3(i*.34f,.38f,-.43f);
            shield.transform.localRotation = Quaternion.Euler(90f,0f,0f);
            shield.transform.localScale = new Vector3(.17f,.04f,.17f);
            Object.Destroy(shield.GetComponent<Collider>());
            TowerFactory.SetColor(shield, i % 2 == 0 ? new Color(.55f,.25f,.08f) : new Color(.52f,.43f,.25f));
        }

        return root;
    }
}
