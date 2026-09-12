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
    public int blockCapacity = 3;
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
        Enemy attackTarget = null;
        float best = float.MaxValue;
        int blocked = 0;
        float radiusSq = blockRadius * blockRadius;

        foreach (Enemy enemy in EnemyRegistry.All)
        {
            if (enemy == null) continue;
            float d = (enemy.transform.position - transform.position).sqrMagnitude;
            if (d > radiusSq) continue;

            if (blocked < blockCapacity)
            {
                enemy.SetBlockedByGuard(this);
                blocked++;
            }
            if (d < best)
            {
                best = d;
                attackTarget = enemy;
            }
        }

        if (attackTarget != null && Time.time >= nextAttack)
        {
            nextAttack = Time.time + 1f / Mathf.Max(.01f, attackRate);
            attackTarget.ReceiveDamage(new DamagePacket(damage, DamageType.Physical, TowerType.TrojanGuard));
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
