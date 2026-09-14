using UnityEngine;

public sealed class CharacterWeaponPresentation : MonoBehaviour
{
    const string NockedArrowName = "NockedArrow_Candidate";

    CharacterWeaponSocketResolver sockets;
    GameObject nockedArrow;
    Renderer[] spearRenderers;
    bool spearReleased;

    public bool SpearReleased => spearReleased;

    void Awake()
    {
        Refresh();
    }

    void OnDisable()
    {
        RestoreSpear();
        SetNockedArrowVisible(false);
    }

    public void Refresh()
    {
        sockets = GetComponent<CharacterWeaponSocketResolver>();
        if (sockets == null) sockets = gameObject.AddComponent<CharacterWeaponSocketResolver>();
        sockets.Refresh();

        Transform spearRoot = sockets.SpearRoot();
        spearRenderers = spearRoot != null ? spearRoot.GetComponentsInChildren<Renderer>(true) : null;
        spearReleased = false;
        SetSpearVisible(true);

        Transform existing = FindChildByName(transform, NockedArrowName);
        if (existing != null) nockedArrow = existing.gameObject;
        if (nockedArrow != null)
        {
            Transform anchor = sockets.ArrowNockAnchor();
            if (anchor != null && nockedArrow.transform.parent != anchor)
                nockedArrow.transform.SetParent(anchor, false);
        }
    }

    public void PrepareBow()
    {
        EnsureNockedArrow();
        SetNockedArrowVisible(true);
    }

    public Vector3 ReleaseArrow()
    {
        Vector3 point = sockets != null ? sockets.ArrowReleasePoint() : transform.position;
        SetNockedArrowVisible(false);
        return point;
    }

    public void RestoreArrow()
    {
        PrepareBow();
    }

    public Vector3 ReleaseSpear()
    {
        Vector3 point = sockets != null ? sockets.SpearReleasePoint() : transform.position;
        spearReleased = true;
        SetSpearVisible(false);
        return point;
    }

    public void RestoreSpear()
    {
        spearReleased = false;
        SetSpearVisible(true);
    }

    void EnsureNockedArrow()
    {
        if (nockedArrow != null) return;
        if (sockets == null) Refresh();

        Transform anchor = sockets != null ? sockets.ArrowNockAnchor() : transform;
        if (anchor == null) anchor = transform;

        nockedArrow = new GameObject(NockedArrowName);
        nockedArrow.transform.SetParent(anchor, false);
        nockedArrow.transform.localPosition = Vector3.zero;
        nockedArrow.transform.localRotation = Quaternion.identity;
        nockedArrow.transform.localScale = Vector3.one;

        Transform shaft = CreatePart("Shaft", PrimitiveType.Cylinder, nockedArrow.transform);
        shaft.localPosition = new Vector3(0f, 0f, .03f);
        shaft.localRotation = Quaternion.Euler(90f, 0f, 0f);
        shaft.localScale = new Vector3(.014f, .34f, .014f);

        Transform tip = CreatePart("Tip", PrimitiveType.Cube, nockedArrow.transform);
        tip.localPosition = new Vector3(0f, 0f, .42f);
        tip.localRotation = Quaternion.Euler(0f, 0f, 45f);
        tip.localScale = new Vector3(.045f, .045f, .10f);

        Transform featherA = CreatePart("Fletching A", PrimitiveType.Cube, nockedArrow.transform);
        featherA.localPosition = new Vector3(.035f, 0f, -.28f);
        featherA.localRotation = Quaternion.Euler(0f, 0f, 24f);
        featherA.localScale = new Vector3(.045f, .012f, .14f);

        Transform featherB = CreatePart("Fletching B", PrimitiveType.Cube, nockedArrow.transform);
        featherB.localPosition = new Vector3(-.035f, 0f, -.28f);
        featherB.localRotation = Quaternion.Euler(0f, 0f, -24f);
        featherB.localScale = new Vector3(.045f, .012f, .14f);
    }

    void SetNockedArrowVisible(bool visible)
    {
        if (nockedArrow != null) nockedArrow.SetActive(visible);
    }

    void SetSpearVisible(bool visible)
    {
        if (spearRenderers == null) return;
        foreach (Renderer renderer in spearRenderers)
            if (renderer != null) renderer.enabled = visible;
    }

    static Transform CreatePart(string name, PrimitiveType primitive, Transform parent)
    {
        GameObject part = GameObject.CreatePrimitive(primitive);
        part.name = name;
        part.transform.SetParent(parent, false);
        Collider collider = part.GetComponent<Collider>();
        if (collider != null) Object.Destroy(collider);
        return part.transform;
    }

    static Transform FindChildByName(Transform root, string objectName)
    {
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        foreach (Transform candidate in all)
            if (candidate != null && candidate.name == objectName) return candidate;
        return null;
    }
}
