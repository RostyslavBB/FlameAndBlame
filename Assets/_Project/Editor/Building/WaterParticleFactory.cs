using FB.Game.Common;
using UnityEngine;
using UnityEngine.Rendering;

namespace FB.Editor.Building
{
    /// <summary>Shuriken setup for the hose jet and impact splashes, plus their transparent droplet material.</summary>
    internal static class WaterParticleFactory
    {
        public static Material CreateMaterial(Texture2D droplet, Color color) =>
            ParticleMaterialFactory.Create(AssetPaths.WaterParticleMaterial, droplet, color);

        /// <summary>Continuous jet along local +Z. Emission and speed are driven at runtime by HoseController.</summary>
        public static ParticleSystem CreateJet(Transform parent, Material material)
        {
            ParticleSystem particles = CreateSystem("WaterJet", parent, material);
            ParticleSystem.MainModule main = particles.main;
            main.loop = true;
            main.playOnAwake = true;
            main.startLifetime = 1.4f;
            main.startSpeed = 10f;
            main.startSize = 0.12f;
            main.maxParticles = 450;
            main.gravityModifier = 0.6f;

            ParticleSystem.EmissionModule emission = particles.emission;
            emission.rateOverTime = 0f;

            ParticleSystem.ShapeModule shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 2.5f;
            shape.radius = 0.02f;

            ParticleSystem.SizeOverLifetimeModule size = particles.sizeOverLifetime;
            size.enabled = true;
            size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 0.6f, 1f, 1.6f));

            ParticleSystem.CollisionModule collision = particles.collision;
            collision.enabled = true;
            collision.type = ParticleSystemCollisionType.World;
            collision.mode = ParticleSystemCollisionMode.Collision3D;
            // High: exact raycasts, so moving props and the neighbour are never missed by the voxel cache.
            collision.quality = ParticleSystemCollisionQuality.High;
            collision.collidesWith = GameLayers.AllButPlayerMask;
            collision.enableDynamicColliders = true;
            // High quality = exact raycasts; capping the shapes each particle may consider keeps it affordable.
            collision.maxCollisionShapes = 64;
            collision.sendCollisionMessages = true;
            collision.bounce = 0.1f;
            collision.dampen = 0.6f;
            collision.lifetimeLoss = 0.35f;
            collision.radiusScale = 0.5f;

            ParticleSystemRenderer renderer = particles.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Stretch;
            renderer.velocityScale = 0.03f;
            renderer.lengthScale = 1.2f;
            return particles;
        }

        /// <summary>Soft spray cloud around the jet; emission is driven by HoseController with the pressure.</summary>
        public static ParticleSystem CreateMist(Transform parent, Material material)
        {
            ParticleSystem particles = CreateSystem("Mist", parent, material);
            ParticleSystem.MainModule main = particles.main;
            main.loop = true;
            main.playOnAwake = true;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.35f, 0.7f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(5f, 11f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.12f, 0.3f);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 6.28f);
            main.gravityModifier = 0.3f;
            main.maxParticles = 80;

            ParticleSystem.EmissionModule emission = particles.emission;
            emission.rateOverTime = 0f;

            ParticleSystem.ShapeModule shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 9f;
            shape.radius = 0.03f;

            ParticleSystem.SizeOverLifetimeModule size = particles.sizeOverLifetime;
            size.enabled = true;
            size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 0.6f, 1f, 3f));

            Gradient fade = new Gradient();
            fade.SetKeys(new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
                new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.22f, 0.2f), new GradientAlphaKey(0f, 1f) });
            ParticleSystem.ColorOverLifetimeModule color = particles.colorOverLifetime;
            color.enabled = true;
            color.color = fade;
            return particles;
        }

        /// <summary>One-shot burst of droplets along local +Z (the surface normal).</summary>
        public static ParticleSystem CreateSplash(Transform parent, Material material)
        {
            ParticleSystem particles = CreateSystem("WaterSplash", parent, material);
            ParticleSystem.MainModule main = particles.main;
            main.loop = false;
            main.playOnAwake = false;
            main.duration = 0.3f;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.55f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(1.5f, 3.5f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.06f, 0.15f);
            main.maxParticles = 24;
            main.gravityModifier = 1.5f;

            ParticleSystem.EmissionModule emission = particles.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 16) });

            ParticleSystem.ShapeModule shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 40f;
            shape.radius = 0.05f;
            return particles;
        }

        internal static ParticleSystem CreateSystem(string name, Transform parent, Material material)
        {
            GameObject go = new GameObject(name);
            if (parent != null)
            {
                go.transform.SetParent(parent, false);
            }

            ParticleSystem particles = go.AddComponent<ParticleSystem>();
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ParticleSystem.MainModule main = particles.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.startColor = Color.white;

            ParticleSystemRenderer renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            return particles;
        }
    }
}
