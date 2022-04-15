using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViralVial.Ability.Supernatural.MindControl;
using ViralVial.Ability.Supernatural.ShockWave;
using ViralVial.Ability.Supernatural.TimeFreeze;
using ViralVial.Weapons;
using ViralVial.Utilities;

namespace ViralVial.Ability
{
    public class AbilitiesReactionController : MonoBehaviour
    {
        [Header("Mind Control")]
        public bool MindControl;
        [SerializeField] private GameObject mindControlAnimation;
        [Tooltip("Script must inherit IMindControllable.")]
        [SerializeField] private MonoBehaviour IMindControllableScript;
        private IMindControllable IMindControllableScriptCast;

        [Header("Time Freeze")]
        public bool TimeFreeze;
        [Tooltip("Script must inherit ITimeFreezeable.")]
        [SerializeField] private MonoBehaviour ITimeFreezeableScript;
        private ITimeFreezable ITimeFreezeableScriptCast;

        [Header("Shock Wave")]
        public bool ShockWave;
        [SerializeField] private MonoBehaviour IShockWaveableScript;
        private IShockWaveable IShockWaveableScriptCast;

        [Header("Elemental Attack")]
        public bool ElementalAttack;
        [SerializeField] private GameObject fireAttackBurningAnimation;

        [Tooltip("Script must inherit IDamageable.")]
        [SerializeField] private MonoBehaviour IDamageableScript;
        private IDamageable IDamageableScriptCast;

        private void Start()
        {
            IDamageableScriptCast = (IDamageable)IDamageableScript;

            if (MindControl) InitMindControl();
            if (TimeFreeze) InitTimeFreeze();
            if (ShockWave) InitShockWave();
            if (ElementalAttack) InitElementalAttack();

            EventManager.Instance.SubscribeToEvent("StartAlarmClock", OnStartAlarmClock);
            EventManager.Instance.SubscribeToEvent("EndAlarmClock", OnEndAlarmClock);
        }

        private void OnDestroy()
        {
            if (MindControl) OnDestroyMindControl();
            if (TimeFreeze) OnDestroyTimeFreeze();
            if (ShockWave) OnDestroyShockWave();
            if (ElementalAttack) OnDestroyElementalAttack();

            EventManager.Instance.UnsubscribeFromEvent("StartAlarmClock", OnStartAlarmClock);
            EventManager.Instance.UnsubscribeFromEvent("EndAlarmClock", OnEndAlarmClock);
        }

        // Alarm Clock --------------------------------------------------------

        private GameObject savedTarget;

        private void OnStartAlarmClock(Dictionary<string, object> args)
        {
            savedTarget = IMindControllableScriptCast.Target;
            IMindControllableScriptCast.Target = (GameObject)args["target"];
        }

        private void OnEndAlarmClock(Dictionary<string, object> args)
        {
            IMindControllableScriptCast.Target = (GameObject)args["target"];
        }


        // Shock Wave ---------------------------------------------------------

        private CoroutineRunner shockWaveCoroutine;

        private void InitShockWave()
        {
            IShockWaveableScriptCast = (IShockWaveable)IShockWaveableScript;
            shockWaveCoroutine = new CoroutineRunner(this);
        }

        private void OnDestroyShockWave() { }

        public void ApplyShockWave(Vector3 origin, float range, float speed)
        {
            if (!ShockWave) return;
            float distanceToMove = (range - Vector3.Distance(transform.position, origin));
            Vector3 direction = (transform.position - origin).normalized;
            shockWaveCoroutine.Restart(ShockWaveMoverCoroutine(distanceToMove, direction, speed));
        }

        private IEnumerator ShockWaveMoverCoroutine(float distanceToMove, Vector3 direction, float speed)
        {
            float distanceMoved = 0f;
            IShockWaveableScriptCast.PauseMovementBeforeShockWave();
            while (distanceMoved < distanceToMove)
            {
                float distanceToMoveInFrame = speed * Time.deltaTime;
                transform.position += direction * distanceToMoveInFrame;
                distanceMoved += distanceToMoveInFrame;
                yield return null;
            }
            IShockWaveableScriptCast.ResumeMovementAfterShockWave();
        }

        // Time Freeze --------------------------------------------------------

        private void InitTimeFreeze()
        {
            EventManager.Instance.SubscribeToEvent("TimeFreezeStart", OnTimeFreezeStart);
            EventManager.Instance.SubscribeToEvent("TimeFreezeEnd", OnTimeFreezeEnd);
            ITimeFreezeableScriptCast = (ITimeFreezable)ITimeFreezeableScript;
        }

        private void OnDestroyTimeFreeze()
        {
            EventManager.Instance.UnsubscribeFromEvent("TimeFreezeStart", OnTimeFreezeStart);
            EventManager.Instance.UnsubscribeFromEvent("TimeFreezeEnd", OnTimeFreezeEnd);
        }

        private void OnTimeFreezeStart()
        {
            ITimeFreezeableScriptCast.Freeze();
        }

        private void OnTimeFreezeEnd()
        {
            ITimeFreezeableScriptCast.Unfreeze();
        }

        // Elemental Attack -----------------------------------------------------------------------

        private bool burning = false;
        private CoroutineRunner burningEffectCoroutine;
        private CoroutineRunner burningTimerCoroutine;

        private void InitElementalAttack()
        {
            burningEffectCoroutine = new CoroutineRunner(this);
            burningTimerCoroutine = new CoroutineRunner(this);
        }

        private void OnDestroyElementalAttack() { }

        public void ApplyFireEffect(float burnTime, float damage, float damageInterval, bool fireSpreads, float spreadRange, float spreadChance)
        {
            if (!ElementalAttack) return;

            burning = true;

            burningTimerCoroutine.Restart(BurningTimerCoroutine(burnTime));
            burningEffectCoroutine.Restart(BurningEffectCoroutine(burnTime, damage, damageInterval, fireSpreads, spreadRange, spreadChance));
        }

        private IEnumerator BurningEffectCoroutine(float burnTime, float damage, float damageInterval, bool fireSpreads, float spreadRange, float spreadChance)
        {
            GameObject burningAnimation = Object.Instantiate(fireAttackBurningAnimation,
                                                             gameObject.transform.position,
                                                             gameObject.transform.rotation,
                                                             gameObject.transform);
            Destroy(burningAnimation, burnTime);

            while (burning)
            {
                IDamageableScriptCast.TakeDamage(damage);
                if (fireSpreads)
                {
                    Collider[] colliders = Physics.OverlapSphere(transform.position, spreadRange, LayerMask.GetMask(Utilities.Constants.EnemyLayerName));
                    foreach (var collider in colliders)
                    {
                        if (Random.Range(0f, 1) > spreadChance || GameObject.ReferenceEquals(collider.gameObject, gameObject)) continue;
                        AbilitiesReactionController enemyToSpreadTo = collider.gameObject.GetComponent<AbilitiesReactionController>();
                        if (enemyToSpreadTo.burning) continue;
                        collider.gameObject.GetComponent<AbilitiesReactionController>().ApplyFireEffect(burnTime, damage, damageInterval, fireSpreads, spreadRange, spreadChance);
                    }
                }
                yield return new WaitForSeconds(damageInterval);
            }
        }

        private IEnumerator BurningTimerCoroutine(float burnTime)
        {
            yield return new WaitForSeconds(burnTime);
            burning = false;
        }

        // Mind Control Functions------------------------------------------------------------------
        private List<GameObject> potentialTargetsList;
        private int currentTargetIndex = 0;
        private bool underMindControl = false;
        private GameObject target
        {
            get { return IMindControllableScriptCast.Target; }
            set { IMindControllableScriptCast.Target = value; }
        }

        private CoroutineRunner mindControlCoroutine;
        private CoroutineRunner mindControlDeathCoroutine;

        private void InitMindControl()
        {
            EventManager.Instance.SubscribeToEvent("MindControl", OnMindControl);
            IMindControllableScriptCast = (IMindControllable)IMindControllableScript;

            mindControlCoroutine = new CoroutineRunner(this);
            mindControlDeathCoroutine = new CoroutineRunner(this);
        }

        private void OnDestroyMindControl()
        {
            EventManager.Instance.UnsubscribeFromEvent("MindControl", OnMindControl);
        }

        private void OnMindControl(Dictionary<string, object> args)
        {
            StartMindControl(
                args["potentialTargetsList"] as List<GameObject>,
                (Vector3)args["originOfMindControl"],
                (float)args["radiusOfMindControl"],
                (float)args["abilityDuration"],
                (float)args["healthMultiplier"],
                (float)args["damageMultiplier"]
            );
        }

        private void StartMindControl(List<GameObject> targetsList, Vector3 origin, float radius, float duration, float healthMultiplier, float damageMultiplier)
        {
            potentialTargetsList = targetsList;

            if (underMindControl)
            {
                currentTargetIndex = 0;
                target = null;
                return;
            }

            origin = new Vector3(origin.x, transform.position.y, origin.z); // we only want to check distance in xz-plane

            if (Vector3.Distance(origin, transform.position) < radius)
            {
                GameObject animation = Object.Instantiate(mindControlAnimation, gameObject.transform);
                animation.transform.localScale = transform.localScale;
                underMindControl = true;
                IMindControllableScriptCast.StartMindControl();
                IMindControllableScriptCast.Damage *= damageMultiplier;
                IMindControllableScriptCast.Health *= healthMultiplier;
                mindControlCoroutine.Start(HandleMindControl(duration));
            }
            else potentialTargetsList.Add(gameObject);
        }

        private IEnumerator HandleMindControl(float durationOfMindControl)
        {
            mindControlDeathCoroutine.Restart(MindControlDeathCoroutine(durationOfMindControl));
            while (true)
            {
                target = null;
                yield return WaitForNewTargetsInList();
                yield return WaitForTargetToDie();
            }
        }

        private IEnumerator WaitForTargetToDie()
        {
            while (target != null) yield return null;
        }

        private IEnumerator WaitForNewTargetsInList()
        {
            while (currentTargetIndex >= potentialTargetsList.Count) yield return null;
            target = potentialTargetsList[currentTargetIndex];
            currentTargetIndex++;
        }

        private IEnumerator MindControlDeathCoroutine(float durationOfMindControl)
        {
            yield return new WaitForSeconds(durationOfMindControl);
            IMindControllableScriptCast.ForceDeath();
        }
    }
}
