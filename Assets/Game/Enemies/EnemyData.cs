using UnityEngine;

public enum EnemyArchetype
{
    Infantry,
    Runner,
    HeavyHoplite,
    ShieldBearer,
    Archer,
    BatteringRam,
    Boss
}

[CreateAssetMenu(menuName = "TheTroyGame/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string id;
    public string displayName;
    public EnemyArchetype archetype = EnemyArchetype.Infantry;
    public float hpMultiplier = 1f;
    public float speedMultiplier = 1f;
    public int reward = 20;
    public int baseDamage = 1;
    public float scale = 1f;
    public float armor;
    public float arrowResistance;
    public float attackRange;
    public float attackInterval = 1.5f;
    public Color color = Color.white;
}
