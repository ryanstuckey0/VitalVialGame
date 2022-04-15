using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViralVial.Player;
using ViralVial.Player.TechTreeCode;
using ViralVial.Utilities;

namespace ViralVial.Ability.Supernatural.TimeFreeze
{
    /// <summary>
    /// Fires off the TimeFreeze event. Designed to be used by player to freeze projectiles and enemies.
    /// </summary>
    class TimeFreezeAbility : IAbility
    {
        public float AbilityDuration { get; set; } = 5f;
        public IPlayer OwningPlayer { get; set; }
        public AbilityType AbilityType { get; } = AbilityType.Supernatural;

        private float playerSpeedMultiplier = 1f;
        private string[] affectedEnemies;
        private PermittedActionsState abilityPermittedActionsState;
        private bool spawnedTimeFreeze = false;

        private CoroutineRunner useAbilityCoroutine;
        private CoroutineRunner activateAbilityCoroutine;

        public TimeFreezeAbility()
        {
            abilityPermittedActionsState = new PermittedActionsState
            {
                Move = true,
                MoveAim = true
            };

            EventManager.Instance.SubscribeToEvent("ActivateTimeFreezeAction", OnActivateTimeFreezeAction);

            useAbilityCoroutine = new CoroutineRunner();
            activateAbilityCoroutine = new CoroutineRunner();
        }

        public void OnDestroy()
        {
            EventManager.Instance.UnsubscribeFromEvent("ActivateTimeFreezeAction", OnActivateTimeFreezeAction);
        }

        public bool UseAbility()
        {
            useAbilityCoroutine.Start(UseAbilityCoroutine());
            return true;
        }

        public void UpgradeAbility(AbilityLevel abilityUpgrade)
        {
            float tempFloat = 0.0f;
            if (abilityUpgrade.stats.TryGetValue("Duration", out tempFloat)) AbilityDuration = tempFloat;
            if (abilityUpgrade.stats.TryGetValue("Player.SpeedMultiplier", out tempFloat)) playerSpeedMultiplier = tempFloat;
            affectedEnemies = abilityUpgrade?.affectedEnemies ?? affectedEnemies;
        }

        private IEnumerator UseAbilityCoroutine()
        {
            OwningPlayer.PermittedActions.LoadStateAndLock(abilityPermittedActionsState);
            OwningPlayer.BasePlayerController.PlayerWeaponAnimationController.SetIKHandsEnabled(false);
            OwningPlayer.SetUpperBodyAnimatorTrigger(AnimatorTrigger.StartCastingTrigger);
            OwningPlayer.BasePlayerController.PlayerAnimator.SetInteger("AttackStyle", (int)AbilityAnimationsCodes.TimeFreezeAbility);

            spawnedTimeFreeze = false;
            yield return new WaitUntil(() => spawnedTimeFreeze);

            OwningPlayer.SetWholeBodyAnimatorTrigger(AnimatorTrigger.StopCastingTrigger);
            OwningPlayer.BasePlayerController.PlayerWeaponAnimationController.SetIKHandsEnabled(true, delay: 1);

            OwningPlayer.PermittedActions.UnlockPlayer();
            OwningPlayer.PermittedActions.ChangeAll(true);
        }

        private void OnActivateTimeFreezeAction()
        {
            spawnedTimeFreeze = true;
            Object.Destroy(Object.Instantiate(OwningPlayer.BasePlayerController.TimeFreezeAnimationPrefab, OwningPlayer.Transform.position, Quaternion.identity), 10f);
            activateAbilityCoroutine.Start(ActivateAbilityCoroutine());
        }

        private IEnumerator ActivateAbilityCoroutine()
        {
            EventManager.Instance.InvokeEvent("TimeFreezeStart");
            OwningPlayer.PlayerAttributes.MovementSpeed *= playerSpeedMultiplier;
            yield return new WaitForSeconds(AbilityDuration);
            EventManager.Instance.InvokeEvent("TimeFreezeEnd");
            OwningPlayer.PlayerAttributes.MovementSpeed /= playerSpeedMultiplier;
        }
    }
}
