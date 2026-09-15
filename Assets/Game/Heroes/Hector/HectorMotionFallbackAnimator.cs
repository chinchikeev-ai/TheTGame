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
    HectorCombatAction trackedAction;
    float actionStartedAt;
    float hitReactionUntil;
    float reviveUntil;
    bool blockedHitReaction;

    public bool UsingProceduralFallback => animator == null || !animator.enabled || animator.runtimeAnimatorController == null;
    public bool HasAnimationTargets => hasTargets;

    public void Initialize(HectorController controller)
    {
        hector = controller;
        previousPosition = transform.position;
        trackedAction = HectorCombatAction.None;
        actionStartedAt = Time.time;
        CacheRig();
    }

    public void PlayHitReaction(bool blocked)
    {
        blockedHitReaction = blocked;
        hitReactionUntil = Time.time + (blocked ? .18f : .24f);
    }

    public void PlayRevive()
    {
        reviveUntil = Time.time + .58f;
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
        trackedAction = HectorCombatAction.None;
        actionStartedAt = Time.time;
    }

    void LateUpdate()
    {
        if (hector == null) hector = GetComponent<HectorController>();
        if (!rigCached) CacheRig();
        if (!UsingProceduralFallback || !hasTargets || hector == null) return;

        RestoreBindPose();
        TrackAction();

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

        if (Time.time < reviveUntil)
        {
            ApplyRevivePose();
            return;
        }

        if (hector.CurrentAction != HectorCombatAction.None)
            ApplyActionPose(hector.CurrentAction, ActionProgress(hector.CurrentAction));
        else if (hector.ShieldWallActive)
            ApplyShieldHold();

        if (Time.time < hitReactionUntil)
            ApplyHitReaction(blockedHitReaction, 1f - Mathf.Clamp01((hitReactionUntil - Time.time) / (blockedHitReaction ? .18f : .24f)));
    }

    void TrackAction()
    {
        if (trackedAction == hector.CurrentAction) return;
        trackedAction = hector.CurrentAction;
        actionStartedAt = Time.time;
    }

    float ActionProgress(HectorCombatAction action)
    {
        float duration = ActionDuration(action);
        if (duration <= .01f) return 1f;
        return Mathf.Clamp01((Time.time - actionStartedAt) / duration);
    }

    float ActionDuration(HectorCombatAction action)
    {
        switch (action)
        {
            case HectorCombatAction.BasicAttack: return Mathf.Max(.05f, hector.basicAttackActionLock);
            case HectorCombatAction.WarCry: return Mathf.Max(.05f, hector.warCryActionLock);
            case HectorCombatAction.ShieldWall: return Mathf.Max(.05f, hector.shieldWallActionLock);
            case HectorCombatAction.SpearThrow: return Mathf.Max(.05f, hector.spearThrowActionLock);
            case HectorCombatAction.Ultimate: return Mathf.Max(.05f, hector.ultimateActionLock);
            default: return .5f;
        }
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
        AddRotation(leftArm, Quaternion.Euler(-8f, 0f, -10f));
        AddRotation(rightArm, Quaternion.Euler(-10f, 0f, 8f));
    }

    void ApplyWalkCycle(float blend)
    {
        float phase = Time.time * 8.2f;
        float swing = Mathf.Sin(phase) * 27f * blend;
        float counter = Mathf.Sin(phase + Mathf.PI) * 27f * blend;
        float bounce = Mathf.Abs(Mathf.Sin(phase)) * .025f * blend;

        AddRotation(leftLeg, Quaternion.Euler(swing, 0f, 0f));
        AddRotation(rightLeg, Quaternion.Euler(counter, 0f, 0f));
        AddRotation(leftArm, Quaternion.Euler(counter * .42f, 0f, 0f));
        AddRotation(rightArm, Quaternion.Euler(swing * .42f, 0f, 0f));
        AddPosition(torso, new Vector3(0f, bounce, 0f));
        AddRotation(torso, Quaternion.Euler(5f * blend, 0f, Mathf.Sin(phase * .5f) * 2f * blend));
    }

    void ApplyActionPose(HectorCombatAction action, float t)
    {
        switch (action)
        {
            case HectorCombatAction.BasicAttack:
                ApplyBasicAttack(t);
                break;
            case HectorCombatAction.WarCry:
                ApplyWarCry(t);
                break;
            case HectorCombatAction.ShieldWall:
                ApplyShieldDeploy(t);
                break;
            case HectorCombatAction.SpearThrow:
                ApplySpearThrow(t);
                break;
            case HectorCombatAction.Ultimate:
                ApplyUltimate(t);
                break;
        }
    }

    void ApplyBasicAttack(float t)
    {
        float strike = Bell(t, .16f, .62f);
        float recover = Smooth01((t - .62f) / .38f);
        float amount = strike * (1f - recover * .35f);
        AddRotation(torso, Quaternion.Euler(7f * amount, 20f * amount, 0f));
        AddRotation(rightArm, Quaternion.Euler(-88f * amount, 16f * amount, -14f * amount));
        AddRotation(leftArm, Quaternion.Euler(22f * amount, 0f, 18f * amount));
        AddRotation(rightLeg, Quaternion.Euler(-12f * amount, 0f, 0f));
        AddPosition(torso, new Vector3(0f, 0f, .07f * amount));
    }

    void ApplyWarCry(float t)
    {
        float rise = Smooth01(t / .34f);
        float settle = 1f - Smooth01((t - .68f) / .32f);
        float amount = Mathf.Clamp01(rise * settle);
        AddRotation(torso, Quaternion.Euler(-10f * amount, 0f, 0f));
        AddRotation(leftArm, Quaternion.Euler(-86f * amount, 0f, -34f * amount));
        AddRotation(rightArm, Quaternion.Euler(-72f * amount, 0f, 38f * amount));
        AddRotation(head, Quaternion.Euler(-18f * amount, 0f, 0f));
        AddPosition(torso, new Vector3(0f, .04f * amount, 0f));
    }

    void ApplyShieldDeploy(float t)
    {
        float brace = Smooth01(t / .34f);
        float settle = 1f - Smooth01((t - .78f) / .22f) * .15f;
        ApplyShieldPose(brace * settle, true);
    }

    void ApplyShieldHold()
    {
        float breathe = .94f + Mathf.Sin(Time.time * 4.1f) * .025f;
        ApplyShieldPose(breathe, false);
    }

    void ApplyShieldPose(float amount, bool deploying)
    {
        AddRotation(torso, Quaternion.Euler(12f * amount, -8f * amount, 0f));
        AddRotation(leftArm, Quaternion.Euler(-72f * amount, 2f * amount, -42f * amount));
        AddRotation(rightArm, Quaternion.Euler(-24f * amount, 0f, 16f * amount));
        AddRotation(leftLeg, Quaternion.Euler(12f * amount, 0f, -5f * amount));
        AddRotation(rightLeg, Quaternion.Euler(-9f * amount, 0f, 5f * amount));
        AddPosition(torso, new Vector3(0f, -.025f * amount, deploying ? .02f * amount : 0f));
    }

    void ApplySpearThrow(float t)
    {
        float windup = Smooth01(t / .43f);
        float release = Smooth01((t - .43f) / .20f);
        float recover = Smooth01((t - .68f) / .32f);

        float armBack = windup * (1f - release);
        float armForward = release * (1f - recover);
        float twist = (windup * 30f) - (release * 48f) + (recover * 18f);

        AddRotation(torso, Quaternion.Euler(5f * windup, twist, 0f));
        AddRotation(rightArm, Quaternion.Euler(-126f * armBack + 72f * armForward, 10f, 18f));
        AddRotation(leftArm, Quaternion.Euler(32f * windup - 18f * armForward, 0f, -24f * windup));
        AddRotation(rightLeg, Quaternion.Euler(-18f * windup + 10f * release, 0f, 0f));
        AddRotation(leftLeg, Quaternion.Euler(8f * windup, 0f, 0f));
        AddPosition(torso, new Vector3(0f, 0f, .09f * armForward));
    }

    void ApplyUltimate(float t)
    {
        float gather = Smooth01(t / .40f);
        float burst = Smooth01((t - .40f) / .18f);
        float recover = Smooth01((t - .68f) / .32f);
        float wide = Mathf.Max(gather * (1f - burst * .25f), burst * (1f - recover));

        AddRotation(torso, Quaternion.Euler(-15f * wide + 10f * burst, 0f, 0f));
        AddRotation(leftArm, Quaternion.Euler(-126f * wide, 0f, -38f * wide));
        AddRotation(rightArm, Quaternion.Euler(-126f * wide, 0f, 38f * wide));
        AddRotation(head, Quaternion.Euler(-22f * wide + 10f * burst, 0f, 0f));
        AddRotation(leftLeg, Quaternion.Euler(8f * wide, 0f, -5f * wide));
        AddRotation(rightLeg, Quaternion.Euler(8f * wide, 0f, 5f * wide));
        AddPosition(torso, new Vector3(0f, .055f * wide - .025f * burst, 0f));
    }

    void ApplyHitReaction(bool blocked, float t)
    {
        float pulse = Mathf.Sin(Mathf.Clamp01(t) * Mathf.PI);
        if (blocked)
        {
            AddRotation(torso, Quaternion.Euler(-4f * pulse, -7f * pulse, 0f));
            AddRotation(leftArm, Quaternion.Euler(-16f * pulse, 0f, -12f * pulse));
            return;
        }

        AddRotation(torso, Quaternion.Euler(-10f * pulse, 0f, 7f * pulse));
        AddRotation(head, Quaternion.Euler(8f * pulse, -6f * pulse, 0f));
        AddRotation(rightArm, Quaternion.Euler(12f * pulse, 0f, 8f * pulse));
    }

    void ApplyRevivePose()
    {
        float remaining = Mathf.Clamp01((reviveUntil - Time.time) / .58f);
        float t = 1f - remaining;
        float lift = Smooth01(t);
        float fall = 1f - lift;
        AddRotation(torso, Quaternion.Euler(18f * fall, 0f, 34f * fall));
        AddRotation(head, Quaternion.Euler(8f * fall, 0f, 18f * fall));
        AddRotation(leftArm, Quaternion.Euler(28f * fall, 0f, -18f * fall));
        AddRotation(rightArm, Quaternion.Euler(-18f * fall, 0f, 22f * fall));
        AddPosition(torso, new Vector3(0f, -.08f * fall, 0f));
    }

    void ApplyDownedPose()
    {
        AddRotation(torso, Quaternion.Euler(8f, 0f, 72f));
        AddRotation(head, Quaternion.Euler(0f, 0f, 42f));
        AddRotation(leftArm, Quaternion.Euler(38f, 0f, -28f));
        AddRotation(rightArm, Quaternion.Euler(-28f, 0f, 32f));
        AddRotation(leftLeg, Quaternion.Euler(18f, 0f, -8f));
        AddRotation(rightLeg, Quaternion.Euler(-16f, 0f, 8f));
        AddPosition(torso, new Vector3(0f, -.12f, 0f));
    }

    static float Smooth01(float value)
    {
        value = Mathf.Clamp01(value);
        return value * value * (3f - 2f * value);
    }

    static float Bell(float value, float start, float end)
    {
        float up = Smooth01((value - start) / Mathf.Max(.001f, (end - start) * .45f));
        float down = 1f - Smooth01((value - (start + (end - start) * .55f)) / Mathf.Max(.001f, (end - start) * .45f));
        return Mathf.Clamp01(up * down);
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
