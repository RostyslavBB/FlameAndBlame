using FB.Game.Blame;
using UnityEngine;

namespace FB.Game.Puddles
{
    /// <summary>Runtime model of one puddle on the floor (a disc in XZ).</summary>
    public sealed class Puddle
    {
        /// <summary>Pooled visual; null while the model is not in use.</summary>
        public PuddleView View { get; internal set; }

        /// <summary>The water cause that produced the current <see cref="Cause"/>.</summary>
        internal Cause LastWaterCause { get; set; }
        public Vector3 Center { get; internal set; }
        public float Radius { get; internal set; }
        public float Water { get; internal set; }

        /// <summary>Cause(Puddle) whose parent is the last water that reached this puddle.</summary>
        public Cause Cause { get; internal set; }

        public bool IsElectrified { get; internal set; }

        /// <summary>Cause of the live power source that electrified this puddle.</summary>
        public Cause PowerCause { get; internal set; }

        public bool Contains(Vector3 point, float margin = 0f)
        {
            float dx = point.x - Center.x;
            float dz = point.z - Center.z;
            float radius = Radius + margin;
            return dx * dx + dz * dz <= radius * radius;
        }

        public bool Overlaps(Puddle other)
        {
            float dx = other.Center.x - Center.x;
            float dz = other.Center.z - Center.z;
            float reach = Radius + other.Radius;
            return dx * dx + dz * dz <= reach * reach;
        }

        /// <summary>True if the disc touches the XZ footprint of the bounds (expanded by margin).</summary>
        public bool Touches(Bounds bounds, float margin)
        {
            float x = Mathf.Clamp(Center.x, bounds.min.x - margin, bounds.max.x + margin);
            float z = Mathf.Clamp(Center.z, bounds.min.z - margin, bounds.max.z + margin);
            return Contains(new Vector3(x, Center.y, z));
        }
    }
}
