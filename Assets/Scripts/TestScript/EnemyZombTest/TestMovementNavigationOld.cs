using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace ViralVial.Script.TestScript.EnemyZombTest
{
    public class TestMovementNavigationOld : MonoBehaviour
    {
        enum EnemyState
        {
            //idle, walk, scream, run, attack, death
            //5, 4, 0, 2, 1, 3
            Attack = 1,
            Idle = 5,
            Walking = 4,
            Scream = 0,
            Death = 3,
            Running = 2
        }
        EnemyState currentEnemyState = EnemyState.Idle;
        private int currentEnemyAnimationState = (int)EnemyState.Idle;
        public NavMeshAgent agent;
        public AnimationInstancing.AnimationInstancing animationInstancing;
        private GameObject target = null;
        private float timerSearch;
        private bool targetFound = false;
        public GameObject fov;
        private TestColliderDetection firstCollider;

        private void Start()
        {
            //target = player;
            agent.isStopped = true;
            targetFound = false;

            timerSearch = Random.Range(3.0f, 10.0f);
            firstCollider = fov.GetComponent<TestColliderDetection>();

            firstCollider.OnTriggerEnter_Action += firstCollider_OnTriggerEnter;
        }

        private void firstCollider_OnTriggerEnter(Collider other)
        {
            Debug.Log("Something detected: " + other.tag);
            if (other.tag == "Player")
            {
                target = other.gameObject;
                targetFound = true;
                agent.SetDestination(other.transform.position);
                agent.isStopped = false;
                firstCollider.OnTriggerEnter_Action -= firstCollider_OnTriggerEnter;
                //firstCollider.enabled = false;
            }

        }

        private void Update()
        {
            //UpdateState();
            if (target != null && agent.destination != target.transform.position)
            {
                agent.SetDestination(target.transform.position);
            }
            //TODO: check health of game object
            switch (currentEnemyState)
            {
                case EnemyState.Running:
                    animationInstancing.PlayAnimation((int)EnemyState.Running);
                    //checks if current animation finished, starts next
                    //StartCoroutine("AnimationCheck");
                    //time to live for chasing player
                    UpdateState();
                    break;
                case EnemyState.Walking:
                    animationInstancing.PlayAnimation((int)EnemyState.Walking);
                    //checks if current animation finished, starts next
                    //StartCoroutine("AnimationCheck");
                    //if timer for idling over, walk
                    UpdateState();
                    break;
                case EnemyState.Idle:
                    animationInstancing.PlayAnimation((int)EnemyState.Idle);
                    //checks if current animation finished, starts next
                    //StartCoroutine("AnimationCheck");
                    //if timer for walking over, idle
                    UpdateState();
                    break;
                case EnemyState.Attack:
                    animationInstancing.PlayAnimation((int)EnemyState.Attack);
                    //checks if current animation finished, starts next
                    //StartCoroutine("AnimationCheck");
                    //if done attacking, check if target null, else run/attack
                    UpdateState();
                    break;
                case EnemyState.Scream:
                    animationInstancing.PlayAnimation((int)EnemyState.Scream);
                    //checks if current animation finished, starts next
                    //StartCoroutine("AnimationCheck");
                    //if done screaming, running
                    UpdateState();
                    break;
                case EnemyState.Death:
                    animationInstancing.PlayAnimation((int)EnemyState.Death);
                    //checks if current animation finished, starts next
                    //StartCoroutine("AnimationCheck");
                    //if death animation over, delete gameobject
                    //TODO: delete game object
                    UpdateState();
                    break;
                default:
                    Debug.Log("Yo shit is fucked in this statemachine");
                    break;
            }

            //debuginn purposes to see state
            //Debug.Log(currentEnemyState);
        }

        //attacking, idle, walking, scream, death, running
        // 0, 1, 2, 3, 4, 5 <- 6 animations total
        private void UpdateState()
        {
            //if health zero, currentEnemyState = EnemyState.Death
            //return

            //if target in range, attack
            //return
            if (target != null && agent.remainingDistance < 2)
            {
                currentEnemyState = EnemyState.Attack;
                //agent.SetDestination(transform.position);
                return;
            }

            //if target found, scream
            //return
            if (target == null && targetFound)
            {
                currentEnemyState = EnemyState.Scream;
                //reset to false so scream state isn't updated again
                //targetFound = false;
                return;
            }

            //if target != null, running
            //return
            if (target != null)
            {
                currentEnemyState = EnemyState.Running;
                return;
            }

            //transition from walking to idle
            if (target == null && currentEnemyState == EnemyState.Walking)
            {
                var distance = Vector3.Distance(transform.position, agent.destination);
                if (timerSearch <= 0 || (distance < 1))
                {
                    ResetTimer();
                    currentEnemyState = EnemyState.Idle;

                    agent.isStopped = true;
                    return;
                }
            }

            //transition from idle to walking
            if (target == null && currentEnemyState == EnemyState.Idle)
            {
                if (timerSearch <= 0)
                {
                    ResetTimer();
                    currentEnemyState = EnemyState.Walking;

                    var walkingDest = Random.insideUnitSphere * 20;
                    var distance = Vector3.Distance(transform.position, walkingDest);
                    while (distance < 5)
                    {
                        walkingDest = Random.insideUnitSphere * 20;
                        distance = Vector3.Distance(transform.position, walkingDest);
                    }
                    agent.SetDestination(walkingDest);
                    agent.isStopped = false;
                    return;
                }
            }

            timerSearch -= Time.deltaTime;
            //Debug.Log(timerSearch + " " + agent.hasPath);
        }

        private void ResetTimer()
        {
            timerSearch = Random.Range(3.0f, 10.0f);
        }
    }
}