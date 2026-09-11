using UnityEngine;

[CreateAssetMenu(menuName = "TheTroyGame/Tower Data")]
public class TowerData : ScriptableObject
{
    public TowerType type;
    public string displayName;
    public int cost;
    public float damage;
    public float range;
    public float attacksPerSecond;
    public float projectileSpeed;
    public float splashRadius;
    public float slowMultiplier = 1f;
    public float slowDuration;
    [Range(0.5f, 0.9f)] public float sellRatio = 0.65f;
}
