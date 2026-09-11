using UnityEngine;

public class Tower : MonoBehaviour
{
    public float range = 5.5f;
    public float damage = 34f;
    public float fireRate = 1.4f;
    public Transform head;

    float nextFireTime;

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameEnded) return;

        Enemy target = FindTarget();
        if (target == null) return;

        if (head != null)
        {
            Vector3 dir = target.transform.position - head.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
                head.rotation = Quaternion.Slerp(head.rotation, Quaternion.LookRotation(dir), 12f * Time.deltaTime);
        }

        if (Time.time >= nextFireTime)
        {
            target.TakeDamage(damage);
            nextFireTime = Time.time + 1f / fireRate;
            CreateTracer(target.transform.position + Vector3.up * 0.8f);
        }
    }

    Enemy FindTarget()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy closest = null;
        float best = range * range;

        foreach (Enemy enemy in enemies)
        {
            float d = (enemy.transform.position - transform.position).sqrMagnitude;
            if (d < best)
            {
                best = d;
                closest = enemy;
            }
        }
        return closest;
    }

    void CreateTracer(Vector3 target)
    {
        Vector3 start = head != null ? head.position + head.forward * 0.7f : transform.position + Vector3.up;
        GameObject tracer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tracer.name = "Tracer";
        Collider c = tracer.GetComponent<Collider>();
        if (c != null) Destroy(c);

        Vector3 mid = (start + target) * 0.5f;
        Vector3 delta = target - start;
        tracer.transform.position = mid;
        tracer.transform.rotation = Quaternion.LookRotation(delta);
        tracer.transform.localScale = new Vector3(0.05f, 0.05f, delta.magnitude);
        Destroy(tracer, 0.06f);
    }
}
