using System;
using FB.Game.Blame;
using FB.Game.Fire;
using FB.Game.Signals;
using UnityEngine;
using Zenject;

namespace FB.Game.Surfaces
{
    /// <summary>Wet surfaces get darker, burned ones turn black, broken ones dim. Uses property blocks.</summary>
    public sealed class SurfaceTintService : IInitializable, IDisposable
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        private readonly SignalBus _signalBus;
        private readonly SurfaceRegistry _registry;
        private readonly FireConfig _config;
        private readonly MaterialPropertyBlock _block = new MaterialPropertyBlock();

        public SurfaceTintService(SignalBus signalBus, SurfaceRegistry registry, FireConfig config)
        {
            _signalBus = signalBus;
            _registry = registry;
            _config = config;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<SurfaceStatusChangedSignal>(OnStatusChanged);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<SurfaceStatusChangedSignal>(OnStatusChanged);
        }

        private void OnStatusChanged(SurfaceStatusChangedSignal signal)
        {
            Surface surface = _registry.All[signal.Surface.Id];
            float tint = TintOf(surface);
            MeshRenderer[] renderers = surface.Entity.Renderers;
            for (int i = 0; i < renderers.Length; i++)
            {
                if (Mathf.Approximately(tint, 1f))
                {
                    renderers[i].SetPropertyBlock(null);
                    continue;
                }

                Color color = surface.BaseColors[i];
                _block.SetColor(BaseColorId, new Color(color.r * tint, color.g * tint, color.b * tint, color.a));
                renderers[i].SetPropertyBlock(_block);
            }
        }

        private float TintOf(Surface surface)
        {
            StatusContainer statuses = surface.Statuses;
            if (statuses.Has(SurfaceStatus.Ruined))
            {
                Cause cause = statuses.CauseOf(SurfaceStatus.Ruined);
                return cause != null && cause.Action == CauseAction.Burn ? _config.CharredTint : _config.BrokenTint;
            }

            return statuses.Has(SurfaceStatus.Wet) ? _config.WetTint : 1f;
        }
    }
}
