using System.Collections.Generic;

public static class EnemyRegistry
{
    static readonly HashSet<Enemy> enemies = new HashSet<Enemy>();
    public static IEnumerable<Enemy> All => enemies;
    public static int AliveCount => enemies.Count;
    public static void Register(Enemy enemy) { if (enemy != null) enemies.Add(enemy); }
    public static void Unregister(Enemy enemy) { if (enemy != null) enemies.Remove(enemy); }
    public static void Clear() => enemies.Clear();
}
