using UnityEngine;

namespace Yuha
{
    /// <summary>
    /// <c>CrowdAudio<c> Plays audio clips for crowd reactions and Game Over.
    /// </summary>
    public class CrowdAudio : MonoBehaviour
    {
        [SerializeField] private AudioClip _booClip;
        [SerializeField] private AudioClip _applauseClip;
        [SerializeField] private AudioClip _gameOverClip;
        private AudioSource _audioSource;

        private void Awake() => _audioSource = GetComponent<AudioSource>();

        private void OnEnable()
        {
            Building.OnBurnedDown += PlayBooClip;
            Building.OnExtinguished += PlayApplauseClip;
            PlayerHealth.OnGameOver += PlayGameOverClip;
        }
        
        private void OnDisable()
        {
            Building.OnBurnedDown -= PlayBooClip;
            Building.OnExtinguished -= PlayApplauseClip;
            PlayerHealth.OnGameOver -= PlayGameOverClip;
        }

        private void PlayBooClip(Building building) => _audioSource.PlayOneShot(_booClip);
        
        private void PlayApplauseClip() => _audioSource.PlayOneShot(_applauseClip);

        private void PlayGameOverClip()
        {
            Building.OnBurnedDown -= PlayBooClip;
            _audioSource.PlayOneShot(_gameOverClip);
        } 
    }
}