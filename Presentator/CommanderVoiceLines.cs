using UnityEngine;

namespace Yuha
{
    /// <summary>
    /// Manages the commander to play specific voice lines.
    /// </summary>
    public class CommanderVoiceLines : MonoBehaviour
    {
        [Header("VOICE LINES")] public AudioClip[] commanderClips;

        public void PlayVoiceLine(int index) => AudioSource.PlayClipAtPoint(commanderClips[index], transform.position);
    }
}