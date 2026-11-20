using System.Collections.Generic;
using FB.Game.Audio;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FB.Editor.Audio
{
    /// <summary>
    /// Synthesizes every game sound into WAV files (deterministic seeds, so rebuilding is idempotent), fills the
    /// <see cref="AudioConfig"/> mixing table and builds the scene's pool of audio sources.
    /// </summary>
    internal static class AudioAssetsBuilder
    {
        private const int Voices = 12;
        private static readonly Vector2 NoPitch = Vector2.one;

        public static void BuildConfig(AudioConfig config)
        {
            EditorAssetUtility.EnsureFolder(AssetPaths.Audio);
            List<SoundEntry> sounds = new List<SoundEntry>
            {
                Entry(SoundId.HoseLoop, 0.5f, NoPitch, 0f, false, Clip("Hose_Loop", AmbienceRecipes.HoseLoop(1))),
                Entry(SoundId.FireLoop, 0.75f, NoPitch, 0f, true, Clip("Fire_Loop", AmbienceRecipes.FireLoop(2))),
                Entry(SoundId.Steam, 0.6f, new Vector2(0.9f, 1.15f), 0.15f, true, Clip("Steam", AmbienceRecipes.Steam(5))),
                Entry(SoundId.Zap, 0.8f, new Vector2(0.9f, 1.1f), 0.25f, true,
                    Clip("Zap_A", HitRecipes.Zap(6)), Clip("Zap_B", HitRecipes.Zap(7))),
                Entry(SoundId.Spark, 0.8f, new Vector2(0.9f, 1.2f), 0.1f, true, Clip("Spark", HitRecipes.Spark(8))),
                Entry(SoundId.Impact, 0.7f, new Vector2(0.9f, 1.1f), 0.04f, true,
                    Clip("Impact_A", HitRecipes.Impact(9, 140f)), Clip("Impact_B", HitRecipes.Impact(10, 180f)),
                    Clip("Impact_C", HitRecipes.Impact(11, 230f))),
                Entry(SoundId.FeedLine, 0.4f, NoPitch, 0.1f, false, Clip("Cash", UiRecipes.Cash())),
                Entry(SoundId.Tick, 0.45f, NoPitch, 0.05f, false, Clip("Tick", UiRecipes.Tick())),
                Entry(SoundId.Stamp, 1f, NoPitch, 0f, false, Clip("Stamp", HitRecipes.Stamp(15))),
                Entry(SoundId.Whoosh, 0.6f, new Vector2(0.95f, 1.05f), 0.1f, false, Clip("Whoosh", AmbienceRecipes.Whoosh(16))),
                Entry(SoundId.Pop, 0.5f, NoPitch, 0.03f, false, Clip("Pop", UiRecipes.Pop()))
            };

            config.EditorSetSounds(sounds.ToArray());
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }

        public static AudioRig BuildRig(Scene scene)
        {
            GameObject root = new GameObject("Audio");
            SceneManager.MoveGameObjectToScene(root, scene);
            AudioSource[] voices = new AudioSource[Voices];
            for (int i = 0; i < Voices; i++)
            {
                voices[i] = Source(root.transform, $"Voice{i}", true);
            }

            AudioRig rig = root.AddComponent<AudioRig>();
            EditorAssetUtility.SetArray(rig, "_voices", voices);
            EditorAssetUtility.SetReference(rig, "_ui", Source(root.transform, "Ui", false));
            EditorAssetUtility.SetReference(rig, "_hoseLoop", Source(root.transform, "HoseLoop", false));
            EditorAssetUtility.SetReference(rig, "_fireLoop", Source(root.transform, "FireLoop", true));
            return rig;
        }

        private static AudioSource Source(Transform parent, string name, bool spatial)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            AudioSource source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = spatial ? 1f : 0f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = 1.5f;
            source.maxDistance = 14f;
            source.dopplerLevel = 0f;
            return source;
        }

        private static SoundEntry Entry(SoundId id, float volume, Vector2 pitch, float cooldown, bool spatial,
            params AudioClip[] clips) =>
            new SoundEntry(id, clips, volume, pitch, cooldown, spatial);

        private static AudioClip Clip(string name, float[] samples)
        {
            string path = $"{AssetPaths.Audio}/SFX_{name}.wav";
            WavFile.Write(path, samples, SoundSynth.Rate);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);

            AudioImporter importer = (AudioImporter)AssetImporter.GetAtPath(path);
            importer.forceToMono = true;
            AudioImporterSampleSettings settings = importer.defaultSampleSettings;
            settings.loadType = AudioClipLoadType.DecompressOnLoad;
            settings.compressionFormat = AudioCompressionFormat.Vorbis;
            settings.quality = 0.6f;
            importer.defaultSampleSettings = settings;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        }
    }
}
