using UnityEngine;

public sealed class EconomyController
{
    public int Money { get; private set; }
    public int GoldEarned { get; private set; }
    public int GoldSpent { get; private set; }

    public EconomyController(int startingGold)
    {
        Money = Mathf.Max(0, startingGold);
    }

    public void AddIncome(int amount)
    {
        if (amount <= 0) return;
        Money += amount;
        GoldEarned += amount;
    }

    public void AddRefund(int amount)
    {
        if (amount <= 0) return;
        Money += amount;
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0) return true;
        if (Money < amount) return false;
        Money -= amount;
        GoldSpent += amount;
        return true;
    }
}
