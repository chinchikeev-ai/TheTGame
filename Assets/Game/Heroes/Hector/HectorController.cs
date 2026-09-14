using System.Collections.Generic;
using UnityEngine;

public enum HectorCombatAction
{
    None,
    BasicAttack,
    WarCry,
    ShieldWall,
    SpearThrow,
    Ultimate,
    Downed
}

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
    public float combatTurnSpeed = 900f;
    public float basicAttackActionLock = .58f;
    public float warCryRadius = 5.2f;
    public float warCryDuration = 10f;
    public float warCryCooldown = 20f;
    public float warCryActionLock = .72f;
    public float shieldWallCooldown = 22f;
    public float shieldWallDuration = 4f;
    public float shieldWallDamageMultiplier = .55f;
    public float shieldWallFrontalDot = .05f;
    public float shieldWallActionLock = .72f;
    public float spearThrowCooldown = 12f;
    public float spearThrowRange = 10f;
    public float spearThrowDamage = 220f;
    public float spearThrowActionLock = .82f;
    public float ultimateCooldown = 75f;
    public float ultimateDuration = 12f;
    public float ultimateEnemyRadius = 8f;
    public float ultimateDamage = 100f;
    public float ultimateActionLock = .95f;
    public float downedDuration = 12f;
    public float battlefieldMargin = .65f;

    readonly List<Enemy> enemySnapshot = new List<Enemy>(64);
    readonly List<Vector3> roadMovePoints = new List<Vector3>(16);
    Transform[][] movementRoutes;
    int roadMoveIndex;
    Vector3 destination;
    Vector3 spawnPosition;
    Vector3 actionFacingPoint;
    float nextAttack;
    float nextWarCry;
    float nextShieldWall;
    float shieldWallUntil;
    float nextSpearThrow;
    float nextUltimate;
    float reviveAt;
    float actionLockUntil;
    int actionSerial;
    bool hasActionFacingPoint;
    HectorPresentationBridge presentation;

    public bool ShieldWallActive => !IsDowned && Time.time < shieldWallUntil;
    public HectorCombatAction CurrentAction { get; private set; } = HectorCombatAction.None;
    public bool IsActionLocked => !IsDowned && CurrentAction != HectorCombatAction.None && Time.time < actionLockUntil;
    public bool CanMove => !IsDowned && !IsActionLocked && !ShieldWallActive;
    public bool CanAcceptCombatCommand => !IsDowned && !IsActionLocked && !ShieldWallActive;
    public bool RouteMovementEnabled => HectorRouteNavigator.HasRoutes(movementRoutes);
    public Vector3 MoveDestination => destination;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnDestroy() { if (Instance == this) Instance = null; }

    public void ConfigureMovementRoutes(Transform[][] routes, bool moveToGateStart)
    {
        movementRoutes = routes;
        if (!RouteMovementEnabled) return;

        float height = transform.position.y;
        Vector3 start = moveToGateStart
            ? HectorRouteNavigator.GateStart(movementRoutes, height)
            : HectorRouteNavigator.ProjectToNearestRoute(transform.position, movementRoutes, height);
        transform.position = start;
        destination = start;
        spawnPosition = start;
        roadMovePoints.Clear();
        roadMoveIndex = 0;
    }

    void Start()
    {
        if (RouteMovementEnabled)
            transform.position = HectorRouteNavigator.ProjectToNearestRoute(transform.position, movementRoutes, transform.position.y);
        else
            transform.position = MapBuilder.ClampToPlayableArea(transform.position, battlefieldMargin);
        destination = transform.position;
        spawnPosition = transform.position;
        Health = maxHealth;
        presentation = GetComponent<HectorPresentationBridge>();
        presentation?.Initialize(this);
        presentation?.SetSelected(Selected);
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameEnded) return;
        if (IsDowned) { if (Time.time >= reviveAt) Revive(); return; }
        RefreshActionState();
        UpdateActionFacing();
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

    public Vector3 ConstrainMoveDestination(Vector3 worldPosition)
    {
        if (RouteMovementEnabled)
            return HectorRouteNavigator.ProjectToNearestRoute(worldPosition, movementRoutes, transform.position.y);
        worldPosition.y = transform.position.y;
        return MapBuilder.ClampToPlayableArea(worldPosition, battlefieldMargin);
    }

    public void MoveTo(Vector3 worldPosition)
    {
        if (!CanMove) return;

        if (RouteMovementEnabled)
        {
            Vector3 requested = ConstrainMoveDestination(worldPosition);
            if (!HectorRouteNavigator.BuildPath(transform.position, requested, movementRoutes, transform.position.y, roadMovePoints))
                return;
            roadMoveIndex = 0;
            destination = roadMovePoints[0];
            return;
        }

        destination = ConstrainMoveDestination(worldPosition);
    }

    void Move()
    {
        if (!CanMove) return;

        if (RouteMovementEnabled)
        {
            if (roadMoveIndex >= roadMovePoints.Count) return;
            destination = roadMovePoints[roadMoveIndex];
            MoveTowardsDestination();
            Vector3 remaining = destination - transform.position;
            remaining.y = 0f;
            if (remaining.sqrMagnitude <= .01f)
            {
                transform.position = destination;
                roadMoveIndex++;
                if (roadMoveIndex < roadMovePoints.Count) destination = roadMovePoints[roadMoveIndex];
            }
            return;
        }

        destination = MapBuilder.ClampToPlayableArea(destination, battlefieldMargin);
        MoveTowardsDestination();
        transform.position = MapBuilder.ClampToPlayableArea(transform.position, battlefieldMargin);
    }

    void MoveTowardsDestination()
    {
        Vector3 delta = destination - transform.position;
        delta.y = 0f;
        if (delta.sqrMagnitude > .01f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(delta), 10f * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
    }

    void AutoAttack()
    {
        if (!CanAcceptCombatCommand || Time.time < nextAttack) return;
        if (presentation != null && !presentation.SpearAvailable) return;
        Enemy best = FindNearestEnemy(attackRange);
        if (best == null) return;
        if (!TryBeginAction(HectorCombatAction.BasicAttack, basicAttackActionLock, best.transform.position, out int token)) return;
        nextAttack = Time.time + 1f / Mathf.Max(.01f, attackRate);
        Vector3 impactPoint = best.transform.position;
        if (presentation == null) { ApplyBasicAttackImpact(token, best); return; }
        presentation.PlayAttackImpact(impactPoint, () =>
        {
            if (IsDowned || best == null || !best.IsAlive) return;
            ApplyBasicAttackImpact(token, best);
        });
    }

    void ApplyBasicAttackImpact(int token, Enemy target)
    {
        if (!ActionTokenValid(token) || target == null || !target.IsAlive) return;
        float multiplier = GameManager.Instance != null ? GameManager.Instance.PlayerDamageMultiplier : 1f;
        target.ReceiveDamage(new DamagePacket(attackDamage * multiplier, DamageType.Hero));
    }

    public void TakeDamage(float damage) => ApplyIncomingDamage(damage, null);
    public void TakeDamage(float damage, Vector3 sourcePoint) => ApplyIncomingDamage(damage, sourcePoint);

    void ApplyIncomingDamage(float damage, Vector3? sourcePoint)
    {
        if (IsDowned || Health <= 0f) return;
        float incoming = Mathf.Max(1f, damage);
        bool blocked = ShieldWallActive && (!sourcePoint.HasValue || IsFacingSource(sourcePoint.Value));
        float applied = blocked ? incoming * Mathf.Clamp(shieldWallDamageMultiplier, .05f, 1f) : incoming;
        Health -= applied;
        if (blocked) presentation?.PlayShieldBlockImpact(incoming);
        else presentation?.PlayDamageImpact(applied);
        if (Health <= 0f) Down();
    }

    bool IsFacingSource(Vector3 sourcePoint)
    {
        Vector3 direction = sourcePoint - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude <= .001f) return true;
        return Vector3.Dot(transform.forward, direction.normalized) >= shieldWallFrontalDot;
    }

    void Down()
    {
        Health = 0f;
        shieldWallUntil = 0f;
        CancelActionState(HectorCombatAction.Downed);
        IsDowned = true;
        SetSelected(false);
        reviveAt = Time.time + downedDuration;
        destination = transform.position;
        roadMovePoints.Clear();
        presentation?.SetDownedState(true);
        RuntimeFileLogger.Event("HECTOR", $"Downed; reviveIn={downedDuration:0.0}s");
    }

    void Revive()
    {
        actionSerial++;
        CurrentAction = HectorCombatAction.None;
        actionLockUntil = 0f;
        hasActionFacingPoint = false;
        IsDowned = false;
        Health = maxHealth * .50f;
        transform.position = RouteMovementEnabled
            ? HectorRouteNavigator.ProjectToNearestRoute(spawnPosition, movementRoutes, spawnPosition.y)
            : MapBuilder.ClampToPlayableArea(spawnPosition, battlefieldMargin);
        destination = transform.position;
        roadMovePoints.Clear();
        roadMoveIndex = 0;
        presentation?.SetDownedState(false);
        presentation?.PlayReviveEffect();
        RuntimeFileLogger.Event("HECTOR", $"Revived hp={Health:0}/{maxHealth:0}");
    }

    public void UseWarCry()
    {
        if (Time.time < nextWarCry) return;
        if (!TryBeginAction(HectorCombatAction.WarCry, warCryActionLock, null, out int token)) return;
        nextWarCry = Time.time + warCryCooldown;
        if (presentation == null) ApplyWarCryImpact(token);
        else presentation.PlayWarCryImpact(() => ApplyWarCryImpact(token));
    }

    void ApplyWarCryImpact(int token)
    {
        if (!ActionTokenValid(token)) return;
        int buffed = 0;
        float radiusSq = warCryRadius * warCryRadius;
        foreach (Tower tower in TowerRegistry.All)
        {
            if (tower == null || (tower.transform.position - transform.position).sqrMagnitude > radiusSq) continue;
            tower.ApplyWarCry(warCryDuration, 1.15f, 1.30f);
            buffed++;
        }
        RuntimeFileLogger.Event("HECTOR", $"War Cry used; towersBuffed={buffed}");
        presentation?.PlayWarCryEffect(warCryRadius);
    }

    public void UseShieldWall()
    {
        if (Time.time < nextShieldWall) return;
        if (!TryBeginAction(HectorCombatAction.ShieldWall, shieldWallActionLock, null, out int token)) return;
        nextShieldWall = Time.time + shieldWallCooldown;
        if (presentation == null) ApplyShieldWallImpact(token);
        else presentation.PlayShieldWallImpact(() => ApplyShieldWallImpact(token));
    }

    void ApplyShieldWallImpact(int token)
    {
        if (!ActionTokenValid(token)) return;
        float duration = Mathf.Max(.2f, shieldWallDuration);
        shieldWallUntil = Time.time + duration;
        Vector3 center = transform.position + transform.forward * 2.2f;
        HectorShieldWallFactory.Create(center, transform.rotation, duration);
        presentation?.PlayShieldWallEffect(center, duration);
        RuntimeFileLogger.Event("HECTOR", $"Shield Wall used; duration={duration:0.0}s; damageMultiplier={shieldWallDamageMultiplier:0.00}");
    }

    public void UseSpearThrow()
    {
        if (Time.time < nextSpearThrow) return;
        if (presentation != null && !presentation.SpearAvailable) return;
        Enemy target = FindNearestEnemy(spearThrowRange);
        if (target == null) return;
        if (!TryBeginAction(HectorCombatAction.SpearThrow, spearThrowActionLock, target.transform.position, out int token)) return;
        nextSpearThrow = Time.time + spearThrowCooldown;
        Vector3 impactPoint = target.transform.position;
        if (presentation == null) { ApplySpearThrowFallback(token, target); return; }
        presentation.PlaySpearImpact(impactPoint, () =>
        {
            if (IsDowned || target == null || !target.IsAlive) return;
            if (!ActionTokenValid(token)) return;
            presentation.LaunchSpearFlight(target.transform, point =>
            {
                if (target == null || !target.IsAlive) return;
                float multiplier = GameManager.Instance != null ? GameManager.Instance.PlayerDamageMultiplier : 1f;
                target.ReceiveDamage(new DamagePacket(spearThrowDamage * multiplier, DamageType.Hero));
                target.ApplyArmorBreak(.25f, 6f);
                RuntimeFileLogger.Event("HECTOR", $"Spear Throw hit {target.name} damage={spearThrowDamage * multiplier:0}; impact={point}");
            });
        });
    }

    void ApplySpearThrowFallback(int token, Enemy target)
    {
        if (!ActionTokenValid(token) || target == null || !target.IsAlive) return;
        float multiplier = GameManager.Instance != null ? GameManager.Instance.PlayerDamageMultiplier : 1f;
        target.ReceiveDamage(new DamagePacket(spearThrowDamage * multiplier, DamageType.Hero));
        target.ApplyArmorBreak(.25f, 6f);
        RuntimeFileLogger.Event("HECTOR", $"Spear Throw hit {target.name} damage={spearThrowDamage * multiplier:0}");
    }

    public void UseUltimate()
    {
        if (Time.time < nextUltimate) return;
        if (!TryBeginAction(HectorCombatAction.Ultimate, ultimateActionLock, null, out int token)) return;
        nextUltimate = Time.time + ultimateCooldown;
        if (presentation == null) ApplyUltimateImpact(token);
        else presentation.PlayUltimateImpact(() => ApplyUltimateImpact(token));
    }

    void ApplyUltimateImpact(int token)
    {
        if (!ActionTokenValid(token)) return;
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
        float multiplier = GameManager.Instance != null ? GameManager.Instance.PlayerDamageMultiplier : 1f;
        enemySnapshot.Clear();
        foreach (Enemy enemy in EnemyRegistry.All)
            if (enemy != null && enemy.IsAlive) enemySnapshot.Add(enemy);
        for (int i = 0; i < enemySnapshot.Count; i++)
        {
            Enemy enemy = enemySnapshot[i];
            if (enemy == null || !enemy.IsAlive || (enemy.transform.position - transform.position).sqrMagnitude > radiusSq) continue;
            enemy.ReceiveDamage(new DamagePacket(ultimateDamage * multiplier, DamageType.Hero));
            enemy.ApplySlow(.70f, 4f);
            enemiesHit++;
        }
        enemySnapshot.Clear();
        RuntimeFileLogger.Event("HECTOR", $"For Troy ultimate used; towersBuffed={towersBuffed}, enemiesHit={enemiesHit}, hp={Health:0}/{maxHealth:0}");
        presentation?.PlayUltimateEffect(ultimateEnemyRadius);
    }

    bool TryBeginAction(HectorCombatAction action, float lockDuration, Vector3? facingPoint, out int token)
    {
        token = actionSerial;
        if (!CanAcceptCombatCommand) return false;
        actionSerial++;
        token = actionSerial;
        CurrentAction = action;
        actionLockUntil = Time.time + Mathf.Max(.05f, lockDuration);
        hasActionFacingPoint = facingPoint.HasValue;
        if (facingPoint.HasValue) actionFacingPoint = facingPoint.Value;
        UpdateActionFacing();
        return true;
    }

    bool ActionTokenValid(int token) => !IsDowned && token == actionSerial;

    void RefreshActionState()
    {
        if (CurrentAction == HectorCombatAction.None || CurrentAction == HectorCombatAction.Downed || Time.time < actionLockUntil) return;
        CurrentAction = HectorCombatAction.None;
        actionLockUntil = 0f;
        hasActionFacingPoint = false;
    }

    void CancelActionState(HectorCombatAction nextState)
    {
        actionSerial++;
        CurrentAction = nextState;
        actionLockUntil = 0f;
        hasActionFacingPoint = false;
    }

    void UpdateActionFacing()
    {
        if (!hasActionFacingPoint || IsDowned) return;
        Vector3 direction = actionFacingPoint - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude <= .001f) return;
        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Mathf.Max(0f, combatTurnSpeed) * Time.deltaTime);
    }

    Enemy FindNearestEnemy(float radius)
    {
        Enemy best = null;
        float bestSq = radius * radius;
        foreach (Enemy enemy in EnemyRegistry.All)
        {
            if (enemy == null || !enemy.IsAlive) continue;
            float distanceSq = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distanceSq >= bestSq) continue;
            bestSq = distanceSq;
            best = enemy;
        }
        return best;
    }

    public float ActionLockRemaining => IsActionLocked ? Mathf.Max(0f, actionLockUntil - Time.time) : 0f;
    public float WarCryCooldownRemaining => Mathf.Max(0f, nextWarCry - Time.time);
    public float ShieldWallCooldownRemaining => Mathf.Max(0f, nextShieldWall - Time.time);
    public float ShieldWallRemaining => ShieldWallActive ? Mathf.Max(0f, shieldWallUntil - Time.time) : 0f;
    public float SpearThrowCooldownRemaining => Mathf.Max(0f, nextSpearThrow - Time.time);
    public float UltimateCooldownRemaining => Mathf.Max(0f, nextUltimate - Time.time);
}
