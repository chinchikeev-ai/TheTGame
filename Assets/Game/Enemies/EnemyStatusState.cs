using UnityEngine;

public readonly struct EnemyBurnTickResult
{
    public EnemyBurnTickResult(bool showVisual, float damage)
    {
        ShowVisual = showVisual;
        Damage = damage;
    }

    public bool ShowVisual { get; }
    public float Damage { get; }
}

public sealed class EnemyStatusState
{
    float slowMultiplier = 1f;
    float slowUntil;
    float burnUntil;
    float burnDps;
    float nextBurnTick;
    float nextBurnVisual;
    float armorBreakUntil;
    float armorBreakAmount;
    float commanderUntil;
    float commanderSpeedMultiplier = 1f;
    float commanderDamageMultiplier = 1f;

    public float SpeedMultiplier => slowMultiplier * commanderSpeedMultiplier;
    public float DamageMultiplier => commanderDamageMultiplier;

    public EnemyBurnTickResult TickBurn(float now)
    {
        bool showVisual = false;
        float damage = 0f;

        if (now < burnUntil && burnDps > 0f)
        {
            if (now >= nextBurnVisual)
            {
                nextBurnVisual = now + .24f;
                showVisual = true;
            }

            if (now >= nextBurnTick)
            {
                nextBurnTick = now + 1f;
                damage = burnDps;
            }
        }
        else if (now >= burnUntil)
        {
            burnDps = 0f;
        }

        return new EnemyBurnTickResult(showVisual, damage);
    }

    public void RefreshMovementModifiers(float now)
    {
        if (now >= slowUntil) slowMultiplier = 1f;
        if (now < commanderUntil) return;

        commanderSpeedMultiplier = 1f;
        commanderDamageMultiplier = 1f;
    }

    public float CurrentArmor(float baseArmor, float now)
    {
        return now < armorBreakUntil
            ? Mathf.Max(0f, baseArmor - armorBreakAmount)
            : baseArmor;
    }

    public void ApplySlow(float multiplier, float duration, float now)
    {
        multiplier = Mathf.Clamp(multiplier, .15f, 1f);
        if (multiplier < slowMultiplier || now >= slowUntil) slowMultiplier = multiplier;
        slowUntil = Mathf.Max(slowUntil, now + duration);
    }

    public void ApplyBurn(float damagePerSecond, float duration, float now)
    {
        burnDps = Mathf.Max(burnDps, damagePerSecond);
        burnUntil = Mathf.Max(burnUntil, now + duration);
        nextBurnTick = Mathf.Min(nextBurnTick <= 0f ? now + .5f : nextBurnTick, now + .5f);
        nextBurnVisual = Mathf.Min(nextBurnVisual <= 0f ? now : nextBurnVisual, now);
    }

    public void ApplyArmorBreak(float amount, float duration, float now)
    {
        armorBreakAmount = Mathf.Max(armorBreakAmount, Mathf.Clamp01(amount));
        armorBreakUntil = Mathf.Max(armorBreakUntil, now + duration);
    }

    public void ApplyCommanderAura(float speedMultiplier, float damageMultiplier, float duration, float now)
    {
        commanderSpeedMultiplier = Mathf.Max(commanderSpeedMultiplier, speedMultiplier);
        commanderDamageMultiplier = Mathf.Max(commanderDamageMultiplier, damageMultiplier);
        commanderUntil = Mathf.Max(commanderUntil, now + duration);
    }
}
