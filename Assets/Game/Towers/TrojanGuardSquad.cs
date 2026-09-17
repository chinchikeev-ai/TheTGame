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
    public float blockDamageMultiplier = .65f;
    public float braceTurnSpeed = 8f;
    public float frontalBlockDot = .05f;
    public bool IsAlive => Health > 0f;
    public float Health { get; private set; }
    public int BlockedCount => blockedEnemies.Count;
    public bool IsBraced => IsAlive && blockedEnemies.Count > 0;

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
        presentation?.SetBlocking(false);
        ReleaseAll();
    }

    public void Initialize(Tower tower)
    {
        ownerTower = tower;
        SyncCombatRange();
        Health = maxHealth;
    }

    public void ApplyRally(float duration, float damageMultiplier, float rateMultiplier)
    {
        rallyUntil = Mathf.Max(rallyUntil, Time.time + duration);
        rallyDamage = Mathf.Max(rallyDamage, damageMultiplier);
        rallyRate = Mathf.Max(rallyRate, rateMultiplier);
        RuntimeEffects.Instance?.PlayHeroAbilitySound();
        CombatImpactPresentation.Pulse(transform.position, new Color(1f,.55f,.10f), blockRadius * 2.2f, .30f);
    }

    public bool TryReserve(Enemy enemy)
    {
        if (!IsAlive || enemy == null || !enemy.IsAlive) return false;
        CleanupBlocked();
        if (blockedEnemies.Contains(enemy)) return true;
        if (blockedEnemies.Count >= Mathf.Max(0, blockCapacity)) return false;
        blockedEnemies.Add(enemy);
        ownerTower?.crewAnimation?.PlayGuardBlock();
        presentation?.SetBlocking(true);
        return true;
    }

    public bool OwnsReservation(Enemy enemy) => enemy != null && blockedEnemies.Contains(enemy);

    public void Release(Enemy enemy)
    {
        if (enemy != null) blockedEnemies.Remove(enemy);
        if (blockedEnemies.Count == 0) presentation?.SetBlocking(false);
    }

    void Update()
    {
        if (!IsAlive || GameManager.Instance == null || GameManager.Instance.GameEnded) return;
        SyncCombatRange();
        if (Time.time >= rallyUntil)
        {
            rallyDamage = 1f;
            rallyRate = 1f;
        }

        CleanupBlocked();
        FillOpenSlots();
        UpdateDefensivePresentation();

        Enemy attackTarget = FindNearestBlockedEnemy();
        if (attackTarget != null && Time.time >= nextAttack)
        {
            nextAttack = Time.time + 1f / Mathf.Max(.01f, attackRate * rallyRate);
            ownerTower?.crewAnimation?.PlayGuardPoke();
            float attackDamage = damage * rallyDamage;
            if (presentation == null)
            {
                ApplyAttackImpact(attackTarget, attackDamage);
            }
            else
            {
                presentation.PlaySpearAttack(() => ApplyAttackImpact(attackTarget, attackDamage));
            }
        }
    }

    void SyncCombatRange()
    {
        if (ownerTower == null) return;
        blockRadius = Mathf.Max(.1f, ownerTower.range);
    }

    void UpdateDefensivePresentation()
    {
        Enemy braceTarget = FindNearestBlockedEnemy();
        bool braced = braceTarget != null;
        presentation?.SetBlocking(braced);
        if (!braced) return;

        Vector3 direction = braceTarget.transform.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude <= .001f) return;
        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Mathf.Max(0f, braceTurnSpeed) * Time.deltaTime);
    }

    void ApplyAttackImpact(Enemy attackTarget, float attackDamage)
    {
        if (!IsAlive || attackTarget == null || !attackTarget.IsAlive || !blockedEnemies.Contains(attackTarget)) return;

        attackTarget.ReceiveDamage(new DamagePacket(attackDamage, DamageType.Physical, TowerType.TrojanGuard));
        CombatImpactPresentation.MeleeHit(attackTarget.transform.position + Vector3.up * .55f, TowerType.TrojanGuard);
        Vector3 shotPoint = transform.position + Vector3.up * .8f;
        RuntimeEffects.Instance?.PlayShotSound(TowerType.TrojanGuard);
        CombatImpactPresentation.ShotFlash(shotPoint, TowerType.TrojanGuard);
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
        if (blockedEnemies.Count == 0) presentation?.SetBlocking(false);
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
        presentation?.SetBlocking(false);
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
        ApplyIncomingDamage(amount, null);
    }

    public void TakeDamage(float amount, Vector3 sourcePoint)
    {
        ApplyIncomingDamage(amount, sourcePoint);
    }

    void ApplyIncomingDamage(float amount, Vector3? sourcePoint)
    {
        if (!IsAlive) return;
        float incoming = Mathf.Max(1f, amount);
        bool blocked = IsBraced && (!sourcePoint.HasValue || IsFacingSource(sourcePoint.Value));
        float applied = blocked ? incoming * Mathf.Clamp(blockDamageMultiplier, .05f, 1f) : incoming;
        Health -= applied;

        if (blocked)
        {
            RuntimeEffects.Instance?.PlayShieldBlockSound(incoming >= 25f);
            CombatImpactPresentation.Pulse(transform.position + Vector3.up * .72f, new Color(.95f,.68f,.22f), .72f, .16f);
            ownerTower?.crewAnimation?.PlayGuardBlock();
            presentation?.SetBlocking(true);
        }
        else
        {
            presentation?.PlayHit();
        }

        if (Health <= 0f)
        {
            Health = 0f;
            presentation?.SetBlocking(false);
            ReleaseAll();
            presentation?.PlayDeath(false);
            RuntimeFileLogger.Event("GUARD", "Trojan Guard squad destroyed");
            if (ownerTower != null) ownerTower.DestroyWithoutRefund();
            else Destroy(gameObject);
        }
    }

    bool IsFacingSource(Vector3 sourcePoint)
    {
        Vector3 direction = sourcePoint - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude <= .001f) return true;
        return Vector3.Dot(transform.forward, direction.normalized) >= frontalBlockDot;
    }
}
