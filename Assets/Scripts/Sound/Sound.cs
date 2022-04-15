using UnityEngine;

namespace ViralVial.Sound
{
    [System.Serializable]
    public class Sound
    {

        [Tooltip("Sound will stop playing if this scene is loaded. Leave empty to not stop on a scene.")]
        public string SceneToStopOn;

        [Tooltip("Set to true to start sound on scene load. The scene this sound starts on will be it's Dictionary key value in AudioManager.cs.")]
        public bool StartOnSceneLoad;

        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume = 0.5f;

        [Range(-3, 3)]
        public float pitch;

        public bool loop;

        [HideInInspector]
        public AudioSource source;


        public void Play()
        {
            source.Play();
        }

        public void Stop()
        {
            source.Stop();
        }
    }
}
