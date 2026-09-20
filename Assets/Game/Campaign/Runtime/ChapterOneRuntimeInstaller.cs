using UnityEngine;

public static class ChapterOneRuntimeInstaller
{
    public const string ProfileId = "chapter01_landing";
    const string HectorSelectionTargetName = "HectorSelectionTarget";

    public static ChapterRuntimeContext Install(ChapterData chapter, Camera camera)
    {
        ConfigureCameraAndLighting(camera);

        MapBuilder mapBuilder = EnsureComponent<MapBuilder>("MapBuilder");
        bool worldNeedsBuild = mapBuilder.Paths == null || mapBuilder.Paths.Length == 0;
        Transform dressingRoot = null;
        Transform gateRoot = null;
        if (worldNeedsBuild)
        {
            mapBuilder.BuildMap();
            dressingRoot = ChapterOneVisualEnhancer.Enhance();
            gateRoot = TroyGateHeroBuilder.Build();
            ChapterOneWallLife.Build();
        }

        EnsureComponent<ChapterOneAtmosphereController>("ChapterOneAtmosphere");
        EnsureComponent<ChapterOnePlaythroughReporter>("ChapterOnePlaythroughReporter");

        TowerPlacement placement = EnsureComponent<TowerPlacement>("TowerPlacement");
        if (placement.gameCamera == null) placement.gameCamera = camera;

        EnsureHector(camera, mapBuilder.Paths);
        HectorHUD hectorHud = EnsureComponent<HectorHUD>("HectorHUD");
        EnsureComponent<EnemyInspectorPresentation>("EnemyInspector");
        EnsureComponent<PreMapPatronSelectionPresentation>("PreMapPatronSelection");
        PatronCommentaryPresentation patron = EnsureComponent<PatronCommentaryPresentation>("PatronCommentary");
        EnsureComponent<LandingPresentation>("LandingPresentation");

        ChapterOneCinematicCamera cinematic = Object.FindFirstObjectByType<ChapterOneCinematicCamera>();
        if (cinematic == null)
        {
            cinematic = new GameObject("ChapterOneCinematicCamera").AddComponent<ChapterOneCinematicCamera>();
            cinematic.Initialize(camera);
        }

        ChapterOneGuidancePresentation guidance = EnsureComponent<ChapterOneGuidancePresentation>("ChapterOneGuidance");
        EnsureChapterPresentationStack(mapBuilder.CoastRoot, dressingRoot, gateRoot, hectorHud, guidance, patron);

        RuntimeFileLogger.Event("CHAPTER_RUNTIME", $"Installed {ProfileId} for {chapter.chapterId}");
        return new ChapterRuntimeContext(mapBuilder.Paths);
    }

    static void EnsureChapterPresentationStack(
        Transform coastRoot,
        Transform dressingRoot,
        Transform gateRoot,
        HectorHUD hectorHud,
        ChapterOneGuidancePresentation guidance,
        PatronCommentaryPresentation patron)
    {
        // Chapter I owns these lifecycle components. Dependencies are injected from
        // the composition root instead of rediscovered by scene object names.
        ChapterOneShoreLife shoreLife = EnsureComponent<ChapterOneShoreLife>("ChapterOneShoreLife");
        ChapterOneCoastEdgeClosure edgeClosure = EnsureComponent<ChapterOneCoastEdgeClosure>("ChapterOneCoastEdgeClosure");
        edgeClosure.Initialize(coastRoot);

        ChapterOneAegeanSeaPresentation sea = EnsureComponent<ChapterOneAegeanSeaPresentation>("ChapterOneAegeanSeaPresentation");
        sea.Initialize(coastRoot, shoreLife, edgeClosure);

        ChapterOneAegeanSeaVolumePass seaVolume = EnsureComponent<ChapterOneAegeanSeaVolumePass>("ChapterOneAegeanSeaVolumePass");
        seaVolume.Initialize(coastRoot);

        EnsureComponent<ChapterOneBattlefieldDetails>("ChapterOneBattlefieldDetails");
        TroyCityBackdropPresentation city = EnsureComponent<TroyCityBackdropPresentation>("TroyCityBackdropPresentation");
        EnsureComponent<TroyFireLifePresentation>("TroyFireLifePresentation");
        EnsureComponent<TroyGateDamagePresentation>("TroyGateDamagePresentation");

        ChapterOneFactionStaging factions = EnsureComponent<ChapterOneFactionStaging>("ChapterOneFactionStaging");
        factions.Initialize(coastRoot, dressingRoot, gateRoot, city);

        EnsureComponent<MenelausEntrancePresentation>("MenelausEntrancePresentation");
        EnsureComponent<ChapterOneEncounterPresentation>("ChapterOneEncounterPresentation");
        ChapterOneUiCompactPresentation compact = EnsureComponent<ChapterOneUiCompactPresentation>("ChapterOneUiCompactPresentation");
        compact.Initialize(hectorHud, guidance, patron);
    }

    static T EnsureComponent<T>(string objectName) where T : Component
    {
        T existing = Object.FindFirstObjectByType<T>();
        return existing != null ? existing : new GameObject(objectName).AddComponent<T>();
    }

    static void EnsureHector(Camera camera, Transform[][] routes)
    {
        HectorController controller = Object.FindFirstObjectByType<HectorController>();
        GameObject hector;

        if (controller == null)
        {
            hector = HeroVisualFactory.Create(TroyHeroId.Hector);
            hector.name = "Hector";
            hector.transform.position = HectorRouteNavigator.HasRoutes(routes)
                ? HectorRouteNavigator.GateStart(routes, .6f)
                : MapBuilder.CellToWorld(new Vector2Int(16, 6), .6f);
            controller = hector.AddComponent<HectorController>();
        }
        else
        {
            hector = controller.gameObject;
        }

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

    static void ConfigureCameraAndLighting(Camera camera)
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

        if (Object.FindFirstObjectByType<Light>() == null)
        {
            GameObject lightObject = new GameObject("Directional Light");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.28f;
            light.color = new Color(1f, .77f, .54f);
            light.shadows = LightShadows.Soft;
            light.shadowStrength = .76f;
            lightObject.transform.rotation = Quaternion.Euler(51f, -34f, 0f);
        }
    }
}
