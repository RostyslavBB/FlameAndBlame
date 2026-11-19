using UnityEngine;

namespace FB.Editor.Building
{
    /// <summary>Shuriken setups for flames, smoke, steam and sparks (cartoon look, capped particle counts).</summary>
    internal static class FireFxFactory
    {
        public static ParticleSystem Flames(Transform parent, Material material)
        {
            ParticleSystem particles = WaterParticleFactory.CreateSystem("Flames", parent, material);
            ParticleSystem.MainModule main = particles.main;
            main.loop = true;
            main.playOnAwake = false;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.6f, 1f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.8f, 1.6f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.65f, 1.15f);
            main.startRotation = new ParticleSystem.MinMaxCurve(-0.25f, 0.25f);
            main.gravityModifier = -0.3f;
            main.maxParticles = 160;

            Emission(particles, 30f);
            BoxShape(particles);
            Shrink(particles, 1f, 0.2f);
            Fade(particles, Color.white, 1f);

            ParticleSystem.TextureSheetAnimationModule sheet = particles.textureSheetAnimation;
            sheet.enabled = true;
            sheet.numTilesX = 4;
            sheet.numTilesY = 4;
            sheet.frameOverTime = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 0f, 1f, 1f));
            sheet.cycleCount = 1;
            return particles;
        }

        public static ParticleSystem Smoke(Transform parent, Material material)
        {
            ParticleSystem particles = WaterParticleFactory.CreateSystem("Smoke", parent, material);
            ParticleSystem.MainModule main = particles.main;
            main.loop = true;
            main.playOnAwake = false;
            main.startLifetime = new ParticleSystem.MinMaxCurve(2.5f, 4f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.6f, 1.2f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.6f, 1.1f);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 6.28f);
            main.gravityModifier = -0.12f;
            main.maxParticles = 70;

            Emission(particles, 12f);
            BoxShape(particles);
            Shrink(particles, 1f, 2.2f);
            Fade(particles, new Color(0.16f, 0.15f, 0.15f), 0.7f);
            return particles;
        }

        /// <summary>Bright additive core inside the flames (HDR color, blooms with post-processing).</summary>
        public static ParticleSystem Core(Transform parent, Material material)
        {
            ParticleSystem particles = Flames(parent, material);
            particles.name = "FlameCore";
            ParticleSystem.MainModule main = particles.main;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.35f, 0.6f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.38f);
            main.maxParticles = 50;
            Emission(particles, 22f);
            Fade(particles, new Color(1f, 0.85f, 0.45f), 0.9f);
            return particles;
        }

        /// <summary>Large soft additive glow hugging the base of the fire.</summary>
        public static ParticleSystem Glow(Transform parent, Material material)
        {
            ParticleSystem particles = WaterParticleFactory.CreateSystem("Glow", parent, material);
            ParticleSystem.MainModule main = particles.main;
            main.loop = true;
            main.playOnAwake = false;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.6f, 1f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.05f, 0.2f);
            main.startSize = new ParticleSystem.MinMaxCurve(1f, 1.6f);
            main.maxParticles = 12;
            Emission(particles, 6f);
            BoxShape(particles);
            Fade(particles, new Color(1f, 0.45f, 0.12f), 0.35f);
            return particles;
        }

        /// <summary>Hot embers drifting up with turbulence.</summary>
        public static ParticleSystem Embers(Transform parent, Material material)
        {
            ParticleSystem particles = WaterParticleFactory.CreateSystem("Embers", parent, material);
            ParticleSystem.MainModule main = particles.main;
            main.loop = true;
            main.playOnAwake = false;
            main.startLifetime = new ParticleSystem.MinMaxCurve(1f, 2f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.6f, 1.6f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.025f, 0.05f);
            main.startColor = new Color(1f, 0.6f, 0.2f);
            main.gravityModifier = -0.15f;
            main.maxParticles = 40;
            Emission(particles, 10f);
            BoxShape(particles);

            ParticleSystem.NoiseModule noise = particles.noise;
            noise.enabled = true;
            noise.strength = 0.6f;
            noise.frequency = 0.8f;
            noise.quality = ParticleSystemNoiseQuality.Low;

            Fade(particles, Color.white, 1f);
            ParticleSystemRenderer renderer = particles.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Stretch;
            renderer.velocityScale = 0.06f;
            renderer.lengthScale = 1.5f;
            return particles;
        }

        public static ParticleSystem Steam(Material material)
        {
            ParticleSystem particles = Burst("SteamPuff", material, 10, 25f);
            ParticleSystem.MainModule main = particles.main;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.8f, 1.4f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.8f, 1.6f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.6f);
            main.gravityModifier = -0.2f;
            Shrink(particles, 1f, 2.5f);
            Fade(particles, Color.white, 0.7f);
            return particles;
        }

        public static ParticleSystem Sparks(Material material)
        {
            ParticleSystem particles = Burst("SparkBurst", material, 25, 60f);
            ParticleSystem.MainModule main = particles.main;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.6f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 5f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.03f, 0.06f);
            main.startColor = new Color(1f, 0.95f, 0.5f);
            main.gravityModifier = 1.5f;

            ParticleSystemRenderer renderer = particles.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Stretch;
            renderer.velocityScale = 0.05f;
            renderer.lengthScale = 2f;
            return particles;
        }

        private static ParticleSystem Burst(string name, Material material, short count, float coneAngle)
        {
            ParticleSystem particles = WaterParticleFactory.CreateSystem(name, null, material);
            ParticleSystem.MainModule main = particles.main;
            main.loop = false;
            main.playOnAwake = false;
            main.duration = 0.3f;
            main.maxParticles = count;

            ParticleSystem.EmissionModule emission = particles.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, count) });

            ParticleSystem.ShapeModule shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = coneAngle;
            shape.radius = 0.08f;
            return particles;
        }

        private static void Emission(ParticleSystem particles, float rate)
        {
            ParticleSystem.EmissionModule emission = particles.emission;
            emission.rateOverTime = rate;
        }

        private static void BoxShape(ParticleSystem particles)
        {
            ParticleSystem.ShapeModule shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(0.5f, 0.2f, 0.5f);
            shape.rotation = new Vector3(-90f, 0f, 0f);
        }

        private static void Shrink(ParticleSystem particles, float from, float to)
        {
            ParticleSystem.SizeOverLifetimeModule size = particles.sizeOverLifetime;
            size.enabled = true;
            size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, from, 1f, to));
        }

        private static void Fade(ParticleSystem particles, Color color, float maxAlpha)
        {
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new[] { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) },
                new[]
                {
                    new GradientAlphaKey(0f, 0f), new GradientAlphaKey(maxAlpha, 0.15f), new GradientAlphaKey(0f, 1f)
                });
            ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particles.colorOverLifetime;
            colorOverLifetime.enabled = true;
            colorOverLifetime.color = gradient;
        }
    }
}
