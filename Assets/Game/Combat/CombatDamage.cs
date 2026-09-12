using UnityEngine;

public enum DamageType
{
    Physical,
    Piercing,
    Fire,
    Hero
}

public struct DamagePacket
{
    public float amount;
    public DamageType type;
    public TowerType? towerSource;

    public DamagePacket(float amount, DamageType type, TowerType? towerSource = null)
    {
        this.amount = amount;
        this.type = type;
        this.towerSource = towerSource;
    }
}

public static class DamageRules
{
    public static DamageType ForTower(TowerType type)
    {
        switch (type)
        {
            case TowerType.Cannon:
            case TowerType.SpearThrower:
                return DamageType.Piercing;
            case TowerType.FireTower:
                return DamageType.Fire;
            default:
                return DamageType.Physical;
        }
    }
}
