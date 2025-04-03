using UnityEngine;

namespace CannonMonke
{
    public class JumpState : BaseState
    {
        public JumpState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            Debug.Log("On Enter Jump State");
            animator.CrossFade(JumpHash, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            player.HandleGravity();
            player.HandleMovement();
        }
    }
}
