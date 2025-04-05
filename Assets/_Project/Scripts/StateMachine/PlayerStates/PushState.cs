using UnityEngine;

namespace CannonMonke
{
    public class PushState : BaseState
    {
        public PushState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            //Debug.Log("On Enter Push State");
            animator.CrossFade(PushHash, shortCrossFadeDuration);
            player.Push();
        }
    }
}
