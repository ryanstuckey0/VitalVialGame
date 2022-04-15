using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using ViralVial.Ability;

namespace ViralVial.Player.TechTreeCode
{
    public static class TechTreeUtilities
    {
        public const string AbilitiesClassNamespace = "ViralVial.Ability.";

        public static AbilitySlot LoadAbilitySlotFromJson(JObject abilityJson, IPlayer owningPlayer)
        {
            string abilityName = (string)abilityJson["name"];
            string className = AbilitiesClassNamespace + (string)abilityJson["c#class"];
            Type classType = Type.GetType(className);
            IAbility ability;
            if (classType == null)
            {
                Debug.LogWarning($"Attempted to load ability {abilityName} but could not find C# Class {className}. Setting ability to null.");
                ability = null;
            }
            else
            {
                ability = GetAbilityClass(classType);
                ability.OwningPlayer = owningPlayer;
            }

            AbilitySlot abilitySlot = new AbilitySlot(id: (string)abilityJson["id"])
            {
                Ability = ability,
                AbilityName = abilityName,
                PrereqAbilitiesAND = ((JArray)abilityJson["prereqs"]["AND"]).ToObject<string[]>(),
                PrereqAbilitiesOR = ((JArray)abilityJson["prereqs"]["OR"]).ToObject<string[]>(),
                AbilityLevelsList = GetAbilityLevels((JArray)abilityJson["levels"]),
            };

            return abilitySlot;
        }

        private static IAbility GetAbilityClass(Type classType)
        {
            IAbility ability = (IAbility)Activator.CreateInstance(classType);
            return ability;
        }

        private static Dictionary<string, AbilityLevel> GetAbilityLevels(JArray abilityLevelsArray)
        {
            Dictionary<string, AbilityLevel> abilityLevelsDict = new Dictionary<string, AbilityLevel>();

            foreach (JObject abilityLevelJson in abilityLevelsArray)
            {
                AbilityLevel abilityLevel = new AbilityLevel()
                {
                    id = (string)abilityLevelJson["number"],
                    name = (string)abilityLevelJson["name"],
                    description = (string)abilityLevelJson["description"],
                    prereqs = ((JArray)abilityLevelJson["prereqs"])?.ToObject<string[]>(),
                    effect = (string)abilityLevelJson["effect"],
                    affectedEnemies = ((JArray)abilityLevelJson["Enemies"])?.ToObject<string[]>(),
                    childAbility = ((JObject)abilityLevelJson["ability"])?.ToObject<Dictionary<string, string>>(),
                    costSkillPoints = (int)abilityLevelJson["cost"],
                    stats = ((JObject)abilityLevelJson["stats"])?.ToObject<Dictionary<string, float>>(),

                };

                abilityLevelsDict.Add((string)abilityLevelJson["number"], abilityLevel);
            }

            return abilityLevelsDict;
        }
    }
}
