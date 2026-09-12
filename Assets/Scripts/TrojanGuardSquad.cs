using System.Collections.Generic;
using UnityEngine;

public class TrojanGuardSquad : MonoBehaviour
{
    static readonly List<TrojanGuardSquad> all = new List<TrojanGuardSquad>();
    public static IReadOnlyList<TrojanGuardSquad> All => all;

    public float maxHealth = 700f;
    public float blockRadius = 1.25f;
    public float damage = 34f;
    public float attackRate = 1.0f;
    public bool IsAlive => Health > 0f;
    public float Health { get; private set; }

    Tower ownerTower;
    float nextAttack;

    void OnEnable() { if (!all.Contains(this)) all.Add(this); }
    void OnDisable() => all.Remove(this);

    public void Initialize(Tower tower)
    {
        ownerTower = tower;
        Health = maxHealth;
    }

    void Update()
    {
        if (!IsAlive || GameManager.Instance == null || GameManager.Instance.GameEnded) return;
        Enemy target = null;
        float best = blockRadius * blockRadius;
        foreach (Enemy enemy in EnemyRegistry.All)
        {
            if (enemy == null) continue;
            float d = (enemy.transform.position - transform.position).sqrMagnitude;
            if (d <= best) { best = d; target = enemy; }
        }
        if (target != null)
        {
            target.SetBlockedByGuard(this);
            if (Time.time >= nextAttack)
            {
                nextAttack = Time.time + 1f / Mathf.Max(.01f, attackRate);
                target.ReceiveDamage(new DamagePacket(damage, DamageType.Physical, TowerType.TrojanGuard));
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (!IsAlive) return;
        Health -= Mathf.Max(1f, amount);
        if (Health <= 0f)
        {
            Health = 0f;
            RuntimeFileLogger.Event("GUARD", "Trojan Guard squad destroyed");
            if (ownerTower != null) ownerTower.DestroyWithoutRefund();
            else Destroy(gameObject);
        }
    }
}
