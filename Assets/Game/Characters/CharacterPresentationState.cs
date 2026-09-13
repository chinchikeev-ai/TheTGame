using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPresentationState : MonoBehaviour
{
    readonly Dictionary<int, AnimatorControllerParameterType> parameterTypes = new Dictionary<int, AnimatorControllerParameterType>();

    Animator animator;
    RuntimeAnimatorController cachedController;
    Vector3 baseScale;
    Coroutine fallbackRoutine;
    Coroutine bowRoutine;
    bool dead;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        baseScale = transform.localScale;
        RefreshParameterCache();
    }

    public void SetMoving(bool moving)
    {
        if (dead || animator == null) return;
        if (HasParameter("Speed", AnimatorControllerParameterType.Float))
            animator.SetFloat(Animator.StringToHash("Speed"), moving ? 1f : 0f, .08f, Time.deltaTime);
        else if (HasParameter("Moving", AnimatorControllerParameterType.Bool))
            animator.SetBool(Animator.StringToHash("Moving"), moving);
    }

    public void PlayAttack() => Trigger("Attack");
    public void PlaySpearAttack() => Trigger("Poke", "Attack");
    public void PlayCommand() => Trigger("Command", "Attack");
    public void PlayAbilityQ() => Trigger("AbilityQ", "Attack");
    public void PlayAbilityE() => Trigger("AbilityE", "Attack");
    public void PlayAbilityR() => Trigger("AbilityR", "Attack");
    public void PlayAbilityF() => Trigger("AbilityF", "Attack");
    public void PlayBlock() => Trigger("Block", "Attack");
    public void PlayPoke() => PlaySpearAttack();
    public void PlayDraw() => Trigger("Draw", "Attack");
    public void PlayRelease() => Trigger("Release", "Attack");
    public void PlayReload() => Trigger("Reload", "Attack");
    public void PlayTension() => Trigger("Tension", "Attack");
    public void PlayFire() => Trigger("Fire", "Attack");
    public void PlayCast() => Trigger("Cast", "Attack");
    public void PlayChannel() => Trigger("Channel", "Cast");
    public void PlayStoke() => Trigger("Stoke", "Attack");
    public void PlayThrow() => Trigger("Throw", "Attack");

    public void PrepareBow()
    {
        Trigger("Draw");
    }

    public void PlayBowShot(float redrawDelay = .18f)
    {
        if (dead || animator == null) return;

        Trigger("Release", "Attack");
        if (!HasParameter("Draw", AnimatorControllerParameterType.Trigger)) return;

        if (bowRoutine != null) StopCoroutine(bowRoutine);
        bowRoutine = StartCoroutine(RedrawBow(Mathf.Max(.01f, redrawDelay)));
    }

    public void PlayHit()
    {
        if (dead) return;
        if (animator != null && HasParameter("Hit", AnimatorControllerParameterType.Trigger))
        {
            animator.SetTrigger(Animator.StringToHash("Hit"));
            return;
        }

        if (fallbackRoutine != null) StopCoroutine(fallbackRoutine);
        fallbackRoutine = StartCoroutine(HitPulse());
    }

    public void SetDowned(bool downed)
    {
        if (animator != null && HasParameter("IsDowned", AnimatorControllerParameterType.Bool))
            animator.SetBool(Animator.StringToHash("IsDowned"), downed);
    }

    public float PlayDeath(bool boss)
    {
        if (dead) return 0f;
        SetMoving(false);
        dead = true;

        if (bowRoutine != null)
        {
            StopCoroutine(bowRoutine);
            bowRoutine = null;
        }

        float duration = boss ? 1.25f : .70f;
        if (animator != null)
        {
            if (HasParameter("Die", AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger(Animator.StringToHash("Die"));
                return duration;
            }
            if (HasParameter("IsDead", AnimatorControllerParameterType.Bool))
            {
                animator.SetBool(Animator.StringToHash("IsDead"), true);
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

        int parameterHash = Animator.StringToHash(parameter);
        if (HasParameter(parameterHash, AnimatorControllerParameterType.Trigger))
        {
            animator.ResetTrigger(parameterHash);
            animator.SetTrigger(parameterHash);
            return;
        }

        if (string.IsNullOrEmpty(fallback)) return;
        int fallbackHash = Animator.StringToHash(fallback);
        if (!HasParameter(fallbackHash, AnimatorControllerParameterType.Trigger)) return;
        animator.ResetTrigger(fallbackHash);
        animator.SetTrigger(fallbackHash);
    }

    IEnumerator RedrawBow(float delay)
    {
        yield return new WaitForSeconds(delay);
        Trigger("Draw");
        bowRoutine = null;
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
        return HasParameter(Animator.StringToHash(parameterName), type);
    }

    bool HasParameter(int parameterHash, AnimatorControllerParameterType type)
    {
        RefreshParameterCache();
        return parameterTypes.TryGetValue(parameterHash, out AnimatorControllerParameterType actualType) && actualType == type;
    }

    void RefreshParameterCache()
    {
        if (animator == null)
        {
            parameterTypes.Clear();
            cachedController = null;
            return;
        }

        RuntimeAnimatorController controller = animator.runtimeAnimatorController;
        if (controller == cachedController && parameterTypes.Count > 0) return;

        cachedController = controller;
        parameterTypes.Clear();
        foreach (AnimatorControllerParameter parameter in animator.parameters)
            parameterTypes[Animator.StringToHash(parameter.name)] = parameter.type;
    }
}
