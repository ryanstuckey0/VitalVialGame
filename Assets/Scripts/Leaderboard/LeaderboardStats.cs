using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using ViralVial.SaveSystem;
using ViralVial.Utilities;

namespace ViralVial.Leaderboard
{
    public class LeaderboardStats
    {
        private static LeaderboardStats instance = null;
        private static string saveFile = Application.persistentDataPath + "/leaderboard.json";
        private static int amountSaved = 10;
        private int wavesStarted, killCount, wavesFinished;

        private LeaderboardStats()
        {
            wavesStarted = 0;
            wavesFinished = 0;
            killCount = 0;
            EventManager.Instance.SubscribeToEvent("UnloadGameContent", OnUnloadGameContent);
            EventManager.Instance.SubscribeToEvent("WaveFinished", OnFinishedWave);
            EventManager.Instance.SubscribeToEvent("InitWaveSpawner", OnInitWaveSpawner);
        }

        private void OnUnloadGameContent()
        {
            wavesStarted = 0;
            wavesFinished = 0;
            killCount = 0;
        }

        public static LeaderboardStats Instance
        {
            get
            {
                if (instance == null) instance = new LeaderboardStats();
                return instance;
            }
        }

        public static void OnInitWaveSpawner(Dictionary<string, object> args)
        {
            Instance.killCount = (int)args["kills"];
            Instance.wavesFinished = (int)args["wavesFinished"];
        }

        private void OnFinishedWave(Dictionary<string, object> args)
        {
            wavesFinished = (int)args["value"];
        }

        public void AddToKillCount(int amount)
        {
            killCount += amount;
        }

        public GameProgress GetGameProgress()
        {
            return new GameProgress
            {
                WavesFinished = wavesFinished,
                KillCount = killCount
            };
        }

        public int GetWaveCount()
        {
            return wavesFinished;
        }

        public int GetKillCount()
        {
            return killCount;
        }

        public void SaveLeaderboard(string initials)
        {
            var oldData = LoadLeaderboard();
            oldData.Add(new LeaderboardData(initials, wavesFinished, killCount));
            var newData = GetTop(oldData, amountSaved);
            File.WriteAllText(saveFile, JsonUtility.ToJson(new WrappingList(newData)));
        }

        public List<LeaderboardData> LoadLeaderboard()
        {
            if (!File.Exists(saveFile)) return new List<LeaderboardData>();

            return GetTop(JsonUtility.FromJson<WrappingList>(File.ReadAllText(saveFile)).list, amountSaved);
        }

        private List<LeaderboardData> GetTop(List<LeaderboardData> data, int amount)
        {
            return data.OrderByDescending(x => x.WaveCount).ThenByDescending(x => x.KillCount).ThenBy(x => x.Initials).Take(amount).ToList();
        }

        [System.Serializable]
        private class WrappingList
        {
            public WrappingList(List<LeaderboardData> list)
            {
                this.list = list;
            }

            public List<LeaderboardData> list;
        }

        [System.Serializable]
        public class LeaderboardData
        {
            public LeaderboardData(string initials, int waveCount, int killCount)
            {
                Initials = initials;
                WaveCount = waveCount;
                KillCount = killCount;
            }

            public string Initials;
            public int WaveCount;
            public int KillCount;
        }
    }
}
