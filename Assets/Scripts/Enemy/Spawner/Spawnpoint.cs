using UnityEngine;

namespace ViralVial.Enemy.Spawner
{
    public class Spawnpoint : MonoBehaviour
    {
        void Start()
        {
            FindObjectOfType<WaveSpawner>()?.AddSpawnpoint(transform.position);
            GetComponent<Renderer>().enabled = false;
        }
    }
}
