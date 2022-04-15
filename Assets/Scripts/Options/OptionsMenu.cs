using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ViralVial.Options
{
    public class OptionsMenu : MonoBehaviour
    {
        Resolution[] resolutions;
        public TMP_Dropdown resolutionDropdown;
        int currentResolutionIndex;
        bool isFull = false;
        Resolution res;

        void Awake() 
        {
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();
            for (int i = 0; i < resolutions.Length; i++) 
            {
                string option =resolutions[i].width + "x" + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) 
                {
                    currentResolutionIndex = i;
                }

            }
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
            LoadResolutiuonIndexToDropDown();
            LoadScreenOptionToToggle();
        }

        public void SetFullScreen(bool isFullScreen) 
        {
            Screen.fullScreen = isFullScreen;
            isFull = isFullScreen;
            // ResolutionScreenSetting.instance.SaveDataToJson(res, isFull,currentResolutionIndex);
        }
        public void SetResolution(int resolutionIndex) 
        {
            currentResolutionIndex = resolutionIndex;
            res = resolutions[resolutionIndex];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
            // ResolutionScreenSetting.instance.SaveDataToJson(res, isFull, currentResolutionIndex);
        }

        /**
         * not sure if its work or not, neeed to check it after build game
         */
        public void LoadResolutiuonIndexToDropDown() 
        {
            int m_resolutionIndex = ResolutionScreenSetting.instance.LoadResIndexFromJson();
            resolutionDropdown.value = m_resolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }
        public void LoadScreenOptionToToggle() 
        {
            bool m_isFull = ResolutionScreenSetting.instance.LoadScreenDataFromJson();
            SetFullScreen(m_isFull);
        }
    }
}
