using UnityEngine;

namespace FB.Core.Input
{
    /// <summary>Read-only view of player input. Actions are defined in code, no .inputactions assets.</summary>
    public interface IInputService
    {
        /// <summary>WASD / arrows / left stick, each axis in -1..1.</summary>
        Vector2 Move { get; }

        /// <summary>Mouse delta in pixels for the current frame.</summary>
        Vector2 Look { get; }

        bool JumpPressed { get; }
        bool FireHeld { get; }
        bool FirePressed { get; }
        bool InteractPressed { get; }
        bool RestartPressed { get; }
        bool CancelPressed { get; }

        /// <summary>F1 or backquote (F1 opens browser help in some browsers).</summary>
        bool DebugPressed { get; }
    }
}
