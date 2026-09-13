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
    public bool IsAlive => Health > 0f && !dying;
    public bool IsBlockedByGuard => blockingGuard != null;
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
    CharacterPresentationState presentation;
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
    float nextBurnVisual;
    float armorBreakUntil;
    float armorBreakAmount;
    float commanderUntil;
    float commanderSpeedMultiplier = 1f;
    float commanderDamageMultiplier = 1f;
    bool attackingGate;
    bool dying;
    TrojanGuardSquad blockingGuard;

    void OnEnable() => EnemyRegistry.Register(this);

    void OnDisable()
    {
        ReleaseGuardReservation();
        EnemyRegistry.Unregister(this);
    }

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
        dying = false;
        presentation = GetComponent<CharacterPresentationState>();
        if (presentation == null) presentation = gameObject.AddComponent<CharacterPresentationState>();
        healthBar = gameObject.AddComponent<EnemyHealthBar>();
        HeavyEnemyGroundVfx.Attach(this);
    }

    public bool TrySetBlockedByGuard(TrojanGuardSquad guard)
    {
        if (!IsAlive || guard == null || !guard.IsAlive) return false;
        if (blockingGuard == guard) return guard.TryReserve(this);

        if (blockingGuard != null)
        {
            if (blockingGuard.IsAlive && blockingGuard.OwnsReservation(this)) return false;
            blockingGuard.Release(this);
            blockingGuard = null;
        }

        if (!guard.TryReserve(this)) return false;
        blockingGuard = guard;
        presentation?.SetMoving(false);
        return true;
    }

    public void SetBlockedByGuard(TrojanGuardSquad guard) => TrySetBlockedByGuard(guard);

    public void ClearBlockedByGuard(TrojanGuardSquad guard)
    {
        if (blockingGuard != guard) return;
        TrojanGuardSquad previous = blockingGuard;
        blockingGuard = null;
        previous?.Release(this);
    }

    void ReleaseGuardReservation()
    {
        if (blockingGuard == null) return;
        TrojanGuardSquad previous = blockingGuard;
        blockingGuard = null;
        previous.Release(this);
    }

    void Update()
    {
        if (dying || GameManager.Instance == null || GameManager.Instance.GameEnded || Health <= 0f) return;
        TickStatuses();
        if (Health <= 0f) return;
        if (attackingGate)
        {
            presentation?.SetMoving(false);
            AttackGateOverTime();
            return;
        }
        if (waypoints == null || waypoints.Length == 0 || waypointIndex >= waypoints.Length)
        {
            presentation?.SetMoving(false);
            return;
        }

        if (Time.time >= slowUntil) slowMultiplier = 1f;
        if (Time.time >= commanderUntil)
        {
            commanderSpeedMultiplier = 1f;
            commanderDamageMultiplier = 1f;
        }
        speed = baseSpeed * slowMultiplier * commanderSpeedMultiplier;

        if (blockingGuard != null)
        {
            bool reservationValid = blockingGuard.IsAlive && blockingGuard.OwnsReservation(this) &&
                                    Vector3.Distance(transform.position, blockingGuard.transform.position) <= blockingGuard.blockRadius * 1.35f;
            if (!reservationValid)
            {
                ReleaseGuardReservation();
            }
            else
            {
                presentation?.SetMoving(false);
                if (Time.time >= nextAttack)
                {
                    nextAttack = Time.time + attackInterval;
                    presentation?.PlayAttack();
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
                presentation?.SetMoving(false);
                if (Time.time >= nextAttack)
                {
                    nextAttack = Time.time + attackInterval;
                    presentation?.PlayAttack();
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
                presentation?.SetMoving(false);
                if (Time.time >= nextAttack)
                {
                    nextAttack = Time.time + attackInterval;
                    presentation?.PlayAttack();
                    GameManager.Instance.DamageBase(Mathf.Max(1, Mathf.RoundToInt(baseDamage * commanderDamageMultiplier)));
                    if (RuntimeEffects.Instance != null) RuntimeEffects.Instance.PlayHit(finalTarget.position, false);
                }
                return;
            }
        }

        Transform target = waypoints[waypointIndex];
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;
        bool moving = direction.sqrMagnitude > .001f;
        presentation?.SetMoving(moving);
        if (moving)
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
        if (Time.time < burnUntil && burnDps > 0f)
        {
            if (Time.time >= nextBurnVisual)
            {
                nextBurnVisual = Time.time + .24f;
                CombatImpactPresentation.BurnStatus(transform.position);
            }

            if (Time.time >= nextBurnTick)
            {
                nextBurnTick = Time.time + 1f;
                ReceiveDamage(new DamagePacket(burnDps, DamageType.Fire));
            }
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
        if (!IsAlive) return;
        float currentArmor = armor;
        if (Time.time < armorBreakUntil) currentArmor = Mathf.Max(0f, currentArmor - armorBreakAmount);

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
            (Archetype == EnemyArchetype.HeavyHoplite || Archetype == EnemyArchetype.ShieldBearer || Archetype == EnemyArchetype.BatteringRam)) bonus = 1.5f;
        if (packet.towerSource.HasValue && packet.towerSource.Value == TowerType.TrojanGuard &&
            (Archetype == EnemyArchetype.Infantry || Archetype == EnemyArchetype.Runner)) bonus = 1.25f;

        float finalDamage = packet.amount * bonus * (1f - Mathf.Clamp01(armorFactor));
        if (packet.towerSource.HasValue && packet.towerSource.Value == TowerType.MachineGun)
            finalDamage *= 1f - arrowResistance;

        Health -= Mathf.Max(1f, finalDamage);
        presentation?.PlayHit();
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
        nextBurnVisual = Mathf.Min(nextBurnVisual <= 0f ? Time.time : nextBurnVisual, Time.time);
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
        if (dying) return;
        dying = true;
        Health = 0f;
        ReleaseGuardReservation();
        EnemyRegistry.Unregister(this);

        if (GameManager.Instance != null)
        {
            if (Archetype == EnemyArchetype.Boss) GameManager.Instance.RecordBossDefeated();
            GameManager.Instance.RecordKill();
            GameManager.Instance.AddMoney(GameManager.Instance.RewardFor(reward));
        }

        if (healthBar != null) healthBar.enabled = false;
        foreach (Collider collider in GetComponentsInChildren<Collider>()) collider.enabled = false;
        CombatImpactPresentation.EnemyDeath(transform.position, Archetype);
        if (RuntimeEffects.Instance != null) RuntimeEffects.Instance.PlayDeath(transform.position, Archetype == EnemyArchetype.Boss);
        float presentationDelay = presentation != null ? presentation.PlayDeath(Archetype == EnemyArchetype.Boss) : .1f;
        Destroy(gameObject, Mathf.Max(.08f, presentationDelay));
    }

    void ReachBase()
    {
        ReleaseGuardReservation();
        presentation?.SetMoving(false);
        if (GameManager.Instance != null)
        {
            int damage = Mathf.Max(1, Mathf.RoundToInt(baseDamage * commanderDamageMultiplier));
            if (Archetype == EnemyArchetype.Boss)
            {
                attackingGate = true;
                waypointIndex = waypoints != null ? waypoints.Length : waypointIndex;
                GameManager.Instance.BossReachedGate(damage);
                RuntimeEffects.Instance?.PlayHit(transform.position, true);
                return;
            }
            else
            {
                GameManager.Instance.RecordLeak();
                GameManager.Instance.DamageBase(damage);
            }
        }
        if (RuntimeEffects.Instance != null) RuntimeEffects.Instance.PlayDeath(transform.position, Archetype == EnemyArchetype.Boss || Archetype == EnemyArchetype.BatteringRam);
        Destroy(gameObject);
    }

    void AttackGateOverTime()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameEnded) return;
        if (Time.time < nextAttack) return;
        nextAttack = Time.time + Mathf.Max(.65f, attackInterval);
        presentation?.PlayAttack();
        int damage = Mathf.Max(1, Mathf.RoundToInt(baseDamage * commanderDamageMultiplier));
        GameManager.Instance.BossReachedGate(damage);
        RuntimeEffects.Instance?.PlayHit(transform.position, true);
    }
}
