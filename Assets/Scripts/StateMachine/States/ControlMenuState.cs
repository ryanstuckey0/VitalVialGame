using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace ViralVial
{
    public class ControlMenuState : BaseState
    {
        private ControlMenuView view;

        public override void Setup()
        {
            base.Setup();

            //Load main menu scene and get view
            var scene = SceneManager.LoadScene("ControlMenu", new LoadSceneParameters(LoadSceneMode.Additive));
            StateMachine.StartCoroutine(SetupView(scene));
        }

        private IEnumerator SetupView(Scene scene)
        {
            while (!scene.isLoaded) yield return null;
            view = Object.FindObjectOfType<ControlMenuView>(true);
            view.OnMainMenu += GoToMainMenu;

            view.EventSystem?.gameObject.SetActive(true);

            view.Show();
        }

        public override void Cleanup()
        {
            view.EventSystem?.gameObject.SetActive(false);
            view.Hide();
            view.OnMainMenu -= GoToMainMenu;
            //Unload scene
            SceneManager.UnloadSceneAsync("ControlMenu", UnloadSceneOptions.None);
            view = null;

            base.Cleanup();
        }

        private void GoToMainMenu()
        {
            StateMachine.ChangeState(new MainMenuState());
        }

        private void GoToMainMenu(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.performed) return;
            GoToMainMenu();
        }
    }
}
