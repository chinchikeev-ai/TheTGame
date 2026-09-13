using UnityEngine;
using UnityEngine.UI;

public static class BuildTooltipTacticalDecorator
{
    static readonly Color bronze = new Color(.76f, .39f, .12f, .96f);
    static readonly Color gold = new Color(1f, .72f, .22f, .98f);
    static readonly Color red = new Color(.78f, .16f, .07f, .98f);
    static readonly Color blue = new Color(.18f, .48f, .72f, .98f);
    static readonly Color green = new Color(.23f, .62f, .30f, .98f);
    static readonly Color purple = new Color(.48f, .31f, .66f, .98f);

    public static void Apply(Transform buildButton, TowerType towerType)
    {
        if (buildButton == null) return;
        Transform root = buildButton.root;
        Transform tooltip = root != null ? root.Find("BuildHoverTooltip") : null;
        if (tooltip == null) return;

        ConfigurePortrait(tooltip, towerType);
        BuildBadges(tooltip, towerType);
    }

    static void ConfigurePortrait(Transform tooltip, TowerType towerType)
    {
        Transform existing = tooltip.Find("TowerArtPortrait");
        Image image;
        if (existing == null)
        {
            GameObject portrait = new GameObject("TowerArtPortrait");
            portrait.transform.SetParent(tooltip, false);
            image = portrait.AddComponent<Image>();
            image.raycastTarget = false;
            RectTransform rt = image.rectTransform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
            rt.anchoredPosition = new Vector2(-205f, 0f);
            rt.sizeDelta = new Vector2(96f, 96f);
        }
        else
        {
            image = existing.GetComponent<Image>();
            if (image == null) image = existing.gameObject.AddComponent<Image>();
        }

        image.sprite = TroyHudArt.Tower(towerType);
        image.color = Color.white;
    }

    static void BuildBadges(Transform tooltip, TowerType towerType)
    {
        Transform old = tooltip.Find("TacticalBadges");
        if (old != null) Object.Destroy(old.gameObject);

        GameObject row = new GameObject("TacticalBadges");
        row.transform.SetParent(tooltip, false);
        RectTransform rowRt = row.AddComponent<RectTransform>();
        rowRt.anchorMin = rowRt.anchorMax = rowRt.pivot = new Vector2(.5f, .5f);
        rowRt.anchoredPosition = new Vector2(-72f, -54f);
        rowRt.sizeDelta = new Vector2(390f, 34f);

        string[] labels;
        Color[] colors;
        switch (towerType)
        {
            case TowerType.SpearThrower:
                labels = new[] { "ARMOR", "HEAVY" };
                colors = new[] { bronze, red };
                break;
            case TowerType.MachineGun:
                labels = new[] { "RANGED", "LIGHT" };
                colors = new[] { blue, green };
                break;
            case TowerType.Cannon:
                labels = new[] { "SIEGE", "BOSS", "PIERCE" };
                colors = new[] { bronze, red, gold };
                break;
            case TowerType.Slow:
                labels = new[] { "SLOW", "SUPPORT" };
                colors = new[] { blue, purple };
                break;
            case TowerType.FireTower:
                labels = new[] { "AOE", "BURN", "ZONE" };
                colors = new[] { red, new Color(1f, .33f, .04f, .98f), bronze };
                break;
            case TowerType.TrojanGuard:
                labels = new[] { "BLOCK", "FRONT" };
                colors = new[] { bronze, red };
                break;
            default:
                labels = new[] { "TACTICAL" };
                colors = new[] { bronze };
                break;
        }

        float x = -rowRt.sizeDelta.x * .5f + 44f;
        for (int i = 0; i < labels.Length; i++)
        {
            float width = Mathf.Max(66f, labels[i].Length * 8.5f + 22f);
            CreateBadge(row.transform, labels[i], new Vector2(x + width * .5f, 0f), width, colors[i]);
            x += width + 8f;
        }
    }

    static void CreateBadge(Transform parent, string label, Vector2 pos, float width, Color accent)
    {
        GameObject badge = new GameObject("Badge_" + label);
        badge.transform.SetParent(parent, false);
        Image background = badge.AddComponent<Image>();
        background.sprite = TroyHudArt.Panel();
        background.type = Image.Type.Sliced;
        background.color = new Color(.22f, .11f, .045f, .98f);
        background.raycastTarget = false;

        RectTransform rt = background.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(width, 27f);

        Outline outline = badge.AddComponent<Outline>();
        outline.effectColor = accent;
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        GameObject stripe = new GameObject("Icon");
        stripe.transform.SetParent(badge.transform, false);
        Image stripeImage = stripe.AddComponent<Image>();
        stripeImage.color = accent;
        stripeImage.raycastTarget = false;
        RectTransform stripeRt = stripeImage.rectTransform;
        stripeRt.anchorMin = stripeRt.anchorMax = stripeRt.pivot = new Vector2(0f, .5f);
        stripeRt.anchoredPosition = new Vector2(5f, 0f);
        stripeRt.sizeDelta = new Vector2(6f, 17f);

        GameObject textObject = new GameObject("Label");
        textObject.transform.SetParent(badge.transform, false);
        Text text = textObject.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = label;
        text.fontSize = 11;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(.98f, .90f, .78f, 1f);
        text.raycastTarget = false;
        RectTransform textRt = text.rectTransform;
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = new Vector2(10f, 0f);
        textRt.offsetMax = Vector2.zero;
    }
}
