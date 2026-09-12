using System.Collections;
using UnityEngine;

public sealed class MenuScreenTransition : MonoBehaviour
{
    CanvasGroup group;
    Coroutine routine;
    bool initialized;

    void Awake()
    {
        EnsureGroup();
        initialized = true;
    }

    void OnEnable()
    {
        EnsureGroup();
        if (!initialized)
        {
            initialized = true;
            return;
        }

        if (routine != null) StopCoroutine(routine);
        group.alpha = 0f;
        group.blocksRaycasts = true;
        group.interactable = true;
        routine = StartCoroutine(FadeTo(1f, 0.16f, false));
    }

    void EnsureGroup()
    {
        if (group != null) return;
        group = GetComponent<CanvasGroup>();
        if (group == null) group = gameObject.AddComponent<CanvasGroup>();
    }

    public void Show(float duration = 0.16f)
    {
        EnsureGroup();
        if (routine != null) StopCoroutine(routine);
        gameObject.SetActive(true);
        group.alpha = 0f;
        group.blocksRaycasts = true;
        group.interactable = true;
        routine = StartCoroutine(FadeTo(1f, duration, false));
    }

    public void Hide(float duration = 0.12f)
    {
        EnsureGroup();
        if (!gameObject.activeSelf) return;
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FadeTo(0f, duration, true));
    }

    IEnumerator FadeTo(float target, float duration, bool disableAfter)
    {
        float start = group.alpha;
        float elapsed = 0f;
        group.blocksRaycasts = !disableAfter;
        group.interactable = !disableAfter;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
            t = t * t * (3f - 2f * t);
            group.alpha = Mathf.Lerp(start, target, t);
            yield return null;
        }

        group.alpha = target;
        if (disableAfter) gameObject.SetActive(false);
        routine = null;
    }
}
