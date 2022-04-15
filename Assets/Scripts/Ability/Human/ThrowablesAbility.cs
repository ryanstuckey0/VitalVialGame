using ViralVial.Player;
using ViralVial.Player.TechTreeCode;

namespace ViralVial.Ability.Human
{
    public class ThrowablesAbility : IAbility
    {

        public IPlayer OwningPlayer { get; set; }
        public AbilityType AbilityType { get; } = AbilityType.Human;

        public bool UseAbility() { return true; }
        public void UpgradeAbility(AbilityLevel abilityUpgrade)
        {
            switch (abilityUpgrade.effect)
            {
                case "Rock":
                    OwningPlayer.Inventory.UnlockItem(InventoryItem.Rock);
                    break;
                case "AlarmClock":
                    OwningPlayer.Inventory.UnlockItem(InventoryItem.AlarmBomb);
                    break;
                case "Grenade":
                    OwningPlayer.Inventory.UnlockItem(InventoryItem.Grenade);
                    break;
                case "ProximityMine":
                    OwningPlayer.Inventory.UnlockItem(InventoryItem.ProximityMine);
                    break;
                case "ThrowableInventorySize":
                    float newCapacityMultiplier = abilityUpgrade.stats["ThrowableInventorySizeMultiplier"];
                    OwningPlayer.Inventory.ModifyMaxItemCapacity(InventoryItem.Rock, (int)(OwningPlayer.Inventory.GetItemCapacity(InventoryItem.Rock) * newCapacityMultiplier));
                    OwningPlayer.Inventory.ModifyMaxItemCapacity(InventoryItem.AlarmBomb, (int)(OwningPlayer.Inventory.GetItemCapacity(InventoryItem.AlarmBomb) * newCapacityMultiplier));
                    OwningPlayer.Inventory.ModifyMaxItemCapacity(InventoryItem.Grenade, (int)(OwningPlayer.Inventory.GetItemCapacity(InventoryItem.Grenade) * newCapacityMultiplier));
                    OwningPlayer.Inventory.ModifyMaxItemCapacity(InventoryItem.ProximityMine, (int)(OwningPlayer.Inventory.GetItemCapacity(InventoryItem.ProximityMine) * newCapacityMultiplier));
                    break;
            }
        }

        public void OnDestroy() { }
    }
}