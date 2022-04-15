using UnityEngine;
using ViralVial.TestScript.AbilitiesTestScene;

namespace ViralVial
{
    public class EnemyTestSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject EnemyPrefab;

        private void OnSpawnEnemy() {
            Instantiate(EnemyPrefab, transform.position + transform.forward * 5, Quaternion.identity).GetComponent<BasicTargettedNavController>().Target = gameObject;
        }
    }
}
