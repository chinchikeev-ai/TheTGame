using UnityEngine;
using UnityEngine.UI;
using static CombatHudUiFactory;

public sealed class CombatCornerControlsPresentation : MonoBehaviour
{
    GameObject modernHud;
    GameObject magicFlyout;
    Button magicToggle;
    Button magicAction;
    Text magicToggleText;
    Text magicActionText;
    bool bound;

    string L(string en, string ru) => GameLanguage.T(en, ru);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<CombatCornerControlsPresentation>() == null)
            new GameObject("CombatCornerControlsPresentation").AddComponent<CombatCornerControlsPresentation>();
    }

    void Update()
    {
        if (!bound)
        {
            Bind();
            if (!bound) return;
        }

        RefreshMagic();
    }

    void Bind()
    {
        modernHud = GameObject.Find("ModernCombatHUD");
        if (modernHud == null) return;

        Transform root = modernHud.transform;
        RepositionDefenseDock(root);
        SimplifyTopRightActions(root);
        BuildMagicControl(root);
        bound = true;
    }

    void RepositionDefenseDock(Transform root)
    {
        Transform dock = root.Find("BuildDock");
        if (dock != null)
        {
            RectTransform rt = dock as RectTransform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1f, 0f);
            rt.anchoredPosition = new Vector2(-104f, 24f);
            rt.sizeDelta = new Vector2(1040f, 148f);
        }

        Transform tooltip = root.Find("BuildHoverTooltip");
        if (tooltip != null)
        {
            RectTransform rt = tooltip as RectTransform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1f, 0f);
            rt.anchoredPosition = new Vector2(-104f, 190f);
        }
    }

    void SimplifyTopRightActions(Transform root)
    {
        Transform actions = root.Find("CombatActions");
        if (actions == null) return;

        Button[] buttons = actions.GetComponentsInChildren<Button>(true);
        if (buttons.Length > 0)
            buttons[0].gameObject.SetActive(false);

        if (buttons.Length > 1)
        {
            RectTransform giftRect = buttons[1].transform as RectTransform;
            giftRect.anchoredPosition = Vector2.zero;
            giftRect.sizeDelta = new Vector2(282f, 40f);
        }

        RectTransform panelRect = actions as RectTransform;
        panelRect.sizeDelta = new Vector2(330f, 66f);
    }

    void BuildMagicControl(Transform root)
    {
        Transform existing = root.Find("MagicCornerControls");
        if (existing != null)
            Destroy(existing.gameObject);

        GameObject container = new GameObject("MagicCornerControls", typeof(RectTransform));
        container.transform.SetParent(root, false);
        RectTransform containerRect = container.GetComponent<RectTransform>();
        containerRect.anchorMin = Vector2.zero;
        containerRect.anchorMax = Vector2.one;
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;

        magicToggle = Button(container.transform, L("MAGIC", "МАГИЯ"), Vector2.zero, new Vector2(64, 64), ToggleMagicFlyout, true);
        magicToggle.gameObject.name = "MagicToggle";
        RectTransform toggleRect = magicToggle.transform as RectTransform;
        toggleRect.anchorMin = toggleRect.anchorMax = toggleRect.pivot = new Vector2(1f, 0f);
        toggleRect.anchoredPosition = new Vector2(-24f, 104f);
        toggleRect.sizeDelta = new Vector2(64f, 64f);
        magicToggleText = magicToggle.GetComponentInChildren<Text>();
        if (magicToggleText != null)
        {
            magicToggleText.fontSize = 10;
            magicToggleText.resizeTextForBestFit = true;
            magicToggleText.resizeTextMinSize = 8;
            magicToggleText.resizeTextMaxSize = 11;
        }

        magicFlyout = Panel(
            container.transform,
            "MagicFlyout",
            new Vector2(-24f, 184f),
            new Vector2(300f, 84f),
            new Color(.035f, .022f, .016f, .97f),
            new Vector2(1f, 0f),
            new Vector2(1f, 0f));

        magicAction = Button(magicFlyout.transform, "", Vector2.zero, new Vector2(268f, 54f), CastMagic, true);
        magicAction.gameObject.name = "Magic_Primary";
        magicActionText = magicAction.GetComponentInChildren<Text>();
        if (magicActionText != null) magicActionText.fontSize = 13;
        magicFlyout.SetActive(false);
    }

    void ToggleMagicFlyout()
    {
        if (magicFlyout == null) return;
        magicFlyout.SetActive(!magicFlyout.activeSelf);
    }

    void CastMagic()
    {
        GameManager gm = GameManager.Instance;
        if (gm != null && gm.UseMagic() && magicFlyout != null)
            magicFlyout.SetActive(false);
    }

    void RefreshMagic()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null)
        {
            if (magicFlyout != null) magicFlyout.SetActive(false);
            return;
        }

        float cooldown = gm.MagicCooldownRemaining;
        bool ready = cooldown <= .01f && !gm.GameEnded && EnemyRegistry.AliveCount > 0;

        if (magicToggleText != null)
        {
            magicToggleText.text = cooldown > .01f
                ? Mathf.CeilToInt(cooldown) + L("s", "с")
                : L("MAGIC", "МАГИЯ");
        }

        if (magicActionText != null)
        {
            magicActionText.text = cooldown > .01f
                ? L("DIVINE POWER   ", "БОЖЕСТВЕННАЯ СИЛА   ") + Mathf.CeilToInt(cooldown) + L("s", "с")
                : L("DIVINE POWER   READY", "БОЖЕСТВЕННАЯ СИЛА   ГОТОВА");
        }

        if (magicAction != null) magicAction.interactable = ready;
        if (magicToggle != null) magicToggle.interactable = !gm.GameEnded;
    }
}
