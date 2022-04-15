using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ViralVial
{
    public class WinView : BaseView
    {
        public UnityAction OnMainMenu;
        public UnityAction OnQuit;

        public void GoToMainMenu()
        {
            OnMainMenu?.Invoke();
        }

        public void GoToQuit()
        {
            OnQuit?.Invoke();
        }
    }
}
