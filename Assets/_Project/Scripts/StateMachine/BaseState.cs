using UnityEngine;

namespace CannonMonke
{
    public abstract class BaseState : IState
    {
        protected readonly PlayerController player;
        protected readonly Animator animator;

        protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
        protected static readonly int JumpHash = Animator.StringToHash("Jump");
        protected static readonly int FallingHash = Animator.StringToHash("Falling");

        protected const float crossFadeDuration = 0.1f;
        protected const float longCrossFadeDuration = 1f;

        protected BaseState(PlayerController player, Animator animator)
        {
            this.player = player;
            this.animator = animator;
        }

        public virtual void FixedUpdate()
        {
            // no op
        }

        public virtual void OnEnter()
        {
            // no op
        }

        public virtual void OnExit()
        {
            Debug.Log("On Exit Base State");
        }

        public virtual void Update()
        {
            // no op
        }
    }
}
