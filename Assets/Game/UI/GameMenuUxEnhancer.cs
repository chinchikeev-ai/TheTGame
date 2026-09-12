using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public sealed class GameMenuUxEnhancer : MonoBehaviour
{
    GameMenuController menu;
    Canvas canvas;
    GameObject modal;
    Text modalTitle;
    Text modalBody;
    Button confirmButton;
    Button cancelButton;
    Action pendingAction;

    string L(string en, string ru) => GameLanguage.T(en, ru);

    IEnumerator Start()
    {
        yield return null;
        menu = FindFirstObjectByType<GameMenuController>();
        canvas = FindFirstObjectByType<Canvas>();
        if (menu == null || canvas == null) yield break;

        EnsureEventSystem();
        EnhanceScreens();
        EnhanceButtons();
        BuildConfirmationModal();
        ConfigureCampaignFlow();
        SelectFirstAvailableButton();
    }

    void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null) return;
        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        InputSystemUIInputModule inputModule = eventSystemObject.AddComponent<InputSystemUIInputModule>();
        inputModule.AssignDefaultActions();
    }

    void EnhanceScreens()
    {
        string[] names = { "MainMenu", "LevelSelect", "Settings", "PauseMenu", "EndMenu" };
        foreach (string screenName in names)
        {
            Transform screen = canvas.transform.Find(screenName);
            if (screen == null) continue;
            if (screen.GetComponent<MenuScreenTransition>() == null)
                screen.gameObject.AddComponent<MenuScreenTransition>();
        }
    }

    void EnhanceButtons()
    {
        foreach (Button button in canvas.GetComponentsInChildren<Button>(true))
        {
            if (button.GetComponent<MenuButtonFeedback>() == null)
                button.gameObject.AddComponent<MenuButtonFeedback>();
            Navigation navigation = button.navigation;
            navigation.mode = Navigation.Mode.Automatic;
            button.navigation = navigation;
        }
    }

    void ConfigureCampaignFlow()
    {
        Button continueButton = FindButton("CONTINUE", "ПРОДОЛЖИТЬ");
        if (continueButton != null)
        {
            bool canContinue = HasCampaignProgress();
            continueButton.interactable = canContinue;
            Text label = continueButton.GetComponentInChildren<Text>();
            if (label != null && !canContinue)
                label.text = L("CONTINUE  •  NO SAVE", "ПРОДОЛЖИТЬ  •  НЕТ СОХРАНЕНИЯ");
        }

        Button newCampaignButton = FindButton("NEW CAMPAIGN", "НОВАЯ КАМПАНИЯ");
        if (newCampaignButton != null)
        {
            newCampaignButton.onClick.RemoveAllListeners();
            newCampaignButton.onClick.AddListener(RequestNewCampaign);
        }

        ReplaceWithConfirmation(FindButton("RESTART CHAPTER", "ПЕРЕЗАПУСТИТЬ ГЛАВУ"),
            L("RESTART CHAPTER?", "ПЕРЕЗАПУСТИТЬ ГЛАВУ?"),
            L("Current battle progress will be lost.", "Текущий прогресс битвы будет потерян."), "RestartScene");

        ReplaceWithConfirmation(FindButton("MAIN MENU", "ГЛАВНОЕ МЕНЮ"),
            L("RETURN TO MAIN MENU?", "ВЕРНУТЬСЯ В ГЛАВНОЕ МЕНЮ?"),
            L("The current battle will end. Campaign progress already saved will remain.", "Текущая битва завершится. Уже сохранённый прогресс кампании останется."), "ReturnToMainMenu");

        ReplaceWithConfirmation(FindButton("EXIT", "ВЫХОД"),
            L("EXIT GAME?", "ВЫЙТИ ИЗ ИГРЫ?"),
            L("Unsaved battle progress will be lost.", "Несохранённый прогресс битвы будет потерян."), "QuitGame");
    }

    bool HasCampaignProgress()
    {
        return CampaignController.Instance != null && CampaignController.Instance.HasProgress;
    }

    void RequestNewCampaign()
    {
        if (!HasCampaignProgress())
        {
            StartNewCampaign();
            return;
        }
        ShowConfirmation(L("START A NEW CAMPAIGN?", "НАЧАТЬ НОВУЮ КАМПАНИЮ?"),
            L("Existing campaign progress will be erased. Settings will be kept.", "Текущий прогресс кампании будет удалён. Настройки игры сохранятся."),
            StartNewCampaign);
    }

    void StartNewCampaign()
    {
        CampaignController.Instance?.ResetProgress();
        RuntimeFileLogger.Event("CAMPAIGN", "New campaign started from main menu");
        menu.SendMessage("ShowLevels", SendMessageOptions.DontRequireReceiver);
    }

    void ReplaceWithConfirmation(Button button, string title, string body, string menuMethod)
    {
        if (button == null) return;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => ShowConfirmation(title, body,
            () => menu.SendMessage(menuMethod, SendMessageOptions.DontRequireReceiver)));
    }

    Button FindButton(params string[] labels)
    {
        foreach (Button button in canvas.GetComponentsInChildren<Button>(true))
        {
            Text text = button.GetComponentInChildren<Text>(true);
            if (text == null) continue;
            foreach (string label in labels)
                if (string.Equals(text.text, label, StringComparison.OrdinalIgnoreCase)) return button;
        }
        return null;
    }

    void BuildConfirmationModal()
    {
        modal = new GameObject("ConfirmationModal");
        modal.transform.SetParent(canvas.transform, false);
        Image backdrop = modal.AddComponent<Image>();
        backdrop.color = new Color(.01f, .006f, .004f, .78f);
        RectTransform modalRect = backdrop.rectTransform;
        modalRect.anchorMin = Vector2.zero;
        modalRect.anchorMax = Vector2.one;
        modalRect.offsetMin = modalRect.offsetMax = Vector2.zero;

        GameObject card = new GameObject("ConfirmationCard");
        card.transform.SetParent(modal.transform, false);
        Image cardImage = card.AddComponent<Image>();
        cardImage.color = new Color(.075f, .04f, .024f, .99f);
        RectTransform cardRect = cardImage.rectTransform;
        cardRect.anchorMin = cardRect.anchorMax = cardRect.pivot = new Vector2(.5f, .5f);
        cardRect.sizeDelta = new Vector2(700f, 390f);
        Outline outline = card.AddComponent<Outline>();
        outline.effectColor = new Color(.85f, .46f, .17f, .65f);
        outline.effectDistance = new Vector2(2f, -2f);

        modalTitle = MakeText(card.transform, "Title", new Vector2(0, 115), new Vector2(610, 70), 34, new Color(1f, .66f, .24f, 1f), FontStyle.Bold);
        modalBody = MakeText(card.transform, "Body", new Vector2(0, 25), new Vector2(570, 100), 20, new Color(.94f, .86f, .75f, 1f), FontStyle.Normal);
        confirmButton = MakeModalButton(card.transform, L("CONFIRM", "ПОДТВЕРДИТЬ"), new Vector2(-155, -115), true, ConfirmPending);
        cancelButton = MakeModalButton(card.transform, L("CANCEL", "ОТМЕНА"), new Vector2(155, -115), false, CancelPending);
        modal.AddComponent<MenuScreenTransition>();
        modal.SetActive(false);
    }

    Text MakeText(Transform parent, string name, Vector2 position, Vector2 size, int fontSize, Color color, FontStyle fontStyle)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Text text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        RectTransform rect = text.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return text;
    }

    Button MakeModalButton(Transform parent, string label, Vector2 position, bool primary, UnityEngine.Events.UnityAction action)
    {
        GameObject go = new GameObject(label);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = primary ? new Color(.57f, .105f, .05f, 1f) : new Color(.28f, .18f, .11f, 1f);
        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);
        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(260f, 62f);
        Text text = MakeText(go.transform, "Label", Vector2.zero, new Vector2(250f, 58f), 20,
            primary ? new Color(1f, .9f, .55f, 1f) : new Color(.95f, .82f, .68f, 1f), FontStyle.Bold);
        text.rectTransform.anchorMin = Vector2.zero;
        text.rectTransform.anchorMax = Vector2.one;
        text.rectTransform.offsetMin = text.rectTransform.offsetMax = Vector2.zero;
        go.AddComponent<MenuButtonFeedback>();
        Navigation navigation = button.navigation;
        navigation.mode = Navigation.Mode.Automatic;
        button.navigation = navigation;
        return button;
    }

    void ShowConfirmation(string title, string body, Action action)
    {
        pendingAction = action;
        modalTitle.text = title;
        modalBody.text = body;
        confirmButton.GetComponentInChildren<Text>().text = L("CONFIRM", "ПОДТВЕРДИТЬ");
        cancelButton.GetComponentInChildren<Text>().text = L("CANCEL", "ОТМЕНА");
        modal.GetComponent<MenuScreenTransition>().Show();
        EventSystem.current?.SetSelectedGameObject(cancelButton.gameObject);
    }

    void ConfirmPending()
    {
        Action action = pendingAction;
        pendingAction = null;
        modal.GetComponent<MenuScreenTransition>().Hide();
        action?.Invoke();
    }

    void CancelPending()
    {
        pendingAction = null;
        modal.GetComponent<MenuScreenTransition>().Hide();
        SelectFirstAvailableButton();
    }

    void SelectFirstAvailableButton()
    {
        if (EventSystem.current == null || canvas == null) return;
        foreach (Button button in canvas.GetComponentsInChildren<Button>(true))
        {
            if (!button.gameObject.activeInHierarchy || !button.interactable) continue;
            EventSystem.current.SetSelectedGameObject(button.gameObject);
            return;
        }
    }
}
