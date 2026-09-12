using System.Collections.Generic;

public static class TowerRegistry
{
    static readonly List<Tower> towers = new List<Tower>();
    public static IReadOnlyList<Tower> All => towers;
    public static int Count => towers.Count;

    public static void Register(Tower tower)
    {
        if (tower != null && !towers.Contains(tower)) towers.Add(tower);
    }

    public static void Unregister(Tower tower)
    {
        if (tower != null) towers.Remove(tower);
    }

    public static void Clear()
    {
        towers.Clear();
    }
}
