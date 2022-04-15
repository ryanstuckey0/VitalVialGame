using System.Collections.Generic;
using UnityEngine;
using ViralVial.Utilities;

namespace ViralVial.Player.Animation
{
    public class PlayerAnimationController
    {
        private IPlayer owningPlayer;
        private Animator playerAnimator;

        public PlayerAnimationController(IPlayer owningPlayer)
        {
            this.owningPlayer = owningPlayer;
            playerAnimator = owningPlayer.BasePlayerController.PlayerAnimator;

            EventManager.Instance.SubscribeToEvent("MovementSpeedChange", OnMovementSpeedChange);
            EventManager.Instance.SubscribeToEvent("ReloadSpeedMultiplierChange", OnReloadSpeedMultiplierChange);
            EventManager.Instance.SubscribeToEvent("MeleeSpeedMultiplierChange", OnMeleeSpeedMultiplierChange);
            EventManager.Instance.SubscribeToEvent("DashSpeedMultiplierChange", OnDashSpeedMultiplierChange);
        }

        public void OnDestroy()
        {
            EventManager.Instance.UnsubscribeFromEvent("MovementSpeedChange", OnMovementSpeedChange);
            EventManager.Instance.UnsubscribeFromEvent("ReloadSpeedMultiplierChange", OnReloadSpeedMultiplierChange);
            EventManager.Instance.UnsubscribeFromEvent("MeleeSpeedMultiplierChange", OnMeleeSpeedMultiplierChange);
            EventManager.Instance.UnsubscribeFromEvent("DashSpeedMultiplierChange", OnDashSpeedMultiplierChange);
        }

        private void OnMovementSpeedChange(Dictionary<string, object> args)
        {
            playerAnimator.SetFloat("MovementSpeedFloat", (float)args["newValue"]);
        }

        private void OnReloadSpeedMultiplierChange(Dictionary<string, object> args)
        {
            playerAnimator.SetFloat("ReloadSpeedFloat", playerAnimator.GetFloat("ReloadSpeedFloat") + (float)args["newValue"]);
        }

        private void OnMeleeSpeedMultiplierChange(Dictionary<string, object> args)
        {
            playerAnimator.SetFloat("MeleeSpeedFloat", playerAnimator.GetFloat("MeleeSpeedFloat") + (float)args["newValue"]);
        }

        private void OnDashSpeedMultiplierChange(Dictionary<string, object> args)
        {
            playerAnimator.SetFloat("DashSpeedFloat", playerAnimator.GetFloat("DashSpeedFloat") + (float)args["newValue"]);
        }
    }
}