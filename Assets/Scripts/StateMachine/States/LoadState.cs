using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ViralVial.SaveSystem;

namespace ViralVial
{
    public class LoadState : BaseState
    {
        private LoadView view;

        public override void Setup()
        {
            base.Setup();

            var scene = SceneManager.LoadScene("LoadScene", new LoadSceneParameters(LoadSceneMode.Additive));
            StateMachine.StartCoroutine(SetupView(scene));
        }

        private IEnumerator SetupView(Scene scene)
        {
            while (!scene.isLoaded) yield return null;
            view = Object.FindObjectOfType<LoadView>(true);
            view.OnMainMenu += GoToMainMenu;
            view.OnLoadSave += LoadSavedGame;
            view.EventSystem?.gameObject.SetActive(true);
            view.Show();
        }

        public override void Cleanup()
        {
            view.EventSystem?.gameObject.SetActive(false);
            
            view.Hide();
            view.OnMainMenu -= GoToMainMenu;
            view.OnLoadSave -= LoadSavedGame;
            //Unload scene
            SceneManager.UnloadSceneAsync("LoadScene", UnloadSceneOptions.None);
            view = null;

            base.Cleanup();
        }

        private void GoToMainMenu()
        {
            StateMachine.ChangeState(new MainMenuState());
        }

        private void LoadSavedGame(SaveSlot saveSlot)
        {
            StateMachine.ChangeState(new RunState(saveSlot));
        }
    }
}
