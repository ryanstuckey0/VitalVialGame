using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ViralVial.Utilities;

namespace ViralVial.Leaderboard
{
    public class WaveGUI : MonoBehaviour
    {
        public Text waveText;
        public Image WaveIndicatorImage;
        public Sprite InWaveSprite;
        public Sprite BetweenWavesSprite;

        private void Awake()
        {
            waveText.text = $"Wave: 0";
        }

        private void OnEnable()
        {
            EventManager.Instance.SubscribeToEvent("WaveStarted", OnWaveStarted);
            EventManager.Instance.SubscribeToEvent("WaveFinished", OnWaveFinished);
            EventManager.Instance.SubscribeToEvent("InitWaveSpawner", OnInitWaveSpawner);
        }

        private void OnDisable()
        {
            EventManager.Instance.UnsubscribeFromEvent("WaveStarted", OnWaveStarted);
            EventManager.Instance.UnsubscribeFromEvent("WaveFinished", OnWaveFinished);
            EventManager.Instance.UnsubscribeFromEvent("InitWaveSpawner", OnInitWaveSpawner);
        }

        private void OnWaveStarted(Dictionary<string, object> args)
        {
            waveText.text = $"Wave: {(int)args["value"]}";
            WaveIndicatorImage.sprite = InWaveSprite;
        }

        private void OnWaveFinished(Dictionary<string, object> args)
        {
            WaveIndicatorImage.sprite = BetweenWavesSprite;
        }

        private void OnInitWaveSpawner(Dictionary<string, object> args)
        {
            waveText.text = $"Wave: {(int)args["wavesFinished"]}";
        }
    }
}
