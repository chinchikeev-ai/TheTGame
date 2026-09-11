using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    Text statsText;
    Text waveText;
    Text nextWaveText;
    Text helpText;
    Text endText;
    EnemySpawner spawner;

    void Start()
    {
        spawner = FindFirstObjectByType<EnemySpawner>();
        BuildUI();
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();

        statsText.text = $"COINS  {GameManager.Instance.Money}    BASE  {GameManager.Instance.BaseHealth}";
        waveText.text = $"WAVE  {GameManager.Instance.CurrentWave}/{GameManager.Instance.MaxWaves}";

        if (spawner != null)
        {
            if (spawner.WaveActive)
                nextWaveText.text = $"WAVE ACTIVE  •  NEXT: {spawner.NextWaveEnemyCount} enemies  •  HP x{spawner.NextWaveHpMultiplier:0.00}  •  SPD x{spawner.NextWaveSpeedMultiplier:0.00}";
            else if (spawner.InterWaveCountdown > 0f)
                nextWaveText.text = $"NEXT WAVE IN {Mathf.CeilToInt(spawner.InterWaveCountdown)}s  •  {spawner.NextWaveEnemyCount} enemies  •  HP x{spawner.NextWaveHpMultiplier:0.00}  •  SPD x{spawner.NextWaveSpeedMultiplier:0.00}";
            else
                nextWaveText.text = $"NEXT WAVE  •  {spawner.NextWaveEnemyCount} enemies";
        }

        endText.gameObject.SetActive(GameManager.Instance.GameEnded);
        if (GameManager.Instance.GameEnded)
            endText.text = GameManager.Instance.EndMessage;
    }

    void BuildUI()
    {
        GameObject canvasObj = new GameObject("GameCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObj.AddComponent<GraphicRaycaster>();

        statsText = CreateText(canvas.transform, "Stats", new Vector2(24, -22), new Vector2(620, 48), 30, TextAnchor.UpperLeft);
        waveText = CreateText(canvas.transform, "Wave", new Vector2(24, -68), new Vector2(360, 42), 24, TextAnchor.UpperLeft);

        nextWaveText = CreateText(canvas.transform, "NextWave", new Vector2(0, -22), new Vector2(900, 48), 22, TextAnchor.UpperCenter);
        RectTransform nextRt = nextWaveText.rectTransform;
        nextRt.anchorMin = new Vector2(0.5f, 1f);
        nextRt.anchorMax = new Vector2(0.5f, 1f);
        nextRt.pivot = new Vector2(0.5f, 1f);

        helpText = CreateText(canvas.transform, "Help", new Vector2(-24, 24), new Vector2(620, 80), 20, TextAnchor.LowerRight);
        RectTransform helpRt = helpText.rectTransform;
        helpRt.anchorMin = new Vector2(1f, 0f);
        helpRt.anchorMax = new Vector2(1f, 0f);
        helpRt.pivot = new Vector2(1f, 0f);
        helpText.text = "WASD / ARROWS — PAN    MOUSE WHEEL — ZOOM\nCLICK GREEN BUILD POINT — BUILD TOWER ($100)";

        endText = CreateText(canvas.transform, "End", Vector2.zero, new Vector2(900, 180), 64, TextAnchor.MiddleCenter);
        RectTransform endRt = endText.rectTransform;
        endRt.anchorMin = new Vector2(0.5f, 0.5f);
        endRt.anchorMax = new Vector2(0.5f, 0.5f);
        endRt.pivot = new Vector2(0.5f, 0.5f);
        endText.gameObject.SetActive(false);
    }

    Text CreateText(Transform parent, string name, Vector2 pos, Vector2 size, int fontSize, TextAnchor alignment)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Text t = go.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = fontSize;
        t.fontStyle = FontStyle.Bold;
        t.color = Color.white;
        t.alignment = alignment;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;

        RectTransform rt = t.rectTransform;
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return t;
    }
}
