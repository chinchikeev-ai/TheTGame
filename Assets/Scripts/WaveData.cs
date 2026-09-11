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

    [Header("Pacing")]
    public float preparationTime = 20f;
    public float spawnInterval = 3f;
    public float targetDuration = 60f;
}
