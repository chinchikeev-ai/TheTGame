using System.Collections.Generic;
using UnityEngine;

public class HectorController : MonoBehaviour
{
    public static HectorController Instance { get; private set; }

    public bool Selected { get; private set; }
    public float maxHealth = 500f;
    public float Health { get; private set; }
    public bool IsDowned { get; private set; }
    public float DownedRemaining => IsDowned ? Mathf.Max(0f, reviveAt - Time.time) : 0f;

    public float moveSpeed = 6f;
    public float attackRange = 2.4f;
    public float attackDamage = 45f;
    public float attackRate = 1.1f;

    public float warCryRadius = 5.2f;
    public float warCryDuration = 10f;
    public float warCryCooldown = 20f;
    public float shieldWallCooldown = 22f;
    public float spearThrowCooldown = 12f;
    public float spearThrowRange = 10f;
    public float spearThrowDamage = 220f;
    public float ultimateCooldown = 75f;
    public float ultimateDuration = 12f;
    public float ultimateEnemyRadius = 8f;
    public float downedDuration = 12f;
    public float battlefieldMargin = 0.65f;

    readonly List<Enemy> enemySnapshot = new List<Enemy>(64);

    Vector3 destination;
    Vector3 spawnPosition;
    float nextAttack;
    float nextWarCry;
    float nextShieldWall;
    float nextSpearThrow;
    float nextUltimate;
    float reviveAt;
    HectorPresentationBridge presentation;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        transform.position = MapBuilder.ClampToPlayableArea(transform.position, battlefieldMargin);
        destination = transform.position;
        spawnPosition = transform.position;
        Health = maxHealth;
        presentation = GetComponent<HectorPresentationBridge>();
        presentation?.SetSelected(Selected);
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameEnded) return;

        if (IsDowned)
        {
            if (Time.time >= reviveAt) Revive();
            return;
        }

        Move();
        AutoAttack();
    }

    public void SetSelected(bool selected)
    {
        bool next = !IsDowned && selected;
        if (Selected == next) return;
        Selected = next;
        presentation?.SetSelected(Selected);
    }

    public void MoveTo(Vector3 worldPosition)
    {
        if (IsDowned) return;
        destination = worldPosition;
        destination.y = transform.position.y;
        destination = MapBuilder.ClampToPlayableArea(destination, battlefieldMargin);
    }

    void Move()
    {
        destination = MapBuilder.ClampToPlayableArea(destination, battlefieldMargin);
        Vector3 delta = destination - transform.position;
        delta.y = 0f;
        if (delta.sqrMagnitude > .01f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(delta), 10f * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
        transform.position = MapBuilder.ClampToPlayableArea(transform.position, battlefieldMargin);
    }

    void AutoAttack()
    {
        if (Time.time < nextAttack) return;
        Enemy best = FindNearestEnemy(attackRange);
        if (best == null) return;

        nextAttack = Time.time + 1f / Mathf.Max(.01f, attackRate);
        best.ReceiveDamage(new DamagePacket(attackDamage, DamageType.Hero));
        presentation?.PlayAttackImpact(best.transform.position);
    }

    public void TakeDamage(float damage)
    {
        if (IsDowned || Health <= 0f) return;
        Health -= Mathf.Max(1f, damage);
        presentation?.PlayDamageImpact(damage);
        if (Health <= 0f) Down();
    }

    void Down()
    {
        Health = 0f;
        IsDowned = true;
        SetSelected(false);
        reviveAt = Time.time + downedDuration;
        destination = transform.position;
        RuntimeFileLogger.Event("HECTOR", $"Downed; reviveIn={downedDuration:0.0}s");
    }

    void Revive()
    {
        IsDowned = false;
        Health = maxHealth * .50f;
        transform.position = MapBuilder.ClampToPlayableArea(spawnPosition, battlefieldMargin);
        destination = transform.position;
        RuntimeFileLogger.Event("HECTOR", $"Revived hp={Health:0}/{maxHealth:0}");
    }

    public void UseWarCry()
    {
        if (IsDowned || Time.time < nextWarCry) return;
        nextWarCry = Time.time + warCryCooldown;

        int buffed = 0;
        float radiusSq = warCryRadius * warCryRadius;
        foreach (Tower tower in TowerRegistry.All)
        {
            if (tower == null) continue;
            if ((tower.transform.position - transform.position).sqrMagnitude > radiusSq) continue;
            tower.ApplyWarCry(warCryDuration, 1.15f, 1.30f);
            buffed++;
        }

        RuntimeFileLogger.Event("HECTOR", $"War Cry used; towersBuffed={buffed}");
        presentation?.PlayWarCry(warCryRadius);
    }

    public void UseShieldWall()
    {
        if (IsDowned || Time.time < nextShieldWall) return;
        nextShieldWall = Time.time + shieldWallCooldown;

        Vector3 center = transform.position + transform.forward * 2.2f;
        HectorShieldWallFactory.Create(center, transform.rotation);

        RuntimeFileLogger.Event("HECTOR", "Shield Wall used");
        presentation?.PlayShieldWall(center);
    }

    public void UseSpearThrow()
    {
        if (IsDowned || Time.time < nextSpearThrow) return;
        Enemy target = FindNearestEnemy(spearThrowRange);
        if (target == null) return;

        nextSpearThrow = Time.time + spearThrowCooldown;
        target.ReceiveDamage(new DamagePacket(spearThrowDamage, DamageType.Hero));
        target.ApplyArmorBreak(.25f, 6f);
        RuntimeFileLogger.Event("HECTOR", $"Spear Throw hit {target.name} damage={spearThrowDamage:0}");
        presentation?.PlaySpearImpact(target.transform.position);
    }

    public void UseUltimate()
    {
        if (IsDowned || Time.time < nextUltimate) return;
        nextUltimate = Time.time + ultimateCooldown;
        Health = Mathf.Min(maxHealth, Health + maxHealth * .35f);

        int towersBuffed = 0;
        foreach (Tower tower in TowerRegistry.All)
        {
            if (tower == null) continue;
            tower.ApplyWarCry(ultimateDuration, 1.35f, 1.50f);
            towersBuffed++;
        }

        int enemiesHit = 0;
        float radiusSq = ultimateEnemyRadius * ultimateEnemyRadius;
        enemySnapshot.Clear();
        foreach (Enemy enemy in EnemyRegistry.All)
            if (enemy != null) enemySnapshot.Add(enemy);

        for (int i = 0; i < enemySnapshot.Count; i++)
        {
            Enemy enemy = enemySnapshot[i];
            if (enemy == null || (enemy.transform.position - transform.position).sqrMagnitude > radiusSq) continue;
            enemy.ReceiveDamage(new DamagePacket(100f, DamageType.Hero));
            if (enemy != null) enemy.ApplySlow(.70f, 4f);
            enemiesHit++;
        }
        enemySnapshot.Clear();

        RuntimeFileLogger.Event("HECTOR", $"For Troy ultimate used; towersBuffed={towersBuffed}, enemiesHit={enemiesHit}, hp={Health:0}/{maxHealth:0}");
        presentation?.PlayUltimate(ultimateEnemyRadius);
    }

    Enemy FindNearestEnemy(float radius)
    {
        Enemy best = null;
        float bestSq = radius * radius;
        foreach (Enemy enemy in EnemyRegistry.All)
        {
            if (enemy == null) continue;
            float distanceSq = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distanceSq >= bestSq) continue;
            bestSq = distanceSq;
            best = enemy;
        }
        return best;
    }

    public float WarCryCooldownRemaining => Mathf.Max(0f, nextWarCry - Time.time);
    public float ShieldWallCooldownRemaining => Mathf.Max(0f, nextShieldWall - Time.time);
    public float SpearThrowCooldownRemaining => Mathf.Max(0f, nextSpearThrow - Time.time);
    public float UltimateCooldownRemaining => Mathf.Max(0f, nextUltimate - Time.time);
}
