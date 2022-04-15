using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ViralVial
{
    public class TechTreeState : BaseState
    {
        private RunState game;
        private TechTreeView view;

        public TechTreeState(RunState game = null)
        {
            this.game = game;
        }

        public override void Setup()
        {
            base.Setup();

            //Pause time for game
            Time.timeScale = 0f;

            //Load TechTree scene and get view
            var scene = SceneManager.LoadScene("TechTree", new LoadSceneParameters(LoadSceneMode.Additive));
            StateMachine.StartCoroutine(SetupView(scene));
        }

        private IEnumerator SetupView(Scene scene)
        {
            while (!scene.isLoaded) yield return null;
            view = Object.FindObjectOfType<TechTreeView>(true);
            view.OnGame += GoToRunState;

            view.EventSystem?.gameObject.SetActive(true);

            view.Show();
        }

        public override void Cleanup()
        {
            view.EventSystem?.gameObject.SetActive(false);

            //Cleanup view
            view.Hide();
            view.OnGame -= GoToRunState;

            //Unload scene
            SceneManager.UnloadSceneAsync("TechTree", UnloadSceneOptions.None);
            view = null;

            //Unpause time for game
            Time.timeScale = 1f;

            base.Cleanup();
        }

        private void GoToRunState()
        {
            game.LoadContent = false; //We do not want to reload content
            StateMachine.ChangeState(game);
        }

    }
}
