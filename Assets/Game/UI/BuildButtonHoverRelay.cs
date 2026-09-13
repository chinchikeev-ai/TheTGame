using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class BuildButtonHoverRelay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Action onEnter;
    Action onExit;

    static readonly Color bronze = new Color(.76f, .39f, .12f, .96f);
    static readonly Color gold = new Color(1f, .72f, .22f, .98f);
    static readonly Color red = new Color(.78f, .16f, .07f, .98f);
    static readonly Color blue = new Color(.18f, .48f, .72f, .98f);
    static readonly Color green = new Color(.23f, .62f, .30f, .98f);
    static readonly Color purple = new Color(.48f, .31f, .66f, .98f);

    public void Initialize(Action enter, Action exit)
    {
        onEnter = enter;
        onExit = exit;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke();
        ApplyTacticalVisuals();
    }

    public void OnPointerExit(PointerEventData eventData) => onExit?.Invoke();

    void OnDisable() => onExit?.Invoke();

    void ApplyTacticalVisuals()
    {
        Transform root = transform.root;
        Transform tooltip = root != null ? root.Find("BuildHoverTooltip") : null;
        if (tooltip == null) return;

        int slot = InferSlot();
        ConfigureSilhouette(tooltip, slot);
        BuildBadges(tooltip, slot);
        HideLegacyTagLine(tooltip);
    }

    int InferSlot()
    {
        string n = gameObject.name;
        for (int slot = 1; slot <= 6; slot++)
            if (n.StartsWith("[" + slot + "]", StringComparison.Ordinal)) return slot;
        return 0;
    }

    void ConfigureSilhouette(Transform tooltip, int slot)
    {
        RectTransform head = FindRect(tooltip, "SilhouetteHead");
        RectTransform body = FindRect(tooltip, "SilhouetteBody");
        RectTransform weapon = FindRect(tooltip, "SilhouetteWeapon");
        if (head == null || body == null || weapon == null) return;

        Image headImage = head.GetComponent<Image>();
        Image bodyImage = body.GetComponent<Image>();
        Image weaponImage = weapon.GetComponent<Image>();
        if (headImage == null || bodyImage == null || weaponImage == null) return;

        head.anchoredPosition = new Vector2(-214f, 36f);
        head.sizeDelta = new Vector2(34f, 34f);
        head.localRotation = Quaternion.identity;
        body.anchoredPosition = new Vector2(-214f, -20f);
        body.sizeDelta = new Vector2(54f, 74f);
        body.localRotation = Quaternion.identity;
        weapon.anchoredPosition = new Vector2(-182f, -2f);
        weapon.sizeDelta = new Vector2(10f, 104f);
        weapon.localRotation = Quaternion.Euler(0f, 0f, -18f);

        headImage.color = new Color(.74f, .42f, .14f, .94f);
        bodyImage.color = new Color(.56f, .25f, .09f, .94f);
        weaponImage.color = gold;

        switch (slot)
        {
            case 1: // Spear Throwers
                head.sizeDelta = new Vector2(31f, 31f);
                body.sizeDelta = new Vector2(48f, 72f);
                weapon.anchoredPosition = new Vector2(-176f, -2f);
                weapon.sizeDelta = new Vector2(7f, 124f);
                weapon.localRotation = Quaternion.Euler(0f, 0f, -12f);
                weaponImage.color = new Color(.92f, .74f, .34f, .98f);
                break;
            case 2: // Archers
                body.sizeDelta = new Vector2(46f, 68f);
                weapon.anchoredPosition = new Vector2(-178f, 3f);
                weapon.sizeDelta = new Vector2(8f, 86f);
                weapon.localRotation = Quaternion.Euler(0f, 0f, 35f);
                weaponImage.color = new Color(.82f, .54f, .22f, .98f);
                break;
            case 3: // Ballista
                head.anchoredPosition = new Vector2(-220f, 25f);
                head.sizeDelta = new Vector2(27f, 27f);
                body.anchoredPosition = new Vector2(-219f, -23f);
                body.sizeDelta = new Vector2(42f, 58f);
                weapon.anchoredPosition = new Vector2(-184f, -15f);
                weapon.sizeDelta = new Vector2(72f, 18f);
                weapon.localRotation = Quaternion.Euler(0f, 0f, 0f);
                weaponImage.color = new Color(.72f, .50f, .20f, .98f);
                break;
            case 4: // Apollo
                head.sizeDelta = new Vector2(36f, 36f);
                body.sizeDelta = new Vector2(50f, 76f);
                bodyImage.color = new Color(.54f, .40f, .13f, .96f);
                weapon.anchoredPosition = new Vector2(-174f, -5f);
                weapon.sizeDelta = new Vector2(9f, 110f);
                weapon.localRotation = Quaternion.Euler(0f, 0f, 4f);
                weaponImage.color = new Color(1f, .87f, .36f, .98f);
                break;
            case 5: // Fire Crew
                head.sizeDelta = new Vector2(32f, 32f);
                body.sizeDelta = new Vector2(58f, 72f);
                bodyImage.color = new Color(.65f, .20f, .07f, .96f);
                weapon.anchoredPosition = new Vector2(-180f, -8f);
                weapon.sizeDelta = new Vector2(30f, 60f);
                weapon.localRotation = Quaternion.Euler(0f, 0f, -28f);
                weaponImage.color = new Color(1f, .30f, .05f, .98f);
                break;
            case 6: // Shield Guard
                head.sizeDelta = new Vector2(36f, 36f);
                body.sizeDelta = new Vector2(62f, 78f);
                bodyImage.color = new Color(.48f, .20f, .08f, .98f);
                weapon.anchoredPosition = new Vector2(-178f, -8f);
                weapon.sizeDelta = new Vector2(44f, 66f);
                weapon.localRotation = Quaternion.identity;
                weaponImage.color = new Color(.75f, .45f, .14f, .98f);
                break;
        }
    }

    void BuildBadges(Transform tooltip, int slot)
    {
        Transform old = tooltip.Find("TacticalBadges");
        if (old != null) Destroy(old.gameObject);

        GameObject row = new GameObject("TacticalBadges");
        row.transform.SetParent(tooltip, false);
        RectTransform rowRt = row.AddComponent<RectTransform>();
        rowRt.anchorMin = rowRt.anchorMax = rowRt.pivot = new Vector2(.5f, .5f);
        rowRt.anchoredPosition = new Vector2(-72f, -54f);
        rowRt.sizeDelta = new Vector2(390f, 34f);

        string[] labels;
        Color[] colors;
        switch (slot)
        {
            case 1:
                labels = new[] { "ARMOR", "HEAVY" };
                colors = new[] { bronze, red };
                break;
            case 2:
                labels = new[] { "RANGED", "LIGHT" };
                colors = new[] { blue, green };
                break;
            case 3:
                labels = new[] { "SIEGE", "BOSS", "PIERCE" };
                colors = new[] { bronze, red, gold };
                break;
            case 4:
                labels = new[] { "SLOW", "SUPPORT" };
                colors = new[] { blue, purple };
                break;
            case 5:
                labels = new[] { "AOE", "BURN", "ZONE" };
                colors = new[] { red, new Color(1f, .33f, .04f, .98f), bronze };
                break;
            case 6:
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

    void CreateBadge(Transform parent, string label, Vector2 pos, float width, Color accent)
    {
        GameObject badge = new GameObject("Badge_" + label);
        badge.transform.SetParent(parent, false);
        Image background = badge.AddComponent<Image>();
        background.color = new Color(.08f, .05f, .035f, .97f);
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

    void HideLegacyTagLine(Transform tooltip)
    {
        Text[] texts = tooltip.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            RectTransform rt = texts[i].rectTransform;
            if (Vector2.Distance(rt.anchoredPosition, new Vector2(-116f, -47f)) < 2f)
            {
                texts[i].enabled = false;
                return;
            }
        }
    }

    RectTransform FindRect(Transform parent, string name)
    {
        Transform t = parent.Find(name);
        return t != null ? t.GetComponent<RectTransform>() : null;
    }
}
