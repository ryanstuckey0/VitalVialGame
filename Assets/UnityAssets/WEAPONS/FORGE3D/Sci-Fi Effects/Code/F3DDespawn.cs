using UnityEngine;
using ViralVial.Utilities;

namespace FORGE3D
{
    public class F3DDespawn : MonoBehaviour
    {

        public float DespawnDelay; // Despawn delay in ms

        AudioSource aSrc; // Cached audio source component

        void Awake()
        {
            // Get audio source component
            aSrc = GetComponent<AudioSource>();
        }

        // OnSpawned called by pool manager 
        public void OnSpawned()
        {
            Functions.CreateNewTimer(DespawnDelay, Despawn);
        }

        // OnDespawned called by pool manager 
        public void OnDespawned()
        {

        }

        // Despawn game object this script attached to
        public void Despawn()
        {
            if (this == null) return;
            F3DPoolManager.Pools["GeneratedPool"].Despawn(transform);
        }
    }
}