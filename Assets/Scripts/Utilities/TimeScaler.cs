using UnityEngine;

namespace ViralVial.Utilities
{
    public class TimeScaler : MonoBehaviour
    {
        [Range(0,1)]
        public float Timescale = 1.0f;

        // Update is called once per frame
        void Update()
        {
            Time.timeScale = Timescale;
        }
    }
}
