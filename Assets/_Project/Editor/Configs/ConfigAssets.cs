using FB.Game.Config;
using FB.Game.Level;
using UnityEditor;

namespace FB.Editor.Configs
{
    /// <summary>
    /// Creates config assets with defaults. Existing assets keep hand-tuned values; the level layout is
    /// regenerated only when the code defaults got a newer version.
    /// </summary>
    internal sealed class ConfigAssets
    {
        private ConfigAssets(PaletteConfig palette, PlayerConfig player, RoundConfig round, HoseConfig hose,
            LevelLayoutConfig layout)
        {
            Palette = palette;
            Player = player;
            Round = round;
            Hose = hose;
            Layout = layout;
        }

        public PaletteConfig Palette { get; }
        public PlayerConfig Player { get; }
        public RoundConfig Round { get; }
        public HoseConfig Hose { get; }
        public LevelLayoutConfig Layout { get; }

        public static ConfigAssets Ensure()
        {
            PaletteConfig palette = Load<PaletteConfig>("PaletteConfig");
            PaletteDefaults.Merge(palette);
            EditorUtility.SetDirty(palette);

            LevelLayoutConfig layout = Load<LevelLayoutConfig>("LevelLayoutConfig");
            if (layout.DefaultsVersion < LevelLayoutDefaults.Version)
            {
                LevelLayoutDefaults.Apply(layout);
                EditorUtility.SetDirty(layout);
            }

            ConfigAssets configs = new ConfigAssets(palette, Load<PlayerConfig>("PlayerConfig"),
                Load<RoundConfig>("RoundConfig"), Load<HoseConfig>("HoseConfig"), layout);
            AssetDatabase.SaveAssets();
            return configs;
        }

        private static T Load<T>(string name) where T : UnityEngine.ScriptableObject =>
            EditorAssetUtility.LoadOrCreate<T>($"{AssetPaths.Configs}/{name}.asset", out _);
    }
}
