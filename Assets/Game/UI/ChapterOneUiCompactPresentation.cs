using UnityEngine;
using UnityEngine.UI;

// Final presentation-only compacting pass for the real Chapter I gameplay frame.
// It intentionally does not own UI content or interaction; it only reduces the
// visual footprint of existing panels after their normal owners have built them.
public sealed class ChapterOneUiCompactPresentation : MonoBehaviour
{
    const float HectorScale = .75f;
    const float CornerActionGap = 12f;
    static readonly Vector2 CornerActionSize = new Vector2(128f, 112f);
    static readonly Vector2 CornerActionPairSize = new Vector2(
        CornerActionSize.x * 2f + CornerActionGap,
        CornerActionSize.y);

    Text compactMagicText;
    HectorHUD hectorHud;
    ChapterOneGuidancePresentation guidance;
    PatronCommentaryPresentation patron;
    Transform boundHud, boundHector, boundGuidance, boundPatron;

    public void Initialize(HectorHUD hector, ChapterOneGuidancePresentation chapterGuidance, PatronCommentaryPresentation patronCommentary)
    {
        hectorHud = hector;
        guidance = chapterGuidance;
        patron = patronCommentary;
    }

    void LateUpdate()
    {
        // Roots can arrive after the menu closes. Bind independently, once per root.
        TryBindAndApply();
        if (compactMagicText != null)
            compactMagicText.text = BuildCompactMagicLabel();
    }

    bool TryBindAndApply()
    {
        Transform hud = ModernCombatHud.Instance != null ? ModernCombatHud.Instance.HudRoot : null;
        Transform hector = hectorHud != null ? hectorHud.HudRoot : null;
        Transform guidanceRoot = guidance != null ? guidance.UiRoot : null;
        Transform patronRoot = patron != null ? patron.UiRoot : null;
        if (hud != null && hud != boundHud) { ApplyModernCombatHud(hud); boundHud = hud; }
        if (hector != null && hector != boundHector) { ApplyHectorHud(hector); boundHector = hector; }
        if (guidanceRoot != null && guidanceRoot != boundGuidance) { ApplyGuidanceCards(guidanceRoot); boundGuidance = guidanceRoot; }
        if (patronRoot != null && patronRoot != boundPatron) { ApplyPatronCard(patronRoot); boundPatron = patronRoot; }
        return hud != null && hector != null && guidanceRoot != null && patronRoot != null;
    }

    void ApplyModernCombatHud(Transform hud)
    {

        Transform top = FindDescendant(hud, "TopResources");
        SetScale(top, .80f);
        SetAnchoredPosition(top, new Vector2(16f, -16f));

        Transform wave = FindDescendant(hud, "WaveStatus");
        SetScale(wave, .78f);
        SetAnchoredPosition(wave, new Vector2(0f, -12f));

        Transform prep = FindDescendant(hud, "FirstWavePreparation");
        SetScale(prep, .66f);
        SetAnchoredPosition(prep, new Vector2(0f, -142f));
        SetPanelAlpha(prep, .90f);

        Transform buildDock = FindDescendant(hud, "BuildDock");
        SetScale(buildDock, .78f);

        Transform selectedCard = FindDescendant(hud, "SelectedTowerCard");
        SetScale(selectedCard, .80f);

        Transform tooltip = FindDescendant(hud, "BuildHoverTooltip");
        SetScale(tooltip, .80f);

        ApplyBottomRightActions(hud);
    }

    void ApplyHectorHud(Transform hector)
    {
        Transform panel = FindDescendant(hector, "HectorPanel");
        if (panel == null) return;

        SetScale(panel, HectorScale);
        RectTransform rect = panel as RectTransform;
        if (rect == null) return;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0f, 0f);
        rect.anchoredPosition = new Vector2(14f, 14f);
    }

    void ApplyBottomRightActions(Transform hudRoot)
    {
        Transform defenders = FindDescendant(hudRoot, "DefendersToggle");
        Transform magicPanel = FindDescendant(hudRoot, "DivinePowerActions");
        if (defenders == null || magicPanel == null) return;

        RectTransform pair = EnsureActionPair(hudRoot);
        if (pair == null) return;

        // The pair owns only layout. Existing ModernCombatHud controls keep all
        // gameplay handlers/state. Reparenting makes the Golden Reference
        // relationship structural instead of relying on two unrelated offsets.
        if (magicPanel.parent != pair) magicPanel.SetParent(pair, false);
        if (defenders.parent != pair) defenders.SetParent(pair, false);

        ConfigurePairChild(magicPanel, new Vector2(0f, 0f), new Vector2(0f, 0f));
        ConfigurePairChild(defenders, new Vector2(1f, 0f), new Vector2(1f, 0f));

        // Keep MAGIC visually identical in footprint to DEFENDERS.
        Transform magicButton = FindDescendant(magicPanel, "Magic_Primary");
        RectTransform magicButtonRect = magicButton as RectTransform;
        if (magicButtonRect != null)
        {
            magicButtonRect.anchorMin = magicButtonRect.anchorMax = magicButtonRect.pivot = new Vector2(.5f, .5f);
            magicButtonRect.anchoredPosition = Vector2.zero;
            magicButtonRect.sizeDelta = CornerActionSize;
            magicButtonRect.localScale = Vector3.one;
        }

        Transform magicIcon = FindDescendant(magicButton, "MagicIcon");
        RectTransform iconRect = magicIcon as RectTransform;
        if (iconRect != null)
        {
            iconRect.anchoredPosition = new Vector2(0f, 28f);
            iconRect.sizeDelta = new Vector2(56f, 56f);
        }

        if (magicButton != null)
        {
            Text magicText = magicButton.GetComponentInChildren<Text>(true);
            if (magicText != null)
            {
                RectTransform textRect = magicText.rectTransform;
                textRect.anchoredPosition = new Vector2(0f, -31f);
                textRect.sizeDelta = new Vector2(112f, 42f);
                magicText.fontSize = 13;
                magicText.alignment = TextAnchor.MiddleCenter;
                magicText.lineSpacing = .88f;
                magicText.resizeTextForBestFit = true;
                magicText.resizeTextMinSize = 9;
                magicText.resizeTextMaxSize = 14;
                compactMagicText = magicText;
                compactMagicText.text = BuildCompactMagicLabel();
            }
        }
    }

    static RectTransform EnsureActionPair(Transform hudRoot)
    {
        Transform existing = FindDescendant(hudRoot, "CombatActionPair");
        RectTransform pair = existing as RectTransform;
        if (pair == null)
        {
            GameObject pairObject = new GameObject("CombatActionPair", typeof(RectTransform));
            pairObject.transform.SetParent(hudRoot, false);
            pair = pairObject.GetComponent<RectTransform>();
        }

        pair.anchorMin = pair.anchorMax = pair.pivot = new Vector2(1f, 0f);
        pair.anchoredPosition = new Vector2(-18f, 18f);
        pair.sizeDelta = CornerActionPairSize;
        pair.localScale = Vector3.one;
        pair.SetAsLastSibling();
        return pair;
    }

    static void ConfigurePairChild(Transform target, Vector2 anchor, Vector2 pivot)
    {
        RectTransform rect = target as RectTransform;
        if (rect == null) return;

        rect.anchorMin = rect.anchorMax = anchor;
        rect.pivot = pivot;
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = CornerActionSize;
        rect.localScale = Vector3.one;
    }

    static string BuildCompactMagicLabel()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return GameLanguage.T("MAGIC", "МАГИЯ");

        if (gm.GameEnded)
            return GameLanguage.T("MAGIC\nENDED", "МАГИЯ\nБОЙ ОКОНЧЕН");

        if (gm.MagicCooldownRemaining > .01f)
        {
            int seconds = Mathf.CeilToInt(gm.MagicCooldownRemaining);
            return GameLanguage.T($"MAGIC\n{seconds}s", $"МАГИЯ\n{seconds}с");
        }

        if (EnemyRegistry.AliveCount <= 0)
            return GameLanguage.T("MAGIC\nWAIT", "МАГИЯ\nОЖИДАНИЕ");

        return GameLanguage.T("MAGIC\nREADY", "МАГИЯ\nГОТОВО");
    }

    void ApplyGuidanceCards(Transform root)
    {
        Transform objective = FindDescendant(root, "ChapterObjective");
        SetScale(objective, .70f);
        SetAnchoredPosition(objective, new Vector2(16f, -132f));

        Transform tutorial = FindDescendant(root, "ContextTutorial");
        SetScale(tutorial, .70f);
        SetAnchoredPosition(tutorial, new Vector2(16f, -214f));
    }

    void ApplyPatronCard(Transform root)
    {
        Transform card = FindDescendant(root, "PatronCommentaryCard");
        RectTransform rect = card as RectTransform;
        if (rect != null)
        {
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-16f, -16f);
        }
        SetScale(card, .70f);
        SetPanelAlpha(card, .93f);
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
