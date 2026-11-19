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
        public const string PuddlePrefab = Prefabs + "/Puddle.prefab";
        public const string ArcPrefab = Prefabs + "/ElectricArc.prefab";
        public const string FeedEntryPrefab = Prefabs + "/FeedEntry.prefab";
        public const string Fonts = Root + "/Fonts";
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
