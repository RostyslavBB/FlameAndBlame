using UnityEngine;
using UnityEngine.Rendering;

namespace FB.Editor.Building
{
    /// <summary>
    /// Big soft smoke billboards drifting just under the ceiling. Billboards always face the camera, so the smoke
    /// reads as a volume from any angle, which the flat layers alone cannot do. The emission rate is driven at
    /// runtime by the amount of smoke (SmokeView); the particle count is capped for WebGL overdraw.
    /// </summary>
    internal static class CeilingPuffs
    {
        private const int MaxPuffs = 45;
        private static readonly Color PuffColor = new Color(0.36f, 0.33f, 0.32f);

        public static ParticleSystem Create(Transform layer, Vector2 area, Material material)
        {
            ParticleSystem particles = WaterParticleFactory.CreateSystem("CeilingPuffs", layer, material);
            particles.transform.localPosition = new Vector3(0f, 0.25f, 0f);
            particles.transform.localRotation = Quaternion.identity;

            ParticleSystem.MainModule main = particles.main;
            main.loop = true;
            main.playOnAwake = true;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.startLifetime = new ParticleSystem.MinMaxCurve(7f, 11f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(0.03f, 0.12f);
            main.startSize = new ParticleSystem.MinMaxCurve(1.6f, 2.8f);
            main.startRotation = new ParticleSystem.MinMaxCurve(0f, 6.28f);
            main.startColor = PuffColor;
            main.gravityModifier = 0f;
            main.maxParticles = MaxPuffs;

            ParticleSystem.EmissionModule emission = particles.emission;
            emission.enabled = true;
            emission.rateOverTime = 0f;

            ParticleSystem.ShapeModule shape = particles.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(area.x, 0.35f, area.y);
            shape.rotation = Vector3.zero;
            shape.randomDirectionAmount = 1f;

            ParticleSystem.RotationOverLifetimeModule rotation = particles.rotationOverLifetime;
            rotation.enabled = true;
            rotation.z = new ParticleSystem.MinMaxCurve(-0.15f, 0.15f);

            ParticleSystem.SizeOverLifetimeModule size = particles.sizeOverLifetime;
            size.enabled = true;
            size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 0.75f, 1f, 1.25f));

            ParticleSystem.ColorOverLifetimeModule color = particles.colorOverLifetime;
            color.enabled = true;
            Gradient fade = new Gradient();
            fade.SetKeys(
                new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
                new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.32f, 0.3f), new GradientAlphaKey(0.32f, 0.7f), new GradientAlphaKey(0f, 1f) });
            color.color = fade;

            ParticleSystem.CollisionModule collision = particles.collision;
            collision.enabled = false;

            ParticleSystemRenderer renderer = particles.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            // A puff right in front of the lens would fill the screen: cap its on-screen size.
            renderer.maxParticleSize = 0.35f;
            renderer.sortingFudge = 10f;
            return particles;
        }
    }
}
