using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using ViralVial.Player;
using ViralVial.Player.MonoBehaviourScript;
using ViralVial.Utilities;

namespace ViralVial.SaveSystem
{
    public class SaveSceneController : MonoBehaviour
    {
        public SaveView SaveView;

        [Header("Buttons")]
        public SerializableDictionary<Button> SaveSlotButtons;
        public SerializableDictionary<Text> SaveSlotText;
        public Button SaveButton;
        public Button OverwriteButton;
        public Button OverwriteConfirmButton;
        public Button OverwriteCancelButton;

        [Header("Data Panel")]
        public GameObject DataPanelSaveSlotTextTitle;
        public Text DataPanelSaveSlotText;
        public Text CurrentPlayerLevelText;
        public Text CurrentWaveCountText;
        public Text CurrentKillCountText;
        public Text SlotPlayerLevelText;
        public Text SlotWaveCountText;
        public Text SlotKillCountText;

        private Dictionary<string, SaveSlot> saveSlots = new Dictionary<string, SaveSlot>();
        private string selectedSaveSlotName;
        private SaveSlot selectedSaveSlot;

        private bool inSaveExamineMode = false;
        private bool inOverWriteMode = false;

        private IPlayer player;

        public void Start()
        {
            player = Object.FindObjectOfType<BasePlayerController>().OwningPlayer;

            if (SaveLoadSystem.AutoSaveExists()) saveSlots.Add("current", SaveLoadSystem.LoadSaveSlot(Constants.AutoSaveSlotName));
            else saveSlots.Add("current", SaveLoadSystem.GetSaveSlot(player));

            CurrentPlayerLevelText.text = $"Player Level: {saveSlots["current"].Player.Level.ToString()}";
            CurrentWaveCountText.text = $"Waves Completed: {saveSlots["current"].GameProgress.WavesFinished.ToString()}";
            CurrentKillCountText.text = $"Kill Count: {saveSlots["current"].GameProgress.KillCount.ToString()}";

            UpdateSlotButtonsIfFileExists();
        }

        private void UpdateSlotButtonsIfFileExists()
        {
            foreach (var saveSlotName in new string[] { Constants.SaveSlot1Name, Constants.SaveSlot2Name, Constants.SaveSlot3Name })
            {
                bool saveExists = SaveLoadSystem.SaveExists(saveSlotName);
                if (saveExists)
                    saveSlots.Add(saveSlotName, SaveLoadSystem.LoadSaveSlot(saveSlotName));
                SaveSlotText[saveSlotName].text = $"{SaveLoadSystem.GetNiceSaveSlotName(saveSlotName)}: {(saveExists ? saveSlots[saveSlotName].DateTime : "Empty")}";
            }
        }

        public void SelectSaveSlot(string saveSlotName)
        {
            if (selectedSaveSlotName != null) SaveSlotButtons[selectedSaveSlotName].interactable = true;
            SaveSlotButtons[saveSlotName].interactable = false;

            selectedSaveSlotName = saveSlotName;

            // case 1: no save file in slot
            if (!saveSlots.ContainsKey(saveSlotName))
            {
                OverwriteButton.interactable = false;
                SaveButton.interactable = true;

                SaveButton.Select();
            }
            // case 2: save file in slot
            else
            {
                OverwriteButton.interactable = true;
                SaveButton.interactable = false;

                DataPanelSaveSlotText.text = SaveLoadSystem.GetNiceSaveSlotName(saveSlotName);

                // update stats panel
                selectedSaveSlot = saveSlots[selectedSaveSlotName];

                // update stats for save slot
                SlotPlayerLevelText.text = $"Player Level: {selectedSaveSlot.Player.Level}";
                SlotWaveCountText.text = $"Waves Completed: {selectedSaveSlot.GameProgress.WavesFinished.ToString()}";
                SlotKillCountText.text = $"Kill Count: {selectedSaveSlot.GameProgress.KillCount.ToString()}";

                OverwriteButton.Select();
            }

            inOverWriteMode = false;
            inSaveExamineMode = true;
        }

        public void DeselectSaveSlot()
        {
            SaveSlotButtons[selectedSaveSlotName].interactable = true;

            OverwriteButton.interactable = false;
            SaveButton.interactable = false;

            DataPanelSaveSlotText.text = null;
            SlotPlayerLevelText.text = null;
            SlotWaveCountText.text = null;
            SlotKillCountText.text = null;

            SaveSlotButtons[selectedSaveSlotName].Select();

            selectedSaveSlotName = null;
            selectedSaveSlot = null;

            inSaveExamineMode = false;
        }

        public void SaveSaveSlot()
        {
            SaveLoadSystem.SaveSaveSlot(selectedSaveSlotName, player);
            SaveView.OnPause.Invoke();
        }

        public void OverwriteSaveSlot()
        {
            foreach (var button in SaveSlotButtons)
                button.Value.interactable = false;

            SaveButton.interactable = false;
            OverwriteButton.interactable = false;
            OverwriteConfirmButton.interactable = true;
            OverwriteCancelButton.interactable = true;

            inSaveExamineMode = false;
            inOverWriteMode = true;

            OverwriteConfirmButton.Select();
        }

        public void ConfirmOverwrite()
        {
            SaveLoadSystem.RemoveSave(selectedSaveSlotName);
            SaveSaveSlot();
        }

        public void CancelOverwrite()
        {
            foreach (var button in SaveSlotButtons)
            {
                if (button.Key == selectedSaveSlotName) continue;
                button.Value.interactable = true;
            }

            OverwriteButton.interactable = true;
            OverwriteConfirmButton.interactable = false;
            OverwriteCancelButton.interactable = false;

            inOverWriteMode = false;
            inSaveExamineMode = true;

            SaveButton.Select();
        }

        public void OnGoBack()
        {
            if (inSaveExamineMode) DeselectSaveSlot();
            else if (inOverWriteMode) CancelOverwrite();
            else SaveView.OnPause.Invoke();
        }

        public void OnGoBack(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.performed) return;
            OnGoBack();
        }
    }
}