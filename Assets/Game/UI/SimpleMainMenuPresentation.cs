using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class SimpleMainMenuPresentation : MonoBehaviour
{
    GameObject appliedRoot;
    GameObject armyOverlay;
    Text armyTitle;
    Text armyBody;
    GameMenuController controller;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoStart()
    {
        if (FindFirstObjectByType<SimpleMainMenuPresentation>() == null)
            new GameObject("SimpleMainMenuPresenter").AddComponent<SimpleMainMenuPresentation>();
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        appliedRoot = null;
        armyOverlay = null;
        armyTitle = null;
        armyBody = null;
        controller = null;
    }

    void Update()
    {
        if (armyOverlay != null && armyOverlay.activeSelf && GameInput.PausePressed())
            HideArmy();
    }

    void LateUpdate()
    {
        if (appliedRoot != null) return;

        controller = FindFirstObjectByType<GameMenuController>();
        GameObject canvasObject = GameObject.Find("MenuCanvas");
        if (controller == null || canvasObject == null) return;

        Transform mainMenu = canvasObject.transform.Find("MainMenu");
        if (mainMenu == null) return;
        Transform approved = mainMenu.Find("ApprovedMainMenu");
        if (approved == null) return;

        Build(approved);
        appliedRoot = approved.gameObject;
        RuntimeFileLogger.Event("MENU", "Simplified main menu applied: Play / Army / Settings / Exit");
    }

    void Build(Transform approved)
    {
        Transform old = approved.Find("SimpleMenuLayer");
        if (old != null) Destroy(old.gameObject);

        Button[] legacyButtons = approved.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < legacyButtons.Length; i++)
            legacyButtons[i].interactable = false;

        GameObject layer = new GameObject("SimpleMenuLayer");
        layer.transform.SetParent(approved, false);
        RectTransform layerRect = layer.AddComponent<RectTransform>();
        Stretch(layerRect);

        GameObject panel = MakePanel(layer.transform, "MainActions", new Vector2(455f, 0f), new Vector2(610f, 690f), new Color(.045f, .020f, .009f, .96f));
        AddText(panel.transform, "THE TROY GAME", new Vector2(0f, 260f), new Vector2(540f, 72f), 42, FontStyle.Bold, new Color(1f, .66f, .16f, 1f));
        AddText(panel.transform, "GODS DEFENSE", new Vector2(0f, 215f), new Vector2(500f, 42f), 19, FontStyle.Bold, new Color(.86f, .70f, .48f, 1f));

        MakeButton(panel.transform, L("PLAY", "ИГРАТЬ"), new Vector2(0f, 105f), new Vector2(470f, 82f), () => InvokeController("ShowLevels"), true);
        MakeButton(panel.transform, L("ARMY", "АРМИЯ"), new Vector2(0f, 5f), new Vector2(470f, 72f), ShowArmy, false);
        MakeButton(panel.transform, L("SETTINGS", "НАСТРОЙКИ"), new Vector2(0f, -88f), new Vector2(470f, 68f), () => InvokeController("ShowSettingsFromMain"), false);
        MakeButton(panel.transform, L("EXIT", "ВЫХОД"), new Vector2(0f, -178f), new Vector2(330f, 60f), () => InvokeController("QuitGame"), false);

        AddText(panel.transform,
            L("Campaign • Army • Settings", "Кампания • Армия • Настройки"),
            new Vector2(0f, -268f), new Vector2(500f, 34f), 14, FontStyle.Normal, new Color(.70f, .60f, .49f, .86f));

        BuildArmyOverlay(layer.transform);
    }

    void BuildArmyOverlay(Transform parent)
    {
        armyOverlay = new GameObject("ArmyOverlay");
        armyOverlay.transform.SetParent(parent, false);
        Image backdrop = armyOverlay.AddComponent<Image>();
        backdrop.color = new Color(.018f, .009f, .004f, .985f);
        Stretch(backdrop.rectTransform);

        GameObject panel = MakePanel(armyOverlay.transform, "ArmyCard", Vector2.zero, new Vector2(1180f, 760f), new Color(.065f, .030f, .013f, .99f));
        AddText(panel.transform, L("ARMY", "АРМИЯ"), new Vector2(0f, 305f), new Vector2(800f, 64f), 42, FontStyle.Bold, new Color(1f, .66f, .16f, 1f));
        AddText(panel.transform,
            L("Everything you command and everything you face — in one place.", "Всё, чем вы командуете, и всё, с чем сражаетесь — в одном месте."),
            new Vector2(0f, 258f), new Vector2(900f, 42f), 16, FontStyle.Normal, new Color(.83f, .72f, .60f, 1f));

        MakeButton(panel.transform, L("HECTOR", "ГЕКТОР"), new Vector2(-330f, 185f), new Vector2(280f, 58f), () => ShowArmyTab(0), true);
        MakeButton(panel.transform, L("DEFENDERS", "ЗАЩИТНИКИ"), new Vector2(0f, 185f), new Vector2(280f, 58f), () => ShowArmyTab(1), false);
        MakeButton(panel.transform, L("ENEMIES", "ВРАГИ"), new Vector2(330f, 185f), new Vector2(280f, 58f), () => ShowArmyTab(2), false);

        GameObject content = MakePanel(panel.transform, "ArmyContent", new Vector2(0f, -40f), new Vector2(980f, 360f), new Color(.035f, .020f, .012f, .96f));
        armyTitle = AddText(content.transform, "", new Vector2(-405f, 120f), new Vector2(760f, 50f), 28, FontStyle.Bold, new Color(1f, .75f, .28f, 1f), TextAnchor.MiddleLeft);
        armyBody = AddText(content.transform, "", new Vector2(-405f, -20f), new Vector2(810f, 215f), 18, FontStyle.Normal, new Color(.90f, .82f, .72f, 1f), TextAnchor.UpperLeft);

        MakeButton(panel.transform, L("BACK", "НАЗАД"), new Vector2(0f, -320f), new Vector2(260f, 54f), HideArmy, false);
        ShowArmyTab(0);
        armyOverlay.SetActive(false);
    }

    void ShowArmy()
    {
        if (armyOverlay == null) return;
        ShowArmyTab(0);
        armyOverlay.transform.SetAsLastSibling();
        armyOverlay.SetActive(true);
        RuntimeFileLogger.Event("MENU", "Opened Army screen");
    }

    void HideArmy()
    {
        if (armyOverlay == null) return;
        armyOverlay.SetActive(false);
        RuntimeFileLogger.Event("MENU", "Returned from Army screen");
    }

    void ShowArmyTab(int tab)
    {
        if (armyTitle == null || armyBody == null) return;
        switch (tab)
        {
            case 0:
                armyTitle.text = L("HECTOR • PRINCE OF TROY", "ГЕКТОР • ПРИНЦ ТРОИ");
                armyBody.text = L(
                    "Battlefield hero and mobile front-line commander.\n\nAbilities: Q War Cry • E Shield Wall • R Spear Throw • F Ultimate.\nHector moves along battlefield roads, fights enemies directly and revives after being downed.",
                    "Герой поля боя и мобильный командир передовой.\n\nСпособности: Q Боевой клич • E Стена щитов • R Бросок копья • F Ультимейт.\nГектор движется по дорогам поля боя, сражается с врагами напрямую и восстанавливается после падения.");
                break;
            case 1:
                armyTitle.text = L("DEFENDERS OF TROY", "ЗАЩИТНИКИ ТРОИ");
                armyBody.text = L(
                    "Trojan Guard • Archer Post • Spear Wall • Ballista • Priests of Apollo • Fire Keeper.\n\nThis screen is the single home for defensive-unit information. Detailed cards and final production art can be added here without creating more main-menu sections.",
                    "Троянская гвардия • Лучники • Стена копий • Баллиста • Жрецы Аполлона • Хранитель огня.\n\nЭто единый раздел информации о защитниках. Подробные карточки и финальный арт добавляются сюда без новых пунктов главного меню.");
                break;
            default:
                armyTitle.text = L("ENEMIES", "ВРАГИ");
                armyBody.text = L(
                    "Infantry • Runners • Heavy Hoplites • Shield Bearers • Archers • Battering Ram • Menelaus.\n\nEnemy records describe battlefield roles and threats. New enemies should extend this tab instead of creating a separate encyclopedia menu.",
                    "Пехота • Бегуны • Тяжёлые гоплиты • Щитоносцы • Лучники • Таран • Менелай.\n\nЗаписи о врагах объясняют их роль и угрозу. Новые противники добавляются в эту вкладку, а не в отдельное меню-энциклопедию.");
                break;
        }
    }

    void InvokeController(string methodName)
    {
        if (controller == null) controller = FindFirstObjectByType<GameMenuController>();
        if (controller == null) return;
        MethodInfo method = typeof(GameMenuController).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (method == null)
        {
            RuntimeFileLogger.Event("MENU", "Menu action not found: " + methodName);
            return;
        }
        method.Invoke(controller, null);
    }

    string L(string en, string ru) => GameLanguage.T(en, ru);

    GameObject MakePanel(Transform parent, string name, Vector2 position, Vector2 size, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = color;
        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(.90f, .46f, .11f, .65f);
        outline.effectDistance = new Vector2(2f, -2f);
        return go;
    }

    Button MakeButton(Transform parent, string label, Vector2 position, Vector2 size, UnityEngine.Events.UnityAction action, bool primary)
    {
        GameObject go = new GameObject(label + " Button");
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = primary ? new Color(.62f, .15f, .045f, .99f) : new Color(.20f, .105f, .050f, .98f);
        RectTransform rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = primary ? new Color(1f, .66f, .16f, .90f) : new Color(.58f, .32f, .13f, .70f);
        outline.effectDistance = new Vector2(2f, -2f);
        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);
        go.AddComponent<MenuButtonFeedback>();
        AddText(go.transform, label, Vector2.zero, size - new Vector2(18f, 10f), primary ? 26 : 22, FontStyle.Bold, new Color(1f, .88f, .69f, 1f));
        return button;
    }

    Text AddText(Transform parent, string value, Vector2 position, Vector2 size, int fontSize, FontStyle style, Color color, TextAnchor anchor = TextAnchor.MiddleCenter)
    {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(parent, false);
        Text text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.alignment = anchor;
        text.color = color;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.raycastTarget = false;
        RectTransform rect = text.rectTransform;
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        return text;
    }

    static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
