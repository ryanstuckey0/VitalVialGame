using RPGCharacterAnims;
using UnityEngine;
using ViralVial.Input;
using ViralVial.Player.Animation;
using ViralVial.Sound;
using ViralVial.Weapons;

namespace ViralVial.Player.MonoBehaviourScript
{
    /// <summary>
    /// Enables central access to scripts used by the player.
    /// </summary>
    public class BasePlayerController : MonoBehaviour, IDamageable
    {
        [Header("Player MonoBehaviour Scripts")]
        public PlayerMovementController PlayerMovementController;
        public RPGCharacterController PlayerAnimationController;
        public PlayerEquipmentController PlayerEquipmentController;
        public PlayerWeaponAnimationController PlayerWeaponAnimationController;
        public PlayerAnimationEvents PlayerAnimationEvents;
        public PlayerAnimationGameObjects PlayerAnimationGameObjects;
        public PlayerCrosshairController PlayerCrosshairController;
        public GameObjectAudioController PlayerAudioController;
        public IKHands PlayerIKHandsController;
        public PlayerInputController PlayerInputController;

        [Header("Other Important References")]
        public Animator PlayerAnimator;

        [Header("Ability Prefabs")]
        public GameObject MindControlSelectorCirclePrefab;
        public GameObject MindControlHandAnimationPrefab;
        public GameObject TimeFreezeAnimationPrefab;
        public GameObject ShockWaveAnimationPrefab;
        public GameObject BlinkAnimationPrefab;
        public GameObject FireAttackPrefab;
        public GameObject FireAttackHandPrefab;
        public GameObject HealthBoostAnimationPrefab;
        public GameObject LevelUpAnimationPrefab;

        // Fields not configureable in editor
        public float Health
        {
            get { return OwningPlayer.Health; }
            set { OwningPlayer.Health = value; }
        }
        public float CurrentMaxHealth
        {
            get { return OwningPlayer.MaxHealth; }
            set { OwningPlayer.MaxHealth = value; }
        }
        public float Exp
        {
            get { return OwningPlayer.Experience; }
            set { OwningPlayer.Experience = value; }
        }
        public int Level
        {
            get { return OwningPlayer.Level; }
            set { OwningPlayer.Level = value; }
        }
        public int SkillPoints
        {
            get { return OwningPlayer.SkillPoints; }
            set { OwningPlayer.SkillPoints = value; }
        }

        public ViralVialPlayerInput PlayerInput;
        public IPlayer OwningPlayer { get; private set; }
        private bool initialized = false;
        private void Awake() { Init(); }
        public IPlayer Init()
        {
            if (initialized)
            {
                Debug.LogWarning("This player has already been initialized but you attempted to initialize it again.");
                return OwningPlayer;
            }

            OwningPlayer = new BasePlayer(this);
            initialized = true;
            return OwningPlayer;
        }

        public void TakeDamage(float damageAmount)
        {
            if (OwningPlayer.IsDead) return;
            Health -= damageAmount;
            PlayerAudioController.PlayAudio($"OnHurt{Random.Range(1, 9)}");
        }

        private void OnDestroy()
        {
            OwningPlayer.OnDestroy();
        }
    }
}
