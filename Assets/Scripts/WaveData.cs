using UnityEngine;

[CreateAssetMenu(menuName = "TheTroyGame/Wave Data")]
public class WaveData : ScriptableObject
{
    public int waveNumber;
    public int enemyCount;
    public float hpMultiplier = 1f;
    public float speedMultiplier = 1f;
    public int heavyEvery;
    public bool hasBoss;
}
