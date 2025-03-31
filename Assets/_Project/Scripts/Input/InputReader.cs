using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;

namespace CannonMonke
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "CannonMonke/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions
    {
        public event UnityAction<Vector2> Move = delegate {};
        public event UnityAction<Vector2, bool> Look = delegate {};
        public event UnityAction EnableMouseControlCamera = delegate {};
        public event UnityAction DisableMouseControlCamera = delegate {};
        public event UnityAction<bool> Jump = delegate {};
        public event UnityAction<bool> Fire = delegate {};
        public event UnityAction<bool> Interact = delegate {};
        public event UnityAction<bool> Emote1 = delegate {};

        PlayerInputActions inputActions;

        public Vector3 Direction => (inputActions.Player.Move.ReadValue<Vector2>());

        void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerInputActions();
                inputActions.Player.SetCallbacks(this);
            }

            inputActions.Enable();
        }

        void OnDisable()
        {
            inputActions.Disable();
        }

        public void EnablePlayerActions()
        {
            inputActions.Enable();
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Fire.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Fire.Invoke(false);
                    break;
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Jump.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Jump.Invoke(false);
                    break;
            }
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Look.Invoke(context.ReadValue<Vector2>(), isDeviceMouse(context));
        }

        bool isDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

        public void OnMove(InputAction.CallbackContext context)
        {
            Move.Invoke(context.ReadValue<Vector2>());
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            // no op
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Interact.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Interact.Invoke(false);
                    break;
            }
        }

        public void OnEmote1(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Emote1.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Emote1.Invoke(false);
                    break;
            }
        }
    }
}
