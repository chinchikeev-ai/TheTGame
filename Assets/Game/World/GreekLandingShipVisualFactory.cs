using UnityEngine;

public static class GreekLandingShipVisualFactory
{
    public static GameObject Create(Transform parent, Vector3 position, string name = "Greek Landing Ship", float yaw = 0f)
    {
        GameObject root = new GameObject(name);
        root.transform.SetParent(parent, false);
        root.transform.position = position;
        root.transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        Color darkWood = new Color(.25f,.13f,.065f);
        Color warmWood = new Color(.42f,.20f,.08f);
        Color bronze = new Color(.56f,.31f,.10f);
        Color sailColor = new Color(.72f,.64f,.50f);

        Part(root.transform,"Lower Hull",PrimitiveType.Cube,Vector3.zero,new Vector3(2.5f,.28f,.78f),darkWood);
        Part(root.transform,"Deck",PrimitiveType.Cube,new Vector3(-.05f,.28f,0f),new Vector3(2.15f,.10f,.66f),warmWood);
        Part(root.transform,"Port Gunwale",PrimitiveType.Cube,new Vector3(-.05f,.48f,-.43f),new Vector3(2.18f,.12f,.08f),warmWood);
        Part(root.transform,"Starboard Gunwale",PrimitiveType.Cube,new Vector3(-.05f,.48f,.43f),new Vector3(2.18f,.12f,.08f),warmWood);
        Part(root.transform,"Keel",PrimitiveType.Cube,new Vector3(-.10f,-.22f,0f),new Vector3(2.18f,.08f,.12f),bronze);
        Part(root.transform,"Prow",PrimitiveType.Cube,new Vector3(1.45f,.22f,0f),new Vector3(.68f,.42f,.58f),warmWood,Quaternion.Euler(0f,0f,-23f));
        Part(root.transform,"Prow Horn",PrimitiveType.Cube,new Vector3(1.88f,.35f,0f),new Vector3(.44f,.09f,.09f),bronze,Quaternion.Euler(0f,0f,-17f));
        Part(root.transform,"Stern",PrimitiveType.Cube,new Vector3(-1.35f,.24f,0f),new Vector3(.45f,.50f,.62f),new Color(.36f,.17f,.07f),Quaternion.Euler(0f,0f,18f));
        Part(root.transform,"Mast",PrimitiveType.Cylinder,new Vector3(-.05f,1.02f,0f),new Vector3(.045f,.95f,.045f),new Color(.32f,.18f,.08f));
        Part(root.transform,"Yard",PrimitiveType.Cylinder,new Vector3(-.05f,1.55f,0f),new Vector3(.035f,.62f,.035f),warmWood,Quaternion.Euler(90f,0f,0f));
        Part(root.transform,"Square Sail",PrimitiveType.Cube,new Vector3(-.05f,1.12f,0f),new Vector3(.05f,.78f,.92f),sailColor);
        Part(root.transform,"Sail Stripe",PrimitiveType.Cube,new Vector3(-.085f,1.12f,0f),new Vector3(.012f,.13f,.94f),new Color(.34f,.40f,.50f));

        for (int i = -3; i <= 3; i++)
        {
            float x = i * .34f;
            Part(root.transform,"Bench",PrimitiveType.Cube,new Vector3(x,.39f,0f),new Vector3(.11f,.045f,.62f),warmWood);
            GameObject oar = Part(root.transform,"Oar",PrimitiveType.Cylinder,new Vector3(x,.31f,0f),new Vector3(.022f,.88f,.022f),warmWood,Quaternion.Euler(90f,0f,0f));
            oar.transform.localRotation = Quaternion.Euler(90f,0f,i % 2 == 0 ? 7f : -7f);
            Part(root.transform,"Hull Shield",PrimitiveType.Cylinder,new Vector3(x,.48f,-.48f),new Vector3(.17f,.04f,.17f),i % 2 == 0 ? bronze : new Color(.52f,.43f,.25f),Quaternion.Euler(90f,0f,0f));
        }

        return root;
    }

    static GameObject Part(Transform parent, string name, PrimitiveType type, Vector3 localPosition, Vector3 localScale, Color color, Quaternion? localRotation = null)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPosition;
        go.transform.localScale = localScale;
        if (localRotation.HasValue) go.transform.localRotation = localRotation.Value;
        Collider collider = go.GetComponent<Collider>();
        if (collider != null) Object.Destroy(collider);
        TowerFactory.SetColor(go, color);
        return go;
    }
}
