using UnityEngine;

public enum GameState
{
    Preparing,
    WaveRunning,
    BetweenWaves,
    Paused,
    Victory,
    Defeat,
    NarrativeEvent
}

public class GameStateController : MonoBehaviour
{
    public static GameStateController Instance { get; private set; }
    public GameState State { get; private set; } = GameState.Preparing;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void SetState(GameState state) => State = state;
}
