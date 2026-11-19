using UnityEngine;
using Zenject;

namespace FB.Core.Input
{
    /// <summary>Locks the cursor on click (browser-friendly) and releases it on Escape.</summary>
    public sealed class CursorService : ICursorService, ITickable
    {
        private readonly IInputService _input;

        public CursorService(IInputService input)
        {
            _input = input;
        }

        public bool IsLocked => Cursor.lockState == CursorLockMode.Locked;
        public bool LockAllowed { get; set; } = true;

        public void Tick()
        {
            if (_input.CancelPressed)
            {
                Unlock();
                return;
            }

            if (LockAllowed && !IsLocked && _input.FirePressed)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        public void Unlock()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
