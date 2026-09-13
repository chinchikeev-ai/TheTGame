using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    Enemy enemy;
    Camera targetCamera;
    Transform back;
    Transform fill;
    Vector3 fillBaseScale;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        targetCamera = Camera.main;

        GameObject backObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        backObj.name = "HP_Back";
        backObj.transform.SetParent(transform);
        backObj.transform.localPosition = new Vector3(0f, 1.65f, 0f);
        backObj.transform.localScale = new Vector3(1.15f, 0.10f, 0.08f);
        Destroy(backObj.GetComponent<Collider>());
        TowerFactory.SetColor(backObj, new Color(0.12f, 0.12f, 0.12f));
        back = backObj.transform;

        GameObject fillObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fillObj.name = "HP_Fill";
        fillObj.transform.SetParent(transform);
        fillObj.transform.localPosition = new Vector3(0f, 1.65f, -0.055f);
        fillObj.transform.localScale = new Vector3(1.05f, 0.065f, 0.06f);
        Destroy(fillObj.GetComponent<Collider>());
        TowerFactory.SetColor(fillObj, new Color(0.20f, 0.90f, 0.25f));

        fill = fillObj.transform;
        fillBaseScale = fill.localScale;
        Refresh();
    }

    void LateUpdate()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        if (targetCamera == null) return;

        Vector3 euler = targetCamera.transform.eulerAngles;
        Quaternion rotation = Quaternion.Euler(euler.x, euler.y, 0f);
        if (back != null) back.rotation = rotation;
        if (fill != null) fill.rotation = rotation;
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
}
