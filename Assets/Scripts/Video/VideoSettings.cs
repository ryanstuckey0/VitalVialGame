using UnityEngine;

namespace ViralVial.Video
{
    [System.Serializable]
    public class VideoSettings
    {
        public bool IsFullScreen;
        public SimpleResolution Resolution;
    }

    [System.Serializable]
    public class SimpleResolution
    {
        public int Width;
        public int Height;

        public SimpleResolution() { }

        public SimpleResolution(Resolution resolution)
        {
            Width = resolution.width;
            Height = resolution.height;
        }

        public override string ToString()
        {
            return $"{Width}x{Height}";
        }
    }
}