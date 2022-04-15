using UnityEngine;
using ViralVial.Player.Animation;
using ViralVial.Weapons;

namespace ViralVial.Player.MonoBehaviourScript.DebugScript
{
    public class DebugWeaponSelectorController : MonoBehaviour
    {
        private PlayerWeaponAnimationController weaponAnimationController;
        private InventoryItem[] guns = new InventoryItem[] {
            InventoryItem.NoGun,
            InventoryItem.Pistol,
            InventoryItem.SMG,
            InventoryItem.AR,
            InventoryItem.Shotgun,
            InventoryItem.LMG
        };

        private int equippedGun = 0;

        private InventoryItem[] melees = new InventoryItem[] {
            InventoryItem.NoMelee,
            InventoryItem.Knife,
            InventoryItem.Machete,
            InventoryItem.Katana
        };

        private int equippedMelee = 0;

        private IPlayer player;

        private void Start()
        {
        }

        private void OnGetUnarmedGun()
        {
            player.EquipmentManager.EquipGun(InventoryItem.NoGun);
        }

        private void OnGetUnarmedMelee()
        {
            player.EquipmentManager.EquipMelee(InventoryItem.NoMelee);
        }

        private void OnGetNoMelee()
        {
            player.EquipmentManager.EquipMelee(InventoryItem.NoMelee);
        }

        private void OnGetKnife()
        {
            player.EquipmentManager.EquipMelee(InventoryItem.Knife);
        }

        private void OnGetMachete()
        {
            player.EquipmentManager.EquipMelee(InventoryItem.Machete);
        }

        private void OnGetKatana()
        {
            player.EquipmentManager.EquipMelee(InventoryItem.Katana);
        }

        private void OnGetPistol()
        {
            player.EquipmentManager.EquipGun(InventoryItem.Pistol);
        }

        private void OnGetSMG()
        {
            player.EquipmentManager.EquipGun(InventoryItem.SMG);
        }

        private void OnGetAR()
        {
            player.EquipmentManager.EquipGun(InventoryItem.AR);
        }

        private void OnGetShotgun()
        {
            player.EquipmentManager.EquipGun(InventoryItem.Shotgun);
        }

        private void OnGetLMG()
        {
            player.EquipmentManager.EquipGun(InventoryItem.LMG);
        }

        private void OnCycleGun()
        {
            equippedGun = equippedGun < guns.Length - 1 ? equippedGun + 1 : 0;
            player.EquipmentManager.EquipGun(guns[equippedGun]);
        }

        private void OnCycleMelee() 
        {
            equippedMelee = equippedMelee < melees.Length - 1 ? equippedMelee + 1 : 0;
            player.EquipmentManager.EquipMelee(melees[equippedMelee]);
        }

        private void OnDeath() {
            player.BasePlayerController.Health -= 1000;
        }
    }
}
