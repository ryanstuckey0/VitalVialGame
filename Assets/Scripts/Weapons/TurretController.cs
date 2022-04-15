using System;
using System.IO;
using FORGE3D;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViralVial.Enemy;
using ViralVial.Utilities;

namespace ViralVial.Weapons
{
    public class TurretController : MonoBehaviour
    {
        [SerializeField] private string configJsonFile;
        [SerializeField] private F3DTurret F3DTurret;
        [SerializeField] private F3DFXController F3DFXController;
        [SerializeField] private GameObject destroyedTurret;

        [Header("Barrel Overheat Lights")]
        [SerializeField] private float overheatLightIntensityMultiplier = 10;
        [SerializeField] private Light[] barrelOverheatLights;
        [SerializeField] private ParticleSystem barrelSmoke;


        // Configured via JSON ------------------------------------------------------------------------
        [HideInInspector] public float Damage { get; private set; }
        private float targetHoldLockRange;
        private int ammoCapacity_field;
        private int ammoCapacity
        {
            get { return ammoCapacity_field; }
            set
            {
                ammoCapacity_field = value;
                if (ammoCapacity <= 0) OnDeath();
            }
        }
        private float timeBetweenShots;
        private float targetSearchInteval;
        private float targetDetectionRange;
        private float overheatTemperature;
        private float cooldownTemperature;
        private float temperatureOverheatRate;
        private float temperatureCooldownRate;

        // Internal utility fields --------------------------------------------------------------------
        private bool targetLocked = false;
        private bool firing = false;

        private List<GameObject> targets;
        private GameObject target;
        private IEnemy currentTargetIEnemy;
        private int currentTargetIndex = 0;

        private WaitForSeconds targetSearchTimer;
        private WaitForSeconds temperatureUpdateTimer;
        private WaitForSeconds firingTimer;

        private const float temperatureUpdateInterval = 0.2f;
        private const float yValueTargetOffset = 1;

        private float temperatureBackingField = 0;
        private float temperature
        {
            get { return temperatureBackingField; }
            set
            {
                temperatureBackingField = value;
                Array.ForEach<Light>(barrelOverheatLights, (light) => light.intensity = Mathf.Pow(temperature / overheatTemperature * overheatLightIntensityMultiplier, 2));
                if (temperatureBackingField >= overheatTemperature) OnOverheat();
                else if (temperature <= cooldownTemperature) OnCooldown();
            }
        }

        // Control Methods ----------------------------------------------------------------------------

        public void Init(float overheatTemperatureMultiplier = 1, float ammoCapacityMultiplier = 1)
        {
            InitViaJson(JObject.Parse(File.ReadAllText(Application.streamingAssetsPath + configJsonFile)));
            targetSearchTimer = new WaitForSeconds(targetSearchInteval);
            temperatureUpdateTimer = new WaitForSeconds(temperatureUpdateInterval);
            firingTimer = new WaitForSeconds(timeBetweenShots);
            barrelSmoke.Stop();

            ammoCapacity = (int)(ammoCapacity * ammoCapacityMultiplier);
            overheatTemperature *= overheatTemperatureMultiplier;
        }

        private void Start()
        {
            StartCoroutine(ControlLoopCoroutine());
        }

        private void InitViaJson(JObject configJson)
        {
            ammoCapacity = ((int)configJson.GetValue("ammoCapacity"));
            timeBetweenShots = 1f / ((float)configJson.GetValue("rateOfFire"));
            WeaponUtilities.TurretDamagePerShot = ((float)configJson.GetValue("damagePerShot"));
            targetSearchInteval = ((float)configJson.GetValue("targetSearchInterval"));
            targetDetectionRange = ((float)configJson.GetValue("targetDetectionRange"));
            targetHoldLockRange = ((float)configJson.GetValue("targetHoldLockRange"));
            overheatTemperature = ((float)configJson.GetValue("overheatTemperature"));
            cooldownTemperature = ((float)configJson.GetValue("cooldownTemperature"));
            temperatureOverheatRate = ((float)configJson.GetValue("temperatureOverheatRate"));
            temperatureCooldownRate = ((float)configJson.GetValue("temperatureCooldownRate"));
        }

        private IEnumerator ControlLoopCoroutine()
        {
            StartCoroutine(TemperatureControlCoroutine());
            StartCoroutine(FiringCoroutine());
            while (true)
            {
                yield return CheckForTargetsCoroutine();
                yield return TargetLockCoroutine();
            }
        }

        private IEnumerator CheckForTargetsCoroutine()
        {
            if (targets != null && currentTargetIndex < targets.Count)
            {
                OnTargetLock(targets[currentTargetIndex++]);
                yield break;
            }

            targets = new List<GameObject>();
            currentTargetIndex = 0;
            while (true)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, targetDetectionRange, LayerMask.GetMask(Constants.EnemyLayerName));
                foreach (var collider in colliders) targets.Add(collider.gameObject);
                if (targets.Count > 0)
                {
                    OnTargetLock(targets[currentTargetIndex++]);
                    yield break;
                }
                // maybe also add an idle animation, like spinning head back and forth
                yield return targetSearchTimer;
            }
        }

        private IEnumerator TargetLockCoroutine()
        {
            while (targetLocked)
            {
                if (LostTarget()) OnTargetLoss();
                else
                {
                    F3DTurret.SetNewTarget(target.transform.position + new Vector3(0, yValueTargetOffset, 0));
                    yield return null;
                }
            }
        }

        private bool LostTarget()
        {
            return target == null || (currentTargetIEnemy?.IsDead ?? false) || Vector3.Distance(target.transform.position, transform.position) > targetHoldLockRange;
        }

        private void StartFiring()
        {
            if (!targetLocked) return;
            firing = true;
        }

        private void StopFiring()
        {
            firing = false;
        }

        private IEnumerator FiringCoroutine()
        {
            while (true)
            {
                yield return new WaitUntil(() => firing);
                while (firing)
                {
                    F3DFXController.Vulcan();
                    ammoCapacity--;
                    yield return firingTimer;
                }
            }
        }

        private IEnumerator TemperatureControlCoroutine()
        {
            while (true)
            {
                while (!firing) yield return null;
                yield return OverheatControlCoroutine();
                yield return CooldownControlCoroutine();
            }
        }

        private void OnOverheat()
        {
            StopFiring();
            barrelSmoke.Play();
        }

        private void OnCooldown()
        {
            StartFiring();
            barrelSmoke.Stop();
        }

        private void OnTargetLoss()
        {
            firing = false;
            targetLocked = false;
        }

        private void OnTargetLock(GameObject target)
        {
            if (target == null) return;
            this.target = target;
            currentTargetIEnemy = target.GetComponent<IEnemy>();
            firing = true;
            targetLocked = true;
        }

        private IEnumerator OverheatControlCoroutine()
        {
            while (temperature < overheatTemperature && firing)
            {
                temperature += temperatureOverheatRate * temperatureUpdateInterval;
                yield return temperatureUpdateTimer;
            }
        }

        private IEnumerator CooldownControlCoroutine()
        {
            while (temperature > 0 && !firing)
            {
                temperature -= temperatureCooldownRate * temperatureUpdateInterval;
                if (targetLocked && temperature < cooldownTemperature) break;
                yield return temperatureUpdateTimer;
            }
        }

        private void OnDeath()
        {
            Instantiate(destroyedTurret, transform.position, transform.rotation).GetComponent<TurretBurnoutController>().Init(F3DTurret.Swivel.transform.rotation);
            Destroy(gameObject);
        }
    }
}
