using UnityEngine;

namespace CannonMonke
{
    public class HoldingLocomotionState : BaseState
    {
        public HoldingLocomotionState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter() 
        {
            //Debug.Log("On Enter Holding Locomotion State.");
            animator.CrossFade(HoldingLocomotionHash, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            player.HandleMovement();
        }
    }
}
