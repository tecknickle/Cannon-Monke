using UnityEngine;

namespace CannonMonke
{
    public class HoldingThrowState : BaseState
    {
        public HoldingThrowState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            Debug.Log("On Enter Holding Throw State.");
            animator.CrossFade(HoldingThrowHash, shortCrossFadeDuration);
        }
    }
}
