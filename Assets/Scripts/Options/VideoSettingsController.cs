using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViralVial.Video;
using ViralVial.Utilities;

namespace ViralVial.Options
{
    public class VideoSettingsController : MonoBehaviour
    {
        public Toggle FullScreenToggle;
        public Dropdown ResolutionDropdown;

        private List<SimpleResolution> simpleResolutions;

        public void Awake()
        {
            List<string> resolutionDropdownOptions = new List<string>();
            simpleResolutions = new List<SimpleResolution>();

            foreach (var res in Screen.resolutions)
            {
                if (resolutionDropdownOptions.Contains(res.ToResolutionString())) continue;
                string resolutionString = res.ToResolutionString();
                resolutionDropdownOptions.Insert(0, resolutionString);
                simpleResolutions.Insert(0, new SimpleResolution(res));
            }

            ResolutionDropdown.AddOptions(resolutionDropdownOptions);

            VideoSettings videoSettings = VideoManager.GetCurrentVideoStatus();
            FullScreenToggle.isOn = videoSettings.IsFullScreen;
            ResolutionDropdown.value = resolutionDropdownOptions.IndexOf(videoSettings.Resolution.ToString());
        }

        public void OnFullscreenToggleChange(bool isFullScreen)
        {
            Screen.fullScreen = isFullScreen;
        }

        public void OnResolutionDropdownChange(int newValue)
        {
            Screen.SetResolution(simpleResolutions[newValue].Width, simpleResolutions[newValue].Height, Screen.fullScreenMode);
        }
    }
}