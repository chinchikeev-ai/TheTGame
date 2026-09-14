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
    Renderer bodyRenderer;
    Vector3 previousPosition;
    bool previousDowned;
    bool selected;

    public void Initialize(HectorController controller)
    {
        hector = controller;
        abilityPresentation = GetComponent<HectorAbilityPresentation>();
        if (abilityPresentation == null) abilityPresentation = gameObject.AddComponent<HectorAbilityPresentation>();
        characterPresentation = GetComponent<CharacterPresentationState>();
        if (characterPresentation == null) characterPresentation = gameObject.AddComponent<CharacterPresentationState>();
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
        if (characterPresentation == null)
        {
            impact?.Invoke();
            PlayMeleeImpact(point);
            return;
        }

        characterPresentation.PlaySpearAttack(() =>
        {
            if (hector == null || hector.IsDowned) return;
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

    public void PlayWarCry(float radius)
    {
        characterPresentation?.PlayAbilityQ();
        abilityPresentation?.PlayWarCry(radius);
    }

    public void PlayShieldWall(Vector3 center)
    {
        characterPresentation?.PlayAbilityE();
        abilityPresentation?.PlayShieldWall(center, transform.rotation);
    }

    public void PlaySpearImpact(Vector3 point, Action impact)
    {
        if (characterPresentation == null)
        {
            impact?.Invoke();
            abilityPresentation?.PlaySpearImpact(point);
            return;
        }

        characterPresentation.PlayAbilityR(() =>
        {
            if (hector == null || hector.IsDowned) return;
            impact?.Invoke();
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
