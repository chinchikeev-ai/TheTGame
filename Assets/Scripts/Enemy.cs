using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    public float speed = 2.2f;
    public int reward = 20;
    public int baseDamage = 1;
    public EnemyArchetype Archetype { get; private set; } = EnemyArchetype.Infantry;

    public float Health { get; private set; }
    public float Health01 => maxHealth <= 0f ? 0f : Mathf.Clamp01(Health / maxHealth);

    Transform[] waypoints;
    int waypointIndex;
    EnemyHealthBar healthBar;
    float baseSpeed;
    float slowMultiplier = 1f;
    float slowUntil;
    float armor;
    float arrowResistance;
    float attackRange;
    float attackInterval = 1.5f;
    float nextRangedAttack;

    void OnEnable() => EnemyRegistry.Register(this);
    void OnDisable() => EnemyRegistry.Unregister(this);

    public void InitFromData(Transform[] path, EnemyData data, float waveHpMultiplier, float waveSpeedMultiplier)
    {
        waypoints = path;
        if (data != null)
        {
            Archetype = data.archetype;
            reward = data.reward;
            baseDamage = data.baseDamage;
            transform.localScale *= data.scale;
            maxHealth *= data.hpMultiplier * waveHpMultiplier;
            speed *= data.speedMultiplier * waveSpeedMultiplier;
            armor = Mathf.Clamp01(data.armor);
            arrowResistance = Mathf.Clamp01(data.arrowResistance);
            attackRange = Mathf.Max(0f, data.attackRange);
            attackInterval = Mathf.Max(.25f, data.attackInterval);
        }
        baseSpeed = speed;
        Health = maxHealth;
        waypointIndex = 0;
        healthBar = gameObject.AddComponent<EnemyHealthBar>();
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameEnded) return;
        if (waypoints == null || waypoints.Length == 0 || waypointIndex >= waypoints.Length) return;
        if (Time.time >= slowUntil) slowMultiplier = 1f;
        speed = baseSpeed * slowMultiplier;

        Transform finalTarget = waypoints[waypoints.Length - 1];
        if (Archetype == EnemyArchetype.Archer && attackRange > 0f)
        {
            float gateDistance = Vector3.Distance(transform.position, finalTarget.position);
            if (gateDistance <= attackRange)
            {
                if (Time.time >= nextRangedAttack)
                {
                    nextRangedAttack = Time.time + attackInterval;
                    GameManager.Instance.DamageBase(baseDamage);
                    if (RuntimeEffects.Instance != null) RuntimeEffects.Instance.PlayHit(finalTarget.position, false);
                }
                return;
            }
        }

        Transform target = waypoints[waypointIndex];
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > .001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 10f * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < .15f)
        {
            waypointIndex++;
            if (waypointIndex >= waypoints.Length) ReachBase();
        }
    }

    public void TakeDamage(float damage) => ApplyDamage(damage, null);
    public void TakeDamage(float damage, TowerType sourceType) => ApplyDamage(damage, sourceType);

    void ApplyDamage(float damage, TowerType? sourceType)
    {
        if (Health <= 0f) return;
        float effectiveArmor = armor;
        if (sourceType.HasValue && sourceType.Value == TowerType.Cannon) effectiveArmor *= .45f;

        float bonus = 1f;
        if (sourceType.HasValue && sourceType.Value == TowerType.SpearThrower &&
            (Archetype == EnemyArchetype.HeavyHoplite || Archetype == EnemyArchetype.ShieldBearer || Archetype == EnemyArchetype.BatteringRam)) bonus = 1.5f;
        if (sourceType.HasValue && sourceType.Value == TowerType.TrojanGuard &&
            (Archetype == EnemyArchetype.Infantry || Archetype == EnemyArchetype.Runner)) bonus = 1.25f;

        float finalDamage = damage * bonus * (1f - effectiveArmor);
        if (sourceType.HasValue && sourceType.Value == TowerType.MachineGun)
            finalDamage *= 1f - arrowResistance;
        Health -= Mathf.Max(1f, finalDamage);
        if (RuntimeEffects.Instance != null) RuntimeEffects.Instance.PlayHit(transform.position, baseDamage > 1);
        if (healthBar != null) healthBar.Refresh();
        if (Health <= 0f) Die();
    }

    public void ApplySlow(float multiplier, float duration)
    {
        multiplier = Mathf.Clamp(multiplier, .15f, 1f);
        if (multiplier < slowMultiplier || Time.time >= slowUntil) slowMultiplier = multiplier;
        slowUntil = Mathf.Max(slowUntil, Time.time + duration);
    }

    void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RecordKill();
            GameManager.Instance.AddMoney(reward);
        }
        if (RuntimeEffects.Instance != null) RuntimeEffects.Instance.PlayDeath(transform.position, Archetype == EnemyArchetype.Boss);
        Destroy(gameObject);
    }

    void ReachBase()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RecordLeak();
            GameManager.Instance.DamageBase(baseDamage);
        }
        if (RuntimeEffects.Instance != null) RuntimeEffects.Instance.PlayDeath(transform.position, Archetype == EnemyArchetype.Boss || Archetype == EnemyArchetype.BatteringRam);
        Destroy(gameObject);
    }
}
