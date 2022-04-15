using UnityEngine.Events;
using UnityEngine.InputSystem;
using ViralVial.SaveSystem;

namespace ViralVial
{
    public class LoadView : BaseView
    {
        public UnityAction OnMainMenu;
        public UnityAction<SaveSlot> OnLoadSave;

        public void GoToMainMenu()
        {
            OnMainMenu?.Invoke();
        }

        public void GoToMainMenu(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.performed) return;
            GoToMainMenu();
        }


        public void LoadSave(SaveSlot saveSlot)
        {
            OnLoadSave?.Invoke(saveSlot);
        }
    }
}
