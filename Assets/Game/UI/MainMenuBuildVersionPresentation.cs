using UnityEngine;
using UnityEngine.UI;

public sealed class MainMenuBuildVersionPresentation : MonoBehaviour
{
    GameObject boundMainMenu;
    GameObject badge;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<MainMenuBuildVersionPresentation>() == null)
            new GameObject("MainMenuBuildVersionPresentation").AddComponent<MainMenuBuildVersionPresentation>();
    }

    void Update()
    {
        if (boundMainMenu != null && badge != null) return;

        GameObject mainMenu = GameObject.Find("MainMenu");
        if (mainMenu == null) return;

        boundMainMenu = mainMenu;
        Transform existing = mainMenu.transform.Find("BuildVersionBadge");
        if (existing != null)
        {
            badge = existing.gameObject;
            return;
        }

        badge = BuildBadge(mainMenu.transform);
    }

    static GameObject BuildBadge(Transform parent)
    {
        GameObject root = new GameObject("BuildVersionBadge");
        root.transform.SetParent(parent, false);

        Image background = root.AddComponent<Image>();
        background.color = new Color(.035f, .018f, .010f, .78f);
        background.raycastTarget = false;

        RectTransform rect = background.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(22f, -22f);
        rect.sizeDelta = new Vector2(390f, 50f);

        Outline outline = root.AddComponent<Outline>();
        outline.effectColor = new Color(.73f, .43f, .16f, .55f);
        outline.effectDistance = new Vector2(1f, -1f);

        GameObject labelObject = new GameObject("BuildVersionText");
        labelObject.transform.SetParent(root.transform, false);
        Text label = labelObject.AddComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.text = BuildVersionInfo.MenuBadge;
        label.fontSize = 15;
        label.fontStyle = FontStyle.Bold;
        label.color = new Color(1f, .82f, .52f, .96f);
        label.alignment = TextAnchor.MiddleLeft;
        label.horizontalOverflow = HorizontalWrapMode.Overflow;
        label.verticalOverflow = VerticalWrapMode.Truncate;
        label.raycastTarget = false;

        RectTransform labelRect = label.rectTransform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(16f, 0f);
        labelRect.offsetMax = new Vector2(-12f, 0f);

        Shadow shadow = labelObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, .75f);
        shadow.effectDistance = new Vector2(1f, -1f);

        return root;
    }
}
