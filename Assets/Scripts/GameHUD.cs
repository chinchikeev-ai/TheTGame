using UnityEngine;

public class GameHUD : MonoBehaviour
{
    GUIStyle labelStyle;
    GUIStyle smallStyle;
    GUIStyle boxStyle;
    GUIStyle centerStyle;

    void InitStyles()
    {
        labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 22, fontStyle = FontStyle.Bold };
        labelStyle.normal.textColor = Color.white;
        smallStyle = new GUIStyle(labelStyle) { fontSize = 15, fontStyle = FontStyle.Normal };
        boxStyle = new GUIStyle(GUI.skin.box) { fontSize = 18, alignment = TextAnchor.MiddleCenter };
        centerStyle = new GUIStyle(labelStyle) { fontSize = 42, alignment = TextAnchor.MiddleCenter };
    }

    void OnGUI()
    {
        if (labelStyle == null) InitStyles();
        if (GameManager.Instance == null) return;

        GUI.Box(new Rect(15, 15, 440, 105), "");
        GUI.Label(new Rect(30, 25, 190, 30), $"COINS  {GameManager.Instance.Money}", labelStyle);
        GUI.Label(new Rect(235, 25, 190, 30), $"BASE  {GameManager.Instance.BaseHealth}", labelStyle);
        GUI.Label(new Rect(30, 65, 220, 30), $"WAVE  {GameManager.Instance.CurrentWave}/{GameManager.Instance.MaxWaves}", labelStyle);
        GUI.Label(new Rect(235, 69, 190, 26), "TOWER  $100", smallStyle);

        GUI.Box(new Rect(Screen.width - 325, 15, 310, 66), "MOVE MOUSE TO PLACE\nLEFT CLICK TO BUILD", boxStyle);

        if (GameManager.Instance.GameEnded)
        {
            GUI.Box(new Rect(Screen.width / 2f - 250, Screen.height / 2f - 95, 500, 190), "");
            GUI.Label(new Rect(Screen.width / 2f - 220, Screen.height / 2f - 55, 440, 65), GameManager.Instance.EndMessage, centerStyle);
            GUI.Label(new Rect(Screen.width / 2f - 180, Screen.height / 2f + 20, 360, 35), "Stop Play Mode to restart", smallStyle);
        }
    }
}
