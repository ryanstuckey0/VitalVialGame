using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ViralVial.Player;

namespace ViralVial
{
    public class SaveState : BaseState
    {
        private SaveView view;
        private RunState game;

        public SaveState(RunState game)
        {

            this.game = game;
        }


        public override void Setup()
        {
            base.Setup();

            Time.timeScale = 0;

            //Load save scene and get view
            var scene = SceneManager.LoadScene("SaveScene", new LoadSceneParameters(LoadSceneMode.Additive));
            StateMachine.StartCoroutine(SetupView(scene));
        }

        private IEnumerator SetupView(Scene scene)
        {
            while (!scene.isLoaded) yield return null;
            view = Object.FindObjectOfType<SaveView>(true);
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
            SceneManager.UnloadSceneAsync("SaveScene", UnloadSceneOptions.None);
            view = null;

            base.Cleanup();
        }

        public void GoToPause()
        {
            StateMachine.ChangeState(new PauseState(game));
        }
    }
}
