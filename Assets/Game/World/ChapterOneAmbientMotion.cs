using UnityEngine;

public class ChapterOneAmbientMotion : MonoBehaviour
{
    public enum MotionKind { Sea, Flame, Banner, Smoke, Dust, Ember, HeatShimmer, Surf, Swell }
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
            case MotionKind.Dust:
                transform.localPosition = basePosition + new Vector3(Mathf.Sin(t * .42f) * .30f, Mathf.Sin(t * .72f) * .035f, Mathf.Cos(t * .37f) * .20f);
                transform.localRotation = baseRotation * Quaternion.Euler(0f, Mathf.Sin(t * .31f) * 8f, 0f);
                transform.localScale = new Vector3(baseScale.x * (1f + Mathf.Sin(t * .55f) * .08f), baseScale.y, baseScale.z * (1f + Mathf.Cos(t * .48f) * .10f));
                break;
            case MotionKind.Ember:
                float cycle = Mathf.Repeat(t * .42f, 1f);
                float flicker = 1f + Mathf.Sin(t * 10.4f) * .16f;
                transform.localPosition = basePosition + new Vector3(Mathf.Sin(t * 2.8f) * .13f, cycle * .92f, Mathf.Cos(t * 2.1f) * .09f);
                transform.localScale = baseScale * Mathf.Max(.38f, (1f - cycle * .48f) * flicker);
                break;
            case MotionKind.HeatShimmer:
                transform.localPosition = basePosition + new Vector3(Mathf.Sin(t * 4.1f) * .025f, Mathf.Sin(t * 3.4f) * .035f, 0f);
                transform.localScale = new Vector3(baseScale.x * (1f + Mathf.Sin(t * 3.7f) * .08f), baseScale.y * (1f + Mathf.Cos(t * 4.3f) * .11f), baseScale.z);
                break;
            case MotionKind.Surf:
                float surfPulse = (Mathf.Sin(t * 1.35f) + 1f) * .5f;
                transform.localPosition = basePosition + new Vector3(surfPulse * .12f, Mathf.Sin(t * 2.1f) * .006f, Mathf.Sin(t * .62f) * .025f);
                transform.localScale = new Vector3(baseScale.x * (.84f + surfPulse * .24f), baseScale.y, baseScale.z * (.94f + Mathf.Cos(t * .74f) * .05f));
                break;
            case MotionKind.Swell:
                float swellPulse = (Mathf.Sin(t * .78f) + 1f) * .5f;
                transform.localPosition = basePosition + new Vector3(Mathf.Sin(t * .72f) * .055f, Mathf.Sin(t * 1.1f) * .012f, Mathf.Cos(t * .51f) * .018f);
                transform.localScale = new Vector3(baseScale.x * (.88f + swellPulse * .18f), baseScale.y, baseScale.z * (1f + Mathf.Cos(t * .64f) * .035f));
                break;
        }
    }
}
