using UnityEngine;
using UnityEngine.InputSystem;
using ViralVial.Player.Animation;

namespace ViralVial.Player.MonoBehaviourScript
{
    public class TurretPlacementInputController : MonoBehaviour
    {

        private PlayerWeaponAnimationController playerWeaponAnimationController;
        private TurretHolographController turretHolographController;
        private InputActionMap savedActiveActionMap;
        private IPlayer owningPlayer;

        public void Init(IPlayer owningPlayer, PlayerWeaponAnimationController playerWeaponAnimationController, TurretHolographController turretHolographController)
        {
            this.owningPlayer = owningPlayer;
            this.playerWeaponAnimationController = playerWeaponAnimationController;
            this.turretHolographController = turretHolographController;

            savedActiveActionMap = owningPlayer.BasePlayerController.PlayerInputController.CurrentActionMap;
            owningPlayer.BasePlayerController.PlayerInput.AbilitiesTurretPlacement.PlaceTurret.performed += PlaceTurret_performed;
            owningPlayer.BasePlayerController.PlayerInput.AbilitiesTurretPlacement.CancelTurret.performed += CancelTurret_performed;
            owningPlayer.BasePlayerController.PlayerInputController.SwitchActionMap(owningPlayer.BasePlayerController.PlayerInput.AbilitiesTurretPlacement);
        }

        private void PlaceTurret_performed(InputAction.CallbackContext callbackContext)
        {
            if (!turretHolographController.CanPlace) return;
            playerWeaponAnimationController.PlaceTurret();
            Destroy(this);
        }

        private void CancelTurret_performed(InputAction.CallbackContext callbackContext)
        {
            playerWeaponAnimationController.CancelTurret();
            Destroy(this);
        }

        private void OnDestroy()
        {
            owningPlayer.BasePlayerController.PlayerInputController.SwitchActionMap(owningPlayer.BasePlayerController.PlayerInput.Player);
            owningPlayer.BasePlayerController.PlayerInput.AbilitiesTurretPlacement.PlaceTurret.performed -= PlaceTurret_performed;
            owningPlayer.BasePlayerController.PlayerInput.AbilitiesTurretPlacement.CancelTurret.performed -= CancelTurret_performed;
        }
    }
}