using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViralVial.Player;
using ViralVial.Player.TechTreeCode;
using ViralVial.Utilities;

namespace ViralVial.Ability.Supernatural.ShockWave
{
    public class ShockWaveAbility : IAbility
    {
        public IPlayer OwningPlayer { get; set; }
        public AbilityType AbilityType { get; } = AbilityType.Supernatural;

        private string[] affectedEnemies;
        private float range = 0;
        private float damage = 0;
        private float pushSpeed = 25;
        private bool spawnedShockWave = false;
        private PermittedActionsState shockWaveAnimationPermittedActions;

        private CoroutineRunner useAbilityCoroutine;

        public ShockWaveAbility()
        {
            shockWaveAnimationPermittedActions = new PermittedActionsState
            {
                Move = true,
                MoveAim = true
            };

            useAbilityCoroutine = new CoroutineRunner();

            EventManager.Instance.SubscribeToEvent("SpawnShockWaveAnimation", OnSpawnShockWaveAnimation);
        }

        public void OnDestroy()
        {
            EventManager.Instance.UnsubscribeFromEvent("SpawnShockWaveAnimation", OnSpawnShockWaveAnimation);
        }

        public bool UseAbility()
        {
            useAbilityCoroutine.Start(UseAbilityCoroutine());
            return true;
        }

        public void UpgradeAbility(AbilityLevel abilityUpgrade)
        {
            float tempFloat = 0.0f;
            if (abilityUpgrade.stats.TryGetValue("Range", out tempFloat)) range = tempFloat;
            if (abilityUpgrade.stats.TryGetValue("Damage", out tempFloat)) damage = tempFloat;
            affectedEnemies = abilityUpgrade?.affectedEnemies ?? affectedEnemies;
        }

        private IEnumerator UseAbilityCoroutine()
        {
            OwningPlayer.PermittedActions.LoadStateAndLock(shockWaveAnimationPermittedActions);

            OwningPlayer.BasePlayerController.PlayerWeaponAnimationController.SetIKHandsEnabled(false);
            OwningPlayer.SetUpperBodyAnimatorTrigger(AnimatorTrigger.StartCastingTrigger);
            OwningPlayer.BasePlayerController.PlayerAnimator.SetInteger("AttackStyle", (int)AbilityAnimationsCodes.ShockWaveAbility);
            spawnedShockWave = false;
            yield return new WaitUntil(() => spawnedShockWave);
            OwningPlayer.BasePlayerController.PlayerWeaponAnimationController.SetIKHandsEnabled(true, delay: 1);

            OwningPlayer.PermittedActions.UnlockPlayer();
            OwningPlayer.PermittedActions.ChangeAll(true);
        }

        private void OnSpawnShockWaveAnimation()
        {
            spawnedShockWave = true;
            GameObject shockWaveAnim = Object.Instantiate(OwningPlayer.BasePlayerController.ShockWaveAnimationPrefab, OwningPlayer.Transform.position, Quaternion.identity);

            RaycastHit hitInfo;
            if (Physics.Raycast(OwningPlayer.Transform.position + 2 * OwningPlayer.Transform.up, -OwningPlayer.Transform.up, out hitInfo, 5f, LayerMask.GetMask(Constants.GroundLayerName))) shockWaveAnim.transform.up = hitInfo.normal;
            Object.Destroy(shockWaveAnim, 8f);

            PlayerUtilities.SpawnShockWave(OwningPlayer, range, damage, pushSpeed);
        }
    }
}
