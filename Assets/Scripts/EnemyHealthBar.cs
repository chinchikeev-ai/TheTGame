using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    Enemy enemy;
    Transform fill;
    Vector3 fillBaseScale;

    void Start()
    {
        enemy = GetComponent<Enemy>();

        GameObject back = GameObject.CreatePrimitive(PrimitiveType.Cube);
        back.name = "HP_Back";
        back.transform.SetParent(transform);
        back.transform.localPosition = new Vector3(0f, 1.65f, 0f);
        back.transform.localScale = new Vector3(1.15f, 0.10f, 0.08f);
        Destroy(back.GetComponent<Collider>());
        SetColor(back, new Color(0.12f, 0.12f, 0.12f));

        GameObject fillObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fillObj.name = "HP_Fill";
        fillObj.transform.SetParent(transform);
        fillObj.transform.localPosition = new Vector3(0f, 1.65f, -0.055f);
        fillObj.transform.localScale = new Vector3(1.05f, 0.065f, 0.06f);
        Destroy(fillObj.GetComponent<Collider>());
        SetColor(fillObj, new Color(0.20f, 0.90f, 0.25f));

        fill = fillObj.transform;
        fillBaseScale = fill.localScale;
        Refresh();
    }

    void LateUpdate()
    {
        if (Camera.main == null) return;
        Vector3 euler = Camera.main.transform.eulerAngles;
        transform.GetChild(transform.childCount - 2).rotation = Quaternion.Euler(euler.x, euler.y, 0f);
        transform.GetChild(transform.childCount - 1).rotation = Quaternion.Euler(euler.x, euler.y, 0f);
    }

    public void Refresh()
    {
        if (enemy == null) enemy = GetComponent<Enemy>();
        if (fill == null || enemy == null) return;

        float ratio = enemy.Health01;
        Vector3 scale = fillBaseScale;
        scale.x = fillBaseScale.x * ratio;
        fill.localScale = scale;
        fill.localPosition = new Vector3(-(fillBaseScale.x - scale.x) * 0.5f, 1.65f, -0.055f);
    }

    static void SetColor(GameObject obj, Color color)
    {
        Renderer r = obj.GetComponent<Renderer>();
        if (r == null) return;
        Material m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (m.shader == null) m = new Material(Shader.Find("Standard"));
        m.color = color;
        r.material = m;
    }
}
