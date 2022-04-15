using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace ViralVial.Player.TechTreeCode
{
    public class TechTree
    {
        /// <summary>
        /// These dictionaries track whether an ability is locked or unlocked. The key is the ability name
        /// and the value is another dictionary. This second dictionary has keys that represent the levels 
        /// for that ability and bool values where if true, the ability is unlocked, else the ability is locked
        /// </summary>
        /// <value>key is an ability name; value is another dictionary with key being level id and value is 
        /// true if unlocked, else false</value>
        public Dictionary<string, Dictionary<string, bool>> AbilitiesProgress { get; private set; }

        /// <summary>
        /// This dictionary holds the ability name for the key and the Supernatural AbilitySlot for the value
        /// </summary>
        public Dictionary<string, AbilitySlot> SupernaturalAbilitySlots { get; private set; }

        /// <summary>
        /// This dictionary holds the ability name for the key and the Human AbilitySlot for the value
        /// </summary>
        public Dictionary<string, AbilitySlot> HumanAbilitySlots { get; private set; }

        public readonly IPlayer OwningPlayer;
        private readonly string AbilitiesJsonFilePath = Application.streamingAssetsPath + "/Abilities";

        public TechTree(IPlayer player)
        {
            OwningPlayer = player;
            AbilitiesProgress = new Dictionary<string, Dictionary<string, bool>>();
            LoadSupernaturalAbilities(Directory.GetFiles(AbilitiesJsonFilePath + "/Supernatural/", "*.json"));
            LoadHumanAbilities(Directory.GetFiles(AbilitiesJsonFilePath + "/Human/", "*.json"));
        }

        public void OnDestroy()
        {
            foreach (var abilitySlot in SupernaturalAbilitySlots.Values) abilitySlot.OnDestroy();
            foreach (var abilitySlot in HumanAbilitySlots.Values) abilitySlot.OnDestroy();
        }

        public void Initialize(Dictionary<string, Dictionary<string, bool>> abilitiesProgress)
        {
            foreach (var ability in abilitiesProgress)
            {
                AbilitySlot abilitySlot;
                bool supernatural = SupernaturalAbilitySlots.TryGetValue(ability.Key, out abilitySlot);
                if (!supernatural) HumanAbilitySlots.TryGetValue(ability.Key, out abilitySlot);
                if (abilitySlot == null)
                {
                    Debug.LogError($"Attempted to unlock ability id {ability.Key} but it was not in either dictionary. Stopping TechTree initialization.");
                    return;
                }

                foreach (var abilityLevel in ability.Value)
                {
                    if (!abilityLevel.Value || abilityLevel.Key == "DEBUG") continue;
                    abilitySlot.ForceUpgradeAbility(abilityLevel.Key);
                }
            }
        }

        public bool AbilityIsUnlocked(string ability, string level = "base")
        {
            return AbilitiesProgress[ability][level];
        }

        public bool UnlockSupernaturalAbility(string abilityId)
        {
            return SupernaturalAbilitySlots[abilityId].UnlockAbility();
        }

        public bool UnlockHumanAbility(string abilityId)
        {
            return HumanAbilitySlots[abilityId].UnlockAbility();
        }

        public bool UpgradeSupernaturalAbility(string abilityId, string upgradeId)
        {
            return SupernaturalAbilitySlots[abilityId].UpgradeAbility(upgradeId);
        }

        public bool UpgradeHumanAbility(string abilityId, string upgradeId)
        {
            return HumanAbilitySlots[abilityId].UpgradeAbility(upgradeId);
        }

        public bool UnlockAbility(string abilityId, string upgradeId = "")
        {
            if (SupernaturalAbilitySlots.ContainsKey(abilityId))
            {
                if (upgradeId == "base") return SupernaturalAbilitySlots[abilityId].UnlockAbility();
                else return SupernaturalAbilitySlots[abilityId].UpgradeAbility(upgradeId);
            }
            else if (HumanAbilitySlots.ContainsKey(abilityId))
            {
                if (upgradeId == "base") return HumanAbilitySlots[abilityId].UnlockAbility();
                else return HumanAbilitySlots[abilityId].UpgradeAbility(upgradeId);
            }

            return false;
        }

        public bool CanUnlockAbility(string abilityId, string upgradeId = "")
        {
            if (SupernaturalAbilitySlots.ContainsKey(abilityId))
            {
                if (upgradeId == "base") return SupernaturalAbilitySlots[abilityId].CanUnlock();
                else return SupernaturalAbilitySlots[abilityId].CanUpgrade(upgradeId);
            }
            else if (HumanAbilitySlots.ContainsKey(abilityId))
            {
                if (upgradeId == "base") return HumanAbilitySlots[abilityId].CanUnlock();
                else return HumanAbilitySlots[abilityId].CanUpgrade(upgradeId);
            }

            return false;
        }

        // Private Utility Functions ------------------------------------------
        private void LoadSupernaturalAbilities(string[] abilityJsonFiles)
        {
            SupernaturalAbilitySlots = new Dictionary<string, AbilitySlot>();
            foreach (string abilityJsonFile in abilityJsonFiles)
            {
                AbilitySlot abilitySlot = TechTreeUtilities.LoadAbilitySlotFromJson(JObject.Parse(File.ReadAllText(abilityJsonFile)), OwningPlayer);
                if (abilitySlot == null) continue;

                abilitySlot.TechTree = this;

                AbilitiesProgress.Add(abilitySlot.Id, new Dictionary<string, bool>());
                foreach (var abilityLevel in abilitySlot.AbilityLevelsList)
                    AbilitiesProgress[abilitySlot.Id].Add(abilityLevel.Key, false);

                SupernaturalAbilitySlots.Add(abilitySlot.Id, abilitySlot);
            }
        }

        private void LoadHumanAbilities(string[] abilityJsonFiles)
        {
            HumanAbilitySlots = new Dictionary<string, AbilitySlot>();
            foreach (string abilityJsonFile in abilityJsonFiles)
            {
                AbilitySlot abilitySlot = TechTreeUtilities.LoadAbilitySlotFromJson(JObject.Parse(File.ReadAllText(abilityJsonFile)), OwningPlayer);
                if (abilitySlot == null) continue;

                abilitySlot.TechTree = this;

                AbilitiesProgress.Add(abilitySlot.Id, new Dictionary<string, bool>());
                foreach (var abilityLevel in abilitySlot.AbilityLevelsList)
                    AbilitiesProgress[abilitySlot.Id].Add(abilityLevel.Key, false);

                HumanAbilitySlots.Add(abilitySlot.Id, abilitySlot);
            }
        }
    }
}