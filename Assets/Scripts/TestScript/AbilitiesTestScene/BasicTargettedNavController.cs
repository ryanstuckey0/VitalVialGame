using UnityEngine.AI;
using UnityEngine;
using ViralVial.Ability.Supernatural.MindControl;
using ViralVial.Ability.Supernatural.ShockWave;
using ViralVial.Ability.Supernatural.TimeFreeze;

namespace ViralVial.TestScript.AbilitiesTestScene
{
    /// <summary>
    /// Basic controller for entity that uses a NavMesh to pursue target. Is affected by all player abilities.
    /// </summary>
    public class BasicTargettedNavController : MonoBehaviour, ITimeFreezable, IMindControllable, IShockWaveable
    {
        public NavMeshAgent agent;
        float IMindControllable.Health {get; set;}

        float IMindControllable.Damage { get; set; }

        [SerializeField]
        private GameObject target;
        public GameObject Target
        {
            get { return target; }
            set
            {
                target = value;
                if (agent != null) agent.isStopped = timeIsFrozen || target == null;
            }
        }

        [SerializeField] private float stopFollowingRange = 4f;
        private bool timeIsFrozen = false;

        private void Update()
        {
            if (timeIsFrozen) return;
            if (target != null)
            {
                agent.SetDestination(Target.transform.position);
                agent.isStopped = Vector3.Distance(transform.position, target.transform.position) < stopFollowingRange;
            }
        }

        public void StartMindControl() { }

        public void Freeze()
        {
            Debug.Log("time frozen");
            timeIsFrozen = true;
            agent.isStopped = true;
        }

        public void Unfreeze()
        {
            timeIsFrozen = false;
            agent.isStopped = false;
        }

        public void PauseMovementBeforeShockWave()
        {
            Freeze();
        }

        public void ResumeMovementAfterShockWave()
        {
            Unfreeze();
        }

        public void ForceDeath() { Destroy(gameObject); }

        public void StopMindControl()
        {
        }
    }
}
