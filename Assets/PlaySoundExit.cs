using UnityEngine;

namespace CannonMonke
{
    public class PlaySoundExit : StateMachineBehaviour
    {
        [SerializeField] SoundType sound;
        [SerializeField, Range(0, 1)] float volume = 1f;

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Play a sound when entering this state
            if (SoundManager.instance != null)
            {
                SoundManager.PlaySound(sound, volume);
            }
            else
            {
                Debug.LogWarning("SoundManager instance is null.");
            }
        }
    }
}
