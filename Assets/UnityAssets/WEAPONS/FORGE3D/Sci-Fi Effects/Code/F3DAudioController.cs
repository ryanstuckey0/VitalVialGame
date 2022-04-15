using UnityEngine;
using System.Collections;

namespace FORGE3D
{
    public class F3DAudioController : MonoBehaviour
    {
        // Singleton instance 
        public static F3DAudioController instance;

        // Audio timers
        float timer_01, timer_02;
        public Transform audioSource;
        public AudioClip vulcanShot; // Shot prefab
        public float vulcanDelay; // Shot delay in ms
        public float vulcanHitDelay; // Hit delay in ms

        [Range(0, 1)]
        public float audioVolume;

        void Awake()
        {
            // Initialize singleton
            instance = this;
        }

        void Update()
        {
            // Update timers
            timer_01 += Time.deltaTime;
            timer_02 += Time.deltaTime;
        }

        // Play vulcan shot audio at specific position
        public void VulcanShot(Vector3 pos)
        {
            // Audio source can only be played once for each vulcanDelay
            if (timer_01 >= vulcanDelay)
            {
                // Spawn audio source prefab from pool
                AudioSource aSrc =
                    F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(audioSource, vulcanShot, pos, null)
                        .gameObject.GetComponent<AudioSource>();

                if (aSrc != null)
                {
                    // Modify audio source settings specific to it's type
                    aSrc.pitch = Random.Range(0.95f, 1f);
                    aSrc.volume = Random.Range(0.8f, 1f) * audioVolume;
                    aSrc.minDistance = 5f;
                    aSrc.loop = false;
                    aSrc.Play();

                    // Reset delay timer
                    timer_01 = 0f;
                }
            }
        }
    }
}