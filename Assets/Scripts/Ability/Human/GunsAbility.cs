using ViralVial.Player;
using ViralVial.Player.TechTreeCode;
using ViralVial.Weapons;

namespace ViralVial.Ability.Human
{
    public class GunsAbility : IAbility
    {
        public IPlayer OwningPlayer { get; set; }
        public AbilityType AbilityType { get; } = AbilityType.Human;


        public bool UseAbility() { return true; }
        public void UpgradeAbility(AbilityLevel abilityUpgrade)
        {
            switch (abilityUpgrade.effect)
            {
                case "SMG":
                    OwningPlayer.Inventory.UnlockItem(InventoryItem.SMG);
                    break;
                case "AR":
                    OwningPlayer.Inventory.UnlockItem(InventoryItem.AR);
                    break;
                case "Shotgun":
                    OwningPlayer.Inventory.UnlockItem(InventoryItem.Shotgun);
                    break;
                case "LMG":
                    OwningPlayer.Inventory.UnlockItem(InventoryItem.LMG);
                    break;
                case "MoreDamage":
                    OwningPlayer.PlayerAttributes.GunDamageMultiplier = abilityUpgrade.stats["DamageMultiplier"];
                    break;
                case "MagazineSizeIncrease":
                    OwningPlayer.PlayerAttributes.GunMagazineSizeMultiplier = abilityUpgrade.stats["MagazineSizeMultiplier"];
                    break;
            }
        }

        public void OnDestroy() { }
    }
}