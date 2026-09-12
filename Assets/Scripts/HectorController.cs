using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    Vector3 destination;
    Vector3 spawnPosition;
    float nextAttack;
    float nextWarCry;
    float nextShieldWall;
    float nextSpearThrow;
    float nextUltimate;
    float reviveAt;
    Renderer body;

    void Awake() => Instance = this;

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        destination = transform.position;
        spawnPosition = transform.position;
        Health = maxHealth;
        body = GetComponentInChildren<Renderer>();
        RefreshColor();
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameEnded) return;

        if (IsDowned)
        {
            if (Time.time >= reviveAt) Revive();
            return;
        }

        HandleSelectionAndMove();
        Move();
        AutoAttack();

        if (!Selected) return;
        if (GameInput.Ability1Pressed()) UseWarCry();
        if (GameInput.Ability2Pressed()) UseShieldWall();
        if (GameInput.Ability3Pressed()) UseSpearThrow();
        if (GameInput.UltimatePressed()) UseUltimate();
    }

    void HandleSelectionAndMove()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        if (GameInput.PrimaryPressed() && (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject()))
        {
            Ray ray = cam.ScreenPointToRay(GameInput.PointerPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 200f))
            {
                HectorController h = hit.collider.GetComponentInParent<HectorController>();
                Selected = h == this;
                RefreshColor();
            }
        }

        if (Selected && GameInput.SecondaryPressed() && (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject()))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = cam.ScreenPointToRay(GameInput.PointerPosition);
            if (plane.Raycast(ray, out float enter))
            {
                destination = ray.GetPoint(enter);
                destination.y = transform.position.y;
            }
        }
    }

    void Move()
    {
        Vector3 delta = destination - transform.position;
        delta.y = 0f;
        if (delta.sqrMagnitude > .01f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(delta), 10f * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
    }

    void AutoAttack()
    {
        if (Time.time < nextAttack) return;
        Enemy best = FindNearestEnemy(attackRange);
        if (best == null) return;
        nextAttack = Time.time + 1f / attackRate;
        best.ReceiveDamage(new DamagePacket(attackDamage, DamageType.Hero));
        RuntimeEffects.Instance?.PlayHit(best.transform.position, false);
    }

    public void TakeDamage(float damage)
    {
        if (IsDowned || Health <= 0f) return;
        Health -= Mathf.Max(1f, damage);
        RuntimeEffects.Instance?.PlayHit(transform.position, damage >= 25f);
        if (Health <= 0f) Down();
    }

    void Down()
    {
        Health = 0f;
        IsDowned = true;
        Selected = false;
        reviveAt = Time.time + downedDuration;
        destination = transform.position;
        RefreshColor();
        RuntimeFileLogger.Event("HECTOR", $"Downed; reviveIn={downedDuration:0.0}s");
    }

    void Revive()
    {
        IsDowned = false;
        Health = maxHealth * .50f;
        transform.position = spawnPosition;
        destination = spawnPosition;
        RefreshColor();
        RuntimeFileLogger.Event("HECTOR", $"Revived hp={Health:0}/{maxHealth:0}");
    }

    public void UseWarCry()
    {
        if (IsDowned || Time.time < nextWarCry) return;
        nextWarCry = Time.time + warCryCooldown;
        int buffed = 0;
        foreach (Tower tower in TowerRegistry.All)
        {
            if (tower == null) continue;
            if (Vector3.Distance(tower.transform.position, transform.position) > warCryRadius) continue;
            tower.ApplyWarCry(warCryDuration, 1.15f, 1.30f);
            buffed++;
        }
        RuntimeFileLogger.Event("HECTOR", $"War Cry used; towersBuffed={buffed}");
        RuntimeEffects.Instance?.PlayHit(transform.position, true);
    }

    public void UseShieldWall()
    {
        if (IsDowned || Time.time < nextShieldWall) return;
        nextShieldWall = Time.time + shieldWallCooldown;

        Vector3 center = transform.position + transform.forward * 2.2f;
        GameObject root = new GameObject("Hector Shield Wall");
        root.transform.position = center;
        root.transform.rotation = transform.rotation;
        ShieldWallZone zone = root.AddComponent<ShieldWallZone>();

        for (int i = -1; i <= 1; i++)
        {
            GameObject shield = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shield.name = "Shield";
            shield.transform.SetParent(root.transform, false);
            shield.transform.localPosition = new Vector3(i * 1.05f, .55f, 0f);
            shield.transform.localScale = new Vector3(.9f, 1.1f, .28f);
            TowerFactory.SetColor(shield, new Color(.70f, .50f, .18f));
            Destroy(shield.GetComponent<Collider>());
        }

        Destroy(root, zone.duration + .1f);
        RuntimeFileLogger.Event("HECTOR", "Shield Wall used");
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
        RuntimeEffects.Instance?.PlayHit(target.transform.position, true);
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
        List<Enemy> snapshot = new List<Enemy>(EnemyRegistry.All);
        foreach (Enemy enemy in snapshot)
        {
            if (enemy == null) continue;
            if ((enemy.transform.position - transform.position).sqrMagnitude > radiusSq) continue;
            enemy.ReceiveDamage(new DamagePacket(100f, DamageType.Hero));
            if (enemy != null) enemy.ApplySlow(.70f, 4f);
            enemiesHit++;
        }

        RuntimeFileLogger.Event("HECTOR", $"For Troy ultimate used; towersBuffed={towersBuffed}, enemiesHit={enemiesHit}, hp={Health:0}/{maxHealth:0}");
        RuntimeEffects.Instance?.PlayHit(transform.position, true);
    }

    Enemy FindNearestEnemy(float radius)
    {
        Enemy best = null;
        float bestSq = radius * radius;
        foreach (Enemy enemy in EnemyRegistry.All)
        {
            if (enemy == null) continue;
            float d = (enemy.transform.position - transform.position).sqrMagnitude;
            if (d < bestSq) { bestSq = d; best = enemy; }
        }
        return best;
    }

    public float WarCryCooldownRemaining => Mathf.Max(0f, nextWarCry - Time.time);
    public float ShieldWallCooldownRemaining => Mathf.Max(0f, nextShieldWall - Time.time);
    public float SpearThrowCooldownRemaining => Mathf.Max(0f, nextSpearThrow - Time.time);
    public float UltimateCooldownRemaining => Mathf.Max(0f, nextUltimate - Time.time);

    void RefreshColor()
    {
        if (body == null) return;
        Color color = IsDowned ? new Color(.25f, .25f, .25f) : Selected ? new Color(.95f,.78f,.18f) : new Color(.72f,.48f,.12f);
        Material material = body.material;
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color")) material.SetColor("_Color", color);
    }
}
