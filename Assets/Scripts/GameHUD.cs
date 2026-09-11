using UnityEngine;

public class GameHUD : MonoBehaviour
{
    GUIStyle labelStyle;
    GUIStyle buttonStyle;
    GUIStyle centerStyle;

    void InitStyles()
    {
        labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 22, fontStyle = FontStyle.Bold };
        labelStyle.normal.textColor = Color.white;
        buttonStyle = new GUIStyle(GUI.skin.box) { fontSize = 18, alignment = TextAnchor.MiddleCenter };
        centerStyle = new GUIStyle(labelStyle) { fontSize = 42, alignment = TextAnchor.MiddleCenter };
    }

    void OnGUI()
    {
        if (labelStyle == null) InitStyles();
        if (GameManager.Instance == null) return;

        GUI.Box(new Rect(15, 15, 410, 95), "");
        GUI.Label(new Rect(30, 25, 180, 30), $"COINS: {GameManager.Instance.Money}", labelStyle);
        GUI.Label(new Rect(220, 25, 180, 30), $"BASE: {GameManager.Instance.BaseHealth}", labelStyle);
        GUI.Label(new Rect(30, 65, 220, 30), $"WAVE: {GameManager.Instance.CurrentWave}/{GameManager.Instance.MaxWaves}", labelStyle);
        GUI.Box(new Rect(Screen.width - 260, 15, 245, 55), "CLICK GROUND: BUILD TOWER ($100)", buttonStyle);

        if (GameManager.Instance.GameEnded)
        {
            GUI.Box(new Rect(Screen.width / 2f - 220, Screen.height / 2f - 75, 440, 150), "");
            GUI.Label(new Rect(Screen.width / 2f - 200, Screen.height / 2f - 35, 400, 70), GameManager.Instance.EndMessage, centerStyle);
        }
    }
}
