using System.Collections;
using UnityEngine;

public sealed class TowerSupportMechanismPresentation : MonoBehaviour
{
    Transform ballistaBolt;
    Transform ballistaBow;
    Transform counterweight;
    Transform apolloFocus;
    Transform apolloDisc;
    Transform fireCore;
    Transform archerBow;
    Transform aimedSpear;

    Vector3 boltBasePosition;
    Vector3 bowBaseScale;
    Quaternion counterweightBaseRotation;
    Vector3 apolloFocusBaseScale;
    Quaternion apolloDiscBaseRotation;
    Vector3 fireCoreBaseScale;
    Vector3 archerBowBaseScale;
    Vector3 aimedSpearBasePosition;

    Coroutine activeRoutine;
    bool cached;

    void Start()
    {
        Cache();
    }

    public void PlayAttack(TowerType type)
    {
        Cache();
        if (activeRoutine != null) StopCoroutine(activeRoutine);

        switch (type)
        {
            case TowerType.MachineGun:
                activeRoutine = StartCoroutine(ArcherSnap());
                StartCoroutine(PulseAccent("Archer Release Flash",PrimitiveType.Sphere,new Vector3(0f,1.42f,1.05f),new Vector3(.08f,.08f,.08f),new Vector3(.26f,.15f,.26f),new Color(.92f,.62f,.20f),.12f));
                break;
            case TowerType.SpearThrower:
                activeRoutine = StartCoroutine(SpearThrust());
                StartCoroutine(PulseAccent("Spear Thrust Flash",PrimitiveType.Sphere,new Vector3(0f,1.03f,1.32f),new Vector3(.07f,.07f,.07f),new Vector3(.22f,.12f,.22f),new Color(.82f,.54f,.18f),.13f));
                break;
            case TowerType.Cannon:
                activeRoutine = StartCoroutine(BallistaCycle());
                StartCoroutine(PulseAccent("Ballista Recoil Burst",PrimitiveType.Sphere,new Vector3(0f,1.12f,.88f),new Vector3(.12f,.09f,.12f),new Vector3(.38f,.20f,.38f),new Color(.69f,.49f,.24f),.18f));
                break;
            case TowerType.Slow:
                activeRoutine = StartCoroutine(ApolloPulse());
                StartCoroutine(PulseAccent("Apollo Solar Pulse",PrimitiveType.Cylinder,new Vector3(0f,1.60f,.04f),new Vector3(.18f,.025f,.18f),new Vector3(.75f,.035f,.75f),new Color(.96f,.75f,.22f),.30f));
                break;
            case TowerType.FireTower:
                activeRoutine = StartCoroutine(FirePulse());
                StartCoroutine(PulseAccent("Fire Throw Burst",PrimitiveType.Sphere,new Vector3(0f,1.32f,.20f),new Vector3(.14f,.18f,.14f),new Vector3(.42f,.58f,.42f),new Color(1f,.36f,.04f),.22f));
                break;
        }
    }

    void Cache()
    {
        if (cached) return;
        cached = true;

        ballistaBolt = FindNamedAny("Role_Ballista_Bolt","Ballista Bolt");
        ballistaBow = FindNamedAny("Role_Ballista_MainBow","Ballista Bow");
        counterweight = FindNamedAny("Counterweight");
        apolloFocus = FindNamedAny("Apollo Focus");
        apolloDisc = FindNamedAny("Role_Priest_ApolloDisc","Apollo Disc");
        fireCore = FindNamedAny("Role_Fire_MainFlame","Fire Core");
        archerBow = FindNamedAny("Role_Archer_GiantBow","Archer Aimed Bow","Archer Bow");
        aimedSpear = FindNamedAny("Aimed Spear","Role_SpearWall_ForwardSpear");

        if (ballistaBolt != null) boltBasePosition = ballistaBolt.localPosition;
        if (ballistaBow != null) bowBaseScale = ballistaBow.localScale;
        if (counterweight != null) counterweightBaseRotation = counterweight.localRotation;
        if (apolloFocus != null) apolloFocusBaseScale = apolloFocus.localScale;
        if (apolloDisc != null) apolloDiscBaseRotation = apolloDisc.localRotation;
        if (fireCore != null) fireCoreBaseScale = fireCore.localScale;
        if (archerBow != null) archerBowBaseScale = archerBow.localScale;
        if (aimedSpear != null) aimedSpearBasePosition = aimedSpear.localPosition;
    }

    IEnumerator ArcherSnap()
    {
        if (archerBow == null)
        {
            activeRoutine = null;
            yield break;
        }

        const float duration = .16f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float kick = Mathf.Sin(t * Mathf.PI);
            archerBow.localScale = new Vector3(
                archerBowBaseScale.x * (1f + kick * .16f),
                archerBowBaseScale.y * (1f - kick * .10f),
                archerBowBaseScale.z * (1f + kick * .08f));
            yield return null;
        }
        archerBow.localScale = archerBowBaseScale;
        activeRoutine = null;
    }

    IEnumerator SpearThrust()
    {
        if (aimedSpear == null)
        {
            activeRoutine = null;
            yield break;
        }

        const float duration = .18f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float thrust = Mathf.Sin(t * Mathf.PI);
            aimedSpear.localPosition = aimedSpearBasePosition + Vector3.forward * (.24f * thrust);
            yield return null;
        }
        aimedSpear.localPosition = aimedSpearBasePosition;
        activeRoutine = null;
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
                ballistaBow.localScale = new Vector3(Mathf.Lerp(bowBaseScale.x * 1.10f, bowBaseScale.x * .84f, t), bowBaseScale.y, bowBaseScale.z);
            if (counterweight != null)
                counterweight.localRotation = counterweightBaseRotation * Quaternion.Euler(Mathf.Lerp(0f, 20f, t), 0f, 0f);
            yield return null;
        }

        if (ballistaBolt != null) ballistaBolt.gameObject.SetActive(false);
        yield return new WaitForSeconds(.10f);

        if (ballistaBolt != null)
        {
            ballistaBolt.gameObject.SetActive(true);
            ballistaBolt.localPosition = boltBasePosition + Vector3.back * .68f;
        }

        const float reloadDuration = .28f;
        elapsed = 0f;
        while (elapsed < reloadDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / reloadDuration));
            if (ballistaBolt != null)
                ballistaBolt.localPosition = Vector3.Lerp(boltBasePosition + Vector3.back * .68f, boltBasePosition, t);
            if (ballistaBow != null)
                ballistaBow.localScale = new Vector3(Mathf.Lerp(bowBaseScale.x * .84f, bowBaseScale.x * 1.10f, t), bowBaseScale.y, bowBaseScale.z);
            if (counterweight != null)
                counterweight.localRotation = counterweightBaseRotation * Quaternion.Euler(Mathf.Lerp(20f, 0f, t), 0f, 0f);
            yield return null;
        }

        if (ballistaBolt != null) ballistaBolt.localPosition = boltBasePosition;
        if (ballistaBow != null) ballistaBow.localScale = bowBaseScale;
        if (counterweight != null) counterweight.localRotation = counterweightBaseRotation;
        activeRoutine = null;
    }

    IEnumerator ApolloPulse()
    {
        const float duration = .40f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float pulse = 1f + Mathf.Sin(t * Mathf.PI) * .48f;
            if (apolloFocus != null) apolloFocus.localScale = apolloFocusBaseScale * pulse;
            if (apolloDisc != null) apolloDisc.localRotation = apolloDiscBaseRotation * Quaternion.Euler(0f, 0f, t * 145f);
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

        const float duration = .32f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float pulse = Mathf.Sin(t * Mathf.PI);
            fireCore.localScale = new Vector3(
                fireCoreBaseScale.x * (1f + pulse * .72f),
                fireCoreBaseScale.y * (1f + pulse * 1.00f),
                fireCoreBaseScale.z * (1f + pulse * .72f));
            yield return null;
        }
        fireCore.localScale = fireCoreBaseScale;
        activeRoutine = null;
    }

    IEnumerator PulseAccent(string name,PrimitiveType type,Vector3 localPosition,Vector3 startScale,Vector3 endScale,Color color,float duration)
    {
        GameObject accent = GameObject.CreatePrimitive(type);
        accent.name = name;
        accent.transform.SetParent(transform,false);
        accent.transform.localPosition = localPosition;
        accent.transform.localScale = startScale;
        Collider collider = accent.GetComponent<Collider>();
        if (collider != null) Destroy(collider);
        TowerFactory.SetColor(accent,color);

        float elapsed = 0f;
        while (elapsed < duration && accent != null)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f,1f,Mathf.Clamp01(elapsed / duration));
            accent.transform.localScale = Vector3.Lerp(startScale,endScale,t);
            yield return null;
        }
        if (accent != null) Destroy(accent);
    }

    Transform FindNamedAny(params string[] names)
    {
        Transform[] all = GetComponentsInChildren<Transform>(true);
        for (int n = 0; n < names.Length; n++)
            for (int i = 0; i < all.Length; i++)
                if (all[i] != null && all[i].name == names[n]) return all[i];
        return null;
    }
}
