using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using ViralVial.Player.Animation;
using ViralVial.Player.TechTreeCode;
using ViralVial.SaveSystem;
using ViralVial.Weapons;
using ViralVial.Utilities;

namespace ViralVial.Player
{
    /// <summary>
    /// Manages currently equipped tools and abilities for player.
    /// Responds to events fired off by PlayerController including using 
    /// hotbar abilities, switching active throwable, using active throwable, firing ranged weapon, 
    /// and using melee weapon.
    /// </summary>
    public class EquipmentManager
    {
        public AbilitySlot[] HotbarAbilities { get; private set; }

        public InventoryItem EquippedGun { get => weaponAnimationController.EquippedGun; }
        public InventoryItem EquippedMelee { get => weaponAnimationController.EquippedMelee; }
        public InventoryItem EquippedThrowable { get => weaponAnimationController.EquippedThrowable; }
        public GameObject EquippedGunGameObject { get => weaponAnimationController.ActiveGunGameObject; }
        private InventoryItem equippedThrowable { get => weaponAnimationController.EquippedThrowable; }
        private IPlayer owner;
        private PlayerWeaponAnimationController weaponAnimationController;

        public GunController Pistol => weaponAnimationController.PistolControllerScript;
        public GunController SMG => weaponAnimationController.SMGControllerScript;
        public GunController AR => weaponAnimationController.ARControllerScript;
        public GunController Shotgun => weaponAnimationController.ShotgunControllerScript;
        public GunController LMG => weaponAnimationController.LMGControllerScript;

        public EquipmentManager(IPlayer owningPlayer)
        {
            owner = owningPlayer;
            weaponAnimationController = owningPlayer.BasePlayerController.PlayerWeaponAnimationController;
            HotbarAbilities = new AbilitySlot[4];
        }

        public void OnDestroy() { }

        public PlayerLoadout RetrievePlayerLoadout()
        {

            string[] equippedAbilities = new string[HotbarAbilities.Length];
            for (int i = 0; i < HotbarAbilities.Length; i++)
                equippedAbilities[i] = HotbarAbilities[i]?.Id ?? null;

            return new PlayerLoadout()
            {
                Gun = EquippedGun,
                Melee = EquippedMelee,
                Throwable = equippedThrowable,
                HotbarAbilities = equippedAbilities
            };
        }

        public void Initialize(PlayerLoadout loadout)
        {
            for (int i = 0; i < HotbarAbilities.Length; i++)
            {
                if (loadout.HotbarAbilities[i] == null) continue;
                AssignAbilityToSlot(loadout.HotbarAbilities[i], i + 1);
            }

            EquipGun(loadout.Gun);
            EquipMelee(loadout.Melee);
            EquipThrowable(loadout.Throwable);
        }

        // Equipment Functions ------------------------------------------------

        public IEnumerator SheathAllWeaponsCoroutine()
        {
            yield return EquipGunCoroutine(InventoryItem.NoGun);
            EquipMelee(InventoryItem.NoMelee);
        }

        public bool EquipGun(InventoryItem gunType)
        {
            weaponAnimationController.SwitchGun(gunType);
            return true;
        }

        public IEnumerator EquipGunCoroutine(InventoryItem gunType)
        {
            yield return weaponAnimationController.SwitchGunCoroutineUtility(gunType);
        }

        public void PressGunTrigger()
        {
            weaponAnimationController.PressGunTrigger();
        }

        public void ReleaseGunTrigger()
        {
            weaponAnimationController.ReleaseGunTrigger();
        }

        public void EquipMelee(InventoryItem meleeType)
        {
            EventManager.Instance.InvokeEvent("HotbarAbilityAssign",
                                                new Dictionary<string, object>() {
                                                    { "assignedItemName", meleeType.ToString() },
                                                    { "assignedItemId", null },
                                                    { "slotId", "M" }
                                                });
            weaponAnimationController.SwitchMelee(meleeType);
        }

        public void UseMelee()
        {
            weaponAnimationController.UseMelee();
        }

        public void EquipThrowable(InventoryItem throwableType)
        {
            EventManager.Instance.InvokeEvent("HotbarAbilityAssign",
                new Dictionary<string, object>() {
                    { "assignedItemName", throwableType.ToString() },
                    { "assignedItemId", null },
                    { "slotId", "T" }
                }
            );

            weaponAnimationController.SwitchThrowable(throwableType);
        }

        public void ThrowThrowable()
        {
            weaponAnimationController.UseThrowable();
        }

        public void Reload()
        {
            weaponAnimationController.ReloadGun();
        }

        public void StartDualWield()
        {
            weaponAnimationController.StartDualWield();
        }

        public void EndDualWield()
        {
            weaponAnimationController.EndDualWield();
        }

        // Hotbar Functions ---------------------------------------------------

        /// <summary>
        /// Uses ability in ability slot.
        /// </summary>
        /// <param name="slotNumber">ability slot to use, any number 1-4</param>
        public void UseAbilitySlot(int slotNumber)
        {
            HotbarAbilities[slotNumber - 1]?.UseAbility(slotNumber);
        }

        /// <summary>
        /// Assigns an ability to desired slot. Fails if current ability in slot is cooling down or if ability is already equipped.
        /// </summary>
        /// <param name="abilityToAssign">ability to assign</param>
        /// <param name="slotNumber">slot to assign ability to, any number 1-4</param>
        /// <returns>true if successful, else false</returns>
        public bool AssignAbilityToSlot(AbilitySlot abilityToAssign, int slotNumber)
        {
            if (!CanEquipToSlot(abilityToAssign, slotNumber)) return false;

            int? equippedSlot = AbilityIsCurrentlyEquipped(abilityToAssign);
            if (equippedSlot != null)
            {
                HotbarAbilities[(int)equippedSlot - 1] = null;
                EventManager.Instance.InvokeEvent("HotbarAbilityAssign",
                                                    new Dictionary<string, object>() {
                                                    { "assignedItemName", null },
                                                    { "assignedItemId", null },
                                                    { "slotId", equippedSlot.ToString() }
                                                    });
            }

            HotbarAbilities[slotNumber - 1] = abilityToAssign;
            EventManager.Instance.InvokeEvent("HotbarAbilityAssign",
                                                new Dictionary<string, object>() {
                                                    { "assignedItemName", abilityToAssign.AbilityName },
                                                    { "assignedItemId", abilityToAssign.Id },
                                                    { "slotId", slotNumber.ToString() }
                                                });
            return true;
        }

        public bool AssignAbilityToSlot(string abilityId, int slotNumber)
        {
            AbilitySlot tempSlot;
            if (owner.TechTree.SupernaturalAbilitySlots.TryGetValue(abilityId, out tempSlot))
                return owner.EquipmentManager.AssignAbilityToSlot(tempSlot, slotNumber);
            else
            {
                Debug.Log($"Ability {abilityId} not found. Unable to assign to slot {slotNumber}");
                return false;
            }
        }

        public bool CanEquipToSlot(AbilitySlot abilityToCheck, int slotNumber)
        {
            return abilityToCheck != null
                && abilityToCheck.AbilityType == Ability.AbilityType.Supernatural
                && !abilityToCheck.Locked
                && !(AbilityIsCurrentlyEquipped(abilityToCheck) != null && abilityToCheck.CoolingDown)
                && HotbarAbilities[slotNumber - 1]?.Id != abilityToCheck.Id
                && !(HotbarAbilities[slotNumber - 1] != null && AbilityIsCoolingDown(slotNumber));
        }

        public bool CanEquipToSlot(string abilityId, int slotNumber)
        {
            return CanEquipToSlot(GetAbilitySlotFromId(abilityId), slotNumber);
        }

        public bool CanEquipSlot(int slotNumber)
        {
            return !HotbarAbilities[slotNumber - 1]?.CoolingDown ?? false;
        }

        private AbilitySlot GetAbilitySlotFromId(string abilityId)
        {
            AbilitySlot tempSlot = null;
            owner.TechTree.SupernaturalAbilitySlots.TryGetValue(abilityId, out tempSlot);
            if (tempSlot == null) owner.TechTree.HumanAbilitySlots.TryGetValue(abilityId, out tempSlot);
            if (tempSlot == null) Debug.Log($"Ability {abilityId} not found.");
            return tempSlot;
        }

        public int? AbilityIsCurrentlyEquipped(AbilitySlot abilityToAssign)
        {
            for (int i = 0; i < HotbarAbilities.Length; i++)
            {
                if (HotbarAbilities[i] == null) continue;
                if (string.Equals(HotbarAbilities[i].Id, abilityToAssign.Id)) return i + 1;
            }
            return null;
        }

        public bool AbilityIsCoolingDown(int slotNumber)
        {
            return HotbarAbilities[slotNumber - 1].CoolingDown;
        }

        public GunController GetGun(InventoryItem gunType)
        {
            switch (gunType)
            {
                case InventoryItem.Pistol: return Pistol;
                case InventoryItem.SMG: return SMG;
                case InventoryItem.AR: return AR;
                case InventoryItem.Shotgun: return Shotgun;
                case InventoryItem.LMG: return LMG;
                default:
                    Debug.LogWarning($"No gun found in EquipmentManager.GetGun for gun type {gunType}.");
                    return null;
            }
        }
    }
}
