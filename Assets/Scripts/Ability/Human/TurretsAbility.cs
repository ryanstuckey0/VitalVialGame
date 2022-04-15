using ViralVial.Player;
using ViralVial.Player.TechTreeCode;

namespace ViralVial.Ability.Human
{
    public class TurretsAbility : IAbility
    {
        public IPlayer OwningPlayer { get; set; }
        public AbilityType AbilityType { get; } = AbilityType.Human;

        public bool UseAbility() { return true; }

        public void UpgradeAbility(AbilityLevel abilityUpgrade)
        {
            switch (abilityUpgrade.effect)
            {
                case "UnlockTurrets":
                    OwningPlayer.Inventory.UnlockItem(InventoryItem.Turret);
                    OwningPlayer.Inventory.AddToInventory(InventoryItem.Turret, 2);
                    break;
                case "TurretInventorySize":
                    OwningPlayer.Inventory.ModifyMaxItemCapacity(InventoryItem.Turret, (int)(OwningPlayer.Inventory.GetItemCapacity(InventoryItem.Turret) * abilityUpgrade.stats["TurretInventorySizeMultiplier"]));
                    break;
                case "TurretAmmoCapacity":
                    OwningPlayer.PlayerAttributes.TurretAmmoCapacityMultiplier = abilityUpgrade.stats["TurretAmmoCapacityMultiplier"];
                    break;
                case "TurretDamage":
                    OwningPlayer.PlayerAttributes.TurretDamageMultiplier = abilityUpgrade.stats["TurretDamageMultiplier"];
                    break;
                case "TurretOverheatLimit":
                    OwningPlayer.PlayerAttributes.TurretOverheatLimitMultiplier = abilityUpgrade.stats["TurretOverheatLimit"];
                    break;
            }
        }

        public void OnDestroy() { }
    }
}