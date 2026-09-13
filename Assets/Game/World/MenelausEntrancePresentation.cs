using System.Collections;
using UnityEngine;

public sealed class MenelausEntrancePresentation : MonoBehaviour
{
    bool played;
    Camera cam;
    CameraController cameraController;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<MenelausEntrancePresentation>() == null)
            new GameObject("MenelausEntrancePresentation").AddComponent<MenelausEntrancePresentation>();
    }

    void Start()
    {
        cam = Camera.main;
        cameraController = cam != null ? cam.GetComponent<CameraController>() : null;
    }

    void Update()
    {
        if (played || GameManager.Instance == null || GameManager.Instance.MapNumber != 1 || GameManager.Instance.GameEnded) return;
        Enemy boss = FindBoss();
        if (boss == null) return;
        played = true;
        StartCoroutine(PlayEntrance(boss));
    }

    IEnumerator PlayEntrance(Enemy boss)
    {
        if (boss == null) yield break;
        if (cam == null) cam = Camera.main;

        GameObject staging = BuildStaging(boss.transform.position);
        RuntimeFileLogger.Event("PRESENTATION", "Menelaus battlefield entrance started");
        RuntimeEffects.Instance?.PlayHeroPulse(boss.transform.position, new Color(.82f,.11f,.035f), 7.5f, .75f);

        if (cam == null)
        {
            yield return new WaitForSecondsRealtime(2.4f);
            Destroy(staging, 3f);
            yield break;
        }

        Vector3 originalPosition = cam.transform.position;
        Quaternion originalRotation = cam.transform.rotation;
        float originalSize = cam.orthographicSize;
        bool controllerWasEnabled = cameraController != null && cameraController.enabled;
        if (cameraController != null) cameraController.enabled = false;

        Vector3 focusPosition = new Vector3(boss.transform.position.x + 1.2f, 18.0f, boss.transform.position.z - 4.0f);
        Quaternion focusRotation = Quaternion.Euler(68f, -8f, 0f);
        float focusSize = 5.8f;

        yield return BlendCamera(originalPosition, originalRotation, originalSize, focusPosition, focusRotation, focusSize, .55f);
        yield return new WaitForSecondsRealtime(1.35f);
        RuntimeEffects.Instance?.PlayHeroPulse(boss.transform.position, new Color(1f,.35f,.06f), 4.2f, .38f);
        yield return BlendCamera(focusPosition, focusRotation, focusSize, originalPosition, originalRotation, originalSize, .75f);

        if (cameraController != null) cameraController.enabled = controllerWasEnabled;
        Destroy(staging, 3.5f);
        RuntimeFileLogger.Event("PRESENTATION", "Menelaus battlefield entrance completed");
    }

    IEnumerator BlendCamera(Vector3 fromPos, Quaternion fromRot, float fromSize, Vector3 toPos, Quaternion toRot, float toSize, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration && cam != null)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f,1f,Mathf.Clamp01(elapsed / duration));
            cam.transform.position = Vector3.Lerp(fromPos,toPos,t);
            cam.transform.rotation = Quaternion.Slerp(fromRot,toRot,t);
            cam.orthographicSize = Mathf.Lerp(fromSize,toSize,t);
            yield return null;
        }
    }

    GameObject BuildStaging(Vector3 bossPosition)
    {
        GameObject root = new GameObject("MenelausEntrance_Staging");
        root.transform.position = bossPosition;

        CreateStandard(root.transform, new Vector3(-1.45f,0f,-1.15f), -8f);
        CreateStandard(root.transform, new Vector3(-1.45f,0f,1.15f), 8f);
        CreateStandard(root.transform, new Vector3(.95f,0f,-1.55f), -14f);
        CreateStandard(root.transform, new Vector3(.95f,0f,1.55f), 14f);
        CreateCommandBoat(root.transform, new Vector3(-3.6f,.08f,0f));
        return root;
    }

    void CreateStandard(Transform parent, Vector3 localPosition, float yaw)
    {
        GameObject root = new GameObject("Spartan Standard");
        root.transform.SetParent(parent,false);
        root.transform.localPosition = localPosition;
        root.transform.localRotation = Quaternion.Euler(0f,yaw,0f);

        GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pole.transform.SetParent(root.transform,false);
        pole.transform.localPosition = new Vector3(0f,1.15f,0f);
        pole.transform.localScale = new Vector3(.045f,1.15f,.045f);
        Destroy(pole.GetComponent<Collider>());
        TowerFactory.SetColor(pole,new Color(.32f,.19f,.09f));

        GameObject banner = GameObject.CreatePrimitive(PrimitiveType.Cube);
        banner.transform.SetParent(root.transform,false);
        banner.transform.localPosition = new Vector3(.34f,1.62f,0f);
        banner.transform.localScale = new Vector3(.68f,.52f,.035f);
        Destroy(banner.GetComponent<Collider>());
        TowerFactory.SetColor(banner,new Color(.55f,.045f,.025f));

        GameObject bronze = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bronze.transform.SetParent(root.transform,false);
        bronze.transform.localPosition = new Vector3(.34f,1.63f,-.04f);
        bronze.transform.localScale = new Vector3(.42f,.07f,.02f);
        Destroy(bronze.GetComponent<Collider>());
        TowerFactory.SetColor(bronze,new Color(.84f,.54f,.15f));
    }

    void CreateCommandBoat(Transform parent, Vector3 localPosition)
    {
        GameObject root = new GameObject("Menelaus Command Boat");
        root.transform.SetParent(parent,false);
        root.transform.localPosition = localPosition;
        root.transform.localRotation = Quaternion.Euler(0f,88f,0f);

        GameObject hull = GameObject.CreatePrimitive(PrimitiveType.Cube);
        hull.transform.SetParent(root.transform,false);
        hull.transform.localScale = new Vector3(2.8f,.32f,.78f);
        Destroy(hull.GetComponent<Collider>());
        TowerFactory.SetColor(hull,new Color(.24f,.12f,.055f));

        GameObject rail = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rail.transform.SetParent(root.transform,false);
        rail.transform.localPosition = new Vector3(0f,.35f,0f);
        rail.transform.localScale = new Vector3(2.25f,.12f,.64f);
        Destroy(rail.GetComponent<Collider>());
        TowerFactory.SetColor(rail,new Color(.48f,.26f,.08f));

        GameObject mast = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        mast.transform.SetParent(root.transform,false);
        mast.transform.localPosition = new Vector3(-.15f,1.05f,0f);
        mast.transform.localScale = new Vector3(.05f,.95f,.05f);
        Destroy(mast.GetComponent<Collider>());
        TowerFactory.SetColor(mast,new Color(.30f,.16f,.07f));

        GameObject sail = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sail.transform.SetParent(root.transform,false);
        sail.transform.localPosition = new Vector3(-.15f,1.25f,0f);
        sail.transform.localScale = new Vector3(.035f,.72f,.92f);
        Destroy(sail.GetComponent<Collider>());
        TowerFactory.SetColor(sail,new Color(.63f,.10f,.055f));

        for (int i=0;i<5;i++)
        {
            GameObject shield = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            shield.transform.SetParent(root.transform,false);
            shield.transform.localPosition = new Vector3(-1.4f + i*.7f,.40f,.50f);
            shield.transform.localRotation = Quaternion.Euler(90f,0f,0f);
            shield.transform.localScale = new Vector3(.20f,.025f,.20f);
            Destroy(shield.GetComponent<Collider>());
            TowerFactory.SetColor(shield,new Color(.65f,.16f,.055f));
        }
    }

    Enemy FindBoss()
    {
        foreach (Enemy enemy in EnemyRegistry.All)
            if (enemy != null && enemy.Archetype == EnemyArchetype.Boss && enemy.Health > 0f) return enemy;
        return null;
    }
}
