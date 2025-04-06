using UnityEngine;

namespace CannonMonke
{
    public class CannonModeState : BaseState
    {
        public CannonModeState(PlayerController player, Animator animator) : base(player, animator) { } 

        public override void OnEnter()
        {
            Debug.Log("On Enter Cannon Mode State");
            // placeholder animation
            animator.CrossFade(LocomotionHash, shortCrossFadeDuration);
        }

        public override void Update()
        {
            // no op
            // In cannon mode, we might want to handle aiming or other logic here
            // for animations especially linked to input
        }

        public override void OnExit()
        {
            Debug.Log("On Exit Cannon Mode State");
        }
    }
}
