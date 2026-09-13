using System.Collections;
using UnityEngine;

public sealed class TowerSupportMechanismPresentation : MonoBehaviour
{
    Tower tower;
    Transform ballistaBolt;
    Transform ballistaBow;
    Transform counterweight;
    Transform apolloFocus;
    Transform apolloDisc;
    Transform fireCore;

    Vector3 boltBasePosition;
    Vector3 bowBaseScale;
    Quaternion counterweightBaseRotation;
    Vector3 apolloFocusBaseScale;
    Quaternion apolloDiscBaseRotation;
    Vector3 fireCoreBaseScale;

    Coroutine activeRoutine;
    bool cached;

    void Start()
    {
        tower = GetComponent<Tower>();
        Cache();
    }

    public void PlayAttack(TowerType type)
    {
        Cache();
        if (activeRoutine != null) StopCoroutine(activeRoutine);

        switch (type)
        {
            case TowerType.Cannon:
                activeRoutine = StartCoroutine(BallistaCycle());
                break;
            case TowerType.Slow:
                activeRoutine = StartCoroutine(ApolloPulse());
                break;
            case TowerType.FireTower:
                activeRoutine = StartCoroutine(FirePulse());
                break;
        }
    }

    void Cache()
    {
        if (cached) return;
        cached = true;
        ballistaBolt = FindNamed("Ballista Bolt");
        ballistaBow = FindNamed("Ballista Bow");
        counterweight = FindNamed("Counterweight");
        apolloFocus = FindNamed("Apollo Focus");
        apolloDisc = FindNamed("Apollo Disc");
        fireCore = FindNamed("Fire Core");

        if (ballistaBolt != null) boltBasePosition = ballistaBolt.localPosition;
        if (ballistaBow != null) bowBaseScale = ballistaBow.localScale;
        if (counterweight != null) counterweightBaseRotation = counterweight.localRotation;
        if (apolloFocus != null) apolloFocusBaseScale = apolloFocus.localScale;
        if (apolloDisc != null) apolloDiscBaseRotation = apolloDisc.localRotation;
        if (fireCore != null) fireCoreBaseScale = fireCore.localScale;
    }

    IEnumerator BallistaCycle()
    {
        const float releaseDuration = .08f;
        float elapsed = 0f;
        while (elapsed < releaseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / releaseDuration);
            if (ballistaBow != null)
                ballistaBow.localScale = new Vector3(Mathf.Lerp(bowBaseScale.x * 1.08f, bowBaseScale.x * .90f, t), bowBaseScale.y, bowBaseScale.z);
            if (counterweight != null)
                counterweight.localRotation = counterweightBaseRotation * Quaternion.Euler(Mathf.Lerp(0f, 16f, t), 0f, 0f);
            yield return null;
        }

        if (ballistaBolt != null) ballistaBolt.gameObject.SetActive(false);
        yield return new WaitForSeconds(.10f);

        if (ballistaBolt != null)
        {
            ballistaBolt.gameObject.SetActive(true);
            ballistaBolt.localPosition = boltBasePosition + Vector3.back * .58f;
        }

        const float reloadDuration = .24f;
        elapsed = 0f;
        while (elapsed < reloadDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / reloadDuration));
            if (ballistaBolt != null)
                ballistaBolt.localPosition = Vector3.Lerp(boltBasePosition + Vector3.back * .58f, boltBasePosition, t);
            if (ballistaBow != null)
                ballistaBow.localScale = new Vector3(Mathf.Lerp(bowBaseScale.x * .90f, bowBaseScale.x * 1.08f, t), bowBaseScale.y, bowBaseScale.z);
            if (counterweight != null)
                counterweight.localRotation = counterweightBaseRotation * Quaternion.Euler(Mathf.Lerp(16f, 0f, t), 0f, 0f);
            yield return null;
        }

        if (ballistaBolt != null) ballistaBolt.localPosition = boltBasePosition;
        if (ballistaBow != null) ballistaBow.localScale = bowBaseScale;
        if (counterweight != null) counterweight.localRotation = counterweightBaseRotation;
        activeRoutine = null;
    }

    IEnumerator ApolloPulse()
    {
        const float duration = .34f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float pulse = 1f + Mathf.Sin(t * Mathf.PI) * .35f;
            if (apolloFocus != null) apolloFocus.localScale = apolloFocusBaseScale * pulse;
            if (apolloDisc != null) apolloDisc.localRotation = apolloDiscBaseRotation * Quaternion.Euler(0f, 0f, t * 95f);
            yield return null;
        }
        if (apolloFocus != null) apolloFocus.localScale = apolloFocusBaseScale;
        if (apolloDisc != null) apolloDisc.localRotation = apolloDiscBaseRotation;
        activeRoutine = null;
    }

    IEnumerator FirePulse()
    {
        if (fireCore == null)
        {
            activeRoutine = null;
            yield break;
        }

        const float duration = .28f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float pulse = 1f + Mathf.Sin(t * Mathf.PI) * .55f;
            fireCore.localScale = new Vector3(fireCoreBaseScale.x * pulse, fireCoreBaseScale.y * (1f + Mathf.Sin(t * Mathf.PI) * .75f), fireCoreBaseScale.z * pulse);
            yield return null;
        }
        fireCore.localScale = fireCoreBaseScale;
        activeRoutine = null;
    }

    Transform FindNamed(string objectName)
    {
        Transform[] all = GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < all.Length; i++)
            if (all[i] != null && all[i].name == objectName) return all[i];
        return null;
    }
}
