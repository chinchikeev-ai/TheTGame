using UnityEngine;
using UnityEngine.UI;

// Final presentation-only compacting pass for the real Chapter I gameplay frame.
// It intentionally does not own UI content or interaction; it only reduces the
// visual footprint of existing panels after their normal owners have built them.
public sealed class ChapterOneUiCompactPresentation : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<ChapterOneUiCompactPresentation>() == null)
            new GameObject("ChapterOneUiCompactPresentation").AddComponent<ChapterOneUiCompactPresentation>();
    }

    void LateUpdate()
    {
        GameManager gm = GameManager.Instance;
        if (gm != null && gm.MapNumber != 1) return;

        ApplyModernCombatHud();
        ApplyGuidanceCards();
        ApplyPatronCard();
        ApplyCombatActions();
    }

    void ApplyModernCombatHud()
    {
        GameObject hud = GameObject.Find("ModernCombatHUD");
        if (hud == null) return;

        Transform top = FindDescendant(hud.transform, "TopResources");
        SetScale(top, .80f);
        SetAnchoredPosition(top, new Vector2(16f, -16f));

        Transform wave = FindDescendant(hud.transform, "WaveStatus");
        SetScale(wave, .82f);
        SetAnchoredPosition(wave, new Vector2(0f, -14f));

        Transform prep = FindDescendant(hud.transform, "FirstWavePreparation");
        SetScale(prep, .68f);
        SetAnchoredPosition(prep, new Vector2(0f, -148f));
        SetPanelAlpha(prep, .90f);

        Transform buildDock = FindDescendant(hud.transform, "BuildDock");
        SetScale(buildDock, .84f);

        Transform selectedCard = FindDescendant(hud.transform, "SelectedTowerCard");
        SetScale(selectedCard, .84f);

        Transform tooltip = FindDescendant(hud.transform, "BuildHoverTooltip");
        SetScale(tooltip, .84f);

        Transform magicToggle = FindDescendant(hud.transform, "MagicToggle");
        SetScale(magicToggle, .82f);

        Transform defendersToggle = FindDescendant(hud.transform, "DefendersToggle");
        SetScale(defendersToggle, .82f);

        Transform magicFlyout = FindDescendant(hud.transform, "MagicFlyout");
        SetScale(magicFlyout, .80f);
    }

    void ApplyGuidanceCards()
    {
        GameObject root = GameObject.Find("ChapterOneGuidanceUI");
        if (root == null) return;

        Transform objective = FindDescendant(root.transform, "ChapterObjective");
        SetScale(objective, .74f);
        SetAnchoredPosition(objective, new Vector2(18f, -138f));

        Transform tutorial = FindDescendant(root.transform, "ContextTutorial");
        SetScale(tutorial, .74f);
        SetAnchoredPosition(tutorial, new Vector2(18f, -224f));
    }

    void ApplyPatronCard()
    {
        GameObject root = GameObject.Find("PatronCommentaryUI");
        if (root == null) return;

        Transform card = FindDescendant(root.transform, "PatronCommentaryCard");
        SetScale(card, .78f);
        SetPanelAlpha(card, .93f);
    }

    void ApplyCombatActions()
    {
        GameObject actions = GameObject.Find("CombatActions");
        if (actions == null) return;

        SetScale(actions.transform, .78f);
        SetPanelAlpha(actions.transform, .93f);
    }

    static Transform FindDescendant(Transform root, string objectName)
    {
        if (root == null) return null;
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < all.Length; i++)
            if (all[i] != null && all[i].name == objectName) return all[i];
        return null;
    }

    static void SetScale(Transform target, float scale)
    {
        if (target == null) return;
        target.localScale = Vector3.one * scale;
    }

    static void SetAnchoredPosition(Transform target, Vector2 position)
    {
        RectTransform rect = target as RectTransform;
        if (rect != null) rect.anchoredPosition = position;
    }

    static void SetPanelAlpha(Transform target, float alpha)
    {
        if (target == null) return;
        Image image = target.GetComponent<Image>();
        if (image == null) return;
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
