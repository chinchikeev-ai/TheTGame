public static class CombatHudTowerCatalog
{
    static string L(string en, string ru) => GameLanguage.T(en, ru);

    public static string BuildLabel(TowerType type, string key) =>
        $"[{key}]  {TowerName(type)}\n{TowerFactory.GetCost(type)} {L("GOLD", "ЗОЛОТА")}";

    public static string TowerName(TowerType type)
    {
        switch (type)
        {
            case TowerType.SpearThrower: return L("SPEAR", "КОПЬЯ");
            case TowerType.MachineGun: return L("ARCHERS", "ЛУЧНИКИ");
            case TowerType.Cannon: return L("BALLISTA", "БАЛЛИСТА");
            case TowerType.Slow: return L("APOLLO", "АПОЛЛОН");
            case TowerType.FireTower: return L("FIRE", "ОГОНЬ");
            case TowerType.TrojanGuard: return L("GUARD", "СТРАЖА");
            default: return type.ToString();
        }
    }

    public static string TowerDisplayName(TowerType type)
    {
        switch (type)
        {
            case TowerType.MachineGun: return L("TROJAN ARCHERS", "ТРОЯНСКИЕ ЛУЧНИКИ");
            case TowerType.SpearThrower: return L("SPEAR THROWERS", "МЕТАТЕЛИ КОПИЙ");
            case TowerType.Cannon: return L("BALLISTA CREW", "РАСЧЁТ БАЛЛИСТЫ");
            case TowerType.Slow: return L("PRIESTS OF APOLLO", "ЖРЕЦЫ АПОЛЛОНА");
            case TowerType.FireTower: return L("FIRE CREW", "ОГНЕННЫЙ РАСЧЁТ");
            case TowerType.TrojanGuard: return L("SHIELD GUARD", "ЩИТОВАЯ ГВАРДИЯ");
            default: return type.ToString().ToUpperInvariant();
        }
    }

    public static string TowerRole(TowerType type)
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

    public static string TowerTags(TowerType type)
    {
        switch (type)
        {
            case TowerType.MachineGun: return "[RANGED]   [LIGHT]";
            case TowerType.SpearThrower: return "[ARMOR]   [HEAVY]";
            case TowerType.Cannon: return "[SIEGE]   [BOSS]   [PIERCE]";
            case TowerType.Slow: return "[SLOW]   [SUPPORT]";
            case TowerType.FireTower: return "[AOE]   [BURN]   [ZONE]";
            case TowerType.TrojanGuard: return "[BLOCK]   [FRONTLINE]";
            default: return "";
        }
    }

    public static string StrongAgainst(TowerType type)
    {
        switch (type)
        {
            case TowerType.MachineGun: return L("Runners • Infantry", "Бегуны • Пехота");
            case TowerType.SpearThrower: return L("Heavy • Shields", "Тяжёлые • Щитоносцы");
            case TowerType.Cannon: return L("Boss • Siege • Heavy", "Босс • Осада • Тяжёлые");
            case TowerType.Slow: return L("Fast groups", "Быстрые группы");
            case TowerType.FireTower: return L("Dense groups", "Плотные группы");
            case TowerType.TrojanGuard: return L("Holding lanes", "Удержание линии");
            default: return "—";
        }
    }

    public static string WeakAgainst(TowerType type)
    {
        switch (type)
        {
            case TowerType.MachineGun: return L("Heavy armor", "Тяжёлая броня");
            case TowerType.SpearThrower: return L("Light swarms", "Толпы лёгких");
            case TowerType.Cannon: return L("Fast swarms", "Быстрые толпы");
            case TowerType.Slow: return L("Damage races", "Чистый урон");
            case TowerType.FireTower: return L("Single heavy", "Одиночные тяжёлые");
            case TowerType.TrojanGuard: return L("Ranged pressure", "Дальний обстрел");
            default: return "—";
        }
    }
}
