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

        GameObject[] boats = new GameObject[3];
        float[] z = { 6.0f, 0f, -6.0f };
        for (int i = 0; i < boats.Length; i++)
            boats[i] = CreateBoat(root.transform, new Vector3(-18.5f - i * .8f, .12f, z[i]));

        float duration = 5.5f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            for (int i = 0; i < boats.Length; i++)
            {
                if (boats[i] == null) continue;
                Vector3 start = new Vector3(-18.5f - i * .8f, .12f, z[i]);
                Vector3 end = new Vector3(-14.7f, .10f, z[i] * .88f);
                boats[i].transform.position = Vector3.Lerp(start, end, t);
            }
            yield return null;
        }

        for (int i = 0; i < 10; i++)
        {
            GameObject soldier = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            soldier.name = "Landing Greek Silhouette";
            soldier.transform.SetParent(root.transform);
            soldier.transform.position = new Vector3(-13.5f + (i % 3) * .35f, .42f, -5.5f + i * 1.2f);
            soldier.transform.localScale = new Vector3(.30f, .42f, .30f);
            Object.Destroy(soldier.GetComponent<Collider>());
            TowerFactory.SetColor(soldier, new Color(.34f, .22f, .14f));
        }

        RuntimeFileLogger.Event("CHAPTER", "Chapter I landing presentation completed");
        Destroy(title, 1.5f);
        Destroy(root, 8f);
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
        panel.color = new Color(.025f, .03f, .04f, .80f);
        RectTransform pr = panel.rectTransform;
        pr.anchorMin = new Vector2(.5f, .5f);
        pr.anchorMax = new Vector2(.5f, .5f);
        pr.pivot = new Vector2(.5f, .5f);
        pr.anchoredPosition = new Vector2(0, 160);
        pr.sizeDelta = new Vector2(760, 150);

        Text headline = MakeText(panelObj.transform, "Headline", 38, TextAnchor.MiddleCenter);
        headline.text = GameLanguage.T("CHAPTER I — THE LANDING", "ГЛАВА I — ВЫСАДКА");
        RectTransform hr = headline.rectTransform;
        hr.anchorMin = new Vector2(0, .45f);
        hr.anchorMax = Vector2.one;
        hr.offsetMin = new Vector2(20, 0);
        hr.offsetMax = new Vector2(-20, -8);

        Text sub = MakeText(panelObj.transform, "Subtitle", 18, TextAnchor.MiddleCenter);
        sub.text = GameLanguage.T("Hold the shore. Protect Troy. Menelaus is coming.", "Удержите берег. Защитите Трою. Менелай идёт.");
        RectTransform sr = sub.rectTransform;
        sr.anchorMin = Vector2.zero;
        sr.anchorMax = new Vector2(1, .45f);
        sr.offsetMin = new Vector2(20, 10);
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
        hull.transform.localScale = new Vector3(2.1f, .30f, .72f);
        Object.Destroy(hull.GetComponent<Collider>());
        TowerFactory.SetColor(hull, new Color(.28f, .17f, .09f));

        GameObject shieldLine = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shieldLine.transform.SetParent(root.transform, false);
        shieldLine.transform.localPosition = new Vector3(.1f, .38f, 0f);
        shieldLine.transform.localScale = new Vector3(1.5f, .32f, .15f);
        Object.Destroy(shieldLine.GetComponent<Collider>());
        TowerFactory.SetColor(shieldLine, new Color(.55f, .38f, .18f));
        return root;
    }
}
