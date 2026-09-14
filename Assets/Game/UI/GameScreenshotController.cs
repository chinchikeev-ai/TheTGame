using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public sealed class GameScreenshotController : MonoBehaviour
{
    const string ScreenshotFolderName = "Screenshots";
    const float ToastLifetimeSeconds = 2.2f;

    static readonly string[] BlockingMenuNames =
    {
        "MainMenu", "LevelSelect", "Settings", "PauseMenu", "EndMenu", "ConfirmationModal"
    };

    public static Vector2 CaptureButtonAnchor => new Vector2(1f, .5f);
    public static Vector2 CaptureButtonOffset => new Vector2(-24f, 0f);
    public static Vector2 CaptureButtonSize => new Vector2(112f, 72f);

    Canvas screenshotCanvas;
    Canvas menuCanvas;
    Button captureButton;
    Text toast;
    Coroutine toastRoutine;
    bool captureInProgress;

    void Start()
    {
        EnsureCaptureUi();
        FindMenuCanvas();
    }

    void Update()
    {
        if (captureButton == null) EnsureCaptureUi();
        if (menuCanvas == null) FindMenuCanvas();

        if (captureButton != null)
            captureButton.gameObject.SetActive(!captureInProgress && !IsMenuBlockingCaptureButton());

        if (GameInput.ScreenshotPressed())
            Capture();
    }

    public void Capture()
    {
        if (captureInProgress) return;
        StartCoroutine(CaptureRoutine());
    }

    IEnumerator CaptureRoutine()
    {
        captureInProgress = true;
        if (captureButton != null) captureButton.gameObject.SetActive(false);

        string directory = Path.Combine(Application.persistentDataPath, ScreenshotFolderName);
        Directory.CreateDirectory(directory);

        string fileName = BuildFileName(DateTime.Now);
        string fullPath = Path.Combine(directory, fileName);

        // Let the current frame render without the screenshot control itself.
        // This keeps the captured gameplay image clean for art review.
        yield return new WaitForEndOfFrame();
        ScreenCapture.CaptureScreenshot(fullPath);
        RuntimeFileLogger.Event("SCREENSHOT", "Captured gameplay screenshot: " + fullPath);

        // CaptureScreenshot finishes from the rendered frame. Restore the control
        // and show feedback on the following frame so neither appears in the image.
        yield return null;
        captureInProgress = false;
        if (captureButton != null)
            captureButton.gameObject.SetActive(!IsMenuBlockingCaptureButton());
        ShowToast(GameLanguage.T("Screenshot saved: ", "Снимок сохранён: ") + fileName);
    }

    public static string BuildFileName(DateTime timestamp)
    {
        return "TheTroyGame_" + timestamp.ToString("yyyy-MM-dd_HH-mm-ss-fff") + ".png";
    }

    void EnsureCaptureUi()
    {
        if (screenshotCanvas == null)
        {
            GameObject existing = GameObject.Find("ScreenshotUI");
            if (existing != null) screenshotCanvas = existing.GetComponent<Canvas>();

            if (screenshotCanvas == null)
            {
                GameObject canvasObject = new GameObject("ScreenshotUI");
                screenshotCanvas = canvasObject.AddComponent<Canvas>();
                screenshotCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                screenshotCanvas.sortingOrder = 5000;

                CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920f, 1080f);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = .5f;

                canvasObject.AddComponent<GraphicRaycaster>();
            }
        }

        if (captureButton == null && screenshotCanvas != null)
            captureButton = BuildCaptureButton(screenshotCanvas.transform);
    }

    Button BuildCaptureButton(Transform parent)
    {
        GameObject buttonObject = new GameObject("ScreenshotButton");
        buttonObject.transform.SetParent(parent, false);

        Image background = buttonObject.AddComponent<Image>();
        background.color = new Color(.16f, .075f, .035f, .92f);

        RectTransform rect = background.rectTransform;
        rect.anchorMin = CaptureButtonAnchor;
        rect.anchorMax = CaptureButtonAnchor;
        rect.pivot = new Vector2(1f, .5f);
        rect.anchoredPosition = CaptureButtonOffset;
        rect.sizeDelta = CaptureButtonSize;

        Outline outline = buttonObject.AddComponent<Outline>();
        outline.effectColor = new Color(.84f, .53f, .18f, .92f);
        outline.effectDistance = new Vector2(2f, -2f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = background;
        button.onClick.AddListener(new UnityAction(Capture));

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, .88f, .72f, 1f);
        colors.pressedColor = new Color(.78f, .58f, .44f, 1f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;

        GameObject textObject = new GameObject("Label");
        textObject.transform.SetParent(buttonObject.transform, false);
        Text label = textObject.AddComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.text = "SHOT\nF12";
        label.fontSize = 17;
        label.fontStyle = FontStyle.Bold;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = new Color(1f, .80f, .42f, 1f);
        label.raycastTarget = false;
        label.horizontalOverflow = HorizontalWrapMode.Overflow;
        label.verticalOverflow = VerticalWrapMode.Overflow;

        RectTransform labelRect = label.rectTransform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(8f, 4f);
        labelRect.offsetMax = new Vector2(-8f, -4f);

        return button;
    }

    void ShowToast(string message)
    {
        EnsureToast();
        if (toast == null) return;

        toast.text = message;
        toast.gameObject.transform.parent.gameObject.SetActive(true);

        if (toastRoutine != null) StopCoroutine(toastRoutine);
        toastRoutine = StartCoroutine(HideToastAfterDelay());
    }

    IEnumerator HideToastAfterDelay()
    {
        yield return new WaitForSecondsRealtime(ToastLifetimeSeconds);
        if (toast != null) toast.gameObject.transform.parent.gameObject.SetActive(false);
        toastRoutine = null;
    }

    void EnsureToast()
    {
        if (toast != null) return;
        EnsureCaptureUi();
        if (screenshotCanvas == null) return;

        Transform existing = screenshotCanvas.transform.Find("ScreenshotToast");
        if (existing != null)
        {
            toast = existing.GetComponentInChildren<Text>(true);
            return;
        }

        GameObject panel = new GameObject("ScreenshotToast");
        panel.transform.SetParent(screenshotCanvas.transform, false);
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

    void FindMenuCanvas()
    {
        GameObject menuObject = GameObject.Find("MenuCanvas");
        menuCanvas = menuObject != null ? menuObject.GetComponent<Canvas>() : null;
    }

    bool IsMenuBlockingCaptureButton()
    {
        if (menuCanvas == null) return false;
        for (int i = 0; i < BlockingMenuNames.Length; i++)
        {
            Transform menu = menuCanvas.transform.Find(BlockingMenuNames[i]);
            if (menu != null && menu.gameObject.activeInHierarchy) return true;
        }
        return false;
    }
}
