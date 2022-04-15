using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ViralVial
{
    public class SceneInputController : MonoBehaviour
    {
        public PlayerInput PlayerInput;
        public Selectable SelectOnSwitchToGamepad;

        private void Awake()
        {
            OnControlsChanged(PlayerInput);
        }

        public void OnControlsChanged(PlayerInput playerInput)
        {
            switch (playerInput.currentControlScheme)
            {
                case "Keyboard&Mouse": OnSwitchToKeyboard(); break;
                case "Gamepad": OnSwitchToGamepad(); break;
            }
        }

        private void OnSwitchToGamepad()
        {
            SelectOnSwitchToGamepad.Select();
            Cursor.visible = false;
        }

        private void OnSwitchToKeyboard()
        {
            Cursor.visible = true;
        }
    }
}
