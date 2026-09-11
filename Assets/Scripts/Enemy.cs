using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    public float speed = 2.2f;
    public int reward = 20;

    float health;
    Transform[] waypoints;
    int waypointIndex;

    public void Init(Transform[] path, float healthMultiplier = 1f, float speedMultiplier = 1f)
    {
        waypoints = path;
        maxHealth *= healthMultiplier;
        speed *= speedMultiplier;
        health = maxHealth;
        waypointIndex = 0;
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameEnded) return;
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[waypointIndex];
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 10f * Time.deltaTime);

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.15f)
        {
            waypointIndex++;
            if (waypointIndex >= waypoints.Length)
                ReachBase();
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f) Die();
    }

    void Die()
    {
        if (GameManager.Instance != null) GameManager.Instance.AddMoney(reward);
        Destroy(gameObject);
    }

    void ReachBase()
    {
        if (GameManager.Instance != null) GameManager.Instance.DamageBase(1);
        Destroy(gameObject);
    }
}
