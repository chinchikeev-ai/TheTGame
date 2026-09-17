using UnityEngine;
using UnityEngine.UI;

// Final presentation-only compacting pass for the real Chapter I gameplay frame.
// It intentionally does not own UI content or interaction; it only reduces the
// visual footprint of existing panels after their normal owners have built them.
public sealed class ChapterOneUiCompactPresentation : MonoBehaviour
{
    const float HectorScale = .75f;
    static readonly Vector2 CornerActionSize = new Vector2(128f, 112f);

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
        ApplyHectorHud();
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
        SetScale(wave, .78f);
        SetAnchoredPosition(wave, new Vector2(0f, -12f));

        Transform prep = FindDescendant(hud.transform, "FirstWavePreparation");
        SetScale(prep, .66f);
        SetAnchoredPosition(prep, new Vector2(0f, -142f));
        SetPanelAlpha(prep, .90f);

        Transform buildDock = FindDescendant(hud.transform, "BuildDock");
        SetScale(buildDock, .78f);

        Transform selectedCard = FindDescendant(hud.transform, "SelectedTowerCard");
        SetScale(selectedCard, .80f);

        Transform tooltip = FindDescendant(hud.transform, "BuildHoverTooltip");
        SetScale(tooltip, .80f);

        ApplyBottomRightActions(hud.transform);
    }

    void ApplyHectorHud()
    {
        GameObject hector = GameObject.Find("HectorHUD");
        if (hector == null) return;

        Transform panel = FindDescendant(hector.transform, "HectorPanel");
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
        Transform magicButton = FindDescendant(hudRoot, "Magic_Primary");

        ConfigureCornerAction(defenders, new Vector2(-18f, 18f));
        ConfigureCornerAction(magicPanel, new Vector2(-158f, 18f));

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
                magicText.lineSpacing = .88f;
                magicText.resizeTextForBestFit = true;
                magicText.resizeTextMinSize = 9;
                magicText.resizeTextMaxSize = 14;
                magicText.text = BuildCompactMagicLabel();
            }
        }
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

    static void ConfigureCornerAction(Transform target, Vector2 position)
    {
        RectTransform rect = target as RectTransform;
        if (rect == null) return;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(1f, 0f);
        rect.anchoredPosition = position;
        rect.sizeDelta = CornerActionSize;
        rect.localScale = Vector3.one;
    }

    void ApplyGuidanceCards()
    {
        GameObject root = GameObject.Find("ChapterOneGuidanceUI");
        if (root == null) return;

        Transform objective = FindDescendant(root.transform, "ChapterObjective");
        SetScale(objective, .70f);
        SetAnchoredPosition(objective, new Vector2(16f, -132f));

        Transform tutorial = FindDescendant(root.transform, "ContextTutorial");
        SetScale(tutorial, .70f);
        SetAnchoredPosition(tutorial, new Vector2(16f, -214f));
    }

    void ApplyPatronCard()
    {
        GameObject root = GameObject.Find("PatronCommentaryUI");
        if (root == null) return;

        Transform card = FindDescendant(root.transform, "PatronCommentaryCard");
        RectTransform rect = card as RectTransform;
        if (rect != null)
        {
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-16f, -16f);
        }
        SetScale(card, .70f);
        SetPanelAlpha(card, .93f);
    }

    void ApplyCombatActions()
    {
        GameObject actions = GameObject.Find("CombatActions");
        if (actions == null) return;

        SetScale(actions.transform, .74f);
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