using System.Collections.Generic;
using UnityEngine;
using ViralVial.Player.MonoBehaviourScript;
using ViralVial.Player.TechTreeCode;
using ViralVial.SaveSystem;

namespace ViralVial.Player
{
    public interface IPlayer
    {
        BasePlayerController BasePlayerController { get; }

        EquipmentManager EquipmentManager { get; }
        TechTree TechTree { get; }
        Inventory Inventory { get; }
        PermittedActions PermittedActions { get; }
        PlayerAttributes PlayerAttributes { get; }

        Transform Transform { get; }
        Vector3 CrosshairCenterPosition { get; }
        GameObject GameObject { get; }
        bool Animating { get; set; }

        bool IsDead { get; }
        bool Invincible { get; set; }
        float Health { get; set; }
        float MaxHealth { get; set; }
        float Experience { get; set; }
        int Level { get; set; }
        int SkillPoints { get; set; }

        void Initialize(PlayerInfo playerInfo);
        PlayerInfo RetrievePlayerInfo();

        void LookAtFromScreenPoint(Vector2 screenPoint);
        void LookAtFromWorldPoint(Vector3 worldPoint);

        void SetWholeBodyAnimatorTrigger(AnimatorTrigger trigger);
        void SetUpperBodyAnimatorTrigger(AnimatorTrigger trigger);

        void OnAddExp(Dictionary<string, object> args);
        void OnDeath();
        void OnDestroy();
    }
}
