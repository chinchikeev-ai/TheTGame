using System;
using UnityEngine;

public sealed class ChapterRuntimeContext
{
    public ChapterRuntimeContext(
        Transform[][] paths,
        MapBuilder mapBuilder,
        TowerPlacement placement,
        HectorController hector)
    {
        Paths = paths;
        MapBuilder = mapBuilder;
        Placement = placement;
        Hector = hector;
    }

    public Transform[][] Paths { get; }
    public MapBuilder MapBuilder { get; }
    public TowerPlacement Placement { get; }
    public HectorController Hector { get; }
}

public static class ChapterRuntimeInstaller
{
    public static ChapterRuntimeContext Install(ChapterData chapter, GameRuntimeContext runtime)
    {
        if (chapter == null) throw new ArgumentNullException(nameof(chapter));
        if (runtime == null) throw new ArgumentNullException(nameof(runtime));
        if (runtime.Camera == null) throw new ArgumentNullException(nameof(runtime.Camera));

        switch (chapter.runtimeProfile)
        {
            case ChapterOneRuntimeInstaller.ProfileId:
                return ChapterOneRuntimeInstaller.Install(chapter, runtime);
            default:
                throw new InvalidOperationException($"Unsupported chapter runtime profile '{chapter.runtimeProfile}' for chapter '{chapter.chapterId}'. Add a dedicated runtime installer before enabling this chapter.");
        }
    }
}
