using System.Collections.Generic;
using FB.Game.Interactions;
using FB.Game.Surfaces;
using UnityEditor;
using UnityEngine;

namespace FB.Editor.Configs
{
    /// <summary>Material profiles and interaction rule assets. Created once; later edits in the Inspector are kept.</summary>
    internal static class FireDefaults
    {
        private const string ProfilesFolder = AssetPaths.Configs + "/Materials";
        private const string RulesFolder = AssetPaths.Configs + "/Rules";

        public static Dictionary<MaterialKind, MaterialProfileConfig> EnsureProfiles()
        {
            Dictionary<MaterialKind, MaterialProfileConfig> profiles = new Dictionary<MaterialKind, MaterialProfileConfig>
            {
                [MaterialKind.Wood] = Profile(MaterialKind.Wood, true, 1f, 0.7f, 30f, 150, false),
                [MaterialKind.Fabric] = Profile(MaterialKind.Fabric, true, 0.6f, 1.4f, 18f, 100, false),
                [MaterialKind.Electronics] = Profile(MaterialKind.Electronics, true, 1.2f, 0.5f, 15f, 500, true),
                [MaterialKind.Plant] = Profile(MaterialKind.Plant, true, 1.5f, 0.4f, 12f, 80, true),
                [MaterialKind.Metal] = Profile(MaterialKind.Metal, false, 99f, 0f, 1f, 200, false),
                [MaterialKind.Grease] = Profile(MaterialKind.Grease, true, 0.3f, 2f, 120f, 20, false)
            };
            return profiles;
        }

        public static void EnsureRules(InteractionRulesConfig config)
        {
            List<InteractionRule> rules = new List<InteractionRule>
            {
                Rule<ExtinguishRule>("R_Extinguish", null),
                Rule<WaterCoolingRule>("R_WaterCooling", null),
                Rule<WaterDamageRule>("R_ShortCircuit",
                    rule => rule.EditorSetup(MaterialKind.Electronics, 0.8f, RuinReason.ShortCircuit)),
                Rule<WaterDamageRule>("R_Drown", rule => rule.EditorSetup(MaterialKind.Plant, 3f, RuinReason.Drowned)),
                Rule<NeighborIgnitionRule>("R_NeighborIgnition", null)
            };

            if (config.Rules.Count == 0)
            {
                config.EditorSetRules(rules);
                EditorUtility.SetDirty(config);
            }
        }

        private static MaterialProfileConfig Profile(MaterialKind kind, bool flammable, float ignition,
            float flammability, float burnSeconds, int value, bool waterSensitive)
        {
            MaterialProfileConfig profile = EditorAssetUtility.LoadOrCreate<MaterialProfileConfig>(
                $"{ProfilesFolder}/MP_{kind}.asset", out bool created);
            if (created)
            {
                profile.EditorSetup(kind, flammable, ignition, flammability, burnSeconds, value, waterSensitive);
                EditorUtility.SetDirty(profile);
            }

            return profile;
        }

        private static T Rule<T>(string name, System.Action<T> setup) where T : InteractionRule
        {
            T rule = EditorAssetUtility.LoadOrCreate<T>($"{RulesFolder}/{name}.asset", out bool created);
            if (created && setup != null)
            {
                setup(rule);
                EditorUtility.SetDirty(rule);
            }

            return rule;
        }
    }
}
