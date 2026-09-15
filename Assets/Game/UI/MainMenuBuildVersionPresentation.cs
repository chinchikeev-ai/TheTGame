using UnityEngine;
using UnityEngine.UI;

public sealed class MainMenuBuildVersionPresentation : MonoBehaviour
{
    GameObject boundMainMenu;
    GameObject badge;
    Text label;
    float nextRefreshAt;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<MainMenuBuildVersionPresentation>() == null)
            new GameObject("MainMenuBuildVersionPresentation").AddComponent<MainMenuBuildVersionPresentation>();
    }

    void Update()
    {
        if (boundMainMenu != null && badge != null)
        {
            if (!badge.activeSelf) badge.SetActive(true);
            if (badge.transform.GetSiblingIndex() != boundMainMenu.transform.childCount - 1)
                badge.transform.SetAsLastSibling();
        }

        if (boundMainMenu == null || badge == null || label == null)
            BindOrBuildBadge();

        if (label != null && Time.unscaledTime >= nextRefreshAt)
        {
            label.text = BuildVersionInfo.CompactMenuBadge;
            nextRefreshAt = Time.unscaledTime + 1f;
        }
    }

    void BindOrBuildBadge()
    {
        GameObject mainMenu = GameObject.Find("MainMenu");
        if (mainMenu == null) return;

        boundMainMenu = mainMenu;
        Transform existing = mainMenu.transform.Find("BuildVersionBadge");
        if (existing != null)
        {
            badge = existing.gameObject;
            label = badge.GetComponentInChildren<Text>(true);
            ApplyCompactLayout();
            return;
        }

        badge = BuildBadge(mainMenu.transform, out label);
        nextRefreshAt = 0f;
    }

    void ApplyCompactLayout()
    {
        if (badge == null) return;
        RectTransform rect = badge.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(18f, -18f);
            rect.sizeDelta = new Vector2(220f, 40f);
        }
        if (label != null)
        {
            label.fontSize = 14;
            label.alignment = TextAnchor.MiddleCenter;
        }
    }

    static GameObject BuildBadge(Transform parent, out Text label)
    {
        GameObject root = new GameObject("BuildVersionBadge");
        root.transform.SetParent(parent, false);

        Image background = root.AddComponent<Image>();
        background.color = new Color(.035f, .018f, .010f, .76f);
        background.raycastTarget = false;

        RectTransform rect = background.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(18f, -18f);
        rect.sizeDelta = new Vector2(220f, 40f);

        Outline outline = root.AddComponent<Outline>();
        outline.effectColor = new Color(.73f, .43f, .16f, .62f);
        outline.effectDistance = new Vector2(1f, -1f);

        GameObject labelObject = new GameObject("BuildVersionText");
        labelObject.transform.SetParent(root.transform, false);
        label = labelObject.AddComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.text = BuildVersionInfo.CompactMenuBadge;
        label.fontSize = 14;
        label.fontStyle = FontStyle.Bold;
        label.color = new Color(1f, .82f, .52f, .98f);
        label.alignment = TextAnchor.MiddleCenter;
        label.horizontalOverflow = HorizontalWrapMode.Overflow;
        label.verticalOverflow = VerticalWrapMode.Truncate;
        label.raycastTarget = false;

        RectTransform labelRect = label.rectTransform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(8f, 0f);
        labelRect.offsetMax = new Vector2(-8f, 0f);

        Shadow shadow = labelObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, .80f);
        shadow.effectDistance = new Vector2(1f, -1f);

        return root;
    }
}
