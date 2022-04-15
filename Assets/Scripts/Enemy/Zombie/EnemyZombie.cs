using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ViralVial.Weapons;
using ViralVial.Ability.Supernatural.TimeFreeze;
using ViralVial.Ability.Supernatural.MindControl;
using ViralVial.Ability.Supernatural.ShockWave;
using ViralVial.Utilities;
using ViralVial.Leaderboard;
using ViralVial.Sound;
using ViralVial.Player.Pickups;

namespace ViralVial.Enemy.Zombie
{
    public class EnemyZombie : MonoBehaviour, IEnemy, IDamageable, ITimeFreezable, IMindControllable, IShockWaveable
    {
        //public references EnemyZombieBrute
        public NavMeshAgent agent;
        public AnimationInstancing.AnimationInstancing animationInstancing;
        public GameObjectAudioController AudioController;
        public EnemyType EnemyType;
        public float DelayUntilDamage = 0.4f;

        //Starting stats for zombies
        [SerializeField] private float health = 100;
        [SerializeField] private float speed = 3;
        [SerializeField] private float damage = 10;
        [SerializeField] private float experience = 75;

        public float Health { get => health; }

        public float Speed { get => speed; }

        public float Damage { get => damage; }

        public float Experience { get => experience; }

        public bool IsDead { get => currentEnemyState == EnemyState.Death; }

        private CoroutineRunner currentStateCoroutine;
        private WaitForSeconds attackAnimationTimer;
        private WaitForSeconds idleAnimationTimer;
        private WaitForSeconds runAnimationTimer;
        private WaitForSeconds walkAnimationTimer;
        private WaitForSeconds damageCoroutineTimer;


        //private variables 
        private EnemyState currentEnemyState
        {
            get { return currentEnemyState_backingField; }
            set
            {
                if (value == currentEnemyState) return;
                currentEnemyState_backingField = value;
                switch (value)
                {
                    case EnemyState.Death:
                        currentStateCoroutine.Restart(DeathStateCoroutine());
                        break;
                    case EnemyState.Walking:
                        currentStateCoroutine.Restart(WalkStateCoroutine());
                        break;
                    case EnemyState.Attack:
                        currentStateCoroutine.Restart(AttackStateCoroutine());
                        break;
                    case EnemyState.Running:
                        currentStateCoroutine.Restart(RunStateCoroutine());
                        break;
                    case EnemyState.Idle:
                        currentStateCoroutine.Restart(IdleStateCoroutine());
                        break;
                }
            }
        }
        private EnemyState currentEnemyState_backingField;
        private float timerSearch;
        private bool targetFound;
        private bool mindControlled = false;
        private bool attack_CR = false;
        private GameObject target = null;
        private bool timeIsFrozen = false;
        private Dictionary<EnemyState, float> animationTimes;

        // Start is called before the first frame update
        void Start()
        {
            animationTimes = new Dictionary<EnemyState, float>(); // TODO: remove me
            foreach (var enemyState in ZombieAnimationHelper.AnimationIndexDictionary[EnemyType])
                animationTimes.Add(enemyState.Key, (float)animationInstancing.aniInfo[enemyState.Value].totalFrame / animationInstancing.aniInfo[enemyState.Value].fps);

            currentStateCoroutine = new CoroutineRunner(this);

            attackAnimationTimer = new WaitForSeconds((float)animationInstancing.aniInfo[(int)EnemyState.Attack].totalFrame / animationInstancing.aniInfo[(int)EnemyState.Attack].fps - DelayUntilDamage);
            idleAnimationTimer = new WaitForSeconds((float)animationInstancing.aniInfo[(int)EnemyState.Idle].totalFrame / animationInstancing.aniInfo[(int)EnemyState.Idle].fps);
            runAnimationTimer = new WaitForSeconds((float)animationInstancing.aniInfo[(int)EnemyState.Running].totalFrame / animationInstancing.aniInfo[(int)EnemyState.Running].fps);
            walkAnimationTimer = new WaitForSeconds((float)animationInstancing.aniInfo[(int)EnemyState.Walking].totalFrame / animationInstancing.aniInfo[(int)EnemyState.Walking].fps);
            damageCoroutineTimer = new WaitForSeconds(DelayUntilDamage);

            //starting enemy state is idle
            currentEnemyState = EnemyState.Idle;

            //TODO: change stats based on difficulty of tile, x,y = difficulty
            //target is player and false
            agent.speed = speed;
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
            if (timeIsFrozen || IsDead) return;
            //UpdateState();
            if (target != null && agent.destination != target.transform.position)
            {
                agent.SetDestination(target.transform.position);
            }

            UpdateState();
        }

        private bool WithinRange()
        {
            return Vector3.Distance(target.transform.position, gameObject.transform.position) <= 2.0f;
        }

        //attacking, idle, walking, scream, death, running
        // 0, 1, 2, 3, 4, 5 <- 6 animations total
        private void UpdateState()
        {
            //if health zero, currentEnemyState = EnemyState.Death
            //return
            if (health <= 0)
            {
                currentEnemyState = EnemyState.Death;
            }

            // if within attacking range, attack
            else if (target != null && agent.remainingDistance < 2)
            {
                currentEnemyState = EnemyState.Attack;
                Vector3 dir = target.transform.position - transform.position;
                dir.y = 0;
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, agent.angularSpeed * Time.deltaTime);
            }

            //if target != null, running
            else if (target != null)
            {
                currentEnemyState = EnemyState.Running;
            }

            timerSearch -= Time.deltaTime;
        }

        private void RemoveBoxCollider()
        {
            Destroy(GetComponent<BoxCollider>());
        }

        //reset the timer for walking and wandering
        private void ResetTimer()
        {
            timerSearch = Random.Range(10.0f, 30.0f);
        }

        //Mind Control functions
        float IMindControllable.Health
        {
            get { return health; }
            set { health = value; }
        }

        float IMindControllable.Damage
        {
            get { return damage; }
            set { damage = value; }
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

        public void ForceDeath() { currentEnemyState = EnemyState.Death; } // TODO: change to use death state instead

        //Freeze time functions
        public void Freeze()
        {
            animationInstancing.Pause();
            timeIsFrozen = true;
            if (agent != null) agent.isStopped = true;
        }

        public void Unfreeze()
        {
            animationInstancing.Resume();
            timeIsFrozen = false;
            if (agent != null) agent.isStopped = false;
        }

        //Shockwave functions
        public void PauseMovementBeforeShockWave()
        {
            if (agent != null) agent.isStopped = true;
        }

        public void ResumeMovementAfterShockWave()
        {
            if (agent != null) agent.isStopped = false;
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
                    hit.transform.gameObject.GetComponent<HitBox>()?.OnWeaponHit(damage);
                }
            }
            else
            {
                bool raycastHit = Physics.BoxCast(this.transform.position, Vector3.one / 2, this.transform.forward, out hit, Quaternion.identity, 2, LayerMask.GetMask(Constants.EnemyLayerName));
                //foreach (var hit in raycastHits)
                if (raycastHit)
                {
                    hit.transform.gameObject.GetComponent<HitBox>()?.OnWeaponHit(damage);
                }
            }
        }

        //function to make enemy take damage
        public void TakeDamage(float damage)
        {
            health -= damage;
            AudioController.PlayAudio("HitSound01");
            if (health <= 0) currentEnemyState = EnemyState.Death;
        }

        //waits until animation has finished before transitioning
        IEnumerator DeathStateCoroutine()
        {
            animationInstancing.PlayAnimation(GetAnimationIndex(EnemyType, EnemyState.Death));
            Destroy(agent);
            //remove enemy box collider and invoke exp event
            LeaderboardStats.Instance.AddToKillCount(1);
            EventManager.Instance.InvokeEvent("AddExp", new Dictionary<string, object> { { "experience", experience } });
            AudioController.PlayAudio("DeathSound01");
            RemoveBoxCollider();
            GlobalPickupSpawner.SpawnPickup(transform.position + new Vector3(0, 1, 0));
            yield return new WaitForSeconds(animationTimes[EnemyState.Death]);
            Destroy(gameObject);
        }

        IEnumerator AttackStateCoroutine()
        {
            while (true)
            {
                animationInstancing.PlayAnimation(GetAnimationIndex(EnemyType, EnemyState.Attack));
                yield return damageCoroutineTimer;
                agent.isStopped = false;
                if (WithinRange())
                    InflictDamage();
                yield return attackAnimationTimer;
            }
        }

        private IEnumerator DamageCoroutine()
        {
            yield return new WaitForSeconds(DelayUntilDamage);

        }

        private IEnumerator RunStateCoroutine()
        {
            while (true)
            {
                animationInstancing.PlayAnimation(GetAnimationIndex(EnemyType, EnemyState.Running));
                yield return runAnimationTimer;
            }
        }

        private IEnumerator WalkStateCoroutine()
        {
            while (true)
            {
                animationInstancing.PlayAnimation(GetAnimationIndex(EnemyType, EnemyState.Walking));
                yield return walkAnimationTimer;
            }
        }

        private IEnumerator IdleStateCoroutine()
        {
            while (true)
            {
                animationInstancing.PlayAnimation(GetAnimationIndex(EnemyType, EnemyState.Idle));
                yield return idleAnimationTimer;
            }
        }

        private int GetAnimationIndex(EnemyType enemyType, EnemyState enemyState)
        {
            return ZombieAnimationHelper.AnimationIndexDictionary[enemyType][enemyState];
        }
    }
}
