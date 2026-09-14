using System;
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
            if (waypointIndex >= waypoints.Length) return 1f;
            return Mathf.Clamp01((float)waypointIndex / waypoints.Length);
        }
    }

    readonly EnemyStatusState statuses = new EnemyStatusState();

    Transform[] waypoints;
    int waypointIndex;
    EnemyHealthBar healthBar;
    CharacterPresentationState presentation;
    CharacterWeaponSocketResolver weaponSockets;
    float baseSpeed;
    float armor;
    float arrowResistance;
    float attackRange;
    float attackInterval = 1.5f;
    float nextAttack;
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
        weaponSockets = GetComponent<CharacterWeaponSocketResolver>();
        if (weaponSockets == null) weaponSockets = gameObject.AddComponent<CharacterWeaponSocketResolver>();
        weaponSockets.Refresh();
        if (Archetype == EnemyArchetype.Archer) presentation.PrepareBow();
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

        TickBurn();
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

        statuses.RefreshMovementModifiers(Time.time);
        speed = baseSpeed * statuses.SpeedMultiplier;

        if (TryHandleGuardCombat()) return;
        if (TryHandleHectorCombat()) return;
        if (TryHandleRangedGateCombat()) return;

        MoveAlongPath();
    }

    bool TryHandleGuardCombat()
    {
        if (blockingGuard == null) return false;

        float releaseRadius = blockingGuard.blockRadius * 1.35f;
        bool reservationValid = blockingGuard.IsAlive && blockingGuard.OwnsReservation(this) &&
                                (transform.position - blockingGuard.transform.position).sqrMagnitude <= releaseRadius * releaseRadius;
        if (!reservationValid)
        {
            ReleaseGuardReservation();
            return false;
        }

        presentation?.SetMoving(false);
        if (Time.time >= nextAttack)
        {
            nextAttack = Time.time + attackInterval;
            TrojanGuardSquad targetGuard = blockingGuard;
            float damage = Mathf.Max(4f, baseDamage * 18f * statuses.DamageMultiplier);
            PlayCombatAttack(() =>
            {
                if (!IsAlive || targetGuard == null || !targetGuard.IsAlive || blockingGuard != targetGuard) return;
                if (Archetype == EnemyArchetype.Archer)
                {
                    LaunchArcherArrow(targetGuard.transform, Vector3.up * .75f, () =>
                    {
                        if (targetGuard == null || !targetGuard.IsAlive) return;
                        targetGuard.TakeDamage(damage);
                    });
                    return;
                }
                targetGuard.TakeDamage(damage);
            });
        }
        return true;
    }

    bool TryHandleHectorCombat()
    {
        HectorController hector = HectorController.Instance;
        if (hector == null || hector.IsDowned) return false;

        float meleeRange = 1.35f + transform.localScale.x * .25f;
        float rangedRange = Archetype == EnemyArchetype.Archer && attackRange > 0f ? attackRange : 0f;
        float allowedRange = Mathf.Max(meleeRange, rangedRange);
        if ((transform.position - hector.transform.position).sqrMagnitude > allowedRange * allowedRange) return false;

        presentation?.SetMoving(false);
        if (Time.time >= nextAttack)
        {
            nextAttack = Time.time + attackInterval;
            float damage = Mathf.Max(1f, baseDamage * 12f * statuses.DamageMultiplier);
            PlayCombatAttack(() =>
            {
                if (!IsAlive || hector == null || hector.IsDowned) return;
                if (Archetype == EnemyArchetype.Archer)
                {
                    LaunchArcherArrow(hector.transform, Vector3.up * .75f, () =>
                    {
                        if (hector == null || hector.IsDowned) return;
                        hector.TakeDamage(damage);
                    });
                    return;
                }
                hector.TakeDamage(damage);
            });
        }
        return true;
    }

    bool TryHandleRangedGateCombat()
    {
        if (Archetype != EnemyArchetype.Archer || attackRange <= 0f) return false;

        Transform finalTarget = waypoints[waypoints.Length - 1];
        if ((transform.position - finalTarget.position).sqrMagnitude > attackRange * attackRange) return false;

        presentation?.SetMoving(false);
        if (Time.time >= nextAttack)
        {
            nextAttack = Time.time + attackInterval;
            int damage = Mathf.Max(1, Mathf.RoundToInt(baseDamage * statuses.DamageMultiplier));
            PlayCombatAttack(() =>
            {
                if (!IsAlive || GameManager.Instance == null || GameManager.Instance.GameEnded || finalTarget == null) return;
                LaunchArcherArrow(finalTarget, Vector3.up * .55f, () =>
                {
                    if (GameManager.Instance == null || GameManager.Instance.GameEnded || finalTarget == null) return;
                    GameManager.Instance.DamageBase(damage);
                    RuntimeEffects.Instance?.PlayHitSound(false);
                    CombatImpactPresentation.GateHit(finalTarget.position, false);
                });
            });
        }
        return true;
    }

    void LaunchArcherArrow(Transform target, Vector3 targetOffset, Action impact)
    {
        if (!IsAlive || target == null) return;
        Vector3 start = weaponSockets != null
            ? weaponSockets.ArrowReleasePoint()
            : transform.TransformPoint(new Vector3(.18f, .95f, .28f));
        CombatFlightPresentation.SpawnArrow(start, target, targetOffset, _ => impact?.Invoke());
    }

    void PlayCombatAttack(Action impact)
    {
        if (presentation == null)
        {
            impact?.Invoke();
            return;
        }

        switch (Archetype)
        {
            case EnemyArchetype.Infantry:
            case EnemyArchetype.HeavyHoplite:
            case EnemyArchetype.ShieldBearer:
                presentation.PlaySpearAttack(impact);
                break;
            case EnemyArchetype.Archer:
                presentation.PlayBowShot(impact);
                break;
            default:
                presentation.PlayAttack(impact);
                break;
        }
    }

    void MoveAlongPath()
    {
        Transform target = waypoints[waypointIndex];
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;
        bool moving = direction.sqrMagnitude > .001f;
        presentation?.SetMoving(moving);

        if (moving)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 10f * Time.deltaTime);

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        if ((transform.position - target.position).sqrMagnitude > .0225f) return;

        waypointIndex++;
        if (waypointIndex >= waypoints.Length) ReachBase();
    }

    void TickBurn()
    {
        EnemyBurnTickResult tick = statuses.TickBurn(Time.time);
        if (tick.ShowVisual) CombatImpactPresentation.BurnStatus(transform.position);
        if (tick.Damage > 0f) ReceiveDamage(new DamagePacket(tick.Damage, DamageType.Fire));
    }

    public void TakeDamage(float damage) => ReceiveDamage(new DamagePacket(damage, DamageType.Physical));
    public void TakeDamage(float damage, TowerType sourceType) => ReceiveDamage(new DamagePacket(damage, DamageRules.ForTower(sourceType), sourceType));

    public void ReceiveDamage(DamagePacket packet)
    {
        if (!IsAlive) return;

        float currentArmor = statuses.CurrentArmor(armor, Time.time);
        Health -= EnemyDamageResolver.Resolve(packet, Archetype, currentArmor, arrowResistance);
        presentation?.PlayHit();
        healthBar?.Refresh();
        if (Health <= 0f) Die();
    }

    public void ApplySlow(float multiplier, float duration) => statuses.ApplySlow(multiplier, duration, Time.time);
    public void ApplyBurn(float damagePerSecond, float duration) => statuses.ApplyBurn(damagePerSecond, duration, Time.time);
    public void ApplyArmorBreak(float amount, float duration) => statuses.ApplyArmorBreak(amount, duration, Time.time);
    public void ApplyCommanderAura(float speedMultiplier, float damageMultiplier, float duration) =>
        statuses.ApplyCommanderAura(speedMultiplier, damageMultiplier, duration, Time.time);

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
        RuntimeEffects.Instance?.PlayDeathSound(Archetype == EnemyArchetype.Boss);
        float presentationDelay = presentation != null ? presentation.PlayDeath(Archetype == EnemyArchetype.Boss) : .1f;
        Destroy(gameObject, Mathf.Max(.08f, presentationDelay));
    }

    void ReachBase()
    {
        ReleaseGuardReservation();
        presentation?.SetMoving(false);

        if (GameManager.Instance != null)
        {
            int damage = Mathf.Max(1, Mathf.RoundToInt(baseDamage * statuses.DamageMultiplier));
            if (Archetype == EnemyArchetype.Boss)
            {
                attackingGate = true;
                waypointIndex = waypoints != null ? waypoints.Length : waypointIndex;
                GameManager.Instance.BossReachedGate(damage);
                RuntimeEffects.Instance?.PlayHitSound(true);
                CombatImpactPresentation.GateHit(transform.position, true);
                return;
            }

            GameManager.Instance.RecordLeak();
            GameManager.Instance.DamageBase(damage);
        }

        RuntimeEffects.Instance?.PlayDeathSound(Archetype == EnemyArchetype.BatteringRam);
        CombatImpactPresentation.EnemyBreach(transform.position, Archetype);
        Destroy(gameObject);
    }

    void AttackGateOverTime()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameEnded || Time.time < nextAttack) return;

        nextAttack = Time.time + Mathf.Max(.65f, attackInterval);
        int damage = Mathf.Max(1, Mathf.RoundToInt(baseDamage * statuses.DamageMultiplier));
        PlayCombatAttack(() =>
        {
            if (!IsAlive || GameManager.Instance == null || GameManager.Instance.GameEnded) return;
            GameManager.Instance.BossReachedGate(damage);
            RuntimeEffects.Instance?.PlayHitSound(true);
            CombatImpactPresentation.GateHit(transform.position, true);
        });
    }
}
