using System.Collections;
using UnityEngine;
using ViralVial.Player;
using ViralVial.Player.TechTreeCode;
using ViralVial.Utilities;

namespace ViralVial.Ability.Supernatural.ElementalAttack
{
    public class FireAttackAbility : IAbility
    {
        public IPlayer OwningPlayer { get; set; }
        public AbilityType AbilityType { get; } = AbilityType.Supernatural;

        private float duration;
        private float damageIntervalTime;
        private float damagePerInterval;
        private float burnTime;
        private float rateOfFire;

        private bool abilityIsRunning = false;
        private float timeBetweenShots;
        private PermittedActionsState abilityActivePermittedActionsState;
        private PermittedActionsState firingPermittedActionsState;
        private InventoryItem equippedGun;
        private InventoryItem equippedMelee;

        private CoroutineRunner timerCoroutine;
        private CoroutineRunner useAbilityCoroutine;

        // Fire Spreading
        private bool fireSpreads = false;
        private float spreadRange;
        private float spreadChance;

        public FireAttackAbility()
        {
            abilityActivePermittedActionsState = new PermittedActionsState()
            {
                UseHotbarSlots = true,
                Move = true,
                MoveAim = true,
                Dash = true
            };
            firingPermittedActionsState = new PermittedActionsState
            {
                MoveAim = true
            };

            timerCoroutine = new CoroutineRunner();
            useAbilityCoroutine = new CoroutineRunner();
        }

        public void OnDestroy() { }

        public bool UseAbility()
        {
            if (abilityIsRunning) return false;
            useAbilityCoroutine.Start(UseAbilityCoroutine());
            return true;
        }

        public void UpgradeAbility(AbilityLevel abilityUpgrade)
        {
            float tempFloat = 0.0f;
            if (abilityUpgrade.stats.TryGetValue("Duration", out tempFloat)) duration = tempFloat;
            if (abilityUpgrade.stats.TryGetValue("DamageIntervalTime", out tempFloat)) damageIntervalTime = tempFloat;
            if (abilityUpgrade.stats.TryGetValue("DamagePerInterval", out tempFloat)) damagePerInterval = tempFloat;
            if (abilityUpgrade.stats.TryGetValue("BurnTime", out tempFloat)) burnTime = tempFloat;
            if (abilityUpgrade.stats.TryGetValue("RateOfFire", out tempFloat))
            {
                rateOfFire = tempFloat;
                timeBetweenShots = 1f / rateOfFire;
            }

            if (abilityUpgrade.effect == "fire-spreads")
            {
                fireSpreads = true;
                spreadRange = abilityUpgrade.stats["SpreadRange"];
                spreadChance = abilityUpgrade.stats["SpreadChance"];
            }
        }

        private IEnumerator UseAbilityCoroutine()
        {
            abilityIsRunning = true;

            equippedGun = OwningPlayer.EquipmentManager.EquippedGun;
            equippedMelee = OwningPlayer.EquipmentManager.EquippedMelee;
            yield return OwningPlayer.EquipmentManager.SheathAllWeaponsCoroutine();

            OwningPlayer.PermittedActions.LoadStateAndLock(abilityActivePermittedActionsState);

            // spawn fire on hand
            GameObject leftHandFire = Object.Instantiate(
                OwningPlayer.BasePlayerController.FireAttackHandPrefab,
                OwningPlayer.BasePlayerController.PlayerAnimationGameObjects.LeftHand.transform.position,
                OwningPlayer.BasePlayerController.PlayerAnimationGameObjects.LeftHand.transform.rotation,
                OwningPlayer.BasePlayerController.PlayerAnimationGameObjects.LeftHand.transform);
            GameObject rightHandFire = Object.Instantiate(
                OwningPlayer.BasePlayerController.FireAttackHandPrefab,
                OwningPlayer.BasePlayerController.PlayerAnimationGameObjects.RightHand.transform.position,
                OwningPlayer.BasePlayerController.PlayerAnimationGameObjects.RightHand.transform.rotation,
                OwningPlayer.BasePlayerController.PlayerAnimationGameObjects.RightHand.transform);

            ElementalAttackInputController triggerControllerScript = OwningPlayer.GameObject.AddComponent<ElementalAttackInputController>();
            triggerControllerScript.Init(OwningPlayer, this);
            timerCoroutine.Start(AbilityTimerCoroutine());

            while (abilityIsRunning)
            {
                if (triggerControllerScript.Firing)
                {
                    OwningPlayer.PermittedActions.UnlockPlayer();
                    OwningPlayer.PermittedActions.LoadStateAndLock(firingPermittedActionsState);

                    OwningPlayer.BasePlayerController.PlayerWeaponAnimationController.SetIKHandsEnabled(false);
                    OwningPlayer.SetWholeBodyAnimatorTrigger(AnimatorTrigger.StartCastingTrigger);
                    OwningPlayer.BasePlayerController.PlayerAnimator.SetInteger("AttackStyle", (int)AbilityAnimationsCodes.FireAttackAbility);

                    yield return new WaitForSeconds(0.5f);

                    while (abilityIsRunning && triggerControllerScript.Firing)
                    {
                        GameObject fireball = Object.Instantiate(
                            OwningPlayer.BasePlayerController.FireAttackPrefab,
                            OwningPlayer.BasePlayerController.PlayerAnimationGameObjects.LeftHandProjectileSpawnPoint.transform.position + OwningPlayer.Transform.forward,
                            Quaternion.identity
                        );
                        fireball.transform.forward = OwningPlayer.Transform.forward;
                        fireball.GetComponentInChildren<FireballCollisionController>().Init(
                            burnTime,
                            damageIntervalTime,
                            damagePerInterval,
                            fireSpreads,
                            spreadRange,
                            spreadChance);
                        Object.Destroy(fireball, 1f);
                        yield return new WaitForSeconds(timeBetweenShots);
                    }

                    OwningPlayer.PermittedActions.UnlockPlayer();
                    OwningPlayer.PermittedActions.LoadStateAndLock(abilityActivePermittedActionsState);

                    OwningPlayer.SetWholeBodyAnimatorTrigger(AnimatorTrigger.StopCastingTrigger);
                    OwningPlayer.BasePlayerController.PlayerWeaponAnimationController.SetIKHandsEnabled(true, delay: 1);
                }

                yield return null;
            }

            OwningPlayer.PermittedActions.UnlockPlayer();
            OwningPlayer.PermittedActions.ChangeAll(true);

            yield return OwningPlayer.EquipmentManager.EquipGunCoroutine(equippedGun);
            OwningPlayer.EquipmentManager.EquipMelee(equippedMelee);

            Object.Destroy(triggerControllerScript);
            Object.Destroy(leftHandFire);
            Object.Destroy(rightHandFire);
        }

        private IEnumerator AbilityTimerCoroutine()
        {
            yield return new WaitForSeconds(duration);
            abilityIsRunning = false;
        }

        public void CancelAbility()
        {
            timerCoroutine.Stop();
            abilityIsRunning = false;
        }
    }
}