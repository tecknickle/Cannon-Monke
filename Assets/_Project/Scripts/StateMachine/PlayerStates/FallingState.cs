using UnityEngine;

namespace CannonMonke
{
    public class FallingState : BaseState
    {
        public FallingState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            Debug.Log("Enter Falling State");
            animator.CrossFade(FallingHash, longCrossFadeDuration);
        }

        public override void FixedUpdate()
        {
            player.HandleGravity();
            player.HandleMovement();
        }
    }
}
