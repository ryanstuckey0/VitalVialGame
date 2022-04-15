using System.Collections;
using UnityEngine;

namespace ViralVial.Player.MonoBehaviourScript.DebugScript
{
    public class PlayerDebugLoader : MonoBehaviour
    {
        [SerializeField] private bool useDebugLoader;
        [SerializeField] private BasePlayerController BasePlayerController;

        [Header("Stats")]
        [SerializeField] private bool loadPlayerStats;
        [SerializeField] private float health;
        [SerializeField] private float maxHealth;
        [SerializeField] private int level;
        [SerializeField] private float experience;
        [SerializeField] private int skillPoints;


        [Header("Abilities")]
        [SerializeField] private bool loadPlayerAbilities;
        [SerializeField] private AbilityType[] hotbarAbilities = new AbilityType[4];
        [SerializeField] private string abilityLevelToLoad = "DEBUG";
        [SerializeField] private AbilityType[] humanAbilities;
        [SerializeField] private string[] humanAbilityLevelsToUnlock;

        [Header("Inventory: Unlocked Guns")]
        [SerializeField] private bool loadPlayerGuns;
        [SerializeField] private bool Pistol;
        [SerializeField] private bool SMG;
        [SerializeField] private bool AR;
        [SerializeField] private bool Shotgun;
        [SerializeField] private bool LMG;

        [Header("Inventory: Ammo")]
        [SerializeField] private int ARAmmo = 0;
        [SerializeField] private int PistolSMGAmmo = 0;
        [SerializeField] private int LMGAmmo = 0;
        [SerializeField] private int ShotgunAmmo = 0;

        [Header("Inventory: Unlocked Melee")]
        [SerializeField] private bool loadPlayerMelee;
        [SerializeField] private bool Knife;
        [SerializeField] private bool Machete;
        [SerializeField] private bool Katana;

        [SerializeField] private InventoryItem equippedMelee = InventoryItem.NoMelee;

        [Header("Inventory: Unlocked Throwables")]
        [SerializeField] private bool loadPlayerThrowables;
        [SerializeField] private bool Rock;
        [SerializeField] private bool AlarmClock;
        [SerializeField] private bool ProximityMine;
        [SerializeField] private bool Grenade;
        [SerializeField] private bool Turret;

        [Header("Inventory: Throwables Count")]
        [SerializeField] private int Rocks;
        [SerializeField] private int AlarmClocks;
        [SerializeField] private int ProximityMines;
        [SerializeField] private int Grenades;
        [SerializeField] private int Turrets;

        [SerializeField] private InventoryItem equippedThrowable = InventoryItem.NoThrowable;

        private void Start()
        {
            if (!useDebugLoader) return;
            StartCoroutine(LoadPlayerCoroutine());
        }

        private IEnumerator LoadPlayerCoroutine()
        {
            yield return new WaitForSeconds(1);
            IPlayer owningPlayer = BasePlayerController.OwningPlayer;

            if (loadPlayerStats) LoadStats(owningPlayer);
            if (loadPlayerAbilities) PreloadAbilities(owningPlayer);
            LoadInventory(owningPlayer);

            owningPlayer.BasePlayerController.PlayerInputController.EnableInput();

            Destroy(this);
        }

        private void LoadStats(IPlayer owningPlayer)
        {
            owningPlayer.MaxHealth = maxHealth;
            owningPlayer.Health = health;
            owningPlayer.SkillPoints = skillPoints;
            owningPlayer.Level = level;
            owningPlayer.Experience = experience;
        }

        private void LoadInventory(IPlayer owningPlayer)
        {
            if (loadPlayerGuns)
            {
                AddToInventory(owningPlayer, InventoryItem.ARAmmo, ARAmmo);
                AddToInventory(owningPlayer, InventoryItem.PistolSMGAmmo, PistolSMGAmmo);
                AddToInventory(owningPlayer, InventoryItem.LMGAmmo, LMGAmmo);
                AddToInventory(owningPlayer, InventoryItem.ShotgunAmmo, ShotgunAmmo);

                if (Pistol) owningPlayer.Inventory.UnlockItem(InventoryItem.Pistol);
                if (SMG) owningPlayer.Inventory.UnlockItem(InventoryItem.SMG);
                if (AR) owningPlayer.Inventory.UnlockItem(InventoryItem.AR);
                if (Shotgun) owningPlayer.Inventory.UnlockItem(InventoryItem.Shotgun);
                if (LMG) owningPlayer.Inventory.UnlockItem(InventoryItem.LMG);
            }

            if (loadPlayerMelee)
            {
                if (Knife) owningPlayer.Inventory.UnlockItem(InventoryItem.Knife);
                if (Machete) owningPlayer.Inventory.UnlockItem(InventoryItem.Machete);
                if (Katana) owningPlayer.Inventory.UnlockItem(InventoryItem.Katana);

                if (equippedMelee != InventoryItem.NoMelee) owningPlayer.EquipmentManager.EquipMelee(equippedMelee);

            }

            if (loadPlayerThrowables)
            {
                // throwables
                if (Rock)
                {
                    owningPlayer.Inventory.UnlockItem(InventoryItem.Rock);
                    AddToInventory(owningPlayer, InventoryItem.Rock, Rocks);
                }
                if (AlarmClock)
                {
                    owningPlayer.Inventory.UnlockItem(InventoryItem.AlarmBomb);
                    AddToInventory(owningPlayer, InventoryItem.AlarmBomb, AlarmClocks);
                }
                if (ProximityMine)
                {
                    owningPlayer.Inventory.UnlockItem(InventoryItem.ProximityMine);
                    AddToInventory(owningPlayer, InventoryItem.ProximityMine, ProximityMines);
                }
                if (Grenade)
                {
                    owningPlayer.Inventory.UnlockItem(InventoryItem.Grenade);
                    AddToInventory(owningPlayer, InventoryItem.Grenade, Grenades);
                }

                if (Turret)
                {
                    owningPlayer.Inventory.UnlockItem(InventoryItem.Turret);
                    AddToInventory(owningPlayer, InventoryItem.Turret, Turrets);
                }

                if (equippedThrowable != InventoryItem.NoThrowable) owningPlayer.EquipmentManager.EquipThrowable(equippedThrowable);
            }
        }

        private void AddToInventory(IPlayer player, InventoryItem inventoryItem, int number)
        {
            InventorySlot slot = player.Inventory.CurrentInventory[inventoryItem];
            slot.CurrentCount = number;
            player.Inventory.CurrentInventory[inventoryItem] = slot;
        }

        private void PreloadAbilities(IPlayer owningPlayer)
        {
            for (int i = 0; i < 4; i++)
            {
                string abilityIdString = MapAbilityTypeToString(hotbarAbilities[i]);
                if (abilityIdString == null) continue;
                owningPlayer.TechTree.SupernaturalAbilitySlots[abilityIdString].ForceUnlockAbility(abilityLevelToLoad);
                owningPlayer.EquipmentManager.AssignAbilityToSlot(owningPlayer.TechTree.SupernaturalAbilitySlots[abilityIdString], i + 1);
            }

            for (int i = 0; i < humanAbilities.Length; i++)
            {
                string abilityIdString = MapAbilityTypeToString(humanAbilities[i]);
                if (abilityIdString == null) continue;
                owningPlayer.TechTree.HumanAbilitySlots[abilityIdString].ForceUnlockAbility(humanAbilityLevelsToUnlock[i]);
            }
        }

        private string MapAbilityTypeToString(AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.MindControl: return "mind-control";
                case AbilityType.Blink: return "blink";
                case AbilityType.HealthBoost: return "health-boost";
                case AbilityType.TimeFreeze: return "time-freeze";
                case AbilityType.ShockWave: return "shock-wave";
                case AbilityType.ElementalFire: return "fire-attack";
                case AbilityType.Melee: return "melee";
                case AbilityType.Buffs: return "buffs";
                case AbilityType.Guns: return "guns";
                case AbilityType.Throwables: return "throwables";
                default: return null;
            }
        }

        private enum AbilityType
        {
            MindControl,
            ShockWave,
            TimeFreeze,
            Blink,
            ElementalFire,
            HealthBoost,
            Melee,
            Buffs,
            Throwables,
            Guns,
            None
        }
    }
}
