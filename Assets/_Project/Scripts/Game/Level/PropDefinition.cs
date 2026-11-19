using System;
using System.Collections.Generic;
using FB.Game.Surfaces;
using UnityEngine;

namespace FB.Game.Level
{
    /// <summary>
    /// A prop made of parts. Convention: local +Z points to the wall behind the prop, the front faces -Z.
    /// </summary>
    [Serializable]
    public sealed class PropDefinition
    {
        [SerializeField] private string _id;
        [SerializeField] private Vector3 _position;
        [SerializeField] private float _yaw;
        [SerializeField] private bool _isDynamic;
        [SerializeField, Min(0.01f)] private float _mass = 1f;
        [SerializeField] private List<PropPart> _parts;

        [Header("Gameplay surface (None = decoration)")]
        [SerializeField] private MaterialKind _surface;
        [SerializeField, Min(0)] private int _value;
        [SerializeField] private string _displayName;

        [Header("Imported model (empty = procedural parts)")]
        [Tooltip("Asset path of a CC0 model; when set, Parts are ignored.")]
        [SerializeField] private string _model;
        [Tooltip("Box the model is uniformly scaled to fit (m). Bottom-center sits on the prop origin.")]
        [SerializeField] private Vector3 _modelFit;
        [SerializeField] private List<ModelMaterial> _modelMaterials = new List<ModelMaterial>();

        public PropDefinition(string id, Vector3 position, float yaw, bool isDynamic, float mass,
            params PropPart[] parts)
        {
            _id = id;
            _position = position;
            _yaw = yaw;
            _isDynamic = isDynamic;
            _mass = mass;
            _parts = new List<PropPart>(parts);
        }

        /// <summary>Stable name, later used to attach gameplay components (flammable, electric...).</summary>
        public string Id => _id;
        public Vector3 Position => _position;
        public float Yaw => _yaw;
        public bool IsDynamic => _isDynamic;
        public float Mass => _mass;
        public IReadOnlyList<PropPart> Parts => _parts;
        public MaterialKind Surface => _surface;
        public bool HasSurface => _surface != MaterialKind.None;

        /// <summary>Price in $ (0 = material default).</summary>
        public int Value => _value;

        /// <summary>Name shown in the blame feed and the damage bill.</summary>
        public string DisplayName => _displayName;

        public string Model => _model;
        public bool HasModel => !string.IsNullOrEmpty(_model);
        public Vector3 ModelFit => _modelFit;
        public IReadOnlyList<ModelMaterial> ModelMaterials => _modelMaterials;

        /// <summary>Uses an imported model instead of procedural parts. Returns itself.</summary>
        public PropDefinition WithModel(string model, Vector3 fit, params ModelMaterial[] materials)
        {
            _model = model;
            _modelFit = fit;
            _modelMaterials = new List<ModelMaterial>(materials);
            return this;
        }

        /// <summary>Marks the prop as a gameplay surface. Returns itself for compact layout tables.</summary>
        public PropDefinition WithSurface(MaterialKind kind, int value, string displayName)
        {
            _surface = kind;
            _value = value;
            _displayName = displayName;
            return this;
        }
    }
}
