using System.Collections;
using UnityEngine;

public class ChapterOneCinematicCamera : MonoBehaviour
{
    Camera cam;
    CameraController controller;
    Vector3 gameplayPosition;
    Quaternion gameplayRotation;
    float gameplaySize;
    bool played;

    public void Initialize(Camera target)
    {
        cam = target;
        controller = cam != null ? cam.GetComponent<CameraController>() : null;
        if (cam == null) return;
        gameplayPosition = cam.transform.position;
        gameplayRotation = cam.transform.rotation;
        gameplaySize = cam.orthographicSize;
    }

    void Update()
    {
        if (played || cam == null || GameManager.Instance == null) return;
        if (GameManager.Instance.RunTime <= .01f) return;
        played = true;
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        if (controller != null) controller.enabled = false;

        Vector3 sea = new Vector3(-8.8f, 24.8f, -7.2f);
        Quaternion seaRot = Quaternion.Euler(70f, 16f, 0f);
        Vector3 battlefield = new Vector3(.25f, 23.9f, -5.7f);
        Quaternion battlefieldRot = Quaternion.Euler(72f, 3f, 0f);
        Vector3 gate = new Vector3(6.8f, 22.7f, -5.0f);
        Quaternion gateRot = Quaternion.Euler(72f, -10f, 0f);

        cam.transform.position = sea;
        cam.transform.rotation = seaRot;
        cam.orthographicSize = 8.8f;

        yield return MoveShot(sea, seaRot, 8.8f, battlefield, battlefieldRot, 8.45f, 1.45f);
        yield return MoveShot(battlefield, battlefieldRot, 8.45f, gate, gateRot, 7.85f, 1.20f);
        yield return new WaitForSecondsRealtime(.28f);
        yield return MoveShot(gate, gateRot, 7.85f, gameplayPosition, gameplayRotation, gameplaySize, 1.40f);

        cam.transform.position = gameplayPosition;
        cam.transform.rotation = gameplayRotation;
        cam.orthographicSize = gameplaySize;
        if (controller != null) controller.enabled = true;
    }

    IEnumerator MoveShot(Vector3 fromPos, Quaternion fromRot, float fromSize, Vector3 toPos, Quaternion toRot, float toSize, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
            cam.transform.position = Vector3.Lerp(fromPos, toPos, t);
            cam.transform.rotation = Quaternion.Slerp(fromRot, toRot, t);
            cam.orthographicSize = Mathf.Lerp(fromSize, toSize, t);
            yield return null;
        }
    }
}
