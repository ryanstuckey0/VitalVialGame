using System.Collections.Generic;
using UnityEngine;
using ViralVial.Utilities;

namespace ViralVial.Player
{
    public class PlayerAttributes
    {
        // Movement Modifiers
        private float movementSpeed = 1;
        public float MovementSpeed
        {
            get { return movementSpeed; }
            set { movementSpeed = FireEventOnValueChange<float>(value, "MovementSpeedChange"); }
        }

        // Gun Modifiers
        private float gunDamageMultiplier = 0;
        public float GunDamageMultiplier
        {
            get { return gunDamageMultiplier; }
            set { gunDamageMultiplier = FireEventOnValueChange<float>(value, "GunDamageMultiplierChange"); }
        }

        private float gunMagazineSizeMultiplier = 0;
        public float GunMagazineSizeMultiplier
        {
            get { return gunMagazineSizeMultiplier; }
            set { gunMagazineSizeMultiplier = FireEventOnValueChange<float>(value, "GunMagazineSizeMultiplierChange"); }
        }

        private float reloadSpeedMultiplier = 0;
        public float ReloadSpeedMultiplier
        {
            get { return reloadSpeedMultiplier; }
            set { reloadSpeedMultiplier = FireEventOnValueChange<float>(value, "ReloadSpeedMultiplierChange"); }
        }

        // Melee Modifiers
        private float meleeSpeedMultiplier = 0;
        public float MeleeSpeedMultiplier
        {
            get { return meleeSpeedMultiplier; }
            set { meleeSpeedMultiplier = FireEventOnValueChange<float>(value, "MeleeSpeedMultiplierChange"); }
        }

        private float meleeDamageMultiplier = 0;
        public float MeleeDamageMultiplier
        {
            get { return meleeDamageMultiplier; }
            set { meleeDamageMultiplier = FireEventOnValueChange<float>(value, "MeleeDamageMultiplierChange"); }
        }

        private float overallDamageMultiplier = 0;
        public float OverallDamageMultiplier
        {
            get { return overallDamageMultiplier; }
            set { overallDamageMultiplier = FireEventOnValueChange<float>(value, "OverallDamageMultiplierChange"); }
        }

        // Dash Modifiers
        private float dashSpeedMultiplier = 0;
        public float DashSpeedMultiplier
        {
            get { return dashSpeedMultiplier; }
            set { dashSpeedMultiplier = FireEventOnValueChange<float>(value, "DashSpeedMultiplierChange"); }
        }

        private float postDashSpeedBoostMultiplier = 0;
        public float PostDashSpeedBoostMultiplier
        {
            get { return postDashSpeedBoostMultiplier; }
            set { postDashSpeedBoostMultiplier = FireEventOnValueChange<float>(value, "PostDashSpeedBoostMultiplierChange"); }
        }

        private float postDashSpeedBoostDuration = 0;
        public float PostDashSpeedBoostDuration
        {
            get { return postDashSpeedBoostDuration; }
            set
            {
                postDashSpeedBoostDuration = FireEventOnValueChange<float>(value, "PostDashSpeedBoostDurationChange");
                CoroutineYielderCache.AddOrModifyYielder("PostDashSpeedBoostDuration", new WaitForSeconds(value));
            }
        }

        private float postDashInvulnerabilityTime = 0;
        public float PostDashInvulnerabilityTime
        {
            get { return postDashInvulnerabilityTime; }
            set
            {
                postDashInvulnerabilityTime = FireEventOnValueChange<float>(value, "PostDashInvulnerabilityTimeChange");
                CoroutineYielderCache.AddOrModifyYielder("PostDashInvulnerabilityTime", new WaitForSeconds(value));
            }
        }

        private bool postDashShockWaveEnabled = false;
        public bool PostDashShockWaveEnabled
        {
            get { return postDashShockWaveEnabled; }
            set { postDashShockWaveEnabled = FireEventOnValueChange<bool>(value, "PostDashShockWaveEnabledChange"); }
        }

        private float postDashShockWaveRange;
        public float PostDashShockWaveRange
        {
            get { return postDashShockWaveRange; }
            set { postDashShockWaveRange = FireEventOnValueChange<float>(value, "PostDashShockWaveRangeChange"); }
        }

        private float postDashShockWaveDamage;
        public float PostDashShockWaveDamage
        {
            get { return postDashShockWaveDamage; }
            set { postDashShockWaveDamage = FireEventOnValueChange<float>(value, "PostDashShockWaveDamageChange"); }
        }

        // Health Modifiers
        private float lowHealthRegenThreshold = 0;
        public float LowHealthRegenThreshold
        {
            get { return lowHealthRegenThreshold; }
            set { lowHealthRegenThreshold = FireEventOnValueChange<float>(value, "LowHealthRegenThresholdChange"); }
        }

        private float lowHealthRegenAmount = 0;
        public float LowHealthRegenAmount
        {
            get { return lowHealthRegenAmount; }
            set { lowHealthRegenAmount = FireEventOnValueChange<float>(value, "LowHealthRegenAmountChange"); }
        }

        private float lowHealthRegenInterval = 0;
        public float LowHealthRegenInterval
        {
            get { return lowHealthRegenInterval; }
            set
            {
                lowHealthRegenInterval = FireEventOnValueChange<float>(value, "LowHealthRegenIntervalChange");
                CoroutineYielderCache.AddOrModifyYielder("LowHealthRegenInterval", new WaitForSeconds(value));
            }
        }

        // Turrets
        private float turretAmmoCapacityMultiplier = 1;
        public float TurretAmmoCapacityMultiplier
        {
            get { return turretAmmoCapacityMultiplier; }
            set { turretAmmoCapacityMultiplier = FireEventOnValueChange<float>(value, "TurretAmmoCapacityMultiplierChange"); }
        }

        private float turretDamageMultiplier = 1;
        public float TurretDamageMultiplier
        {
            get { return turretDamageMultiplier; }
            set
            {
                turretDamageMultiplier = FireEventOnValueChange<float>(value, "TurretDamageMultiplierChange");
                Weapons.WeaponUtilities.TurretDamagePerShot *= value;
            }
        }

        private float turretOverheatRate = 1;
        public float TurretOverheatLimitMultiplier
        {
            get { return turretOverheatRate; }
            set { turretOverheatRate = FireEventOnValueChange<float>(value, "TurretOverheatRateChange"); }
        }

        private T FireEventOnValueChange<T>(T value, string eventName)
        {
            EventManager.Instance.InvokeEvent(eventName, new Dictionary<string, object>() { { "newValue", value } });
            return value;
        }

        public void OnDestroy() { }
    }
}