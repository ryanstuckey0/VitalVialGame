using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ViralVial
{
    public class MainMenuState : BaseState
    {
        private MainMenuView view;

        public override void Setup()
        {
            base.Setup();

            //Load main menu scene and get view
            var scene = SceneManager.LoadScene("MainMenu", new LoadSceneParameters(LoadSceneMode.Additive));
            StateMachine.StartCoroutine(SetupView(scene));
        }

        private IEnumerator SetupView(Scene scene)
        {
            while (!scene.isLoaded) yield return null;
            view = Object.FindObjectOfType<MainMenuView>(true);
            view.OnGame += GoToGame;
            view.OnQuit += GoToQuit;
            view.OnControl += GoToControl;
            view.OnLoad += GoToLoad;
            view.OnOptions += GoToOptions;

            view.EventSystem?.gameObject.SetActive(true);

            SaveSystem.SaveLoadSystem.InitSaveLoadSystem();

            view.Show();
        }

        public override void Cleanup()
        {
            view.EventSystem?.gameObject.SetActive(false);
            view.Hide();
            view.OnGame -= GoToGame;
            view.OnQuit -= GoToQuit;
            view.OnControl -= GoToControl;
            view.OnLoad -= GoToLoad;
            view.OnOptions -= GoToOptions;
            //Unload scene
            SceneManager.UnloadSceneAsync("MainMenu", UnloadSceneOptions.None);
            view = null;

            base.Cleanup();
        }

        private void GoToGame()
        {
            StateMachine.ChangeState(new RunState(true));
        }

        public void GoToLoad()
        {
            StateMachine.ChangeState(new LoadState());
        }

        private void GoToControl()
        {
            StateMachine.ChangeState(new ControlMenuState());
        }

        private void GoToQuit()
        {
            StateMachine.ChangeState(new ExitState());
        }
        private void GoToOptions()
        {
            StateMachine.ChangeState(new OptionsState());
        }
    }
}
