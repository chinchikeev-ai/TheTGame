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
    LineRenderer outerRing;
    LineRenderer innerRing;
    Transform runeRoot;
    Material ringMaterial;

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
        Shader shader = Shader.Find("Sprites/Default");
        if (shader == null) shader = Shader.Find("Unlit/Color");
        ringMaterial = new Material(shader);
        ringMaterial.color = new Color(1f, .18f, .04f, .78f);

        outerRing = CreateRing("Menelaus Aura Outer", auraRadius, .09f, 72, new Color(1f,.20f,.04f,.90f));
        innerRing = CreateRing("Menelaus Aura Inner", auraRadius * .72f, .045f, 64, new Color(.92f,.48f,.08f,.72f));

        GameObject runes = new GameObject("Menelaus Aura Runes");
        runes.transform.SetParent(transform, false);
        runes.transform.localPosition = new Vector3(0f,.05f,0f);
        runeRoot = runes.transform;

        for (int i = 0; i < 8; i++)
        {
            float a = i * Mathf.PI * 2f / 8f;
            GameObject rune = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rune.name = "Commander Rune";
            rune.transform.SetParent(runeRoot, false);
            rune.transform.localPosition = new Vector3(Mathf.Cos(a) * auraRadius * .86f, 0f, Mathf.Sin(a) * auraRadius * .86f);
            rune.transform.localRotation = Quaternion.Euler(0f, -a * Mathf.Rad2Deg, 0f);
            rune.transform.localScale = new Vector3(.12f,.018f,.42f);
            Collider c = rune.GetComponent<Collider>();
            if (c != null) Destroy(c);
            TowerFactory.SetColor(rune, new Color(.95f,.25f,.045f,.80f));
        }
    }

    LineRenderer CreateRing(string name, float radius, float width, int segments, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(transform, false);
        go.transform.localPosition = new Vector3(0f,.055f,0f);
        LineRenderer line = go.AddComponent<LineRenderer>();
        line.loop = true;
        line.useWorldSpace = false;
        line.positionCount = segments;
        line.startWidth = width;
        line.endWidth = width;
        line.material = ringMaterial;
        line.startColor = color;
        line.endColor = color;
        line.numCornerVertices = 2;
        for (int i=0;i<segments;i++)
        {
            float a = i * Mathf.PI * 2f / segments;
            line.SetPosition(i,new Vector3(Mathf.Cos(a)*radius,0f,Mathf.Sin(a)*radius));
        }
        return line;
    }

    void UpdateAuraVisual()
    {
        float t = Time.unscaledTime;
        float pulse = 1f + Mathf.Sin(t * 3.4f) * .035f;
        if (outerRing != null) outerRing.transform.localScale = Vector3.one * pulse;
        if (innerRing != null)
        {
            innerRing.transform.localScale = Vector3.one * (1f - Mathf.Sin(t * 3.1f) * .025f);
            innerRing.transform.localRotation = Quaternion.Euler(0f,t*18f,0f);
        }
        if (runeRoot != null)
        {
            runeRoot.localRotation = Quaternion.Euler(0f,-t*24f,0f);
            runeRoot.localScale = Vector3.one * (1f + Mathf.Sin(t*4.2f)*.025f);
        }
    }

    void OnDestroy()
    {
        if (ringMaterial != null) Destroy(ringMaterial);
        if (!Application.isPlaying) return;
        if (GameManager.Instance != null && GameManager.Instance.BossDefeated)
            RuntimeFileLogger.Event("BOSS", "Menelaus defeated");
        else if (GameManager.Instance != null && GameManager.Instance.BossBreached)
            RuntimeFileLogger.Event("BOSS", "Menelaus breached Troy");
        else
            RuntimeFileLogger.Event("BOSS", "Menelaus removed before encounter resolution");
    }
}
