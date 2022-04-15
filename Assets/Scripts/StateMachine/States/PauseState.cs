using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ViralVial.Player;
using ViralVial.SaveSystem;

namespace ViralVial
{
    public class PauseState : BaseState
    {
        private RunState game;
        private PauseView view;

        public PauseState(RunState game)
        {
            this.game = game;
        }

        public override void Setup()
        {
            base.Setup();

            //Pause time for game
            Time.timeScale = 0f;

            //Load pause menu scene and get view
            var scene = SceneManager.LoadScene("PauseMenu", new LoadSceneParameters(LoadSceneMode.Additive));
            StateMachine.StartCoroutine(SetupView(scene));
        }

        private IEnumerator SetupView(Scene scene)
        {
            while (!scene.isLoaded) yield return null;

            view = Object.FindObjectOfType<PauseView>(true);

            view.OnResume += GoToRunState;
            view.OnSave += GoToSave;
            view.OnQuit += GoToQuitGame;
            view.OnMainMenu += GoToMainMenu;
            view.OnOptions += GoToOptions;

            view.EventSystem?.gameObject.SetActive(true);

            view.Show();
        }

        public override void Cleanup()
        {
            view.EventSystem?.gameObject.SetActive(false);

            //Cleanup view
            view.Hide();
            view.OnResume -= GoToRunState;
            view.OnSave -= GoToSave;
            view.OnQuit -= GoToQuitGame;
            view.OnMainMenu -= GoToMainMenu;
            view.OnOptions -= GoToOptions;

            //Unload scene
            SceneManager.UnloadSceneAsync("PauseMenu", UnloadSceneOptions.None);
            view = null;

            //Unpause time for game
            Time.timeScale = 1f;

            base.Cleanup();
        }

        private void GoToSave()
        {
            StateMachine.ChangeState(new SaveState(game));
        }

        private void GoToRunState()
        {
            game.LoadContent = false; //We do not want to reload content
            StateMachine.ChangeState(game);
        }

        private void GoToQuitGame()
        {
            StateMachine.ChangeState(new ExitState());
        }

        private void GoToMainMenu()
        {
            game.UnloadGameContent();
            StateMachine.ChangeState(new MainMenuState());
        }

        private void GoToOptions()
        {
            StateMachine.ChangeState(new PauseMenuOptionsState(this));
        }
    }
}
