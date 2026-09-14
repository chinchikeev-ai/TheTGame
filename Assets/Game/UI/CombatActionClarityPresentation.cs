using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class CombatActionClarityPresentation : MonoBehaviour
{
    Button magicButton;
    Text magicText;
    Button giftButton;
    GameObject giftOverlay;
    bool bound;

    IEnumerator Start()
    {
        while (!bound)
        {
            bound = TryBind();
            if (!bound) yield return null;
        }
    }

    void LateUpdate()
    {
        if (!bound)
        {
            bound = TryBind();
            return;
        }

        if (giftButton != null && giftButton.gameObject.activeSelf)
            giftButton.gameObject.SetActive(false);
        if (giftOverlay != null && giftOverlay.activeSelf)
            giftOverlay.SetActive(false);

        RefreshMagic();
    }

    bool TryBind()
    {
        GameObject actions = GameObject.Find("CombatActions");
        if (actions == null) return false;
        Button[] buttons = actions.GetComponentsInChildren<Button>(true);
        if (buttons.Length == 0) return false;

        magicButton = buttons[0];
        magicText = magicButton.GetComponentInChildren<Text>(true);
        giftButton = buttons.Length > 1 ? buttons[1] : null;
        giftOverlay = GameObject.Find("DivineGiftChoiceOverlay");

        RectTransform panel = actions.transform as RectTransform;
        if (panel != null) panel.sizeDelta = new Vector2(370f, 132f);
        RectTransform magicRect = magicButton.transform as RectTransform;
        if (magicRect != null)
        {
            magicRect.anchoredPosition = Vector2.zero;
            magicRect.sizeDelta = new Vector2(330f, 94f);
        }
        if (magicText != null)
        {
            magicText.fontSize = 14;
            magicText.lineSpacing = 1.1f;
        }

        if (giftButton != null) giftButton.gameObject.SetActive(false);
        if (giftOverlay != null) giftOverlay.SetActive(false);
        RefreshMagic();
        return magicButton != null && magicText != null;
    }

    void RefreshMagic()
    {
        if (magicButton == null || magicText == null) return;
        GameManager gm = GameManager.Instance;
        if (gm == null)
        {
            magicButton.interactable = false;
            return;
        }

        float cooldown = gm.MagicCooldownRemaining;
        bool enemiesPresent = EnemyRegistry.AliveCount > 0;
        bool ready = cooldown <= .01f && enemiesPresent && !gm.GameEnded;
        magicButton.interactable = ready;

        string state;
        if (gm.GameEnded)
            state = GameLanguage.T("BATTLE ENDED", "БИТВА ЗАВЕРШЕНА");
        else if (cooldown > .01f)
            state = GameLanguage.T($"READY IN {Mathf.CeilToInt(cooldown)}s", $"ГОТОВО ЧЕРЕЗ {Mathf.CeilToInt(cooldown)}с");
        else if (!enemiesPresent)
            state = GameLanguage.T("WAITING FOR ENEMIES", "ЖДЁМ ПРОТИВНИКОВ");
        else
            state = GameLanguage.T("READY — CLICK TO CAST", "ГОТОВО — НАЖМИТЕ");

        magicText.text = GameLanguage.T(
            $"DIVINE STORM • ALL ENEMIES\n120 DMG + 50% SLOW FOR 5s\n{state}",
            $"БОЖЕСТВЕННАЯ БУРЯ • ВСЕ ВРАГИ\n120 УРОНА + ЗАМЕДЛЕНИЕ 50% НА 5с\n{state}");
    }
}
