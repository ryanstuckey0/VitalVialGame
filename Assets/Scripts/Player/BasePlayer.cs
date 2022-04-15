using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViralVial.Input;
using ViralVial.Player.Animation;
using ViralVial.Player.MonoBehaviourScript;
using ViralVial.Player.TechTreeCode;
using ViralVial.SaveSystem;
using ViralVial.Utilities;

namespace ViralVial.Player
{
    public class BasePlayer : IPlayer
    {
        public BasePlayerController BasePlayerController { get; private set; }

        public EquipmentManager EquipmentManager { get; private set; }
        public Inventory Inventory { get; private set; }
        public PermittedActions PermittedActions { get; private set; }
        private PlayerAnimationController PlayerAnimationController;
        public PlayerAttributes PlayerAttributes { get; private set; }
        public TechTree TechTree { get; private set; }

        public GameObject GameObject { get => BasePlayerController?.gameObject; }
        public Transform Transform { get => BasePlayerController?.transform; }
        public Vector3 CrosshairCenterPosition { get => EquipmentManager.EquippedGunGameObject?.transform.position ?? Transform.position; }

        public bool Animating { get => BasePlayerController.PlayerAnimationEvents.Animating; set => BasePlayerController.PlayerAnimationEvents.Animating = value; }

        private Dictionary<string, object> hudEventDictionary;

        private bool healthRegenerating = false;
        private CoroutineRunner regenerateHealthCoroutine;
        public bool Invincible { get; set; }
        public bool IsDead { get; private set; }
        private float health;
        public float Health
        {
            get { return health; }
            set
            {
                if (Invincible || health <= 0) return;
                health = value > MaxHealth ? MaxHealth : value;
                if (health <= 0) OnDeath();
                else if (!healthRegenerating && health < PlayerAttributes.LowHealthRegenThreshold)
                {
                    healthRegenerating = true;
                    regenerateHealthCoroutine.Start(RegenerateHealthCoroutine());
                }
                hudEventDictionary["health"] = health;
                RefreshHud();
            }
        }

        private float maxHealth;
        public float MaxHealth
        {
            get { return maxHealth; }
            set
            {
                maxHealth = value;
                hudEventDictionary["maxHealth"] = maxHealth;
                RefreshHud();
            }
        }

        private float experienceToLevelUp_backingField;
        private float experienceToLevelUp
        {
            get { return experienceToLevelUp_backingField; }
            set
            {
                experienceToLevelUp_backingField = value;
                hudEventDictionary["experienceToLevelUp"] = experienceToLevelUp_backingField;
                RefreshHud();
            }
        }
        private float experience;
        public float Experience
        {
            get { return experience; }
            set
            {
                experience = CheckForLevelUp(value);
                hudEventDictionary["experience"] = experience;
                RefreshHud();
            }
        }

        private int level;
        public int Level
        {
            get { return level; }
            set
            {
                level = value;
                ForceUpdateExperienceToNextLevel();
                hudEventDictionary["level"] = level;
                RefreshHud();
            }
        }

        private int skillPoints;
        public int SkillPoints
        {
            get { return skillPoints; }
            set
            {
                skillPoints = value;
            }
        }

        public BasePlayer(BasePlayerController basePlayerController)
        {
            hudEventDictionary = new Dictionary<string, object> {
                {"health", null},
                {"maxHealth", null},
                {"level", null},
                {"experience", null},
                {"experienceToLevelUp", null}
            };

            BasePlayerController = basePlayerController;

            PermittedActions = new PermittedActions();
            PlayerAttributes = new PlayerAttributes();

            Inventory = new Inventory(this);
            PlayerAnimationController = new PlayerAnimationController(this);
            EquipmentManager = new EquipmentManager(this);
            TechTree = new TechTree(this);

            Inventory.UnlockItem(InventoryItem.Pistol);
            Inventory.AddToInventory(InventoryItem.PistolSMGAmmo, 20);

            health = PlayerConstants.StartingHealth;
            maxHealth = PlayerConstants.StartingHealth;
            experience = PlayerConstants.StartingExperience;
            level = PlayerConstants.StartingLevel;
            experienceToLevelUp_backingField = PlayerConstants.StartingExperienceToLevelUp;
            InitializeHud();

            regenerateHealthCoroutine = new CoroutineRunner();

            EventManager.Instance.SubscribeToEvent("AddExp", OnAddExp);
            EventManager.Instance.SubscribeToEvent("AddHeal", OnMaxHealth);
            EventManager.Instance.SubscribeToEvent("AddPlayerAttribute", OnAddPlayerAttribute);
        }

        public void OnDestroy()
        {
            EventManager.Instance.UnsubscribeFromEvent("AddExp", OnAddExp);
            EventManager.Instance.UnsubscribeFromEvent("AddHeal", OnMaxHealth);
            EventManager.Instance.UnsubscribeFromEvent("AddPlayerAttribute", OnAddPlayerAttribute);

            PermittedActions.OnDestroy();
            PlayerAttributes.OnDestroy();
            Inventory.OnDestroy();
            PlayerAnimationController.OnDestroy();
            EquipmentManager.OnDestroy();
            TechTree.OnDestroy();

        }

        public void LookAtFromScreenPoint(Vector2 screenPoint)
        {
            Plane playerPlane = new Plane(Vector3.up, Transform.position);
            Ray ray = Camera.main.ScreenPointToRay(screenPoint);
            float hitdist = 0.0f;
            if (playerPlane.Raycast(ray, out hitdist))
            {
                Vector3 targetPoint = ray.GetPoint(hitdist);
                Vector3 lookTarget = new Vector3(targetPoint.x - Transform.position.x, Transform.position.z - targetPoint.z, 0);
                BasePlayerController.PlayerAnimationController.SetFaceInput(lookTarget);
            }
        }

        public void LookAtFromWorldPoint(Vector3 worldPoint)
        {
            Vector3 lookTarget = new Vector3(worldPoint.x - Transform.position.x, Transform.position.z - worldPoint.z, 0);
            BasePlayerController.PlayerAnimationController.SetFaceInput(lookTarget);
        }

        /// <summary>
        /// Triggers an action to be done on the animator.
        /// </summary>
        /// <param name="animator">player animator to use</param>
        /// <param name="trigger">action to be done</param>
        public void SetWholeBodyAnimatorTrigger(AnimatorTrigger trigger)
        {
            BasePlayerController.PlayerAnimator.SetInteger("TriggerNumber", (int)trigger);
            BasePlayerController.PlayerAnimator.SetTrigger("Trigger");
        }

        /// <summary>
        /// Triggers an action to be done on the animator.
        /// </summary>
        /// <param name="animator">player animator to use</param>
        /// <param name="trigger">action to be done</param>
        public void SetUpperBodyAnimatorTrigger(AnimatorTrigger trigger)
        {
            BasePlayerController.PlayerAnimator.SetInteger("UpperBodyTriggerNumber", (int)trigger);
            BasePlayerController.PlayerAnimator.SetTrigger("UpperBodyTrigger");
        }

        public void OnDeath()
        {
            Health = 0;
            IsDead = true;
            PermittedActions.ChangeAll(false);
            PermittedActions.LockPlayer();
            SetUpperBodyAnimatorTrigger(AnimatorTrigger.EndAnimationTrigger);
            SetWholeBodyAnimatorTrigger(AnimatorTrigger.DeathTrigger);
            BasePlayerController.PlayerWeaponAnimationController.SetIKHandsEnabled(false);

            Functions.CreateNewTimer(1, PlayDeathAudio);
            Functions.CreateNewTimer(3, InvokeDeathEvent);
        }

        private void PlayDeathAudio()
        {
            BasePlayerController.PlayerAudioController.PlayAudio($"OnDeath{UnityEngine.Random.Range(1, 3)}");
        }

        private void InvokeDeathEvent()
        {
            EventManager.Instance.InvokeEvent("PlayerDeath");
        }

        public void OnAddExp(Dictionary<string, object> args)
        {
            Experience += (float)args["experience"];
        }

        public void OnMaxHealth()
        {
            Health = MaxHealth;
        }

        public void OnAddPlayerAttribute(Dictionary<string, object> args)
        {
            switch (args["attribute"])
            {
                case "movementSpeed":
                    PlayerAttributes.MovementSpeed = PlayerAttributes.MovementSpeed + 0.1f;
                    break;
                case "gunDamageMultiplier":
                    PlayerAttributes.GunDamageMultiplier = PlayerAttributes.GunDamageMultiplier + 0.05f;
                    break;
                case "gunMagazineSizeMultiplier":
                    PlayerAttributes.GunMagazineSizeMultiplier = PlayerAttributes.GunMagazineSizeMultiplier + 0.1f;
                    break;
                case "meleeSpeedMultiplier":
                    PlayerAttributes.MeleeSpeedMultiplier = PlayerAttributes.MeleeSpeedMultiplier + 0.1f;
                    break;
                case "meleeDamageMultiplier":
                    PlayerAttributes.MeleeDamageMultiplier = PlayerAttributes.MeleeDamageMultiplier + 0.1f;
                    break;
                case "reloadSpeedMultiplier":
                    PlayerAttributes.ReloadSpeedMultiplier = PlayerAttributes.ReloadSpeedMultiplier + 0.2f;
                    break;
                case "postDashSpeedBoostDuration":
                    PlayerAttributes.PostDashSpeedBoostDuration = PlayerAttributes.PostDashSpeedBoostDuration + 0.1f;
                    break;
            }
        }

        public PlayerInfo RetrievePlayerInfo()
        {
            return new PlayerInfo()
            {
                Health = this.Health,
                currentMaxHealth = MaxHealth,
                Exp = Experience,
                Level = this.Level,
                SkillPoints = this.SkillPoints,
                position = SaveLoadSystem.ConvertToSerializableVector3(Transform.position),
                abilitiesProgress = TechTree.AbilitiesProgress,
                Loadout = EquipmentManager.RetrievePlayerLoadout(),
                Inventory = Inventory.RetrievePlayerInventory()
            };
        }

        public void Initialize(PlayerInfo playerInfo)
        {
            Health = playerInfo.Health;
            MaxHealth = playerInfo.currentMaxHealth;
            Experience = playerInfo.Exp;
            Level = playerInfo.Level;
            experienceToLevelUp = PlayerConstants.StartingExperienceToLevelUp * ((float)Math.Pow(PlayerConstants.ExperienceIncreaseRate, Level - PlayerConstants.StartingLevel));
            SkillPoints = playerInfo.SkillPoints;
            Transform.position = SaveLoadSystem.ConvertFromSerializableVector3(playerInfo.position);
            TechTree.Initialize(playerInfo.abilitiesProgress);
            EquipmentManager.Initialize(playerInfo.Loadout);
            Inventory.Initialize(playerInfo.Inventory);
        }

        private float CheckForLevelUp(float experience)
        {
            if (experience >= experienceToLevelUp)
            {
                float remainingExperience = experience - experienceToLevelUp;

                Level++;
                MaxHealth += 10f;
                Health = MaxHealth;
                SkillPoints += (Level / 10 + 1) * PlayerConstants.LevelUpSkillPointMultiplier;

                BasePlayerController.PlayerAudioController.PlayAudio("OnLevelUp");

                GameObject.Destroy(GameObject.Instantiate(BasePlayerController.LevelUpAnimationPrefab, Transform.position, Transform.rotation, Transform), 5);

                experience = remainingExperience;
            }
            return experience;
        }

        private void ForceUpdateExperienceToNextLevel()
        {
            experienceToLevelUp = PlayerConstants.StartingExperienceToLevelUp * ((float)Math.Pow(PlayerConstants.ExperienceIncreaseRate, Level - PlayerConstants.StartingLevel));
        }

        /// <summary>
        /// Invokes all HUD related events to update it. Uses the EventManager's wait for subscriber method to invoke the event
        /// even if it does not yet have subscribers. Only needs to be called at beginning of game in case the Base Player loads 
        /// before the HUD can subscribe to all its event.
        /// </summary>
        private void InitializeHud()
        {
            hudEventDictionary["health"] = Health;
            hudEventDictionary["maxHealth"] = MaxHealth;
            hudEventDictionary["level"] = Level;
            hudEventDictionary["experience"] = Experience;
            hudEventDictionary["experienceToLevelUp"] = experienceToLevelUp;
            EventManager.Instance.InvokeEventOrWaitOnSubscriber("UpdateHudPlayerInfo", hudEventDictionary);
        }

        private void RefreshHud()
        {
            EventManager.Instance.InvokeEvent("UpdateHudPlayerInfo", hudEventDictionary);
        }

        private IEnumerator RegenerateHealthCoroutine()
        {
            while (Health < PlayerAttributes.LowHealthRegenThreshold)
            {
                Health += PlayerAttributes.LowHealthRegenAmount;
                yield return CoroutineYielderCache.GetYielder("LowHealthRegenInterval");
            }
            healthRegenerating = false;
        }
    }
}
