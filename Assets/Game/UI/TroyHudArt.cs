using System.Collections.Generic;
using UnityEngine;

public static class TroyHudArt
{
    static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();

    static readonly Color Bronze = new Color(.58f,.30f,.085f,1f);
    static readonly Color DarkBronze = new Color(.22f,.095f,.035f,1f);
    static readonly Color Gold = new Color(.96f,.63f,.16f,1f);
    static readonly Color PaleGold = new Color(1f,.88f,.48f,1f);
    static readonly Color TrojanRed = new Color(.63f,.075f,.035f,1f);
    static readonly Color Skin = new Color(.73f,.43f,.25f,1f);
    static readonly Color GreekBlue = new Color(.20f,.33f,.47f,1f);

    public static Sprite Panel(bool boss = false)
    {
        string key = boss ? "panel_boss" : "panel";
        if (Cache.TryGetValue(key, out Sprite s)) return s;
        Texture2D t = NewTexture(64, 64);
        Color baseC = boss ? new Color(.12f,.025f,.018f,1f) : new Color(.075f,.045f,.025f,1f);
        Color bronze = boss ? new Color(.72f,.16f,.045f,1f) : new Color(.55f,.30f,.10f,1f);
        for (int y=0;y<64;y++) for (int x=0;x<64;x++)
        {
            int e = Mathf.Min(Mathf.Min(x,63-x), Mathf.Min(y,63-y));
            float grain = (((x*17 + y*31) % 13) - 6) * .004f;
            Color c = e < 3 ? bronze : e < 6 ? Gold*.72f : e < 9 ? bronze*.72f : baseC*(.92f + .12f*(x+y)/126f + grain);
            t.SetPixel(x,y,c);
        }
        CornerStud(t,8,8,Gold); CornerStud(t,55,8,Gold); CornerStud(t,8,55,Gold); CornerStud(t,55,55,Gold);
        t.Apply();
        s = Sprite.Create(t,new Rect(0,0,64,64),new Vector2(.5f,.5f),100f,0,SpriteMeshType.FullRect,new Vector4(10,10,10,10));
        Cache[key]=s; return s;
    }

    public static Sprite Portrait(string key)
    {
        string cacheKey = "portrait_" + key;
        if (Cache.TryGetValue(cacheKey, out Sprite s)) return s;
        Texture2D t = NewTexture(128,128);
        RadialBackground(t, new Color(.035f,.018f,.012f,1f), key=="menelaus" ? new Color(.32f,.045f,.025f,1f) : new Color(.28f,.12f,.025f,1f));
        Ring(t,64,64,61,56, key=="menelaus" ? new Color(.80f,.18f,.05f,1f) : Bronze);
        Ring(t,64,64,55,52,Gold);

        if (key=="menelaus") DrawMenelaus(t); else DrawHector(t);

        t.Apply();
        s = Sprite.Create(t,new Rect(0,0,128,128),new Vector2(.5f,.5f));
        Cache[cacheKey]=s; return s;
    }

    public static Sprite Ability(string key)
    {
        string cacheKey = "ability_" + key;
        if (Cache.TryGetValue(cacheKey, out Sprite s)) return s;
        Texture2D t = NewTexture(96,96);
        RadialBackground(t,new Color(.025f,.015f,.01f,1f),new Color(.18f,.08f,.02f,1f));
        Ring(t,48,48,45,40,Bronze); Ring(t,48,48,39,37,Gold*.72f);
        if (key=="warcry")
        {
            FillTriangle(t,new Vector2(18,52),new Vector2(48,72),new Vector2(48,32),TrojanRed);
            FillTriangle(t,new Vector2(78,52),new Vector2(48,72),new Vector2(48,32),TrojanRed);
            Circle(t,48,50,15,Gold); Circle(t,48,50,9,new Color(.10f,.03f,.015f,1f));
            Line(t,48,65,48,82,PaleGold,4);
        }
        else if (key=="shieldwall")
        {
            ShieldShape(t,48,50,28,32,Bronze); ShieldShape(t,48,50,22,26,new Color(.38f,.14f,.035f,1f));
            Line(t,48,25,48,73,Gold,5); Line(t,29,49,67,49,Gold,4);
        }
        else if (key=="spear")
        {
            Line(t,26,72,69,26,PaleGold,6); FillTriangle(t,new Vector2(68,20),new Vector2(82,18),new Vector2(78,33),Gold);
            Line(t,23,75,30,82,TrojanRed,6);
        }
        else if (key=="ultimate")
        {
            DrawFlame(t,48,50,31,new Color(1f,.20f,.025f,1f),Gold,PaleGold);
            Line(t,48,17,48,32,PaleGold,4); Line(t,20,30,31,40,Gold,4); Line(t,76,30,65,40,Gold,4);
        }
        else if (key=="magic")
        {
            Line(t,48,20,48,76,PaleGold,5); Line(t,24,48,72,48,Gold,5); Circle(t,48,48,12,new Color(.30f,.65f,1f,1f));
        }
        else if (key=="gift")
        {
            Rect(t,23,38,50,34,Bronze); Rect(t,20,31,56,12,Gold); Rect(t,44,29,8,47,PaleGold); Ring(t,39,31,10,6,TrojanRed); Ring(t,57,31,10,6,TrojanRed);
        }
        else DrawFlame(t,48,48,24,TrojanRed,Gold,PaleGold);
        t.Apply(); s=Sprite.Create(t,new Rect(0,0,96,96),new Vector2(.5f,.5f)); Cache[cacheKey]=s; return s;
    }

    public static Sprite Icon(string key)
    {
        if (Cache.TryGetValue(key, out Sprite s)) return s;
        Texture2D t = NewTexture(64,64);
        Circle(t,32,32,29,new Color(.045f,.028f,.018f,.98f));
        Ring(t,32,32,28,24,Bronze); Ring(t,32,32,23,21,new Color(.16f,.075f,.025f,1f));
        if (key=="gold") { Circle(t,32,32,17,Gold); Ring(t,32,32,17,13,PaleGold); Rect(t,29,18,6,28,Bronze); Rect(t,23,29,18,5,Bronze); }
        else if (key=="gate") { Rect(t,18,20,28,28,Bronze); Rect(t,22,16,6,32,Gold); Rect(t,36,16,6,32,Gold); Rect(t,16,44,32,6,Gold); }
        else if (key=="enemy") { GreekHelmet(t,32,33,20); }
        else if (key=="hector") return Portrait("hector");
        else if (key=="boss") return Portrait("menelaus");
        else if (key=="archer") DrawArcher(t);
        else if (key=="spear") DrawSpear(t);
        else if (key=="ballista") DrawBallista(t);
        else if (key=="slow") DrawApollo(t);
        else if (key=="fire") DrawFlame(t,32,32,19,new Color(1f,.22f,.03f,1f),Gold,PaleGold);
        else if (key=="shield") ShieldShape(t,32,33,18,21,Bronze);
        else if (key=="magic") { Line(t,32,13,32,50,Gold,5); Line(t,20,31,44,31,PaleGold,4); Circle(t,32,31,8,new Color(.25f,.63f,1f,1f)); }
        else if (key=="gift") { Rect(t,16,23,32,25,Bronze); Rect(t,14,19,36,8,Gold); Rect(t,29,18,6,31,PaleGold); }
        else if (key=="sword") { Line(t,20,45,44,18,PaleGold,5); Line(t,18,43,25,50,Bronze,6); }
        else { Circle(t,32,32,12,Gold); }
        t.Apply(); s=Sprite.Create(t,new Rect(0,0,64,64),new Vector2(.5f,.5f)); Cache[key]=s; return s;
    }

    public static Sprite Tower(TowerType type)
    {
        string key = "tower_art_" + type;
        if (Cache.TryGetValue(key, out Sprite s)) return s;
        Texture2D t = NewTexture(96,96);
        RadialBackground(t,new Color(.025f,.015f,.01f,1f),new Color(.19f,.075f,.02f,1f));
        Ring(t,48,48,45,40,Bronze); Ring(t,48,48,39,37,Gold*.70f);
        Rect(t,20,70,56,7,DarkBronze); Rect(t,27,64,42,8,Bronze);
        switch(type)
        {
            case TowerType.SpearThrower: DrawTowerSoldier(t,"spear"); break;
            case TowerType.MachineGun: DrawTowerSoldier(t,"archer"); break;
            case TowerType.Cannon: DrawBallistaLarge(t); break;
            case TowerType.Slow: DrawPriestLarge(t); break;
            case TowerType.FireTower: DrawFireCrew(t); break;
            case TowerType.TrojanGuard: DrawGuardLarge(t); break;
            default: Circle(t,48,48,14,Gold); break;
        }
        t.Apply(); s=Sprite.Create(t,new Rect(0,0,96,96),new Vector2(.5f,.5f)); Cache[key]=s; return s;
    }

    public static Sprite Enemy(string key)
    {
        string cacheKey="enemy_"+key;
        if(Cache.TryGetValue(cacheKey,out Sprite s)) return s;
        Texture2D t=NewTexture(72,72);
        RadialBackground(t,new Color(.02f,.018f,.018f,1f),new Color(.08f,.13f,.18f,1f));
        Ring(t,36,36,33,29,new Color(.30f,.42f,.55f,1f));
        if(key=="runner") { GreekHelmet(t,36,38,16); Line(t,18,54,30,46,PaleGold,4); Line(t,54,54,42,46,PaleGold,4); }
        else if(key=="heavy") { GreekHelmet(t,36,37,20); Rect(t,22,47,28,11,new Color(.24f,.29f,.34f,1f)); }
        else if(key=="shield") { GreekHelmet(t,38,38,16); ShieldShape(t,24,42,14,18,new Color(.34f,.43f,.53f,1f)); }
        else if(key=="archer") { GreekHelmet(t,31,40,14); Ring(t,47,38,13,11,new Color(.42f,.48f,.52f,1f)); Line(t,50,25,50,52,PaleGold,2); }
        else if(key=="boss") { GreekHelmet(t,36,37,21); Line(t,36,13,36,27,new Color(.78f,.14f,.04f,1f),5); Ring(t,36,36,28,26,new Color(.78f,.14f,.04f,1f)); }
        else GreekHelmet(t,36,38,17);
        t.Apply(); s=Sprite.Create(t,new Rect(0,0,72,72),new Vector2(.5f,.5f)); Cache[cacheKey]=s; return s;
    }

    static void DrawHector(Texture2D t)
    {
        Rect(t,31,81,66,31,new Color(.25f,.075f,.025f,1f));
        ShieldShape(t,38,86,23,28,Bronze);
        Circle(t,65,65,24,Skin); Rect(t,47,61,36,25,Skin);
        Rect(t,50,62,30,5,new Color(.23f,.09f,.045f,1f));
        Rect(t,55,70,6,4,new Color(.09f,.045f,.025f,1f)); Rect(t,71,70,6,4,new Color(.09f,.045f,.025f,1f));
        Line(t,61,56,70,56,new Color(.24f,.07f,.035f,1f),3);
        Rect(t,42,72,46,11,Bronze); FillTriangle(t,new Vector2(39,75),new Vector2(90,75),new Vector2(65,102),Bronze);
        FillTriangle(t,new Vector2(48,96),new Vector2(82,96),new Vector2(65,119),Gold);
        Rect(t,62,95,6,24,TrojanRed); Rect(t,58,102,14,8,PaleGold);
        Line(t,92,34,92,110,PaleGold,5); FillTriangle(t,new Vector2(86,29),new Vector2(98,29),new Vector2(92,16),Gold);
    }

    static void DrawMenelaus(Texture2D t)
    {
        Rect(t,29,82,70,30,new Color(.16f,.20f,.25f,1f));
        Circle(t,64,65,24,Skin); Rect(t,47,61,35,25,Skin);
        Rect(t,52,69,7,4,new Color(.08f,.04f,.02f,1f)); Rect(t,71,69,7,4,new Color(.08f,.04f,.02f,1f));
        Rect(t,45,75,41,10,new Color(.40f,.48f,.55f,1f));
        FillTriangle(t,new Vector2(40,77),new Vector2(89,77),new Vector2(65,104),new Color(.48f,.54f,.58f,1f));
        Rect(t,61,98,7,21,new Color(.74f,.13f,.045f,1f));
        Line(t,28,102,101,102,new Color(.64f,.16f,.06f,1f),5);
        GreekHelmet(t,65,63,28);
        Line(t,65,14,65,34,new Color(.78f,.12f,.04f,1f),7);
    }

    static void DrawTowerSoldier(Texture2D t,string role)
    {
        Circle(t,48,39,11,Skin); Rect(t,38,50,20,25,Bronze); FillTriangle(t,new Vector2(35,36),new Vector2(61,36),new Vector2(48,21),Gold);
        if(role=="spear") { Line(t,67,20,67,74,PaleGold,4); FillTriangle(t,new Vector2(62,19),new Vector2(72,19),new Vector2(67,9),Gold); }
        else { Ring(t,67,46,17,14,new Color(.55f,.31f,.11f,1f)); Line(t,71,28,71,64,PaleGold,3); Line(t,56,46,78,46,PaleGold,2); }
    }

    static void DrawBallistaLarge(Texture2D t)
    {
        Rect(t,25,55,46,8,Bronze); Rect(t,43,34,10,31,Gold); Line(t,17,42,79,42,PaleGold,5); Line(t,31,62,22,76,DarkBronze,5); Line(t,65,62,74,76,DarkBronze,5); Circle(t,26,70,8,DarkBronze); Circle(t,70,70,8,DarkBronze);
    }

    static void DrawPriestLarge(Texture2D t)
    {
        Circle(t,48,35,11,Skin); FillTriangle(t,new Vector2(31,73),new Vector2(65,73),new Vector2(48,43),new Color(.70f,.50f,.12f,1f)); Ring(t,48,43,24,21,Gold); Line(t,70,24,70,71,PaleGold,4); Circle(t,70,22,7,Gold);
    }

    static void DrawFireCrew(Texture2D t)
    {
        Circle(t,37,40,10,Skin); Rect(t,28,49,18,22,Bronze); Circle(t,59,43,9,Skin); Rect(t,52,51,16,20,DarkBronze); DrawFlame(t,58,30,17,new Color(1f,.20f,.02f,1f),Gold,PaleGold); Rect(t,49,52,20,8,TrojanRed);
    }

    static void DrawGuardLarge(Texture2D t)
    {
        Circle(t,52,37,10,Skin); Rect(t,43,46,18,26,Bronze); ShieldShape(t,34,50,18,24,new Color(.68f,.34f,.08f,1f)); Line(t,69,24,69,73,PaleGold,4);
    }

    static void DrawArcher(Texture2D t){ Ring(t,29,32,17,14,Bronze); Line(t,38,15,38,49,Gold,3); Line(t,18,31,47,31,PaleGold,2); }
    static void DrawSpear(Texture2D t){ Line(t,31,14,31,51,PaleGold,4); FillTriangle(t,new Vector2(26,14),new Vector2(36,14),new Vector2(31,7),Gold); ShieldShape(t,43,38,12,15,Bronze); }
    static void DrawBallista(Texture2D t){ Rect(t,16,34,32,6,Bronze); Rect(t,29,18,6,31,Gold); Line(t,12,22,52,22,PaleGold,3); Circle(t,20,45,6,DarkBronze); Circle(t,44,45,6,DarkBronze); }
    static void DrawApollo(Texture2D t){ Circle(t,32,32,13,new Color(.95f,.64f,.18f,1f)); for(int i=0;i<8;i++){float a=i*Mathf.PI/4f;Line(t,32+(int)(Mathf.Cos(a)*18),32+(int)(Mathf.Sin(a)*18),32+(int)(Mathf.Cos(a)*25),32+(int)(Mathf.Sin(a)*25),PaleGold,2);} }

    static void GreekHelmet(Texture2D t,int cx,int cy,int size)
    {
        Color steel=new Color(.38f,.46f,.52f,1f); Color edge=new Color(.68f,.74f,.76f,1f);
        Circle(t,cx,cy,size,steel); Rect(t,cx-size,cy-size,size*2,size,steel); Rect(t,cx-3,cy-size-9,6,12,new Color(.63f,.12f,.045f,1f)); Rect(t,cx-size+3,cy+2,size*2-6,5,edge); Rect(t,cx-3,cy-5,6,size+7,edge);
    }

    static void ShieldShape(Texture2D t,int cx,int cy,int w,int h,Color c)
    {
        int left=cx-w/2,right=cx+w/2,top=cy-h/2,bottom=cy+h/2;
        for(int y=top;y<=bottom;y++)
        {
            float p=(y-top)/(float)Mathf.Max(1,h); int inset=Mathf.RoundToInt(Mathf.Max(0,p-.58f)*w*.85f);
            for(int x=left+inset;x<=right-inset;x++) if(In(t,x,y)) t.SetPixel(x,y,c);
        }
        Line(t,cx,top+3,cx,bottom-4,Gold,3);
    }

    static void DrawFlame(Texture2D t,int cx,int cy,int size,Color outer,Color middle,Color inner)
    {
        FillTriangle(t,new Vector2(cx-size,cy+size/2),new Vector2(cx+size,cy+size/2),new Vector2(cx,cy-size),outer);
        Circle(t,cx,cy+size/3,size,outer); FillTriangle(t,new Vector2(cx-size/2,cy+size/3),new Vector2(cx+size/2,cy+size/3),new Vector2(cx+size/5,cy-size*3/4),middle); Circle(t,cx,cy+size/3,size/2,middle); Circle(t,cx,cy+size/3,size/4,inner);
    }

    static void RadialBackground(Texture2D t,Color edge,Color center)
    {
        Vector2 c=new Vector2((t.width-1)*.5f,(t.height-1)*.5f); float max=c.magnitude;
        for(int y=0;y<t.height;y++) for(int x=0;x<t.width;x++) { float d=Vector2.Distance(new Vector2(x,y),c)/max; float noise=(((x*13+y*23)%17)-8)*.003f; t.SetPixel(x,y,Color.Lerp(center,edge,Mathf.Clamp01(d))+new Color(noise,noise,noise,0)); }
    }

    static Texture2D NewTexture(int w,int h){Texture2D t=new Texture2D(w,h,TextureFormat.RGBA32,false);t.filterMode=FilterMode.Bilinear;t.wrapMode=TextureWrapMode.Clamp;Color c=new Color(0,0,0,0);for(int y=0;y<h;y++)for(int x=0;x<w;x++)t.SetPixel(x,y,c);return t;}
    static bool In(Texture2D t,int x,int y)=>x>=0&&y>=0&&x<t.width&&y<t.height;
    static void Rect(Texture2D t,int x,int y,int w,int h,Color c){for(int yy=y;yy<y+h;yy++)for(int xx=x;xx<x+w;xx++)if(In(t,xx,yy))t.SetPixel(xx,yy,c);}
    static void Circle(Texture2D t,int cx,int cy,int r,Color c){int rr=r*r;for(int y=-r;y<=r;y++)for(int x=-r;x<=r;x++)if(x*x+y*y<=rr&&In(t,cx+x,cy+y))t.SetPixel(cx+x,cy+y,c);}
    static void Ring(Texture2D t,int cx,int cy,int ro,int ri,Color c){int a=ro*ro,b=ri*ri;for(int y=-ro;y<=ro;y++)for(int x=-ro;x<=ro;x++){int d=x*x+y*y;if(d<=a&&d>=b&&In(t,cx+x,cy+y))t.SetPixel(cx+x,cy+y,c);}}
    static void CornerStud(Texture2D t,int x,int y,Color c){Circle(t,x,y,3,c);Circle(t,x,y,1,PaleGold);}
    static void Line(Texture2D t,int x0,int y0,int x1,int y1,Color c,int width)
    {
        int dx=Mathf.Abs(x1-x0),sx=x0<x1?1:-1,dy=-Mathf.Abs(y1-y0),sy=y0<y1?1:-1,err=dx+dy;
        while(true){Circle(t,x0,y0,Mathf.Max(1,width/2),c);if(x0==x1&&y0==y1)break;int e2=2*err;if(e2>=dy){err+=dy;x0+=sx;}if(e2<=dx){err+=dx;y0+=sy;}}
    }
    static void FillTriangle(Texture2D t,Vector2 a,Vector2 b,Vector2 c,Color color)
    {
        int minX=Mathf.FloorToInt(Mathf.Min(a.x,Mathf.Min(b.x,c.x))),maxX=Mathf.CeilToInt(Mathf.Max(a.x,Mathf.Max(b.x,c.x))); int minY=Mathf.FloorToInt(Mathf.Min(a.y,Mathf.Min(b.y,c.y))),maxY=Mathf.CeilToInt(Mathf.Max(a.y,Mathf.Max(b.y,c.y)));
        float area=Edge(a,b,c); if(Mathf.Abs(area)<.001f)return;
        for(int y=minY;y<=maxY;y++)for(int x=minX;x<=maxX;x++){Vector2 p=new Vector2(x+.5f,y+.5f);float w0=Edge(b,c,p),w1=Edge(c,a,p),w2=Edge(a,b,p);if((w0>=0&&w1>=0&&w2>=0)||(w0<=0&&w1<=0&&w2<=0))if(In(t,x,y))t.SetPixel(x,y,color);}
    }
    static float Edge(Vector2 a,Vector2 b,Vector2 c)=>(c.x-a.x)*(b.y-a.y)-(c.y-a.y)*(b.x-a.x);
}
