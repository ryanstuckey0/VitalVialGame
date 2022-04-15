using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ViralVial.Player;
using ViralVial.Player.MonoBehaviourScript;
using ViralVial.SaveSystem;
using ViralVial.Utilities;

namespace ViralVial
{
    public class RunState : BaseState
    {
        public bool LoadContent = true;

        private RunView view;
        private IPlayer player;
        private SaveSlot saveSlot;
        private Scene previousActiveScene;

        public RunState(bool loadContent = true)
        {
            LoadContent = loadContent;
            SaveLoadSystem.RemoveAutoSave(); // remove since starting a new game
        }

        public RunState(SaveSlot saveSlot, bool loadContent = true)
        {
            LoadContent = loadContent;
            this.saveSlot = saveSlot;
            SaveLoadSystem.RemoveAutoSave(); // remove since starting a new game
        }

        public override void Setup()
        {
            base.Setup();

            if (LoadContent)
            {
                var scene = LoadGameContent();
                StateMachine.StartCoroutine(GetSceneObjects(scene));
            }
            else
            {
                SetupView();
            }

            EventManager.Instance.SubscribeToEvent("PlayerDeath", GoToGameLoss);
        }

        private IEnumerator GetSceneObjects(Scene scene)
        {
            yield return new WaitUntil(() => scene.isLoaded); //Wait for loading screen to load

            yield return null;

            //Start game loading
            var gameLevel = Object.FindObjectOfType<LoadingProgress>().StartSceneLoad("Workshop");
            yield return new WaitUntil(() => gameLevel.isDone);

            view = Object.FindObjectOfType<RunView>(true);
            SetupView();

            player = GameObject.FindGameObjectWithTag("Player").GetComponent<BasePlayerController>().OwningPlayer;

            previousActiveScene = SceneManager.GetActiveScene();
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Workshop"));

            if (saveSlot != null)
                LoadFromSave(saveSlot);
        }

        private void LoadFromSave(SaveSlot saveSlot)
        {
            view.BuyableDoorController.LoadDoorsProgress(saveSlot.BuyableDoorsProgress);
            view.WaveSpawner.LoadFromSaveFile(saveSlot.GameProgress);
            player.Initialize(saveSlot.Player);
        }

        private void SetupView()
        {
            view.OnGameWin += GoToGameWin;
            view.OnGameLoss += GoToGameLoss;
            view.OnTechTree += GoToTechTree;
            view.OnPause += GoToPause;
            view.OnInventory += GoToInventory;

            EventManager.Instance.InvokeEvent("CrosshairActive", new Dictionary<string, object>() { { "active", true } });
            EventManager.Instance.InvokeEvent("EnablePlayerInput");

            view.EventSystem?.gameObject.SetActive(true);

            AudioListener.pause = false;

            view.Show();
        }

        public override void Cleanup()
        {
            view.EventSystem?.gameObject.SetActive(false);

            //Cleanup view
            view.Hide();

            view.OnGameWin -= GoToGameWin;
            view.OnGameLoss -= GoToGameLoss;
            view.OnTechTree -= GoToTechTree;
            view.OnPause -= GoToPause;
            view.OnInventory -= GoToInventory;

            EventManager.Instance.InvokeEvent("CrosshairActive", new Dictionary<string, object>() { { "active", false } });
            EventManager.Instance.InvokeEvent("DisablePlayerInput");
            EventManager.Instance.UnsubscribeFromEvent("PlayerDeath", GoToGameLoss);

            AudioListener.pause = true;

            base.Cleanup();
        }

        private void GoToGameWin()
        {
            StateMachine.ChangeState(new WinState(this));
        }

        private void GoToGameLoss()
        {
            StateMachine.ChangeState(new LossState(this));
        }

        private void GoToTechTree()
        {
            StateMachine.ChangeState(new TechTreeState(this));
        }

        private void GoToPause()
        {
            StateMachine.ChangeState(new PauseState(this));
        }

        private void GoToInventory()
        {
            StateMachine.ChangeState(new InventoryState(this));
        }

        private Scene LoadGameContent()
        {
            return SceneManager.LoadScene("LoadingScreen", new LoadSceneParameters(LoadSceneMode.Additive));
        }

        public Scene ReloadGameContent()
        {
            return LoadGameContent();
        }

        public void UnloadGameContent()
        {
            view = null;
            EventManager.Instance.InvokeEvent("UnloadGameContent");
            SceneManager.SetActiveScene(previousActiveScene);
            SceneManager.UnloadSceneAsync("Workshop", UnloadSceneOptions.None);
        }
    }
}
