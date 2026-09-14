using System;
using UnityEngine;

public sealed class ChapterRuntimeContext
{
    public ChapterRuntimeContext(Transform[][] paths)
    {
        Paths = paths;
    }

    public Transform[][] Paths { get; }
}

public static class ChapterRuntimeInstaller
{
    public static ChapterRuntimeContext Install(ChapterData chapter, Camera camera)
    {
        if (chapter == null) throw new ArgumentNullException(nameof(chapter));
        if (camera == null) throw new ArgumentNullException(nameof(camera));

        switch (chapter.runtimeProfile)
        {
            case ChapterOneRuntimeInstaller.ProfileId:
                return ChapterOneRuntimeInstaller.Install(chapter, camera);
            default:
                throw new InvalidOperationException($"Unsupported chapter runtime profile '{chapter.runtimeProfile}' for chapter '{chapter.chapterId}'. Add a dedicated runtime installer before enabling this chapter.");
        }
    }
}
