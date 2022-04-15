using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ViralVial.Utilities;

namespace ViralVial.Player.MonoBehaviourScript
{
    public class PlayerCrosshairController : MonoBehaviour
    {
        public GameObject CrosshairScreenSpace;
        [SerializeField] private BasePlayerController basePlayerController;

        private const float gamepadRightStickDeadzone = 0.8f;
        private IPlayer player;
        private float crosshairDistanceFromPlayer = 0;

        private void Start()
        {
            player = basePlayerController.OwningPlayer;

            EventManager.Instance.SubscribeToEvent("CrosshairActive", OnCrosshairActive);
            Cursor.visible = false;

            player.LookAtFromWorldPoint(Vector3.forward);
            player.BasePlayerController.PlayerAnimationController.StartAction("Face");
        }

        private void FixedUpdate()
        {
            UpdateCrosshairPosition();
        }

        public void ChangeCrosshairActive(bool active)
        {
            CrosshairScreenSpace.SetActive(active);
            Cursor.visible = !active;
            if (active) player.LookAtFromScreenPoint(CrosshairScreenSpace.transform.position);
        }

        private void UpdateCrosshairPosition()
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, player.CrosshairCenterPosition + player.Transform.forward * crosshairDistanceFromPlayer);
            if (screenPoint.x < 0 ||
                screenPoint.x > Screen.width ||
                screenPoint.y < 0 ||
                screenPoint.y > Screen.height) return;
            CrosshairScreenSpace.transform.position = screenPoint;
        }

        public void MoveAimMouse_performed(InputAction.CallbackContext callbackContext)
        {
            if (!player.PermittedActions.MoveAim) return;

            Vector3 inputValue = Mouse.current.position.ReadValue();
            if (inputValue.x < 0 ||
                inputValue.x > Screen.width ||
                inputValue.y < 0 ||
                inputValue.y > Screen.height) return;
            UpdatePlayerLookDirection(inputValue);
        }

        public void MoveAimGamepad_performed(InputAction.CallbackContext callbackContext)
        {
            if (!player.PermittedActions.MoveAim) return;

            Vector2 stickPos = Gamepad.current.rightStick.ReadValue();
            if (stickPos.magnitude < gamepadRightStickDeadzone) return;
            Vector3 inputValue = RectTransformUtility.WorldToScreenPoint(Camera.main, player.Transform.position) + stickPos * Screen.height / 4;
            if (inputValue == Vector3.zero) return;

            UpdatePlayerLookDirection(inputValue);
        }

        private void UpdatePlayerLookDirection(Vector2 inputValue)
        {
            Ray ray = Camera.main.ScreenPointToRay(inputValue);
            RaycastHit hitInfo;
            Physics.Raycast(ray.origin, ray.direction, out hitInfo, Mathf.Infinity, LayerMask.GetMask(Constants.GroundLayerName));
            crosshairDistanceFromPlayer = Vector3.Distance(player.Transform.position, new Vector3(hitInfo.point.x, player.Transform.position.y, hitInfo.point.z));
            Vector3 worldSpacePosition = player.Transform.position + (hitInfo.point - player.Transform.position).normalized * crosshairDistanceFromPlayer;
            worldSpacePosition = new Vector3(worldSpacePosition.x, 0.1f, worldSpacePosition.z);

            player.LookAtFromWorldPoint(worldSpacePosition);
        }

        private void OnDestroy()
        {
            Cursor.visible = true;
            EventManager.Instance.UnsubscribeFromEvent("CrosshairActive", OnCrosshairActive);
        }

        private void OnCrosshairActive(Dictionary<string, object> dictArgs)
        {
            ChangeCrosshairActive((bool)dictArgs["active"]);
        }
    }
}
