using ViralVial.Player;
using ViralVial.Player.TechTreeCode;

namespace ViralVial.Ability.Human
{
    public class BuffsAbility : IAbility
    {

        public IPlayer OwningPlayer { get; set; }
        public AbilityType AbilityType { get; } = AbilityType.Human;

        public bool UseAbility() { return true; }
        public void UpgradeAbility(AbilityLevel abilityUpgrade)
        {
            switch (abilityUpgrade.effect)
            {
                case "MoreDamage":
                    OwningPlayer.PlayerAttributes.OverallDamageMultiplier = abilityUpgrade.stats["DamageMultiplier"];
                    break;
                case "DashSpeedBoost":
                    OwningPlayer.PlayerAttributes.PostDashSpeedBoostMultiplier = abilityUpgrade.stats["SpeedBoost"];
                    OwningPlayer.PlayerAttributes.PostDashSpeedBoostDuration = abilityUpgrade.stats["SpeedBoostDuration"];
                    break;
                case "DashSpeed":
                    OwningPlayer.PlayerAttributes.DashSpeedMultiplier = abilityUpgrade.stats["DashSpeedMultiplier"];
                    break;
                case "DashShockWave":
                    OwningPlayer.PlayerAttributes.PostDashShockWaveEnabled = true;
                    OwningPlayer.PlayerAttributes.PostDashShockWaveDamage = abilityUpgrade.stats["DashShockWaveDamage"];
                    OwningPlayer.PlayerAttributes.PostDashShockWaveRange = abilityUpgrade.stats["DashShockWaveRange"];
                    break;
                case "ReloadSpeed":
                    OwningPlayer.PlayerAttributes.ReloadSpeedMultiplier = abilityUpgrade.stats["ReloadMultiplier"];
                    break;
                case "ProtectedDash":
                    OwningPlayer.PlayerAttributes.PostDashInvulnerabilityTime = abilityUpgrade.stats["Duration"];
                    break;
                case "HealthRegen":
                    OwningPlayer.PlayerAttributes.LowHealthRegenThreshold = abilityUpgrade.stats["HealthThreshold"];
                    OwningPlayer.PlayerAttributes.LowHealthRegenAmount = abilityUpgrade.stats["RegenAmount"];
                    OwningPlayer.PlayerAttributes.LowHealthRegenInterval = abilityUpgrade.stats["RegenInterval"];
                    break;
            }
        }

        public void OnDestroy() { }
    }
}