using System;
using UnityEngine;

public static class HectorProductionCombatPresentation
{
    public static void PlayWarCryImpact(this HectorPresentationBridge bridge, Action impact)
    {
        PlayTimed(bridge, state => state.PlayAbilityQ(impact), impact);
    }

    public static void PlayWarCryEffect(this HectorPresentationBridge bridge, float radius)
    {
        bridge.GetComponent<HectorAbilityPresentation>()?.PlayWarCry(radius);
    }

    public static void PlayShieldWallImpact(this HectorPresentationBridge bridge, Action impact)
    {
        PlayTimed(bridge, state => state.PlayAbilityE(impact), impact);
    }

    public static void PlayShieldWallEffect(this HectorPresentationBridge bridge, Vector3 center, float duration)
    {
        bridge.PlayShieldWall(center, Mathf.Max(.2f, duration));
    }

    public static void PlayUltimateImpact(this HectorPresentationBridge bridge, Action impact)
    {
        PlayTimed(bridge, state => state.PlayAbilityF(impact), impact);
    }

    public static void PlayUltimateEffect(this HectorPresentationBridge bridge, float radius)
    {
        bridge.GetComponent<HectorAbilityPresentation>()?.PlayUltimate(radius);
    }

    public static void SetDownedState(this HectorPresentationBridge bridge, bool downed)
    {
        bridge.GetComponent<CharacterPresentationState>()?.SetDowned(downed);
        if (downed) bridge.GetComponent<CharacterWeaponPresentation>()?.RestoreSpear();
    }

    public static void PlayReviveEffect(this HectorPresentationBridge bridge)
    {
        bridge.GetComponent<CharacterPresentationState>()?.SetDowned(false);
        bridge.GetComponent<CharacterWeaponPresentation>()?.RestoreSpear();
        CombatImpactPresentation.Pulse(bridge.transform.position + Vector3.up * .6f,
            new Color(.95f,.70f,.22f), 1.7f, .35f);
        RuntimeEffects.Instance?.PlayHeroAbilitySound();
    }

    static void PlayTimed(HectorPresentationBridge bridge, Action<CharacterPresentationState> play, Action fallback)
    {
        CharacterPresentationState state = bridge.GetComponent<CharacterPresentationState>();
        if (state == null)
        {
            fallback?.Invoke();
            return;
        }
        play(state);
    }
}
