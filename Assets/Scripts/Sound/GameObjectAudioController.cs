using UnityEngine;
using ViralVial.Utilities;

namespace ViralVial.Sound
{
    /// <summary>
    /// Useful to add a collection of audio clips to a single GameObject using only one audio source. 
    /// Includes a serializable dictionary so it can be configured in the editor.
    /// </summary>
    public class GameObjectAudioController : MonoBehaviour
    {
        public AudioSource PlayerAudioSource;
        public bool IgnoreAudioListenerPause = false;
        public SerializableDictionary<AudioClipHolder> AudioDictionary;

        private void Awake()
        {
            PlayerAudioSource.ignoreListenerPause = IgnoreAudioListenerPause;
            foreach (var element in AudioDictionary) element.Value.AudioSource = PlayerAudioSource;
        }

        /// <summary>
        /// Plays an audio that has been loaded into the dictionary. Does not check if dictionary 
        /// contains key for performance,  so will throw an exception if audioName is not in 
        /// dictionary.
        /// </summary>
        /// <param name="audioName">name of audio in dictionary</param>
        public void PlayAudio(string audioName)
        {
            AudioDictionary[audioName].Play();
        }

        /// <summary>
        /// Plays an audio that has been loaded into the dictionary using the specified audio source. 
        /// Does not check if dictionary contains key for performance, so will throw an exception 
        /// if audioName is not in dictionary.
        /// </summary>
        /// <param name="audioName">name of audio in dictionary</param>
        /// <param name="audioSource">audio source to play from</param>
        public void PlayAudio(string audioName, AudioSource audioSource)
        {
            AudioDictionary[audioName].Play(audioSource);
        }
    }
}