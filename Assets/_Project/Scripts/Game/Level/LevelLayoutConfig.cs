using System.Collections.Generic;
using FB.Game.Config;
using UnityEngine;

namespace FB.Game.Level
{
    /// <summary>
    /// Data description of the apartment. The editor tool Tools/Flame&amp;Blame/Rebuild Level turns it into scene objects.
    /// </summary>
    [CreateAssetMenu(menuName = "FB/Level Layout Config", fileName = "LevelLayoutConfig")]
    public sealed class LevelLayoutConfig : ScriptableObject
    {
        [SerializeField, Min(2f)] private float _wallHeight = 2.8f;
        [SerializeField, Min(0.05f)] private float _wallThickness = 0.15f;
        [SerializeField] private PaletteColor _wallColor = PaletteColor.Wall;
        [SerializeField] private PaletteColor _ceilingColor = PaletteColor.Ceiling;
        [SerializeField] private Vector3 _spawnPosition;
        [SerializeField] private float _spawnYaw;
        [Tooltip("World position of the tap spout where the hose is attached.")]
        [SerializeField] private Vector3 _hoseAnchor;
        [Tooltip("Version of the editor defaults this asset was generated from. Older versions are regenerated.")]
        [SerializeField] private int _defaultsVersion;
        [SerializeField] private List<RoomDefinition> _rooms = new List<RoomDefinition>();
        [SerializeField] private List<WallDefinition> _walls = new List<WallDefinition>();
        [SerializeField] private List<DoorwayDefinition> _doorways = new List<DoorwayDefinition>();
        [SerializeField] private List<PropDefinition> _props = new List<PropDefinition>();

        public float WallHeight => _wallHeight;
        public float WallThickness => _wallThickness;
        public PaletteColor WallColor => _wallColor;
        public PaletteColor CeilingColor => _ceilingColor;
        public Vector3 SpawnPosition => _spawnPosition;
        public float SpawnYaw => _spawnYaw;
        public Vector3 HoseAnchor => _hoseAnchor;
        public int DefaultsVersion => _defaultsVersion;
        public IReadOnlyList<RoomDefinition> Rooms => _rooms;
        public IReadOnlyList<WallDefinition> Walls => _walls;
        public IReadOnlyList<DoorwayDefinition> Doorways => _doorways;
        public IReadOnlyList<PropDefinition> Props => _props;

#if UNITY_EDITOR
        public void EditorSetLayout(int defaultsVersion, Vector3 spawnPosition, float spawnYaw, Vector3 hoseAnchor,
            IEnumerable<RoomDefinition> rooms, IEnumerable<WallDefinition> walls,
            IEnumerable<DoorwayDefinition> doorways, IEnumerable<PropDefinition> props)
        {
            _defaultsVersion = defaultsVersion;
            _spawnPosition = spawnPosition;
            _spawnYaw = spawnYaw;
            _hoseAnchor = hoseAnchor;
            _rooms = new List<RoomDefinition>(rooms);
            _walls = new List<WallDefinition>(walls);
            _doorways = new List<DoorwayDefinition>(doorways);
            _props = new List<PropDefinition>(props);
        }
#endif
    }
}
