using FB.Game.Blame;
using FB.Game.Fire;
using FB.Game.Signals;
using UnityEngine;
using Zenject;

namespace FB.Game.Surfaces
{
    public sealed class SurfaceActions : ISurfaceActions
    {
        private readonly SignalBus _signalBus;
        private readonly ICauseFactory _causes;
        private readonly FireConfig _config;

        public SurfaceActions(SignalBus signalBus, ICauseFactory causes, FireConfig config)
        {
            _signalBus = signalBus;
            _causes = causes;
            _config = config;
        }

        public void AddWater(Surface target, float amount, Cause cause)
        {
            target.Water += amount;
            if (target.Water >= _config.WetThreshold && target.Statuses.Add(SurfaceStatus.Wet, cause))
            {
                StatusChanged(target, SurfaceStatus.Wet, true);
            }
        }

        public void Dry(Surface target)
        {
            if (target.Statuses.Remove(SurfaceStatus.Wet))
            {
                StatusChanged(target, SurfaceStatus.Wet, false);
            }
        }

        public void AddHeat(Surface target, float amount)
        {
            target.Heat = Mathf.Max(0f, target.Heat + amount);
        }

        public void Ignite(Surface target, Cause cause)
        {
            Ignite(target, cause, null);
        }

        public void IgniteFrom(Surface target, Surface source)
        {
            Cause parent = source.Statuses.CauseOf(SurfaceStatus.Burning);
            Ignite(target, _causes.Create(ActorId.Fire, CauseAction.FireSpread, parent), source);
        }

        public void Extinguish(Surface target, Cause cause)
        {
            if (!target.Statuses.Remove(SurfaceStatus.Burning))
            {
                return;
            }

            target.Heat = 0f;
            StatusChanged(target, SurfaceStatus.Burning, false);
            _signalBus.Fire(new FireExtinguishedSignal(target, cause));
        }

        public void Ruin(Surface target, RuinReason reason, Cause parent)
        {
            if (target.IsRuined)
            {
                return;
            }

            if (target.Statuses.Remove(SurfaceStatus.Burning))
            {
                StatusChanged(target, SurfaceStatus.Burning, false);
            }

            Cause cause = reason == RuinReason.Burned
                ? _causes.Create(ActorId.Fire, CauseAction.Burn, parent)
                : _causes.Create(parent?.Instigator ?? ActorId.Player, ToAction(reason), parent);
            target.Statuses.Add(SurfaceStatus.Ruined, cause);
            StatusChanged(target, SurfaceStatus.Ruined, true);
            _signalBus.Fire(new ItemRuinedSignal(target, target.Value, reason, cause));
        }

        private void Ignite(Surface target, Cause cause, Surface source)
        {
            if (target.IsBurning || target.IsRuined || !target.Profile.Flammable)
            {
                return;
            }

            target.Heat = Mathf.Max(target.Heat, target.Profile.IgnitionHeat * 1.2f);
            target.Statuses.Add(SurfaceStatus.Burning, cause);
            StatusChanged(target, SurfaceStatus.Burning, true);
            _signalBus.Fire(new FireSpreadSignal(target, source, cause));
        }

        private void StatusChanged(Surface target, SurfaceStatus status, bool added)
        {
            _signalBus.Fire(new SurfaceStatusChangedSignal(target, status, added));
        }

        private static CauseAction ToAction(RuinReason reason) =>
            reason == RuinReason.Drowned ? CauseAction.Drown : CauseAction.ShortCircuit;
    }
}
