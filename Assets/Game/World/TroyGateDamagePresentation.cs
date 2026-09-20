using UnityEngine;

public sealed class TroyGateDamagePresentation : MonoBehaviour
{
    int lastHealth = -1;
    int stage;
    float nextPulse;
    GameObject root;

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.MapNumber != 1) return;
        root = new GameObject("Chapter01_GateDamageVFX");
    }

    void Update()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null || gm.MapNumber != 1 || gm.MaxBaseHealth <= 0 || root == null) return;

        if (lastHealth < 0) lastHealth = gm.BaseHealth;
        if (gm.BaseHealth < lastHealth)
        {
            int lost = lastHealth - gm.BaseHealth;
            lastHealth = gm.BaseHealth;
            PlayHit(lost);
        }

        float ratio = gm.BaseHealth / (float)gm.MaxBaseHealth;
        int targetStage = ratio <= .25f ? 3 : ratio <= .50f ? 2 : ratio <= .75f ? 1 : 0;
        while (stage < targetStage)
        {
            stage++;
            AddPersistentDamage(stage);
        }

        if (stage >= 2 && Time.unscaledTime >= nextPulse)
        {
            nextPulse = Time.unscaledTime + (stage >= 3 ? 1.6f : 2.8f);
            float gateX = MapBuilder.CellToWorld(new Vector2Int(17,6)).x;
            CombatImpactPresentation.Pulse(
                new Vector3(gateX-.45f,.55f,Random.Range(-1.1f,1.1f)),
                new Color(1f,.26f,.04f),
                1.5f,
                .22f);
        }
    }

    void PlayHit(int damage)
    {
        float gateX = MapBuilder.CellToWorld(new Vector2Int(17,6)).x;
        Vector3 p = new Vector3(gateX-.45f,.75f,Random.Range(-1.2f,1.2f));
        bool heavy = damage >= 2;
        RuntimeEffects.Instance?.PlayHitSound(heavy);
        CombatImpactPresentation.GateHit(p, heavy);

        for(int i=0;i<4;i++)
        {
            GameObject chip = Primitive("Gate Impact Chip",PrimitiveType.Cube,p+new Vector3(Random.Range(-.18f,.18f),Random.Range(-.05f,.25f),Random.Range(-.18f,.18f)),new Vector3(.08f,.05f,.06f),new Color(.36f,.27f,.16f));
            chip.transform.rotation = Random.rotation;
            Destroy(chip,.8f);
        }
        RuntimeFileLogger.Event("PRESENTATION",$"Gate impact VFX damage={damage}");
    }

    void AddPersistentDamage(int level)
    {
        float gateX = MapBuilder.CellToWorld(new Vector2Int(17,6)).x;
        Color charColor = new Color(.16f,.10f,.06f);
        for(int i=0;i<level+1;i++)
        {
            float z = -1.05f + i*(2.1f/Mathf.Max(1,level));
            Primitive("Gate Scorch",PrimitiveType.Cube,new Vector3(gateX-.50f,.45f+level*.18f,z),new Vector3(.03f,.42f+.12f*level,.10f),charColor,Quaternion.Euler(0f,0f,18f*(i%2==0?1:-1)));
        }
        CreateSmoke(new Vector3(gateX-.42f,.95f+level*.18f,level%2==0?.65f:-.65f),level);
    }

    void CreateSmoke(Vector3 pos,int level)
    {
        for(int i=0;i<2+level;i++)
        {
            GameObject puff = Primitive("Gate Damage Smoke",PrimitiveType.Sphere,pos+new Vector3(i*.05f,i*.28f,(i%2==0?.05f:-.04f)),new Vector3(.20f+i*.05f,.16f+i*.04f,.20f+i*.05f),new Color(.17f,.15f,.13f));
            ChapterOneAmbientMotion motion = puff.AddComponent<ChapterOneAmbientMotion>();
            motion.kind = ChapterOneAmbientMotion.MotionKind.Smoke;
            motion.phase = level*.8f+i*.45f;
        }
    }

    GameObject Primitive(string name,PrimitiveType type,Vector3 pos,Vector3 scale,Color color,Quaternion? rot=null)
    {
        GameObject go=GameObject.CreatePrimitive(type);
        go.name=name;
        go.transform.SetParent(root.transform,false);
        go.transform.position=pos;
        go.transform.localScale=scale;
        if(rot.HasValue) go.transform.rotation=rot.Value;
        Collider c=go.GetComponent<Collider>(); if(c!=null) Destroy(c);
        TowerFactory.SetColor(go,color);
        return go;
    }
}
