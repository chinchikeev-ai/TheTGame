using System.Collections.Generic;
using UnityEngine;

public class TrojanGuardSquad : MonoBehaviour
{
    static readonly List<TrojanGuardSquad> all = new List<TrojanGuardSquad>();
    readonly HashSet<Enemy> blockedEnemies = new HashSet<Enemy>();
    readonly List<Enemy> cleanupBuffer = new List<Enemy>(4);

    public static IReadOnlyList<TrojanGuardSquad> All => all;

    public float maxHealth = 700f;
    public float blockRadius = 1.25f;
    public float damage = 34f;
    public float attackRate = 1.0f;
    public int blockCapacity = 3;
    public bool IsAlive => Health > 0f;
    public float Health { get; private set; }
    public int BlockedCount => blockedEnemies.Count;

    Tower ownerTower;
    float nextAttack;
    float rallyUntil;
    float rallyDamage = 1f;
    float rallyRate = 1f;
    CharacterPresentationState presentation;

    void OnEnable()
    {
        if (!all.Contains(this)) all.Add(this);
        presentation = GetComponent<CharacterPresentationState>();
        if (presentation == null) presentation = gameObject.AddComponent<CharacterPresentationState>();
    }

    void OnDisable()
    {
        all.Remove(this);
        ReleaseAll();
    }

    public void Initialize(Tower tower)
    {
        ownerTower = tower;
        Health = maxHealth;
    }

    public void ApplyRally(float duration, float damageMultiplier, float rateMultiplier)
    {
        rallyUntil = Mathf.Max(rallyUntil, Time.time + duration);
        rallyDamage = Mathf.Max(rallyDamage, damageMultiplier);
        rallyRate = Mathf.Max(rallyRate, rateMultiplier);
        RuntimeEffects.Instance?.PlayHeroPulse(transform.position, new Color(1f,.55f,.10f), blockRadius * 2.2f, .30f);
    }

    public bool TryReserve(Enemy enemy)
    {
        if (!IsAlive || enemy == null || !enemy.IsAlive) return false;
        CleanupBlocked();
        if (blockedEnemies.Contains(enemy)) return true;
        if (blockedEnemies.Count >= Mathf.Max(0, blockCapacity)) return false;
        blockedEnemies.Add(enemy);
        return true;
    }

    public bool OwnsReservation(Enemy enemy) => enemy != null && blockedEnemies.Contains(enemy);

    public void Release(Enemy enemy)
    {
        if (enemy != null) blockedEnemies.Remove(enemy);
    }

    void Update()
    {
        if (!IsAlive || GameManager.Instance == null || GameManager.Instance.GameEnded) return;
        if (Time.time >= rallyUntil)
        {
            rallyDamage = 1f;
            rallyRate = 1f;
        }

        CleanupBlocked();
        FillOpenSlots();

        Enemy attackTarget = FindNearestBlockedEnemy();
        if (attackTarget != null && Time.time >= nextAttack)
        {
            nextAttack = Time.time + 1f / Mathf.Max(.01f, attackRate * rallyRate);
            presentation?.PlayAttack();
            attackTarget.ReceiveDamage(new DamagePacket(damage * rallyDamage, DamageType.Physical, TowerType.TrojanGuard));
            RuntimeEffects.Instance?.PlayShot(TowerType.TrojanGuard, transform.position + Vector3.up * .8f);
        }
    }

    void FillOpenSlots()
    {
        int capacity = Mathf.Max(0, blockCapacity);
        if (blockedEnemies.Count >= capacity) return;

        float radiusSq = blockRadius * blockRadius;
        while (blockedEnemies.Count < capacity)
        {
            Enemy nearest = null;
            float nearestSq = radiusSq;

            foreach (Enemy enemy in EnemyRegistry.All)
            {
                if (enemy == null || !enemy.IsAlive || enemy.IsBlockedByGuard || blockedEnemies.Contains(enemy)) continue;
                float distanceSq = (enemy.transform.position - transform.position).sqrMagnitude;
                if (distanceSq > nearestSq) continue;
                nearestSq = distanceSq;
                nearest = enemy;
            }

            if (nearest == null || !nearest.TrySetBlockedByGuard(this)) break;
        }
    }

    void CleanupBlocked()
    {
        if (blockedEnemies.Count == 0) return;
        float releaseRadius = blockRadius * 1.35f;
        float releaseRadiusSq = releaseRadius * releaseRadius;

        cleanupBuffer.Clear();
        foreach (Enemy enemy in blockedEnemies)
        {
            bool valid = enemy != null && IsAlive && enemy.IsAlive &&
                         (enemy.transform.position - transform.position).sqrMagnitude <= releaseRadiusSq;
            if (!valid) cleanupBuffer.Add(enemy);
        }

        for (int i = 0; i < cleanupBuffer.Count; i++)
        {
            Enemy enemy = cleanupBuffer[i];
            if (enemy == null) blockedEnemies.Remove(enemy);
            else enemy.ClearBlockedByGuard(this);
        }
        cleanupBuffer.Clear();
    }

    Enemy FindNearestBlockedEnemy()
    {
        Enemy bestEnemy = null;
        float bestDistance = float.MaxValue;
        foreach (Enemy enemy in blockedEnemies)
        {
            if (enemy == null || !enemy.IsAlive) continue;
            float distance = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distance >= bestDistance) continue;
            bestDistance = distance;
            bestEnemy = enemy;
        }
        return bestEnemy;
    }

    void ReleaseAll()
    {
        if (blockedEnemies.Count == 0) return;

        cleanupBuffer.Clear();
        foreach (Enemy enemy in blockedEnemies) cleanupBuffer.Add(enemy);
        for (int i = 0; i < cleanupBuffer.Count; i++)
        {
            Enemy enemy = cleanupBuffer[i];
            if (enemy != null) enemy.ClearBlockedByGuard(this);
        }
        blockedEnemies.Clear();
        cleanupBuffer.Clear();
    }

    public void TakeDamage(float amount)
    {
        if (!IsAlive) return;
        Health -= Mathf.Max(1f, amount);
        presentation?.PlayHit();
        if (Health <= 0f)
        {
            Health = 0f;
            ReleaseAll();
            presentation?.PlayDeath(false);
            RuntimeFileLogger.Event("GUARD", "Trojan Guard squad destroyed");
            if (ownerTower != null) ownerTower.DestroyWithoutRefund();
            else Destroy(gameObject);
        }
    }
}
