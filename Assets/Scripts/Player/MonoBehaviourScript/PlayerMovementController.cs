using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using RPGCharacterAnims;
using ViralVial.Utilities;

namespace ViralVial.Player.MonoBehaviourScript
{
    public class PlayerMovementController : MonoBehaviour
    {
        [SerializeField] private BasePlayerController BasePlayerController;

        private RPGCharacterController playerAnimationController;
        private IPlayer owningPlayer;

        private bool dashing = false;
        private bool speedBoosting = false;
        private const float dashTimeout = 1;

        private WaitForSeconds dashTimeoutTimer;
        private PermittedActionsState dodgePermittedActions;

        private void Start()
        {
            owningPlayer = BasePlayerController.OwningPlayer;
            playerAnimationController = BasePlayerController.PlayerAnimationController;

            dashTimeoutTimer = CoroutineYielderCache.AddOrModifyYielder("DashTimeout", new WaitForSeconds(dashTimeout)) as WaitForSeconds;

            EventManager.Instance.SubscribeToEvent("PlayerDisallowMove", OnStopMovement);
            EventManager.Instance.SubscribeToEvent("Animator_FinishedDash", OnAnimator_FinishedDash);

            dodgePermittedActions = new PermittedActionsState
            {
                Move = true,
                MoveAim = true
            };
        }

        private void OnDestroy()
        {
            EventManager.Instance.UnsubscribeFromEvent("PlayerDisallowMove", OnStopMovement);
            EventManager.Instance.UnsubscribeFromEvent("Animator_FinishedDash", OnAnimator_FinishedDash);
        }

        public void Move_performed(InputAction.CallbackContext callbackContext)
        {
            Vector2 moveInput;
            if (!owningPlayer.PermittedActions.Move) moveInput = Vector2.zero;
            else moveInput = callbackContext.ReadValue<Vector2>();
            playerAnimationController.SetMoveInput(new Vector3(moveInput.x, moveInput.y, 0f));
        }

        public void Move_canceled(InputAction.CallbackContext callbackContext)
        {
            OnStopMovement();
        }

        private void OnStopMovement()
        {
            playerAnimationController.SetMoveInput(Vector3.zero);
        }

        public void Dodge_performed(InputAction.CallbackContext callbackContext)
        {
            if (!owningPlayer.PermittedActions.Dash) return;
            owningPlayer.PermittedActions.LoadStateAndLock(dodgePermittedActions);
            owningPlayer.Invincible = true;
            StartCoroutine(DodgeCoroutine());
        }

        private IEnumerator DodgeCoroutine()
        {
            yield return new WaitUntil(() => playerAnimationController.CanEndAction("Face"));
            playerAnimationController.EndAction("Face");

            owningPlayer.SetWholeBodyAnimatorTrigger(AnimatorTrigger.DiveRollTrigger);

            dashing = true;
            StartCoroutine(DashTimeoutCoroutine());
            yield return new WaitUntil(() => !dashing);
            if (owningPlayer.PlayerAttributes.PostDashSpeedBoostMultiplier > 0 && !speedBoosting)
            {
                speedBoosting = true;
                StartCoroutine(DashSpeedBoostCoroutine());
            }
            if (owningPlayer.PlayerAttributes.PostDashShockWaveEnabled)
            {
                PlayerUtilities.SpawnShockWave(
                    player: owningPlayer,
                    range: owningPlayer.PlayerAttributes.PostDashShockWaveRange,
                    damage: owningPlayer.PlayerAttributes.PostDashShockWaveDamage,
                    pushSpeed: 25);
            }

            if (owningPlayer.PlayerAttributes.PostDashInvulnerabilityTime > 0)
                StartCoroutine(DashInvincibilityCoroutine());
            else owningPlayer.Invincible = false;

            yield return new WaitUntil(() => playerAnimationController.CanStartAction("Face"));
            playerAnimationController.StartAction("Face");

            owningPlayer.PermittedActions.UnlockPlayer();
            owningPlayer.PermittedActions.ChangeAll(true);
        }

        private void OnAnimator_FinishedDash()
        {
            dashing = false;
        }

        private IEnumerator DashTimeoutCoroutine()
        {
            yield return dashTimeoutTimer;
            dashing = false;
        }

        private IEnumerator DashInvincibilityCoroutine()
        {
            yield return CoroutineYielderCache.GetYielder("PostDashInvulnerabilityTime");
            owningPlayer.Invincible = false;
        }

        private IEnumerator DashSpeedBoostCoroutine()
        {
            owningPlayer.PlayerAttributes.MovementSpeed *= owningPlayer.PlayerAttributes.PostDashSpeedBoostMultiplier;
            yield return CoroutineYielderCache.GetYielder("PostDashSpeedBoostDuration");
            owningPlayer.PlayerAttributes.MovementSpeed /= owningPlayer.PlayerAttributes.PostDashSpeedBoostMultiplier;
            speedBoosting = false;
        }
    }
}
