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
        private int _minX = int.MaxValue;
        private int _minZ = int.MaxValue;
        private int _maxX = -1;
        private int _maxZ = -1;

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

        /// <summary>False once every cell has dried: the simulation then costs nothing.</summary>
        public bool HasWater => _maxX >= 0;

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
                    Include(x, z);
                }
            }
        }

        /// <summary>
        /// One flow step over the wet area plus a one-cell border (water can only spread one cell per step), so
        /// a dry floor costs nothing and a single puddle costs only its own size. Returns false when there is no
        /// water; <paramref name="touched"/> is the area whose cells may have changed (for the texture upload).
        /// </summary>
        public bool Step(float flowRate, float film, float evaporation, out RectInt touched)
        {
            touched = default;
            if (!HasWater)
            {
                return false;
            }

            int x0 = Mathf.Max(0, _minX - 1);
            int z0 = Mathf.Max(0, _minZ - 1);
            int x1 = Mathf.Min(Width - 1, _maxX + 1);
            int z1 = Mathf.Min(Height - 1, _maxZ + 1);
            touched = new RectInt(x0, z0, x1 - x0 + 1, z1 - z0 + 1);
            int rowLength = x1 - x0 + 1;
            for (int z = z0; z <= z1; z++)
            {
                int start = z * Width + x0;
                System.Array.Clear(_delta, start, rowLength);
                System.Array.Copy(Causes, start, _nextCause, start, rowLength);
            }

            for (int z = z0; z <= z1; z++)
            {
                for (int x = x0; x <= x1; x++)
                {
                    int index = z * Width + x;
                    if (Blocked[index])
                    {
                        continue;
                    }

                    if (x < x1)
                    {
                        Exchange(index, index + 1, flowRate, film);
                    }

                    if (z < z1)
                    {
                        Exchange(index, index + Width, flowRate, film);
                    }
                }
            }

            _minX = _minZ = int.MaxValue;
            _maxX = _maxZ = -1;
            for (int z = z0; z <= z1; z++)
            {
                for (int x = x0; x <= x1; x++)
                {
                    int index = z * Width + x;
                    float depth = Mathf.Max(0f, Depth[index] + _delta[index] - evaporation);
                    Depth[index] = depth;
                    Causes[index] = depth > 0f ? _nextCause[index] : null;
                    if (depth > 0f)
                    {
                        Include(x, z);
                    }
                }
            }

            return true;
        }

        private void Include(int x, int z)
        {
            _minX = Mathf.Min(_minX, x);
            _minZ = Mathf.Min(_minZ, z);
            _maxX = Mathf.Max(_maxX, x);
            _maxZ = Mathf.Max(_maxZ, z);
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
