using System.Collections.Generic;
using UnityEngine;

namespace FB.Game.Surfaces
{
    /// <summary>
    /// Runtime model of a gameplay object: statuses plus the numbers simulated by fire and water systems.
    /// Mutated only through <see cref="ISurfaceActions"/> and the simulation; everyone else reads it.
    /// </summary>
    public sealed class Surface : ISurface
    {
        private readonly SurfaceEntity _entity;
        private readonly List<SurfaceNeighbor> _neighbors = new List<SurfaceNeighbor>();

        public Surface(int id, SurfaceEntity entity)
        {
            Id = id;
            _entity = entity;
            Profile = entity.Profile;
            Fuel = Profile.BurnSeconds;
            BaseColors = new Color[entity.Renderers.Length];
        }

        public int Id { get; }
        public string DisplayName => _entity.DisplayName;
        public MaterialKind Kind => Profile.Kind;
        public int Value => _entity.Value;
        public StatusContainer Statuses { get; } = new StatusContainer();
        public Transform Root => _entity.transform;
        public MaterialProfileConfig Profile { get; }
        public SurfaceEntity Entity => _entity;
        public IReadOnlyList<SurfaceNeighbor> Neighbors => _neighbors;

        /// <summary>Original material colors, used to tint wet/charred states.</summary>
        public Color[] BaseColors { get; }

        public float Heat { get; internal set; }
        public float Fuel { get; internal set; }
        public float Water { get; internal set; }

        public bool IsBurning => Statuses.Has(SurfaceStatus.Burning);
        public bool IsRuined => Statuses.Has(SurfaceStatus.Ruined);

        public Bounds Bounds
        {
            get
            {
                MeshRenderer[] renderers = _entity.Renderers;
                Bounds bounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                {
                    bounds.Encapsulate(renderers[i].bounds);
                }

                return bounds;
            }
        }

        internal void AddNeighbor(SurfaceNeighbor neighbor) => _neighbors.Add(neighbor);

        public override string ToString() => DisplayName;
    }
}
