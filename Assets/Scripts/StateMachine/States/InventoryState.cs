using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ViralVial
{
    public class InventoryState : BaseState
    {
        private RunState game;
        private InventoryView view;

        public InventoryState(RunState game)
        {
            this.game = game;
        }

        public override void Setup()
        {
            base.Setup();

            //Pause time for game
            Time.timeScale = 0f;

            //Load TechTree scene and get view
            var scene = SceneManager.LoadScene("Inventory", new LoadSceneParameters(LoadSceneMode.Additive));
            StateMachine.StartCoroutine(SetupView(scene));
        }

        private IEnumerator SetupView(Scene scene)
        {
            while (!scene.isLoaded) yield return null;
            view = Object.FindObjectOfType<InventoryView>(true);
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
            SceneManager.UnloadSceneAsync("Inventory", UnloadSceneOptions.None);
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
