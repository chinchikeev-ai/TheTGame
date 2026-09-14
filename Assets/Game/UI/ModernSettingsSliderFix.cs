using UnityEngine;
using UnityEngine.UI;

public sealed class ModernSettingsSliderFix : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<ModernSettingsSliderFix>() == null)
            new GameObject("ModernSettingsSliderFix").AddComponent<ModernSettingsSliderFix>();
    }

    void LateUpdate()
    {
        GameObject menuCanvas = GameObject.Find("MenuCanvas");
        if (menuCanvas == null) return;

        Transform settings = menuCanvas.transform.Find("Settings");
        if (settings == null || !settings.gameObject.activeInHierarchy) return;

        Transform modern = settings.Find("ModernSettingsPanel");
        if (modern == null) return;

        Slider[] sliders = modern.GetComponentsInChildren<Slider>(true);
        for (int i = 0; i < sliders.Length; i++)
            Repair(sliders[i]);
    }

    static void Repair(Slider slider)
    {
        if (slider == null || slider.GetComponent<SettingsSliderLayoutMarker>() != null) return;

        RectTransform sliderRect = slider.transform as RectTransform;
        if (sliderRect == null) return;
        sliderRect.sizeDelta = new Vector2(470f, 36f);

        Transform background = slider.transform.Find("Background");
        Transform fill = slider.transform.Find("Fill");
        Transform handle = slider.transform.Find("Handle");
        if (background == null || fill == null || handle == null) return;

        RectTransform backgroundRect = background as RectTransform;
        backgroundRect.anchorMin = new Vector2(0f, .5f);
        backgroundRect.anchorMax = new Vector2(1f, .5f);
        backgroundRect.pivot = new Vector2(.5f, .5f);
        backgroundRect.offsetMin = new Vector2(0f, -6f);
        backgroundRect.offsetMax = new Vector2(0f, 6f);
        Image backgroundImage = background.GetComponent<Image>();
        if (backgroundImage != null) backgroundImage.raycastTarget = false;

        GameObject fillAreaObject = new GameObject("Fill Area", typeof(RectTransform));
        fillAreaObject.transform.SetParent(slider.transform, false);
        RectTransform fillArea = fillAreaObject.GetComponent<RectTransform>();
        fillArea.anchorMin = new Vector2(0f, .5f);
        fillArea.anchorMax = new Vector2(1f, .5f);
        fillArea.pivot = new Vector2(.5f, .5f);
        fillArea.offsetMin = new Vector2(10f, -6f);
        fillArea.offsetMax = new Vector2(-10f, 6f);

        fill.SetParent(fillArea, false);
        RectTransform fillRect = fill as RectTransform;
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.pivot = new Vector2(.5f, .5f);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        Image fillImage = fill.GetComponent<Image>();
        if (fillImage != null) fillImage.raycastTarget = false;

        GameObject handleAreaObject = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleAreaObject.transform.SetParent(slider.transform, false);
        RectTransform handleArea = handleAreaObject.GetComponent<RectTransform>();
        handleArea.anchorMin = new Vector2(0f, .5f);
        handleArea.anchorMax = new Vector2(1f, .5f);
        handleArea.pivot = new Vector2(.5f, .5f);
        handleArea.offsetMin = new Vector2(11f, -18f);
        handleArea.offsetMax = new Vector2(-11f, 18f);

        handle.SetParent(handleArea, false);
        RectTransform handleRect = handle as RectTransform;
        handleRect.anchorMin = handleRect.anchorMax = handleRect.pivot = new Vector2(.5f, .5f);
        handleRect.anchoredPosition = Vector2.zero;
        handleRect.sizeDelta = new Vector2(22f, 28f);

        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.direction = Slider.Direction.LeftToRight;
        slider.wholeNumbers = false;
        slider.AddComponent<SettingsSliderLayoutMarker>();
    }
}

public sealed class SettingsSliderLayoutMarker : MonoBehaviour
{
}
