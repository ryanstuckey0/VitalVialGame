using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using ViralVial.Utilities;

namespace ViralVial.Sound
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        [SerializeField] private AudioMixer audioMixer;
        public AudioMixerGroup MusicAudioMixerGroup;

        public SerializableDictionary<Sound> sounds;

        public static AudioMixer AudioMixer { get { return Instance.audioMixer; } }
        public static string AudioSettingsFilePath => $"{Application.persistentDataPath}/{Constants.AudioSaveFileName}";

        private HashSet<string> playingSounds = new HashSet<string>();

        // initialize
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            foreach (var pair in sounds)
            {
                Sound sound = pair.Value;
                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.outputAudioMixerGroup = MusicAudioMixerGroup;
                sound.source.clip = sound.clip;

                sound.source.volume = sound.volume;
                sound.source.pitch = sound.pitch;
                sound.source.loop = sound.loop;

                sound.source.ignoreListenerPause = true;
            }
        }

        private void Start()
        {
            InitializeAudioSettingsFromFile();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoadPlay;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoadPlay;
        }

        private void OnSceneLoadPlay(Scene scene, LoadSceneMode mode)
        {
            OnSceneChangeStop(scene);
            if (sounds.ContainsKey(scene.name) && sounds[scene.name].StartOnSceneLoad && !playingSounds.Contains(scene.name))
            {
                Sound sound = sounds[scene.name];
                sound.Play();
                playingSounds.Add(scene.name);
            }
        }

        private void OnSceneChangeStop(Scene newScene)
        {
            // not efficient, but we should only ever have one or two sounds playing at once
            // could also switch to be async, but I'm not sure how to do that
            List<string> soundsToRemove = new List<string>();
            foreach (var soundName in playingSounds)
            {
                if (sounds[soundName].SceneToStopOn == newScene.name)
                {
                    sounds[soundName].Stop();
                    soundsToRemove.Add(soundName);
                }
            }

            foreach (var soundName in soundsToRemove)
                playingSounds.Remove(soundName);
        }

        private static void InitializeAudioSettingsFromFile()
        {
            VolumeData volumeData = Functions.ReadFileAndDeserialize<VolumeData>(AudioSettingsFilePath);
            if (volumeData == null) return;
            InitializeAudioMixer(volumeData);
        }

        public static void SaveAudioSettingsToFile()
        {
            Functions.SerializeAndWriteToFile(GetAudioMixerStatus(), AudioSettingsFilePath);
        }

        public static void InitializeAudioMixer(VolumeData volumeData)
        {
            AudioMixer.SetFloat("MasterVolume", volumeData.masterVolume);
            AudioMixer.SetFloat("MusicVolume", volumeData.musicVolume);
            AudioMixer.SetFloat("SFXVolume", volumeData.sfxVolume);
            AudioMixer.SetFloat("MenuVolume", volumeData.menuVolume);
        }

        public static VolumeData GetAudioMixerStatus()
        {
            float tempFloat;
            VolumeData volumeData = new VolumeData();
            if (AudioMixer.GetFloat("MasterVolume", out tempFloat)) volumeData.masterVolume = tempFloat;
            if (AudioMixer.GetFloat("MusicVolume", out tempFloat)) volumeData.musicVolume = tempFloat;
            if (AudioMixer.GetFloat("SFXVolume", out tempFloat)) volumeData.sfxVolume = tempFloat;
            if (AudioMixer.GetFloat("MenuVolume", out tempFloat)) volumeData.menuVolume = tempFloat;
            return volumeData;
        }
    }
}
