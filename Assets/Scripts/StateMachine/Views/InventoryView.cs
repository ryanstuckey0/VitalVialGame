using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ViralVial
{
    public class InventoryView : BaseView
    {
        public UnityAction OnGame;

        public void GoToGame()
        {
            OnGame?.Invoke();
        }

        public void GoToGame(InputAction.CallbackContext value)
        {
            if (value.performed) OnGame?.Invoke();
        }
    }
}
