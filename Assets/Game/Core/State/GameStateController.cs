using UnityEngine;

public enum GameState
{
    Preparing = 0,
    EncounterRunning = 1,
    WaveRunning = EncounterRunning,
    BetweenEncounters = 2,
    BetweenWaves = BetweenEncounters,
    Paused = 3,
    Victory = 4,
    Defeat = 5,
    NarrativeEvent = 6
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
