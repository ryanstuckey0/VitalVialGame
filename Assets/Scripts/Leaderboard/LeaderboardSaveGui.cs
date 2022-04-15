using UnityEngine;
using UnityEngine.UI;

namespace ViralVial.Leaderboard
{
    public class LeaderboardSaveGui : MonoBehaviour
    {
        [SerializeField]
        private InputField initials;
        [SerializeField]
        private Button saveButton;

        public void SaveNewScore()
        {
            LeaderboardStats.Instance.SaveLeaderboard(initials.text.Substring(0, 3));
            saveButton.interactable = false;
        }
    }
}
