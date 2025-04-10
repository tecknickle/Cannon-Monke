using UnityEngine;
using System;

namespace CannonMonke
{
    public enum SoundType
    {
        Push,
        Landing,
        Jump,
        Throw,
        Pickup,
        Emote1,
        CannonFire,
        CannonLoad,
        CannonReset,
        CannonDryFire,
        ExplosiveOrdinance,
        SplatterOrdinance,
        CrateGenericOrdinance,
        RacoonOrdinance
        // Add more sound types as needed
    }

    [RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
    // Ensure an AudioSource component is present
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] SoundList[] soundList;
        public static SoundManager instance;
        AudioSource audioSource;

        void Awake()
        {
            instance = this;       
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        
        public static void PlaySound(SoundType sound, float volume = 0.5f)
        {
            AudioClip[] clips = instance.soundList[(int)sound].Sounds;
            AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
            instance.audioSource.PlayOneShot(randomClip, volume);
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            // Ensure the enum names are synchronized with the soundList array in the editor
            string[] names = Enum.GetNames(typeof(SoundType));
            Array.Resize(ref soundList, names.Length);
            for (int i = 0; i < soundList.Length; i++)
            {
                soundList[i].name = names[i];
            }
        }
#endif
    }

    [Serializable]
    // Serializable struct to hold sound clips for each sound type
    public struct SoundList
    {
        public AudioClip[] Sounds { get => sounds; }
        [HideInInspector] public string name;
        [SerializeField] AudioClip[] sounds;
    }

}

