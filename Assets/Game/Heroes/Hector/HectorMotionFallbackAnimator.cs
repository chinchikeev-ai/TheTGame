using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lightweight runtime animation safety net for Hector.
/// It is active only when the loaded visual has no enabled Animator Controller.
/// Authored animation always wins when a valid controller is present.
/// </summary>
public sealed class HectorMotionFallbackAnimator : MonoBehaviour
{
    struct PartPose
    {
        public Transform transform;
        public Vector3 localPosition;
        public Quaternion localRotation;

        public PartPose(Transform transform)
        {
            this.transform = transform;
            localPosition = transform != null ? transform.localPosition : Vector3.zero;
            localRotation = transform != null ? transform.localRotation : Quaternion.identity;
        }
    }

    HectorController hector;
    Animator animator;

    PartPose torso;
    PartPose head;
    PartPose leftArm;
    PartPose rightArm;
    PartPose leftLeg;
    PartPose rightLeg;

    bool rigCached;
    bool hasTargets;
    Vector3 previousPosition;
    float motionBlend;

    public bool UsingProceduralFallback => animator == null || !animator.enabled || animator.runtimeAnimatorController == null;
    public bool HasAnimationTargets => hasTargets;

    public void Initialize(HectorController controller)
    {
        hector = controller;
        previousPosition = transform.position;
        CacheRig();
    }

    void Awake()
    {
        if (hector == null) hector = GetComponent<HectorController>();
        CacheRig();
        previousPosition = transform.position;
    }

    void OnEnable()
    {
        previousPosition = transform.position;
    }

    void LateUpdate()
    {
        if (hector == null) hector = GetComponent<HectorController>();
        if (!rigCached) CacheRig();
        if (!UsingProceduralFallback || !hasTargets || hector == null) return;

        RestoreBindPose();

        Vector3 worldDelta = transform.position - previousPosition;
        worldDelta.y = 0f;
        previousPosition = transform.position;
        bool moving = !hector.IsDowned && worldDelta.sqrMagnitude > .00025f && hector.CanMove;
        motionBlend = Mathf.MoveTowards(motionBlend, moving ? 1f : 0f, Time.deltaTime * 8f);

        if (hector.IsDowned)
        {
            ApplyDownedPose();
            return;
        }

        ApplyIdleBreathing();
        if (motionBlend > .001f) ApplyWalkCycle(motionBlend);
        ApplyActionPose(hector.CurrentAction);
    }

    void CacheRig()
    {
        animator = GetComponentInChildren<Animator>(true);

        Transform torsoTransform = null;
        Transform headTransform = null;
        Transform leftArmTransform = null;
        Transform rightArmTransform = null;
        Transform leftLegTransform = null;
        Transform rightLegTransform = null;

        if (animator != null && animator.isHuman && animator.avatar != null && animator.avatar.isValid)
        {
            torsoTransform = FirstNonNull(
                animator.GetBoneTransform(HumanBodyBones.Chest),
                animator.GetBoneTransform(HumanBodyBones.Spine),
                animator.GetBoneTransform(HumanBodyBones.UpperChest));
            headTransform = animator.GetBoneTransform(HumanBodyBones.Head);
            leftArmTransform = FirstNonNull(
                animator.GetBoneTransform(HumanBodyBones.LeftUpperArm),
                animator.GetBoneTransform(HumanBodyBones.LeftLowerArm));
            rightArmTransform = FirstNonNull(
                animator.GetBoneTransform(HumanBodyBones.RightUpperArm),
                animator.GetBoneTransform(HumanBodyBones.RightLowerArm));
            leftLegTransform = FirstNonNull(
                animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg),
                animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg));
            rightLegTransform = FirstNonNull(
                animator.GetBoneTransform(HumanBodyBones.RightUpperLeg),
                animator.GetBoneTransform(HumanBodyBones.RightLowerLeg));
        }

        // Procedural runtime fallback model uses these exact names.
        torsoTransform ??= FindRecursive(transform, "Torso");
        headTransform ??= FindRecursive(transform, "Head");
        leftArmTransform ??= FindRecursive(transform, "ArmL");
        rightArmTransform ??= FindRecursive(transform, "ArmR");
        leftLegTransform ??= FindRecursive(transform, "LegL");
        rightLegTransform ??= FindRecursive(transform, "LegR");

        torso = new PartPose(torsoTransform);
        head = new PartPose(headTransform);
        leftArm = new PartPose(leftArmTransform);
        rightArm = new PartPose(rightArmTransform);
        leftLeg = new PartPose(leftLegTransform);
        rightLeg = new PartPose(rightLegTransform);

        hasTargets = torso.transform != null || leftArm.transform != null || rightArm.transform != null || leftLeg.transform != null || rightLeg.transform != null;
        rigCached = true;

        RuntimeFileLogger.Event("HECTOR_ANIMATION",
            UsingProceduralFallback
                ? $"Procedural fallback active; targets={(hasTargets ? "ready" : "missing")}."
                : $"Authored Animator active: {animator.runtimeAnimatorController.name}.");
    }

    void RestoreBindPose()
    {
        Restore(torso);
        Restore(head);
        Restore(leftArm);
        Restore(rightArm);
        Restore(leftLeg);
        Restore(rightLeg);
    }

    void ApplyIdleBreathing()
    {
        float breathe = Mathf.Sin(Time.time * 2.15f);
        AddPosition(torso, new Vector3(0f, breathe * .012f, 0f));
        AddRotation(torso, Quaternion.Euler(breathe * 1.2f, 0f, 0f));
        AddRotation(head, Quaternion.Euler(-breathe * .7f, 0f, 0f));
    }

    void ApplyWalkCycle(float blend)
    {
        float phase = Time.time * 8.2f;
        float swing = Mathf.Sin(phase) * 27f * blend;
        float counter = Mathf.Sin(phase + Mathf.PI) * 27f * blend;
        float bounce = Mathf.Abs(Mathf.Sin(phase)) * .025f * blend;

        AddRotation(leftLeg, Quaternion.Euler(swing, 0f, 0f));
        AddRotation(rightLeg, Quaternion.Euler(counter, 0f, 0f));
        AddRotation(leftArm, Quaternion.Euler(counter * .72f, 0f, 0f));
        AddRotation(rightArm, Quaternion.Euler(swing * .72f, 0f, 0f));
        AddPosition(torso, new Vector3(0f, bounce, 0f));
        AddRotation(torso, Quaternion.Euler(0f, 0f, Mathf.Sin(phase * .5f) * 2f * blend));
    }

    void ApplyActionPose(HectorCombatAction action)
    {
        float pulse = Mathf.Sin(Time.time * 13f);
        switch (action)
        {
            case HectorCombatAction.BasicAttack:
                AddRotation(torso, Quaternion.Euler(7f, 18f, 0f));
                AddRotation(rightArm, Quaternion.Euler(-64f + pulse * 8f, 12f, -12f));
                AddRotation(leftArm, Quaternion.Euler(18f, 0f, 16f));
                break;
            case HectorCombatAction.WarCry:
                AddRotation(torso, Quaternion.Euler(-8f, 0f, 0f));
                AddRotation(leftArm, Quaternion.Euler(-78f, 0f, -30f));
                AddRotation(rightArm, Quaternion.Euler(-78f, 0f, 30f));
                AddRotation(head, Quaternion.Euler(-14f, 0f, 0f));
                break;
            case HectorCombatAction.ShieldWall:
                AddRotation(torso, Quaternion.Euler(8f, -8f, 0f));
                AddRotation(leftArm, Quaternion.Euler(-54f, 0f, -34f));
                AddRotation(rightArm, Quaternion.Euler(-18f, 0f, 14f));
                break;
            case HectorCombatAction.SpearThrow:
                AddRotation(torso, Quaternion.Euler(4f, 24f, 0f));
                AddRotation(rightArm, Quaternion.Euler(-112f + pulse * 7f, 8f, 14f));
                AddRotation(leftArm, Quaternion.Euler(28f, 0f, -20f));
                break;
            case HectorCombatAction.Ultimate:
                AddRotation(torso, Quaternion.Euler(-12f, 0f, 0f));
                AddRotation(leftArm, Quaternion.Euler(-118f, 0f, -32f));
                AddRotation(rightArm, Quaternion.Euler(-118f, 0f, 32f));
                AddRotation(head, Quaternion.Euler(-20f, 0f, 0f));
                break;
        }
    }

    void ApplyDownedPose()
    {
        AddRotation(torso, Quaternion.Euler(8f, 0f, 72f));
        AddRotation(head, Quaternion.Euler(0f, 0f, 42f));
        AddRotation(leftArm, Quaternion.Euler(38f, 0f, -28f));
        AddRotation(rightArm, Quaternion.Euler(-28f, 0f, 32f));
        AddRotation(leftLeg, Quaternion.Euler(18f, 0f, -8f));
        AddRotation(rightLeg, Quaternion.Euler(-16f, 0f, 8f));
    }

    static void Restore(PartPose pose)
    {
        if (pose.transform == null) return;
        pose.transform.localPosition = pose.localPosition;
        pose.transform.localRotation = pose.localRotation;
    }

    static void AddPosition(PartPose pose, Vector3 offset)
    {
        if (pose.transform == null) return;
        pose.transform.localPosition += offset;
    }

    static void AddRotation(PartPose pose, Quaternion offset)
    {
        if (pose.transform == null) return;
        pose.transform.localRotation = pose.transform.localRotation * offset;
    }

    static Transform FirstNonNull(params Transform[] candidates)
    {
        for (int i = 0; i < candidates.Length; i++)
            if (candidates[i] != null) return candidates[i];
        return null;
    }

    static Transform FindRecursive(Transform root, string exactName)
    {
        if (root == null) return null;
        if (root.name == exactName) return root;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform found = FindRecursive(root.GetChild(i), exactName);
            if (found != null) return found;
        }
        return null;
    }
}
