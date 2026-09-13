using System.Collections.Generic;
using UnityEngine;

public static class TroyHudArt
{
    static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>();

    public static Sprite Panel(bool boss = false)
    {
        string key = boss ? "panel_boss" : "panel";
        if (Cache.TryGetValue(key, out Sprite s)) return s;
        Texture2D t = NewTexture(64, 64);
        Color baseC = boss ? new Color(.12f,.025f,.018f,1f) : new Color(.075f,.045f,.025f,1f);
        Color bronze = boss ? new Color(.72f,.16f,.045f,1f) : new Color(.55f,.30f,.10f,1f);
        Color gold = new Color(.92f,.62f,.20f,1f);
        for (int y=0;y<64;y++) for (int x=0;x<64;x++)
        {
            int e = Mathf.Min(Mathf.Min(x,63-x), Mathf.Min(y,63-y));
            Color c = e < 3 ? bronze : e < 6 ? gold*.72f : e < 9 ? bronze*.72f : baseC*(.92f + .12f*(x+y)/126f);
            t.SetPixel(x,y,c);
        }
        t.Apply();
        s = Sprite.Create(t,new Rect(0,0,64,64),new Vector2(.5f,.5f),100f,0,SpriteMeshType.FullRect,new Vector4(10,10,10,10));
        Cache[key]=s; return s;
    }

    public static Sprite Icon(string key)
    {
        if (Cache.TryGetValue(key, out Sprite s)) return s;
        Texture2D t = NewTexture(64,64);
        Color bronze = new Color(.62f,.33f,.10f,1f), gold = new Color(1f,.72f,.20f,1f), pale = new Color(1f,.90f,.60f,1f);
        Circle(t,32,32,28,new Color(.045f,.028f,.018f,.98f));
        Ring(t,32,32,27,23,bronze);
        if (key=="gold") { Circle(t,32,32,17,gold); Ring(t,32,32,17,13,pale); Rect(t,29,18,6,28,bronze); Rect(t,23,29,18,5,bronze); }
        else if (key=="gate") { Rect(t,18,20,28,28,bronze); Rect(t,22,16,6,32,gold); Rect(t,36,16,6,32,gold); Rect(t,16,44,32,6,gold); }
        else if (key=="enemy") { Circle(t,32,38,13,bronze); Rect(t,19,20,26,13,bronze); Rect(t,27,31,4,4,pale); Rect(t,35,31,4,4,pale); }
        else if (key=="hector") { Circle(t,32,40,12,new Color(.72f,.42f,.22f,1f)); Rect(t,20,17,24,18,bronze); Rect(t,30,47,4,10,new Color(.72f,.12f,.05f,1f)); }
        else if (key=="boss") { Circle(t,32,39,12,new Color(.70f,.38f,.19f,1f)); Rect(t,20,17,24,18,gold); Rect(t,30,47,4,10,pale); }
        else if (key=="archer") { Ring(t,31,32,17,14,bronze); Rect(t,38,15,3,34,gold); Rect(t,18,31,29,3,pale); }
        else if (key=="spear") { Rect(t,30,15,4,35,pale); Rect(t,26,12,12,7,gold); }
        else if (key=="ballista") { Rect(t,16,34,32,6,bronze); Rect(t,29,18,6,31,gold); Rect(t,12,22,40,4,pale); }
        else if (key=="slow") { Ring(t,32,32,17,14,new Color(.25f,.65f,1f,1f)); Rect(t,30,20,4,14,pale); Rect(t,32,31,11,4,pale); }
        else if (key=="fire") { Rect(t,24,24,16,25,new Color(1f,.28f,.04f,1f)); Circle(t,32,28,10,gold); }
        else if (key=="shield") { Rect(t,18,20,28,25,bronze); Rect(t,29,18,6,30,gold); }
        else if (key=="magic") { Rect(t,29,13,6,34,gold); Rect(t,20,29,24,5,pale); }
        else if (key=="gift") { Rect(t,16,23,32,25,bronze); Rect(t,14,19,36,8,gold); Rect(t,29,18,6,31,pale); }
        else { Circle(t,32,32,12,gold); }
        t.Apply(); s=Sprite.Create(t,new Rect(0,0,64,64),new Vector2(.5f,.5f)); Cache[key]=s; return s;
    }

    public static Sprite Tower(TowerType type)
    {
        switch(type){case TowerType.SpearThrower:return Icon("spear");case TowerType.MachineGun:return Icon("archer");case TowerType.Cannon:return Icon("ballista");case TowerType.Slow:return Icon("slow");case TowerType.FireTower:return Icon("fire");case TowerType.TrojanGuard:return Icon("shield");default:return Icon("dot");}
    }

    static Texture2D NewTexture(int w,int h){Texture2D t=new Texture2D(w,h,TextureFormat.RGBA32,false);t.filterMode=FilterMode.Bilinear;t.wrapMode=TextureWrapMode.Clamp;Color c=new Color(0,0,0,0);for(int y=0;y<h;y++)for(int x=0;x<w;x++)t.SetPixel(x,y,c);return t;}
    static void Rect(Texture2D t,int x,int y,int w,int h,Color c){for(int yy=y;yy<y+h;yy++)for(int xx=x;xx<x+w;xx++)if(xx>=0&&yy>=0&&xx<t.width&&yy<t.height)t.SetPixel(xx,yy,c);}
    static void Circle(Texture2D t,int cx,int cy,int r,Color c){int rr=r*r;for(int y=-r;y<=r;y++)for(int x=-r;x<=r;x++)if(x*x+y*y<=rr&&cx+x>=0&&cy+y>=0&&cx+x<t.width&&cy+y<t.height)t.SetPixel(cx+x,cy+y,c);}
    static void Ring(Texture2D t,int cx,int cy,int ro,int ri,Color c){int a=ro*ro,b=ri*ri;for(int y=-ro;y<=ro;y++)for(int x=-ro;x<=ro;x++){int d=x*x+y*y;if(d<=a&&d>=b&&cx+x>=0&&cy+y>=0&&cx+x<t.width&&cy+y<t.height)t.SetPixel(cx+x,cy+y,c);}}
}
