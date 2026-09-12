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
        if (cam != null)
        {
            gameplayPosition = cam.transform.position;
            gameplayRotation = cam.transform.rotation;
            gameplaySize = cam.orthographicSize;
        }
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

        Vector3 sea = new Vector3(-8.5f, 24.5f, -8.0f);
        Quaternion seaRot = Quaternion.Euler(70f, 18f, 0f);
        Vector3 gate = new Vector3(5.8f, 23.0f, -5.2f);
        Quaternion gateRot = Quaternion.Euler(72f, -11f, 0f);

        cam.transform.position = sea;
        cam.transform.rotation = seaRot;
        cam.orthographicSize = 8.9f;

        yield return MoveShot(sea, seaRot, 8.9f, gate, gateRot, 8.2f, 2.25f);
        yield return new WaitForSeconds(.35f);
        yield return MoveShot(gate, gateRot, 8.2f, gameplayPosition, gameplayRotation, gameplaySize, 1.65f);

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
