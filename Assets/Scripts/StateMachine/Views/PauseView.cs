using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ViralVial
{
    public class PauseView : BaseView
    {
        public UnityAction OnResume;
        public UnityAction OnMainMenu;
        public UnityAction OnQuit;
        public UnityAction OnSave;
        public UnityAction OnOptions;

        public void GoToResume()
        {
            OnResume?.Invoke();
        }

        public void GoToResume(InputAction.CallbackContext value)
        {
            if (value.performed) OnResume?.Invoke();
        }

        public void GoToMainMenu()
        {
            OnMainMenu?.Invoke();
        }

        public void GoToQuit()
        {
            OnQuit?.Invoke();
        }

        public void GoToSave()
        {
            OnSave?.Invoke();
        }

        public void GoToOptions()
        {
            OnOptions?.Invoke();
        }
    }
}
