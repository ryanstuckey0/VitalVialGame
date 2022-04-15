using ViralVial.Player;

namespace ViralVial.Weapons
{
    public static class WeaponUtilities
    {
        public static float TurretDamagePerShot;
        public static float TurretDamageMultiplier = 1;

        public static bool Is1HandedWeapon(InventoryItem weaponType)
        {
            switch (weaponType)
            {
                case InventoryItem.Pistol:
                    return true;
                default: return false;
            }
        }

        public enum WeaponSide
        {
            Left = 1,
            Right = 2
        }
    }
}