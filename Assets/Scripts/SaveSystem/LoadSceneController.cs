using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using ViralVial.Utilities;

namespace ViralVial.SaveSystem
{
    public class LoadSceneController : MonoBehaviour
    {
        public LoadView LoadView;

        [Header("Buttons")]
        public SerializableDictionary<Button> SaveButtons;
        public Button LoadButton;
        public Button DeleteButton;
        public Button DeleteConfirmButton;
        public Button DeleteCancelButton;
        public Button BackButton;

        [Header("Data Panel")]
        public Text DateTimeText;
        public Text PlayerLevelText;
        public Text WaveCountText;
        public Text KillCountText;
        public Text TitleText;

        private SaveSlot selectedSaveSlot;
        private string selectedSaveSlotName;
        private bool inDeleteMode = false;
        private bool inSaveExamineMode = false;
        private string[] saveSlotNames;
        private Dictionary<string, SaveSlot> cachedSaves;

        private void Awake()
        {
            saveSlotNames = new string[] { Constants.AutoSaveSlotName, Constants.SaveSlot1Name, Constants.SaveSlot2Name, Constants.SaveSlot3Name };
            cachedSaves = new Dictionary<string, SaveSlot>();

            foreach (var saveSlotName in saveSlotNames)
            {
                if (SaveLoadSystem.SaveExists(saveSlotName))
                {
                    SaveButtons[saveSlotName].interactable = true;
                    cachedSaves.Add(saveSlotName, SaveLoadSystem.LoadSaveSlot(saveSlotName));
                }
                else SaveButtons[saveSlotName].interactable = false;
            }
        }

        public void SelectSaveSlot(string saveSlotName)
        {
            inSaveExamineMode = true;

            if (selectedSaveSlot != null) SaveButtons[selectedSaveSlotName].interactable = true;
            SaveButtons[saveSlotName].interactable = false;

            // update stats panel
            selectedSaveSlotName = saveSlotName;
            selectedSaveSlot = cachedSaves[saveSlotName];
            TitleText.text = SaveLoadSystem.GetNiceSaveSlotName(saveSlotName);
            DateTimeText.text = $"Time: {selectedSaveSlot.DateTime}";
            PlayerLevelText.text = $"Player Level: {selectedSaveSlot.Player.Level}";
            WaveCountText.text = $"Current Wave: {selectedSaveSlot.GameProgress.WavesFinished.ToString()}";
            KillCountText.text = $"Kill Count: {selectedSaveSlot.GameProgress.KillCount.ToString()}";

            LoadButton.interactable = true;
            DeleteButton.interactable = true;

            LoadButton.Select();
        }

        public void DeselectSaveSlot()
        {
            LoadButton.interactable = false;
            DeleteButton.interactable = false;

            UpdateSlotButtonsIfFileExists();

            selectedSaveSlotName = null;
            selectedSaveSlot = null;

            TitleText.text = "";
            DateTimeText.text = "";
            PlayerLevelText.text = "";
            WaveCountText.text = "";
            KillCountText.text = "";

            SelectFirstAvailableSaveSlotButton();

            inSaveExamineMode = false;
        }

        public void LoadSaveSlot()
        {
            LoadView.OnLoadSave.Invoke(selectedSaveSlot);
        }

        public void DeleteSaveSlot()
        {
            foreach (var button in SaveButtons)
                button.Value.interactable = false;

            DeleteButton.interactable = false;
            LoadButton.interactable = false;
            DeleteConfirmButton.interactable = true;
            DeleteCancelButton.interactable = true;

            inSaveExamineMode = false;
            inDeleteMode = true;

            DeleteConfirmButton.Select();
        }

        public void ConfirmDelete()
        {
            // delete file
            SaveLoadSystem.RemoveSave(selectedSaveSlotName);

            // update buttons
            SaveButtons[selectedSaveSlotName].interactable = false;

            LoadButton.interactable = false;
            DeleteButton.interactable = false;
            DeleteConfirmButton.interactable = false;
            DeleteCancelButton.interactable = false;

            UpdateSlotButtonsIfFileExists();
            DeselectSaveSlot();

            selectedSaveSlot = null;
            selectedSaveSlotName = null;

            inDeleteMode = false;
            inSaveExamineMode = false;
        }

        public void CancelDelete()
        {
            UpdateSlotButtonsIfFileExists();
            SaveButtons[selectedSaveSlotName].interactable = false;

            LoadButton.interactable = true;
            DeleteButton.interactable = true;
            DeleteConfirmButton.interactable = false;
            DeleteCancelButton.interactable = false;

            inDeleteMode = false;
            inSaveExamineMode = true;

            DeleteButton.Select();
        }

        public void OnPressBack(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.performed) return;
            OnPressBack();
        }

        public void OnPressBack()
        {
            if (inSaveExamineMode) DeselectSaveSlot();
            else if (inDeleteMode) CancelDelete();
            else LoadView.OnMainMenu.Invoke();
        }

        private void UpdateSlotButtonsIfFileExists()
        {
            foreach (var saveSlotName in saveSlotNames)
                SaveButtons[saveSlotName].interactable = SaveLoadSystem.SaveExists(saveSlotName);
        }

        private void SelectFirstAvailableSaveSlotButton()
        {
            Button button;
            if ((button = GetFirstSaveSlotButton()) != null) button.Select();
            else BackButton.Select();
        }

        private Button GetFirstSaveSlotButton()
        {
            foreach (var saveSlotName in saveSlotNames)
                if (SaveLoadSystem.SaveExists(saveSlotName)) return SaveButtons[saveSlotName];
            return null;
        }
    }
}