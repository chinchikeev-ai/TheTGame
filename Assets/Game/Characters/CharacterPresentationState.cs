using System;
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
    Coroutine impactRoutine;
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
    public void PlayAttack(Action impact) => PlayTimedAttack("Attack", null, impact, .46f, .34f);
    public void PlaySpearAttack() => Trigger("Poke", "Attack");
    public void PlaySpearAttack(Action impact) => PlayTimedAttack("Poke", "Attack", impact, .48f, .34f);
    public void PlayCommand() => Trigger("Command", "Attack");
    public void PlayAbilityQ() => Trigger("AbilityQ", "Attack");
    public void PlayAbilityE() => Trigger("AbilityE", "Attack");
    public void PlayAbilityR() => Trigger("AbilityR", "Attack");
    public void PlayAbilityR(Action impact) => PlayTimedAttack("AbilityR", "Attack", impact, .52f, .38f);
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
        PlayBowShot(null, redrawDelay);
    }

    public void PlayBowShot(Action impact, float redrawDelay = .18f)
    {
        if (dead) return;

        string stateName = Trigger("Release", "Attack");
        if (impactRoutine != null) StopCoroutine(impactRoutine);
        if (impact != null)
            impactRoutine = StartCoroutine(InvokeAtAnimationPhase(stateName, .40f, .30f, impact));

        if (animator == null || !HasParameter("Draw", AnimatorControllerParameterType.Trigger)) return;

        if (bowRoutine != null) StopCoroutine(bowRoutine);
        bowRoutine = StartCoroutine(RedrawBowAfterImpact(stateName, Mathf.Max(.01f, redrawDelay)));
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
        if (impactRoutine != null)
        {
            StopCoroutine(impactRoutine);
            impactRoutine = null;
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

    void PlayTimedAttack(string parameter, string fallback, Action impact, float normalizedImpact, float fallbackDelay)
    {
        if (dead) return;
        string stateName = Trigger(parameter, fallback);
        if (impact == null) return;

        if (impactRoutine != null) StopCoroutine(impactRoutine);
        impactRoutine = StartCoroutine(InvokeAtAnimationPhase(stateName, normalizedImpact, fallbackDelay, impact));
    }

    string Trigger(string parameter, string fallback = null)
    {
        if (dead || animator == null) return null;

        int parameterHash = Animator.StringToHash(parameter);
        if (HasParameter(parameterHash, AnimatorControllerParameterType.Trigger))
        {
            animator.ResetTrigger(parameterHash);
            animator.SetTrigger(parameterHash);
            return parameter;
        }

        if (string.IsNullOrEmpty(fallback)) return null;
        int fallbackHash = Animator.StringToHash(fallback);
        if (!HasParameter(fallbackHash, AnimatorControllerParameterType.Trigger)) return null;
        animator.ResetTrigger(fallbackHash);
        animator.SetTrigger(fallbackHash);
        return fallback;
    }

    IEnumerator InvokeAtAnimationPhase(string stateName, float normalizedImpact, float fallbackDelay, Action impact)
    {
        float startedAt = Time.time;
        float maxWait = Mathf.Max(.45f, fallbackDelay * 1.75f);
        int stateHash = string.IsNullOrEmpty(stateName) ? 0 : Animator.StringToHash(stateName);

        while (!dead && Time.time - startedAt < maxWait)
        {
            if (animator != null && stateHash != 0 && IsStateAtOrBeyondPhase(stateHash, normalizedImpact))
            {
                impact?.Invoke();
                impactRoutine = null;
                yield break;
            }

            if ((animator == null || stateHash == 0) && Time.time - startedAt >= fallbackDelay)
            {
                impact?.Invoke();
                impactRoutine = null;
                yield break;
            }

            yield return null;
        }

        if (!dead) impact?.Invoke();
        impactRoutine = null;
    }

    bool IsStateAtOrBeyondPhase(int stateHash, float normalizedImpact)
    {
        if (animator == null || !animator.isActiveAndEnabled) return false;

        AnimatorStateInfo current = animator.GetCurrentAnimatorStateInfo(0);
        if (current.shortNameHash == stateHash && current.normalizedTime >= normalizedImpact) return true;

        if (!animator.IsInTransition(0)) return false;
        AnimatorStateInfo next = animator.GetNextAnimatorStateInfo(0);
        return next.shortNameHash == stateHash && next.normalizedTime >= normalizedImpact;
    }

    IEnumerator RedrawBowAfterImpact(string releaseStateName, float redrawDelay)
    {
        float startedAt = Time.time;
        int stateHash = string.IsNullOrEmpty(releaseStateName) ? 0 : Animator.StringToHash(releaseStateName);
        while (!dead && Time.time - startedAt < .60f)
        {
            if (animator == null || stateHash == 0 || IsStateAtOrBeyondPhase(stateHash, .58f)) break;
            yield return null;
        }

        if (!dead)
        {
            yield return new WaitForSeconds(redrawDelay);
            Trigger("Draw");
        }
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
        Quaternion endRotation = startRotation * Quaternion.Euler(0f, 0f, UnityEngine.Random.value < .5f ? 82f : -82f);
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
