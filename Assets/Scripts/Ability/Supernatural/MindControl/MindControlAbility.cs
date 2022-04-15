using System.Collections;
using System.Collections.Generic;
using ViralVial.Player;
using ViralVial.Player.TechTreeCode;
using ViralVial.Utilities;
using UnityEngine;

namespace ViralVial.Ability.Supernatural.MindControl
{
    public class MindControlAbility : IAbility
    {
        public IPlayer OwningPlayer { get; set; }
        public AbilityType AbilityType { get; } = AbilityType.Supernatural;


        private float abilityDuration;
        private float radius;
        private float timeout;
        private const float timeDelayUntilAbilityActivation = 0.5f;
        private bool waitingForPlayerToSelectEnemies;
        private bool selectionTimedOut;
        private Vector3 originOfMindControl;
        private float enemyHealthMultiplier = 1;
        private float enemyDamageMultiplier = 1;

        private PermittedActionsState abilityPermittedActionsState;

        private CoroutineRunner mindControlCoroutine;
        private CoroutineRunner abilityAnimationCoroutine;

        public MindControlAbility()
        {
            abilityPermittedActionsState = new PermittedActionsState()
            {
                MoveAim = true
            };

            mindControlCoroutine = new CoroutineRunner();
            abilityAnimationCoroutine = new CoroutineRunner();
        }

        public void OnDestroy()
        {
            EventManager.Instance.UnsubscribeFromEvent("SelectMindControlEnemies", OnPlayerSelectedEnemies);
        }

        public bool UseAbility()
        {
            if (waitingForPlayerToSelectEnemies) return false;
            mindControlCoroutine.Start(UseAbilityCoroutine());
            return true;
        }

        public void UpgradeAbility(AbilityLevel abilityUpgrade)
        {
            float tempFloat = 0.0f;
            if (abilityUpgrade.stats.TryGetValue("Radius", out tempFloat)) radius = tempFloat;
            if (abilityUpgrade.stats.TryGetValue("Timeout", out tempFloat)) timeout = tempFloat;
            if (abilityUpgrade.stats.TryGetValue("Duration", out tempFloat)) abilityDuration = tempFloat;
            if (abilityUpgrade.stats.TryGetValue("Enemy.HealthMultiplier", out tempFloat)) enemyHealthMultiplier = tempFloat;
            if (abilityUpgrade.stats.TryGetValue("Enemy.DamageMultiplier", out tempFloat)) enemyDamageMultiplier = tempFloat;
        }

        private IEnumerator UseAbilityCoroutine()
        {
            MindControlSelectorController mindControlSelectorController = OwningPlayer.GameObject.AddComponent<MindControlSelectorController>();
            mindControlSelectorController.Init(OwningPlayer, OwningPlayer.BasePlayerController.MindControlSelectorCirclePrefab, timeout);
            mindControlSelectorController.MindControlMouseSelectorCircle.transform.localScale = new Vector3(radius, radius, radius);

            // subscribe to some event that tells us player selected an enemy group
            EventManager.Instance.SubscribeToEvent("SelectMindControlEnemies", OnPlayerSelectedEnemies);
            waitingForPlayerToSelectEnemies = true;
            abilityAnimationCoroutine.Start(ActivatePlayerAbilityAnimation(mindControlSelectorController.MindControlMouseSelectorCircle));
            while (waitingForPlayerToSelectEnemies) { yield return null; }
            EventManager.Instance.UnsubscribeFromEvent("SelectMindControlEnemies", OnPlayerSelectedEnemies);

            if (selectionTimedOut) yield break;
            yield return new WaitForSeconds(timeDelayUntilAbilityActivation);
            InvokeMindControlEvent();
        }

        private void OnPlayerSelectedEnemies(Dictionary<string, object> args)
        {
            originOfMindControl = (Vector3)args["originOfMindControl"];
            waitingForPlayerToSelectEnemies = false;
            selectionTimedOut = (bool)args["timedOut"];
        }

        private void InvokeMindControlEvent()
        {
            // start ability, passing in length, radius, and origin of mind control
            Dictionary<string, object> dictArgs = new Dictionary<string, object>()
            {
                { "abilityDuration", abilityDuration },
                { "originOfMindControl", originOfMindControl },
                { "radiusOfMindControl", radius },
                { "potentialTargetsList", new List<GameObject>() },
                { "healthMultiplier", enemyHealthMultiplier},
                { "damageMultiplier", enemyDamageMultiplier}
            };
            EventManager.Instance.InvokeEvent("MindControl", dictArgs);
        }

        private IEnumerator ActivatePlayerAbilityAnimation(GameObject mindControlSelectorCircle)
        {
            OwningPlayer.PermittedActions.LoadStateAndLock(abilityPermittedActionsState);

            OwningPlayer.BasePlayerController.PlayerWeaponAnimationController.SetIKHandsEnabled(false);
            OwningPlayer.SetWholeBodyAnimatorTrigger(AnimatorTrigger.StartCastingTrigger);
            OwningPlayer.BasePlayerController.PlayerAnimator.SetInteger("AttackStyle", (int)AbilityAnimationsCodes.MindControlAbility);

            yield return new WaitForSeconds(0.2f);

            GameObject handAnimation = Object.Instantiate(
                OwningPlayer.BasePlayerController.MindControlHandAnimationPrefab,
                OwningPlayer.BasePlayerController.PlayerAnimationGameObjects.LeftHand.transform.position,
                OwningPlayer.Transform.rotation,
                OwningPlayer.Transform
            );

            ParticleBeamDistanceController handAnimationController = handAnimation.GetComponent<ParticleBeamDistanceController>();
            while (waitingForPlayerToSelectEnemies)
            {
                handAnimationController.UpdateTargetLength(mindControlSelectorCircle.transform.position);
                handAnimation.transform.position = OwningPlayer.BasePlayerController.PlayerAnimationGameObjects.LeftHand.transform.position;
                yield return null;
            }
            Object.Destroy(handAnimation);

            OwningPlayer.SetWholeBodyAnimatorTrigger(AnimatorTrigger.StopCastingTrigger);
            OwningPlayer.BasePlayerController.PlayerWeaponAnimationController.SetIKHandsEnabled(true, delay: 1);

            OwningPlayer.PermittedActions.UnlockPlayer();
            OwningPlayer.PermittedActions.ChangeAll(true);
        }
    }
}
