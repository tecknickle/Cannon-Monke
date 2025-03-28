using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;

namespace CannonMonke
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions
    {
        public event UnityAction<Vector2> Move = delegate {};
        public event UnityAction<Vector2> Look = delegate {};
        public event UnityAction EnableMouseControlCamera = delegate {};
        public event UnityAction DisableMouseControlCamera = delegate {};
        public event UnityAction<bool> Jump = delegate {};

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
            // no op
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
            Look.Invoke(context.ReadValue<Vector2>());
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Move.Invoke(context.ReadValue<Vector2>());
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            // no op
        }
    }
}
