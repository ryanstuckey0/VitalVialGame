using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace ViralVial
{
    public class LossView : BaseView
    {
        public UnityAction OnMainMenu;
        public UnityAction OnQuit;
        public UnityAction OnTryAgain;

        public void GoToMainMenu()
        {
            OnMainMenu?.Invoke();
        }

        public void GoToMainMenu(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.performed) return;
            OnMainMenu?.Invoke();
        }

        public void GoToQuit()
        {
            OnQuit?.Invoke();
        }

        public void GoToTryAgain()
        {
            OnTryAgain?.Invoke();
        }
    }
}
