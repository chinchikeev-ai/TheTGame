using UnityEngine;

public static class GameLanguage
{
    const string Key = "TheTroyGame.Language";

    public static bool Russian => PlayerPrefs.GetInt(Key, 0) == 1;
    public static string Code => Russian ? "RU" : "EN";

    public static void Toggle()
    {
        PlayerPrefs.SetInt(Key, Russian ? 0 : 1);
        PlayerPrefs.Save();
    }

    public static string T(string en, string ru) => Russian ? ru : en;
}
