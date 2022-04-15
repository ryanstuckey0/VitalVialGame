using UnityEngine;
using ViralVial.Utilities;

namespace ViralVial.Video
{
    public class VideoManager : MonoBehaviour
    {
        public static VideoManager Instance;
        public static string VideoSettingsFilePath => $"{Application.persistentDataPath}/{Constants.VideoSaveFileName}";

        [Header("Default Settings")]
        public bool IsFullScreen;
        public bool UseNativeResolution;
        public SimpleResolution ResolutionToUse;

        private void Awake()
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
        }

        private void Start()
        {
            InitializeVideoSettingsFromFile();
        }

        private static void InitializeVideoSettingsFromFile()
        {
            VideoSettings videoSettings = Functions.ReadFileAndDeserialize<VideoSettings>(VideoSettingsFilePath);
            if (videoSettings == null)
            {
                videoSettings = new VideoSettings();
                videoSettings.IsFullScreen = Instance.IsFullScreen;
                videoSettings.Resolution = Instance.UseNativeResolution
                    ? new SimpleResolution
                    {
                        Width = Screen.width,
                        Height = Screen.height
                    }
                    : new SimpleResolution
                    {
                        Width = Instance.ResolutionToUse.Width,
                        Height = Instance.ResolutionToUse.Height
                    };
            }
            else InitializeVideoSettings(videoSettings);
        }

        public static void SaveVideoSettingsToFile()
        {
            Functions.SerializeAndWriteToFile(GetCurrentVideoStatus(), VideoSettingsFilePath);
        }

        private static void InitializeVideoSettings(VideoSettings videoSettings)
        {
            Screen.fullScreen = videoSettings.IsFullScreen;
            Screen.SetResolution(videoSettings.Resolution.Width, videoSettings.Resolution.Height, Screen.fullScreenMode);
        }

        public static VideoSettings GetCurrentVideoStatus()
        {
            return new VideoSettings
            {
                IsFullScreen = Screen.fullScreen,
                Resolution = new SimpleResolution
                {
                    Width = Screen.width,
                    Height = Screen.height
                }
            };
        }
    }
}