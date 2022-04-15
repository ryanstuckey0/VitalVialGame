using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ViralVial
{
    public class PauseMenuOptionsView : BaseView
    {
        public UnityAction OnPause;

        public void GoToPause()
        {
            OnPause?.Invoke();
        }

        public void GoToPause(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.performed) return;
            GoToPause();
        }
    }
}
