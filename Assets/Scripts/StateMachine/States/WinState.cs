using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ViralVial
{
    public class WinState : BaseState
    {
        private RunState game;
        private WinView view;

        public WinState(RunState game = null)
        {
            this.game = game;
        }

        public override void Setup()
        {
            base.Setup();

            //Pause time for game
            Time.timeScale = 0f;

            //Load win menu scene and get view
            var scene = SceneManager.LoadScene("WinMenu", new LoadSceneParameters(LoadSceneMode.Additive));
            StateMachine.StartCoroutine(SetupView(scene));
        }

        private IEnumerator SetupView(Scene scene)
        {
            while (!scene.isLoaded) yield return null;
            view = Object.FindObjectOfType<WinView>(true);
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
            view.OnQuit -= GoToQuitGame;
            view.OnMainMenu -= GoToMainMenu;

            //Unload scene
            SceneManager.UnloadSceneAsync("WinMenu", UnloadSceneOptions.None);
            view = null;

            //Unpause time for game
            Time.timeScale = 1f;

            base.Cleanup();
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
