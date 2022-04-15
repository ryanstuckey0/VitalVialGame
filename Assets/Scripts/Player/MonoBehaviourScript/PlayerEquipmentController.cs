using UnityEngine;
using UnityEngine.InputSystem;
using ViralVial.Utilities;

namespace ViralVial.Player.MonoBehaviourScript
{
    /// <summary>
    /// Mainly just responsible for sending input to the player's EquipmentManager class.
    /// </summary>
    public class PlayerEquipmentController : MonoBehaviour
    {
        [SerializeField] private BasePlayerController basePlayerController;

        private IPlayer owningPlayer;
        private InventoryItem[] gunTypes = new InventoryItem[6] {
            InventoryItem.NoGun,
            InventoryItem.Pistol,
            InventoryItem.SMG,
            InventoryItem.AR,
            InventoryItem.Shotgun,
            InventoryItem.LMG
        };

        private int equippedGun = 0;

        private void Start()
        {
            owningPlayer = basePlayerController.OwningPlayer;
        }

        public void UseAbilitySlot1_performed(InputAction.CallbackContext callbackContext)
        {
            if (!owningPlayer.PermittedActions.UseHotbarSlots) return;
            owningPlayer.EquipmentManager.UseAbilitySlot(1);
        }

        public void UseAbilitySlot2_performed(InputAction.CallbackContext callbackContext)
        {
            if (!owningPlayer.PermittedActions.UseHotbarSlots) return;
            owningPlayer.EquipmentManager.UseAbilitySlot(2);
        }

        public void UseAbilitySlot3_performed(InputAction.CallbackContext callbackContext)
        {
            if (!owningPlayer.PermittedActions.UseHotbarSlots) return;
            owningPlayer.EquipmentManager.UseAbilitySlot(3);
        }

        public void UseAbilitySlot4_performed(InputAction.CallbackContext callbackContext)
        {
            if (!owningPlayer.PermittedActions.UseHotbarSlots) return;
            owningPlayer.EquipmentManager.UseAbilitySlot(4);
        }

        public void Fire_started(InputAction.CallbackContext callbackContext)
        {
            if (!owningPlayer.PermittedActions.Fire) return;
            owningPlayer.EquipmentManager.PressGunTrigger();
        }

        public void Fire_canceled(InputAction.CallbackContext callbackContext)
        {
            owningPlayer.EquipmentManager.ReleaseGunTrigger();
        }

        public void Melee_performed(InputAction.CallbackContext callbackContext)
        {
            if (!owningPlayer.PermittedActions.Melee) return;
            owningPlayer.EquipmentManager.UseMelee();
        }

        internal void Interact_performed(InputAction.CallbackContext callbackContext)
        {
            EventManager.Instance.InvokeEvent("InteractPerformed");
        }

        public void UseThrowable_performed(InputAction.CallbackContext callbackContext)
        {
            if (!owningPlayer.PermittedActions.UseThrowable) return;
            owningPlayer.EquipmentManager.ThrowThrowable();
        }

        public void Reload_performed(InputAction.CallbackContext callbackContext)
        {
            if (!owningPlayer.PermittedActions.Reload) return;
            owningPlayer.EquipmentManager.Reload();
        }

        public void SwitchGun_performed(InputAction.CallbackContext callbackContext)
        {
            if (!owningPlayer.PermittedActions.SwitchGuns) return;
            float input = callbackContext.ReadValue<float>();
            if (input == 0) return;

            InventoryItem?[] unlockedGuns = owningPlayer.Inventory.UnlockedGuns;
            int newGun = equippedGun;
            do
            {
                if (input > 0) newGun = newGun < unlockedGuns.Length - 1 ? newGun + 1 : 0;
                else if (input < 0) newGun = newGun > 0 ? newGun - 1 : unlockedGuns.Length - 1;
            } while (unlockedGuns[newGun] == null);
            if (newGun == equippedGun) return;

            equippedGun = newGun;
            owningPlayer.EquipmentManager.EquipGun(gunTypes[equippedGun]);
        }
    }
}
