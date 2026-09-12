using UnityEngine;
using UnityEngine.UI;

public class HectorHUD : MonoBehaviour
{
    Text text;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<HectorHUD>() == null)
            new GameObject("HectorHUD").AddComponent<HectorHUD>();
    }

    void Start()
    {
        GameObject canvasObj = new GameObject("HectorHUDCanvas");
        canvasObj.transform.SetParent(transform, false);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 25;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        GameObject panelObj = new GameObject("HectorPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        Image panel = panelObj.AddComponent<Image>();
        panel.color = new Color(.07f, .08f, .10f, .88f);
        RectTransform pr = panel.rectTransform;
        pr.anchorMin = pr.anchorMax = pr.pivot = new Vector2(0f, 0f);
        pr.anchoredPosition = new Vector2(20f, 20f);
        pr.sizeDelta = new Vector2(520f, 90f);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(panelObj.transform, false);
        text = textObj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 18; text.fontStyle = FontStyle.Bold; text.color = Color.white; text.alignment = TextAnchor.MiddleLeft;
        RectTransform tr = text.rectTransform;
        tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one; tr.offsetMin = new Vector2(16, 8); tr.offsetMax = new Vector2(-16, -8);
    }

    void Update()
    {
        HectorController h = HectorController.Instance;
        if (h == null)
        {
            text.text = "HECTOR: --";
            return;
        }

        if (h.IsDowned)
        {
            text.text = GameLanguage.T(
                $"HECTOR DOWNED  •  REVIVE {Mathf.CeilToInt(h.DownedRemaining)}s",
                $"ГЕКТОР ПОВЕРЖЕН  •  ВОЗВРАЩЕНИЕ {Mathf.CeilToInt(h.DownedRemaining)}с");
            return;
        }

        text.text = GameLanguage.T(
            $"HECTOR HP {Mathf.CeilToInt(h.Health)}/{Mathf.CeilToInt(h.maxHealth)}  •  Q WAR CRY {Cd(h.WarCryCooldownRemaining)}  •  E SHIELD WALL {Cd(h.ShieldWallCooldownRemaining)}  •  R SPEAR THROW {Cd(h.SpearThrowCooldownRemaining)}",
            $"ГЕКТОР HP {Mathf.CeilToInt(h.Health)}/{Mathf.CeilToInt(h.maxHealth)}  •  Q БОЕВОЙ КЛИЧ {Cd(h.WarCryCooldownRemaining)}  •  E СТЕНА ЩИТОВ {Cd(h.ShieldWallCooldownRemaining)}  •  R БРОСОК КОПЬЯ {Cd(h.SpearThrowCooldownRemaining)}");
    }

    string Cd(float value) => value <= 0.01f ? GameLanguage.T("READY", "ГОТОВО") : Mathf.CeilToInt(value) + GameLanguage.T("s", "с");
}
