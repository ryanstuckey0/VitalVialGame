using System;
using UnityEngine;

namespace ViralVial.Sound
{
    [Serializable]
    public class AudioClipHolder
    {
        /// <summary>
        /// Audio clip held in this.
        /// </summary>
        public AudioClip AudioClip;

        /// <summary>
        /// Volume of audio clip. Can be adjusted to fine tune volume of individual sounds.
        /// </summary>
        [Range(0, 1)]
        public float Volume = 0.5f;

        [HideInInspector]
        public AudioSource AudioSource;

        /// <summary>
        /// Play this audio clip.
        /// </summary>
        /// <param name="audioSource">audio source to play from; calls PlayOneShot</param>
        /// <param name="volumeScale">volume to scale this by; useful for scaling all audio in a group by volume (i.e., all sound effects)</param>
        public void Play(AudioSource audioSource)
        {
            audioSource.PlayOneShot(AudioClip, Volume);
        }

        /// <summary>
        /// Play this audio clip with the assigned audio source.
        /// </summary>
        public void Play()
        {
            AudioSource.PlayOneShot(AudioClip, Volume);
        }
    }
}