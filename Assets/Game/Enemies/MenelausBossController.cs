using UnityEngine;

public class MenelausBossController : MonoBehaviour
{
    public float auraRadius = 5.5f;
    public float auraSpeedMultiplier = 1.20f;
    public float auraDamageMultiplier = 1.35f;
    public float reinforcementInterval = 20f;
    public int reinforcementCount = 3;

    Enemy self;
    EnemySpawner spawner;
    float nextAuraPulse;
    float nextReinforcement;
    GameObject auraVisual;
    Renderer auraRenderer;

    void Start()
    {
        self = GetComponent<Enemy>();
        spawner = FindFirstObjectByType<EnemySpawner>();
        nextReinforcement = Time.time + 8f;
        BuildAuraVisual();
        RuntimeFileLogger.Event("BOSS", "Menelaus entered battle");
    }

    void Update()
    {
        if (self == null || self.Health <= 0f || GameManager.Instance == null || GameManager.Instance.GameEnded) return;

        UpdateAuraVisual();

        if (Time.time >= nextAuraPulse)
        {
            nextAuraPulse = Time.time + 0.5f;
            foreach (Enemy enemy in EnemyRegistry.All)
            {
                if (enemy == null || enemy == self) continue;
                if ((enemy.transform.position - transform.position).sqrMagnitude <= auraRadius * auraRadius)
                    enemy.ApplyCommanderAura(auraSpeedMultiplier, auraDamageMultiplier, 0.8f);
            }
        }

        if (Time.time >= nextReinforcement)
        {
            nextReinforcement = Time.time + reinforcementInterval;
            if (spawner != null)
            {
                spawner.SpawnMenelausReinforcements(reinforcementCount);
                RuntimeFileLogger.Event("BOSS", $"Menelaus called reinforcements count={reinforcementCount}");
            }
        }
    }

    void BuildAuraVisual()
    {
        auraVisual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        auraVisual.name = "MenelausCommanderAura";
        auraVisual.transform.SetParent(transform, false);
        auraVisual.transform.localPosition = new Vector3(0f, .035f, 0f);
        auraVisual.transform.localScale = new Vector3(auraRadius * 2f, .018f, auraRadius * 2f);
        Collider collider = auraVisual.GetComponent<Collider>();
        if (collider != null) Destroy(collider);
        auraRenderer = auraVisual.GetComponent<Renderer>();
        TowerFactory.SetColor(auraVisual, new Color(.78f, .12f, .05f, .20f));
    }

    void UpdateAuraVisual()
    {
        if (auraVisual == null || auraRenderer == null) return;
        float pulse = 1f + Mathf.Sin(Time.unscaledTime * 3.4f) * .035f;
        auraVisual.transform.localScale = new Vector3(auraRadius * 2f * pulse, .018f, auraRadius * 2f * pulse);

        Material material = auraRenderer.material;
        Color color = Color.Lerp(new Color(.58f, .07f, .035f, .16f), new Color(1f, .22f, .06f, .28f), (Mathf.Sin(Time.unscaledTime * 3.4f) + 1f) * .5f);
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        if (material.HasProperty("_Color")) material.SetColor("_Color", color);
    }

    void OnDestroy()
    {
        if (!Application.isPlaying) return;
        if (GameManager.Instance != null && GameManager.Instance.BossDefeated)
            RuntimeFileLogger.Event("BOSS", "Menelaus defeated");
        else if (GameManager.Instance != null && GameManager.Instance.BossBreached)
            RuntimeFileLogger.Event("BOSS", "Menelaus breached Troy");
        else
            RuntimeFileLogger.Event("BOSS", "Menelaus removed before encounter resolution");
    }
}
