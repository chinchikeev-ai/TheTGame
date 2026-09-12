using System.Collections;
using UnityEngine;

public sealed class MenuScreenTransition : MonoBehaviour
{
    CanvasGroup group;
    RectTransform rect;
    Coroutine routine;
    bool initialized;
    Vector2 basePosition;
    Vector3 baseScale;

    void Awake()
    {
        EnsureComponents();
        basePosition = rect != null ? rect.anchoredPosition : Vector2.zero;
        baseScale = rect != null ? rect.localScale : Vector3.one;
        initialized = true;
    }

    void OnEnable()
    {
        EnsureComponents();
        if (!initialized)
        {
            initialized = true;
            return;
        }

        BeginShow(0.18f);
    }

    void EnsureComponents()
    {
        if (group == null)
        {
            group = GetComponent<CanvasGroup>();
            if (group == null) group = gameObject.AddComponent<CanvasGroup>();
        }
        if (rect == null) rect = transform as RectTransform;
    }

    public void Show(float duration = 0.18f)
    {
        EnsureComponents();
        gameObject.SetActive(true);
        BeginShow(duration);
    }

    public void Hide(float duration = 0.13f)
    {
        EnsureComponents();
        if (!gameObject.activeSelf) return;
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Animate(false, duration, true));
    }

    void BeginShow(float duration)
    {
        if (routine != null) StopCoroutine(routine);
        group.alpha = 0f;
        group.blocksRaycasts = true;
        group.interactable = true;
        if (rect != null)
        {
            rect.anchoredPosition = basePosition + new Vector2(0f, -18f);
            rect.localScale = baseScale * .985f;
        }
        routine = StartCoroutine(Animate(true, duration, false));
    }

    IEnumerator Animate(bool showing, float duration, bool disableAfter)
    {
        float startAlpha = group.alpha;
        float targetAlpha = showing ? 1f : 0f;
        Vector2 startPosition = rect != null ? rect.anchoredPosition : Vector2.zero;
        Vector2 targetPosition = showing ? basePosition : basePosition + new Vector2(0f, 10f);
        Vector3 startScale = rect != null ? rect.localScale : Vector3.one;
        Vector3 targetScale = showing ? baseScale : baseScale * .992f;
        float elapsed = 0f;

        group.blocksRaycasts = showing;
        group.interactable = showing;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, eased);
            if (rect != null)
            {
                rect.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, eased);
                rect.localScale = Vector3.Lerp(startScale, targetScale, eased);
            }
            yield return null;
        }

        group.alpha = targetAlpha;
        if (rect != null)
        {
            rect.anchoredPosition = showing ? basePosition : targetPosition;
            rect.localScale = showing ? baseScale : targetScale;
        }

        if (disableAfter) gameObject.SetActive(false);
        routine = null;
    }
}
