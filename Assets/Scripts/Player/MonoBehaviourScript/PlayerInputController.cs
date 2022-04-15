using UnityEngine;
using UnityEngine.InputSystem;
using ViralVial.ControlMenu;
using ViralVial.Input;
using ViralVial.Utilities;

namespace ViralVial.Player.MonoBehaviourScript
{
    public class PlayerInputController : MonoBehaviour
    {
        public ViralVialPlayerInput PlayerInput { get; private set; }
        public BasePlayerController BasePlayerController;
        public RunView RunView;

        public InputActionMap CurrentActionMap { get; private set; }
        private InputActionMap uiControllerActionMap;

        public void Awake()
        {
            PlayerInput = new ViralVialPlayerInput();
            BasePlayerController.PlayerInput = PlayerInput;
            uiControllerActionMap = PlayerInput.StatesRunState;

            LoadRebinds();

            EventManager.Instance.SubscribeToEvent("EnablePlayerInput", EnableInput);
            EventManager.Instance.SubscribeToEvent("DisablePlayerInput", DisableInput);
        }

        public void OnDestroy()
        {
            EventManager.Instance.UnsubscribeFromEvent("EnablePlayerInput", EnableInput);
            EventManager.Instance.UnsubscribeFromEvent("DisablePlayerInput", DisableInput);
        }

        public void OnEnable()
        {
            // Action Map: Player
            PlayerInput.Player.Move.started += BasePlayerController.PlayerMovementController.Move_performed;
            PlayerInput.Player.Move.performed += BasePlayerController.PlayerMovementController.Move_performed;
            PlayerInput.Player.Move.canceled += BasePlayerController.PlayerMovementController.Move_canceled;
            PlayerInput.Player.Dodge.performed += BasePlayerController.PlayerMovementController.Dodge_performed;

            PlayerInput.Player.Fire.started += BasePlayerController.PlayerEquipmentController.Fire_started;
            PlayerInput.Player.Fire.canceled += BasePlayerController.PlayerEquipmentController.Fire_canceled;
            PlayerInput.Player.UseAbilitySlot1.performed += BasePlayerController.PlayerEquipmentController.UseAbilitySlot1_performed;
            PlayerInput.Player.UseAbilitySlot2.performed += BasePlayerController.PlayerEquipmentController.UseAbilitySlot2_performed;
            PlayerInput.Player.UseAbilitySlot3.performed += BasePlayerController.PlayerEquipmentController.UseAbilitySlot3_performed;
            PlayerInput.Player.UseAbilitySlot4.performed += BasePlayerController.PlayerEquipmentController.UseAbilitySlot4_performed;
            PlayerInput.Player.Melee.performed += BasePlayerController.PlayerEquipmentController.Melee_performed;
            PlayerInput.Player.UseThrowable.performed += BasePlayerController.PlayerEquipmentController.UseThrowable_performed;
            PlayerInput.Player.Reload.performed += BasePlayerController.PlayerEquipmentController.Reload_performed;
            PlayerInput.Player.SwitchGun.performed += BasePlayerController.PlayerEquipmentController.SwitchGun_performed;
            PlayerInput.Player.Interact.performed += BasePlayerController.PlayerEquipmentController.Interact_performed;

            PlayerInput.Player.MoveAimMouse.performed += BasePlayerController.PlayerCrosshairController.MoveAimMouse_performed;
            PlayerInput.Player.MoveAimGamepad.performed += BasePlayerController.PlayerCrosshairController.MoveAimGamepad_performed;


            // Action Map: Abilities/ElementalAttack
            PlayerInput.AbilitiesElementalAttack.MoveAimMouse.performed += BasePlayerController.PlayerCrosshairController.MoveAimMouse_performed;
            PlayerInput.AbilitiesElementalAttack.MoveAimGamepad.performed += BasePlayerController.PlayerCrosshairController.MoveAimGamepad_performed;

            PlayerInput.AbilitiesElementalAttack.Move.started += BasePlayerController.PlayerMovementController.Move_performed;
            PlayerInput.AbilitiesElementalAttack.Move.performed += BasePlayerController.PlayerMovementController.Move_performed;
            PlayerInput.AbilitiesElementalAttack.Move.canceled += BasePlayerController.PlayerMovementController.Move_canceled;
            PlayerInput.AbilitiesElementalAttack.Dodge.performed += BasePlayerController.PlayerMovementController.Dodge_performed;

            PlayerInput.AbilitiesElementalAttack.UseAbilitySlot1.performed += BasePlayerController.PlayerEquipmentController.UseAbilitySlot1_performed;
            PlayerInput.AbilitiesElementalAttack.UseAbilitySlot2.performed += BasePlayerController.PlayerEquipmentController.UseAbilitySlot2_performed;
            PlayerInput.AbilitiesElementalAttack.UseAbilitySlot3.performed += BasePlayerController.PlayerEquipmentController.UseAbilitySlot3_performed;
            PlayerInput.AbilitiesElementalAttack.UseAbilitySlot4.performed += BasePlayerController.PlayerEquipmentController.UseAbilitySlot4_performed;
            PlayerInput.AbilitiesElementalAttack.Interact.performed += BasePlayerController.PlayerEquipmentController.Interact_performed;

            // Action Map: Abilities/MindControl
            PlayerInput.AbilitiesMindControl.Move.started += BasePlayerController.PlayerMovementController.Move_performed;
            PlayerInput.AbilitiesMindControl.Move.performed += BasePlayerController.PlayerMovementController.Move_performed;
            PlayerInput.AbilitiesMindControl.Move.canceled += BasePlayerController.PlayerMovementController.Move_canceled;

            // Action Map: Abilities/TurretPlacement
            PlayerInput.AbilitiesTurretPlacement.Move.started += BasePlayerController.PlayerMovementController.Move_performed;
            PlayerInput.AbilitiesTurretPlacement.Move.performed += BasePlayerController.PlayerMovementController.Move_performed;
            PlayerInput.AbilitiesTurretPlacement.Move.canceled += BasePlayerController.PlayerMovementController.Move_canceled;
            PlayerInput.AbilitiesTurretPlacement.MoveAimMouse.performed += BasePlayerController.PlayerCrosshairController.MoveAimMouse_performed;
            PlayerInput.AbilitiesTurretPlacement.MoveAimGamepad.performed += BasePlayerController.PlayerCrosshairController.MoveAimGamepad_performed;

            // Action Map: States/RunState
            if (RunView != null)
            {
                PlayerInput.StatesRunState.GoToInventory.performed += RunView.GoToInventory;
                PlayerInput.StatesRunState.GoToTechTree.performed += RunView.GoToTechTree;
                PlayerInput.StatesRunState.GoToPause.performed += RunView.GoToPause;
            }

            CurrentActionMap = PlayerInput.Player;
        }

        public void OnDisable()
        {
            PlayerInput.Disable();

            // Action Map: Player
            PlayerInput.Player.Move.started -= BasePlayerController.PlayerMovementController.Move_performed;
            PlayerInput.Player.Move.canceled -= BasePlayerController.PlayerMovementController.Move_canceled;
            PlayerInput.Player.Dodge.performed -= BasePlayerController.PlayerMovementController.Dodge_performed;

            PlayerInput.Player.Fire.started -= BasePlayerController.PlayerEquipmentController.Fire_started;
            PlayerInput.Player.Fire.canceled -= BasePlayerController.PlayerEquipmentController.Fire_canceled;
            PlayerInput.Player.UseAbilitySlot1.performed -= BasePlayerController.PlayerEquipmentController.UseAbilitySlot1_performed;
            PlayerInput.Player.UseAbilitySlot2.performed -= BasePlayerController.PlayerEquipmentController.UseAbilitySlot2_performed;
            PlayerInput.Player.UseAbilitySlot3.performed -= BasePlayerController.PlayerEquipmentController.UseAbilitySlot3_performed;
            PlayerInput.Player.UseAbilitySlot4.performed -= BasePlayerController.PlayerEquipmentController.UseAbilitySlot4_performed;
            PlayerInput.Player.Melee.performed -= BasePlayerController.PlayerEquipmentController.Melee_performed;
            PlayerInput.Player.UseThrowable.performed -= BasePlayerController.PlayerEquipmentController.UseThrowable_performed;
            PlayerInput.Player.Reload.performed -= BasePlayerController.PlayerEquipmentController.Reload_performed;
            PlayerInput.Player.SwitchGun.performed -= BasePlayerController.PlayerEquipmentController.SwitchGun_performed;
            PlayerInput.Player.Interact.performed -= BasePlayerController.PlayerEquipmentController.Interact_performed;

            PlayerInput.Player.MoveAimMouse.performed -= BasePlayerController.PlayerCrosshairController.MoveAimMouse_performed;
            PlayerInput.Player.MoveAimGamepad.performed -= BasePlayerController.PlayerCrosshairController.MoveAimGamepad_performed;


            // Action Map: Abilities/ElementalAttack
            PlayerInput.AbilitiesElementalAttack.MoveAimMouse.performed -= BasePlayerController.PlayerCrosshairController.MoveAimMouse_performed;
            PlayerInput.AbilitiesElementalAttack.MoveAimGamepad.performed -= BasePlayerController.PlayerCrosshairController.MoveAimGamepad_performed;

            PlayerInput.AbilitiesElementalAttack.Move.started -= BasePlayerController.PlayerMovementController.Move_performed;
            PlayerInput.AbilitiesElementalAttack.Move.canceled -= BasePlayerController.PlayerMovementController.Move_canceled;
            PlayerInput.AbilitiesElementalAttack.Dodge.performed -= BasePlayerController.PlayerMovementController.Dodge_performed;

            PlayerInput.AbilitiesElementalAttack.UseAbilitySlot1.performed -= BasePlayerController.PlayerEquipmentController.UseAbilitySlot1_performed;
            PlayerInput.AbilitiesElementalAttack.UseAbilitySlot2.performed -= BasePlayerController.PlayerEquipmentController.UseAbilitySlot2_performed;
            PlayerInput.AbilitiesElementalAttack.UseAbilitySlot3.performed -= BasePlayerController.PlayerEquipmentController.UseAbilitySlot3_performed;
            PlayerInput.AbilitiesElementalAttack.UseAbilitySlot4.performed -= BasePlayerController.PlayerEquipmentController.UseAbilitySlot4_performed;
            PlayerInput.AbilitiesElementalAttack.Interact.performed -= BasePlayerController.PlayerEquipmentController.Interact_performed;

            // Action Map: Abilities/MindControl
            PlayerInput.AbilitiesMindControl.Move.started -= BasePlayerController.PlayerMovementController.Move_performed;
            PlayerInput.AbilitiesMindControl.Move.canceled -= BasePlayerController.PlayerMovementController.Move_canceled;

            // Action Map: Abilities/TurretPlacement
            PlayerInput.AbilitiesTurretPlacement.Move.started -= BasePlayerController.PlayerMovementController.Move_performed;
            PlayerInput.AbilitiesTurretPlacement.Move.canceled -= BasePlayerController.PlayerMovementController.Move_canceled;
            PlayerInput.AbilitiesTurretPlacement.MoveAimMouse.performed -= BasePlayerController.PlayerCrosshairController.MoveAimMouse_performed;
            PlayerInput.AbilitiesTurretPlacement.MoveAimGamepad.performed -= BasePlayerController.PlayerCrosshairController.MoveAimGamepad_performed;

            // Action Map: States/RunState
            if (RunView != null)
            {
                PlayerInput.StatesRunState.GoToInventory.performed -= RunView.GoToInventory;
                PlayerInput.StatesRunState.GoToTechTree.performed -= RunView.GoToTechTree;
                PlayerInput.StatesRunState.GoToPause.performed -= RunView.GoToPause;
            }
        }

        public void EnableInput()
        {
            CurrentActionMap?.Enable();
            uiControllerActionMap.Enable();
        }

        public void DisableInput()
        {
            PlayerInput.Disable();
        }

        public void SwitchActionMap(InputActionMap newActionMap)
        {
            if (newActionMap.enabled) return;

            CurrentActionMap?.Disable();
            CurrentActionMap = newActionMap;
            CurrentActionMap.Enable();
        }

        private void LoadRebinds()
        {
            string rebinds = RebindingSaverLoader.LoadRebinds();
            if (rebinds == null) return;
            PlayerInput.LoadBindingOverridesFromJson(rebinds);
        }
    }
}
