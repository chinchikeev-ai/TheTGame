using UnityEngine;

public static class EnemyDamageResolver
{
    public static float Resolve(DamagePacket packet, EnemyArchetype archetype, float armor, float arrowResistance)
    {
        float armorFactor;
        switch (packet.type)
        {
            case DamageType.Piercing: armorFactor = armor * .45f; break;
            case DamageType.Fire: armorFactor = armor * .20f; break;
            case DamageType.Hero: armorFactor = armor * .15f; break;
            default: armorFactor = armor; break;
        }

        float bonus = SourceBonus(packet.towerSource, archetype);
        float damage = packet.amount * bonus * (1f - Mathf.Clamp01(armorFactor));

        if (packet.towerSource == TowerType.MachineGun)
            damage *= 1f - Mathf.Clamp01(arrowResistance);

        return Mathf.Max(1f, damage);
    }

    static float SourceBonus(TowerType? source, EnemyArchetype archetype)
    {
        if (source == TowerType.SpearThrower &&
            (archetype == EnemyArchetype.HeavyHoplite ||
             archetype == EnemyArchetype.ShieldBearer ||
             archetype == EnemyArchetype.BatteringRam))
            return 1.5f;

        if (source == TowerType.TrojanGuard &&
            (archetype == EnemyArchetype.Infantry || archetype == EnemyArchetype.Runner))
            return 1.25f;

        return 1f;
    }
}
