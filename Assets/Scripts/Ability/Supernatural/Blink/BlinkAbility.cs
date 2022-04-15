using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using ViralVial.Player;
using ViralVial.Player.TechTreeCode;
using ViralVial.Utilities;

namespace ViralVial.Ability.Supernatural.Blink
{
    public class BlinkAbility : IAbility
    {
        public IPlayer OwningPlayer { get; set; }
        public AbilityType AbilityType { get; } = AbilityType.Supernatural;

        private float range;
        private const float speed = 200f;
        private const float raycastYOffset = 20f;
        private string[] layersToCheck = new string[] { Constants.GroundLayerName, Constants.CollisionNoNav, Constants.CollisionNotGround };
        private PermittedActionsState abilityPermittedActions;
        private bool completeBlinkAnimation = false;

        private CoroutineRunner blinkCoroutine;

        // double blink upgrade
        private bool doubleBlinkUnlocked = false;
        private bool canBlinkAgain = false;

        // shock wave upgrade
        private bool shockWaveUnlocked = false;
        private float shockWaveDamage;
        private float shockWaveRange;


        public BlinkAbility()
        {
            abilityPermittedActions = new PermittedActionsState
            {
                Move = true,
                MoveAim = true
            };

            EventManager.Instance.SubscribeToEvent("ActivateBlinkMotion", OnActivateBlinkMotion);
            blinkCoroutine = new CoroutineRunner();
        }

        public void OnDestroy()
        {
            EventManager.Instance.UnsubscribeFromEvent("ActivateBlinkMotion", OnActivateBlinkMotion);
        }

        public bool UseAbility()
        {
            blinkCoroutine.Restart(UseAbilityCoroutine());
            canBlinkAgain = !canBlinkAgain;
            return !canBlinkAgain || !doubleBlinkUnlocked;
        }

        public void UpgradeAbility(AbilityLevel abilityUpgrade)
        {
            float tempFloat = 0.0f;
            if (abilityUpgrade.stats.TryGetValue("Range", out tempFloat)) range = tempFloat;
            if (abilityUpgrade.effect != null)
            {
                switch (abilityUpgrade.effect)
                {
                    case "double-blink":
                        doubleBlinkUnlocked = true;
                        canBlinkAgain = false;
                        break;
                    case "post-blink-shock-wave":
                        shockWaveUnlocked = true;
                        shockWaveDamage = abilityUpgrade.stats["damage"];
                        shockWaveRange = abilityUpgrade.stats["range"];
                        break;
                }
            }
        }

        private IEnumerator UseAbilityCoroutine()
        {
            OwningPlayer.PermittedActions.LoadStateAndLock(abilityPermittedActions);

            OwningPlayer.BasePlayerController.PlayerWeaponAnimationController.SetIKHandsEnabled(false);
            OwningPlayer.SetUpperBodyAnimatorTrigger(AnimatorTrigger.StartCastingTrigger);
            OwningPlayer.BasePlayerController.PlayerAnimator.SetInteger("AttackStyle", (int)AbilityAnimationsCodes.BlinkAbility);

            completeBlinkAnimation = false;
            yield return new WaitUntil(() => completeBlinkAnimation);

            OwningPlayer.BasePlayerController.PlayerWeaponAnimationController.SetIKHandsEnabled(true, delay: 1);

            OwningPlayer.PermittedActions.UnlockPlayer();
            OwningPlayer.PermittedActions.ChangeAll(true);
        }

        private void OnActivateBlinkMotion()
        {
            Vector3 raycastOrigin = OwningPlayer.Transform.position + OwningPlayer.Transform.forward * range + new Vector3(0, raycastYOffset, 0);
            RaycastHit hitInfo;
            if (Physics.Raycast(raycastOrigin, Vector3.down, out hitInfo, Mathf.Infinity, LayerMask.GetMask(layersToCheck)))
            {
                Vector3 travelDirection = (hitInfo.point - OwningPlayer.Transform.position).normalized;

                float maxDistance = range;
                if (Physics.BoxCast(OwningPlayer.Transform.position + new Vector3(0, 1, 0), new Vector3(0.5f, 0.5f, 0.5f), travelDirection, out hitInfo, Quaternion.identity, 2 + range, LayerMask.GetMask(layersToCheck)))
                    maxDistance = hitInfo.distance - 1f;

                GameObject animation = GameObject.Instantiate(OwningPlayer.BasePlayerController.BlinkAnimationPrefab, OwningPlayer.Transform.position + OwningPlayer.Transform.forward * range / 10 + new Vector3(0, 2, 0), OwningPlayer.Transform.rotation);
                animation.GetComponent<RFX4_EffectSettings>().Speed = range * 6;
                animation.transform.forward = travelDirection;
                GameObject.Destroy(animation, 2.5f);

                OwningPlayer.Transform.position += travelDirection * maxDistance;
                if (shockWaveUnlocked)
                    PlayerUtilities.SpawnShockWave(OwningPlayer, shockWaveRange, shockWaveDamage, 25);
            }
            completeBlinkAnimation = true;
        }
    }
}