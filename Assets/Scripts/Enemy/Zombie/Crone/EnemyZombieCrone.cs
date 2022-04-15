using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using ViralVial.Weapons;
using ViralVial.Ability.Supernatural.TimeFreeze;
using ViralVial.Ability.Supernatural.MindControl;
using ViralVial.Ability.Supernatural.ShockWave;
using System.Collections.Generic;
using ViralVial.Utilities;
using ViralVial.Leaderboard;
using ViralVial.Sound;
using ViralVial.Player.Pickups;

namespace ViralVial.Enemy.Zombie.Crone
{
    public class EnemyZombieCrone : MonoBehaviour, IEnemy, IDamageable, ITimeFreezable, IMindControllable, IShockWaveable
    {
        //public references EnemyZombieBrute
        public NavMeshAgent agent;
        public AnimationInstancing.AnimationInstancing animationInstancing;
        public GameObjectAudioController AudioController;
        [SerializeField] private float timeBetweenAttacks = 2;

        //Starting stats for zombies
        public float Health = 50;
        private float Speed = 4;
        private float Damage = 20;
        private float Experience = 200;

        float IEnemy.Health { get; }

        float IEnemy.Speed { get; }

        float IEnemy.Damage { get; }

        float IEnemy.Experience { get; }
        public bool IsDead { get => currentEnemyState == EnemyState.Death; }

        //animation states
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

        //private variables 
        private EnemyState currentEnemyState
        {
            get { return currentEnemyState_backingField; }
            set
            {
                if (value == currentEnemyState) return;
                currentEnemyState_backingField = value;
                //if (value == EnemyState.Attack) StartCoroutine(AttackCoroutine());
            }
        }
        private EnemyState currentEnemyState_backingField;
        private float timerSearch;
        private bool targetFound;
        private bool mindControlled = false;
        private bool attack_CR = false;
        private int attack_CT = 0;
        private GameObject target = null;
        private PlayerColliderDetection detectionCollider;
        private bool timeIsFrozen = false;

        // Start is called before the first frame update
        void Start()
        {
            //starting enemy state is idle
            currentEnemyState = EnemyState.Idle;

            //TODO: change stats based on difficulty of tile, x,y = difficulty
            //target is player and false
            agent.speed = Speed;
            agent.isStopped = false;
            targetFound = true;

            //timerSearch = Random.Range(10.0f, 30.0f);
            //detectionCollider = fov.GetComponent<PlayerColliderDetection>();
            //detectionCollider.OnTriggerEnter_Action += detectionCollider_OnTriggerEnter;
        }

        private void detectionCollider_OnTriggerEnter(Collider other)
        {
            ////Player Detected Collider
            //if (other.tag == "Player")
            //{
            //    target = other.gameObject;
            //    targetFound = true;
            //    agent.SetDestination(other.transform.position);
            //    agent.isStopped = false;
            //    detectionCollider.OnTriggerEnter_Action -= detectionCollider_OnTriggerEnter;
            //    detectionCollider.enabled = false;
            //    Destroy(detectionCollider.gameObject);
            //}
        }

        // Update is called once per frame
        void Update()
        {
            if (timeIsFrozen) return;
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
                    if (!attack_CR)
                    {
                        agent.isStopped = true;
                        StartCoroutine("AnimationAttack");
                    }
                    //animationInstancing.PlayAnimation((int)EnemyState.Attack);
                    //checks if current animation finished, starts next
                    //StartCoroutine("AnimationCheck");
                    //if done attacking, check if target null, else run/attack
                    //UpdateState();
                    break;
                case EnemyState.Scream:
                    StartCoroutine("AnimationScream");
                    //animationInstancing.PlayAnimation((int)EnemyState.Scream);
                    //checks if current animation finished, starts next
                    //StartCoroutine("AnimationCheck");
                    //if done screaming, running
                    //UpdateState();
                    break;
                case EnemyState.Death:
                    StartCoroutine("AnimationCheckAction");
                    //animationInstancing.PlayAnimation((int)EnemyState.Death);
                    //checks if current animation finished, starts next
                    //StartCoroutine("AnimationCheck");
                    //if death animation over, delete gameobject
                    //TODO: delete game object
                    //UpdateState();
                    break;
                default:
                    Debug.Log("Yo shit is fucked in this statemachine");
                    break;
            }

            //debuginn purposes to see state
            //Debug.Log(currentEnemyState);
        }

        private bool WithinRange()
        {
            return Vector3.Distance(target.transform.position, gameObject.transform.position) <= 2.0f;
        }

        private void UpdateState()
        {
            //if health zero, currentEnemyState = EnemyState.Death
            //return
            if (Health <= 0)
            {
                agent.isStopped = true;
                currentEnemyState = EnemyState.Death;
                //remove enemy box collider and invoke exp event
                LeaderboardStats.Instance.AddToKillCount(1);
                EventManager.Instance.InvokeEvent("AddExp", new Dictionary<string, object> { { "experience", Experience } });
                AudioController.PlayAudio("DeathSound01");
                RemoveBoxCollider();
                GlobalPickupSpawner.SpawnPickup(transform.position + new Vector3(0, 1, 0));
                return;
            }

            //if target in range, attack
            //return
            if (target != null && agent.remainingDistance < 2 && WithinRange())
            {
                currentEnemyState = EnemyState.Attack;
                Vector3 dir = target.transform.position - transform.position;
                dir.y = 0;
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, agent.angularSpeed * Time.deltaTime);
                return;
            }

            //if target found, scream
            //return
            if (target == null && targetFound)
            {
                currentEnemyState = EnemyState.Scream;
                //reset to false so scream state isn't updated again
                targetFound = false;
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
                if (timerSearch <= 0 || (distance <= 2))
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

        private void RemoveBoxCollider()
        {
            this.GetComponent<BoxCollider>().enabled = false;
        }

        //reset the timer for walking and wandering
        private void ResetTimer()
        {
            timerSearch = Random.Range(10.0f, 30.0f);
        }

        //Mind Control functions
        float IMindControllable.Health
        {
            get { return Health; }
            set { Health = value; }
        }

        float IMindControllable.Damage
        {
            get { return Damage; }
            set { Damage = value; }
        }
        public GameObject Target
        {
            get { return target; }
            set
            {
                target = value;
                if (agent != null && agent.isOnNavMesh)
                {
                    agent.isStopped = timeIsFrozen || target == null;
                }
            }
        }

        public void ForceDeath() { Destroy(gameObject); } // TODO: change to use death state instead

        //Freeze time functions
        public void Freeze()
        {
            animationInstancing.Pause();
            timeIsFrozen = true;
            agent.isStopped = true;
        }

        public void Unfreeze()
        {
            animationInstancing.Resume();
            timeIsFrozen = false;
            agent.isStopped = false;
        }

        //Shockwave functions
        public void PauseMovementBeforeShockWave()
        {
            agent.isStopped = true;
        }

        public void ResumeMovementAfterShockWave()
        {
            agent.isStopped = false;
        }

        // Mind control function
        public void StartMindControl()
        {
            mindControlled = true;
        }

        public void StopMindControl()
        {
            mindControlled = false;
        }

        //logic for zombie attack state
        private void AttackState()
        {
            animationInstancing.PlayAnimation((int)EnemyState.Attack);
        }

        //logic for zombie idle state
        private void IdleState()
        {
            animationInstancing.PlayAnimation((int)EnemyState.Idle);
        }

        //logic for zombie walk state
        private void WalkState()
        {
            animationInstancing.PlayAnimation((int)EnemyState.Walking);
        }

        //logic for zombie scream state
        private void ScreamState()
        {
            animationInstancing.PlayAnimation((int)EnemyState.Scream);
        }

        //logic for zombie death state
        private void DeathState()
        {
            animationInstancing.PlayAnimation((int)EnemyState.Death);
        }

        //logic for zombie run state
        private void RunState()
        {
            animationInstancing.PlayAnimation((int)EnemyState.Running);
        }

        //function for enemy to inflict damage (they are an enemy)
        public void InflictDamage()
        {
            RaycastHit hit;
            if (!mindControlled)
            {
                bool raycastHit = Physics.BoxCast(this.transform.position, Vector3.one / 2, this.transform.forward, out hit, Quaternion.identity, 2, LayerMask.GetMask(Constants.PlayerLayerName));
                //foreach (var hit in raycastHits)
                if (raycastHit)
                {
                    hit.transform.gameObject.GetComponent<HitBox>()?.OnWeaponHit(Damage);
                }
            }
            else
            {
                bool raycastHit = Physics.BoxCast(this.transform.position, Vector3.one / 2, this.transform.forward, out hit, Quaternion.identity, 2, LayerMask.GetMask(Constants.EnemyLayerName));
                //foreach (var hit in raycastHits)
                if (raycastHit)
                {
                    hit.transform.gameObject.GetComponent<HitBox>()?.OnWeaponHit(Damage);
                }
            }
        }

        //function to make enemy take damage
        public void TakeDamage(float damage)
        {
            Health -= damage;
            AudioController.PlayAudio("HitSound01");
            //if health less than zero play death state
            //if (Health <= 0) DeathState();
        }

        //waits until animation has finished before transitioning
        IEnumerator AnimationCheckAction()
        {
            animationInstancing.PlayAnimation((int)EnemyState.Death);
            bool isPlaying = true;
            while (isPlaying)
            {
                isPlaying = animationInstancing.IsDone();
                yield return new WaitForSeconds(.1f);
            }
            Destroy(gameObject);
        }

        IEnumerator AnimationAttack()
        {
            Debug.Log("attack co routine started");
            attack_CR = true;
            animationInstancing.PlayAnimation((int)EnemyState.Attack);
            bool isPlaying = true;
            while (isPlaying)
            {
                isPlaying = animationInstancing.IsDone();
                yield return new WaitForSeconds(.2f);
            }
            attack_CR = false;
            agent.isStopped = false;
            if (WithinRange())
            {
                InflictDamage();
            }
            UpdateState();
        }

        IEnumerator AnimationScream()
        {
            animationInstancing.PlayAnimation((int)EnemyState.Scream);
            bool isPlaying = true;
            while (isPlaying)
            {
                isPlaying = animationInstancing.IsDone();
                yield return new WaitForSeconds(.2f);
            }
            UpdateState();
        }

        private IEnumerator AttackCoroutine()
        {
            while (currentEnemyState == EnemyState.Attack)
            {
                InflictDamage();
                yield return new WaitForSeconds(timeBetweenAttacks);
            }
        }
    }
}
