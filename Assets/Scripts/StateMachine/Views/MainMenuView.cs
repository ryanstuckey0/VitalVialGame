using UnityEngine;
using UnityEngine.Events;

namespace ViralVial
{
    public class MainMenuView : BaseView
    {
        public UnityAction OnGame;
        public UnityAction OnLoad;
        public UnityAction OnControl;
        public UnityAction OnOptions;
        public UnityAction OnCredits;
        public UnityAction OnQuit;

        public GameObject credits;

        public void GoToGame()
        {
            OnGame?.Invoke();
        }

        public void GoToLoad()
        {
            OnLoad?.Invoke();
        }

        public void GoToControl()
        {
            OnControl?.Invoke();
        }

        public void GoToOptions() 
        {
            OnOptions?.Invoke();
        }
        public void GoToQuit()
        {
            OnQuit?.Invoke();
        }
        public void GoToCredits() 
        {
            if (!credits.gameObject.activeSelf)
            {
                credits.gameObject.SetActive(true);
            }
        }
    }

}
