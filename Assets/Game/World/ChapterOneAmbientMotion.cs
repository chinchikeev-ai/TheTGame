using UnityEngine;

public class ChapterOneAmbientMotion : MonoBehaviour
{
    public enum MotionKind { Sea, Flame, Banner, Smoke }
    public MotionKind kind;
    public float phase;
    Vector3 basePosition;
    Vector3 baseScale;
    Quaternion baseRotation;

    void Start()
    {
        basePosition = transform.localPosition;
        baseScale = transform.localScale;
        baseRotation = transform.localRotation;
        phase = phase == 0f ? Random.Range(0f, 6.28f) : phase;
    }

    void Update()
    {
        float t = Time.time + phase;
        switch (kind)
        {
            case MotionKind.Sea:
                transform.localPosition = basePosition + new Vector3(0f, Mathf.Sin(t * 1.2f) * .025f, Mathf.Sin(t * .55f) * .08f);
                break;
            case MotionKind.Flame:
                transform.localScale = new Vector3(baseScale.x * (1f + Mathf.Sin(t * 7.2f) * .12f), baseScale.y * (1f + Mathf.Sin(t * 9.1f) * .16f), baseScale.z * (1f + Mathf.Cos(t * 6.3f) * .10f));
                transform.localPosition = basePosition + new Vector3(Mathf.Sin(t * 5.7f) * .025f, Mathf.Sin(t * 8.4f) * .035f, 0f);
                break;
            case MotionKind.Banner:
                transform.localRotation = baseRotation * Quaternion.Euler(0f, Mathf.Sin(t * 2.1f) * 5f, Mathf.Sin(t * 2.8f) * 4f);
                break;
            case MotionKind.Smoke:
                transform.localPosition = basePosition + new Vector3(Mathf.Sin(t * .8f) * .06f, Mathf.Sin(t * .5f) * .08f, Mathf.Cos(t * .7f) * .04f);
                transform.localScale = baseScale * (1f + Mathf.Sin(t * .9f) * .06f);
                break;
        }
    }
}
