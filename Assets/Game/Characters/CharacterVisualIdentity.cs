using UnityEngine;

public enum TroyFaction
{
    Trojan,
    Greek
}

public enum TroyVisualRole
{
    Infantry,
    Runner,
    Heavy,
    ShieldBearer,
    Archer,
    Commander,
    Hero
}

public class CharacterVisualIdentity : MonoBehaviour
{
    public TroyFaction faction;
    public TroyVisualRole role;
    public string characterId;
    public string primaryWeapon;
    public string offhandItem;
}
