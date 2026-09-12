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
    public float Armor => armor;
    public float ArrowResistance => arrowResistance;
    public float RouteProgress
    {
        get
        {
            if (waypoints == null || waypoints.Length == 0) return 0f;
            float segment = Mathf.Clamp01((float)waypointIndex / waypoints.Length);
            if (waypointIndex >= waypoints.Length) return 1f;
            return segment;
        }
    }

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
    float nextAttack;

    float burnUntil;
    float burnDps;
    float nextBurnTick;
    float armorBreakUntil;
    float armorBreakAmount;
    float commanderUntil;
    float commanderSpeedMultiplier = 1f;
    float commanderDamageMultiplier = 1f;
    TrojanGuardSquad blockingGuard;

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

    public void SetBlockedByGuard(TrojanGuardSquad guard)
    {
        if (guard != null && guard.IsAlive) blockingGuard = guard;
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameEnded || Health <= 0f) return;
        TickStatuses();
        if (Health <= 0f) return;
        if (waypoints == null || waypoints.Length == 0 || waypointIndex >= waypoints.Length) return;

        if (Time.time >= slowUntil) slowMultiplier = 1f;
        if (Time.time >= commanderUntil)
        {
            commanderSpeedMultiplier = 1f;
            commanderDamageMultiplier = 1f;
        }
        speed = baseSpeed * slowMultiplier * commanderSpeedMultiplier;

        if (blockingGuard != null)
        {
            if (!blockingGuard.IsAlive || Vector3.Distance(transform.position, blockingGuard.transform.position) > blockingGuard.blockRadius * 1.35f)
            {
                blockingGuard = null;
            }
            else
            {
                if (Time.time >= nextAttack)
                {
                    nextAttack = Time.time + attackInterval;
                    blockingGuard.TakeDamage(Mathf.Max(4f, baseDamage * 18f * commanderDamageMultiplier));
                }
                return;
            }
        }

        HectorController hector = HectorController.Instance;
        if (hector != null && !hector.IsDowned)
        {
            float distanceToHector = Vector3.Distance(transform.position, hector.transform.position);
            float meleeRange = 1.35f + transform.localScale.x * .25f;
            bool canAttackHector = distanceToHector <= meleeRange || (Archetype == EnemyArchetype.Archer && attackRange > 0f && distanceToHector <= attackRange);
            if (canAttackHector)
            {
                if (Time.time >= nextAttack)
                {
                    nextAttack = Time.time + attackInterval;
                    hector.TakeDamage(Mathf.Max(1f, baseDamage * 12f * commanderDamageMultiplier));
                }
                return;
            }
        }

        Transform finalTarget = waypoints[waypoints.Length - 1];
        if (Archetype == EnemyArchetype.Archer && attackRange > 0f)
        {
            float gateDistance = Vector3.Distance(transform.position, finalTarget.position);
            if (gateDistance <= attackRange)
            {
                if (Time.time >= nextAttack)
                {
                    nextAttack = Time.time + attackInterval;
                    GameManager.Instance.DamageBase(Mathf.Max(1, Mathf.RoundToInt(baseDamage * commanderDamageMultiplier)));
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

    void TickStatuses()
    {
        if (Time.time < burnUntil && burnDps > 0f && Time.time >= nextBurnTick)
        {
            nextBurnTick = Time.time + 1f;
            ReceiveDamage(new DamagePacket(burnDps, DamageType.Fire));
        }
        else if (Time.time >= burnUntil)
        {
            burnDps = 0f;
        }
    }

    public void TakeDamage(float damage) => ReceiveDamage(new DamagePacket(damage, DamageType.Physical));
    public void TakeDamage(float damage, TowerType sourceType) => ReceiveDamage(new DamagePacket(damage, DamageRules.ForTower(sourceType), sourceType));

    public void ReceiveDamage(DamagePacket packet)
    {
        if (Health <= 0f) return;

        float currentArmor = armor;
        if (Time.time < armorBreakUntil)
            currentArmor = Mathf.Max(0f, currentArmor - armorBreakAmount);

        float armorFactor;
        switch (packet.type)
        {
            case DamageType.Piercing: armorFactor = currentArmor * .45f; break;
            case DamageType.Fire: armorFactor = currentArmor * .20f; break;
            case DamageType.Hero: armorFactor = currentArmor * .15f; break;
            default: armorFactor = currentArmor; break;
        }

        float bonus = 1f;
        if (packet.towerSource.HasValue && packet.towerSource.Value == TowerType.SpearThrower &&
            (Archetype == EnemyArchetype.HeavyHoplite || Archetype == EnemyArchetype.ShieldBearer || Archetype == EnemyArchetype.BatteringRam))
            bonus = 1.5f;
        if (packet.towerSource.HasValue && packet.towerSource.Value == TowerType.TrojanGuard &&
            (Archetype == EnemyArchetype.Infantry || Archetype == EnemyArchetype.Runner))
            bonus = 1.25f;

        float finalDamage = packet.amount * bonus * (1f - Mathf.Clamp01(armorFactor));
        if (packet.towerSource.HasValue && packet.towerSource.Value == TowerType.MachineGun)
            finalDamage *= 1f - arrowResistance;

        Health -= Mathf.Max(1f, finalDamage);
        if (RuntimeEffects.Instance != null) RuntimeEffects.Instance.PlayHit(transform.position, packet.type == DamageType.Fire || baseDamage > 1);
        if (healthBar != null) healthBar.Refresh();
        if (Health <= 0f) Die();
    }

    public void ApplySlow(float multiplier, float duration)
    {
        multiplier = Mathf.Clamp(multiplier, .15f, 1f);
        if (multiplier < slowMultiplier || Time.time >= slowUntil) slowMultiplier = multiplier;
        slowUntil = Mathf.Max(slowUntil, Time.time + duration);
    }

    public void ApplyBurn(float damagePerSecond, float duration)
    {
        burnDps = Mathf.Max(burnDps, damagePerSecond);
        burnUntil = Mathf.Max(burnUntil, Time.time + duration);
        nextBurnTick = Mathf.Min(nextBurnTick <= 0f ? Time.time + .5f : nextBurnTick, Time.time + .5f);
    }

    public void ApplyArmorBreak(float amount, float duration)
    {
        armorBreakAmount = Mathf.Max(armorBreakAmount, Mathf.Clamp01(amount));
        armorBreakUntil = Mathf.Max(armorBreakUntil, Time.time + duration);
    }

    public void ApplyCommanderAura(float speedMultiplier, float damageMultiplier, float duration)
    {
        commanderSpeedMultiplier = Mathf.Max(commanderSpeedMultiplier, speedMultiplier);
        commanderDamageMultiplier = Mathf.Max(commanderDamageMultiplier, damageMultiplier);
        commanderUntil = Mathf.Max(commanderUntil, Time.time + duration);
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
            GameManager.Instance.DamageBase(Mathf.Max(1, Mathf.RoundToInt(baseDamage * commanderDamageMultiplier)));
        }
        if (RuntimeEffects.Instance != null) RuntimeEffects.Instance.PlayDeath(transform.position, Archetype == EnemyArchetype.Boss || Archetype == EnemyArchetype.BatteringRam);
        Destroy(gameObject);
    }
}
