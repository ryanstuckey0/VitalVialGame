using ViralVial.Player;
using ViralVial.Player.TechTreeCode;
using ViralVial.Weapons;

namespace ViralVial.Ability.Human
{
    public class MeleeAbility : IAbility
    {
        public IPlayer OwningPlayer { get; set; }
        public AbilityType AbilityType { get; } = AbilityType.Human;


        public bool UseAbility() { return true; }

        public void UpgradeAbility(AbilityLevel abilityUpgrade)
        {
            switch (abilityUpgrade.effect)
            {
                case "Knife":
                    OwningPlayer.Inventory.UnlockItem(InventoryItem.Knife);
                    break;
                case "Machete":
                    OwningPlayer.Inventory.UnlockItem(InventoryItem.Machete);
                    break;
                case "Katana":
                    OwningPlayer.Inventory.UnlockItem(InventoryItem.Katana);
                    break;
                case "MeleeSpeed":
                    OwningPlayer.PlayerAttributes.MeleeSpeedMultiplier = abilityUpgrade.stats["MeleeSpeedMultiplier"];
                    break;
                case "MeleeDamage":
                    OwningPlayer.PlayerAttributes.MeleeDamageMultiplier = abilityUpgrade.stats["DamageMultiplier"];
                    break;
                case "DualWield":
                    OwningPlayer.PermittedActions.DualWield = true;
                    break;
            }
        }

        public void OnDestroy() { }
    }
}