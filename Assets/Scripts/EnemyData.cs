using UnityEngine;

[CreateAssetMenu(menuName = "TheTroyGame/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string id;
    public string displayName;
    public float hpMultiplier = 1f;
    public float speedMultiplier = 1f;
    public int reward = 20;
    public int baseDamage = 1;
    public float scale = 1f;
    public Color color = Color.white;
}
