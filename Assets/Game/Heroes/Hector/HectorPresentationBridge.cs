using UnityEngine;

public class HectorPresentationBridge : MonoBehaviour
{
    HectorController hector;
    CharacterPresentationState presentation;
    Vector3 previousPosition;
    bool previousDowned;

    public void Initialize(HectorController controller)
    {
        hector = controller;
        presentation = GetComponent<CharacterPresentationState>();
        if (presentation == null) presentation = gameObject.AddComponent<CharacterPresentationState>();
        previousPosition = transform.position;
        previousDowned = hector != null && hector.IsDowned;
        presentation.SetDowned(previousDowned);
    }

    void LateUpdate()
    {
        if (hector == null) return;

        bool downed = hector.IsDowned;
        if (downed != previousDowned)
        {
            presentation.SetDowned(downed);
            previousDowned = downed;
        }

        Vector3 delta = transform.position - previousPosition;
        delta.y = 0f;
        presentation.SetMoving(!downed && delta.sqrMagnitude > .0004f);
        previousPosition = transform.position;
    }
}
