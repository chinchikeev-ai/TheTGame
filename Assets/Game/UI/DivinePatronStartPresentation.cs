using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public sealed class DivinePatronStartPresentation : MonoBehaviour
{
    static DivinePatronStartPresentation instance;

    GameObject overlay;
    Button giftButton;
    Text giftButtonText;
    Button firstWaveButton;
    Text firstWaveButtonText;
    bool copyUpdated;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        if (instance != null) return;
        GameObject host = new GameObject("DivinePatronStartPresentation");
        DontDestroyOnLoad(host);
        instance = host.AddComponent<DivinePatronStartPresentation>();
    }

    IEnumerator Start()
    {
        while (true)
        {
            if (TryBind()) yield break;
            yield return null;
        }
    }

    bool TryBind()
    {
        GameObject overlayObject = GameObject.Find("DivineGiftChoiceOverlay");
        GameObject actionsObject = GameObject.Find("CombatActions");
        GameObject prepObject = GameObject.Find("FirstWavePreparation");
        if (overlayObject == null || actionsObject == null || prepObject == null) return false;

        overlay = overlayObject;
        Button[] actionButtons = actionsObject.GetComponentsInChildren<Button>(true);
        giftButton = actionButtons.FirstOrDefault(b => b != null && b.GetComponentInChildren<Text>(true)?.text.Contains(GameLanguage.Russian ? "ДАР" : "GIFT") == true);
        if (giftButton == null && actionButtons.Length > 1) giftButton = actionButtons[actionButtons.Length - 1];
        giftButtonText = giftButton != null ? giftButton.GetComponentInChildren<Text>(true) : null;

        firstWaveButton = prepObject.GetComponentInChildren<Button>(true);
        firstWaveButtonText = firstWaveButton != null ? firstWaveButton.GetComponentInChildren<Text>(true) : null;
        UpdateChoiceCopy();
        return overlay != null && giftButton != null && firstWaveButton != null;
    }

    void LateUpdate()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || overlay == null || giftButton == null || firstWaveButton == null) return;

        if (!copyUpdated) UpdateChoiceCopy();

        bool choosing = !gm.GameEnded && gm.CurrentWave == 0 && !gm.GiftSelected;
        if (choosing)
        {
            overlay.SetActive(true);
            giftButton.interactable = true;
            if (giftButtonText != null)
                giftButtonText.text = GameLanguage.T("CHOOSE PATRON GOD", "ВЫБРАТЬ БОГА-ПОКРОВИТЕЛЯ");
        }
        else if (gm.GiftSelected)
        {
            overlay.SetActive(false);
            giftButton.interactable = false;
            if (giftButtonText != null)
                giftButtonText.text = GameLanguage.T("PATRON: ", "ПОКРОВИТЕЛЬ: ") + PatronName(gm.SelectedGift);
        }

        EnemySpawner spawner = FindSpawnerFromButton();
        bool firstPreparation = spawner != null && gm.CurrentWave == 0 && !spawner.WaveActive;
        if (firstPreparation)
        {
            firstWaveButton.interactable = false;
            if (firstWaveButtonText != null)
            {
                int seconds = Mathf.Max(0, Mathf.CeilToInt(spawner.InterWaveCountdown));
                firstWaveButtonText.text = gm.GiftSelected
                    ? GameLanguage.T($"ATTACK IN {seconds}s", $"АТАКА ЧЕРЕЗ {seconds}с")
                    : GameLanguage.T("CHOOSE A PATRON FIRST", "СНАЧАЛА ВЫБЕРИТЕ БОГА");
            }
        }
    }

    EnemySpawner FindSpawnerFromButton()
    {
        ModernCombatHud hud = firstWaveButton != null ? firstWaveButton.GetComponentInParent<ModernCombatHud>() : null;
        if (hud != null) return hud.GetComponent<EnemySpawner>();
        return FindFirstObjectByType<EnemySpawner>();
    }

    void UpdateChoiceCopy()
    {
        if (overlay == null) return;
        Text[] texts = overlay.GetComponentsInChildren<Text>(true);
        foreach (Text text in texts)
        {
            if (text == null) continue;
            if (text.text.Contains("One blessing") || text.text.Contains("В каждой волне"))
            {
                text.text = GameLanguage.T(
                    "Choose one patron before battle. The blessing stays active for the whole map.",
                    "Выберите одного бога перед боем. Его дар действует всю карту.");
            }
        }

        Button[] buttons = overlay.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            if (button == null) continue;
            if (button.name == "GiftChoiceCancel") button.gameObject.SetActive(false);
        }
        copyUpdated = true;
    }

    static string PatronName(DivineGiftType gift)
    {
        switch (gift)
        {
            case DivineGiftType.Ares: return GameLanguage.T("ARES", "АРЕС");
            case DivineGiftType.Athena: return GameLanguage.T("ATHENA", "АФИНА");
            case DivineGiftType.Apollo: return GameLanguage.T("APOLLO", "АПОЛЛОН");
            default: return GameLanguage.T("POSEIDON", "ПОСЕЙДОН");
        }
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}
