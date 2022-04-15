using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ViralVial
{
    public class OptionsView : BaseView
    {
        public UnityAction OnMainMenu;
        public void GoToMainMenu()
        {
            OnMainMenu?.Invoke();
        }

        public void GoToMainMenu(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.performed) return;
            OnMainMenu?.Invoke();
        }
    }
}
