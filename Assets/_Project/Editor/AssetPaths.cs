namespace FB.Editor
{
    internal static class AssetPaths
    {
        public const string Root = "Assets/_Project";
        public const string Configs = Root + "/Configs";
        public const string Materials = Root + "/Materials";
        public const string Meshes = Root + "/Meshes";
        public const string Prefabs = Root + "/Prefabs";
        public const string Scenes = Root + "/Scenes";
        public const string Resources = Root + "/Resources";
        public const string Textures = Root + "/Textures";
        public const string SplashPrefab = Prefabs + "/WaterSplash.prefab";
        public const string SteamPrefab = Prefabs + "/SteamPuff.prefab";
        public const string SparksPrefab = Prefabs + "/SparkBurst.prefab";
        public const string FireVfxPrefab = Prefabs + "/FireVfx.prefab";
        public const string PuddleMaterial = Materials + "/M_PuddleWater.mat";
        public const string PuddleMesh = Meshes + "/PuddleGrid.asset";
        public const string SmokeMesh = Meshes + "/SmokeLayers.asset";
        public const string SmokeMaterial = Materials + "/M_SmokeLayer.mat";
        public const string MainPostProcess = Configs + "/PostProcess_Main.asset";
        public const string SmokePostProcess = Configs + "/PostProcess_Smoke.asset";
        public const string ShockPostProcess = Configs + "/PostProcess_Shock.asset";
        public const string CivilianNavMesh = Scenes + "/Game_NavMesh.asset";
        public const string SpiralTexture = Textures + "/T_DizzySpiral.asset";
        public const string SpiralMaterial = Materials + "/M_DizzySpiral.mat";
        public const string MistMaterial = Materials + "/M_Mist.mat";
        public const string StreamMaterial = Materials + "/M_WaterStream.mat";
        public const string ArcPrefab = Prefabs + "/ElectricArc.prefab";
        public const string FeedEntryPrefab = Prefabs + "/FeedEntry.prefab";
        public const string Fonts = Root + "/Fonts";
        public const string Audio = Root + "/Audio";
        public const string WaterParticleMaterial = Materials + "/M_WaterParticle.mat";

        public const string GameScene = Scenes + "/Game.unity";
        public const string PlayerPrefab = Prefabs + "/Player.prefab";
        public const string ProjectContextPrefab = Resources + "/ProjectContext.prefab";
        public const string LightingSettings = Configs + "/GameLighting.lighting";
        public const string PlayerPhysicsMaterial = Materials + "/PM_Player.asset";
        public const string WebGLBuildFolder = "Builds/WebGL";
        public const string WindowsBuildExe = "Builds/Windows/FlameAndBlame.exe";
    }
}
