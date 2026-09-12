using System.Collections;
using UnityEngine;

public sealed class MenuScreenTransition : MonoBehaviour
{
    CanvasGroup group;
    Coroutine routine;

    void Awake()
    {
        group = GetComponent<CanvasGroup>();
        if (group == null) group = gameObject.AddComponent<CanvasGroup>();
    }

    public void Show(float duration = 0.16f)
    {
        if (routine != null) StopCoroutine(routine);
        gameObject.SetActive(true);
        routine = StartCoroutine(FadeTo(1f, duration, false));
    }

    public void Hide(float duration = 0.12f)
    {
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
