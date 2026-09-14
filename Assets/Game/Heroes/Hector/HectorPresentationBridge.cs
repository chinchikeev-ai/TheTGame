using System;
using UnityEngine;

public sealed class HectorPresentationBridge : MonoBehaviour
{
    static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    static readonly int ColorId = Shader.PropertyToID("_Color");

    MaterialPropertyBlock colorBlock;

    HectorController hector;
    HectorAbilityPresentation abilityPresentation;
    CharacterPresentationState characterPresentation;
    CharacterWeaponSocketResolver weaponSockets;
    CharacterWeaponPresentation weaponPresentation;
    Renderer bodyRenderer;
    Vector3 previousPosition;
    float shieldWallPoseUntil;
    bool previousDowned;
    bool selected;

    public bool SpearAvailable => weaponPresentation == null || !weaponPresentation.SpearReleased;

    public void Initialize(HectorController controller)
    {
        hector = controller;
        abilityPresentation = GetComponent<HectorAbilityPresentation>();
        if (abilityPresentation == null) abilityPresentation = gameObject.AddComponent<HectorAbilityPresentation>();
        characterPresentation = GetComponent<CharacterPresentationState>();
        if (characterPresentation == null) characterPresentation = gameObject.AddComponent<CharacterPresentationState>();
        weaponSockets = GetComponent<CharacterWeaponSocketResolver>();
        if (weaponSockets == null) weaponSockets = gameObject.AddComponent<CharacterWeaponSocketResolver>();
        weaponSockets.Refresh();
        weaponPresentation = GetComponent<CharacterWeaponPresentation>();
        if (weaponPresentation == null) weaponPresentation = gameObject.AddComponent<CharacterWeaponPresentation>();
        weaponPresentation.Refresh();
        bodyRenderer = GetComponentInChildren<Renderer>();
        previousPosition = transform.position;
        previousDowned = hector != null && hector.IsDowned;
        characterPresentation.SetDowned(previousDowned);
        RefreshSelectionTint();
    }

    public void SetSelected(bool value)
    {
        selected = value;
        RefreshSelectionTint();
    }

    public void PlayAttackImpact(Vector3 point, Action impact)
    {
        if (!SpearAvailable) return;
        if (characterPresentation == null)
        {
            impact?.Invoke();
            PlayMeleeImpact(point);
            return;
        }

        characterPresentation.PlaySpearAttack(() =>
        {
            if (hector == null || hector.IsDowned || !SpearAvailable) return;
            impact?.Invoke();
            PlayMeleeImpact(point);
        });
    }

    public void PlayDamageImpact(float damage)
    {
        characterPresentation?.PlayHit();
        RuntimeEffects.Instance?.PlayHitSound(damage >= 25f);
        CombatImpactPresentation.HeroHit(transform.position + Vector3.up * .7f, damage >= 25f);
    }

    public void PlayShieldBlockImpact(float incomingDamage)
    {
        characterPresentation?.SetBlocking(true);
        RuntimeEffects.Instance?.PlayShieldBlockSound(incomingDamage >= 25f);
        CombatImpactPresentation.Pulse(transform.position + transform.forward * .25f + Vector3.up * .75f,
            new Color(.96f,.72f,.26f), incomingDamage >= 25f ? .95f : .72f, .18f);
    }

    public void PlayWarCry(float radius)
    {
        characterPresentation?.PlayAbilityQ();
        abilityPresentation?.PlayWarCry(radius);
    }

    public void PlayShieldWall(Vector3 center)
    {
        PlayShieldWall(center, 4f);
    }

    public void PlayShieldWall(Vector3 center, float duration)
    {
        shieldWallPoseUntil = Mathf.Max(shieldWallPoseUntil, Time.time + Mathf.Max(.2f, duration));
        characterPresentation?.PlayAbilityE();
        characterPresentation?.SetBlocking(true);
        abilityPresentation?.PlayShieldWall(center, transform.rotation);
    }

    // Kept as the animation-release hook for the Chapter I timing contract.
    public void PlaySpearImpact(Vector3 point, Action impact)
    {
        if (!SpearAvailable) return;
        if (characterPresentation == null)
        {
            impact?.Invoke();
            return;
        }

        characterPresentation.PlayAbilityR(() =>
        {
            if (hector == null || hector.IsDowned || !SpearAvailable) return;
            impact?.Invoke();
        });
    }

    public void LaunchSpearFlight(Transform target, Action<Vector3> impact)
    {
        if (hector == null || hector.IsDowned || target == null || !SpearAvailable) return;
        Vector3 start = weaponPresentation != null
            ? weaponPresentation.ReleaseSpear()
            : weaponSockets != null
                ? weaponSockets.SpearReleasePoint()
                : transform.TransformPoint(new Vector3(.22f, 1.10f, .42f));

        CombatFlightPresentation.SpawnSpear(start, target, Vector3.up * .65f, point =>
        {
            weaponPresentation?.RestoreSpear();
            impact?.Invoke(point);
            abilityPresentation?.PlaySpearImpact(point);
        });
    }

    public void PlayUltimate(float radius)
    {
        characterPresentation?.PlayAbilityF();
        abilityPresentation?.PlayUltimate(radius);
    }

    void PlayMeleeImpact(Vector3 point)
    {
        CombatImpactPresentation.MeleeHit(point, TowerType.TrojanGuard);
        RuntimeEffects.Instance?.PlayHitSound(false);
    }

    void LateUpdate()
    {
        if (hector == null) return;

        bool downed = hector.IsDowned;
        if (downed != previousDowned)
        {
            characterPresentation.SetDowned(downed);
            previousDowned = downed;
            RefreshSelectionTint();
        }

        bool shieldPoseActive = !downed && Time.time < shieldWallPoseUntil;
        characterPresentation.SetBlocking(shieldPoseActive);

        Vector3 delta = transform.position - previousPosition;
        delta.y = 0f;
        characterPresentation.SetMoving(!downed && delta.sqrMagnitude > .0004f);
        previousPosition = transform.position;
    }

    void RefreshSelectionTint()
    {
        if (bodyRenderer == null) return;
        if (colorBlock == null) colorBlock = new MaterialPropertyBlock();

        Color color = previousDowned
            ? new Color(.25f, .25f, .25f)
            : selected
                ? new Color(.95f,.78f,.18f)
                : new Color(.72f,.48f,.12f);

        colorBlock.Clear();
        colorBlock.SetColor(BaseColorId, color);
        colorBlock.SetColor(ColorId, color);
        bodyRenderer.SetPropertyBlock(colorBlock);
    }
}
