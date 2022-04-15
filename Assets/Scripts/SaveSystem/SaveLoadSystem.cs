using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using ViralVial.Player;
using ViralVial.Utilities;
using ViralVial.Leaderboard;

namespace ViralVial.SaveSystem
{
    public class SaveLoadSystem
    {
        private static string AutoSaveFullPath => $"{Application.persistentDataPath}/{Constants.AutoSaveSlotName}.json";

        private static SaveLoadSystem instance;
        private bool inWave = false;

        private SaveLoadSystem()
        {
            EventManager.Instance.SubscribeToEvent("WaveStarted", OnStartedWave);
            EventManager.Instance.SubscribeToEvent("WaveFinished", OnFinishedWave);
        }

        public static void InitSaveLoadSystem()
        {
            instance = new SaveLoadSystem();
        }

        private void OnStartedWave(Dictionary<string, object> args)
        {
            inWave = true;
        }

        private void OnFinishedWave(Dictionary<string, object> args)
        {
            inWave = false;
        }

        // Dealing with AutoSave file -------------------------------------------------------------
        public static void AutoSave(IPlayer player)
        {
            Functions.SerializeAndWriteToFile(GetSaveSlot(player), AutoSaveFullPath);
        }

        public static bool AutoSaveExists()
        {
            return SaveExists(Constants.AutoSaveSlotName);
        }

        public static void RemoveAutoSave()
        {
            if (AutoSaveExists()) File.Delete(AutoSaveFullPath);
        }

        // Getting and saving to save slots -------------------------------------------------------

        public static void SaveSaveSlot(string saveSlotName, IPlayer player)
        {
            if (instance.inWave) File.Copy(AutoSaveFullPath, GetFullSavePath(saveSlotName));
            else Functions.SerializeAndWriteToFile(GetSaveSlot(player), GetFullSavePath(saveSlotName));
        }

        public static SaveSlot GetSaveSlot(IPlayer player)
        {
            return new SaveSlot
            {
                Player = player.RetrievePlayerInfo(),
                GameProgress = LeaderboardStats.Instance.GetGameProgress(),
                DateTime = DateTime.Now.ToString("MM/dd/yyyy, hh:mm tt"),
                BuyableDoorsProgress = BuyableDoorController.GetDoorProgress()
            };
        }

        public static SaveSlot LoadSaveSlot(string saveSlotName)
        {
            return Functions.ReadFileAndDeserialize<SaveSlot>(GetFullSavePath(saveSlotName));
        }

        public static void RemoveSave(string saveSlotName)
        {
            if (SaveExists(saveSlotName)) File.Delete(GetFullSavePath(saveSlotName));
        }

        // Utility Methods ------------------------------------------------------------------------
        public static bool SaveExists(string saveSlotName)
        {
            return File.Exists(GetFullSavePath(saveSlotName));
        }

        public static string GetNiceSaveSlotName(string saveSlotName)
        {
            switch (saveSlotName)
            {
                case Constants.AutoSaveSlotName: return "Auto Save";
                case Constants.SaveSlot1Name: return "Save Slot 1";
                case Constants.SaveSlot2Name: return "Save Slot 2";
                case Constants.SaveSlot3Name: return "Save Slot 3";
                default: return null;
            }
        }

        public static string GetFullSavePath(string saveSlotName)
        {
            return $"{Application.persistentDataPath}/{saveSlotName}.json";
        }

        public static Vector3Serializable ConvertToSerializableVector3(Vector3 position)
        {
            return new Vector3Serializable
            {
                x = position.x,
                y = position.y,
                z = position.z
            };
        }

        public static Vector3 ConvertFromSerializableVector3(Vector3Serializable position)
        {
            return new Vector3
            {
                x = position.x,
                y = position.y,
                z = position.z
            };
        }
    }

    public struct PlayerLoadout
    {
        public string[] HotbarAbilities;
        public InventoryItem Gun;
        public InventoryItem Melee;
        public InventoryItem Throwable;
    }

    public struct PlayerInventory
    {
        public Dictionary<InventoryItem, GunSavedState> InventoryGuns;
        public Dictionary<InventoryItem, MeleeSavedState> InventoryMelee;
        public Dictionary<InventoryItem, ThrowableSavedState> InventoryThrowables;
        public Dictionary<InventoryItem, AmmoSavedState> InventoryAmmo;
    }

    public struct MeleeSavedState
    {
        public bool Locked;
    }

    public struct GunSavedState
    {
        public int AmmoCount;
        public bool Locked;
    }

    public struct ThrowableSavedState
    {
        public int Count;
        public int MaxCapacity;
        public bool Locked;
    }

    public struct AmmoSavedState
    {
        public int Count;
        public int MaxCapacity;
        public bool Locked;
    }

    public class SaveSlot
    {
        public PlayerInfo Player;
        public GameProgress GameProgress;
        public string DateTime;
        public BuyableDoorsStatus BuyableDoorsProgress;
    }

    public struct GameProgress
    {
        public int WavesFinished;
        public int KillCount;
    }

    public struct Vector3Serializable
    {
        public float x;
        public float y;
        public float z;
    }

    public struct BuyableDoorsStatus
    {
        public List<bool> DoorStatusList;
    }


    public struct PlayerInfo
    {
        public float Health;
        public float currentMaxHealth;
        public float Exp;
        public int Level;
        public int SkillPoints;
        public Vector3Serializable position;

        public Dictionary<string, Dictionary<string, bool>> abilitiesProgress;
        public PlayerLoadout Loadout;
        public PlayerInventory Inventory;
    }
}
