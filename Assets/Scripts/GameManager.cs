using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int Money { get; private set; } = 300;
    public int BaseHealth { get; private set; } = 20;
    public int CurrentWave { get; set; } = 0;
    public int MaxWaves { get; set; } = 7;
    public bool GameEnded { get; private set; }
    public string EndMessage { get; private set; } = "";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddMoney(int amount) => Money += amount;

    public bool SpendMoney(int amount)
    {
        if (GameEnded || Money < amount) return false;
        Money -= amount;
        return true;
    }

    public void DamageBase(int damage)
    {
        if (GameEnded) return;
        BaseHealth = Mathf.Max(0, BaseHealth - damage);
        if (BaseHealth <= 0) LoseGame();
    }

    public void WinGame()
    {
        if (GameEnded) return;
        GameEnded = true;
        EndMessage = "VICTORY";
        GameStateController.Instance?.SetState(GameState.Victory);
    }

    void LoseGame()
    {
        GameEnded = true;
        BaseHealth = 0;
        EndMessage = "GAME OVER";
        GameStateController.Instance?.SetState(GameState.Defeat);
    }
}
