using UnityEngine;

public class Tower : MonoBehaviour
{
    public TowerType Type { get; private set; }
    public int Level { get; private set; } = 1;
    public int PurchaseCost { get; private set; }
    public BuildPoint OwnerPoint { get; set; }

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

    public void Configure(TowerType type, int purchaseCost)
    {
        Type = type;
        PurchaseCost = purchaseCost;
        ApplyBaseStats();
    }

    void ApplyBaseStats()
    {
        switch (Type)
        {
            case TowerType.MachineGun:
                range = 5.8f; damage = 16f; fireRate = 4.2f; projectileSpeed = 18f; splashRadius = 0f; slowMultiplier = 1f; slowDuration = 0f; break;
            case TowerType.Cannon:
                range = 6.8f; damage = 72f; fireRate = 0.75f; projectileSpeed = 10f; splashRadius = 2.2f; slowMultiplier = 1f; slowDuration = 0f; break;
            case TowerType.Slow:
                range = 5.3f; damage = 9f; fireRate = 1.5f; projectileSpeed = 13f; splashRadius = 0f; slowMultiplier = 0.55f; slowDuration = 1.6f; break;
        }
    }

    public int UpgradeCost => Level >= 3 ? 0 : Mathf.RoundToInt(PurchaseCost * (0.65f + Level * 0.35f));
    public int SellValue => Mathf.RoundToInt((PurchaseCost + TotalUpgradeInvestment()) * 0.65f);

    int TotalUpgradeInvestment()
    {
        if (Level <= 1) return 0;
        int total = Mathf.RoundToInt(PurchaseCost * 1.0f);
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
        if (slowMultiplier < 1f) slowMultiplier = Mathf.Max(0.35f, slowMultiplier - 0.08f);
        transform.localScale *= 1.06f;
        return true;
    }

    public void Sell()
    {
        if (GameManager.Instance != null) GameManager.Instance.AddMoney(SellValue);
        if (OwnerPoint != null) OwnerPoint.ClearTower(this);
        Destroy(gameObject);
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameEnded) return;
        Enemy target = FindTarget();
        if (target == null) return;
        RotateHead(target);
        if (Time.time >= nextFireTime)
        {
            Fire(target);
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void RotateHead(Enemy target)
    {
        if (head == null) return;
        Vector3 dir = target.transform.position - head.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            head.rotation = Quaternion.Slerp(head.rotation, Quaternion.LookRotation(dir), 12f * Time.deltaTime);
    }

    Enemy FindTarget()
    {
        Enemy closest = null;
        float best = range * range;
        foreach (Enemy enemy in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            float d = (enemy.transform.position - transform.position).sqrMagnitude;
            if (d < best) { best = d; closest = enemy; }
        }
        return closest;
    }

    void Fire(Enemy target)
    {
        Vector3 start = muzzle != null ? muzzle.position : transform.position + Vector3.up;
        if (RuntimeEffects.Instance != null) RuntimeEffects.Instance.PlayShot(Type, start);

        GameObject projectileObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectileObj.name = Type + "Projectile";
        projectileObj.transform.position = start;
        projectileObj.transform.localScale = Vector3.one * (Type == TowerType.Cannon ? 0.30f : 0.18f);
        Destroy(projectileObj.GetComponent<Collider>());
        TowerFactory.SetColor(projectileObj, Type == TowerType.Slow ? new Color(0.2f,0.75f,1f) : Type == TowerType.Cannon ? new Color(1f,0.35f,0.08f) : new Color(1f,0.78f,0.14f));
        Projectile projectile = projectileObj.AddComponent<Projectile>();
        projectile.Init(target, damage, projectileSpeed, splashRadius, slowMultiplier, slowDuration);
    }
}
