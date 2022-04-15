using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ViralVial.Sound;
using ViralVial.Video;

namespace ViralVial
{
    public class OptionsState : BaseState
    {
        private OptionsView view;

        public override void Setup()
        {
            base.Setup();

            //Load main menu scene and get view
            var scene = SceneManager.LoadScene("Options", new LoadSceneParameters(LoadSceneMode.Additive));
            StateMachine.StartCoroutine(SetupView(scene));
        }

        private IEnumerator SetupView(Scene scene)
        {
            while (!scene.isLoaded) yield return null;
            view = Object.FindObjectOfType<OptionsView>(true);
            view.OnMainMenu += GoToMainMenu;
            view.EventSystem?.gameObject.SetActive(true);
            view.Show();
        }

        public override void Cleanup()
        {
            AudioManager.SaveAudioSettingsToFile();
            VideoManager.SaveVideoSettingsToFile();
            view.EventSystem?.gameObject.SetActive(false);

            view.Hide();
            view.OnMainMenu -= GoToMainMenu;
            //Unload scene
            SceneManager.UnloadSceneAsync("Options", UnloadSceneOptions.None);
            view = null;

            base.Cleanup();
        }

        private void GoToMainMenu()
        {
            StateMachine.ChangeState(new MainMenuState());
        }
    }
}
