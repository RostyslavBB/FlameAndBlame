using FB.Game.Blame;
using UnityEngine;

namespace FB.Game.Puddles
{
    /// <summary>
    /// Shallow-water height field over the floor. Each step moves a fraction of the depth difference to the four
    /// neighbours; water thinner than the film thickness stops spreading, which creates natural puddle edges.
    /// Blocked cells (walls, furniture) never hold water. Every cell remembers the cause of the water in it.
    /// </summary>
    public sealed class WaterGrid
    {
        private readonly float[] _delta;
        private readonly Cause[] _nextCause;

        public WaterGrid(Vector2 origin, int width, int height, float cellSize)
        {
            Origin = origin;
            Width = width;
            Height = height;
            CellSize = cellSize;
            Depth = new float[width * height];
            Blocked = new bool[width * height];
            Electrified = new bool[width * height];
            Causes = new Cause[width * height];
            _delta = new float[width * height];
            _nextCause = new Cause[width * height];
        }

        public Vector2 Origin { get; }
        public int Width { get; }
        public int Height { get; }
        public float CellSize { get; }
        public float[] Depth { get; }
        public bool[] Blocked { get; }
        public bool[] Electrified { get; }
        public Cause[] Causes { get; }
        public int Count => Depth.Length;

        public Vector3 CellCenter(int index) =>
            new Vector3(Origin.x + (index % Width + 0.5f) * CellSize, 0f, Origin.y + (index / Width + 0.5f) * CellSize);

        public bool TryGetCell(Vector3 world, out int index)
        {
            int x = Mathf.FloorToInt((world.x - Origin.x) / CellSize);
            int z = Mathf.FloorToInt((world.z - Origin.y) / CellSize);
            index = z * Width + x;
            return x >= 0 && z >= 0 && x < Width && z < Height;
        }

        public float DepthAt(Vector3 world) => TryGetCell(world, out int index) ? Depth[index] : 0f;

        /// <summary>Adds a volume (m3) of water around a world point, spread over a small 3x3 kernel.</summary>
        public void AddVolume(Vector3 world, float volume, Cause cause, float maxDepth)
        {
            if (!TryGetCell(world, out int center))
            {
                return;
            }

            int cx = center % Width;
            int cz = center / Width;
            float perCell = volume / (CellSize * CellSize) / 9f;
            for (int dz = -1; dz <= 1; dz++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    int x = cx + dx;
                    int z = cz + dz;
                    int index = z * Width + x;
                    if (x < 0 || z < 0 || x >= Width || z >= Height || Blocked[index])
                    {
                        continue;
                    }

                    Depth[index] = Mathf.Min(Depth[index] + perCell, maxDepth);
                    Causes[index] = cause;
                }
            }
        }

        public void Step(float flowRate, float film, float evaporation)
        {
            System.Array.Clear(_delta, 0, _delta.Length);
            System.Array.Copy(Causes, _nextCause, Causes.Length);
            for (int z = 0; z < Height; z++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int index = z * Width + x;
                    if (Blocked[index])
                    {
                        continue;
                    }

                    if (x + 1 < Width)
                    {
                        Exchange(index, index + 1, flowRate, film);
                    }

                    if (z + 1 < Height)
                    {
                        Exchange(index, index + Width, flowRate, film);
                    }
                }
            }

            for (int i = 0; i < Depth.Length; i++)
            {
                float depth = Mathf.Max(0f, Depth[i] + _delta[i] - evaporation);
                Depth[i] = depth;
                Causes[i] = depth > 0f ? _nextCause[i] : null;
            }
        }

        private void Exchange(int a, int b, float flowRate, float film)
        {
            if (Blocked[b])
            {
                return;
            }

            float difference = Depth[a] - Depth[b];
            int from = difference > 0f ? a : b;
            int to = difference > 0f ? b : a;
            float source = Depth[from];
            if (source <= film)
            {
                return;
            }

            // At most a quarter of the movable water may leave per neighbour, so four exchanges never overdraw.
            float flow = Mathf.Min(Mathf.Abs(difference) * flowRate, (source - film) * 0.25f);
            _delta[from] -= flow;
            _delta[to] += flow;
            if (Depth[to] <= film)
            {
                _nextCause[to] = Causes[from];
            }
        }
    }
}
