using System.Collections.Generic;
using FB.Game.Blame;
using FB.Game.Common;
using FB.Game.Signals;
using FB.Game.Surfaces;
using UnityEngine;
using Zenject;

namespace FB.Game.Civilian
{
    /// <summary>
    /// The neighbour bulldozes through loose items: any dynamic gameplay item he runs into gets flung
    /// (harder in panic) and reported once per cooldown as his fault; while he burns, it catches fire too.
    /// </summary>
    public sealed class CivilianChaos
    {
        private const float Interval = 0.1f;
        private const float MinSpeed = 0.4f;

        private readonly CivilianBody _body;
        private readonly SurfaceRegistry _registry;
        private readonly ISurfaceActions _actions;
        private readonly ICauseFactory _causes;
        private readonly SignalBus _signalBus;
        private readonly Collider[] _hits = new Collider[16];
        private readonly Dictionary<int, float> _lastKnock = new Dictionary<int, float>(64);
        private float _next;

        public CivilianChaos(CivilianBody body, SurfaceRegistry registry, ISurfaceActions actions, ICauseFactory causes,
            SignalBus signalBus)
        {
            _body = body;
            _registry = registry;
            _actions = actions;
            _causes = causes;
            _signalBus = signalBus;
        }

        public void Tick()
        {
            float now = Time.time;
            if (now < _next || _body.IsKnocked || _body.Speed < MinSpeed)
            {
                return;
            }

            _next = now + Interval;
            CivilianConfig config = _body.Config;
            Vector3 center = _body.Position + Vector3.up * 0.5f;
            int count = Physics.OverlapSphereNonAlloc(center, config.KnockRadius, _hits, GameLayers.EnvironmentMask,
                QueryTriggerInteraction.Ignore);
            for (int i = 0; i < count; i++)
            {
                Rigidbody body = _hits[i].attachedRigidbody;
                if (body == null || body.isKinematic || !_registry.TryGet(_hits[i], out Surface surface))
                {
                    continue;
                }

                Fling(body, center, config);
                Report(surface, now, config);
                if (_body.IsBurning)
                {
                    // A burning neighbour sets alight whatever he bumps into.
                    _actions.Ignite(surface, _causes.Create(ActorId.Civilian, CauseAction.FireSpread, _body.BurnCause));
                }
            }
        }

        private void Fling(Rigidbody body, Vector3 center, CivilianConfig config)
        {
            Vector3 away = body.worldCenterOfMass - center;
            away.y = 0f;
            away = away.sqrMagnitude > 0.0001f ? away.normalized : _body.View.transform.forward;
            float push = config.KnockSpeed * (_body.IsPanicking ? 1f : 0.5f) / Mathf.Max(1f, Mathf.Sqrt(body.mass));
            body.AddForce((away + Vector3.up * 0.6f) * push, ForceMode.VelocityChange);
            body.AddTorque(Random.onUnitSphere * push * 2f, ForceMode.VelocityChange);
        }

        private void Report(Surface surface, float now, CivilianConfig config)
        {
            if (_lastKnock.TryGetValue(surface.Id, out float last) && now - last < config.KnockCooldown)
            {
                return;
            }

            _lastKnock[surface.Id] = now;
            Cause cause = _causes.Create(ActorId.Civilian, CauseAction.KnockOver);
            _signalBus.Fire(new CivilianIncidentSignal(CivilianIncident.KnockedItem, surface, config.KnockDamage,
                cause));
        }
    }
}
