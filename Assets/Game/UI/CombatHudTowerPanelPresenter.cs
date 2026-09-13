using UnityEngine;
using UnityEngine.UI;

public static class CombatHudTowerPanelPresenter
{
    static string L(string en, string ru) => GameLanguage.T(en, ru);

    public static void RefreshBuildDock(
        TowerPlacement placement,
        EnemySpawner spawner,
        TowerType[] buildTypes,
        Button[] buildButtons,
        Text buildSelectionText)
    {
        if (placement == null || buildSelectionText == null) return;

        int cost = TowerFactory.GetCost(placement.SelectedBuildType);
        TowerType recommended = CombatHudRecommendationPolicy.Recommend(spawner, buildTypes);
        buildSelectionText.text = $"{L("SELECTED", "ВЫБРАНО")}: {CombatHudTowerCatalog.TowerName(placement.SelectedBuildType)}   •   {cost} {L("GOLD", "ЗОЛОТА")}   •   {L("RECOMMENDED", "РЕКОМЕНДАЦИЯ")}: {CombatHudTowerCatalog.TowerName(recommended)}";

        GameManager gm = GameManager.Instance;
        for (int i = 0; i < buildButtons.Length && i < buildTypes.Length; i++)
        {
            Button button = buildButtons[i];
            if (button == null) continue;

            Image image = button.GetComponent<Image>();
            if (image == null) continue;

            bool selected = buildTypes[i] == placement.SelectedBuildType;
            bool recommendedButton = buildTypes[i] == recommended;
            bool affordable = gm != null && gm.Money >= TowerFactory.GetCost(buildTypes[i]);

            if (selected) image.color = new Color(.58f, .11f, .045f, .98f);
            else if (recommendedButton && affordable) image.color = new Color(.48f, .30f, .07f, .98f);
            else if (recommendedButton) image.color = new Color(.29f, .20f, .08f, .92f);
            else image.color = affordable ? new Color(.24f, .14f, .08f, .96f) : new Color(.11f, .085f, .07f, .88f);
        }
    }

    public static void RefreshSelected(
        TowerPlacement placement,
        Text selectedTitle,
        Text selectedStats,
        Text selectedPriority,
        Text selectedUpgradePreview,
        Button upgradeButton,
        Button sellButton,
        Button priorityButton)
    {
        Tower tower = placement != null ? placement.SelectedTower : null;
        bool has = tower != null;

        upgradeButton.gameObject.SetActive(has);
        sellButton.gameObject.SetActive(has);
        priorityButton.gameObject.SetActive(has && tower.Type != TowerType.TrojanGuard);

        if (!has)
        {
            selectedTitle.text = L("SELECT A TOWER-UNIT", "ВЫБЕРИТЕ ОБОРОНУ");
            selectedStats.text = L(
                "Click a deployed Trojan defense to inspect stats, upgrade it, sell it, or change targeting priority.",
                "Нажмите на установленную оборону Трои, чтобы увидеть характеристики, улучшить, продать или изменить приоритет цели.");
            selectedUpgradePreview.text = "";
            selectedPriority.text = "";
            return;
        }

        selectedTitle.text = $"{tower.DisplayName}   LV {tower.Level}/3";
        selectedStats.text = $"{L("DAMAGE", "УРОН")}     {tower.damage:0}\n{L("RANGE", "ДАЛЬНОСТЬ")}       {tower.range:0.0}\n{L("ATTACK RATE", "СКОРОСТЬ АТАКИ")}   {tower.fireRate:0.00}/s\n{L("SELL VALUE", "ЦЕНА ПРОДАЖИ")}      {tower.SellValue}";

        if (tower.Level >= 3)
        {
            selectedUpgradePreview.text = L("MAXIMUM LEVEL REACHED", "ДОСТИГНУТ МАКСИМАЛЬНЫЙ УРОВЕНЬ");
        }
        else
        {
            float nextDamage = tower.damage * 1.32f;
            float nextRange = tower.range * 1.10f;
            float nextRate = tower.fireRate * 1.12f;
            selectedUpgradePreview.text = $"{L("NEXT UPGRADE", "СЛЕДУЮЩЕЕ УЛУЧШЕНИЕ")}   {tower.UpgradeCost} {L("GOLD", "ЗОЛОТА")}\n{tower.damage:0} → {nextDamage:0} DMG   •   {tower.range:0.0} → {nextRange:0.0} RNG   •   {tower.fireRate:0.00} → {nextRate:0.00}/s";
        }

        selectedPriority.text = tower.Type == TowerType.TrojanGuard
            ? L("ROLE: BLOCKING SQUAD", "РОЛЬ: БЛОКИРУЮЩИЙ ОТРЯД")
            : $"{L("TARGETING", "ПРИОРИТЕТ")}: {tower.Priority}";

        GameManager gm = GameManager.Instance;
        upgradeButton.interactable = tower.Level < 3 && gm != null && gm.Money >= tower.UpgradeCost;
    }
}
