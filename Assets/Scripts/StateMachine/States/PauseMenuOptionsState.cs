using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ViralVial
{
    public class PauseMenuOptionsState : BaseState
    {
        private PauseMenuOptionsView view;
        private PauseState pauseState;

        public PauseMenuOptionsState(PauseState pauseState)
        {
            this.pauseState = pauseState;
        }


        public override void Setup()
        {
            base.Setup();

            Time.timeScale = 0;

            //Load save scene and get view
            var scene = SceneManager.LoadScene("PauseMenuOptions", new LoadSceneParameters(LoadSceneMode.Additive));
            StateMachine.StartCoroutine(SetupView(scene));
        }

        private IEnumerator SetupView(Scene scene)
        {
            while (!scene.isLoaded) yield return null;
            view = Object.FindObjectOfType<PauseMenuOptionsView>(true);

            view.OnPause += GoToPause;

            view.EventSystem?.gameObject.SetActive(true);
            view.Show();
        }

        public override void Cleanup()
        {
            view.EventSystem?.gameObject.SetActive(false);
            view.Hide();
            view.OnPause -= GoToPause;

            Time.timeScale = 1;

            //Unload scene
            SceneManager.UnloadSceneAsync("PauseMenuOptions", UnloadSceneOptions.None);
            view = null;

            base.Cleanup();
        }

        public void GoToPause()
        {
            StateMachine.ChangeState(pauseState);
        }
    }
}
