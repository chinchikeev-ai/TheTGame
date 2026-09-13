using UnityEngine;
using UnityEngine.UI;

public sealed class BuildDefenseInfoPresentation : MonoBehaviour
{
    Canvas canvas;
    CanvasGroup group;
    TowerPlacement placement;
    Text titleText;
    Text roleText;
    Text statsText;
    Text strongText;
    Text weakText;
    Text costText;
    Image accent;
    TowerType lastType;
    int lastMoney = int.MinValue;

    string L(string en, string ru) => GameLanguage.T(en, ru);

    void Start()
    {
        placement = FindFirstObjectByType<TowerPlacement>();
        Build();
        Refresh(true);
    }

    void Build()
    {
        GameObject root = new GameObject("BuildDefenseInfo");
        canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 81;

        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;

        group = root.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;
        group.interactable = false;

        GameObject card = MakeRect(root.transform, "DefenseInfoCard", new Vector2(0, 192), new Vector2(940, 132), new Color(.032f, .021f, .016f, .965f));
        RectTransform cardRt = card.GetComponent<RectTransform>();
        cardRt.anchorMin = cardRt.anchorMax = cardRt.pivot = new Vector2(.5f, 0f);

        Outline outline = card.AddComponent<Outline>();
        outline.effectColor = new Color(.64f, .34f, .12f, .58f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        GameObject accentObj = MakeRect(card.transform, "Accent", new Vector2(-466, 0), new Vector2(8, 118), new Color(.82f, .24f, .06f, 1f));
        accent = accentObj.GetComponent<Image>();

        titleText = AddText(card.transform, "", new Vector2(-438, 35), new Vector2(290, 38), 21, new Color(1f, .74f, .30f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        roleText = AddText(card.transform, "", new Vector2(-438, 1), new Vector2(290, 34), 14, new Color(.78f, .69f, .60f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        costText = AddText(card.transform, "", new Vector2(-438, -35), new Vector2(290, 34), 16, new Color(1f, .82f, .42f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);

        statsText = AddText(card.transform, "", new Vector2(-105, 20), new Vector2(300, 78), 15, new Color(.94f, .88f, .79f, 1f), TextAnchor.MiddleLeft, FontStyle.Normal);
        strongText = AddText(card.transform, "", new Vector2(215, 28), new Vector2(270, 52), 14, new Color(.45f, .95f, .54f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
        weakText = AddText(card.transform, "", new Vector2(215, -30), new Vector2(270, 52), 14, new Color(1f, .48f, .37f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
    }

    void Update()
    {
        if (placement == null) placement = FindFirstObjectByType<TowerPlacement>();
        if (placement == null || GameManager.Instance == null)
        {
            if (group != null) group.alpha = 0f;
            return;
        }

        bool hide = GameManager.Instance.GameEnded || IsMenuBlockingCombat();
        group.alpha = hide ? 0f : 1f;
        if (hide) return;

        Refresh(false);
    }

    void Refresh(bool force)
    {
        if (placement == null || GameManager.Instance == null) return;
        TowerType type = placement.SelectedBuildType;
        int money = GameManager.Instance.Money;
        if (!force && type == lastType && money == lastMoney) return;
        lastType = type;
        lastMoney = money;

        TowerData data = BalanceCatalog.GetTower(type);
        if (data == null) return;

        titleText.text = TowerName(type).ToUpperInvariant();
        roleText.text = Role(type);

        bool affordable = money >= data.cost;
        costText.text = affordable
            ? $"{L("COST", "ЦЕНА")}: {data.cost}   •   {L("READY TO BUILD", "МОЖНО СТРОИТЬ")}"
            : $"{L("COST", "ЦЕНА")}: {data.cost}   •   {L("NEED", "НУЖНО")} {data.cost - money} {L("MORE GOLD", "ЗОЛОТА")}";
        costText.color = affordable ? new Color(.48f, .96f, .52f, 1f) : new Color(1f, .46f, .34f, 1f);
        accent.color = affordable ? new Color(.24f, .76f, .29f, 1f) : new Color(.82f, .18f, .08f, 1f);

        float dps = data.damage * Mathf.Max(.01f, data.attacksPerSecond);
        string special = data.splashRadius > 0f
            ? $"\n{L("Splash", "Радиус взрыва")}: {data.splashRadius:0.0}"
            : data.slowMultiplier < .999f
                ? $"\n{L("Slow", "Замедление")}: {Mathf.RoundToInt((1f - data.slowMultiplier) * 100f)}%"
                : "";

        statsText.text =
            $"{L("Damage", "Урон")}: {data.damage:0.#}   •   DPS: {dps:0.#}\n" +
            $"{L("Range", "Дальность")}: {data.range:0.0}   •   {L("Rate", "Темп")}: {data.attacksPerSecond:0.00}/s" + special;

        strongText.text = $"+ {L("STRONG VS", "СИЛЁН ПРОТИВ")}\n{StrongAgainst(type)}";
        weakText.text = $"− {L("WEAK VS", "СЛАБ ПРОТИВ")}\n{WeakAgainst(type)}";
    }

    string TowerName(TowerType type)
    {
        switch (type)
        {
            case TowerType.MachineGun: return L("Trojan Archers", "Троянские лучники");
            case TowerType.SpearThrower: return L("Spear Throwers", "Метатели копий");
            case TowerType.Cannon: return L("Ballista Crew", "Расчёт баллисты");
            case TowerType.Slow: return L("Priests of Apollo", "Жрецы Аполлона");
            case TowerType.FireTower: return L("Fire Crew", "Огненный расчёт");
            case TowerType.TrojanGuard: return L("Shield Guard", "Щитовая гвардия");
            default: return TowerFactory.GetDisplayName(type);
        }
    }

    string Role(TowerType type)
    {
        switch (type)
        {
            case TowerType.MachineGun: return L("RANGED DPS • ANTI-LIGHT", "ДАЛЬНИЙ БОЙ • ПРОТИВ ЛЁГКИХ");
            case TowerType.SpearThrower: return L("ARMOR PIERCE • ANTI-HEAVY", "БРОНЕБОЙНЫЙ • ПРОТИВ ТЯЖЁЛЫХ");
            case TowerType.Cannon: return L("HEAVY SINGLE TARGET • ANTI-SIEGE", "ТЯЖЁЛЫЙ УРОН • ПРОТИВ ОСАДЫ");
            case TowerType.Slow: return L("SUPPORT • CONTROL", "ПОДДЕРЖКА • КОНТРОЛЬ");
            case TowerType.FireTower: return L("AOE • BURN • AREA DENIAL", "AOE • ГОРЕНИЕ • КОНТРОЛЬ ЗОНЫ");
            case TowerType.TrojanGuard: return L("BLOCKER • FRONTLINE", "БЛОКИРОВКА • ПЕРЕДОВАЯ");
            default: return "";
        }
    }

    string StrongAgainst(TowerType type)
    {
        switch (type)
        {
            case TowerType.MachineGun: return L("Runners • Infantry", "Бегуны • Пехота");
            case TowerType.SpearThrower: return L("Heavy Hoplites • Shields", "Тяжёлые гоплиты • Щитоносцы");
            case TowerType.Cannon: return L("Bosses • Siege • Heavy", "Боссы • Осада • Тяжёлые");
            case TowerType.Slow: return L("Fast groups • Chokepoints", "Быстрые группы • Узкие места");
            case TowerType.FireTower: return L("Dense groups • Chokepoints", "Плотные группы • Узкие места");
            case TowerType.TrojanGuard: return L("Holding lanes • Protecting fire zones", "Удержание линии • Огненные зоны");
            default: return "—";
        }
    }

    string WeakAgainst(TowerType type)
    {
        switch (type)
        {
            case TowerType.MachineGun: return L("Heavy armor • Siege", "Тяжёлая броня • Осада");
            case TowerType.SpearThrower: return L("Large light swarms", "Большие толпы лёгких");
            case TowerType.Cannon: return L("Fast swarms", "Быстрые толпы");
            case TowerType.Slow: return L("Direct damage races", "Бой на чистый урон");
            case TowerType.FireTower: return L("Single armored targets", "Одиночные бронированные цели");
            case TowerType.TrojanGuard: return L("Ranged pressure • No DPS", "Дальний обстрел • Мало урона");
            default: return "—";
        }
    }

    bool IsMenuBlockingCombat()
    {
        GameObject menu = GameObject.Find("MenuCanvas");
        if (menu == null) return false;
        string[] names = { "MainMenu", "LevelSelect", "Settings", "PauseMenu", "EndMenu", "ConfirmationModal" };
        for (int i = 0; i < names.Length; i++)
        {
            Transform t = menu.transform.Find(names[i]);
            if (t != null && t.gameObject.activeInHierarchy) return true;
        }
        return false;
    }

    GameObject MakeRect(Transform parent, string name, Vector2 pos, Vector2 size, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image image = go.AddComponent<Image>();
        image.color = color;
        RectTransform rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return go;
    }

    Text AddText(Transform parent, string value, Vector2 pos, Vector2 size, int fontSize, Color color, TextAnchor alignment, FontStyle style)
    {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(parent, false);
        Text text = go.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.fontStyle = style;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform rt = text.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(.5f, .5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return text;
    }
}
