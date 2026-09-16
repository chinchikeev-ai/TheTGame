using UnityEngine;

public sealed class BuildVersionOverlay : MonoBehaviour
{
    static BuildVersionOverlay instance;
    GUIStyle style;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        if (instance != null) return;
        GameObject root = new GameObject("BuildVersionOverlay");
        DontDestroyOnLoad(root);
        instance = root.AddComponent<BuildVersionOverlay>();
    }

    void OnGUI()
    {
        // Keep build identity visible on menu/pause screens without occupying combat HUD space.
        if (Time.timeScale > .001f) return;

        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = Mathf.Max(12, Mathf.RoundToInt(14f * (Screen.height / 1080f))),
                fontStyle = FontStyle.Bold
            };
            style.normal.textColor = new Color(1f, .86f, .58f, .95f);
        }

        float scale = Mathf.Clamp(Screen.height / 1080f, .70f, 1.35f);
        Rect rect = new Rect(15f * scale, 12f * scale, 240f * scale, 36f * scale);

        Color previous = GUI.color;
        GUI.color = new Color(.12f, .055f, .025f, .78f);
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = previous;

        Rect textRect = new Rect(rect.x + 10f * scale, rect.y, rect.width - 20f * scale, rect.height);
        GUI.Label(textRect, BuildVersionInfo.CompactMenuBadge, style);
    }
}
