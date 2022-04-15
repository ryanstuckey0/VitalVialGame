using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ViralVial.Leaderboard
{
    public class LeaderboardGUI : MonoBehaviour
    {
        [SerializeField]
        private List<TMP_Text> ScoreTexts;

        void Start()
        {
            SetLeaderboardText();
        }

        private void SetLeaderboardText()
        {
            var leaderboardStats = LeaderboardStats.Instance.LoadLeaderboard();
            for (int i = 0; i < ScoreTexts.Count && i < leaderboardStats.Count; i++)
            {
                var data = leaderboardStats[i];
                ScoreTexts[i].text = data.Initials + " | Wave: " + data.WaveCount + " | Kills: " + data.KillCount;
            }
        }
    }
}
