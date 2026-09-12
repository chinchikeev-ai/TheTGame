using UnityEngine;

public class Tower : MonoBehaviour
{
    public TowerType Type { get; private set; }
    public string DisplayName { get; private set; }
    public int Level { get; private set; } = 1;
    public int PurchaseCost { get; private set; }
    public BuildPoint OwnerPoint { get; set; }
    public TargetPriority Priority { get; private set; } = TargetPriority.First;

    public float range = 5.5f;
    public float damage = 34f;
    public float fireRate = 1.4f;
    public float projectileSpeed = 12f;
    public float splashRadius;
    public float slowMultiplier = 1f;
    public float slowDuration;
    public Transform head;
    public Transform muzzle;

    float nextFireTime;
    float sellRatio = .65f;
    float warCryUntil;
    float warCryDamage = 1f;
    float warCryRate = 1f;
    TrojanGuardSquad guardSquad;

    void OnEnable() => TowerRegistry.Register(this);
    void OnDisable() => TowerRegistry.Unregister(this);

    public void Configure(TowerType type, int purchaseCost = 0)
    {
        Type = type;
        TowerData data = BalanceCatalog.GetTower(type);
        DisplayName = data.displayName;
        PurchaseCost = purchaseCost > 0 ? purchaseCost : data.cost;
        damage = data.damage;
        range = data.range;
        fireRate = data.attacksPerSecond;
        projectileSpeed = data.projectileSpeed;
        splashRadius = data.splashRadius;
        slowMultiplier = data.slowMultiplier;
        slowDuration = data.slowDuration;
        sellRatio = data.sellRatio;

        if (Type == TowerType.TrojanGuard)
        {
            guardSquad = gameObject.GetComponent<TrojanGuardSquad>();
            if (guardSquad == null) guardSquad = gameObject.AddComponent<TrojanGuardSquad>();
            guardSquad.damage = damage;
            guardSquad.attackRate = fireRate;
            guardSquad.Initialize(this);
        }
    }

    public int UpgradeCost => Level >= 3 ? 0 : Mathf.RoundToInt(PurchaseCost * (.65f + Level * .35f));
    public int SellValue => Mathf.RoundToInt((PurchaseCost + TotalUpgradeInvestment()) * sellRatio);

    int TotalUpgradeInvestment()
    {
        if (Level <= 1) return 0;
        int total = Mathf.RoundToInt(PurchaseCost * 1f);
        if (Level >= 3) total += Mathf.RoundToInt(PurchaseCost * 1.35f);
        return total;
    }

    public bool Upgrade()
    {
        if (Level >= 3 || GameManager.Instance == null) return false;
        int cost = UpgradeCost;
        if (!GameManager.Instance.SpendMoney(cost)) return false;
        Level++;
        range *= 1.10f;
        damage *= 1.32f;
        fireRate *= 1.12f;
        if (splashRadius > 0f) splashRadius *= 1.08f;
        if (slowMultiplier < 1f) slowMultiplier = Mathf.Max(.35f, slowMultiplier - .08f);
        if (guardSquad != null)
        {
            guardSquad.maxHealth *= 1.35f;
            guardSquad.damage = damage;
            guardSquad.attackRate = fireRate;
        }
        transform.localScale *= 1.06f;
        return true;
    }

    public void CyclePriority()
    {
        Priority = (TargetPriority)(((int)Priority + 1) % 5);
        RuntimeFileLogger.Event("TARGET", $"{DisplayName} priority={Priority}");
    }

    public void ApplyWarCry(float duration, float damageMultiplier = 1.15f, float rateMultiplier = 1.30f)
    {
        warCryUntil = Mathf.Max(warCryUntil, Time.time + duration);
        warCryDamage = Mathf.Max(warCryDamage, damageMultiplier);
        warCryRate = Mathf.Max(warCryRate, rateMultiplier);
    }

    public void Sell()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(SellValue);
            GameManager.Instance.RecordTowerSold();
        }
        DestroyWithoutRefund();
    }

    public void DestroyWithoutRefund()
    {
        if (OwnerPoint != null) OwnerPoint.ClearTower(this);
        Destroy(gameObject);
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameEnded) return;
        if (Type == TowerType.TrojanGuard) return;
        if (Time.time >= warCryUntil) { warCryDamage = 1f; warCryRate = 1f; }
        Enemy target = FindTarget();
        if (target == null) return;
        RotateHead(target);
        if (Time.time >= nextFireTime)
        {
            Fire(target);
            nextFireTime = Time.time + 1f / Mathf.Max(.01f, fireRate * warCryRate);
        }
    }

    void RotateHead(Enemy target)
    {
        if (head == null) return;
        Vector3 dir = target.transform.position - head.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > .001f) head.rotation = Quaternion.Slerp(head.rotation, Quaternion.LookRotation(dir), 12f * Time.deltaTime);
    }

    Enemy FindTarget()
    {
        Enemy bestEnemy = null;
        float bestScore = float.NegativeInfinity;
        foreach (Enemy enemy in EnemyRegistry.All)
        {
            if (enemy == null) continue;
            float distanceSq = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distanceSq > range * range) continue;

            float score;
            switch (Priority)
            {
                case TargetPriority.Last: score = -enemy.RouteProgress; break;
                case TargetPriority.Strongest: score = enemy.maxHealth; break;
                case TargetPriority.Weakest: score = -enemy.Health; break;
                case TargetPriority.Closest: score = -distanceSq; break;
                default: score = enemy.RouteProgress; break;
            }
            if (bestEnemy == null || score > bestScore)
            {
                bestScore = score;
                bestEnemy = enemy;
            }
        }
        return bestEnemy;
    }

    void Fire(Enemy target)
    {
        Vector3 start = muzzle != null ? muzzle.position : transform.position + Vector3.up;
        if (RuntimeEffects.Instance != null) RuntimeEffects.Instance.PlayShot(Type, start);
        GameObject projectileObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectileObj.name = DisplayName + " Projectile";
        projectileObj.transform.position = start;
        projectileObj.transform.localScale = Vector3.one * (Type == TowerType.Cannon ? .30f : .18f);
        Destroy(projectileObj.GetComponent<Collider>());
        TowerFactory.SetColor(projectileObj, Type == TowerType.Slow ? new Color(.2f,.75f,1f) : Type == TowerType.Cannon ? new Color(1f,.35f,.08f) : new Color(1f,.78f,.14f));
        Projectile projectile = projectileObj.AddComponent<Projectile>();
        projectile.Init(target, damage * warCryDamage, projectileSpeed, splashRadius, slowMultiplier, slowDuration, Type);
    }
}
