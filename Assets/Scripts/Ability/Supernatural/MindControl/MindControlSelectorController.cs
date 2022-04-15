using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ViralVial.Player;
using ViralVial.Utilities;

namespace ViralVial.Ability.Supernatural.MindControl
{
    public class MindControlSelectorController : MonoBehaviour
    {
        public GameObject MindControlMouseSelectorCircle;
        private IPlayer owningPlayer;

        private float YPositionOffset = 0.1f;
        private float timeoutSeconds;
        private bool savedCursorState;
        private readonly float movementSpeedOnGamepad = Screen.height / 50;
        private Vector2 previousScreenPosition = Vector2.zero;
        private bool finishedSelection = false;
        private Vector2 inputPosition = new Vector2(Screen.width / 2, Screen.height / 2);
        private InputActionMap savedActiveActionMap;
        private const string actionMapToUse = "Abilities/MindControl";


        public void Init(IPlayer owningPlayer, GameObject selectorCirclePrefab, float timeout)
        {
            timeoutSeconds = timeout;

            this.owningPlayer = owningPlayer;
            MindControlMouseSelectorCircle = Instantiate(selectorCirclePrefab, owningPlayer.Transform);

            savedActiveActionMap = owningPlayer.BasePlayerController.PlayerInputController.CurrentActionMap;
            owningPlayer.BasePlayerController.PlayerInput.AbilitiesMindControl.MoveSelectorCircle.performed += OnMoveSelectorCircle;
            owningPlayer.BasePlayerController.PlayerInput.AbilitiesMindControl.ConfirmSelection.performed += ConfirmSelection_performed;
            owningPlayer.BasePlayerController.PlayerInputController.SwitchActionMap(owningPlayer.BasePlayerController.PlayerInput.AbilitiesMindControl);

            savedCursorState = Cursor.visible;
            EventManager.Instance.InvokeEvent("CrosshairActive", new Dictionary<string, object>() { { "active", false } });
            UpdateSelectorCirclePosition(owningPlayer.BasePlayerController.PlayerCrosshairController.CrosshairScreenSpace.transform.position);
            StartCoroutine(TimeoutCoroutine());
        }

        private void FixedUpdate()
        {
            if ((Gamepad.current?.rightStick.ReadValue().magnitude ?? 0) != 0)
                inputPosition = Gamepad.current.rightStick.ReadValue() * movementSpeedOnGamepad + previousScreenPosition;

            if (inputPosition.x > Screen.width) inputPosition.x = Screen.width;
            if (inputPosition.x < 0) inputPosition.x = 0;
            if (inputPosition.y > Screen.height) inputPosition.y = Screen.height;
            if (inputPosition.y < 0) inputPosition.y = 0;

            UpdateSelectorCirclePosition(inputPosition);
        }

        private void OnMoveSelectorCircle(InputAction.CallbackContext callbackContext)
        {
            if (finishedSelection) return;

            if (Mouse.current.wasUpdatedThisFrame) inputPosition = Mouse.current.position.ReadValue();
        }

        private void UpdateSelectorCirclePosition(Vector2 screenPosition)
        {
            previousScreenPosition = screenPosition;

            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask(Constants.GroundLayerName)))
            {
                Vector3 newPosition = hit.point;
                newPosition.y += YPositionOffset;
                owningPlayer.LookAtFromWorldPoint(newPosition);
                float distance = Vector3.Distance(owningPlayer.Transform.position, newPosition);
                MindControlMouseSelectorCircle.transform.position = owningPlayer.BasePlayerController.PlayerAnimationGameObjects.LeftHand.transform.position + owningPlayer.Transform.forward * distance;
                MindControlMouseSelectorCircle.transform.position = new Vector3(MindControlMouseSelectorCircle.transform.position.x,
                                                                                owningPlayer.BasePlayerController.PlayerAnimationGameObjects.LeftHand.transform.position.y - 1,
                                                                                MindControlMouseSelectorCircle.transform.position.z);
            }
        }

        private void ConfirmSelection_performed(InputAction.CallbackContext callbackContext)
        {
            if (finishedSelection) return;
            finishedSelection = true;
            FinishSelection(timedOut: false);
        }

        private void FinishSelection(bool timedOut)
        {
            // invoke event to tell MindControlAbility class we have finished selecting an enemy
            Dictionary<string, object> dictArgs = new Dictionary<string, object>()
            {
                { "originOfMindControl", MindControlMouseSelectorCircle.transform.position },
                { "timedOut", timedOut ? true : false}
            };
            EventManager.Instance.InvokeEvent("SelectMindControlEnemies", dictArgs);
            Cursor.visible = savedCursorState;
            Destroy(MindControlMouseSelectorCircle);
            Destroy(this);
        }

        private IEnumerator TimeoutCoroutine()
        {
            yield return new WaitForSeconds(timeoutSeconds);
            FinishSelection(true);
        }

        private void OnDestroy()
        {
            EventManager.Instance.InvokeEvent("CrosshairActive", new Dictionary<string, object>() { { "active", true } });
            owningPlayer.BasePlayerController.PlayerInputController.SwitchActionMap(savedActiveActionMap);
            owningPlayer.BasePlayerController.PlayerInput.AbilitiesMindControl.MoveSelectorCircle.performed -= OnMoveSelectorCircle;
            owningPlayer.BasePlayerController.PlayerInput.AbilitiesMindControl.ConfirmSelection.performed -= ConfirmSelection_performed;
        }
    }
}
