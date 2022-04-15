using UnityEngine.AI;
using UnityEngine;

namespace ViralVial
{
    public class EnemyNavController : MonoBehaviour
    {
        public NavMeshAgent agent;

        public GameObject player;

        void Update()
        {
            agent.SetDestination(player.transform.position);
        }
    }
}
