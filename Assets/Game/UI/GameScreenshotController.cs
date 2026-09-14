using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameScreenshotController : MonoBehaviour
{
    const string ScreenshotFolderName = "Screenshots";
    const float ToastLifetimeSeconds = 2.2f;

    Text toast;
    Coroutine toastRoutine;

    void Update()
    {
        if (GameInput.ScreenshotPressed())
            Capture();
    }

    public void Capture()
    {
        string directory = Path.Combine(Application.persistentDataPath, ScreenshotFolderName);
        Directory.CreateDirectory(directory);

        string fileName = BuildFileName(DateTime.Now);
        string fullPath = Path.Combine(directory, fileName);
        ScreenCapture.CaptureScreenshot(fullPath);

        RuntimeFileLogger.Event("SCREENSHOT", "Captured gameplay screenshot: " + fullPath);
        StartCoroutine(ShowConfirmationNextFrame(fileName));
    }

    public static string BuildFileName(DateTime timestamp)
    {
        return "TheTroyGame_" + timestamp.ToString("yyyy-MM-dd_HH-mm-ss-fff") + ".png";
    }

    IEnumerator ShowConfirmationNextFrame(string fileName)
    {
        // CaptureScreenshot grabs the current frame. Waiting one frame prevents the
        // confirmation UI from appearing inside the screenshot itself.
        yield return null;
        ShowToast(GameLanguage.T("Screenshot saved: ", "Снимок сохранён: ") + fileName);
    }

    void ShowToast(string message)
    {
        EnsureToast();
        if (toast == null) return;

        toast.text = message;
        toast.gameObject.SetActive(true);

        if (toastRoutine != null) StopCoroutine(toastRoutine);
        toastRoutine = StartCoroutine(HideToastAfterDelay());
    }

    IEnumerator HideToastAfterDelay()
    {
        yield return new WaitForSecondsRealtime(ToastLifetimeSeconds);
        if (toast != null) toast.gameObject.SetActive(false);
        toastRoutine = null;
    }

    void EnsureToast()
    {
        if (toast != null) return;

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("ScreenshotCanvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 5000;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        GameObject panel = new GameObject("ScreenshotToast");
        panel.transform.SetParent(canvas.transform, false);
        Image image = panel.AddComponent<Image>();
        image.color = new Color(.08f, .045f, .025f, .94f);
        image.raycastTarget = false;

        RectTransform panelRect = image.rectTransform;
        panelRect.anchorMin = new Vector2(.5f, 0f);
        panelRect.anchorMax = new Vector2(.5f, 0f);
        panelRect.pivot = new Vector2(.5f, 0f);
        panelRect.anchoredPosition = new Vector2(0f, 34f);
        panelRect.sizeDelta = new Vector2(620f, 54f);

        Outline outline = panel.AddComponent<Outline>();
        outline.effectColor = new Color(.72f, .45f, .16f, .85f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(panel.transform, false);
        toast = textObject.AddComponent<Text>();
        toast.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        toast.fontSize = 16;
        toast.fontStyle = FontStyle.Bold;
        toast.alignment = TextAnchor.MiddleCenter;
        toast.color = new Color(1f, .84f, .55f, 1f);
        toast.raycastTarget = false;

        RectTransform textRect = toast.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(12f, 4f);
        textRect.offsetMax = new Vector2(-12f, -4f);

        panel.SetActive(false);
    }
}
