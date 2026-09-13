using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TowerCrewAnimationBridge : MonoBehaviour
{
    readonly List<CharacterPresentationState> crew = new List<CharacterPresentationState>(4);
    Tower tower;
    Coroutine archerResetRoutine;

    void Start()
    {
        tower = GetComponent<Tower>();
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

    IEnumerator PrepareNextArrow()
    {
        yield return new WaitForSeconds(.18f);
        PlayDraw();
        archerResetRoutine = null;
    }

    void EnsureCrew()
    {
        if (crew.Count == 0) RefreshCrew();
    }
}
