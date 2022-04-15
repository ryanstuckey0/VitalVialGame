using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ViralVial
{
    public class LoadingProgress : MonoBehaviour
    {
        [SerializeField]
        private Image progressBar;

        public AsyncOperation StartSceneLoad(string name)
        {
            var gameLevel = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
            StartCoroutine(LoadSceneAsync(gameLevel));
            return gameLevel;
        }

        private IEnumerator LoadSceneAsync(AsyncOperation gameLevel)
        {
            while(!gameLevel.isDone)
            {
                progressBar.fillAmount = gameLevel.progress;
                //yield return new WaitForEndOfFrame();
                yield return null;
            }
            SceneManager.UnloadSceneAsync("LoadingScreen");
        }
    }
}
