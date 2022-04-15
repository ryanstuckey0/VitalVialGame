using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViralVial.Ability;
using ViralVial.Utilities;

namespace ViralVial.Player.TechTreeCode
{
    /// <summary>
    /// An AbilitySlot provides an easy way to access and hold abilities of different types. It 
    /// includes logic like cooldown time or whether or not the ability has been unlocked. 
    /// Eventually, I think we will also be able to use it to add the abilities to the tech tree.
    /// </summary>
    public class AbilitySlot
    {
        public readonly string Id;
        public string AbilityName { get; set; }
        public IAbility Ability { get; set; }
        public Dictionary<string, AbilityLevel> AbilityLevelsList { get; set; }

        public string[] PrereqAbilitiesOR { get; set; }
        public string[] PrereqAbilitiesAND { get; set; }
        public TechTree TechTree { get; set; }
        public AbilityType AbilityType { get => Ability.AbilityType; }

        public bool Locked { get; private set; } = true;

        // cooldown values
        private float cooldown { get; set; }
        public bool CoolingDown { get; private set; } = false;
        private CoroutineRunner cooldownCoroutine;
        private Dictionary<string, object> cooldownEventDictionary;

        public AbilitySlot(string id)
        {
            Id = id;
            EventManager.Instance.SubscribeToEvent("ResetCooldowns", OnResetCooldowns);
            cooldownCoroutine = new CoroutineRunner();
            cooldownEventDictionary = new Dictionary<string, object> {
                {"time", -1},
                {"slotNumber", -1}
            };
        }

        public void OnDestroy()
        {
            EventManager.Instance.UnsubscribeFromEvent("ResetCooldowns", OnResetCooldowns);
            Ability.OnDestroy();
        }

        /// <summary>
        /// Uses the ability and starts the cooldown timer.
        /// </summary>
        public void UseAbility(int slotNumber)
        {
            if (CoolingDown || Locked) return;
            bool startCooldown = Ability.UseAbility();
            if (startCooldown)
                cooldownCoroutine.Start(StartCooldownCoroutine(slotNumber));
        }

        public void ForceUnlockAbility(string unlockLevelName = "base")
        {
            Locked = false;
            ForceUpgradeAbility(unlockLevelName);
        }

        public void ForceUpgradeAbility(string upgradeName)
        {
            if (upgradeName == "base") Locked = false;
            float tempFloat = 0.0f;
            if (Ability.AbilityType == AbilityType.Supernatural && (AbilityLevelsList[upgradeName].stats?.TryGetValue("Cooldown", out tempFloat) ?? false)) cooldown = tempFloat;
            Ability.UpgradeAbility(AbilityLevelsList[upgradeName]);
            TechTree.AbilitiesProgress[Id][upgradeName] = true;
        }

        public bool UnlockAbility(string unlockLevelName = "base")
        {
            if (!CanUnlock(unlockLevelName)) return false;

            Locked = false;
            UpgradeAbility(unlockLevelName);
            return true;
        }

        public bool CanUnlock(string unlockLevelName = "base")
        {
            if (TechTree.AbilityIsUnlocked(Id, unlockLevelName)) return false; // can't unlock again if already unlocked
            if (!UnlockPrereqsAreSatisfied()) return false; // can't unlocked if prereqs abilities not unlocked
            if (AbilityLevelsList["base"].costSkillPoints > TechTree.OwningPlayer.SkillPoints) return false; // cant unlock if player doesn't have enough skill points
            return true;
        }

        public bool UpgradeAbility(string upgradeName)
        {
            if (!CanUpgrade(upgradeName)) return false;

            float tempFloat = 0.0f;
            if (Ability.AbilityType == AbilityType.Supernatural && (AbilityLevelsList[upgradeName].stats?.TryGetValue("Cooldown", out tempFloat) ?? false)) cooldown = tempFloat;
            Ability.UpgradeAbility(AbilityLevelsList[upgradeName]);
            TechTree.AbilitiesProgress[Id][upgradeName] = true;
            TechTree.OwningPlayer.SkillPoints -= AbilityLevelsList[upgradeName].costSkillPoints;
            return true;
        }

        public bool CanUpgrade(string upgradeName)
        {
            if (Locked) return false; // if ability has not been unlocked
            if (!UpgradePrereqsAreSatisfied(upgradeName)) return false; // if prereqs are not satisfied
            if (TechTree.AbilitiesProgress[Id][upgradeName]) return false; // if upgrade has already been unlocked
            if (upgradeName != "base" && AbilityLevelsList[upgradeName].costSkillPoints > TechTree.OwningPlayer.BasePlayerController.SkillPoints) return false; // if player doesn't have enough skill points
            return true;
        }

        public void OnResetCooldowns()
        {
            cooldownCoroutine.Stop();
            CoolingDown = false;
        }

        private IEnumerator StartCooldownCoroutine(int slotNumber)
        {
            CoolingDown = true;

            cooldownEventDictionary["time"] = cooldown;
            cooldownEventDictionary["slotNumber"] = slotNumber;

            EventManager.Instance.InvokeEvent("StartHotbarSlotCooldown", cooldownEventDictionary);
            yield return new WaitForSeconds(cooldown);
            CoolingDown = false;

            TechTree.OwningPlayer.BasePlayerController.PlayerAudioController.PlayAudio("OnAbilityCooldown");
        }

        private bool UnlockPrereqsAreSatisfied()
        {
            return UnlockANDPrereqsAreSatisfied() && UnlockORPrereqsAreSatisfied();
        }

        private bool UnlockORPrereqsAreSatisfied()
        {
            foreach (string prereqAbility in PrereqAbilitiesOR)
            {
                if (TechTree.AbilityIsUnlocked(prereqAbility)) return true;
            }
            return false || PrereqAbilitiesOR.Length == 0;
        }

        private bool UnlockANDPrereqsAreSatisfied()
        {

            foreach (string prereqAbility in PrereqAbilitiesAND)
            {
                if (TechTree.AbilityIsUnlocked(prereqAbility)) continue;
                return false;
            }
            return true;
        }

        private bool UpgradePrereqsAreSatisfied(string upgradeName)
        {
            foreach (string prereqLevel in AbilityLevelsList[upgradeName].prereqs)
            {
                if (TechTree.AbilityIsUnlocked(Id, prereqLevel)) continue;
                return false;
            }
            return true;
        }
    }
}
