public static class CombatHudSelectedTowerFormatter
{
    static string L(string en, string ru) => GameLanguage.T(en, ru);

    public static string Title(Tower tower) => tower == null
        ? L("SELECT A TOWER-UNIT", "ВЫБЕРИТЕ ОБОРОНУ")
        : $"{tower.DisplayName}   LV {tower.Level}/3";

    public static string Stats(Tower tower)
    {
        if (tower == null)
            return L(
                "Click a deployed Trojan defense to inspect stats, upgrade it, sell it, or change targeting priority.",
                "Нажмите на установленную оборону Трои, чтобы увидеть характеристики, улучшить, продать или изменить приоритет цели.");

        return $"{L("DAMAGE", "УРОН")}     {tower.damage:0}\n{L("RANGE", "ДАЛЬНОСТЬ")}       {tower.range:0.0}\n{L("ATTACK RATE", "СКОРОСТЬ АТАКИ")}   {tower.fireRate:0.00}/s\n{L("SELL VALUE", "ЦЕНА ПРОДАЖИ")}      {tower.SellValue}";
    }

    public static string Upgrade(Tower tower)
    {
        if (tower == null) return "";
        if (tower.Level >= 3) return L("MAXIMUM LEVEL REACHED", "ДОСТИГНУТ МАКСИМАЛЬНЫЙ УРОВЕНЬ");

        float nextDamage = tower.damage * 1.32f;
        float nextRange = tower.range * 1.10f;
        float nextRate = tower.fireRate * 1.12f;
        return $"{L("NEXT UPGRADE", "СЛЕДУЮЩЕЕ УЛУЧШЕНИЕ")}   {tower.UpgradeCost} {L("GOLD", "ЗОЛОТА")}\n{tower.damage:0} -> {nextDamage:0} DMG   •   {tower.range:0.0} -> {nextRange:0.0} RNG   •   {tower.fireRate:0.00} -> {nextRate:0.00}/s";
    }

    public static string Priority(Tower tower)
    {
        if (tower == null) return "";
        return tower.Type == TowerType.TrojanGuard
            ? L("ROLE: BLOCKING SQUAD", "РОЛЬ: БЛОКИРУЮЩИЙ ОТРЯД")
            : $"{L("TARGETING", "ПРИОРИТЕТ")}: {tower.Priority}";
    }
}
