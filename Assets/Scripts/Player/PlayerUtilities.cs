using System.Net.Http.Headers;
using System.Collections.Generic;
using UnityEngine;
using ViralVial.Ability;
using ViralVial.Input;
using ViralVial.Weapons;

namespace ViralVial.Player
{
    public static class PlayerUtilities
    {
        public static InventoryItem GetAmmoForGunType(InventoryItem gunType)
        {
            switch (gunType)
            {
                case InventoryItem.Pistol:
                case InventoryItem.SMG:
                    return InventoryItem.PistolSMGAmmo;
                case InventoryItem.AR: return InventoryItem.ARAmmo;
                case InventoryItem.Shotgun: return InventoryItem.ShotgunAmmo;
                case InventoryItem.LMG: return InventoryItem.LMGAmmo;
                default: return InventoryItem.NoMatch;
            }
        }

        public static void AddInventorySlot(this Dictionary<InventoryItem, InventorySlot> dict, InventoryItem itemType, int maxCapacity, int currentCount = 0, EquippableLocations equippable = EquippableLocations.NotEquippable, bool locked = false)
        {
            dict.Add(itemType, new InventorySlot()
            {
                InventoryItem = itemType,
                MaxCapacity = maxCapacity,
                CurrentCount = currentCount,
                Equippable = equippable,
                Locked = locked
            });
        }

        public static void SpawnShockWave(IPlayer player, float range, float damage, float pushSpeed)
        {
            Collider[] enemiesInRange = Physics.OverlapSphere(player.Transform.position, range, LayerMask.GetMask(Utilities.Constants.EnemyLayerName));
            foreach (var col in enemiesInRange)
            {
                col.GetComponent<AbilitiesReactionController>().ApplyShockWave(player.Transform.position, range, pushSpeed);
                col.GetComponent<HitBox>().OnWeaponHit(damage * (range - Vector3.Distance(player.Transform.position, col.transform.position)));
            }
        }
    }

    public static class PlayerConstants
    {
        public const float StartingHealth = 100f;
        public const int NumberHotbarSlots = 4;

        // Leveling up
        public const int StartingSkillPoints = 0;
        public const int StartingLevel = 0;
        public const int StartingExperience = 0;
        public const float StartingExperienceToLevelUp = 500;
        public const float ExperienceIncreaseRate = 1.3f;
        public const int LevelUpSkillPointMultiplier = 2;

        // Ammo Capacity
        public const int PistolSMGAmmoCapacity = 300;
        public const int ARAmmoCapacity = 400;
        public const int ShotgunAmmoCapacity = 60;
        public const int LMGAmmoCapacity = 600;

        // Gun unlock free ammo
        public const int PistolUnlockAmmo = 100;
        public const int SMGUnlockAmmo = 150;
        public const int ARUnlockAmmo = 200;
        public const int ShotgunUnlockAmmo = 60;
        public const int LMGUnlockAmmo = 200;

        // throwable unlock free throwables
        public const int RockUnlock = 10;
        public const int AlarmBombUnlock = 4;
        public const int GrenadeUnlock = 4;
        public const int ProximityMineUnlock = 3;
        public const int TurretUnlock = 2;
    }

    public enum AnimatorTrigger
    {

        NoTrigger = 0,
        IdleTrigger = 1,
        ActionTrigger = 2,
        ClimbLadderTrigger = 3,
        AttackTrigger = 4,
        AttackKickTrigger = 5,
        AttackDualTrigger = 6,
        AttackCastTrigger = 7,
        SpecialAttackTrigger = 8,
        SpecialEndTrigger = 9,
        CastTrigger = 10,
        CastEndTrigger = 11,
        GetHitTrigger = 12,
        RollTrigger = 13,
        TurnTrigger = 14,
        WeaponSheathTrigger = 15,
        WeaponUnsheathTrigger = 16,
        DodgeTrigger = 17,
        JumpTrigger = 18,
        BlockTrigger = 19,
        DeathTrigger = 20,
        ReviveTrigger = 21,
        BlockBreakTrigger = 22,
        SwimTrigger = 23,
        ReloadTrigger = 24,
        InstantSwitchTrigger = 25,
        KnockbackTrigger = 26,
        KnockdownTrigger = 27,
        DiveRollTrigger = 28,
        CrawlTrigger = 29,
        MeleeAttackTrigger = 30,
        StartCastingTrigger = 31,
        StopCastingTrigger = 32,
        FireGunTrigger = 4,
        SwitchToShootingMovementTrigger = 33,
        SwitchToUnarmedMovementTrigger = 34,
        SwitchToArmedMovementTrigger = 35,
        EndAnimationTrigger = 36,
        UseThrowableTrigger = 37
    }

    public enum AbilityAnimationsCodes
    {
        MindControlAbility = 1,
        ShockWaveAbility = 4,
        BlinkAbility = 5,
        TimeFreezeAbility = 6,
        FireAttackAbility = 7
    }

    public enum ThrowAnimationStyleCodes
    {
        SideArmThrow = 1,
        OverhandThrow = 2,
        UnderhandThrow1 = 3,
        UnderhandThrow2 = 4
    }
}
