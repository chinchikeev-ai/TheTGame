using UnityEngine;

public static class GameUserSettings
{
    const string MasterVolumeKey = "TheTroyGame.Settings.MasterVolume";
    const string MusicVolumeKey = "TheTroyGame.Settings.MusicVolume";
    const string FullscreenKey = "TheTroyGame.Settings.Fullscreen";
    const string ResolutionWidthKey = "TheTroyGame.Settings.ResolutionWidth";
    const string ResolutionHeightKey = "TheTroyGame.Settings.ResolutionHeight";
    const string VSyncKey = "TheTroyGame.Settings.VSync";
    const string FpsLimitKey = "TheTroyGame.Settings.FpsLimit";

    public static float MasterVolume
    {
        get => PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        set
        {
            PlayerPrefs.SetFloat(MasterVolumeKey, Mathf.Clamp01(value));
            ApplyAudio();
        }
    }

    public static float MusicVolume
    {
        get => PlayerPrefs.GetFloat(MusicVolumeKey, 0.8f);
        set
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, Mathf.Clamp01(value));
            ApplyAudio();
        }
    }

    public static bool Fullscreen
    {
        get => PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;
        set
        {
            PlayerPrefs.SetInt(FullscreenKey, value ? 1 : 0);
            Screen.fullScreen = value;
        }
    }

    public static bool VSync
    {
        get => PlayerPrefs.GetInt(VSyncKey, QualitySettings.vSyncCount > 0 ? 1 : 0) == 1;
        set
        {
            PlayerPrefs.SetInt(VSyncKey, value ? 1 : 0);
            QualitySettings.vSyncCount = value ? 1 : 0;
        }
    }

    public static int FpsLimit
    {
        get => PlayerPrefs.GetInt(FpsLimitKey, 60);
        set
        {
            int normalized = value <= 0 ? -1 : Mathf.Clamp(value, 30, 240);
            PlayerPrefs.SetInt(FpsLimitKey, normalized);
            Application.targetFrameRate = normalized;
        }
    }

    public static int ResolutionWidth => PlayerPrefs.GetInt(ResolutionWidthKey, Screen.width);
    public static int ResolutionHeight => PlayerPrefs.GetInt(ResolutionHeightKey, Screen.height);

    public static void ApplySaved()
    {
        AudioListener.volume = MasterVolume;
        QualitySettings.vSyncCount = VSync ? 1 : 0;
        Application.targetFrameRate = FpsLimit;

        int width = ResolutionWidth;
        int height = ResolutionHeight;
        if (width > 0 && height > 0)
            Screen.SetResolution(width, height, Fullscreen);

        ApplyAudio();
    }

    public static void SetResolution(int width, int height)
    {
        if (width <= 0 || height <= 0) return;
        PlayerPrefs.SetInt(ResolutionWidthKey, width);
        PlayerPrefs.SetInt(ResolutionHeightKey, height);
        Screen.SetResolution(width, height, Fullscreen);
    }

    public static void Save() => PlayerPrefs.Save();

    public static void ResetToDefaults()
    {
        PlayerPrefs.DeleteKey(MasterVolumeKey);
        PlayerPrefs.DeleteKey(MusicVolumeKey);
        PlayerPrefs.DeleteKey(FullscreenKey);
        PlayerPrefs.DeleteKey(ResolutionWidthKey);
        PlayerPrefs.DeleteKey(ResolutionHeightKey);
        PlayerPrefs.DeleteKey(VSyncKey);
        PlayerPrefs.DeleteKey(FpsLimitKey);
        ApplySaved();
        Save();
    }

    public static void ApplyAudio()
    {
        AudioListener.volume = MasterVolume;
        if (AncientMusicController.Instance != null)
            AncientMusicController.Instance.SetVolume(MusicVolume);
    }
}
