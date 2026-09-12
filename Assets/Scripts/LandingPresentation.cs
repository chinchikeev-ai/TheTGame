using System.Collections;
using UnityEngine;

public class LandingPresentation : MonoBehaviour
{
    bool played;

    void Update()
    {
        if (played || GameManager.Instance == null || GameManager.Instance.GameEnded) return;
        if (GameManager.Instance.RunTime <= .01f) return;
        played = true;
        StartCoroutine(PlayLanding());
    }

    IEnumerator PlayLanding()
    {
        RuntimeFileLogger.Event("CHAPTER", "Chapter I landing presentation started");
        GameObject root = new GameObject("LandingPresentation_Runtime");

        GameObject[] boats = new GameObject[3];
        float[] z = { 6.0f, 0f, -6.0f };
        for (int i = 0; i < boats.Length; i++)
        {
            boats[i] = CreateBoat(root.transform, new Vector3(-18.5f - i * .8f, .12f, z[i]));
        }

        float duration = 5.5f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            for (int i = 0; i < boats.Length; i++)
            {
                if (boats[i] == null) continue;
                Vector3 start = new Vector3(-18.5f - i * .8f, .12f, z[i]);
                Vector3 end = new Vector3(-14.7f, .10f, z[i] * .88f);
                boats[i].transform.position = Vector3.Lerp(start, end, t);
            }
            yield return null;
        }

        for (int i = 0; i < 10; i++)
        {
            GameObject soldier = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            soldier.name = "Landing Greek Silhouette";
            soldier.transform.SetParent(root.transform);
            soldier.transform.position = new Vector3(-13.5f + (i % 3) * .35f, .42f, -5.5f + i * 1.2f);
            soldier.transform.localScale = new Vector3(.30f,.42f,.30f);
            Object.Destroy(soldier.GetComponent<Collider>());
            TowerFactory.SetColor(soldier, new Color(.34f,.22f,.14f));
        }

        RuntimeFileLogger.Event("CHAPTER", "Chapter I landing presentation completed");
        Destroy(root, 8f);
    }

    GameObject CreateBoat(Transform parent, Vector3 position)
    {
        GameObject root = new GameObject("Incoming Landing Boat");
        root.transform.SetParent(parent);
        root.transform.position = position;

        GameObject hull = GameObject.CreatePrimitive(PrimitiveType.Cube);
        hull.transform.SetParent(root.transform, false);
        hull.transform.localScale = new Vector3(2.1f,.30f,.72f);
        Object.Destroy(hull.GetComponent<Collider>());
        TowerFactory.SetColor(hull, new Color(.28f,.17f,.09f));

        GameObject shieldLine = GameObject.CreatePrimitive(PrimitiveType.Cube);
        shieldLine.transform.SetParent(root.transform, false);
        shieldLine.transform.localPosition = new Vector3(.1f,.38f,0f);
        shieldLine.transform.localScale = new Vector3(1.5f,.32f,.15f);
        Object.Destroy(shieldLine.GetComponent<Collider>());
        TowerFactory.SetColor(shieldLine, new Color(.55f,.38f,.18f));
        return root;
    }
}
