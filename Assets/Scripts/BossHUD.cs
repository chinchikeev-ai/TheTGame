using UnityEngine;
using UnityEngine.UI;

public class BossHUD : MonoBehaviour
{
    GameObject root;
    Text label;
    Image fill;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<BossHUD>() == null)
            new GameObject("BossHUD").AddComponent<BossHUD>();
    }

    void Start()
    {
        GameObject canvasObj = new GameObject("BossHUDCanvas");
        canvasObj.transform.SetParent(transform, false);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 30;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        root = new GameObject("BossBar");
        root.transform.SetParent(canvasObj.transform, false);
        Image bg = root.AddComponent<Image>();
        bg.color = new Color(.08f, .03f, .03f, .94f);
        RectTransform rr = bg.rectTransform;
        rr.anchorMin = rr.anchorMax = rr.pivot = new Vector2(.5f, 1f);
        rr.anchoredPosition = new Vector2(0, -86);
        rr.sizeDelta = new Vector2(720, 62);

        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(root.transform, false);
        fill = fillObj.AddComponent<Image>();
        fill.color = new Color(.72f, .10f, .08f, 1f);
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        RectTransform fr = fill.rectTransform;
        fr.anchorMin = new Vector2(0, 0); fr.anchorMax = new Vector2(1, 1);
        fr.offsetMin = new Vector2(8, 8); fr.offsetMax = new Vector2(-8, -8);

        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(root.transform, false);
        label = textObj.AddComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 22; label.fontStyle = FontStyle.Bold; label.color = Color.white; label.alignment = TextAnchor.MiddleCenter;
        RectTransform tr = label.rectTransform;
        tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one; tr.offsetMin = tr.offsetMax = Vector2.zero;
        root.SetActive(false);
    }

    void Update()
    {
        Enemy boss = null;
        foreach (Enemy enemy in EnemyRegistry.All)
            if (enemy != null && enemy.Archetype == EnemyArchetype.Boss) { boss = enemy; break; }

        root.SetActive(boss != null && boss.Health > 0f);
        if (boss == null) return;
        fill.fillAmount = boss.Health01;
        label.text = $"MENELAUS  {Mathf.CeilToInt(boss.Health)} / {Mathf.CeilToInt(boss.maxHealth)}";
    }
}
