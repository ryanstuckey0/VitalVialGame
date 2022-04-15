using UnityEngine;
using UnityEngine.InputSystem;
using ViralVial.Player;

namespace ViralVial.Ability.Supernatural.ElementalAttack
{
    public class ElementalAttackInputController : MonoBehaviour
    {
        public bool Firing { get; private set; }
        private FireAttackAbility fireAttackAbility;
        private InputActionMap savedActiveActionMap;
        private IPlayer owningPlayer;

        public void Init(IPlayer owningPlayer, FireAttackAbility fireAttackAbility)
        {
            this.owningPlayer = owningPlayer;
            this.fireAttackAbility = fireAttackAbility;

            savedActiveActionMap = owningPlayer.BasePlayerController.PlayerInputController.CurrentActionMap;

            owningPlayer.BasePlayerController.PlayerInput.AbilitiesElementalAttack.Fire.started += Fire_started;
            owningPlayer.BasePlayerController.PlayerInput.AbilitiesElementalAttack.Fire.canceled += Fire_cancelled;
            owningPlayer.BasePlayerController.PlayerInput.AbilitiesElementalAttack.CancelFire.performed += CancelFire_performed;

            owningPlayer.BasePlayerController.PlayerInputController.SwitchActionMap(owningPlayer.BasePlayerController.PlayerInput.AbilitiesElementalAttack);
        }

        private void Fire_started(InputAction.CallbackContext callbackContext)
        {
            Firing = true;
        }

        private void Fire_cancelled(InputAction.CallbackContext callbackContext)
        {
            Firing = false;
        }

        private void CancelFire_performed(InputAction.CallbackContext callbackContext)
        {
            fireAttackAbility.CancelAbility();
        }

        private void OnDestroy()
        {
            owningPlayer.BasePlayerController.PlayerInputController.SwitchActionMap(savedActiveActionMap);

            owningPlayer.BasePlayerController.PlayerInput.AbilitiesElementalAttack.Fire.started -= Fire_started;
            owningPlayer.BasePlayerController.PlayerInput.AbilitiesElementalAttack.Fire.canceled -= Fire_cancelled;
            owningPlayer.BasePlayerController.PlayerInput.AbilitiesElementalAttack.CancelFire.performed -= CancelFire_performed;
        }
    }
}
