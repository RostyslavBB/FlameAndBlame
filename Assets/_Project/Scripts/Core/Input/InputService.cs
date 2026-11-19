using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace FB.Core.Input
{
    /// <summary>Builds the gameplay action map in code and exposes it through <see cref="IInputService"/>.</summary>
    public sealed class InputService : IInputService, IInitializable, IDisposable
    {
        private readonly InputActionMap _map = new InputActionMap("Gameplay");
        private InputAction _move;
        private InputAction _look;
        private InputAction _jump;
        private InputAction _fire;
        private InputAction _interact;
        private InputAction _restart;
        private InputAction _cancel;

        public Vector2 Move => _move.ReadValue<Vector2>();
        public Vector2 Look => _look.ReadValue<Vector2>();
        public bool JumpPressed => _jump.WasPressedThisFrame();
        public bool FireHeld => _fire.IsPressed();
        public bool FirePressed => _fire.WasPressedThisFrame();
        public bool InteractPressed => _interact.WasPressedThisFrame();
        public bool RestartPressed => _restart.WasPressedThisFrame();
        public bool CancelPressed => _cancel.WasPressedThisFrame();

        public void Initialize()
        {
            _move = _map.AddAction("Move", InputActionType.Value);
            _move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
            _move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow").With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow").With("Right", "<Keyboard>/rightArrow");
            _move.AddBinding("<Gamepad>/leftStick");

            _look = _map.AddAction("Look", InputActionType.Value, "<Mouse>/delta");
            _jump = AddButton("Jump", "<Keyboard>/space", "<Gamepad>/buttonSouth");
            _fire = AddButton("Fire", "<Mouse>/leftButton", "<Gamepad>/rightTrigger");
            _interact = AddButton("Interact", "<Keyboard>/e", "<Gamepad>/buttonWest");
            _restart = AddButton("Restart", "<Keyboard>/r", "<Gamepad>/start");
            _cancel = AddButton("Cancel", "<Keyboard>/escape", "<Gamepad>/select");

            _map.Enable();
        }

        public void Dispose()
        {
            _map.Disable();
            _map.Dispose();
        }

        private InputAction AddButton(string name, string binding, string gamepadBinding)
        {
            InputAction action = _map.AddAction(name, InputActionType.Button, binding);
            action.AddBinding(gamepadBinding);
            return action;
        }
    }
}
