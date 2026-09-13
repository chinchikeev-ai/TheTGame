using System.Collections;
using UnityEngine;

public class CharacterPresentationState : MonoBehaviour
{
    Animator animator;
    Vector3 baseScale;
    Coroutine fallbackRoutine;
    bool dead;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        baseScale = transform.localScale;
    }

    public void SetMoving(bool moving)
    {
        if (dead || animator == null) return;
        if (HasParameter("Speed", AnimatorControllerParameterType.Float))
            animator.SetFloat("Speed", moving ? 1f : 0f, .08f, Time.deltaTime);
        else if (HasParameter("Moving", AnimatorControllerParameterType.Bool))
            animator.SetBool("Moving", moving);
    }

    public void PlayAttack() => Trigger("Attack");
    public void PlayCommand() => Trigger("Command", "Attack");
    public void PlayAbilityQ() => Trigger("AbilityQ", "Attack");
    public void PlayAbilityE() => Trigger("AbilityE", "Attack");
    public void PlayAbilityR() => Trigger("AbilityR", "Attack");
    public void PlayAbilityF() => Trigger("AbilityF", "Attack");

    public void PlayHit()
    {
        if (dead) return;
        if (animator != null && HasParameter("Hit", AnimatorControllerParameterType.Trigger))
        {
            animator.SetTrigger("Hit");
            return;
        }

        if (fallbackRoutine != null) StopCoroutine(fallbackRoutine);
        fallbackRoutine = StartCoroutine(HitPulse());
    }

    public void SetDowned(bool downed)
    {
        if (animator != null && HasParameter("IsDowned", AnimatorControllerParameterType.Bool))
            animator.SetBool("IsDowned", downed);
    }

    public float PlayDeath(bool boss)
    {
        if (dead) return 0f;
        SetMoving(false);
        dead = true;

        float duration = boss ? 1.25f : .70f;
        if (animator != null)
        {
            if (HasParameter("Die", AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger("Die");
                return duration;
            }
            if (HasParameter("IsDead", AnimatorControllerParameterType.Bool))
            {
                animator.SetBool("IsDead", true);
                return duration;
            }
        }

        if (fallbackRoutine != null) StopCoroutine(fallbackRoutine);
        fallbackRoutine = StartCoroutine(FallbackDeath(duration));
        return duration;
    }

    void Trigger(string parameter, string fallback = null)
    {
        if (dead || animator == null) return;
        if (HasParameter(parameter, AnimatorControllerParameterType.Trigger))
        {
            animator.ResetTrigger(parameter);
            animator.SetTrigger(parameter);
            return;
        }

        if (!string.IsNullOrEmpty(fallback) && HasParameter(fallback, AnimatorControllerParameterType.Trigger))
        {
            animator.ResetTrigger(fallback);
            animator.SetTrigger(fallback);
        }
    }

    IEnumerator HitPulse()
    {
        Vector3 start = baseScale;
        Vector3 compressed = new Vector3(start.x * 1.04f, start.y * .94f, start.z * 1.04f);
        float elapsed = 0f;
        const float half = .07f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(start, compressed, Mathf.Clamp01(elapsed / half));
            yield return null;
        }
        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(compressed, start, Mathf.Clamp01(elapsed / half));
            yield return null;
        }
        transform.localScale = start;
        fallbackRoutine = null;
    }

    IEnumerator FallbackDeath(float duration)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0f, 0f, Random.value < .5f ? 82f : -82f);
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + Vector3.down * .18f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }
        fallbackRoutine = null;
    }

    bool HasParameter(string parameterName, AnimatorControllerParameterType type)
    {
        if (animator == null) return false;
        foreach (AnimatorControllerParameter parameter in animator.parameters)
            if (parameter.name == parameterName && parameter.type == type) return true;
        return false;
    }
}
