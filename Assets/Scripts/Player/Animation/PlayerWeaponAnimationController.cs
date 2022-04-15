using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using ViralVial.Player.MonoBehaviourScript;
using ViralVial.Weapons;
using ViralVial.Utilities;

namespace ViralVial.Player.Animation
{
    /// <summary>
    /// Controls the animator to trigger weapon change animations.
    /// </summary>
    public class PlayerWeaponAnimationController : MonoBehaviour
    {
        public InventoryItem EquippedGun { get; private set; } = InventoryItem.NoGun;
        public InventoryItem EquippedThrowable { get; private set; } = InventoryItem.NoThrowable;
        public InventoryItem EquippedMelee { get; private set; } = InventoryItem.NoMelee;

        public BasePlayerController BasePlayerController;

        [Header("Gun GameObjects")]
        public GameObject Pistol;
        public GameObject SMG;
        public GameObject AR;
        public GameObject Shotgun;
        public GameObject LMG;
        [HideInInspector] public GameObject ActiveGunGameObject { get; private set; } = null;
        private bool reloading;
        private Dictionary<string, object> ammoIndicatorDictionary;

        [Header("Melee GameObjects")]
        public GameObject FistLeft;
        public GameObject FistRight;
        public GameObject KnifeLeft;
        public GameObject KnifeRight;
        public GameObject MacheteLeft;
        public GameObject MacheteRight;
        public GameObject KatanaLeft;
        public GameObject KatanaRight;
        private GameObject activeMeleeLeftGameObject = null;
        private GameObject activeMeleeRightGameObject = null;

        [Header("Throwable GameObjects")]
        public GameObject Rock;
        public GameObject AlarmClock;
        public GameObject ProximityMines;
        public GameObject Grenade;
        public GameObject TurretMain;
        public TurretHolographController turretHolographController;

        private ThrowableController activeThrowableInProgress;
        private bool placingTurret = false;

        // Gun Controller Scripts -----------------------------------------------------------------
        public GunController PistolControllerScript { get; private set; }
        public GunController SMGControllerScript { get; private set; }
        public GunController ARControllerScript { get; private set; }
        public GunController ShotgunControllerScript { get; private set; }
        public GunController LMGControllerScript { get; private set; }
        private GunController activeGunController;

        // Melee Controller Scripts ---------------------------------------------------------------
        private MeleeController FistLeftControllerScript;
        private MeleeController FistRightControllerScript;
        private MeleeController KnifeLeftControllerScript;
        private MeleeController KnifeRightControllerScript;
        private MeleeController MacheteLeftControllerScript;
        private MeleeController MacheteRightControllerScript;
        private MeleeController KatanaLeftControllerScript;
        private MeleeController KatanaRightControllerScript;
        private MeleeController activeMeleeLeftController;
        private MeleeController activeMeleeRightController;
        private bool dualWielding = false;
        private bool animatingMeleeAttack = false;
        private bool leftMeleeVisible = false;
        private bool leftMeleeVisibleProperty
        {
            get { return leftMeleeVisible; }
            set
            {
                leftMeleeVisible = value;
                ChangeVisibility(activeMeleeLeftGameObject, value);
            }
        }
        private bool rightMeleeVisible = false;
        private bool rightMeleeVisibleProperty
        {
            get { return rightMeleeVisible; }
            set
            {
                rightMeleeVisible = value;
                ChangeVisibility(activeMeleeRightGameObject, value);
            }
        }
        private bool savedLeftMeleeVisibleState;

        private Animator playerAnimator;
        private IPlayer owningPlayer;
        private PermittedActionsState genericPermittedActionsState;

        public Vector3 PlayerForwardVector { get => owningPlayer.Transform.forward; }

        private void Awake()
        {
            PistolControllerScript = Pistol?.GetComponent<GunController>();
            SMGControllerScript = SMG?.GetComponent<GunController>();
            ARControllerScript = AR?.GetComponent<GunController>();
            ShotgunControllerScript = Shotgun?.GetComponent<GunController>();
            LMGControllerScript = LMG?.GetComponent<GunController>();

            Pistol?.SetActive(false);
            SMG?.SetActive(false);
            AR?.SetActive(false);
            Shotgun?.SetActive(false);
            LMG?.SetActive(false);

            FistLeftControllerScript = FistLeft?.GetComponent<MeleeController>();
            FistRightControllerScript = FistRight?.GetComponent<MeleeController>();
            KnifeLeftControllerScript = KnifeLeft?.GetComponent<MeleeController>();
            KnifeRightControllerScript = KnifeRight?.GetComponent<MeleeController>();
            MacheteLeftControllerScript = MacheteLeft?.GetComponent<MeleeController>();
            MacheteRightControllerScript = MacheteRight?.GetComponent<MeleeController>();
            KatanaLeftControllerScript = KatanaLeft?.GetComponent<MeleeController>();
            KatanaRightControllerScript = KatanaRight?.GetComponent<MeleeController>();

            FistLeft?.SetActive(false);
            FistRight?.SetActive(false);
            KnifeLeft?.SetActive(false);
            KnifeRight?.SetActive(false);
            MacheteLeft?.SetActive(false);
            MacheteRight?.SetActive(false);
            KatanaLeft?.SetActive(false);
            KatanaRight?.SetActive(false);

            ammoIndicatorDictionary = new Dictionary<string, object>();
            ammoIndicatorDictionary.Add("ammoInInventory", 0);

            genericPermittedActionsState = new PermittedActionsState()
            {
                Move = true,
                MoveAim = true
            };

            EventManager.Instance.SubscribeToEvent("Animator_ReleaseThrowable", OnAnimator_ReleaseThrowable);
            EventManager.Instance.SubscribeToEvent("Animator_FinishedMelee", OnAnimator_FinishedMelee);
            EventManager.Instance.SubscribeToEvent("Animator_FinishedReload", OnAnimator_FinishedReload);
            EventManager.Instance.SubscribeToEvent("Animator_Reload", OnAnimator_Reload);
            EventManager.Instance.SubscribeToEvent("Animator_CockGun", OnAnimator_CockGun);

            EventManager.Instance.SubscribeToEvent("GunDamageMultiplierChange", OnGunDamageMultiplierChange);
            EventManager.Instance.SubscribeToEvent("MeleeDamageMultiplierChange", OnMeleeDamageMultiplierChange);
            EventManager.Instance.SubscribeToEvent("OverallDamageMultiplierChange", OnOverallDamageMultiplierChange);

            EventManager.Instance.SubscribeToEvent("GunMagazineSizeMultiplierChange", OnGunMagazineSizeMultiplierChange);
            EventManager.Instance.SubscribeToEvent("PlayerDisallowFire", OnPlayerDisallowFire);
        }

        private void OnDestroy()
        {
            EventManager.Instance.UnsubscribeFromEvent("Animator_ReleaseThrowable", OnAnimator_ReleaseThrowable);
            EventManager.Instance.UnsubscribeFromEvent("Animator_FinishedMelee", OnAnimator_FinishedMelee);
            EventManager.Instance.UnsubscribeFromEvent("Animator_FinishedReload", OnAnimator_FinishedReload);
            EventManager.Instance.UnsubscribeFromEvent("Animator_Reload", OnAnimator_Reload);
            EventManager.Instance.UnsubscribeFromEvent("Animator_CockGun", OnAnimator_CockGun);

            EventManager.Instance.UnsubscribeFromEvent("GunDamageMultiplierChange", OnGunDamageMultiplierChange);
            EventManager.Instance.UnsubscribeFromEvent("MeleeDamageMultiplierChange", OnMeleeDamageMultiplierChange);
            EventManager.Instance.UnsubscribeFromEvent("OverallDamageMultiplierChange", OnOverallDamageMultiplierChange);

            EventManager.Instance.UnsubscribeFromEvent("GunMagazineSizeMultiplierChange", OnGunMagazineSizeMultiplierChange);
            EventManager.Instance.UnsubscribeFromEvent("PlayerDisallowFire", OnPlayerDisallowFire);
        }

        private void Start()
        {
            owningPlayer = BasePlayerController.OwningPlayer;
            playerAnimator = owningPlayer.BasePlayerController.PlayerAnimator;

            EquippedGun = InventoryItem.NoGun;
            SwitchGun(EquippedGun);
            UpdateMeleeOnGunSwitch(EquippedGun);

            EquippedMelee = InventoryItem.NoMelee;
            SwitchMelee(EquippedMelee);

            EquippedThrowable = InventoryItem.NoThrowable;
            SwitchThrowable(EquippedThrowable);
        }

        private void OnOverallDamageMultiplierChange(Dictionary<string, object> args)
        {
            OnGunDamageMultiplierChange(args);
            OnMeleeDamageMultiplierChange(args);
        }

        private void OnGunMagazineSizeMultiplierChange(Dictionary<string, object> args)
        {
            float multiplier = (float)args["newValue"];
            PistolControllerScript.AddMagazineCapacityMultiplier(multiplier);
            SMGControllerScript.AddMagazineCapacityMultiplier(multiplier);
            ARControllerScript.AddMagazineCapacityMultiplier(multiplier);
            ShotgunControllerScript.AddMagazineCapacityMultiplier(multiplier);
            LMGControllerScript.AddMagazineCapacityMultiplier(multiplier);
        }

        // Guns: Operation ------------------------------------------------------------------------
        public void PressGunTrigger()
        {
            if (EquippedGun != InventoryItem.NoGun)
                activeGunController.PressGunTrigger(this, StartedFiringCallback, FinishedFiringCallback);
        }

        public void ReleaseGunTrigger()
        {
            if (EquippedGun != InventoryItem.NoGun)
                activeGunController.ReleaseGunTrigger();
        }

        public void StartedFiringCallback()
        {
            owningPlayer.PermittedActions.LoadStateAndLock(genericPermittedActionsState);
        }

        public void FinishedFiringCallback()
        {
            owningPlayer.PermittedActions.UnlockPlayer();
            owningPlayer.PermittedActions.ChangeAll(true);
        }

        private void OnPlayerDisallowFire()
        {
            ReleaseGunTrigger();
        }

        public void OnGunDamageMultiplierChange(Dictionary<string, object> args)
        {
            float multiplier = (float)args["newValue"];
            PistolControllerScript.AddDamageMultiplier(multiplier);
            SMGControllerScript.AddDamageMultiplier(multiplier);
            ARControllerScript.AddDamageMultiplier(multiplier);
            ShotgunControllerScript.AddDamageMultiplier(multiplier);
            LMGControllerScript.AddDamageMultiplier(multiplier);
        }

        // Guns: Reload Gun -----------------------------------------------------------------------------
        public void ReloadGun()
        {
            if (EquippedGun == InventoryItem.NoGun) return;
            ReleaseGunTrigger();
            int reloadAmount = activeGunController.GetReloadAmount();
            int playerAmmoCount = GetAmmoCountForPlayer(EquippedGun);
            reloadAmount = playerAmmoCount < reloadAmount ? playerAmmoCount : reloadAmount;
            if (reloadAmount > 0)
            {
                owningPlayer.PermittedActions.LoadStateAndLock(genericPermittedActionsState);
                StartCoroutine(ReloadGunCoroutine(reloadAmount));
            }
        }

        private IEnumerator ReloadGunCoroutine(int reloadAmount)
        {
            savedLeftMeleeVisibleState = this.leftMeleeVisibleProperty;
            if (savedLeftMeleeVisibleState) leftMeleeVisibleProperty = false;
            SetIKHandsEnabled(false);
            owningPlayer.SetUpperBodyAnimatorTrigger(AnimatorTrigger.ReloadTrigger);

            if (EquippedGun == InventoryItem.Shotgun)
            {
                reloading = true;
                yield return new WaitUntil(() => !reloading); // initial shotgun animation
                playerAnimator.SetBool("reloading", true);
                while (reloadAmount > 0)
                {
                    reloading = true;
                    yield return new WaitUntil(() => !reloading); // single round reload animation
                    reloadAmount--;
                    ModifyAmmoCountForPlayer(EquippedGun, -1);
                    activeGunController.Reload(1);
                }
                playerAnimator.SetBool("reloading", false);
            }
            else
            {
                reloading = true;
                yield return new WaitUntil(() => !reloading);
                activeGunController.Reload(reloadAmount);
                ModifyAmmoCountForPlayer(EquippedGun, -reloadAmount);
            }

            SetIKHandsEnabled(true);

            owningPlayer.PermittedActions.UnlockPlayer();
            owningPlayer.PermittedActions.ChangeAll(true);

            if (savedLeftMeleeVisibleState) leftMeleeVisibleProperty = true;

            UpdateInventoryAmmo();
        }

        private void OnAnimator_FinishedReload()
        {
            reloading = false;
        }

        private void OnAnimator_CockGun()
        {
            activeGunController.PlayCockGunSound();
        }

        private void OnAnimator_Reload()
        {
            activeGunController.PlayReloadSound();
        }

        // Guns: Switch Gun -----------------------------------------------------------------------------
        public void SwitchGun(InventoryItem newGun)
        {
            if (newGun != EquippedGun)
            {
                ReleaseGunTrigger();
                owningPlayer.PermittedActions.LoadStateAndLock(genericPermittedActionsState);
                StartCoroutine(SwitchGunCoroutine(newGun, EquippedGun));
            }
        }

        private void UpdateMeleeOnGunSwitch(InventoryItem newGun)
        {
            if (CanDualWield()) StartDualWield();
            else if (newGun != InventoryItem.NoGun) EndDualWield();
            leftMeleeVisibleProperty = newGun == InventoryItem.NoGun || newGun == InventoryItem.Pistol;
        }

        public IEnumerator SwitchGunCoroutineUtility(InventoryItem newGun)
        {
            if (newGun == EquippedGun || !owningPlayer.PermittedActions.SwitchGuns) yield break;
            ReleaseGunTrigger();
            owningPlayer.PermittedActions.LoadStateAndLock(genericPermittedActionsState);
            yield return SwitchGunCoroutine(newGun, EquippedGun);
        }

        private IEnumerator SwitchGunCoroutine(InventoryItem newWeapon, InventoryItem oldWeapon)
        {
            if (oldWeapon != InventoryItem.NoGun) yield return SheathWeaponCoroutine();
            if (newWeapon != InventoryItem.NoGun) yield return UnsheathWeaponCoroutine(newWeapon);

            activeGunController = GetGunControllerForGun(newWeapon);
            ActiveGunGameObject = GetGameObjectForGun(newWeapon);
            EquippedGun = newWeapon;

            UpdateMeleeOnGunSwitch(newWeapon);

            owningPlayer.PermittedActions.UnlockPlayer();
            owningPlayer.PermittedActions.ChangeAll(true);

            activeGunController?.FireAmmoUpdateEvent();
            UpdateInventoryAmmo();

            EventManager.Instance.InvokeEvent("HotbarAbilityAssign", new Dictionary<string, object>
                {
                    { "assignedItemName", newWeapon.ToString() },
                    { "assignedItemId", null },
                    { "slotId", "G" }
                }
            );
        }

        private void UpdateInventoryAmmo()
        {
            ammoIndicatorDictionary["ammoInInventory"] = GetAmmoCountForPlayer(EquippedGun);
            EventManager.Instance.InvokeEvent("UpdateInventoryAmmo", ammoIndicatorDictionary);
        }

        private IEnumerator SheathWeaponCoroutine(AnimatorWeaponSheathLocation sheathLocation = AnimatorWeaponSheathLocation.Back)
        {
            if (UsesIKHands(EquippedGun)) owningPlayer.BasePlayerController.PlayerIKHandsController.BlendIK(false, 0f, 0f, (int)InventoryItem.NoGun);
            owningPlayer.SetUpperBodyAnimatorTrigger(AnimatorTrigger.WeaponSheathTrigger);
            playerAnimator.SetInteger("SheathLocation", (int)sheathLocation);
            playerAnimator.SetInteger("GunTo", (int)InventoryItem.NoGun);
            playerAnimator.SetInteger("GunFrom", (int)EquippedGun);
            playerAnimator.SetInteger("LeftWeapon", (int)InventoryItem.NoGun);
            playerAnimator.SetInteger("RightWeapon", (int)InventoryItem.NoGun);

            StartCoroutine(ChangeVisibility(GetGameObjectForGun(EquippedGun), false, 0.5f));
            owningPlayer.SetWholeBodyAnimatorTrigger(AnimatorTrigger.SwitchToUnarmedMovementTrigger);
            yield return new WaitForSeconds(0.2f);
        }

        private IEnumerator UnsheathWeaponCoroutine(InventoryItem weaponType, WeaponUtilities.WeaponSide weaponSide = WeaponUtilities.WeaponSide.Right)
        {
            owningPlayer.SetUpperBodyAnimatorTrigger(AnimatorTrigger.WeaponUnsheathTrigger);

            playerAnimator.SetInteger("GunTo", (int)weaponType);
            playerAnimator.SetInteger("GunFrom", (int)InventoryItem.NoGun);
            playerAnimator.SetInteger("SheathLocation", (int)AnimatorWeaponSheathLocation.Back);

            if (WeaponUtilities.Is1HandedWeapon(weaponType))
            {
                playerAnimator.SetInteger("LeftRight", (int)weaponSide);
                switch (weaponSide)
                {
                    case WeaponUtilities.WeaponSide.Left:
                        playerAnimator.SetInteger("LeftWeapon", (int)weaponType);
                        break;
                    case WeaponUtilities.WeaponSide.Right:
                        playerAnimator.SetInteger("RightWeapon", (int)weaponType);
                        break;
                }
            }
            else
            {
                playerAnimator.SetInteger("LeftWeapon", (int)weaponType);
                playerAnimator.SetInteger("RightWeapon", (int)weaponType);
            }

            StartCoroutine(ChangeVisibility(GetGameObjectForGun(weaponType), true, 0.4f));
            if (UsesIKHands(weaponType)) owningPlayer.BasePlayerController.PlayerIKHandsController.BlendIK(true, 0.75f, 0.25f, (int)weaponType);
            owningPlayer.SetWholeBodyAnimatorTrigger(WeaponUtilities.Is1HandedWeapon(weaponType) ? AnimatorTrigger.SwitchToArmedMovementTrigger : AnimatorTrigger.SwitchToShootingMovementTrigger);
            yield return new WaitForSeconds(0.6f);
        }

        private IEnumerator ChangeVisibility(GameObject gameObject, bool isVisible, float delay)
        {
            yield return new WaitForSeconds(delay);
            gameObject?.SetActive(isVisible);
        }

        private void ChangeVisibility(GameObject gameObject, bool isVisible)
        {
            gameObject?.SetActive(isVisible);
        }

        public void SetIKHandsEnabled(bool enabled, float delay = 0.25f, float timeToBlend = 0.25f)
        {
            if (enabled) owningPlayer.BasePlayerController.PlayerIKHandsController.BlendIK(true, delay, timeToBlend, (int)EquippedGun);
            else owningPlayer.BasePlayerController.PlayerIKHandsController.BlendIK(false, 0f, 0f, (int)InventoryItem.NoGun);
        }

        public void PlayFiringAnimation()
        {
            owningPlayer.SetWholeBodyAnimatorTrigger(AnimatorTrigger.FireGunTrigger);
            playerAnimator.SetInteger("Action", 1);
        }

        // Melee ----------------------------------------------------------------------------------

        private bool CanDualWield()
        {
            return EquippedGun == InventoryItem.NoGun
                && !dualWielding
                && owningPlayer.PermittedActions.DualWield;
        }

        public void StartDualWield()
        {
            if (!CanDualWield()) return;
            StartCoroutine(ActivateDualWieldCoroutine());
            leftMeleeVisibleProperty = true;
            rightMeleeVisibleProperty = true;
            dualWielding = true;
            playerAnimator.SetBool("MeleeDualWieldBool", true);
        }

        public void EndDualWield()
        {
            if (!dualWielding) return;
            leftMeleeVisibleProperty = false;
            rightMeleeVisibleProperty = false;
            dualWielding = false;
            playerAnimator.SetBool("MeleeDualWieldBool", false);
        }

        public IEnumerator ActivateDualWieldCoroutine()
        {
            yield return owningPlayer.EquipmentManager.EquipGunCoroutine(InventoryItem.NoGun);
        }

        public void SwitchMelee(InventoryItem newMelee)
        {
            GameObject newMeleeLeftGameObject = GetGameObjectForMelee(newMelee, WeaponUtilities.WeaponSide.Left);
            GameObject newMeleeRightGameObject = GetGameObjectForMelee(newMelee, WeaponUtilities.WeaponSide.Right);

            if (leftMeleeVisible)
            {
                ChangeVisibility(activeMeleeLeftGameObject, false);
                ChangeVisibility(newMeleeLeftGameObject, true);
            }

            if (rightMeleeVisible)
            {
                ChangeVisibility(activeMeleeRightGameObject, false);
                ChangeVisibility(newMeleeRightGameObject, true);
            }

            playerAnimator.SetInteger("MeleeWeapon", (int)newMelee);

            activeMeleeLeftController = GetMeleeControllerForMelee(newMelee, WeaponUtilities.WeaponSide.Left);
            activeMeleeRightController = GetMeleeControllerForMelee(newMelee, WeaponUtilities.WeaponSide.Right);

            activeMeleeLeftGameObject = newMeleeLeftGameObject;
            activeMeleeRightGameObject = newMeleeRightGameObject;
            EquippedMelee = newMelee;
        }

        public void UseMelee()
        {
            owningPlayer.PermittedActions.LoadStateAndLock(genericPermittedActionsState);
            ReleaseGunTrigger();
            StartCoroutine(MeleeAttackCoroutine());
        }

        private IEnumerator MeleeAttackCoroutine()
        {
            if (UsesIKHands(EquippedGun)) SetIKHandsEnabled(false);

            owningPlayer.SetUpperBodyAnimatorTrigger(AnimatorTrigger.MeleeAttackTrigger);
            playerAnimator.SetInteger("AttackStyle", GetRandomMeleeAttackStyle(EquippedMelee));

            if (!leftMeleeVisible) ChangeVisibility(activeMeleeLeftGameObject, isVisible: true);

            activeMeleeLeftController.UseMelee();
            if (dualWielding) activeMeleeRightController.UseMelee();

            animatingMeleeAttack = true;
            yield return new WaitUntil(() => !animatingMeleeAttack);
            // if (dualWielding)
            // {
            //     animatingMeleeAttack = true;
            //     while (animatingMeleeAttack) yield return null;
            // }

            if (!leftMeleeVisible) ChangeVisibility(activeMeleeLeftGameObject, isVisible: false);

            if (UsesIKHands(EquippedGun)) SetIKHandsEnabled(true);

            owningPlayer.PermittedActions.UnlockPlayer();
            owningPlayer.PermittedActions.ChangeAll(true);
        }

        private void OnAnimator_FinishedMelee()
        {
            animatingMeleeAttack = false;
        }

        private void OnMeleeDamageMultiplierChange(Dictionary<string, object> args)
        {
            float multiplier = (float)args["newValue"];
            FistLeftControllerScript.AddDamageMultiplier(multiplier);
            FistRightControllerScript.AddDamageMultiplier(multiplier);
            KnifeLeftControllerScript.AddDamageMultiplier(multiplier);
            KnifeRightControllerScript.AddDamageMultiplier(multiplier);
            MacheteLeftControllerScript.AddDamageMultiplier(multiplier);
            MacheteRightControllerScript.AddDamageMultiplier(multiplier);
            KatanaLeftControllerScript.AddDamageMultiplier(multiplier);
            KatanaRightControllerScript.AddDamageMultiplier(multiplier);
        }

        // Throwables -----------------------------------------------------------------------------

        public void SwitchThrowable(InventoryItem newThrowable)
        {
            EquippedThrowable = newThrowable;
        }

        public void UseThrowable()
        {
            if (EquippedThrowable == InventoryItem.NoThrowable) return;
            if (GetThrowableCountForPlayer(EquippedThrowable) <= 0) return;
            if (EquippedThrowable == InventoryItem.Turret)
            {
                HandleTurretPlacing();
                return;
            }

            owningPlayer.PermittedActions.LoadStateAndLock(genericPermittedActionsState);

            StartCoroutine(UseThrowableCoroutine());
            ModifyThrowableCountForPlayer(EquippedThrowable, -1);
        }

        private void HandleTurretPlacing()
        {
            if(placingTurret) return;
            StartCoroutine(PlaceTurretCoroutine());
        }

        private IEnumerator PlaceTurretCoroutine()
        {
            placingTurret = true;

            turretHolographController.gameObject.SetActive(true);
            owningPlayer.GameObject.AddComponent<TurretPlacementInputController>().Init(owningPlayer, this, turretHolographController);
            yield return new WaitUntil(() => !placingTurret);
            turretHolographController.gameObject.SetActive(false);
        }

        public void PlaceTurret()
        {
            placingTurret = false;
            Object.Instantiate(
                TurretMain,
                turretHolographController.transform.position,
                turretHolographController.transform.rotation
                )
                .GetComponent<TurretController>().Init(
                    owningPlayer.PlayerAttributes.TurretOverheatLimitMultiplier,
                    owningPlayer.PlayerAttributes.TurretAmmoCapacityMultiplier
                );
            ModifyThrowableCountForPlayer(InventoryItem.Turret, -1);
        }

        public void CancelTurret()
        {
            placingTurret = false;
        }

        private IEnumerator UseThrowableCoroutine()
        {
            savedLeftMeleeVisibleState = this.leftMeleeVisibleProperty;
            if (savedLeftMeleeVisibleState) leftMeleeVisibleProperty = false;
            SetIKHandsEnabled(false);
            activeThrowableInProgress = GameObject.Instantiate(
                GetGameObjectForThrowable(EquippedThrowable),
                owningPlayer.BasePlayerController.PlayerAnimationGameObjects.LeftHand.transform.position,
                Quaternion.identity
            ).GetComponent<ThrowableController>();
            activeThrowableInProgress.HandToTrack = owningPlayer.BasePlayerController.PlayerAnimationGameObjects.LeftHand;

            owningPlayer.SetUpperBodyAnimatorTrigger(AnimatorTrigger.UseThrowableTrigger);
            playerAnimator.SetInteger("AttackStyle", (int)activeThrowableInProgress.ThrowAnimationStyle);

            yield break;
        }

        private void OnAnimator_ReleaseThrowable()
        {
            activeThrowableInProgress.Throw(owningPlayer.Transform);

            owningPlayer.PermittedActions.UnlockPlayer();
            owningPlayer.PermittedActions.ChangeAll(true);
            SetIKHandsEnabled(true);
            if (savedLeftMeleeVisibleState) leftMeleeVisibleProperty = true;
        }

        // Private Utility Methods ----------------------------------------------------------------
        private int GetAmmoCountForPlayer(InventoryItem weaponType)
        {
            switch (weaponType)
            {
                case InventoryItem.Pistol:
                case InventoryItem.SMG:
                    return owningPlayer.Inventory.CurrentInventory[InventoryItem.PistolSMGAmmo].CurrentCount;
                case InventoryItem.AR:
                    return owningPlayer.Inventory.CurrentInventory[InventoryItem.ARAmmo].CurrentCount;
                case InventoryItem.LMG:
                    return owningPlayer.Inventory.CurrentInventory[InventoryItem.LMGAmmo].CurrentCount;
                case InventoryItem.Shotgun:
                    return owningPlayer.Inventory.CurrentInventory[InventoryItem.ShotgunAmmo].CurrentCount;
                case InventoryItem.NoGun: return 0;
                default:
                    Debug.LogWarning($"Could not inventory item for ammo type {weaponType.ToString()}.");
                    return -1;
            }
        }

        private int ModifyAmmoCountForPlayer(InventoryItem weaponType, int ammoAmount)
        {
            switch (weaponType)
            {
                case InventoryItem.Pistol:
                case InventoryItem.SMG:
                    return owningPlayer.Inventory.AddToInventory(InventoryItem.PistolSMGAmmo, ammoAmount);
                case InventoryItem.AR:
                    return owningPlayer.Inventory.AddToInventory(InventoryItem.ARAmmo, ammoAmount);
                case InventoryItem.LMG:
                    return owningPlayer.Inventory.AddToInventory(InventoryItem.LMGAmmo, ammoAmount);
                case InventoryItem.Shotgun:
                    return owningPlayer.Inventory.AddToInventory(InventoryItem.ShotgunAmmo, ammoAmount);
                default:
                    Debug.LogWarning($"Could not modify inventory ammo for ammo type {weaponType.ToString()}.");
                    return -404;
            }
        }

        private int ModifyThrowableCountForPlayer(InventoryItem throwableType, int count)
        {
            switch (throwableType)
            {
                case InventoryItem.Rock: return owningPlayer.Inventory.AddToInventory(InventoryItem.Rock, count);
                case InventoryItem.AlarmBomb: return owningPlayer.Inventory.AddToInventory(InventoryItem.AlarmBomb, count);
                case InventoryItem.ProximityMine: return owningPlayer.Inventory.AddToInventory(InventoryItem.ProximityMine, count);
                case InventoryItem.Grenade: return owningPlayer.Inventory.AddToInventory(InventoryItem.Grenade, count);
                case InventoryItem.Turret: return owningPlayer.Inventory.AddToInventory(InventoryItem.Turret, count);
                default:
                    Debug.LogWarning($"Could not modify inventory for throwable type {throwableType.ToString()}.");
                    return -404;
            }
        }

        private int GetThrowableCountForPlayer(InventoryItem throwableType)
        {
            switch (throwableType)
            {
                case InventoryItem.Rock: return owningPlayer.Inventory.CurrentInventory[InventoryItem.Rock].CurrentCount;
                case InventoryItem.AlarmBomb: return owningPlayer.Inventory.CurrentInventory[InventoryItem.AlarmBomb].CurrentCount;
                case InventoryItem.ProximityMine: return owningPlayer.Inventory.CurrentInventory[InventoryItem.ProximityMine].CurrentCount;
                case InventoryItem.Grenade: return owningPlayer.Inventory.CurrentInventory[InventoryItem.Grenade].CurrentCount;
                case InventoryItem.Turret: return owningPlayer.Inventory.CurrentInventory[InventoryItem.Turret].CurrentCount;
                default:
                    Debug.LogWarning($"Could not modify inventory for throwable type {throwableType.ToString()}.");
                    return -404;
            }
        }

        private GameObject GetGameObjectForGun(InventoryItem weaponType)
        {
            switch (weaponType)
            {
                case InventoryItem.NoGun: return null;
                case InventoryItem.Pistol: return Pistol;
                case InventoryItem.SMG: return SMG;
                case InventoryItem.AR: return AR;
                case InventoryItem.Shotgun: return Shotgun;
                case InventoryItem.LMG: return LMG;
                default:
                    Debug.LogError($"A gameobject does not exists for weapon {weaponType} or it has not been added here.");
                    return null;
            }
        }

        private GameObject GetGameObjectForMelee(InventoryItem meleeType, WeaponUtilities.WeaponSide leftOrRight)
        {
            switch (meleeType)
            {
                case InventoryItem.NoMelee: return leftOrRight == WeaponUtilities.WeaponSide.Left ? FistLeft : FistRight;
                case InventoryItem.Knife: return leftOrRight == WeaponUtilities.WeaponSide.Left ? KnifeLeft : KnifeRight;
                case InventoryItem.Machete: return leftOrRight == WeaponUtilities.WeaponSide.Left ? MacheteLeft : MacheteRight;
                case InventoryItem.Katana: return leftOrRight == WeaponUtilities.WeaponSide.Left ? KatanaLeft : KatanaRight;
                default:
                    Debug.LogError($"A gameobject does not exists for melee {meleeType} or it has not been added here.");
                    return null;
            }
        }

        private GameObject GetGameObjectForThrowable(InventoryItem throwableType)
        {
            switch (throwableType)
            {
                case InventoryItem.NoThrowable: return null;
                case InventoryItem.Rock: return Rock;
                case InventoryItem.AlarmBomb: return AlarmClock;
                case InventoryItem.ProximityMine: return ProximityMines;
                case InventoryItem.Grenade: return Grenade;
                case InventoryItem.Turret: return TurretMain;
                default:
                    Debug.LogError($"A gameobject does not exists for throwable {throwableType} or it has not been added here.");
                    return null;
            }
        }

        private GunController GetGunControllerForGun(InventoryItem gunType)
        {
            switch (gunType)
            {
                case InventoryItem.NoGun: return null;
                case InventoryItem.Pistol: return PistolControllerScript;
                case InventoryItem.SMG: return SMGControllerScript;
                case InventoryItem.AR: return ARControllerScript;
                case InventoryItem.Shotgun: return ShotgunControllerScript;
                case InventoryItem.LMG: return LMGControllerScript;
                default:
                    Debug.LogError($"A gun controller script does not exists for gun {gunType} or it has not been added here.");
                    return null;
            }
        }

        private MeleeController GetMeleeControllerForMelee(InventoryItem meleeType, WeaponUtilities.WeaponSide leftOrRight)
        {
            switch (meleeType)
            {
                case InventoryItem.NoMelee: return leftOrRight == WeaponUtilities.WeaponSide.Left ? FistLeftControllerScript : FistRightControllerScript;
                case InventoryItem.Knife: return leftOrRight == WeaponUtilities.WeaponSide.Left ? KnifeLeftControllerScript : KnifeRightControllerScript;
                case InventoryItem.Machete: return leftOrRight == WeaponUtilities.WeaponSide.Left ? MacheteLeftControllerScript : MacheteRightControllerScript;
                case InventoryItem.Katana: return leftOrRight == WeaponUtilities.WeaponSide.Left ? KatanaLeftControllerScript : KatanaRightControllerScript;
                default:
                    Debug.LogError($"A melee controller script does not exists for melee {meleeType} or it has not been added here.");
                    return null;
            }
        }

        private bool UsesIKHands(InventoryItem weaponType)
        {
            switch (weaponType)
            {
                case InventoryItem.SMG:
                case InventoryItem.AR:
                case InventoryItem.Shotgun:
                case InventoryItem.LMG:
                    return true;
                default: return false;
            }
        }

        private int GetRandomMeleeAttackStyle(InventoryItem meleeType)
        {
            switch (meleeType)
            {
                case InventoryItem.NoMelee:
                    if (EquippedGun == InventoryItem.NoGun) return Random.Range(1, 7);
                    if (WeaponUtilities.Is1HandedWeapon(EquippedGun)) return Random.Range(1, 4);
                    else return Random.Range(1, 3);
                case InventoryItem.Knife:
                    return Random.Range(1, dualWielding ? 7 : 4);
                case InventoryItem.Machete:
                case InventoryItem.Katana:
                    return Random.Range(1, dualWielding ? 15 : 8);
                default:
                    Debug.LogWarning($"No attack randoms found for weapon {meleeType.ToString()}");
                    return -1;
            }
        }
    }

    public enum AnimatorWeaponSheathLocation
    {
        Back = 0,
        Hip = 1
    }
}
