using UnityEngine;

namespace FB.Game.Surfaces
{
    /// <summary>Read-only view of a gameplay surface, safe to pass in signals and to UI.</summary>
    public interface ISurface
    {
        int Id { get; }
        string DisplayName { get; }
        MaterialKind Kind { get; }

        /// <summary>Price in $ lost when the surface is ruined.</summary>
        int Value { get; }

        StatusContainer Statuses { get; }

        /// <summary>World bounds of all renderers (recomputed, follows moving props).</summary>
        Bounds Bounds { get; }

        Transform Root { get; }
    }
}
