using UnityEngine;

namespace CannonMonke
{
    public class EmoteState : BaseState
    {
        public EmoteState(PlayerController player, Animator animator) : base(player, animator) { }

        public override void OnEnter()
        {
            Debug.Log("On Enter Emote State");
            animator.CrossFade(Emote1Hash, crossFadeDuration);
        }

        public override void FixedUpdate()
        {
            player.HandleJump();
            player.HandleMovement();
        }
    }
}
