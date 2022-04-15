using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ViralVial
{
    public class LossState : BaseState
    {
        private RunState game;
        private LossView view;

        public LossState(RunState game = null)
        {
            this.game = game;
        }

        public override void Setup()
        {
            base.Setup();

            //Pause time for game
            Time.timeScale = 0f;

            //Load loss menu scene and get view
            var scene = SceneManager.LoadScene("LossMenu", new LoadSceneParameters(LoadSceneMode.Additive));
            StateMachine.StartCoroutine(SetupView(scene));
        }

        private IEnumerator SetupView(Scene scene)
        {
            while (!scene.isLoaded) yield return null;
            view = Object.FindObjectOfType<LossView>(true);
            view.OnTryAgain += GoToTryAgain;
            view.OnQuit += GoToQuitGame;
            view.OnMainMenu += GoToMainMenu;
            view.EventSystem?.gameObject.SetActive(true);
            view.Show();
        }

        public override void Cleanup()
        {
            view.EventSystem?.gameObject.SetActive(false);
            //Cleanup view
            view.Hide();
            view.OnTryAgain -= GoToTryAgain;
            view.OnQuit -= GoToQuitGame;
            view.OnMainMenu -= GoToMainMenu;

            //Unload scene
            SceneManager.UnloadSceneAsync("LossMenu", UnloadSceneOptions.None);
            view = null;

            //Unpause time for game
            Time.timeScale = 1f;

            base.Cleanup();
        }

        private void GoToTryAgain()
        {
            game.UnloadGameContent();
            StateMachine.ChangeState(new RunState(true));
        }

        private void GoToQuitGame()
        {
            // do we have to unload game content here- I think Unity will do this automatically when we quit game, change later if necessary
            // game.UnLoadGameContent(); 
            StateMachine.ChangeState(new ExitState());
        }

        private void GoToMainMenu()
        {
            game.UnloadGameContent();
            StateMachine.ChangeState(new MainMenuState());
        }
    }
}
