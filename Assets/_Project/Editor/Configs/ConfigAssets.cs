using FB.Game.Config;
using FB.Game.Level;
using UnityEditor;

namespace FB.Editor.Configs
{
    /// <summary>Creates config assets with defaults. Existing assets keep the values tuned by hand.</summary>
    internal sealed class ConfigAssets
    {
        private ConfigAssets(PaletteConfig palette, PlayerConfig player, RoundConfig round, LevelLayoutConfig layout)
        {
            Palette = palette;
            Player = player;
            Round = round;
            Layout = layout;
        }

        public PaletteConfig Palette { get; }
        public PlayerConfig Player { get; }
        public RoundConfig Round { get; }
        public LevelLayoutConfig Layout { get; }

        public static ConfigAssets Ensure()
        {
            PaletteConfig palette = EditorAssetUtility.LoadOrCreate<PaletteConfig>(Path("PaletteConfig"), out _);
            PaletteDefaults.Merge(palette);
            EditorUtility.SetDirty(palette);

            PlayerConfig player = EditorAssetUtility.LoadOrCreate<PlayerConfig>(Path("PlayerConfig"), out _);
            RoundConfig round = EditorAssetUtility.LoadOrCreate<RoundConfig>(Path("RoundConfig"), out _);

            LevelLayoutConfig layout = EditorAssetUtility.LoadOrCreate<LevelLayoutConfig>(
                Path("LevelLayoutConfig"), out bool layoutCreated);
            if (layoutCreated)
            {
                LevelLayoutDefaults.Apply(layout);
                EditorUtility.SetDirty(layout);
            }

            AssetDatabase.SaveAssets();
            return new ConfigAssets(palette, player, round, layout);
        }

        private static string Path(string name) => $"{AssetPaths.Configs}/{name}.asset";
    }
}
