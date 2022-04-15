using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViralVial.Player;
using ViralVial.Player.TechTreeCode;
using ViralVial.Utilities;

namespace ViralVial.Ability.Supernatural.HealthBoost
{
    public class HealthBoostAbility : IAbility
    {
        public IPlayer OwningPlayer { get; set; }
        public AbilityType AbilityType { get; } = AbilityType.Supernatural;

        private float duration;
        private float playerSpeed;
        private float healthPerExp;

        private CoroutineRunner useAbilityCoroutine;

        public HealthBoostAbility()
        {
            useAbilityCoroutine = new CoroutineRunner();
        }

        public void OnDestroy()
        {
            EventManager.Instance.UnsubscribeFromEvent("AddExp", OnAddExp);
        }

        public bool UseAbility()
        {
            useAbilityCoroutine.Start(UseAbilityCoroutine());
            return true;
        }

        public void UpgradeAbility(AbilityLevel abilityUpgrade)
        {
            float tempFloat;
            if (abilityUpgrade.stats.TryGetValue("HealthPerExp", out tempFloat)) healthPerExp = tempFloat;
            if (abilityUpgrade.stats.TryGetValue("Duration", out tempFloat)) duration = tempFloat;
            if (abilityUpgrade.stats.TryGetValue("Player.Speed", out tempFloat)) playerSpeed = tempFloat;
        }

        private IEnumerator UseAbilityCoroutine()
        {
            EventManager.Instance.UnsubscribeFromEvent("AddExp", OwningPlayer.OnAddExp);
            EventManager.Instance.SubscribeToEvent("AddExp", OnAddExp);

            Object.Destroy(
                Object.Instantiate(
                    OwningPlayer.BasePlayerController.HealthBoostAnimationPrefab,
                    OwningPlayer.Transform.position + OwningPlayer.Transform.up,
                    OwningPlayer.Transform.rotation * Quaternion.Euler(-90, 0, 0),
                    OwningPlayer.Transform),
                duration);

            OwningPlayer.PlayerAttributes.MovementSpeed *= playerSpeed;

            yield return new WaitForSeconds(duration);

            OwningPlayer.PlayerAttributes.MovementSpeed /= playerSpeed;

            EventManager.Instance.UnsubscribeFromEvent("AddExp", OnAddExp);
            EventManager.Instance.SubscribeToEvent("AddExp", OwningPlayer.OnAddExp);
        }

        private void OnAddExp(Dictionary<string, object> args)
        {
            OwningPlayer.Health += (float)args["experience"] * healthPerExp;
        }
    }
}