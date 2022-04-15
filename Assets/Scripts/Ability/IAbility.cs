using ViralVial.Player;
using ViralVial.Player.TechTreeCode;

namespace ViralVial.Ability
{
    /// <summary>
    /// Interface representing a special ability for a player. Includes a name and a function to 
    /// activate the ability.
    /// </summary>
    public interface IAbility
    {
        IPlayer OwningPlayer { get; set; }
        AbilityType AbilityType { get; }

        /// <summary>
        /// Returns true if the cooldown should start, else false.
        /// </summary>
        /// <returns>true if cooldown should start, else false</returns>
        bool UseAbility();
        void UpgradeAbility(AbilityLevel abilityUpgrade);
        void OnDestroy();
    }

    public enum AbilityType
    {
        Human,
        Supernatural
    }
}
