using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class MainMenuAmbientPresentation : MonoBehaviour
{
    readonly List<RectTransform> embers = new List<RectTransform>();
    readonly List<float> emberSpeeds = new List<float>();
    readonly List<float> emberDrift = new List<float>();
    Canvas canvas;
    RectTransform ambientRoot;
    float phase;

    void Start()
    {
        canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;
        Transform mainMenu = canvas.transform.Find("MainMenu");
        if (mainMenu == null) return;
        Build(mainMenu);
    }

    void Build(Transform mainMenu)
    {
        Transform old = mainMenu.Find("AmbientPresentation");
        if (old != null) Destroy(old.gameObject);

        GameObject root = new GameObject("AmbientPresentation");
        root.transform.SetParent(mainMenu, false);
        ambientRoot = root.AddComponent<RectTransform>();
        ambientRoot.anchorMin = Vector2.zero;
        ambientRoot.anchorMax = Vector2.one;
        ambientRoot.offsetMin = ambientRoot.offsetMax = Vector2.zero;
        ambientRoot.SetAsFirstSibling();

        CreateVignette(root.transform);
        CreateSunGlow(root.transform);
        CreateEmbers(root.transform);
    }

    void CreateVignette(Transform parent)
    {
        GameObject go = new GameObject("WarmVignette");
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = new Color(.16f, .045f, .012f, .10f);
        Stretch(image.rectTransform);
        image.raycastTarget = false;
    }

    void CreateSunGlow(Transform parent)
    {
        GameObject go = new GameObject("SunGlow");
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = new Color(1f, .43f, .08f, .10f);
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.78f, .72f);
        rt.sizeDelta = new Vector2(500f, 500f);
        image.raycastTarget = false;
    }

    void CreateEmbers(Transform parent)
    {
        const int count = 22;
        for (int i = 0; i < count; i++)
        {
            GameObject go = new GameObject("Ember_" + i);
            go.transform.SetParent(parent, false);
            Image image = go.AddComponent<Image>();
            image.color = new Color(1f, .42f + (i % 3) * .08f, .08f, .18f + (i % 4) * .045f);
            image.raycastTarget = false;
            RectTransform rt = image.rectTransform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0f, 0f);
            float x = 780f + (i * 83f) % 980f;
            float y = 40f + (i * 137f) % 980f;
            rt.anchoredPosition = new Vector2(x, y);
            float size = 3f + (i % 4) * 2f;
            rt.sizeDelta = new Vector2(size, size * 1.8f);
            embers.Add(rt);
            emberSpeeds.Add(10f + (i % 7) * 3.5f);
            emberDrift.Add(.45f + (i % 5) * .18f);
        }
    }

    void Update()
    {
        if (ambientRoot == null || !ambientRoot.gameObject.activeInHierarchy) return;
        phase += Time.unscaledDeltaTime;
        for (int i = 0; i < embers.Count; i++)
        {
            RectTransform rt = embers[i];
            if (rt == null) continue;
            Vector2 p = rt.anchoredPosition;
            p.y += emberSpeeds[i] * Time.unscaledDeltaTime;
            p.x += Mathf.Sin(phase * emberDrift[i] + i) * 5f * Time.unscaledDeltaTime;
            if (p.y > 1080f)
            {
                p.y = -15f;
                p.x = 760f + ((i * 149f + Mathf.FloorToInt(phase * 37f)) % 1020f);
            }
            rt.anchoredPosition = p;
        }
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }
}
