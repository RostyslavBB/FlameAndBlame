using System;
using System.Collections.Generic;
using FB.Game.Config;
using FB.Game.Effects;
using FB.Game.Signals;
using UnityEngine;
using Zenject;

namespace FB.Game.Hose
{
    /// <summary>
    /// Turns particle collisions into gameplay: pushes rigidbodies, spawns splashes and fires one
    /// <see cref="WaterHitSignal"/> per hit collider per frame (aggregated to keep the bus quiet).
    /// </summary>
    public sealed class WaterJetCollisionHandler : IInitializable, IDisposable
    {
        private const int EventBufferSize = 256;

        private readonly HoseView _view;
        private readonly HoseConfig _config;
        private readonly IHoseFacade _hose;
        private readonly OneShotEffectSpawner<WaterSplash> _splashes;
        private readonly SignalBus _signalBus;
        private readonly List<ParticleCollisionEvent> _events = new List<ParticleCollisionEvent>(EventBufferSize);

        public WaterJetCollisionHandler(HoseView view, HoseConfig config, IHoseFacade hose,
            OneShotEffectSpawner<WaterSplash> splashes, SignalBus signalBus)
        {
            _view = view;
            _config = config;
            _hose = hose;
            _splashes = splashes;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _view.Jet.Collided += OnCollided;
        }

        public void Dispose()
        {
            _view.Jet.Collided -= OnCollided;
        }

        private void OnCollided(GameObject other)
        {
            int count = _view.Jet.Particles.GetCollisionEvents(other, _events);
            if (count == 0)
            {
                return;
            }

            Vector3 pointSum = Vector3.zero;
            Vector3 normalSum = Vector3.zero;
            Collider target = null;
            for (int i = 0; i < count; i++)
            {
                ParticleCollisionEvent hit = _events[i];
                pointSum += hit.intersection;
                normalSum += hit.normal;
                if (target == null)
                {
                    target = hit.colliderComponent as Collider;
                }

                PushBody(hit);
            }

            Vector3 point = pointSum / count;
            Vector3 normal = normalSum.sqrMagnitude > 1e-6f ? normalSum.normalized : Vector3.up;
            _splashes.TrySpawn(point, normal);
            _signalBus.Fire(new WaterHitSignal(point, normal, count * _config.WaterPerParticle, target,
                _hose.CurrentCause));
        }

        private void PushBody(in ParticleCollisionEvent hit)
        {
            if (!(hit.colliderComponent is Collider collider))
            {
                return;
            }

            Rigidbody body = collider.attachedRigidbody;
            if (body == null || body.isKinematic)
            {
                return;
            }

            // A slight lift makes items skid and tumble instead of grinding against the floor.
            Vector3 impulse = (hit.velocity + Vector3.up * hit.velocity.magnitude * 0.15f) * _config.PushPerParticle;
            impulse = Vector3.ClampMagnitude(impulse, body.mass * _config.MaxPushPerHit);
            body.AddForceAtPosition(impulse, hit.intersection, ForceMode.Impulse);
        }
    }
}
