using System;
using UnityEngine;

public sealed class CharacterWeaponSocketResolver : MonoBehaviour
{
    public const string BowGripSocketName = "Socket_BowGrip";
    public const string ArrowNockSocketName = "Socket_ArrowNock";
    public const string ArrowSocketName = "Socket_ArrowRelease";
    public const string SpearGripSocketName = "Socket_SpearGrip";
    public const string SpearSocketName = "Socket_SpearRelease";
    public const string SourceBowName = "SourceBow_Quaternius_MedievalWeapons";
    public const string SourceSpearName = "SourceSpear_Quaternius_MedievalWeapons";

    static readonly string[] RightHandHints =
    {
        "righthand",
        "right_hand",
        "hand_r",
        "mixamorig:righthand",
        "hand.r"
    };

    Animator animator;
    Transform bowGripSocket;
    Transform arrowNockSocket;
    Transform arrowSocket;
    Transform spearGripSocket;
    Transform spearSocket;
    Transform sourceBow;
    Transform sourceSpear;
    Transform rightHand;
    bool resolved;

    void Awake() => Refresh();

    public Vector3 ArrowReleasePoint()
    {
        EnsureResolved();
        if (arrowSocket != null) return arrowSocket.position;
        if (arrowNockSocket != null) return arrowNockSocket.position;
        if (sourceBow != null) return sourceBow.position;
        if (rightHand != null) return rightHand.position;
        return transform.TransformPoint(new Vector3(.18f, .95f, .28f));
    }

    public Vector3 SpearReleasePoint()
    {
        EnsureResolved();
        if (spearSocket != null) return spearSocket.position;
        if (sourceSpear != null) return sourceSpear.position;
        if (rightHand != null) return rightHand.position;
        return transform.TransformPoint(new Vector3(.22f, 1.10f, .42f));
    }

    public Transform ArrowNockAnchor()
    {
        EnsureResolved();
        return arrowNockSocket ?? arrowSocket ?? sourceBow ?? bowGripSocket ?? rightHand ?? transform;
    }

    public Transform BowRoot()
    {
        EnsureResolved();
        return sourceBow;
    }

    public Transform SpearRoot()
    {
        EnsureResolved();
        return sourceSpear;
    }

    public Transform RightHand()
    {
        EnsureResolved();
        return rightHand;
    }

    public void Refresh()
    {
        animator = GetComponentInChildren<Animator>();
        Transform[] all = GetComponentsInChildren<Transform>(true);
        bowGripSocket = FindExact(all, BowGripSocketName);
        arrowNockSocket = FindExact(all, ArrowNockSocketName);
        arrowSocket = FindExact(all, ArrowSocketName);
        spearGripSocket = FindExact(all, SpearGripSocketName);
        spearSocket = FindExact(all, SpearSocketName);
        sourceBow = FindExact(all, SourceBowName) ?? FindExact(all, "Bow");
        sourceSpear = FindExact(all, SourceSpearName) ?? FindExact(all, "Spear");
        rightHand = ResolveRightHand(all);
        resolved = true;
    }

    void EnsureResolved()
    {
        if (!resolved) Refresh();
    }

    Transform ResolveRightHand(Transform[] all)
    {
        if (animator != null && animator.isHuman)
        {
            Transform humanoidHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
            if (humanoidHand != null) return humanoidHand;
        }

        foreach (string hint in RightHandHints)
        {
            string wanted = hint.ToLowerInvariant();
            foreach (Transform candidate in all)
            {
                if (candidate == null) continue;
                if (candidate.name.ToLowerInvariant().Contains(wanted)) return candidate;
            }
        }

        return null;
    }

    static Transform FindExact(Transform[] all, string objectName)
    {
        foreach (Transform candidate in all)
        {
            if (candidate != null && string.Equals(candidate.name, objectName, StringComparison.Ordinal))
                return candidate;
        }
        return null;
    }
}
