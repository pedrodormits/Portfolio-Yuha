using UnityEngine;

namespace Yuha
{
    /// <summary>
    /// <c>Building</c> Establishes when an audio clip should play, except for the crowd audio clips.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        [HideInInspector] public AudioSource AudioSource;

        private void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
            Instance = this;
        } 
        
        /// <summary>
        /// <c>PlayAudioClip</c> Plays an audio clip only once.
        /// </summary>
        public void PlayAudioClip(AudioClip clip) => AudioSource.PlayOneShot(clip);

        /// <summary>
        /// <c>PlayAudioClip</c> Plays an audio clip that should loop.
        /// </summary>
        public void PlayAudioClipLoop(AudioClip clip, bool loop)
        {
            AudioSource.clip = clip;
            AudioSource.loop = loop;
            AudioSource.Play();
        }

        /// <summary>
        /// <c>StopAudioClip</c> Stop audio clips that are looping.
        /// </summary>
        public void StopAudioClip() => AudioSource.Stop();
    }
}