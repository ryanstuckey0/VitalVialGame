using UnityEngine;
using UnityEngine.UI;
using ViralVial.Sound;

namespace ViralVial.Options
{
    public class VolumeController : MonoBehaviour
    {
        public Slider masterSlider;
        public Slider musicSlider;
        public Slider sfxSlider;
        public Slider menuSlider;

        public void Awake()
        {
            VolumeData volumeData = AudioManager.GetAudioMixerStatus();
            masterSlider.value = volumeData.masterVolume;
            musicSlider.value = volumeData.musicVolume;
            sfxSlider.value = volumeData.sfxVolume;
            menuSlider.value = volumeData.menuVolume;
        }

        public void SetMasterVolume()
        {
            AudioManager.AudioMixer.SetFloat("MasterVolume", masterSlider.value);
        }

        public void SetMusicVolume()
        {
            AudioManager.AudioMixer.SetFloat("MusicVolume", musicSlider.value);
        }

        public void SetSFXVolume()
        {
            AudioManager.AudioMixer.SetFloat("SFXVolume", sfxSlider.value);
        }

        public void SetMenuVolume()
        {
            AudioManager.AudioMixer.SetFloat("MenuVolume", menuSlider.value);
        }
    }
}
