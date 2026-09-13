using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TowerCrewAnimationBridge : MonoBehaviour
{
    readonly List<CharacterPresentationState> crew = new List<CharacterPresentationState>(4);
    Tower tower;
    TowerSupportMechanismPresentation mechanism;
    Coroutine archerResetRoutine;
    Coroutine supportRoutine;

    void Start()
    {
        tower = GetComponent<Tower>();
        mechanism = GetComponent<TowerSupportMechanismPresentation>();
        RefreshCrew();
        if (tower != null && tower.Type == TowerType.MachineGun)
            PlayDraw();
    }

    public void RefreshCrew()
    {
        crew.Clear();
        CharacterVisualIdentity[] identities = GetComponentsInChildren<CharacterVisualIdentity>(true);
        for (int i = 0; i < identities.Length; i++)
        {
            CharacterVisualIdentity identity = identities[i];
            if (identity == null || identity.faction != TroyFaction.Trojan) continue;
            CharacterPresentationState state = identity.GetComponent<CharacterPresentationState>();
            if (state == null) state = identity.gameObject.AddComponent<CharacterPresentationState>();
            if (!crew.Contains(state)) crew.Add(state);
        }
    }

    public void PlayTowerAttack(TowerType type)
    {
        EnsureCrew();
        if (mechanism == null) mechanism = GetComponent<TowerSupportMechanismPresentation>();
        mechanism?.PlayAttack(type);

        switch (type)
        {
            case TowerType.MachineGun:
                PlayRelease();
                if (archerResetRoutine != null) StopCoroutine(archerResetRoutine);
                archerResetRoutine = StartCoroutine(PrepareNextArrow());
                break;
            case TowerType.SpearThrower:
                PlayPoke();
                break;
            case TowerType.Cannon:
                PlayFire();
                RestartSupportRoutine(BallistaReloadCycle());
                break;
            case TowerType.Slow:
                PlayCast();
                RestartSupportRoutine(PriestChannelCycle());
                break;
            case TowerType.FireTower:
                PlayThrow();
                RestartSupportRoutine(FireKeeperCycle());
                break;
        }
    }

    public void PlayGuardBlock()
    {
        EnsureCrew();
        for (int i = 0; i < crew.Count; i++) crew[i]?.PlayBlock();
    }

    public void PlayGuardPoke()
    {
        EnsureCrew();
        for (int i = 0; i < crew.Count; i++) crew[i]?.PlayPoke();
    }

    void PlayDraw()
    {
        EnsureCrew();
        for (int i = 0; i < crew.Count; i++) crew[i]?.PlayDraw();
    }

    void PlayRelease()
    {
        for (int i = 0; i < crew.Count; i++) crew[i]?.PlayRelease();
    }

    void PlayPoke()
    {
        for (int i = 0; i < crew.Count; i++) crew[i]?.PlayPoke();
    }

    void PlayFire()
    {
        for (int i = 0; i < crew.Count; i++) crew[i]?.PlayFire();
    }

    void PlayReload()
    {
        for (int i = 0; i < crew.Count; i++) crew[i]?.PlayReload();
    }

    void PlayTension()
    {
        for (int i = 0; i < crew.Count; i++) crew[i]?.PlayTension();
    }

    void PlayCast()
    {
        for (int i = 0; i < crew.Count; i++) crew[i]?.PlayCast();
    }

    void PlayChannel()
    {
        for (int i = 0; i < crew.Count; i++) crew[i]?.PlayChannel();
    }

    void PlayThrow()
    {
        for (int i = 0; i < crew.Count; i++) crew[i]?.PlayThrow();
    }

    void PlayStoke()
    {
        for (int i = 0; i < crew.Count; i++) crew[i]?.PlayStoke();
    }

    IEnumerator PrepareNextArrow()
    {
        yield return new WaitForSeconds(.18f);
        PlayDraw();
        archerResetRoutine = null;
    }

    IEnumerator BallistaReloadCycle()
    {
        yield return new WaitForSeconds(.10f);
        PlayReload();
        yield return new WaitForSeconds(.18f);
        PlayTension();
        supportRoutine = null;
    }

    IEnumerator PriestChannelCycle()
    {
        yield return new WaitForSeconds(.14f);
        PlayChannel();
        supportRoutine = null;
    }

    IEnumerator FireKeeperCycle()
    {
        yield return new WaitForSeconds(.18f);
        PlayStoke();
        supportRoutine = null;
    }

    void RestartSupportRoutine(IEnumerator routine)
    {
        if (supportRoutine != null) StopCoroutine(supportRoutine);
        supportRoutine = StartCoroutine(routine);
    }

    void EnsureCrew()
    {
        if (crew.Count == 0) RefreshCrew();
    }
}
