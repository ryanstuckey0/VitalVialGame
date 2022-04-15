using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ViralVial.ControlMenu
{
    public class SwitchControl : MonoBehaviour
    {
        public PlayerInput PlayerInput;
        public GameObject _keyboard;
        public GameObject _gamepad;
        public Button _keyboardButton;
        public Button _gamepadButton;

        private void Awake()
        {
            OnControlsChanged(PlayerInput);
        }

        public void OnControlsChanged(PlayerInput playerInput)
        {
            switch (playerInput.currentControlScheme)
            {
                case "Keyboard&Mouse": OnClickKeyboard(); break;
                case "Gamepad": OnClickGamepad(); break;
            }
        }

        public void OnClickKeyboard()
        {
            _keyboard.SetActive(true);
            _keyboardButton.interactable = false;
            _gamepad.SetActive(false);
            _gamepadButton.interactable = true;
        }

        public void OnClickGamepad()
        {
            _gamepad.SetActive(true);
            _gamepadButton.interactable = false;
            _keyboard.SetActive(false);
            _keyboardButton.interactable = true;
        }
    }
}
