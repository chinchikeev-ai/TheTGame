using UnityEngine;

public class Tower : MonoBehaviour
{
    public float range = 5.5f;
    public float damage = 34f;
    public float fireRate = 1.4f;
    public float projectileSpeed = 12f;
    public Transform head;
    public Transform muzzle;

    float nextFireTime;

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

    void Fire(Enemy target)
    {
        Vector3 start = muzzle != null ? muzzle.position : (head != null ? head.position + head.forward * 0.9f : transform.position + Vector3.up);
        GameObject projectileObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectileObj.name = "Projectile";
        projectileObj.transform.position = start;
        projectileObj.transform.localScale = Vector3.one * 0.18f;
        Destroy(projectileObj.GetComponent<Collider>());
        SetProjectileColor(projectileObj);

        Projectile projectile = projectileObj.AddComponent<Projectile>();
        projectile.Init(target, damage, projectileSpeed);
    }

    static void SetProjectileColor(GameObject obj)
    {
        Renderer r = obj.GetComponent<Renderer>();
        if (r == null) return;
        Material m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (m.shader == null) m = new Material(Shader.Find("Standard"));
        m.color = new Color(1f, 0.72f, 0.12f);
        m.EnableKeyword("_EMISSION");
        m.SetColor("_EmissionColor", new Color(1f, 0.35f, 0.02f) * 1.4f);
        r.material = m;
    }
}
