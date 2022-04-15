using System;
using System.Collections.Generic;
using UnityEngine;
using ViralVial.SaveSystem;
using ViralVial.Utilities;

namespace ViralVial.Player
{
    public class Inventory
    {
        public Dictionary<InventoryItem, InventorySlot> CurrentInventory;

        public InventoryItem?[] UnlockedGuns { get; private set; } = new InventoryItem?[6];

        private Dictionary<string, object> eventDictionary = new Dictionary<string, object> { { "value", null } };
        private IPlayer owningPlayer;

        public Inventory(IPlayer player)
        {
            owningPlayer = player;

            CurrentInventory = InitializeInventoryDictionary();
            EventManager.Instance.SubscribeToEvent("AddToPlayerInventory", OnAddToPlayerInventory);
            EventManager.Instance.SubscribeToEvent("AddMaxAmmo", AddMaxAmmo);
            UnlockedGuns[0] = InventoryItem.NoGun;
        }

        public void OnDestroy()
        {
            EventManager.Instance.UnsubscribeFromEvent("AddToPlayerInventory", OnAddToPlayerInventory);
            EventManager.Instance.UnsubscribeFromEvent("AddMaxAmmo", AddMaxAmmo);
        }

        public int AddToInventory(InventoryItem itemName, int amount)
        {
            if (CurrentInventory[itemName].Locked) return 0;
            InventorySlot inventorySlot = CurrentInventory[itemName];
            int takenAmount = amount;
            if (inventorySlot.MaxCapacity == -1) inventorySlot.CurrentCount += amount;
            else
            {
                // TODO: inventory capacity might be able to go below 0; debug this once we add inventory screen
                takenAmount = inventorySlot.MaxCapacity - inventorySlot.CurrentCount;
                if (takenAmount > amount) takenAmount = amount;
                inventorySlot.CurrentCount += takenAmount;
            }

            switch (inventorySlot.InventoryItem)
            {
                case InventoryItem.PistolSMGAmmo:
                case InventoryItem.ARAmmo:
                case InventoryItem.ShotgunAmmo:
                case InventoryItem.LMGAmmo:
                    UpdateHudAmmo(itemName);
                    break;
            }

            return takenAmount;
        }

        public int GetItemCount(InventoryItem inventoryItem)
        {
            return CurrentInventory[inventoryItem].CurrentCount;
        }

        public int GetItemCapacity(InventoryItem inventoryItem)
        {
            return CurrentInventory[inventoryItem].MaxCapacity;
        }

        public bool ItemIsUnlocked(InventoryItem inventoryItem)
        {
            return CurrentInventory[inventoryItem].Locked;
        }

        private void AddMaxAmmo()
        {
            RefillToMaxAmmo();
        }

        public void RefillToMaxAmmo()
        {
            RefillToMaxCapacity(InventoryItem.PistolSMGAmmo);
            RefillToMaxCapacity(InventoryItem.ARAmmo);
            RefillToMaxCapacity(InventoryItem.ShotgunAmmo);
            RefillToMaxCapacity(InventoryItem.LMGAmmo);
            UpdateHudAmmo(PlayerUtilities.GetAmmoForGunType(owningPlayer.EquipmentManager.EquippedGun));
        }

        private void UpdateHudAmmo(InventoryItem ammoType)
        {
            InventoryItem equippedAmmoType = PlayerUtilities.GetAmmoForGunType(owningPlayer.EquipmentManager.EquippedGun);
            if (equippedAmmoType == InventoryItem.NoMatch || ammoType != equippedAmmoType) return;
            EventManager.Instance.InvokeEvent("UpdateInventoryAmmo", new Dictionary<string, object> {
                {
                    "ammoInInventory",
                    CurrentInventory[ammoType].CurrentCount
                }
            });
        }

        public void UnlockItem(InventoryItem item)
        {
            switch (item)
            {
                case InventoryItem.Pistol:
                case InventoryItem.SMG:
                case InventoryItem.AR:
                case InventoryItem.Shotgun:
                case InventoryItem.LMG:
                    UnlockGun(item);
                    return;
                case InventoryItem.Knife:
                case InventoryItem.Machete:
                case InventoryItem.Katana:
                    UnlockMelee(item);
                    return;
                case InventoryItem.Rock:
                case InventoryItem.AlarmBomb:
                case InventoryItem.Grenade:
                case InventoryItem.ProximityMine:
                case InventoryItem.Turret:
                    UnlockThrowable(item);
                    return;
                default:
                    CurrentInventory[item].Locked = false;
                    return;
            }
        }

        public void ModifyMaxItemCapacity(InventoryItem itemType, int newMaxCapacity)
        {
            CurrentInventory[itemType].MaxCapacity = newMaxCapacity;
        }

        public void RefillToMaxCapacity(InventoryItem item)
        {
            if (CurrentInventory[item].Locked) return;
            CurrentInventory[item].CurrentCount = CurrentInventory[item].MaxCapacity;
        }

        // Private Utility Methods ----------------------------------------------------------------

        private void OnAddToPlayerInventory(Dictionary<string, object> dictArgs)
        {
            InventoryItem itemType;
            if (!Enum.TryParse<InventoryItem>((string)dictArgs["itemName"], true, out itemType)) return;
            if (CurrentInventory[itemType].Locked) return;
            AddToInventory(itemType, (int)dictArgs["amount"]);
        }

        public void UnlockGun(InventoryItem gun)
        {
            switch (gun)
            {
                case InventoryItem.Pistol:
                    UnlockedGuns[1] = InventoryItem.Pistol;
                    UnlockItemWithoutCheck(InventoryItem.Pistol);
                    UnlockItem(InventoryItem.PistolSMGAmmo);
                    AddToInventory(InventoryItem.PistolSMGAmmo, PlayerConstants.PistolUnlockAmmo);
                    break;
                case InventoryItem.SMG:
                    UnlockedGuns[2] = InventoryItem.SMG;
                    UnlockItemWithoutCheck(InventoryItem.SMG);
                    UnlockItem(InventoryItem.PistolSMGAmmo);
                    AddToInventory(InventoryItem.PistolSMGAmmo, PlayerConstants.SMGUnlockAmmo);
                    break;
                case InventoryItem.AR:
                    UnlockedGuns[3] = InventoryItem.AR;
                    UnlockItemWithoutCheck(InventoryItem.AR);
                    UnlockItem(InventoryItem.ARAmmo);
                    AddToInventory(InventoryItem.ARAmmo, PlayerConstants.ARUnlockAmmo);
                    break;
                case InventoryItem.Shotgun:
                    UnlockedGuns[4] = InventoryItem.Shotgun;
                    UnlockItemWithoutCheck(InventoryItem.Shotgun);
                    UnlockItem(InventoryItem.ShotgunAmmo);
                    AddToInventory(InventoryItem.ShotgunAmmo, PlayerConstants.ShotgunUnlockAmmo);
                    break;
                case InventoryItem.LMG:
                    UnlockedGuns[5] = InventoryItem.LMG;
                    UnlockItemWithoutCheck(InventoryItem.LMG);
                    UnlockItem(InventoryItem.LMGAmmo);
                    AddToInventory(InventoryItem.LMGAmmo, PlayerConstants.LMGUnlockAmmo);
                    break;
            }
        }

        private void UnlockMelee(InventoryItem meleeType)
        {
            UnlockItemWithoutCheck(meleeType);
            AddToInventory(meleeType, 1);
        }

        private void UnlockThrowable(InventoryItem throwableType)
        {
            UnlockItemWithoutCheck(throwableType);
            switch (throwableType)
            {
                case InventoryItem.Rock:
                    AddToInventory(InventoryItem.Rock, PlayerConstants.RockUnlock);
                    break;
                case InventoryItem.AlarmBomb:
                    AddToInventory(InventoryItem.AlarmBomb, PlayerConstants.AlarmBombUnlock);
                    break;
                case InventoryItem.Grenade:
                    AddToInventory(InventoryItem.Grenade, PlayerConstants.GrenadeUnlock);
                    break;
                case InventoryItem.ProximityMine:
                    AddToInventory(InventoryItem.ProximityMine, PlayerConstants.ProximityMineUnlock);
                    break;
                case InventoryItem.Turret:
                    AddToInventory(InventoryItem.Turret, PlayerConstants.TurretUnlock);
                    break;
            }
            eventDictionary["value"] = throwableType.ToString();
            EventManager.Instance.InvokeEvent("PlayerUnlockedThrowable", eventDictionary);
        }

        private void UnlockItemWithoutCheck(InventoryItem itemType)
        {
            CurrentInventory[itemType].Locked = false;
        }

        private Dictionary<InventoryItem, InventorySlot> InitializeInventoryDictionary()
        {
            Dictionary<InventoryItem, InventorySlot> inventory = new Dictionary<InventoryItem, InventorySlot>();
            inventory.AddInventorySlot(InventoryItem.PistolSMGAmmo, 200, 0, EquippableLocations.NotEquippable, true);
            inventory.AddInventorySlot(InventoryItem.ARAmmo, 400, 0, EquippableLocations.NotEquippable, true);
            inventory.AddInventorySlot(InventoryItem.ShotgunAmmo, 60, 0, EquippableLocations.NotEquippable, true);
            inventory.AddInventorySlot(InventoryItem.LMGAmmo, 600, 0, EquippableLocations.NotEquippable, true);

            inventory.AddInventorySlot(InventoryItem.Pistol, 1, 0, EquippableLocations.NotEquippable, true);
            inventory.AddInventorySlot(InventoryItem.SMG, 1, 0, EquippableLocations.NotEquippable, true);
            inventory.AddInventorySlot(InventoryItem.AR, 1, 0, EquippableLocations.NotEquippable, true);
            inventory.AddInventorySlot(InventoryItem.Shotgun, 1, 0, EquippableLocations.NotEquippable, true);
            inventory.AddInventorySlot(InventoryItem.LMG, 1, 0, EquippableLocations.NotEquippable, true);

            inventory.AddInventorySlot(InventoryItem.Knife, 1, 0, EquippableLocations.MeleeSlot, true);
            inventory.AddInventorySlot(InventoryItem.Machete, 1, 0, EquippableLocations.MeleeSlot, true);
            inventory.AddInventorySlot(InventoryItem.Katana, 1, 0, EquippableLocations.MeleeSlot, true);

            inventory.AddInventorySlot(InventoryItem.Rock, 40, 0, EquippableLocations.ThrowableSlot, true);
            inventory.AddInventorySlot(InventoryItem.Grenade, 8, 0, EquippableLocations.ThrowableSlot, true);
            inventory.AddInventorySlot(InventoryItem.AlarmBomb, 6, 0, EquippableLocations.ThrowableSlot, true);
            inventory.AddInventorySlot(InventoryItem.ProximityMine, 6, 0, EquippableLocations.ThrowableSlot, true);
            inventory.AddInventorySlot(InventoryItem.Turret, 4, 0, EquippableLocations.ThrowableSlot, true);

            return inventory;
        }

        public void Initialize(PlayerInventory inventory)
        {
            // initialize guns
            foreach (var gun in inventory.InventoryGuns)
            {
                CurrentInventory[gun.Key].Locked = gun.Value.Locked;
                owningPlayer.EquipmentManager.GetGun(gun.Key).Reload(gun.Value.AmmoCount);
            }

            // initialize melee
            foreach (var melee in inventory.InventoryMelee)
                CurrentInventory[melee.Key].Locked = melee.Value.Locked;

            // initialize throwables
            foreach (var throwable in inventory.InventoryThrowables)
            {
                CurrentInventory[throwable.Key].Locked = throwable.Value.Locked;
                CurrentInventory[throwable.Key].MaxCapacity = throwable.Value.MaxCapacity;
                CurrentInventory[throwable.Key].CurrentCount = throwable.Value.Count;
            }

            // initialize ammo
            foreach (var ammo in inventory.InventoryAmmo)
            {
                CurrentInventory[ammo.Key].Locked = ammo.Value.Locked;
                CurrentInventory[ammo.Key].MaxCapacity = ammo.Value.MaxCapacity;
                CurrentInventory[ammo.Key].CurrentCount = ammo.Value.Count;
            }
        }

        public PlayerInventory RetrievePlayerInventory()
        {
            PlayerInventory inventory = new PlayerInventory();

            // save guns
            inventory.InventoryGuns = new Dictionary<InventoryItem, GunSavedState>()
            {
                {
                    InventoryItem.Pistol, new GunSavedState {
                    Locked = CurrentInventory[InventoryItem.Pistol].Locked,
                    AmmoCount = owningPlayer.EquipmentManager.Pistol.MagazineCount
                    }
                },
                {
                    InventoryItem.SMG, new GunSavedState {
                    Locked = CurrentInventory[InventoryItem.SMG].Locked,
                    AmmoCount = owningPlayer.EquipmentManager.SMG.MagazineCount
                    }
                },
                {
                    InventoryItem.AR, new GunSavedState {
                    Locked = CurrentInventory[InventoryItem.AR].Locked,
                    AmmoCount = owningPlayer.EquipmentManager.AR.MagazineCount
                    }
                },
                {
                    InventoryItem.Shotgun, new GunSavedState {
                    Locked = CurrentInventory[InventoryItem.Shotgun].Locked,
                    AmmoCount = owningPlayer.EquipmentManager.Shotgun.MagazineCount
                    }
                },
                {
                    InventoryItem.LMG, new GunSavedState {
                        Locked = CurrentInventory[InventoryItem.LMG].Locked,
                        AmmoCount = owningPlayer.EquipmentManager.LMG.MagazineCount
                    }
                }
            };

            // save melee
            inventory.InventoryMelee = new Dictionary<InventoryItem, MeleeSavedState> {
                {
                    InventoryItem.Knife, new MeleeSavedState {
                        Locked = CurrentInventory[InventoryItem.Knife].Locked
                    }
                },
                {
                    InventoryItem.Machete, new MeleeSavedState {
                        Locked = CurrentInventory[InventoryItem.Machete].Locked
                    }
                },
                {
                    InventoryItem.Katana, new MeleeSavedState {
                        Locked = CurrentInventory[InventoryItem.Katana].Locked
                    }
                }
            };

            // save throwables
            inventory.InventoryThrowables = new Dictionary<InventoryItem, ThrowableSavedState> {
                {
                    InventoryItem.Rock, new ThrowableSavedState {
                        Locked = CurrentInventory[InventoryItem.Rock].Locked,
                        MaxCapacity = CurrentInventory[InventoryItem.Rock].MaxCapacity,
                        Count = CurrentInventory[InventoryItem.Rock].CurrentCount
                    }
                },
                {
                    InventoryItem.AlarmBomb, new ThrowableSavedState {
                        Locked = CurrentInventory[InventoryItem.AlarmBomb].Locked,
                        MaxCapacity = CurrentInventory[InventoryItem.AlarmBomb].MaxCapacity,
                        Count = CurrentInventory[InventoryItem.AlarmBomb].CurrentCount
                    }
                },
                {
                    InventoryItem.Grenade, new ThrowableSavedState {
                        Locked = CurrentInventory[InventoryItem.Grenade].Locked,
                        MaxCapacity = CurrentInventory[InventoryItem.Grenade].MaxCapacity,
                        Count = CurrentInventory[InventoryItem.Grenade].CurrentCount
                    }
                },
                {
                    InventoryItem.ProximityMine, new ThrowableSavedState {
                        Locked = CurrentInventory[InventoryItem.ProximityMine].Locked,
                        MaxCapacity = CurrentInventory[InventoryItem.ProximityMine].MaxCapacity,
                        Count = CurrentInventory[InventoryItem.ProximityMine].CurrentCount
                    }
                },
                {
                    InventoryItem.Turret, new ThrowableSavedState {
                        Locked = CurrentInventory[InventoryItem.Turret].Locked,
                        MaxCapacity = CurrentInventory[InventoryItem.Turret].MaxCapacity,
                        Count = CurrentInventory[InventoryItem.Turret].CurrentCount
                    }
                }
            };

            // save ammo
            inventory.InventoryAmmo = new Dictionary<InventoryItem, AmmoSavedState> {
                {
                    InventoryItem.PistolSMGAmmo, new AmmoSavedState {
                        Locked = CurrentInventory[InventoryItem.PistolSMGAmmo].Locked,
                        MaxCapacity = CurrentInventory[InventoryItem.PistolSMGAmmo].MaxCapacity,
                        Count = CurrentInventory[InventoryItem.PistolSMGAmmo].CurrentCount
                    }
                },
                {
                    InventoryItem.ARAmmo, new AmmoSavedState {
                        Locked = CurrentInventory[InventoryItem.ARAmmo].Locked,
                        MaxCapacity = CurrentInventory[InventoryItem.ARAmmo].MaxCapacity,
                        Count = CurrentInventory[InventoryItem.ARAmmo].CurrentCount
                    }
                },
                {
                    InventoryItem.ShotgunAmmo, new AmmoSavedState {
                        Locked = CurrentInventory[InventoryItem.ShotgunAmmo].Locked,
                        MaxCapacity = CurrentInventory[InventoryItem.ShotgunAmmo].MaxCapacity,
                        Count = CurrentInventory[InventoryItem.ShotgunAmmo].CurrentCount
                    }
                },
                {
                    InventoryItem.LMGAmmo, new AmmoSavedState {
                        Locked = CurrentInventory[InventoryItem.LMGAmmo].Locked,
                        MaxCapacity = CurrentInventory[InventoryItem.LMGAmmo].MaxCapacity,
                        Count = CurrentInventory[InventoryItem.LMGAmmo].CurrentCount
                    }
                },
            };

            return inventory;
        }
    }

    [Serializable]
    public enum InventoryItem
    {
        NoMatch = -1,

        // ammo
        PistolSMGAmmo = 10,
        ARAmmo = 11,
        ShotgunAmmo = 12,
        LMGAmmo = 13,

        // Guns
        NoGun = 0,
        Pistol = 30,
        SMG = 32,
        AR = 31,
        Shotgun = 33,
        LMG = 34,

        // Melee
        NoMelee = 1,
        Knife = 50,
        Machete = 51,
        Katana = 52,

        // Throwables
        NoThrowable = 2,
        Rock = 70,
        Grenade = 71,
        AlarmBomb = 72,
        ProximityMine = 73,
        Turret = 74,

        // Pickups
        Pickup
    }

    public class InventorySlot
    {
        public string name;
        public InventoryItem InventoryItem;
        public int MaxCapacity;
        public int CurrentCount;
        public EquippableLocations Equippable;
        public bool Locked;
    }

    public enum EquippableLocations
    {
        Hotbar,
        ThrowableSlot,
        MeleeSlot,
        NotEquippable
    }
}
