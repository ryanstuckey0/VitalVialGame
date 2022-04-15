using FORGE3D;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViralVial.Player.Animation;
using ViralVial.Sound;
using ViralVial.Utilities;

namespace ViralVial.Weapons
{
    public class GunController : MonoBehaviour, IWeapon
    {
        [SerializeField] private string configJsonFile;

        [Header("Firing")]
        [Tooltip("Game object located at the tip of the barrel. This is where the muzzle flash will spawn and raycast will originate from.")]
        [SerializeField] private GameObject bulletExitLocation;
        [SerializeField] private Vector3 raycastOriginOffset;
        [SerializeField] private LayerMask layerMask;

        [Header("Muzzle Flash")]
        [SerializeField] private Transform muzzleFlashPrefab;
        [SerializeField] private Vector3 muzzleFlashScale;

        [Header("Sounds")]
        [SerializeField] private GameObjectAudioController audioController;

        private float damage;
        [HideInInspector] public float Damage { get => damage * damageMultiplier; }
        [HideInInspector] public float BulletMaxTravelDistance;
        private float damageMultiplier = 1;

        private float magazineCapacity_field;
        private int magazineCapacity
        {
            get { return (int)(magazineCapacity_field * magazineCapacityMultiplier); }
            set { magazineCapacity_field = value; }
        }

        private float magazineCapacityMultiplier = 1;
        private bool isFullAuto;
        private float timeBetweenShots;
        public int MagazineCount { get; private set; } = 0;

        private IGunRaycaster gunRayCaster;
        private const float muzzleFlashDestroyDelay = 0.05f;
        private bool canFire = true;

        private bool firing = false;

        private Dictionary<string, object> ammoEventDictionary;
        private PlayerWeaponAnimationController playerWeaponAnimationController;
        private CoroutineRunner fireCoroutine;
        private YieldInstruction timeBetweenShotsYielder;

        private void Awake()
        {
            JObject config = JObject.Parse(File.ReadAllText(Application.streamingAssetsPath + configJsonFile));
            magazineCapacity = (int)config.GetValue("magazineCapacity");
            timeBetweenShotsYielder = new WaitForSeconds(1f / (float)config.GetValue("rateOfFire"));
            isFullAuto = (bool)config.GetValue("isFullAuto");
            BulletMaxTravelDistance = (float)config.GetValue("distance");
            damage = (float)config.GetValue("damage");
            if ((bool)config.GetValue("startWithFullAmmo")) MagazineCount = magazineCapacity;
            JObject raycasterJson = config.GetValue("raycaster").ToObject<JObject>();
            gunRayCaster = (IGunRaycaster)Activator.CreateInstance(Type.GetType((string)raycasterJson.GetValue("class")));
            gunRayCaster.Init(this, raycasterJson, layerMask);


            ammoEventDictionary = new Dictionary<string, object> {
                {"currentCount", MagazineCount},
                {"capacity", magazineCapacity},
            };

            fireCoroutine = new CoroutineRunner(this);

            CoroutineYielderCache.AddOrModifyYielder("GunFireDelay", new WaitForSeconds(0.2f));
        }

        private void Start()
        {
            FireAmmoUpdateEvent();
        }

        public void Reload(int reloadAmount)
        {
            MagazineCount += reloadAmount;
            FireAmmoUpdateEvent();
        }

        public int GetReloadAmount()
        {
            return magazineCapacity - MagazineCount;
        }

        public void PlayCockGunSound()
        {
            audioController.PlayAudio("CockGun");
        }

        public void PlayReloadSound()
        {
            audioController.PlayAudio("ReloadGun");
        }

        public void AddDamageMultiplier(float multiplier)
        {
            damageMultiplier += multiplier;
        }

        public void AddMagazineCapacityMultiplier(float multiplier)
        {
            magazineCapacityMultiplier += multiplier;
        }


        public void PressGunTrigger(PlayerWeaponAnimationController playerWeaponAnimationController = null, Action startedFiringCallback = null, Action finishedFiringCallback = null)
        {
            if (!canFire) return;
            if (MagazineCount <= 0) { audioController.PlayAudio("EmptyClip"); return; }
            this.playerWeaponAnimationController = playerWeaponAnimationController;

            if (fireCoroutine.Start(FireCoroutine(finishedFiringCallback)))
            {
                startedFiringCallback?.Invoke();
                canFire = false;
                firing = true;
            }
        }

        public void ReleaseGunTrigger()
        {
            canFire = true;
            firing = false;
        }

        private IEnumerator FireCoroutine(Action finishedFiringCallback = null)
        {
            yield return isFullAuto ? FireFullAutoCoroutine() : FireNotFullAutoCoroutine();
            finishedFiringCallback?.Invoke();
        }

        private IEnumerator FireNotFullAutoCoroutine()
        {
            playerWeaponAnimationController?.PlayFiringAnimation();
            yield return CoroutineYielderCache.GetYielder("GunFireDelay");
            MagazineCount--;
            FireAmmoUpdateEvent();
            gunRayCaster.FireRays(bulletExitLocation.transform.position + raycastOriginOffset, playerWeaponAnimationController.PlayerForwardVector, BulletMaxTravelDistance);
            audioController.PlayAudio("FireGun");
            SpawnMuzzleFlash();
        }

        private IEnumerator FireFullAutoCoroutine()
        {
            playerWeaponAnimationController?.PlayFiringAnimation();
            yield return CoroutineYielderCache.GetYielder("GunFireDelay");
            while (firing && MagazineCount > 0)
            {
                MagazineCount--;
                FireAmmoUpdateEvent();
                gunRayCaster.FireRays(bulletExitLocation.transform.position + raycastOriginOffset, playerWeaponAnimationController.PlayerForwardVector, BulletMaxTravelDistance);
                audioController.PlayAudio("FireGun");
                SpawnMuzzleFlash();
                yield return timeBetweenShotsYielder;
                playerWeaponAnimationController?.PlayFiringAnimation();
            }
        }

        private void SpawnMuzzleFlash()
        {
            Transform muzzleFlash = F3DPoolManager.Pools["GeneratedPool"].Spawn(muzzleFlashPrefab, bulletExitLocation.transform.position, Quaternion.identity, bulletExitLocation.transform);
            muzzleFlash.forward = bulletExitLocation.transform.forward;
            muzzleFlash.localScale = muzzleFlashScale;
        }

        public void FireAmmoUpdateEvent()
        {
            if (ammoEventDictionary == null) return;
            ammoEventDictionary["currentCount"] = MagazineCount;
            ammoEventDictionary["capacity"] = magazineCapacity;
            EventManager.Instance.InvokeEvent("UpdateGunAmmo", ammoEventDictionary);
        }
    }
}
