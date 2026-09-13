using UnityEngine;

// Compatibility/state service for combat speed. ModernCombatHud owns all combat-control graphics.
public class CombatControlsUI : MonoBehaviour
{
    static readonly float[] Speeds = { 1f, 2f, 3f, 5f };
    static int speedIndex;

    public static float CurrentSpeed => Speeds[Mathf.Clamp(speedIndex, 0, Speeds.Length - 1)];

    public static void IncreaseSpeed()
    {
        speedIndex = Mathf.Min(speedIndex + 1, Speeds.Length - 1);
        Time.timeScale = CurrentSpeed;
    }

    public static void DecreaseSpeed()
    {
        speedIndex = Mathf.Max(speedIndex - 1, 0);
        Time.timeScale = CurrentSpeed;
    }

    public static void ResumeConfiguredSpeed() => Time.timeScale = CurrentSpeed;
}
