using System.Collections.Generic;
using FB.Game.Audio;
using FB.Game.Blame;
using FB.Game.CameraFx;
using FB.Game.Civilian;
using FB.Game.Config;
using FB.Game.Fire;
using FB.Game.Interactions;
using FB.Game.Level;
using FB.Game.Power;
using FB.Game.Puddles;
using FB.Game.Smoke;
using FB.Game.Surfaces;
using FB.Game.UI;
using UnityEditor;
using UnityEngine;

namespace FB.Editor.Configs
{
    /// <summary>
    /// Creates config assets with defaults. Existing assets keep hand-tuned values; the level layout is
    /// regenerated only when the code defaults got a newer version.
    /// </summary>
    internal sealed class ConfigAssets
    {
        private ConfigAssets()
        {
        }

        public PaletteConfig Palette { get; private set; }
        public PlayerConfig Player { get; private set; }
        public RoundConfig Round { get; private set; }
        public HoseConfig Hose { get; private set; }
        public FireConfig Fire { get; private set; }
        public InteractionRulesConfig Rules { get; private set; }
        public PuddleConfig Puddles { get; private set; }
        public SmokeConfig Smoke { get; private set; }
        public PowerConfig Power { get; private set; }
        public ActorRegistryConfig Actors { get; private set; }
        public BlameTextConfig BlameTexts { get; private set; }
        public UiConfig Ui { get; private set; }
        public JuiceConfig Juice { get; private set; }
        public CivilianConfig Civilian { get; private set; }
        public AudioConfig Audio { get; private set; }
        public LevelLayoutConfig Layout { get; private set; }
        public IReadOnlyDictionary<MaterialKind, MaterialProfileConfig> Profiles { get; private set; }

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

            InteractionRulesConfig rules = Load<InteractionRulesConfig>("InteractionRulesConfig");
            FireDefaults.EnsureRules(rules);
            ActorRegistryConfig actors = Load<ActorRegistryConfig>("ActorRegistryConfig");
            BlameDefaults.EnsureActors(actors);
            BlameTextConfig texts = Load<BlameTextConfig>("BlameTextConfig");
            BlameDefaults.EnsureTexts(texts);

            ConfigAssets configs = new ConfigAssets
            {
                Palette = palette,
                Player = Load<PlayerConfig>("PlayerConfig"),
                Round = Load<RoundConfig>("RoundConfig"),
                Hose = Load<HoseConfig>("HoseConfig"),
                Fire = Load<FireConfig>("FireConfig"),
                Rules = rules,
                Puddles = Load<PuddleConfig>("PuddleConfig"),
                Smoke = Load<SmokeConfig>("SmokeConfig"),
                Power = Load<PowerConfig>("PowerConfig"),
                Actors = actors,
                BlameTexts = texts,
                Ui = Load<UiConfig>("UiConfig"),
                Juice = Load<JuiceConfig>("JuiceConfig"),
                Civilian = Load<CivilianConfig>("CivilianConfig"),
                Audio = Load<AudioConfig>("AudioConfig"),
                Layout = layout,
                Profiles = FireDefaults.EnsureProfiles()
            };
            AssetDatabase.SaveAssets();
            return configs;
        }

        private static T Load<T>(string name) where T : ScriptableObject =>
            EditorAssetUtility.LoadOrCreate<T>($"{AssetPaths.Configs}/{name}.asset", out _);
    }
}
