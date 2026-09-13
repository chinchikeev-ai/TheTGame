using UnityEngine;

public static class GreekLandingShipVisualFactory
{
    public static GameObject Create(Transform parent, Vector3 position, string name = "Greek Landing Ship", float yaw = 0f)
    {
        GameObject root = new GameObject(name);
        root.transform.SetParent(parent, false);
        root.transform.position = position;
        root.transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        Color darkWood = new Color(.22f,.115f,.055f);
        Color warmWood = new Color(.39f,.19f,.075f);
        Color lightWood = new Color(.50f,.29f,.13f);
        Color bronze = new Color(.56f,.34f,.13f);
        Color agedBronze = new Color(.45f,.34f,.20f);
        Color sailColor = new Color(.70f,.65f,.54f);
        Color greekBlue = new Color(.32f,.40f,.50f);
        Color rope = new Color(.32f,.26f,.18f);

        Part(root.transform,"Lower Hull",PrimitiveType.Cube,new Vector3(-.05f,-.02f,0f),new Vector3(2.55f,.25f,.68f),darkWood);
        Part(root.transform,"Hull Shoulder",PrimitiveType.Cube,new Vector3(-.08f,.20f,0f),new Vector3(2.38f,.19f,.78f),warmWood);
        Part(root.transform,"Deck",PrimitiveType.Cube,new Vector3(-.08f,.37f,0f),new Vector3(2.10f,.07f,.66f),lightWood);
        Part(root.transform,"Port Gunwale",PrimitiveType.Cube,new Vector3(-.08f,.50f,-.43f),new Vector3(2.18f,.10f,.065f),warmWood);
        Part(root.transform,"Starboard Gunwale",PrimitiveType.Cube,new Vector3(-.08f,.50f,.43f),new Vector3(2.18f,.10f,.065f),warmWood);
        Part(root.transform,"Port Painted Stripe",PrimitiveType.Cube,new Vector3(-.02f,.24f,-.405f),new Vector3(1.95f,.055f,.028f),greekBlue);
        Part(root.transform,"Starboard Painted Stripe",PrimitiveType.Cube,new Vector3(-.02f,.24f,.405f),new Vector3(1.95f,.055f,.028f),greekBlue);
        Part(root.transform,"Keel",PrimitiveType.Cube,new Vector3(-.10f,-.22f,0f),new Vector3(2.15f,.07f,.10f),agedBronze);

        Part(root.transform,"Prow Lower",PrimitiveType.Cube,new Vector3(1.38f,.06f,0f),new Vector3(.62f,.30f,.56f),warmWood,Quaternion.Euler(0f,0f,-18f));
        Part(root.transform,"Raised Prow",PrimitiveType.Cube,new Vector3(1.56f,.42f,0f),new Vector3(.38f,.43f,.48f),lightWood,Quaternion.Euler(0f,0f,-27f));
        Part(root.transform,"Bronze Ram",PrimitiveType.Cube,new Vector3(1.90f,-.08f,0f),new Vector3(.60f,.07f,.10f),bronze,Quaternion.Euler(0f,0f,-4f));
        Part(root.transform,"Prow Crest",PrimitiveType.Cube,new Vector3(1.78f,.68f,0f),new Vector3(.10f,.34f,.11f),bronze,Quaternion.Euler(0f,0f,-24f));

        Part(root.transform,"Stern",PrimitiveType.Cube,new Vector3(-1.38f,.22f,0f),new Vector3(.42f,.48f,.58f),new Color(.34f,.16f,.065f),Quaternion.Euler(0f,0f,18f));
        Part(root.transform,"Stern Post",PrimitiveType.Cube,new Vector3(-1.56f,.66f,0f),new Vector3(.09f,.50f,.10f),lightWood,Quaternion.Euler(0f,0f,16f));
        Part(root.transform,"Stern Fin",PrimitiveType.Cube,new Vector3(-1.60f,.94f,0f),new Vector3(.08f,.26f,.32f),greekBlue,Quaternion.Euler(0f,0f,14f));

        Part(root.transform,"Mast",PrimitiveType.Cylinder,new Vector3(-.08f,1.11f,0f),new Vector3(.038f,.91f,.038f),lightWood);
        Part(root.transform,"Yard",PrimitiveType.Cylinder,new Vector3(-.08f,1.62f,0f),new Vector3(.028f,.66f,.028f),warmWood,Quaternion.Euler(90f,0f,0f));

        Part(root.transform,"Sail Left",PrimitiveType.Cube,new Vector3(-.105f,1.19f,-.47f),new Vector3(.035f,.72f,.46f),sailColor,Quaternion.Euler(0f,3f,-2f));
        Part(root.transform,"Sail Right",PrimitiveType.Cube,new Vector3(-.105f,1.19f,.47f),new Vector3(.035f,.72f,.46f),sailColor*.97f,Quaternion.Euler(0f,-3f,2f));
        Part(root.transform,"Sail Center Stripe",PrimitiveType.Cube,new Vector3(-.145f,1.19f,0f),new Vector3(.018f,.72f,.10f),greekBlue);
        Part(root.transform,"Sail Top Border",PrimitiveType.Cube,new Vector3(-.14f,1.88f,0f),new Vector3(.018f,.055f,.94f),greekBlue*.88f);
        Part(root.transform,"Sail Bottom Border",PrimitiveType.Cube,new Vector3(-.14f,.51f,0f),new Vector3(.018f,.045f,.86f),greekBlue*.78f);

        RodBetween(root.transform,"Fore Rigging",new Vector3(-.08f,1.95f,0f),new Vector3(1.42f,.48f,0f),.012f,rope);
        RodBetween(root.transform,"Aft Rigging",new Vector3(-.08f,1.95f,0f),new Vector3(-1.42f,.58f,0f),.012f,rope);
        RodBetween(root.transform,"Port Stay",new Vector3(-.08f,1.72f,0f),new Vector3(-.20f,.48f,-.42f),.010f,rope);
        RodBetween(root.transform,"Starboard Stay",new Vector3(-.08f,1.72f,0f),new Vector3(-.20f,.48f,.42f),.010f,rope);

        for (int i = -3; i <= 3; i++)
        {
            float x = i * .33f;
            Part(root.transform,"Bench",PrimitiveType.Cube,new Vector3(x,.42f,0f),new Vector3(.10f,.035f,.62f),warmWood);

            GameObject oar = Part(root.transform,"Oar",PrimitiveType.Cylinder,new Vector3(x,.31f,0f),new Vector3(.018f,.90f,.018f),lightWood);
            oar.transform.localRotation = Quaternion.Euler(90f,0f,i%2==0?7f:-7f);
            Part(root.transform,"Port Oar Blade",PrimitiveType.Cube,new Vector3(x,.30f,-.94f),new Vector3(.09f,.035f,.22f),lightWood,Quaternion.Euler(0f,0f,i%2==0?7f:-7f));
            Part(root.transform,"Starboard Oar Blade",PrimitiveType.Cube,new Vector3(x,.30f,.94f),new Vector3(.09f,.035f,.22f),lightWood,Quaternion.Euler(0f,0f,i%2==0?-7f:7f));

            Color shieldColor = i%2==0 ? bronze : agedBronze;
            Part(root.transform,"Port Hull Shield",PrimitiveType.Cylinder,new Vector3(x,.51f,-.49f),new Vector3(.15f,.025f,.15f),shieldColor,Quaternion.Euler(90f,0f,0f));
            Part(root.transform,"Starboard Hull Shield",PrimitiveType.Cylinder,new Vector3(x,.51f,.49f),new Vector3(.15f,.025f,.15f),shieldColor,Quaternion.Euler(90f,0f,0f));
        }

        Part(root.transform,"Port Bow Eye",PrimitiveType.Sphere,new Vector3(1.50f,.42f,-.31f),new Vector3(.055f,.055f,.025f),new Color(.85f,.80f,.64f));
        Part(root.transform,"Starboard Bow Eye",PrimitiveType.Sphere,new Vector3(1.50f,.42f,.31f),new Vector3(.055f,.055f,.025f),new Color(.85f,.80f,.64f));

        return root;
    }

    static GameObject RodBetween(Transform parent,string name,Vector3 a,Vector3 b,float radius,Color color)
    {
        Vector3 delta = b-a;
        GameObject rod = Part(parent,name,PrimitiveType.Cylinder,(a+b)*.5f,new Vector3(radius,delta.magnitude*.5f,radius),color);
        if(delta.sqrMagnitude > .0001f)
            rod.transform.localRotation = Quaternion.FromToRotation(Vector3.up,delta.normalized);
        return rod;
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
