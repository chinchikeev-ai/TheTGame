using UnityEngine;

public static class ChapterOneRuntimeInstaller
{
    public const string ProfileId = "chapter01_landing";
    const string HectorSelectionTargetName = "HectorSelectionTarget";

    public static ChapterRuntimeContext Install(ChapterData chapter, GameRuntimeContext runtime)
    {
        Camera camera = runtime.Camera;
        ConfigureCameraAndLighting(camera, runtime.Sun);

        MapBuilder mapBuilder = runtime.CreateChapter<MapBuilder>("MapBuilder");
        mapBuilder.BuildMap();

        Transform dressingRoot = ChapterOneVisualEnhancer.Enhance();
        Transform gateRoot = TroyGateHeroBuilder.Build();
        ChapterOneWallLife.Build();

        ChapterOneAtmosphereController atmosphere =
            runtime.CreateChapter<ChapterOneAtmosphereController>("ChapterOneAtmosphere");
        atmosphere.Initialize(camera, runtime.Sun);
        runtime.CreateChapter<ChapterOnePlaythroughReporter>("ChapterOnePlaythroughReporter");

        TowerPlacement placement = runtime.CreateChapter<TowerPlacement>("TowerPlacement");
        placement.gameCamera = camera;

        HectorController hector = CreateHector(runtime, camera, mapBuilder.Paths);
        HectorHUD hectorHud = runtime.CreateChapter<HectorHUD>("HectorHUD");
        runtime.CreateChapter<EnemyInspectorPresentation>("EnemyInspector");
        runtime.CreateChapter<PreMapPatronSelectionPresentation>("PreMapPatronSelection");
        PatronCommentaryPresentation patron = runtime.CreateChapter<PatronCommentaryPresentation>("PatronCommentary");
        runtime.CreateChapter<LandingPresentation>("LandingPresentation");

        ChapterOneCinematicCamera cinematic =
            runtime.CreateChapter<ChapterOneCinematicCamera>("ChapterOneCinematicCamera");
        cinematic.Initialize(camera);

        ChapterOneGuidancePresentation guidance =
            runtime.CreateChapter<ChapterOneGuidancePresentation>("ChapterOneGuidance");

        CreateChapterPresentationStack(
            runtime,
            mapBuilder.CoastRoot,
            dressingRoot,
            gateRoot,
            hectorHud,
            guidance,
            patron);

        RuntimeFileLogger.Event("CHAPTER_RUNTIME", $"Installed {ProfileId} for {chapter.chapterId}");
        return new ChapterRuntimeContext(mapBuilder.Paths, mapBuilder, placement, hector);
    }

    static void CreateChapterPresentationStack(
        GameRuntimeContext runtime,
        Transform coastRoot,
        Transform dressingRoot,
        Transform gateRoot,
        HectorHUD hectorHud,
        ChapterOneGuidancePresentation guidance,
        PatronCommentaryPresentation patron)
    {
        ChapterOneShoreLife shoreLife =
            runtime.CreateChapter<ChapterOneShoreLife>("ChapterOneShoreLife");

        ChapterOneCoastEdgeClosure edgeClosure =
            runtime.CreateChapter<ChapterOneCoastEdgeClosure>("ChapterOneCoastEdgeClosure");
        edgeClosure.Initialize(coastRoot);

        ChapterOneAegeanSeaPresentation sea =
            runtime.CreateChapter<ChapterOneAegeanSeaPresentation>("ChapterOneAegeanSeaPresentation");
        sea.Initialize(coastRoot, shoreLife, edgeClosure);

        ChapterOneAegeanSeaVolumePass seaVolume =
            runtime.CreateChapter<ChapterOneAegeanSeaVolumePass>("ChapterOneAegeanSeaVolumePass");
        seaVolume.Initialize(coastRoot);

        runtime.CreateChapter<ChapterOneBattlefieldDetails>("ChapterOneBattlefieldDetails");
        TroyCityBackdropPresentation city =
            runtime.CreateChapter<TroyCityBackdropPresentation>("TroyCityBackdropPresentation");
        runtime.CreateChapter<TroyFireLifePresentation>("TroyFireLifePresentation");
        runtime.CreateChapter<TroyGateDamagePresentation>("TroyGateDamagePresentation");

        ChapterOneFactionStaging factions =
            runtime.CreateChapter<ChapterOneFactionStaging>("ChapterOneFactionStaging");
        factions.Initialize(coastRoot, dressingRoot, gateRoot, city);

        runtime.CreateChapter<MenelausEntrancePresentation>("MenelausEntrancePresentation");
        runtime.CreateChapter<ChapterOneEncounterPresentation>("ChapterOneEncounterPresentation");

        ChapterOneUiCompactPresentation compact =
            runtime.CreateChapter<ChapterOneUiCompactPresentation>("ChapterOneUiCompactPresentation");
        compact.Initialize(hectorHud, guidance, patron);
    }

    static HectorController CreateHector(
        GameRuntimeContext runtime,
        Camera camera,
        Transform[][] routes)
    {
        GameObject hector = HeroVisualFactory.Create(TroyHeroId.Hector);
        hector.name = "Hector";
        hector.transform.SetParent(runtime.ChapterRoot, true);
        hector.transform.position = HectorRouteNavigator.HasRoutes(routes)
            ? HectorRouteNavigator.GateStart(routes, .6f)
            : MapBuilder.CellToWorld(new Vector2Int(16, 6), .6f);

        HectorController controller = hector.GetComponent<HectorController>();
        if (controller == null) controller = hector.AddComponent<HectorController>();

        controller.ConfigureMovementRoutes(routes, true);
        EnsureHectorSelectionTarget(hector.transform);

        HectorPresentationBridge presentation = hector.GetComponent<HectorPresentationBridge>();
        if (presentation == null) presentation = hector.AddComponent<HectorPresentationBridge>();
        presentation.Initialize(controller);

        HectorSelectionPresentation selection = hector.GetComponent<HectorSelectionPresentation>();
        if (selection == null) selection = hector.AddComponent<HectorSelectionPresentation>();
        selection.Initialize(controller);

        HectorInputDriver input = hector.GetComponent<HectorInputDriver>();
        if (input == null) input = hector.AddComponent<HectorInputDriver>();
        input.Initialize(controller, camera);

        return controller;
    }

    static void EnsureHectorSelectionTarget(Transform hector)
    {
        Transform target = hector.Find(HectorSelectionTargetName);
        if (target == null)
        {
            GameObject go = new GameObject(HectorSelectionTargetName);
            target = go.transform;
            target.SetParent(hector, false);
        }

        target.localPosition = Vector3.zero;
        target.localRotation = Quaternion.identity;
        target.localScale = Vector3.one;
        target.gameObject.SetActive(true);

        CapsuleCollider collider = target.GetComponent<CapsuleCollider>();
        if (collider == null) collider = target.gameObject.AddComponent<CapsuleCollider>();
        collider.enabled = true;
        collider.isTrigger = true;
        collider.center = new Vector3(0f, .9f, 0f);
        collider.height = 1.9f;
        collider.radius = .48f;
        collider.direction = 1;
    }

    static void ConfigureCameraAndLighting(Camera camera, Light sun)
    {
        camera.orthographic = true;
        camera.orthographicSize = 10.6f;
        camera.transform.position = new Vector3(.35f, 28.2f, -4.8f);
        camera.transform.rotation = Quaternion.Euler(73f, -1.5f, 0f);
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(.18f, .27f, .30f);

        CameraController controller = camera.GetComponent<CameraController>();
        if (controller == null) controller = camera.gameObject.AddComponent<CameraController>();
        controller.xBounds = new Vector2(-8.0f, 8.0f);
        controller.zBounds = new Vector2(-11.5f, 5.5f);
        controller.minOrthoSize = 7f;
        controller.maxOrthoSize = 13f;

        if (sun == null) return;
        sun.type = LightType.Directional;
        sun.intensity = 1.28f;
        sun.color = new Color(1f, .77f, .54f);
        sun.shadows = LightShadows.Soft;
        sun.shadowStrength = .76f;
        sun.transform.rotation = Quaternion.Euler(51f, -34f, 0f);
    }
}
