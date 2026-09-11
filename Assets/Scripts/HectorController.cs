using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class HectorController : MonoBehaviour
{
    public bool Selected { get; private set; }
    public float moveSpeed = 6f;
    public float attackRange = 2.4f;
    public float attackDamage = 45f;
    public float attackRate = 1.1f;
    public float warCryRadius = 5.2f;
    public float warCryDuration = 10f;
    public float warCryCooldown = 20f;

    Vector3 destination;
    float nextAttack;
    float nextWarCry;
    Renderer body;

    void Start()
    {
        destination = transform.position;
        body = GetComponentInChildren<Renderer>();
        RefreshColor();
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameEnded) return;
        HandleSelectionAndMove();
        Move();
        AutoAttack();
        if (Selected && ReadWarCry()) UseWarCry();
    }

    void HandleSelectionAndMove()
    {
        Camera cam = Camera.main;
        if (cam == null) return;
        if (ReadPrimaryClick() && (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject()))
        {
            Ray ray = cam.ScreenPointToRay(ReadPointer());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 200f))
            {
                HectorController h = hit.collider.GetComponentInParent<HectorController>();
                Selected = h == this;
                RefreshColor();
            }
        }
        if (Selected && ReadSecondaryClick() && (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject()))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = cam.ScreenPointToRay(ReadPointer());
            float enter;
            if (plane.Raycast(ray, out enter))
            {
                destination = ray.GetPoint(enter);
                destination.y = transform.position.y;
            }
        }
    }

    void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
    }

    void AutoAttack()
    {
        if (Time.time < nextAttack) return;
        Enemy best = null;
        float bestSq = attackRange * attackRange;
        foreach (Enemy enemy in EnemyRegistry.All)
        {
            if (enemy == null) continue;
            float d = (enemy.transform.position - transform.position).sqrMagnitude;
            if (d < bestSq) { bestSq = d; best = enemy; }
        }
        if (best == null) return;
        nextAttack = Time.time + 1f / attackRate;
        best.TakeDamage(attackDamage);
        if (RuntimeEffects.Instance != null) RuntimeEffects.Instance.PlayHit(best.transform.position, false);
    }

    public void UseWarCry()
    {
        if (Time.time < nextWarCry) return;
        nextWarCry = Time.time + warCryCooldown;
        foreach (Tower tower in FindObjectsByType<Tower>(FindObjectsSortMode.None))
        {
            if (tower != null && Vector3.Distance(tower.transform.position, transform.position) <= warCryRadius)
                tower.ApplyWarCry(warCryDuration, 1.15f, 1.30f);
        }
        if (RuntimeEffects.Instance != null) RuntimeEffects.Instance.PlayHit(transform.position, true);
    }

    public float WarCryCooldownRemaining => Mathf.Max(0f, nextWarCry - Time.time);

    void RefreshColor()
    {
        if (body == null) return;
        body.material.color = Selected ? new Color(.95f,.78f,.18f) : new Color(.72f,.48f,.12f);
    }

    Vector2 ReadPointer()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
        return Input.mousePosition;
#endif
    }

    bool ReadPrimaryClick()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    bool ReadSecondaryClick()
    {
#if ENABLE_INPUT_SYSTEM
        return Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(1);
#endif
    }

    bool ReadWarCry()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Q);
#endif
    }
}
